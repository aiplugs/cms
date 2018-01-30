using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Aiplugs.CMS.Web.Models;

namespace Aiplugs.CMS.Web.Data
{
  public class ApplicationDbContext : IdentityDbContext<ApplicationUser,ApplicationRole, string>
  {
    public DbSet<File> Files { get; set; }
    public DbSet<Folder> Folders { get; set; }
    public DbSet<Record> Records { get; set; }
    public DbSet<Item> Items { get; set; }
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
      base.OnModelCreating(builder);

      builder.Entity<Folder>().HasMany(e => (IEnumerable<Folder>)e.Children);
      builder.Entity<Folder>().HasMany(e => (IEnumerable<File>)e.Files).WithOne(e => e.Folder);
      builder.Entity<Folder>().HasOne(e => e.Parent);
      builder.Entity<Item>().HasMany(e => e.History).WithOne(e => e.Item);
      builder.Entity<Item>().HasOne(e => e.Current);
    }
  }
}
