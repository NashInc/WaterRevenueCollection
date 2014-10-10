using System.Data.Entity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace SysWaterRev.BusinessLayer.Models
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        //static ApplicationDbContext()
        //{
        //    Database.SetInitializer(new MigrateDatabaseToLatestVersion<ApplicationDbContext, Configuration>());
        //}

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
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<InvoiceLineItem> InvoiceLineItems { get; set; }
        public DbSet<InvoiceMessage> InvoiceMessages { get; set; }
        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Customer>()
                .HasKey(k => k.CustomerId)
                .HasRequired(t => t.CustomerAccount)
                .WithRequiredPrincipal(t => t.OwnerCustomer);
            modelBuilder.Entity<Account>()
                .HasKey(k => k.AccountId)
                .HasRequired(t => t.OwnerCustomer)
                .WithRequiredDependent(t => t.CustomerAccount);
            base.OnModelCreating(modelBuilder);
        }
    }
}