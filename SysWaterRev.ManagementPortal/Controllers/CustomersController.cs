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
        private readonly CustomerService customerService;

        public CustomersController()
        {
            db = new ApplicationDbContext();
            customerService = new CustomerService(UserManager, db);
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
            var customerResult = await customerService.GetCustomer(new GetCustomerRequest { CustomerId = id });
            if (!customerResult.IsSuccess)
            {
                return HttpNotFound();
            }
            var customerViewModel = Map<Customer, CustomerViewModel>(customerResult.ApplicationUser.CustomerDetails);
            return View(customerViewModel);
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
            ModelState.AddModelError("", "Please Correct the Highlighted Errors!");
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
            var appUser = await customerService.GetCustomer(new GetCustomerRequest { CustomerId = id });
            if (appUser.IsSuccess == false)
            {
                return HttpNotFound(appUser.Exception.Message);
            }
            var customerViewModel = Map<Customer, CreateCustomerViewModel>(appUser.ApplicationUser.CustomerDetails);
            return View(customerViewModel);
        }

        // POST: Customers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "CustomerId,FirstName,MiddleName,Surname,PhoneNumber,Identification,EmailAddress,UserGender")]CreateCustomerViewModel customer)
        {
            var updateCustomerResponse = new UpdateCustomerResponse();
            if (ModelState.IsValid)
            {
                updateCustomerResponse = await customerService.UpdateCustomerTaskAsync(new UpdateCustomerRequest{CreateCustomerViewModel = customer});
                if (updateCustomerResponse.IsSuccess)
                {
                    TempData.Clear();
                    TempData.Add("CustomerId", updateCustomerResponse.ApplicationUser.CustomerDetails.CustomerId);
                    return RedirectToAction("Index", "Customers");
                }
                ModelState.AddModelError("", updateCustomerResponse.Exception.Message);
                return View(customer);
            }
            ModelState.AddModelError("", "Please Correct the highlighted Errors");
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