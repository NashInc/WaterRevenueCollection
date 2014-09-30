using System;
using System.Collections.Generic;
using System.Data.Entity;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using SysWaterRev.BusinessLayer.Framework;

namespace SysWaterRev.BusinessLayer.Models
{
    public class ApplicationDbContextInitializer : DropCreateDatabaseIfModelChanges<ApplicationDbContext>
    {
        protected override void Seed(ApplicationDbContext context)
        {
            SeedRoles(context);
            SeedAdminUser(context);
            base.Seed(context);
        }

        private void SeedRoles(ApplicationDbContext databaseContext)
        {
            var manager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(databaseContext));
            foreach (string role in SimpleRevCollectionRoles.AllRoles)
            {
                bool result = manager.RoleExists(role);
                if (!result)
                {
                    manager.Create(new ApplicationRole(role)
                    {
                        Description = "Application Roles"
                    });
                }
            }
        }

        //Create User=Admin@Admin.com with password=Admin@123456 in the Admin role
        public static void SeedAdminUser(ApplicationDbContext db)
        {
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(db));
            const string name = "admin@example.com";
            const string password = "Admin@123456";
            //Create Role Admin if it does not exist
            IdentityRole role = roleManager.FindByName(SimpleRevCollectionRoles.Administrators);
            if (role == null)
            {
                role = new ApplicationRole(SimpleRevCollectionRoles.Administrators);
                IdentityResult roleresult = roleManager.Create(role);
            }
            ApplicationUser user = userManager.FindByName(name);
            if (user == null)
            {
                user = new ApplicationUser
                {
                    UserName = name,
                    Email = name,
                    PhoneNumber = "+254710773556",
                    IsActive = true,
                    EmployeeDetails = new Employee
                    {
                        CreatedBy = "Auto",
                        DateCreated = DateTime.Now,
                        EmailAddress = "mbuthiagrg@gmail.com",
                        EmployeeGender = Gender.Male,
                        EmployeeNumber = "ADMIN123",
                        FirstName = "George",
                        Identification = "28350053",
                        Surname = "Ndungu",
                        MiddleName = "Mbuthia",
                        PhoneNumber = "+254710773556",
                        EmployeeId = IdentityGenerator.NewSequentialGuid()
                    }
                };
                IdentityResult result = userManager.Create(user, password);
                result = userManager.SetLockoutEnabled(user.Id, false);
            }
            // Add user admin to Role Admin if not already added
            IList<string> rolesForUser = userManager.GetRoles(user.Id);
            if (!rolesForUser.Contains(role.Name))
            {
                IdentityResult result = userManager.AddToRole(user.Id, SimpleRevCollectionRoles.Administrators);
            }
        }
    }
}