using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using SimpleRevCollection.Management.Framework;
using SysWaterRev.BusinessLayer.Framework;
using SysWaterRev.BusinessLayer.Models;
using SysWaterRev.BusinessLayer.ViewModels;

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
            List<ChargeScheduleViewModel> chargesSchedulesViewModel =
                Map<List<ChargeSchedule>, List<ChargeScheduleViewModel>>(await db.ChargeSchedules.ToListAsync());
            return View(chargesSchedulesViewModel);
        }

        public async Task<ActionResult> ReadChargeSchedules()
        {
            List<ChargeScheduleViewModel> chargeSchedule =
                Map<List<ChargeSchedule>, List<ChargeScheduleViewModel>>(await db.ChargeSchedules.ToListAsync());
            return Json(chargeSchedule, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ActivateChargeSchedule(Guid? ChargeScheduleId)
        {
            ChargeSchedule chargeSchedule = await db.ChargeSchedules.FindAsync(ChargeScheduleId);
            if (chargeSchedule != null)
            {
                chargeSchedule.IsActive = true;
                chargeSchedule.ActivatedBy = User.Identity.Name;
                chargeSchedule.LastEditDate = DateTime.Now;
                chargeSchedule.LastEditedBy = User.Identity.Name;
                db.Entry(chargeSchedule).State = EntityState.Modified;
                await db.SaveChangesAsync();
                //  TempData.Add("ChargeScheduleId", chargeSchedule.ChargeScheduleId);
                return RedirectToAction("Details", "ChargeSchedules", new {id = ChargeScheduleId});
            }
            return RedirectToAction("Details", "ChargeSchedules", new {id = ChargeScheduleId});
        }

        [HttpPost]
        public JsonResult GetChargesForSchedule([DataSourceRequest] DataSourceRequest request, Guid? ChargeScheduleId)
        {
            DataSourceResult chargesViewModel = db.Charges.Include(x => x.ChargeSchedule)
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

        // GET: ChargeSchedules/Details/5
        [HttpGet]
        public async Task<ActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ChargeSchedule chargeSchedule = await db.ChargeSchedules.FindAsync(id);
            ChargeScheduleViewModel chargeScheduleViewModel =
                Map<ChargeSchedule, ChargeScheduleViewModel>(chargeSchedule);
            if (chargeScheduleViewModel == null)
            {
                return HttpNotFound();
            }
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
        public async Task<ActionResult> Create(
            [Bind(Include = "ChargeScheduleName,Description,EffectiveDate")] ChargeScheduleViewModel chargeScheduleModel)
        {
            chargeScheduleModel.IsActive = false;
            chargeScheduleModel.CreatedBy = User.Identity.Name;
            chargeScheduleModel.DateCreated = DateTime.Now;
            try
            {
                chargeScheduleModel.ChargeScheduleId = IdentityGenerator.NewSequentialGuid();
                ChargeSchedule chargeSchedule = Map<ChargeScheduleViewModel, ChargeSchedule>(chargeScheduleModel);
                db.ChargeSchedules.Add(chargeSchedule);
                await db.SaveChangesAsync();
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
            ChargeSchedule chargeSchedule = await db.ChargeSchedules.FindAsync(id);
            if (chargeSchedule == null)
            {
                return HttpNotFound();
            }
            ChargeScheduleViewModel chargeScheduleViewModel =
                Map<ChargeSchedule, ChargeScheduleViewModel>(chargeSchedule);
            return View(chargeScheduleViewModel);
        }

        // POST: ChargeSchedules/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(
            [Bind(Include = "ChargeScheduleId,ChargeScheduleName,Description,EffectiveDate")] ChargeScheduleViewModel
                chargeSchedule)
        {
            ChargeSchedule dbChargeSchedule = await db.ChargeSchedules.FindAsync(chargeSchedule.ChargeScheduleId);
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
            ChargeSchedule chargeSchedule = await db.ChargeSchedules.FindAsync(id);

            if (chargeSchedule == null)
            {
                return HttpNotFound();
            }
            ChargeScheduleViewModel chargeScheduleViewModel =
                Map<ChargeSchedule, ChargeScheduleViewModel>(chargeSchedule);
            return View(chargeScheduleViewModel);
        }

        // POST: ChargeSchedules/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(Guid id)
        {
            ChargeSchedule chargeSchedule = await db.ChargeSchedules.FindAsync(id);
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
            ChargeSchedule chargeSchedule = await db.ChargeSchedules.FindAsync(ChargeScheduleId);
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
        public async Task<ActionResult> AddChargesToSchedule(
            [Bind(Include = "StartRange,EndRange,ChargeScheduleId,UnitPrice")] ChargeViewModel chargeVm)
        {
            ChargeSchedule chargeSchedule = await db.ChargeSchedules.FindAsync(chargeVm.ChargeScheduleId);
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
                    return RedirectToAction("AddChargesToSchedule", "ChargeSchedules", new {charge.ChargeScheduleId});
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
            bool startResult = int.TryParse(StartRange, out startRange);
            if (startResult && (ChargeScheduleId != null))
            {
                Charge previousCharge =
                    await db.Charges.Where(x => x.ChargeScheduleId == ChargeScheduleId)
                        .OrderByDescending(x => x.DateCreated)
                        .FirstOrDefaultAsync();
                if (previousCharge != null)
                {
                    if (startRange > previousCharge.EndRange)
                    {
                        return Json(true, "application/json");
                    }
                    string errorMessage = string.Format("Start Range {0} is less than previous charge End Range {1}",
                        startRange, previousCharge.EndRange);
                    return Json(errorMessage, "application/json");
                }
                return Json(true, "application/json");
            }
            string dataErrorMessage = string.Format("Please Select a Schedule and Enter a Start Range");
            return Json(dataErrorMessage, "application/json");
        }

        [HttpPost]
        public async Task<JsonResult> ChargeEndRangeValidation(string EndRange, string StartRange,
            Guid? ChargeScheduleId)
        {
            int startRange;
            int endRange;
            bool startResult = int.TryParse(StartRange, out startRange);
            bool endResult = int.TryParse(EndRange, out endRange);

            if (startResult && endResult && (ChargeScheduleId != null))
            {
                if (endRange > startRange)
                {
                    Charge previousCharge =
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
                        string errorMessage = string.Format(
                            "Start Range {0} is less than previous charge End Range {1}",
                            startRange, previousCharge.EndRange);
                        return Json(errorMessage, "application/json");
                    }
                    return Json(true, "application/json");
                }
                string rangeError = string.Format("End Range {0} is less than Start Range {1}", EndRange, StartRange);
                return Json(rangeError, "application/json");
            }
            string errorValues = string.Format("End Range {0} is less than Start Range {1}", EndRange, StartRange);
            return Json(errorValues, "application/json");
        }

        [HttpPost]
        public async Task<JsonResult> ChargeUnitValidation(string StartRange, string EndRange, string UnitPrice,
            Guid? ChargeScheduleId)
        {
            int startRange;
            int endRange;
            decimal unitPrice;
            bool startResult = int.TryParse(StartRange, out startRange);
            bool endResult = int.TryParse(EndRange, out endRange);
            bool unitPriceResult = decimal.TryParse(UnitPrice, out unitPrice);

            if (startResult && endResult && unitPriceResult && (ChargeScheduleId != null))
            {
                if (endRange > startRange)
                {
                    Charge previousCharge =
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
                        string errorMessage = string.Format(
                            "Start Range {0} is less than previous charge End Range {1}",
                            startRange, previousCharge.EndRange);
                        return Json(errorMessage, "application/json");
                    }
                    return Json(true, "application/json");
                }
                string rangeError = string.Format("End Range {0} is less than Start Range {1}", EndRange, StartRange);
                return Json(rangeError, "application/json");
            }
            string errorValues = string.Format("End Range {0} is less than Start Range {1}", EndRange, StartRange);
            return Json(errorValues, "application/json");
        }

        [HttpPost]
        public async Task<JsonResult> ChargeScheduleValidation(Guid? ChargeScheduleId)
        {
            Charge previousCharge =
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