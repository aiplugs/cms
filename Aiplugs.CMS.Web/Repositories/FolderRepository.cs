using System;
using System.Collections.Generic;
using System.Linq;
using Aiplugs.CMS.Web.Data;
using Aiplugs.CMS.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace Aiplugs.CMS.Web.Repositories
{
  public class FolderRepository : IFolderRepository
  {
    public static long Home = -1;
    private readonly ApplicationDbContext db;
    public FolderRepository(ApplicationDbContext dbContext) {
      db = dbContext;
    }
    public void Add(IFolder parent, string name)
    {
      if (parent == null)
        throw new ArgumentNullException(nameof(parent));

      if (string.IsNullOrEmpty(name))
        throw new ArgumentNullException(nameof(name));

      var folder = (Folder)parent;
      db.Attach(folder).Entity.Children.Add(new Folder{ Name = name });
      db.SaveChanges();
    }

    public IFolder GetHome()
    {
      if (Home == -1) {
        Home = db.Folders.Where(f => f.ParentId == null).First().Id;
      }
      return Find(Home);
    }
    public IFolder Find(long id)
    {
      return db.Folders
        .Include(f => f.Children)
        .Include(f => f.Files)
        .Where(f => f.Id == id)
        .FirstOrDefault();
    }

    public IFolder Find(IEnumerable<string> path)
    {
      var folder = GetHome();
      var @enum = path.GetEnumerator();
      while (@enum.MoveNext())
      {
        folder = folder.Children.Where(f => f.Name == (string)@enum.Current).FirstOrDefault();
        if (folder == null)
        {
          return null;
        }
        folder = Find(((Models.Folder)folder).Id);
      }
      return folder;
    }

    public void Remove(IFolder folder)
    {
      var _folder = (Models.Folder)folder;
      if (_folder == null)
        throw new ArgumentNullException(nameof(folder));

      if (_folder.Parent == null)
        throw new ArgumentException("Cannot remove top level folder.");
      
      db.Folders.Remove(_folder);
      db.SaveChanges();
    }

    public void Rename(IFolder folder, string name)
    {
      var _folder = (Models.Folder)folder;
      if (_folder == null)
        throw new ArgumentNullException(nameof(folder));
      
      _folder.Name = name;
      db.SaveChanges();
    }
  }
}