using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using SysWaterRev.BusinessLayer.Framework;
using SysWaterRev.BusinessLayer.Models;
using SysWaterRev.BusinessLayer.ViewModels;
using SysWaterRev.ManagementPortal.Framework;

namespace SysWaterRev.ManagementPortal.Controllers
{
    [Authorize]
    public class MetersController : AbstractController
    {
        private readonly ApplicationDbContext db;

        public MetersController()
        {
            db = new ApplicationDbContext();
        }

        // GET: Meters
        [HttpGet]
        public async Task<ActionResult> Index()
        {
            List<MeterViewModel> metersViewModel =
                Map<List<Meter>, List<MeterViewModel>>(
                    await db.Meters.Include(m => m.OwnerCustomer).Include(m => m.MeterReadings).ToListAsync());
            return View(metersViewModel);
        }

        // GET: Meters/Details/5
        [HttpGet]
        public async Task<ActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var meter = await db.Meters.FindAsync(id);

            if (meter == null)
            {
                return HttpNotFound();
            }
            var meterViewModel = Map<Meter, MeterViewModel>(meter);
            return View(meterViewModel);
        }
        [HttpPost]
        public JsonResult GetReadingsForMeter([DataSourceRequest] DataSourceRequest request, Guid? meterId)
        {
            var readingsWithEmployee = db.Readings.Where(x => x.MeterId == meterId)
                .Select(x => new ReadingViewModel
                {
                    MeterId = x.MeterId,
                    Latitude = x.Latitude,
                    Longitude = x.Longitude,
                    ReadingId = x.ReadingId,
                    IsConfirmed = x.IsConfirmed,
                    ReadingValue = x.ReadingValue,
                    DateCreated = x.DateCreated,
                    CreatedBy = x.CreatedBy,
                    EmployeeId = x.EmployeeId,
                    EmployeeNumber = x.ReadBy.EmployeeNumber,
                    EmployeeFirstName = x.ReadBy.FirstName,
                    EmployeeMiddleName = x.ReadBy.MiddleName,
                    EmployeeSurname = x.ReadBy.Surname,
                }).ToDataSourceResult(request);
            return Json(readingsWithEmployee);
        }

        public async Task<ActionResult> GetCascadeMeters(Guid? CustomerId)
        {
            if (CustomerId == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var meterViewModel =
                Map<List<Meter>, List<MeterViewModel>>(
                    await db.Meters.Where(x => x.CustomerId == CustomerId).ToListAsync());
            return Json(meterViewModel, JsonRequestBehavior.AllowGet);
        }

        // GET: Meters/Create
        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Meters/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "MeterSerialNumber,MeterNumber,CustomerId")] MeterViewModel meter)
        {
            var customer = await db.Customers.FindAsync(meter.CustomerId);
            if (customer != null)
            {
                try
                {
                    meter.CustomerId = customer.CustomerId;
                    meter.CreatedBy = User.Identity.Name;
                    meter.DateCreated = DateTime.Now;
                    meter.MeterId = IdentityGenerator.NewSequentialGuid();
                    var meterModel = Map<MeterViewModel, Meter>(meter);
                    db.Meters.Add(meterModel);
                    await db.SaveChangesAsync();
                    TempData.Clear();
                    TempData.Add("MeterId", meterModel.MeterId);
                    return RedirectToAction("Index", "Meters");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("*", ex);
                    return View(meter);
                }
            }
            return HttpNotFound("Record with the stated ID could not be found!");
        }

        // GET: Meters/Edit/5
        [HttpGet]
        public async Task<ActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var meter = await db.Meters.FindAsync(id);           
            if (meter == null)
            {
                return HttpNotFound();
            }
            var meterViewModel = Map<Meter, MeterViewModel>(meter);
            return View(meterViewModel);
        }

        // POST: Meters/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "MeterId,MeterSerialNumber,MeterNumber,CustomerId")] MeterViewModel meter)
        {
            var existingMeter = await db.Meters.FindAsync(meter.MeterId);
            if (existingMeter != null)
            {
                var existingCustomer = await db.Customers.FindAsync(meter.CustomerId);
                if (existingCustomer != null)
                {
                    existingMeter.CustomerId = existingCustomer.CustomerId;
                    existingMeter.LastEditDate = DateTime.Now;
                    existingMeter.LastEditedBy = User.Identity.Name;
                    existingMeter.MeterSerialNumber = meter.MeterSerialNumber;
                    existingMeter.MeterNumber = meter.MeterNumber;
                    try
                    {
                        db.Entry(existingMeter).State = EntityState.Modified;
                        await db.SaveChangesAsync();
                        TempData.Add("CustomerId", existingMeter.CustomerId);
                        return RedirectToAction("Index");
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError("*", ex);
                        return View(meter);
                    }
                }
                return HttpNotFound();
            }
            return HttpNotFound();
        }

        // GET: Meters/Delete/5
        public async Task<ActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var meter = await db.Meters.FindAsync(id);
          
            if (meter == null)
            {
                return HttpNotFound();
            }
            var meterViewModel = Map<Meter, MeterViewModel>(meter);
            return View(meterViewModel);
        }

        // POST: Meters/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(Guid id)
        {
            var meter = await db.Meters.FindAsync(id);
            db.Meters.Remove(meter);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}