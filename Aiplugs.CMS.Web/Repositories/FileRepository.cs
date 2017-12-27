using System;
using System.Collections.Generic;
using Aiplugs.CMS.Web.Data;
using Aiplugs.CMS.Web.Models;

namespace Aiplugs.CMS.Web.Repositories
{
  public class FileRepository : IFileRepository
  {
    private readonly ApplicationDbContext db;
    public FileRepository(ApplicationDbContext dbContext) {
      db = dbContext;
    }
    public void Add(IFolder parent, string name, string contentType, byte[] binary)
    {
      parent.Files.Add(new File
      {
        Name = name,
        ContentType = contentType,
        Binary = binary,
        Size = binary.Length,
      });
      db.SaveChanges();
    }

    public IFile Find(long id)
    {
      return db.Files.Find(id);
    }

    public void Remove(IFile file)
    {
      var _file = (Models.File)file;
      if (_file == null) 
        new ArgumentNullException(nameof(file));
      
      db.Files.Remove(_file);
      db.SaveChanges();
    }
    public void Replace(IFile file, string name, string contentType, byte[] binary)
    {
       var _file = (Models.File)file;
      if (_file == null) 
        new ArgumentNullException(nameof(file));
      
      _file.Name = name;
      _file.ContentType = contentType;
      _file.Binary = binary;
      _file.Size = binary.Length;

      db.SaveChanges();
    }
  }
}