using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using SysWaterRev.BusinessLayer.Models;
using SysWaterRev.BusinessLayer.ViewModels;
using SysWaterRev.ManagementPortal.Framework;
using System.Data.Entity;
namespace SysWaterRev.ManagementPortal.Controllers
{
    public class BillingsController : AbstractController
    {
        private readonly ApplicationDbContext db;

        public BillingsController()
        {
            db = new ApplicationDbContext();
        }
        // GET: Billings
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<JsonResult> GetBillingCustomers([DataSourceRequest] DataSourceRequest request)
        {
            var customers = Map<List<Customer>, List<CustomerViewModel>>(await db.Customers.Include(x => x.Meters).ToListAsync()).ToDataSourceResult(request);
            return Json(customers);
        }

        [HttpPost]
        public async Task<JsonResult> GetBillingMeters([DataSourceRequest] DataSourceRequest request, Guid? CustomerId)
        {
            var meters =
                Map<List<Meter>, List<MeterViewModel>>(await db.Meters.Include(x => x.MeterReadings).ToListAsync())
                    .ToDataSourceResult(request);          
            return Json(meters);
        }

        [HttpPost]
        public async Task<JsonResult> GetBillingReadings([DataSourceRequest] DataSourceRequest request, Guid? MeterId)
        {
            var readings =
                Map<List<Reading>, List<ReadingViewModel>>(await db.Readings.Include(x => x.MeterRead).ToListAsync())
                    .ToDataSourceResult(request);
            return Json(readings);
        }

        // GET: Billings/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Billings/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Billings/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Billings/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Billings/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Billings/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Billings/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
