using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Aiplugs.CMS.Functions
{
  public class ExternalProcedure : IProcedure
  {
    public string DllPath { get; }
    public string TypeName { get; }
    public string MethodName { get; }

    private string _tempDir;

    public ExternalProcedure(string dllPath, string typeName, string methodName)
    {
      DllPath = dllPath;
      TypeName = typeName;
      MethodName = methodName;

      var dirName = Path.GetDirectoryName(DllPath).TrimStart('/','\\');
      _tempDir = Path.Combine(Path.GetTempPath(), "Aiplugs.CMS.Functions", dirName);
    }
    protected void CreateTempDirIfNotExists()
    {
      if (Directory.Exists(_tempDir) == false)
        Directory.CreateDirectory(_tempDir);
    }
    protected string GetTempFileName(string fileName)
    {
      return Path.Combine(_tempDir,fileName);
    }
    protected void CopyToTemp(IFile file, IStorageService _storage)
    {
      var binary = _storage.LoadFile(file);
      var path = GetTempFileName(file.Name);
      File.WriteAllBytes(path, binary);
      File.SetLastWriteTimeUtc(path, file.LastModifiedAt.UtcDateTime);
    }
    protected void CopyDlls(IStorageService _storage)
    {
      var dlls = GetDlls(_storage);
      var modifiedDlls = FilterModifiedDlls(dlls);
      foreach (var dll in modifiedDlls) {
        CopyToTemp(dll, _storage);
      }
    }
    protected IEnumerable<IFile> GetDlls(IStorageService _storage)
    {
      var dirName = Path.GetDirectoryName(DllPath).Replace(@"\","/");
      var folder = _storage.GetFolder(dirName);
      var dlls = folder.Files.Where(f => Path.GetExtension(f.Name) == ".dll" && f.Size > 0);
      return dlls;
    }
    protected IEnumerable<IFile> FilterModifiedDlls(IEnumerable<IFile> dlls)
    {
      return dlls.Where(dll => File.GetLastWriteTimeUtc(GetTempFileName(dll.Name)) < dll.LastModifiedAt.UtcDateTime);
    }
    public MethodInfo CreateMethod(Context context)
    {
      CreateTempDirIfNotExists();
      CopyDlls(context.StorageService);
      var asm = Assembly.LoadFile(GetTempFileName(Path.GetFileName(DllPath)));
      var type = asm.GetType(TypeName);
      var method = type.GetMethod(MethodName, BindingFlags.Public | BindingFlags.Static);

      return method;
    }
  }
}