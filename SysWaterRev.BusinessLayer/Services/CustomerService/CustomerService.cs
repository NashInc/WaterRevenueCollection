using System;
using System.Data.Entity.Validation;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using SysWaterRev.BusinessLayer.Framework;
using SysWaterRev.BusinessLayer.Models;
using SysWaterRev.BusinessLayer.ViewModels;

namespace SysWaterRev.BusinessLayer.Services.CustomerService
{
    public class CustomerService : IDisposable
    {
        private readonly UserManager<ApplicationUser> userManager;

        public CustomerService(UserManager<ApplicationUser> userManager)
        {
            this.userManager = userManager;
        }

        private ApplicationUser createApplicationUser(CreateCustomerViewModel customerViewModel)
        {
            var appUser = new ApplicationUser
            {
                UserName = customerViewModel.EmailAddress,
                Email = customerViewModel.EmailAddress,
                IsActive = true,
                PhoneNumber = customerViewModel.PhoneNumber,
                CustomerDetails = new Customer
                {
                    CreatedBy = customerViewModel.CreatedBy,
                    DateCreated = DateTime.Now,
                    EmailAddress = customerViewModel.EmailAddress,
                    CustomerId = IdentityGenerator.NewSequentialGuid(),
                    CustomerNumber = IdentityGenerator.GenerateCustomerNumber(),
                    FirstName = customerViewModel.FirstName,
                    PhoneNumber = customerViewModel.PhoneNumber,
                    Identification = customerViewModel.Identification,
                    MiddleName = customerViewModel.MiddleName,
                    Surname = customerViewModel.Surname,
                    UserGender = customerViewModel.UserGender,
                    CustomerAccount = new Account
                    {
                        AccountNumber = IdentityGenerator.GenerateAccountNumber(),
                        CreatedBy = customerViewModel.CreatedBy,
                        DateCreated = DateTime.Now,
                        AccountId = IdentityGenerator.NewSequentialGuid(),

                    }
                }
            };
            return appUser;
        }

        private IdentityResult createAccountTaskAsync(ApplicationUser appUser)
        {
            var createResult = userManager.Create(appUser);
            return createResult;
        }

        private IdentityResult addUserToRoleTaskAsync(ApplicationUser appUser)
        {
            var addToRoleResult = userManager.AddToRole(appUser.Id, SysWaterRevRoles.Customers);
            return addToRoleResult;
        }

        public CreateCustomerResult CreateCustomerTaskAsync(CreateCustomerRequest request)
        {
            var createCustomerResult = new CreateCustomerResult();
            var appUser = createApplicationUser(request.CustomerViewModel);
            var createAccountResult = createAccountTaskAsync(appUser);
            if (createAccountResult.Succeeded)
            {
                var addToRoleResult = addUserToRoleTaskAsync(appUser);
                if (addToRoleResult.Succeeded)
                {
                    createCustomerResult.ApplicationUser = appUser;
                    createCustomerResult.IsSuccess = true;
                    return createCustomerResult;
                }
                createCustomerResult.Exception = new Exception(addToRoleResult.Errors.First());
                createCustomerResult.IsSuccess = false;
            }
            createCustomerResult.Exception = new Exception(createAccountResult.Errors.First());
            createCustomerResult.IsSuccess = false;
            return createCustomerResult;
        }
        public void Dispose()
        {
            userManager.Dispose();
        }
    }
}