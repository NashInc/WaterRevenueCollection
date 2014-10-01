using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using SimpleRevCollection.Management.Framework;
using SysWaterRev.BusinessLayer.Framework;
using SysWaterRev.BusinessLayer.Models;
using SysWaterRev.BusinessLayer.ViewModels;

namespace SysWaterRev.ManagementPortal.Controllers
{
    [Authorize]
    public class CustomersController : AbstractController
    {
        private readonly ApplicationDbContext db;
        private ApplicationUserManager userManager;

        public CustomersController()
        {
            db = new ApplicationDbContext();
        }

        public ApplicationUserManager UserManager
        {
            get { return userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>(); }
            set { userManager = value; }
        }

        // GET: Customers
        [HttpGet]
        public async Task<ActionResult> Index()
        {
            List<CustomerViewModel> customerViewModels = Map<List<Customer>, List<
                CustomerViewModel>>(await db.Customers.ToListAsync());
            return View(customerViewModels);
        }

        // GET: Customers/Details/5
        [HttpGet]
        public async Task<ActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CustomerViewModel customerViewModel = await ComputeTotalUnits(id);
            if (customerViewModel == null)
            {
                return HttpNotFound();
            }
            return View(customerViewModel);
        }

        //private async Task<CustomerViewModel> ComputeTotalUnitCosts(CustomerViewModel customerViewModel)
        //{
        //    var charge =
        //        await
        //            db.SystemSettings.Include(x => x.CurrentChargeSchedule)
        //                .OrderByDescending(x => x.DateCreated)
        //                .FirstOrDefaultAsync();
        //    if (charge != null)
        //    {
        //        charge.CurrentChargeSchedule.Charges.
        //    }
        //}

        private async Task<CustomerViewModel> ComputeTotalUnits(Guid? id)
        {
            CustomerViewModel customerViewModel;
            Customer customer =
                await
                    db.Customers.Include(x => x.Meters)
                        .Include(x => x.Meters.Select(z => z.MeterReadings))
                        .FirstOrDefaultAsync(y => y.CustomerId == id);
            if (customer != null)
            {
                customerViewModel = Map<Customer, CustomerViewModel>(customer);
                ICollection<Meter> metersForCustomer = customer.Meters;
                foreach (Meter meter in metersForCustomer)
                {
                    double totalReadings = 0.0d;
                    double totalConfirmedReadings = 0.0d;
                    double totalCorrectedReadings = 0.0d;
                    double totalCorrectedAndConfirmed = 0.0d;
                    ICollection<Reading> readingsForMeter = meter.MeterReadings;
                    foreach (Reading reading in readingsForMeter)
                    {
                        totalReadings += reading.ReadingValue;
                        if (reading.IsConfirmed != null && reading.IsConfirmed.Value)
                        {
                            totalConfirmedReadings += reading.ReadingValue;
                        }
                        if (reading.CorrectedBy != null)
                        {
                            totalCorrectedReadings += reading.CorrectionValue;
                        }
                        if (reading.IsConfirmed != null && (reading.CorrectedBy != null && reading.IsConfirmed.Value))
                        {
                            totalCorrectedAndConfirmed += reading.CorrectionValue;
                        }
                    }
                    customerViewModel.TotalReadings += totalReadings;
                    customerViewModel.TotalConfirmedReadings += totalConfirmedReadings;
                    customerViewModel.TotalCorrectedAndConfirmed += totalCorrectedAndConfirmed;
                    customerViewModel.TotalCorrectedReadings += totalCorrectedReadings;
                }
                return customerViewModel;
            }
            return null;
        }

        [HttpGet]
        public async Task<JsonResult> GetCascadeCustomers()
        {
            List<CustomerViewModel> customersViewModel = Map<List<Customer>, List<
                CustomerViewModel>>(await db.Customers.ToListAsync());
            return Json(customersViewModel, "application/json", JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetMetersForCustomer([DataSourceRequest] DataSourceRequest request, Guid? CustomerId)
        {
            DataSourceResult customerWithMeters = db.Meters.Where(x => x.CustomerId == CustomerId)
                .Select(x => new MeterViewModel
                {
                    MeterId = x.MeterId,
                    MeterNumber = x.MeterNumber,
                    MeterSerialNumber = x.MeterSerialNumber,
                    ReadingsForMeter = x.MeterReadings.Count,
                    DateCreated = x.DateCreated
                }).ToDataSourceResult(request);
            return Json(customerWithMeters, "application/json");
        }

        // GET: Customers/Create
        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Customers/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(
            [Bind(
                Include =
                    "FirstName,MiddleName,Surname,PhoneNumber,Identification,EmailAddress,UserGender,CustomerNumber")] CustomerViewModel customer)
        {
            var appUser = new ApplicationUser
            {
                IsActive = false,
                PhoneNumber = customer.PhoneNumber,
                UserName = customer.EmailAddress,
                Email = customer.EmailAddress,
                CustomerDetails = new Customer
                {
                    CreatedBy = User.Identity.Name,
                    CustomerId = IdentityGenerator.NewSequentialGuid(),
                    CustomerNumber = IdentityGenerator.GenerateCustomerNumber(),
                    DateCreated = DateTime.Now,
                    EmailAddress = customer.EmailAddress,
                    FirstName = customer.FirstName,
                    UserGender = customer.UserGender,
                    Surname = customer.Surname,
                    PhoneNumber = customer.PhoneNumber,
                    Identification = customer.Identification,
                    MiddleName = customer.MiddleName
                }
            };
            IdentityResult createResult = await UserManager.CreateAsync(appUser);
            if (createResult.Succeeded)
            {
                IdentityResult addToRoleResult =
                    await UserManager.AddToRoleAsync(appUser.Id, SysWaterRevRoles.Customers);
                if (addToRoleResult.Succeeded)
                {
                    TempData.Add("CustomerId", appUser.CustomerDetails.CustomerId);
                    return RedirectToAction("Index", "Customers");
                }
                ModelState.AddModelError("*", "Could Not Add Customer to Role");
                return View(customer);
            }
            ModelState.AddModelError("*", "Could Not Create Customer");
            return View(customer);
        }

        // GET: Customers/Edit/5
        [HttpGet]
        public async Task<ActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customer customer = await db.Customers.FindAsync(id);
            if (customer == null)
            {
                return HttpNotFound();
            }
            CustomerViewModel customerViewModel = Map<Customer, CustomerViewModel>(customer);
            return View(customerViewModel);
        }

        // POST: Customers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(
            [Bind(
                Include =
                    "CustomerId,FirstName,MiddleName,Surname,PhoneNumber,Identification,EmailAddress,UserGender,CustomerNumber"
                )] CustomerViewModel customer)
        {
            Customer customerModel = await db.Customers.FindAsync(customer.CustomerId);
            if (customerModel != null)
            {
                customerModel.FirstName = customer.FirstName;
                customerModel.MiddleName = customer.MiddleName;
                customerModel.Surname = customer.Surname;
                customerModel.PhoneNumber = customer.PhoneNumber;
                customerModel.Identification = customer.Identification;
                customerModel.EmailAddress = customer.EmailAddress;
                customerModel.UserGender = customer.UserGender;
                customerModel.CustomerNumber = customer.CustomerNumber;
                db.Entry(customerModel).State = EntityState.Modified;
                await db.SaveChangesAsync();
                TempData.Add("CustomerId", customerModel.CustomerId);
                return RedirectToAction("Index", "Customers");
            }
            ModelState.AddModelError("*", "Entity with the stated ID Could not be found!");
            return View(customer);
        }

        // GET: Customers/Delete/5
        public async Task<ActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customer customer = await db.Customers.FindAsync(id);
            if (customer == null)
            {
                return HttpNotFound();
            }
            return View(customer);
        }

        // POST: Customers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(Guid id)
        {
            Customer customer = await db.Customers.FindAsync(id);
            db.Customers.Remove(customer);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
                UserManager.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}