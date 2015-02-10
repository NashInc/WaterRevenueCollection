using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using SysWaterRev.BusinessLayer.Framework;
using SysWaterRev.BusinessLayer.Models;
using SysWaterRev.BusinessLayer.ViewModels;

namespace SysWaterRev.BusinessLayer.Services.CustomerService
{
    public class CustomerService
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly ApplicationDbContext db;

        public CustomerService(UserManager<ApplicationUser> userManager)
        {
            this.userManager = userManager;
        }

        public CustomerService(ApplicationDbContext context)
        {
            db = context;
        }

        public CustomerService(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            this.userManager = userManager;
            db = context;
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

        public async Task<GetCustomerResponse> GetCustomer(GetCustomerRequest request)
        {
            var customerResponse = new GetCustomerResponse();
            var customerViewModel =
                await db.Users.SingleOrDefaultAsync(x => x.CustomerDetails.CustomerId == request.CustomerId);
            if (customerViewModel != null)
            {
                customerResponse.ApplicationUser = customerViewModel;
                customerResponse.IsSuccess = true;
                return customerResponse;
            }
            customerResponse.Exception = new Exception("Customer with the stated ID Could not be found!");
            customerResponse.IsSuccess = false;
            return customerResponse;
        }

        public async Task<UpdateCustomerResponse> UpdateCustomerTaskAsync(UpdateCustomerRequest request)
        {
            var updateCustomerResponse = new UpdateCustomerResponse();
            var appUser =
                   await GetCustomer(new GetCustomerRequest { CustomerId = request.CreateCustomerViewModel.CustomerId });
            if (appUser.IsSuccess)
            {
                try
                {
                    appUser.ApplicationUser.CustomerDetails.FirstName = request.CreateCustomerViewModel.FirstName;
                    appUser.ApplicationUser.CustomerDetails.MiddleName = request.CreateCustomerViewModel.MiddleName;
                    appUser.ApplicationUser.CustomerDetails.Surname = request.CreateCustomerViewModel.Surname;
                    appUser.ApplicationUser.CustomerDetails.PhoneNumber = request.CreateCustomerViewModel.PhoneNumber;
                    appUser.ApplicationUser.CustomerDetails.Identification = request.CreateCustomerViewModel.Identification;
                    appUser.ApplicationUser.CustomerDetails.EmailAddress = request.CreateCustomerViewModel.EmailAddress;
                    appUser.ApplicationUser.CustomerDetails.UserGender = request.CreateCustomerViewModel.UserGender;
                    appUser.ApplicationUser.CustomerDetails.LastEditDate = DateTime.Now;
                    appUser.ApplicationUser.CustomerDetails.LastEditedBy = request.CreateCustomerViewModel.CreatedBy;
                    db.Entry(appUser).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                    updateCustomerResponse.ApplicationUser = appUser.ApplicationUser;
                    updateCustomerResponse.IsSuccess = true;
                    updateCustomerResponse.Exception = null;
                    return updateCustomerResponse;
                }
                catch (Exception ex)
                {
                    updateCustomerResponse.Exception = ex;
                    updateCustomerResponse.IsSuccess = false;
                    return updateCustomerResponse;
                }
            }
            updateCustomerResponse.Exception = new Exception("Entity Could not be found!");
            updateCustomerResponse.IsSuccess = false;
            return updateCustomerResponse;
        }
    }
}