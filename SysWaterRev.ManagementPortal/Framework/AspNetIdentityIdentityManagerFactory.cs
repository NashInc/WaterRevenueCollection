using System.Data.Entity;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using SysWaterRev.BusinessLayer.Migrations;
using SysWaterRev.BusinessLayer.Models;
using Thinktecture.IdentityManager;
using Thinktecture.IdentityManager.AspNetIdentity;

namespace SysWaterRev.ManagementPortal.Framework
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
            var userMgr = new UserManager<ApplicationUser>(userStore);
            var roleStore = new RoleStore<ApplicationRole>(db);
            var roleMgr = new RoleManager<ApplicationRole>(roleStore);

            var svc = new AspNetIdentityManagerService<ApplicationUser, string, ApplicationRole, string>(userMgr,
                roleMgr);

            return new DisposableIdentityManagerService(svc, db);

            //var db = new CustomDbContext("CustomAspId");
            //var store = new CustomUserStore(db);
            //var mgr = new CustomUserManager(store);
            //return new Thinktecture.IdentityManager.AspNetIdentity.UserManager<CustomUser, int, CustomUserLogin, CustomUserRole, CustomUserClaim>(mgr, db);
        }
    }
}