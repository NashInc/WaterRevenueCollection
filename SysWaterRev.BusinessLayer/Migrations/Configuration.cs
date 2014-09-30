using System.Data.Entity.Migrations;
using SysWaterRev.BusinessLayer.Models;

namespace SysWaterRev.BusinessLayer.Migrations
{
    public sealed class Configuration : DbMigrationsConfiguration<ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(ApplicationDbContext context)
        {
            Seeds.SeedRoles(context);
            Seeds.SeedAdminUser(context);
            base.Seed(context);
        }
    }
}