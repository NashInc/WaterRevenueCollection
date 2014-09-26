using System.Data.Entity;
using Microsoft.AspNet.Identity.EntityFramework;
using SysWaterRev.API.Models;
using SysWaterRev.BusinessLayer.Migrations;
using SysWaterRev.BusinessLayer.Models;
using Thinktecture.IdentityManager;

namespace SysWaterRev.API
{
    public class AspNetIdentityIdentityManagerFactory
    {
        static AspNetIdentityIdentityManagerFactory()
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<ApplicationDbContext, Configuration>());
        }

        public IIdentityManagerService Create()
        {
            var db = new ApplicationDbContext();
            var userStore = new UserStore<ApplicationUser>(db);
            var userMgr = new Microsoft.AspNet.Identity.UserManager<ApplicationUser>(userStore);
            var roleStore = new RoleStore<ApplicationRole>(db);
            var roleMgr = new Microsoft.AspNet.Identity.RoleManager<ApplicationRole>(roleStore);

            var svc = new Thinktecture.IdentityManager.AspNetIdentity.AspNetIdentityManagerService<ApplicationUser, string, ApplicationRole, string>(userMgr, roleMgr);

            return new DisposableIdentityManagerService(svc, db);
        }
    }
}