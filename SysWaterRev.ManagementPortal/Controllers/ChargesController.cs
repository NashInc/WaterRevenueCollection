using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using SimpleRevCollection.Management.Framework;
using SysWaterRev.BusinessLayer.Framework;
using SysWaterRev.BusinessLayer.Models;
using SysWaterRev.BusinessLayer.ViewModels;

namespace SysWaterRev.ManagementPortal.Controllers
{
    [Authorize]
    public class ChargesController : AbstractController
    {
        private readonly ApplicationDbContext db;

        public ChargesController()
        {
            db = new ApplicationDbContext();
        }

        // GET: Charges
        public async Task<ActionResult> Index()
        {
            var charges = db.Charges.Include(c => c.ChargeSchedule);
            var chargesVm = Map<List<Charge>, List<ChargeViewModel>>(await charges.ToListAsync());
            return View(chargesVm);
        }

        // GET: Charges/Details/5
        public async Task<ActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var charge = await db.Charges.FindAsync(id);            
            if (charge == null)
            {
                return HttpNotFound();
            }
            var chargeViewModel = Map<Charge, ChargeViewModel>(charge);
            return View(chargeViewModel);
        }

        // GET: Charges/Create
        public ActionResult Create()
        {
            ViewBag.ChargeScheduleId = new SelectList(db.ChargeSchedules, "ChargeScheduleId", "ChargeScheduleName");
            return View();
        }

        // POST: Charges/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(
            [Bind(Include = "StartRange,EndRange,ChargeScheduleId,UnitPrice")] ChargeViewModel chargeVm)
        {
            var chargeSchedule = await db.ChargeSchedules.FindAsync(chargeVm.ChargeScheduleId);
            if (chargeSchedule != null)
            {
                try
                {
                    var charge = new Charge
                    {
                        DateCreated = DateTime.Now,
                        StartRange = chargeVm.StartRange,
                        EndRange = chargeVm.EndRange,
                        CreatedBy = User.Identity.Name,
                        ChargeId = IdentityGenerator.NewSequentialGuid(),
                        ChargeScheduleId = chargeSchedule.ChargeScheduleId,
                        UnitPrice = decimal.Parse(chargeVm.UnitPrice)
                    };
                    db.Charges.Add(charge);
                    await db.SaveChangesAsync();
                    TempData.Add("ChargeId", charge.ChargeId);
                    return RedirectToAction("Index", "Charges");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("*", ex);
                    return View(chargeVm);
                }
            }
            return View(chargeVm);
        }

        // GET: Charges/Edit/5
        public async Task<ActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var charge = await db.Charges.FindAsync(id);
            if (charge == null)
            {
                return HttpNotFound();
            }
            var chargeViewModel = Map<Charge, ChargeViewModel>(charge);
            return View(chargeViewModel);
        }

        // POST: Charges/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(
            [Bind(Include = "ChargeId,StartRange,EndRange,ChargeScheduleId")] ChargeViewModel chargeVmModel)
        {
            var charge = await db.Charges.FindAsync(chargeVmModel.ChargeId);
            if (charge != null)
            {
                try
                {
                    charge.ChargeScheduleId = chargeVmModel.ChargeScheduleId;
                    charge.EndRange = chargeVmModel.EndRange;
                    charge.StartRange = chargeVmModel.StartRange;
                    charge.LastEditDate = DateTime.Now;
                    charge.LastEditedBy = User.Identity.Name;
                    db.Entry(charge).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                    TempData.Add("ChargeId", charge.ChargeId);
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("*", ex);
                    return View(chargeVmModel);
                }
            }
            ViewBag.ChargeScheduleId = new SelectList(db.ChargeSchedules, "ChargeScheduleId", "ChargeScheduleName",
                charge.ChargeScheduleId);
            return View(chargeVmModel);
        }

        [HttpPost]
        public async Task<JsonResult> ChargeStartRangeValidation(string StartRange, Guid? ChargeScheduleId)
        {
            int startRange;
            var startResult = int.TryParse(StartRange, out startRange);
            if (startResult && (ChargeScheduleId != null))
            {
                var previousCharge =
                   await db.Charges.Where(x => x.ChargeScheduleId == ChargeScheduleId)
                        .OrderByDescending(x => x.DateCreated)
                        .FirstOrDefaultAsync();
                if (previousCharge != null)
                {
                    if (startRange > previousCharge.EndRange)
                    {
                        return Json(true, "application/json");
                    }
                    var errorMessage = string.Format("Start Range {0} is less than previous charge End Range {1}",
                        startRange, previousCharge.EndRange);
                    return Json(errorMessage, "application/json");
                }
                return Json(true, "application/json");
            }
            var dataErrorMessage = string.Format("Please Select a Schedule and Enter a Start Range");
            return Json(dataErrorMessage, "application/json");
        }

        [HttpPost]
        public async Task<JsonResult> ChargeEndRangeValidation(string EndRange, string StartRange, Guid? ChargeScheduleId)
        {
            int startRange;
            int endRange;
            var startResult = int.TryParse(StartRange, out startRange);
            var endResult = int.TryParse(EndRange, out endRange);

            if (startResult && endResult && (ChargeScheduleId != null))
            {
                if (endRange > startRange)
                {
                    var previousCharge =
                        await
                            db.Charges.Where(x => x.ChargeScheduleId == ChargeScheduleId)
                                .OrderByDescending(x => x.DateCreated)
                                .FirstOrDefaultAsync();
                    if (previousCharge != null)
                    {
                        if (startRange > previousCharge.StartRange)
                        {
                            return Json(true, "application/json");
                        }
                        var errorMessage = string.Format("Start Range {0} is less than previous charge End Range {1}",
                            startRange, previousCharge.EndRange);
                        return Json(errorMessage, "application/json");
                    }
                    return Json(true, "application/json");
                }
                var rangeError = string.Format("End Range {0} is less than Start Range {1}", EndRange, StartRange);
                return Json(rangeError, "application/json");
            }
            var errorValues = string.Format("End Range {0} is less than Start Range {1}", EndRange, StartRange);
            return Json(errorValues, "application/json");
        }

        [HttpPost]
        public async Task<JsonResult> ChargeUnitValidation(string StartRange, string EndRange, string UnitPrice,
            Guid? ChargeScheduleId)
        {
            int startRange;
            int endRange;
            decimal unitPrice;
            var startResult = int.TryParse(StartRange, out startRange);
            var endResult = int.TryParse(EndRange, out endRange);
            var unitPriceResult = decimal.TryParse(UnitPrice, out unitPrice);

            if (startResult && endResult && unitPriceResult && (ChargeScheduleId != null))
            {
                if (endRange > startRange)
                {
                    var previousCharge =
                        await
                            db.Charges.Where(x => x.ChargeScheduleId == ChargeScheduleId)
                                .OrderByDescending(x => x.DateCreated)
                                .FirstOrDefaultAsync();
                    if (previousCharge != null)
                    {
                        if (startRange > previousCharge.EndRange)
                        {
                            if (unitPrice > previousCharge.UnitPrice)
                            {
                                return Json(true, "application/json");
                            }
                            var unitPriceErrorMessage =
                                string.Format("Unit Price {0} is not larger than Previous Previous Unit Price {1}",
                                    unitPrice, previousCharge.UnitPrice);
                            return Json(unitPriceErrorMessage, "application/json");
                        }
                        var errorMessage = string.Format("Start Range {0} is less than previous charge End Range {1}",
                            startRange, previousCharge.EndRange);
                        return Json(errorMessage, "application/json");
                    }
                    return Json(true, "application/json");
                }
                var rangeError = string.Format("End Range {0} is less than Start Range {1}", EndRange, StartRange);
                return Json(rangeError, "application/json");
            }
            var errorValues = string.Format("End Range {0} is less than Start Range {1}", EndRange, StartRange);
            return Json(errorValues, "application/json");
        }

        [HttpPost]
        public async Task<JsonResult> ChargeScheduleValidation(Guid? ChargeScheduleId)
        {
            var previousCharge =
                await
                    db.Charges.Where(x => x.ChargeScheduleId == ChargeScheduleId)
                        .OrderByDescending(x => x.DateCreated)
                        .FirstOrDefaultAsync();
            if (previousCharge != null)
            {
                return Json(true, "application/json");
            }
            return Json(true, "application/json");
        }

        // GET: Charges/Delete/5
        public async Task<ActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var charge = await db.Charges.FindAsync(id);
            if (charge == null)
            {
                return HttpNotFound();
            }
            return View(charge);
        }

        // POST: Charges/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(Guid id)
        {
            var charge = await db.Charges.FindAsync(id);
            db.Charges.Remove(charge);
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