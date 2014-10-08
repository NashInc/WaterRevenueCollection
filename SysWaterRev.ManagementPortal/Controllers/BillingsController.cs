using System;
using System.Linq;
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

        private async Task<List<Tuple<double, double, decimal>>> GenerateChargeTree()
        {
            var chargeSchedule =
               await db.ChargeSchedules.Include(x => x.Charges).OrderByDescending(x => x.DateCreated).FirstOrDefaultAsync();
            var charges = chargeSchedule.Charges;
            var chargeTable = new List<Tuple<double, double, decimal>>();
            foreach (var charge in charges)
            {
                //building the structure
                var row = new Tuple<double, double, decimal>(charge.StartRange, charge.EndRange, charge.UnitPrice);
                chargeTable.Add(row);
            }
            return chargeTable;
        }
        [HttpPost]
        public async Task<JsonResult> GetBillingCustomers([DataSourceRequest] DataSourceRequest request)
        {
            var customers = Map<List<Customer>, List<CustomerViewModel>>(await db.Customers.Include(x => x.Meters).ToListAsync());
            var chargeTable = await GenerateChargeTree();
            foreach (var customer in customers)
            {
                var meters = Map<List<Meter>,List<MeterViewModel>>(await db.Meters.Where(x=>x.CustomerId==customer.CustomerId).ToListAsync());
                foreach (var meter in meters)
                {
                    var readings =
                        Map<List<Reading>, List<ReadingViewModel>>(
                            await db.Readings.Where(x => x.MeterId == meter.MeterId).ToListAsync());
                    foreach (var reading in readings)
                    {
                        foreach (var row in chargeTable)
                        {
                            if (reading.ReadingValue > row.Item1 && reading.ReadingValue < row.Item1)
                            {
                                reading.TotalBill = reading.UnitsConsumedWithNoCorrection * row.Item3;
                            }
                            else
                            {
                                reading.TotalBill = reading.UnitsConsumedWithNoCorrection*chargeTable.Last().Item3;
                            }
                        }
                    }
                    meter.TotalBill = readings.Sum(x => x.TotalBill);
                }
                customer.TotalBill = meters.Sum(x => x.TotalBill);
            }
            return Json(customers.ToDataSourceResult(request));
        }

        [HttpPost]
        public async Task<JsonResult> GetBillingMeters([DataSourceRequest] DataSourceRequest request, Guid? CustomerId)
        {
            var chargeTable = await GenerateChargeTree();
            var meters =
                Map<List<Meter>, List<MeterViewModel>>(await db.Meters.Include(x => x.MeterReadings).ToListAsync());
            foreach (var meter in meters)
            {
                 var readings =
                        Map<List<Reading>, List<ReadingViewModel>>(
                            await db.Readings.Where(x => x.MeterId == meter.MeterId).ToListAsync());
                    foreach (var reading in readings)
                    {
                        foreach (var row in chargeTable)
                        {
                            if (reading.ReadingValue > row.Item1 && reading.ReadingValue < row.Item1)
                            {
                                reading.TotalBill = reading.UnitsConsumedWithNoCorrection * row.Item3;
                            }
                            else
                            {
                                reading.TotalBill = reading.UnitsConsumedWithNoCorrection * chargeTable.Last().Item3;
                            }
                        }
                    }
                    meter.TotalBill = readings.Sum(x => x.TotalBill);
                }           
            return Json(meters.ToDataSourceResult(request));
        }

        [HttpPost]
        public async Task<JsonResult> GetBillingReadings([DataSourceRequest] DataSourceRequest request, Guid? MeterId)
        {
            var chargeTable = await GenerateChargeTree();
            var readings =
                Map<List<Reading>, List<ReadingViewModel>>(await db.Readings.Include(x => x.MeterRead).ToListAsync());
            foreach (var reading in readings)
            {
                foreach (var row in chargeTable)
                {
                    if (reading.ReadingValue > row.Item1 && reading.ReadingValue < row.Item1)
                    {
                        reading.TotalBill = reading.UnitsConsumedWithNoCorrection * row.Item3;
                    }
                    else
                    {
                        reading.TotalBill = reading.UnitsConsumedWithNoCorrection * chargeTable.Last().Item3;
                    }
                }
            }
            return Json(readings.ToDataSourceResult(request));
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
