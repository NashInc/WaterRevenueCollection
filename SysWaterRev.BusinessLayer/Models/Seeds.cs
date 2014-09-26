using System;
using System.Linq;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using SysWaterRev.BusinessLayer.Framework;

namespace SysWaterRev.BusinessLayer.Models
{
    public static class Seeds
    {
        public static void SeedRoles(ApplicationDbContext databaseContext)
        {
            var manager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(databaseContext));
            foreach (var role in from role in SimpleRevCollectionRoles.AllRoles let result = manager.RoleExists(role) where !result select role)
            {
                manager.Create(new ApplicationRole(role)
                {
                    Description = "Application Roles"
                });
            }
        }

        public static void SeedAdminUser(ApplicationDbContext db)
        {
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(db));
            const string name = "admin@example.com";
            const string password = "Admin@123456";
            //Create Role Admin if it does not exist
            var role = roleManager.FindByName(SimpleRevCollectionRoles.Administrators);
            if (role == null)
            {
                role = new ApplicationRole(SimpleRevCollectionRoles.Administrators);
                var roleresult = roleManager.Create(role);
            }
            var user = userManager.FindByName(name);
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
                var result = userManager.Create(user, password);
                result = userManager.SetLockoutEnabled(user.Id, false);
            }
            // Add user admin to Role Admin if not already added
            var rolesForUser = userManager.GetRoles(user.Id);
            if (!rolesForUser.Contains(role.Name))
            {
                var result = userManager.AddToRole(user.Id, SimpleRevCollectionRoles.Administrators);
            }
        }
    }
}