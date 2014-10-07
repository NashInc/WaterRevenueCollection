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
    public class ChargeSchedulesController : AbstractController
    {
        private readonly ApplicationDbContext db;

        public ChargeSchedulesController()
        {
            db = new ApplicationDbContext();
        }

        // GET: ChargeSchedules
        [HttpGet]
        public async Task<ActionResult> Index()
        {
            var chargesSchedulesViewModel =
                Map<List<ChargeSchedule>, List<ChargeScheduleViewModel>>(await db.ChargeSchedules.ToListAsync());
            return View(chargesSchedulesViewModel);
        }

        public async Task<ActionResult> ReadChargeSchedules()
        {
            var chargeSchedule =
                Map<List<ChargeSchedule>, List<ChargeScheduleViewModel>>(await db.ChargeSchedules.ToListAsync());
            return Json(chargeSchedule, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ActivateChargeSchedule(Guid? ChargeScheduleId)
        {
            var chargeSchedule = await db.ChargeSchedules.FindAsync(ChargeScheduleId);
            if (chargeSchedule != null)
            {
                chargeSchedule.IsActive = true;
                chargeSchedule.ActivatedBy = User.Identity.Name;
                chargeSchedule.LastEditDate = DateTime.Now;
                chargeSchedule.LastEditedBy = User.Identity.Name;
                db.Entry(chargeSchedule).State = EntityState.Modified;
                await db.SaveChangesAsync();
                TempData.Clear();
                TempData.Add("ChargeScheduleId", chargeSchedule.ChargeScheduleId);
                return RedirectToAction("Details", "ChargeSchedules", new { id = ChargeScheduleId });
            }
            return RedirectToAction("Details", "ChargeSchedules", new { id = ChargeScheduleId });
        }

        [HttpPost]
        public JsonResult GetChargesForSchedule([DataSourceRequest] DataSourceRequest request, Guid? ChargeScheduleId)
        {
            var chargesViewModel = db.Charges.Include(x => x.ChargeSchedule)
                .Where(x => x.ChargeScheduleId == ChargeScheduleId)
                .Select(x => new ChargeViewModel
                {
                    CreatedBy = x.CreatedBy,
                    UnitPrice = x.UnitPrice.ToString(),
                    StartRange = x.StartRange,
                    EndRange = x.EndRange,
                    DateCreated = x.DateCreated,
                    ChargeScheduleId = x.ChargeScheduleId,
                    ChargeId = x.ChargeId,
                    LastEditDate = x.LastEditDate,
                    LastEditedBy = x.LastEditedBy,
                    ChargeScheduleName = x.ChargeSchedule.ChargeScheduleName
                }).ToDataSourceResult(request);
            return Json(chargesViewModel, "application/json");
        }

        [HttpPost]
        public async Task<JsonResult> UpdateChargesForSchedule([DataSourceRequest] DataSourceRequest request,ChargeViewModel model)
        {
            var chargeSchedule =
                await
                    db.ChargeSchedules.Include(x => x.Charges)
                        .SingleOrDefaultAsync(x => x.ChargeScheduleId == model.ChargeScheduleId);
            if (chargeSchedule != null)
            {
                var charge = chargeSchedule.Charges.SingleOrDefault(x => x.ChargeId == model.ChargeId);
                if (charge != null)
                {
                    var newUnitPrice = decimal.Parse(model.UnitPrice);
                    var previousCharge = new Charge();
                    var nextCharge = new Charge();
                    if (charge.PreviousCharge != null)
                    {
                        previousCharge = charge.PreviousCharge;
                        if (model.EndRange < model.StartRange)
                        {
                            if (model.StartRange > previousCharge.EndRange)
                            {
                                if (newUnitPrice > previousCharge.UnitPrice)
                                {
                                    if (charge.NextCharge != null)
                                    {
                                        nextCharge = charge.NextCharge;
                                        if (model.EndRange < nextCharge.StartRange)
                                        {
                                            if (newUnitPrice < nextCharge.UnitPrice)
                                            {
                                                charge.UnitPrice = newUnitPrice;
                                                charge.StartRange = model.StartRange;
                                                charge.EndRange = model.EndRange;
                                                charge.LastEditDate = DateTime.Now;
                                                charge.LastEditedBy = User.Identity.Name;
                                            }
                                            else
                                            {
                                                var errorMessage =
                                                    string.Format(
                                                        "New Unit Price {0} is Greater than Next charge Unit Price {1}",
                                                        newUnitPrice, nextCharge.UnitPrice);
                                                return Json(errorMessage, "application/json");
                                            }
                                        }
                                        else
                                        {
                                            var errorMessage =
                                                string.Format(
                                                    "New End Range {0} is greater than Next Start Range {1}",
                                                    model.EndRange, nextCharge.StartRange);
                                            return Json(errorMessage, "application/json");
                                        }
                                    }
                                    else
                                    {
                                        charge.UnitPrice = newUnitPrice;
                                        charge.StartRange = model.StartRange;
                                        charge.EndRange = model.EndRange;
                                        charge.LastEditDate = DateTime.Now;
                                        charge.LastEditedBy = User.Identity.Name;
                                    }
                                }
                                else
                                {
                                    var errorMessage =
                                        string.Format(
                                            "New Unit Price {0} is less than Previous charge Unit Price {1}",
                                            newUnitPrice, previousCharge.UnitPrice);
                                    return Json(errorMessage, "application/json");
                                }
                            }
                            else
                            {
                                var errorMessage =
                                    string.Format(
                                        "Start Range {0} is less than Previous End Range {1}",
                                        newUnitPrice, previousCharge.UnitPrice);
                                return Json(errorMessage, "application/json");
                            }
                        }
                        else
                        {
                            charge.UnitPrice = newUnitPrice;
                            charge.StartRange = model.StartRange;
                            charge.EndRange = model.EndRange;
                            charge.LastEditDate = DateTime.Now;
                            charge.LastEditedBy = User.Identity.Name;
                        }
                    }
                    else
                    {
                        charge.UnitPrice = newUnitPrice;
                        charge.StartRange = model.StartRange;
                        charge.EndRange = model.EndRange;
                        charge.LastEditDate = DateTime.Now;
                        charge.LastEditedBy = User.Identity.Name;
                    }
                }
                db.Entry(charge).State=EntityState.Modified;
                await db.SaveChangesAsync();
                return Json(new[] { model }.ToDataSourceResult(request, ModelState));
            }
            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }

        // GET: ChargeSchedules/Details/5
        [HttpGet]
        public async Task<ActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var chargeSchedule = await db.ChargeSchedules.FindAsync(id);
            if (chargeSchedule == null)
            {
                return HttpNotFound();
            }
            var chargeScheduleViewModel =
                Map<ChargeSchedule, ChargeScheduleViewModel>(chargeSchedule);
            return View(chargeScheduleViewModel);
        }

        // GET: ChargeSchedules/Create
        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        // POST: ChargeSchedules/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "ChargeScheduleName,Description,EffectiveDate")] ChargeScheduleViewModel chargeScheduleModel)
        {
            chargeScheduleModel.IsActive = false;
            chargeScheduleModel.CreatedBy = User.Identity.Name;
            chargeScheduleModel.DateCreated = DateTime.Now;
            try
            {
                chargeScheduleModel.ChargeScheduleId = IdentityGenerator.NewSequentialGuid();
                var chargeSchedule = Map<ChargeScheduleViewModel, ChargeSchedule>(chargeScheduleModel);
                db.ChargeSchedules.Add(chargeSchedule);
                await db.SaveChangesAsync();
                TempData.Clear();
                TempData.Add("ChargeScheduleId", chargeSchedule.ChargeScheduleId);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("*", ex);
                return View(chargeScheduleModel);
            }
        }

        // GET: ChargeSchedules/Edit/5
        [HttpGet]
        public async Task<ActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var chargeSchedule = await db.ChargeSchedules.FindAsync(id);
            if (chargeSchedule == null)
            {
                return HttpNotFound();
            }
            var chargeScheduleViewModel =
                Map<ChargeSchedule, ChargeScheduleViewModel>(chargeSchedule);
            return View(chargeScheduleViewModel);
        }

        // POST: ChargeSchedules/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "ChargeScheduleId,ChargeScheduleName,Description,EffectiveDate")] ChargeScheduleViewModel
                chargeSchedule)
        {
            var dbChargeSchedule = await db.ChargeSchedules.FindAsync(chargeSchedule.ChargeScheduleId);
            if (dbChargeSchedule != null)
            {
                dbChargeSchedule.ChargeScheduleName = chargeSchedule.ChargeScheduleName;
                dbChargeSchedule.Description = chargeSchedule.Description;
                dbChargeSchedule.LastEditDate = DateTime.Now;
                dbChargeSchedule.LastEditedBy = User.Identity.Name;
                dbChargeSchedule.EffectiveDate = chargeSchedule.EffectiveDate;
                try
                {
                    db.Entry(dbChargeSchedule).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                    TempData.Clear();
                    TempData.Add("ChargeScheduleId", dbChargeSchedule.ChargeScheduleId);
                    return RedirectToAction("Index", "ChargeSchedules");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("*", ex);
                    return View(chargeSchedule);
                }
            }
            ModelState.AddModelError("*", "Could Not Find Schedule with the stated ID");
            return View(chargeSchedule);
        }

        // GET: ChargeSchedules/Delete/5
        [HttpGet]
        public async Task<ActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var chargeSchedule = await db.ChargeSchedules.FindAsync(id);

            if (chargeSchedule == null)
            {
                return HttpNotFound();
            }
            var chargeScheduleViewModel =
                Map<ChargeSchedule, ChargeScheduleViewModel>(chargeSchedule);
            return View(chargeScheduleViewModel);
        }

        // POST: ChargeSchedules/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(Guid id)
        {
            var chargeSchedule = await db.ChargeSchedules.FindAsync(id);
            db.ChargeSchedules.Remove(chargeSchedule);
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

        #region ChargeActions

        [HttpGet]
        public async Task<ActionResult> AddChargesToSchedule(Guid? ChargeScheduleId)
        {
            var chargeSchedule = await db.ChargeSchedules.FindAsync(ChargeScheduleId);
            if (chargeSchedule != null)
            {
                ViewBag.ChargeScheduleId = chargeSchedule.ChargeScheduleId;
                ViewBag.ChargeScheduleName = chargeSchedule.ChargeScheduleName;
                return View("AddChargesToSchedule");
            }
            return HttpNotFound("Entity with the Stated ID Could not be found!");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddChargesToSchedule([Bind(Include = "StartRange,EndRange,ChargeScheduleId,UnitPrice")]ChargeViewModel chargeVm)
        {
            var chargeSchedule = await db.ChargeSchedules.Include(x => x.Charges).SingleOrDefaultAsync(x => x.ChargeScheduleId == chargeVm.ChargeScheduleId);
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
                        UnitPrice = decimal.Parse(chargeVm.UnitPrice),
                    };
                    var previousCharge = chargeSchedule.Charges.OrderByDescending(x => x.DateCreated).FirstOrDefault();
                    if (previousCharge != null)
                    {
                        charge.PreviousCharge = charge;
                    }
                    db.Charges.Add(charge);
                    await db.SaveChangesAsync();
                    return RedirectToAction("AddChargesToSchedule", "ChargeSchedules", new { charge.ChargeScheduleId });
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("*", ex);
                    return View("AddChargesToSchedule", chargeVm);
                }
            }
            return View("AddChargesToSchedule", chargeVm);
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
                        var errorMessage = string.Format(
                            "Start Range {0} is less than previous charge End Range {1}",
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
        public async Task<JsonResult> ChargeUnitValidation(string StartRange, string EndRange, string UnitPrice, Guid? ChargeScheduleId)
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
                        if (startRange > previousCharge.StartRange)
                        {
                            if (unitPrice > previousCharge.UnitPrice)
                            {
                                return Json(true, "application/json");
                            }
                            string unitPriceErrorMessage =
                                string.Format("Unit Price {0} is not larger than Previous Previous Unit Price {1}",
                                    unitPrice, previousCharge.UnitPrice);
                            return Json(unitPriceErrorMessage, "application/json");
                        }
                        var errorMessage = string.Format(
                            "Start Range {0} is less than previous charge End Range {1}",
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

        #endregion ChargeActions
    }
}