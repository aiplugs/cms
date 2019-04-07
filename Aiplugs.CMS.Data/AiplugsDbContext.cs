using Aiplugs.CMS.Data.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Aiplugs.CMS.Data
{
    public enum DbKind
    {
        Sqlite, SqlServer, CosmosDb
    }
    public class AiplugsDbContext : IdentityDbContext<User, Role, string>
    {
        private DbKind DbKind;
        public DbSet<File> Files { get; set; }
        public DbSet<Folder> Folders { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<ItemRecord> Records { get; set; }
        public DbSet<Job> Jobs { get; set; }

        public AiplugsDbContext(DbContextOptions options, DbKind kind = DbKind.Sqlite) : base(options)
        {
            DbKind = kind;
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=aiplugs.db");
            base.OnConfiguring(optionsBuilder);
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            if (DbKind == DbKind.Sqlite)
            {
                SetupSqlite(builder);
            }

            var file = builder.Entity<File>();
            file.HasIndex(o => new { o.FolderId, o.Name }).IsUnique();

            var folder = builder.Entity<Folder>();
            folder.HasIndex(o => o.Path).IsUnique();
            folder.HasData(new Folder { Id = Helper.CreateKey(), Path = "/", });

            base.OnModelCreating(builder);
        }

        

        void SetupSqlite(ModelBuilder builder)
        {
            var item = builder.Entity<Item>();
            item.Property(o =>  o.Data).HasColumnType("JSON");

            var record = builder.Entity<ItemRecord>();
            record.Property(o => o.Data).HasColumnType("JSON");

            var file = builder.Entity<File>();
            file.Property(o => o.BinaryPath).HasColumnType("VARCHAR");

            var folder = builder.Entity<Folder>();
            folder.Property(o => o.Path).HasColumnType("VARCHAR");

            var job = builder.Entity<Job>();
            job.Property(o => o.Log).HasColumnType("VARCHAR");
            job.Property(o => o.Parameters).HasColumnType("JSON");
        }
    }
}
