using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity.Owin;
using SysWaterRev.BusinessLayer.Framework;
using SysWaterRev.BusinessLayer.Models;
using SysWaterRev.BusinessLayer.ViewModels;
using SysWaterRev.ManagementPortal.Framework;

namespace SysWaterRev.ManagementPortal.Controllers
{
    [Authorize]
    public class ReadingsController : AbstractController
    {
        private readonly ApplicationDbContext db;
        public ReadingsController()
        {
            db = new ApplicationDbContext();
        }
        // GET: Readings

        [HttpGet]
        public async Task<ActionResult> Index()
        {
            var readings = db.Readings.Include(r => r.ReadBy);
            var readingsViewModel =
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
            var reading = await db.Readings.Include(x => x.ReadBy).FirstOrDefaultAsync(x => x.ReadingId == id);

            if (reading == null)
            {
                return HttpNotFound();
            }
            var readingsViewModel = Map<Reading, ReadingViewModel>(reading);
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
                TempData.Clear();
                TempData.Add("ReadingId", reading.ReadingId);
                return RedirectToAction("Details", "Readings", new { id = reading.ReadingId });
            }
            //TempData.Add("ReadingId", ReadingId);
            return RedirectToAction("Details", "Readings", new { id = ReadingId });
        }

        [HttpPost]
        public async Task<ActionResult> GetCustomersWithMeters()
        {
            var customersViewModel =
                Map<List<Customer>, List<CustomerViewModel>>(await db.Customers.Include(x => x.Meters).ToListAsync());

            return Json(customersViewModel, "application/json", JsonRequestBehavior.AllowGet);
        }
        // GET: Readings/Correction/5
        public async Task<ActionResult> Correction(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var reading = await db.Readings.Include(x => x.ReadBy).FirstOrDefaultAsync(x => x.ReadingId == id);
            if (reading == null)
            {
                return HttpNotFound();
            }
            var readingViewModel = Map<Reading, ReadingViewModel>(reading);
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
            var readingModel = await db.Readings.FindAsync(reading.ReadingId);
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
                        TempData.Clear();
                        TempData.Add("ReadingId", reading.ReadingId);
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
        [Authorize(Roles = SysWaterRevRoles.Administrators)]
        public async Task<ActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var reading = await db.Readings.FindAsync(id);
            if (reading == null)
            {
                return HttpNotFound();
            }
            return View(reading);
        }

        [Authorize(Roles = SysWaterRevRoles.Administrators)]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(Guid id)
        {
            var reading = await db.Readings.FindAsync(id);
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