using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using SysWaterRev.BusinessLayer.Models;
using SysWaterRev.BusinessLayer.Services.CustomerService;
using SysWaterRev.BusinessLayer.ViewModels;
using SysWaterRev.ManagementPortal.Framework;

namespace SysWaterRev.ManagementPortal.Controllers
{
    [Authorize]
    public class CustomersController : AbstractController
    {
        private readonly ApplicationDbContext db;

        public CustomersController()
        {
            db = new ApplicationDbContext();
        }

        // GET: Customers
        [HttpGet]
        public async Task<ActionResult> Index()
        {
            var customerViewModels = Map<List<Customer>, List<
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
            var customer = await db.Customers.SingleOrDefaultAsync(x => x.CustomerId == id);
            if (customer == null)
            {
                return HttpNotFound();
            }
            var customerViewModel = Map<Customer, CustomerViewModel>(customer);
            return View(customerViewModel);
        }       
        private async Task<CustomerViewModel> ComputeTotalUnits(Guid? id)
        {
            CustomerViewModel customerViewModel;
            var customer =
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
            var customersViewModel = Map<List<Customer>, List<
                CustomerViewModel>>(await db.Customers.ToListAsync());
            return Json(customersViewModel, "application/json", JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ChildActionOnly]
        public JsonResult GetMetersForCustomer([DataSourceRequest] DataSourceRequest request, Guid? CustomerId)
        {
            var customerWithMeters = db.Meters.Where(x => x.CustomerId == CustomerId)
                .Select(x => new MeterViewModel
                {
                    MeterId = x.MeterId,
                    MeterNumber = x.MeterNumber,
                    MeterSerialNumber = x.MeterSerialNumber,
                    ReadingsForMeter = x.MeterReadings.Count,
                    DateCreated = x.DateCreated
                }).ToDataSourceResult(request);
            return Json(customerWithMeters);
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
        public async Task<ActionResult> Create(CreateCustomerViewModel customer)
        {
            if (ModelState.IsValid)
            {
                using (var customerService = new CustomerService(UserManager))
                {
                    customer.CreatedBy = User.Identity.Name;
                    var createCustomerRequest = new CreateCustomerRequest(customer);
                    var result = customerService.CreateCustomerTaskAsync(createCustomerRequest);
                    if (result.IsSuccess)
                    {
                        await SendEmailConfirmationTokenAsync(result.ApplicationUser.Id, "Confirm Your Account");
                        TempData.Clear();
                        TempData.Add("CustomerId", result.ApplicationUser.CustomerDetails.CustomerId);
                        return RedirectToAction("Index", "Customers");
                    }
                    ModelState.AddModelError("", result.Exception.Message);
                    return View(customer);
                }
            }
            ModelState.AddModelError("","Please Correct the Highlighted Errors!");
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
            var appUser =
                await
                    db.Users.Include(x => x.CustomerDetails)
                        .SingleOrDefaultAsync(x => x.CustomerDetails.CustomerId == id);
            if (appUser == null)
            {
                return HttpNotFound();
            }
            var customerViewModel = Map<Customer, CustomerViewModel>(appUser.CustomerDetails);
            return View(customerViewModel);
        }

        // POST: Customers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "CustomerId,FirstName,MiddleName,Surname,PhoneNumber,Identification,EmailAddress,UserGender,CustomerNumber")]CustomerViewModel customer)
        {
            var appUser = await db.Users.Include(x => x.CustomerDetails).SingleOrDefaultAsync(x => x.CustomerDetails.CustomerId == customer.CustomerId);
            if (appUser != null)
            {
                appUser.CustomerDetails.FirstName = customer.FirstName;
                appUser.CustomerDetails.MiddleName = customer.MiddleName;
                appUser.CustomerDetails.Surname = customer.Surname;
                appUser.CustomerDetails.PhoneNumber = customer.PhoneNumber;
                appUser.CustomerDetails.Identification = customer.Identification;
                appUser.CustomerDetails.EmailAddress = customer.EmailAddress;
                appUser.CustomerDetails.UserGender = customer.UserGender;
                appUser.CustomerDetails.CustomerNumber = customer.CustomerNumber;
                appUser.CustomerDetails.LastEditDate = DateTime.Now;
                appUser.CustomerDetails.LastEditedBy = User.Identity.Name;
                db.Entry(appUser).State = EntityState.Modified;
                await db.SaveChangesAsync();
                TempData.Clear();
                TempData.Add("CustomerId", appUser.CustomerDetails.CustomerId);
                await SendEmailConfirmationTokenAsync(appUser.Id, "Please Confirm Your Account Edit");
                return RedirectToAction("Index", "Customers");
            }
            ModelState.AddModelError("", "Entity with the stated ID Could not be found!");
            return View(customer);
        }

        // GET: Customers/Delete/5
        public async Task<ActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var customer = await db.Customers.FindAsync(id);
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
            var customer = await db.Customers.FindAsync(id);
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