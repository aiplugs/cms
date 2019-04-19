using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Aiplugs.CMS.Core.Models;
using SysFile = System.IO.File;

namespace Aiplugs.CMS.Core.Services
{
    public class ProcedureSerivce : IProcedureService
    {
        const string BUILTIN = "Builtin";
        const string CUSTOM = "Custom";
        private readonly ISettingsService _settings;
        private readonly IStorageService _storage;
        private readonly IJobService _jobs;
        private string[] _builtins = null;
        public ProcedureSerivce(ISettingsService settingsService, IStorageService storageService, IJobService jobService)
        {
            _settings = settingsService;
            _storage = storageService;
            _jobs =  jobService;
        }

        public Type[] GetBuiltinProcedures()
        {
            var procedure = typeof(IProcedure);
            return Assembly.GetExecutingAssembly()
                            .GetTypes()
                            .Where(type => type.GetInterfaces().Contains(procedure))
                            .ToArray();
        }
        public string[] GetBuiltinProcedureNames()
        {
            if (_builtins == null)
            {
                var procedure = typeof(IProcedure);
                _builtins = GetBuiltinProcedures().Select(type => type.Name).ToArray();
            }

            return _builtins;
        }

        public async Task<string> RegisterAsync(string collectionName, string procedureName, IContextParameters parameters)
        {
            if (collectionName.Contains(':'))
                throw new ArgumentException(nameof(collectionName), "Cannot use delimiter(:)");

            if (procedureName.Contains(':'))
                throw new ArgumentException(nameof(procedureName), "Cannot use delimiter(:)");

            if (GetBuiltinProcedureNames().Contains(procedureName))
                return await _jobs.ExclusiveCreateAsync($"{BUILTIN}:{collectionName}:{procedureName}", parameters);

            var collection = await _settings.FindCollectionAsync(collectionName);
            var info = collection.Procedures?.Where(procedure => procedure.Name == procedureName).FirstOrDefault();

            if (info != null)
                return await _jobs.ExclusiveCreateAsync($"{CUSTOM}:{collectionName}:{procedureName}", parameters);

            throw new NotSupportedException($"{procedureName} is not supported.");
        }
        
        public IProcedure Resolve(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException();

            var splited = name.Split(':').ToArray();
            if (splited.Length != 3)
                throw new ArgumentException(nameof(name));
            
            if (splited[0] == CUSTOM)
                return ResolveCustom(splited[1], splited[2]).Result;

            else if (splited[0] == BUILTIN)
                return ResolveBuiltin(splited[2]);
            
            return null;
        }
        public IProcedure ResolveBuiltin(string procedureName)
        {
            var type = GetBuiltinProcedures().First(procedure => procedure.Name == procedureName);
            return (IProcedure)Activator.CreateInstance(type);
        }
        public async Task<IProcedure> ResolveCustom(string collectionName, string procedureName)
        {
            var collection = await _settings.FindCollectionAsync(collectionName);
            var procedure = collection.Procedures.Where(p => p.Name == procedureName).FirstOrDefault();
            if (procedure == null)
                return null;

            var path = (procedure.DllPath??"").Split('/').Where(s => string.IsNullOrEmpty(s) == false).ToArray();
            if (path.Length <= 1)
                return null;
            var head = path.Take(path.Length - 1).ToArray();
            var tail = path.Last();

            var dir = CreateTempDirectory(head);
            var files = await _storage.GetFilesAsync(string.Join("/",head));

            foreach(var file in files)
            {
                await CopyIfNeed(file, dir);
            }

            var asm = Assembly.LoadFrom(Path.Combine(dir, tail));
            var type = Type.GetType($"{procedure.TypeName}, {asm.FullName}");

            return (IProcedure)Activator.CreateInstance(type);
        }
        public async Task CopyIfNeed(IFile file, string dir)
        {
            var path = Path.Combine(dir, file.Name);
            if (NeedCopy(file, path))
            {
                using(var dst = SysFile.OpenWrite(Path.Combine(path)))
                using(var src = _storage.OpenFile(file))
                {
                    await src.CopyToAsync(dst);
                }
            }
        }
        public bool NeedCopy(IFile file, string path)
        {
            if (SysFile.Exists(path) == false)
                return true;
            
            var info = new FileInfo(path);
            if (file.LastModifiedAt > info.LastWriteTimeUtc)
                return true;
            
            return false;
        }
        public string CreateTempDirectory(string[] path)
        {
            path = new[] { "AiplugsCmsBin" }.Concat(path).ToArray();
            var dir = Path.GetTempPath();            
            for(var i = 0; i < path.Length; i++)
            {
                dir = Path.Combine(dir, path[i]);
                if (Directory.Exists(dir) == false)
                    Directory.CreateDirectory(dir);
            }
            return dir;
        }
    }
}