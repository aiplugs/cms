using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Aiplugs.CMS.Core.Models;
using Aiplugs.Functions.Core;

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

        public async Task<long?> RegisterBuiltinProcedureAsync(string collectionName, string procedureName, IContextParameters parameters)
        {
            if (collectionName.Contains(':'))
                throw new ArgumentException(nameof(collectionName), "Cannot use delimiter(:)");
            
            if (procedureName.Contains(':'))
                throw new ArgumentException(nameof(procedureName), "Cannot use delimiter(:)");

            if (GetBuiltinProcedureNames().Contains(procedureName) == false)
                throw new ArgumentOutOfRangeException(nameof(procedureName), "Invalid builtin procedure");

            return await _jobs.ExclusiveCreateAsync($"{BUILTIN}:{collectionName}:{procedureName}", parameters);
        }

        public async Task<long?> RegisterCustomProcedureAsync(string collectionName, ProcedureInfo procedure, IContextParameters parameters)
        {
            if (collectionName.Contains(':'))
                throw new ArgumentException(nameof(collectionName), "Cannot use delimiter(:)");
            
            if (procedure.Name.Contains(':'))
                throw new ArgumentException(nameof(procedure), "Cannot use delimiter(:) to procedure name");

            return await _jobs.ExclusiveCreateAsync($"{CUSTOM}:{collectionName}:{procedure.Name}", parameters);
        }
        public Aiplugs.Functions.Core.IProcedure Resolve(string name)
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
        public Aiplugs.Functions.Core.IProcedure ResolveBuiltin(string procedureName)
        {
            return new BuiltinProcedure(GetBuiltinProcedures().First(procedure => procedure.Name == procedureName));
        }
        public async Task<Aiplugs.Functions.Core.IProcedure> ResolveCustom(string collectionName, string procedureName)
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

            return new CustomProcedure(Path.Combine(dir, tail), procedure.Method);
        }
        public async Task CopyIfNeed(IFile file, string dir)
        {
            var path = Path.Combine(dir, file.Name);
            if (NeedCopy(file, path))
            {
                using(var dst = System.IO.File.OpenWrite(Path.Combine(path)))
                using(var src = _storage.OpenFile(file))
                {
                    await src.CopyToAsync(dst);
                }
            }
        }
        public bool NeedCopy(IFile file, string path)
        {
            if (System.IO.File.Exists(path) == false)
                return true;
            
            var info = new FileInfo(path);
            if (file.LastModifiedAt > info.LastWriteTimeUtc)
                return true;
            
            return false;
        }
        public string CreateTempDirectory(string[] path)
        {
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