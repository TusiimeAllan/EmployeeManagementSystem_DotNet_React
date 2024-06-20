using EmployeeManagementAPI.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagementAPI.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<Salary> Salaries { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Salary>(entity =>
            {
                entity.Property(e => e.BaseSalary)
                    .HasColumnType("decimal(18,2)");

                entity.Property(e => e.Bonus)
                    .HasColumnType("decimal(18,2)");
            });

            modelBuilder.Entity<ApplicationRole>(entity =>
            {
                entity.Property(r => r.ConcurrencyStamp).HasColumnType("varchar(255)").IsRequired();
            });
        }
    }

}
