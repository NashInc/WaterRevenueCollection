using SysWaterRev.BusinessLayer.Models;

namespace SysWaterRev.BusinessLayer.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    public sealed class Configuration : DbMigrationsConfiguration<SysWaterRev.BusinessLayer.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(SysWaterRev.BusinessLayer.Models.ApplicationDbContext context)
        {
            Seeds.SeedRoles(context);
            Seeds.SeedAdminUser(context);
            base.Seed(context);
        }
    }
}
