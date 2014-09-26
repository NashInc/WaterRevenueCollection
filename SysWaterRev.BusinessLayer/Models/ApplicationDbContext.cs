using System.Data.Entity;
using Microsoft.AspNet.Identity.EntityFramework;
using SysWaterRev.BusinessLayer.Migrations;

namespace SysWaterRev.BusinessLayer.Models
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("Name=DefaultConnection", false)
        {
        }

        public DbSet<Customer> Customers { get; set; }

        public DbSet<Employee> Employees { get; set; }

        public DbSet<Reading> Readings { get; set; }

        public DbSet<Meter> Meters { get; set; }

        public DbSet<Charge> Charges { get; set; }

        public DbSet<SystemSetting> SystemSettings { get; set; }

        public DbSet<ChargeSchedule> ChargeSchedules { get; set; }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        static ApplicationDbContext()
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<ApplicationDbContext, Configuration>());
        }
    }
}