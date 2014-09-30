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
    public class SystemSettingsController : AbstractController
    {
        private readonly ApplicationDbContext db;

        public SystemSettingsController()
        {
            db = new ApplicationDbContext();
        }

        // GET: SystemSettings
        [HttpGet]
        public async Task<ActionResult> Index()
        {
            List<SystemSetting> systemSettings =
                await db.SystemSettings.Include(s => s.CurrentChargeSchedule).ToListAsync();
            List<SystemSettingsViewModel> systemViewModel =
                Map<List<SystemSetting>, List<SystemSettingsViewModel>>(systemSettings);
            return View(systemViewModel);
        }

        // GET: SystemSettings/Details/5
        public async Task<ActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SystemSetting systemSetting = await db.SystemSettings.FindAsync(id);
            if (systemSetting == null)
            {
                return HttpNotFound();
            }
            SystemSettingsViewModel systemSettingViewModel = Map<SystemSetting, SystemSettingsViewModel>(systemSetting);
            return View(systemSettingViewModel);
        }

        public async Task<JsonResult> GetSchedules()
        {
            List<ChargeSchedule> schedules =
                await db.ChargeSchedules.Include(x => x.Charges).Where(z => z.IsActive).ToListAsync();
            List<ChargeScheduleViewModel> schedulesViewModel =
                Map<List<ChargeSchedule>, List<ChargeScheduleViewModel>>(schedules);
            return Json(schedulesViewModel, "application/json", JsonRequestBehavior.AllowGet);
        }

        // GET: SystemSettings/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: SystemSettings/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(
            [Bind(Include = "ChargeScheduleId")] SystemSettingsViewModel systemSettingModel)
        {
            ChargeSchedule chargeSchedule = await db.ChargeSchedules.FindAsync(systemSettingModel.ChargeScheduleId);
            if (chargeSchedule != null)
            {
                try
                {
                    var systemSetting = new SystemSetting
                    {
                        SystemSettingId = IdentityGenerator.NewSequentialGuid(),
                        DateCreated = DateTime.Now,
                        CreatedBy = User.Identity.Name,
                        SetBy = User.Identity.Name,
                        ChargeScheduleId = chargeSchedule.ChargeScheduleId
                    };
                    db.SystemSettings.Add(systemSetting);
                    await db.SaveChangesAsync();
                    return RedirectToAction("Index", "SystemSettings");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("*", ex);
                    return View(systemSettingModel);
                }
            }
            ModelState.AddModelError("*", "Error");
            return View(systemSettingModel);
        }

        // GET: SystemSettings/Edit/5
        public async Task<ActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SystemSetting systemSetting = await db.SystemSettings.FindAsync(id);
            if (systemSetting == null)
            {
                return HttpNotFound();
            }
            SystemSettingsViewModel systemSettingViewModel = Map<SystemSetting, SystemSettingsViewModel>(systemSetting);
            return View(systemSettingViewModel);
        }

        // POST: SystemSettings/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(
            [Bind(Include = "SystemSettingId,ChargeScheduleId")] SystemSettingsViewModel systemSettingViewModel)
        {
            ChargeSchedule chargeSchedule = await db.ChargeSchedules.FindAsync(systemSettingViewModel.ChargeScheduleId);
            if (chargeSchedule != null)
            {
                SystemSetting systemSetting = await db.SystemSettings.FindAsync(systemSettingViewModel.SystemSettingId);
                if (systemSetting != null)
                {
                    try
                    {
                        db.Entry(systemSetting).State = EntityState.Modified;
                        await db.SaveChangesAsync();
                        return RedirectToAction("Index");
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError("*", ex);
                        return View(systemSettingViewModel);
                    }
                }
                return HttpNotFound("Could Not Find Entity with that ID");
            }
            return HttpNotFound("Could Not Find Entity with that ID");
        }

        // GET: SystemSettings/Delete/5
        public async Task<ActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SystemSetting systemSetting = await db.SystemSettings.FindAsync(id);
            if (systemSetting == null)
            {
                return HttpNotFound();
            }
            return View(systemSetting);
        }

        // POST: SystemSettings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(Guid id)
        {
            SystemSetting systemSetting = await db.SystemSettings.FindAsync(id);
            db.SystemSettings.Remove(systemSetting);
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