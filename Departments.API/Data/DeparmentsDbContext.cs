using Departments.API._Models.Domain;
using Microsoft.EntityFrameworkCore;
using static System.Net.Mime.MediaTypeNames;

namespace Departments.API.Data
{
    public class DeparmentsDbContext:DbContext
    {
        public DeparmentsDbContext(DbContextOptions dbContextOptions) :base(dbContextOptions)
        {
            
        }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Reminder> Reminders { get; set; }

        public DbSet<Logo> Logos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

     
            modelBuilder.Entity<Department>()
                .HasOne(d => d.ParentDepartment)
                .WithMany()
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
