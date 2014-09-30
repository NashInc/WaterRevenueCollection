using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity.Owin;
using SimpleRevCollection.Management.Framework;
using SysWaterRev.BusinessLayer.Framework;
using SysWaterRev.BusinessLayer.Models;
using SysWaterRev.BusinessLayer.ViewModels;

namespace SysWaterRev.ManagementPortal.Controllers
{
    [Authorize]
    public class ReadingsController : AbstractController
    {
        private readonly ApplicationDbContext db;
        private ApplicationUserManager userManager;

        public ReadingsController()
        {
            db = new ApplicationDbContext();
        }

        public ApplicationUserManager UserManager
        {
            get { return userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>(); }
            private set { userManager = value; }
        }

        // GET: Readings

        [HttpGet]
        public async Task<ActionResult> Index()
        {
            IQueryable<Reading> readings = db.Readings.Include(r => r.ReadBy);
            List<ReadingViewModel> readingsViewModel =
                Map<List<Reading>, List<ReadingViewModel>>(await readings.ToListAsync());
            return View(readingsViewModel);
        }

        // GET: Readings/Details/5
        [HttpGet]
        public async Task<ActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Reading reading = await db.Readings.Include(x => x.ReadBy).FirstOrDefaultAsync(x => x.ReadingId == id);

            if (reading == null)
            {
                return HttpNotFound();
            }
            ReadingViewModel readingsViewModel = Map<Reading, ReadingViewModel>(reading);
            return View(readingsViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ConfirmReading(Guid? ReadingId)
        {
            var reading = await db.Readings.FindAsync(ReadingId);
            if (reading != null)
            {
                reading.IsConfirmed = true;
                reading.ConfirmedBy = User.Identity.Name;
                reading.LastEditDate = DateTime.Now;
                reading.LastEditedBy = User.Identity.Name;
                db.Entry(reading).State = EntityState.Modified;
                await db.SaveChangesAsync();
               // TempData.Add("ReadingId", reading.ReadingId);
                return RedirectToAction("Details", "Readings", new {id = reading.ReadingId});
            }
            //TempData.Add("ReadingId", ReadingId);
            return RedirectToAction("Details", "Readings", new {id = ReadingId});
        }

        [HttpPost]
        public async Task<ActionResult> GetCustomersWithMeters()
        {
            List<CustomerViewModel> customersViewModel =
                Map<List<Customer>, List<CustomerViewModel>>(await db.Customers.Include(x => x.Meters).ToListAsync());

            return Json(customersViewModel, "application/json", JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(
            [Bind(Include = "Latitude,Longitude,ReadingValue,EmployeeId,CustomerId,MeterId")] ReadingViewModel reading)
        {
            var employee = new Employee();
            if (User.IsInRole(SimpleRevCollectionRoles.Administrators))
            {
                employee = await db.Employees.FirstOrDefaultAsync(z => z.EmployeeId == reading.EmployeeId);
            }
            else
            {
                employee = (await UserManager.FindByNameAsync(User.Identity.Name)).EmployeeDetails;
            }
            if (employee != null)
            {
                Customer customer =
                    await
                        db.Customers.FirstOrDefaultAsync(z => z.CustomerId == reading.CustomerId);
                if (customer != null)
                {
                    Meter meter =
                        await
                            db.Meters.Include(x => x.MeterId).FirstOrDefaultAsync(z => z.MeterId == reading.MeterId);
                    if (meter != null)
                    {
                        var readingModel = new Reading
                        {
                            ReadingId = IdentityGenerator.NewSequentialGuid(),
                            Latitude = reading.Latitude,
                            Longitude = reading.Longitude,
                            DateCreated = DateTime.Now,
                            CreatedBy = User.Identity.Name,
                            ReadingValue = reading.ReadingValue,
                            ReadBy = employee,
                            EmployeeId = employee.EmployeeId,
                            MeterId = meter.MeterId,
                            MeterRead = meter
                        };
                        db.Readings.Add(readingModel);
                        await db.SaveChangesAsync();
                        //TempData.Add("ReadingId", readingModel.ReadingId);
                        return RedirectToAction("Index", "Readings");
                    }
                    return HttpNotFound();
                }
                return HttpNotFound();
            }
            return HttpNotFound();
        }

        [HttpPost]
        public async Task<ActionResult> CreateMobile(
            [Bind(Include = "Latitude,Longitude,ReadingValue,MeterId,Accuracy,Altitude,Speed,Heading,AltitudeAccuracy")] ReadingViewModel reading)
        {
            var employee = new Employee();
            if (User.IsInRole(SimpleRevCollectionRoles.Administrators))
            {
                employee = await db.Employees.FirstOrDefaultAsync(z => z.EmployeeId == reading.EmployeeId);
            }
            else
            {
                employee = (await UserManager.FindByNameAsync(User.Identity.Name)).EmployeeDetails;
            }
            if (employee != null)
            {
                Customer customer =
                    await
                        db.Customers.FirstOrDefaultAsync(z => z.CustomerId == reading.CustomerId);
                if (customer != null)
                {
                    Meter meter =
                        await
                            db.Meters.Include(x => x.MeterId).FirstOrDefaultAsync(z => z.MeterId == reading.MeterId);
                    if (meter != null)
                    {
                        // var lastReading = await db.Readings.OrderByDescending(x => x.CreatedBy).FirstOrDefaultAsync();
                        var readingModel = new Reading
                        {
                            ReadingId = IdentityGenerator.NewSequentialGuid(),
                            Latitude = reading.Latitude,
                            Longitude = reading.Longitude,
                            DateCreated = DateTime.Now,
                            CreatedBy = User.Identity.Name,
                            ReadingValue = reading.ReadingValue,
                            ReadBy = employee,
                            EmployeeId = employee.EmployeeId,
                            MeterId = meter.MeterId,
                            MeterRead = meter,
                            Accuracy = reading.Accuracy,
                            Altitude = reading.Altitude,
                            AltitudeAccuracy = reading.AltitudeAccuracy,
                            Speed = reading.Speed,
                            Heading = reading.Heading,
                            LocationDateTime = reading.LocationDateTime,
                        };
                        db.Readings.Add(readingModel);
                        await db.SaveChangesAsync();
                        return Json(readingModel, "applicaion/json");
                    }
                    return HttpNotFound();
                }
                return HttpNotFound();
            }
            return HttpNotFound();
        }

        // GET: Readings/Correction/5
        public async Task<ActionResult> Correction(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Reading reading = await db.Readings.Include(x => x.ReadBy).FirstOrDefaultAsync(x => x.ReadingId == id);
            if (reading == null)
            {
                return HttpNotFound();
            }
            ReadingViewModel readingViewModel = Map<Reading, ReadingViewModel>(reading);
            return View(readingViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Correction(
            [Bind(
                Include =
                    "ReadingId,ReadingValue,CorrectionValue"
                )] ReadingViewModel reading)
        {
            Reading readingModel = await db.Readings.FindAsync(reading.ReadingId);
            if (readingModel != null)
            {
                if (readingModel.ReadingValue == reading.ReadingValue)
                {
                    if (readingModel.ReadingValue != reading.CorrectionValue)
                    {
                        readingModel.CorrectedBy = User.Identity.Name;
                        readingModel.CorrectionValue = reading.CorrectionValue;
                        readingModel.LastEditDate = DateTime.Now;
                        readingModel.LastEditedBy = User.Identity.Name;
                        db.Entry(readingModel).State = EntityState.Modified;
                        await db.SaveChangesAsync();
                       //TempData.Add("ReadingId", reading.ReadingId);
                        return RedirectToAction("Index", "Readings");
                    }
                    ViewBag.Error = "The Correction Value must not be equal to the reading value";
                    return View(reading);
                }
                ViewBag.Error = "Invalid Form Data!";
                return View(reading);
            }
            return View(reading);
        }

        // GET: Readings/Delete/5
        [Authorize(Roles = SimpleRevCollectionRoles.Administrators)]
        public async Task<ActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Reading reading = await db.Readings.FindAsync(id);
            if (reading == null)
            {
                return HttpNotFound();
            }
            return View(reading);
        }

        [Authorize(Roles = SimpleRevCollectionRoles.Administrators)]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(Guid id)
        {
            Reading reading = await db.Readings.FindAsync(id);
            db.Readings.Remove(reading);
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