using Domain.ProjectHierarchy;
using Microsoft.EntityFrameworkCore;

namespace Persistence
{
    public class DataContext : DbContext
    {
        public DbSet<Project> Projects { get; set; }
        public DbSet<ProjectVersion> ProjectVersions { get; set; } = null!;
        public DbSet<ContentFile> ContentFiles { get; set; } = null!;

        public DataContext()
        {

        }
        public DataContext(DbContextOptions options) : base(options) 
        {
            //Database.EnsureDeleted();
            Database.EnsureCreated();
            //Database.Migrate();
        }

        ////protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        ////{
        ////    optionsBuilder.UseSqlite(@"Data Source=..\Storage\PrimoPrjsdb.db");
        ////}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<ProjectTree>().Ignore(c => c.Content);
            base.OnModelCreating(modelBuilder);
        }
    }
}
