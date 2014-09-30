﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using SimpleRevCollection.Management.Framework;
using SysWaterRev.BusinessLayer.Framework;
using SysWaterRev.BusinessLayer.Models;
using SysWaterRev.BusinessLayer.ViewModels;

namespace SysWaterRev.ManagementPortal.Controllers
{
    [Authorize]
    public class EmployeesController : AbstractController
    {
        private readonly ApplicationDbContext db;
        private ApplicationUserManager usermanager;

        public EmployeesController()
        {
            db = new ApplicationDbContext();
        }

        public ApplicationUserManager UserManager
        {
            get { return usermanager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>(); }
            set { usermanager = value; }
        }

        [HttpPost]
        public JsonResult GetReadingsForEmployee([DataSourceRequest] DataSourceRequest request, Guid? EmployeeId)
        {
            DataSourceResult employeeWithMeters = db.Readings.Where(x => x.EmployeeId == EmployeeId)
                .Select(x => new ReadingViewModel
                {
                    ReadingId = x.ReadingId,
                    MeterId = x.MeterId,
                    Latitude = x.Latitude,
                    Longitude = x.Longitude,
                    IsConfirmed = x.IsConfirmed,
                    ReadingValue = x.ReadingValue,
                    DateCreated = x.DateCreated,
                }).ToDataSourceResult(request);
            return Json(employeeWithMeters, "application/json");
        }

        // GET: Employees
        [HttpGet]
        public async Task<ActionResult> Index()
        {
            List<EmployeeViewModel> employeesViewModel =
                Map<List<Employee>, List<EmployeeViewModel>>(await db.Employees.ToListAsync());
            return View(employeesViewModel);
        }

        // GET: Employees/Details/5
        [HttpGet]
        public async Task<ActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Employee employee = await db.Employees.FindAsync(id);

            if (employee == null)
            {
                return HttpNotFound();
            }
            EmployeeViewModel employeeViewModel = Map<Employee, EmployeeViewModel>(employee);
            return View(employeeViewModel);
        }

        public async Task<ActionResult> GetCascadeEmployee()
        {
            List<EmployeeViewModel> employeeViewModel
                = Map<List<Employee>, List<EmployeeViewModel>>(await db.Employees.ToListAsync());
            return Json(employeeViewModel, "application/json", JsonRequestBehavior.AllowGet);
        }

        // GET: Employees/Create
        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Employees/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(
            [Bind(
                Include =
                    "EmployeeId,FirstName,MiddleName,Surname,PhoneNumber,EmailAddress,Identification,EmployeeNumber,EmployeeGender"
                )] EmployeeViewModel employee, params string[] SelectedRoles)
        {
            var appUser = new ApplicationUser
            {
                IsActive = false,
                UserName = employee.EmailAddress,
                PhoneNumber = employee.PhoneNumber,
                Email = employee.EmailAddress,
                EmployeeDetails = new Employee
                {
                    CreatedBy = User.Identity.Name,
                    EmployeeId = IdentityGenerator.NewSequentialGuid(),
                    EmployeeNumber = IdentityGenerator.GenerateEmployeeNumber(),
                    DateCreated = DateTime.Now,
                    EmailAddress = employee.EmailAddress,
                    FirstName = employee.FirstName,
                    Identification = employee.Identification,
                    MiddleName = employee.MiddleName,
                    Surname = employee.Surname,
                    EmployeeGender = employee.EmployeeGender,
                    PhoneNumber = employee.PhoneNumber
                }
            };
            IdentityResult userResult = await usermanager.CreateAsync(appUser);
            if (!userResult.Succeeded) return View(employee);
            IdentityResult addToRoleResult = await usermanager.AddToRolesAsync(appUser.Id, SelectedRoles);
            if (!addToRoleResult.Succeeded) return View(employee);
            TempData.Add("EmployeeId", appUser.EmployeeDetails.EmployeeId);
            return RedirectToAction("Index");
        }

        // GET: Employees/Edit/5
        [HttpGet]
        public async Task<ActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Employee employee = await db.Employees.FindAsync(id);
            if (employee == null)
            {
                return HttpNotFound();
            }
            EmployeeViewModel employeeViewModel = Map<Employee, EmployeeViewModel>(employee);
            return View(employeeViewModel);
        }

        // POST: Employees/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(
            [Bind(
                Include =
                    "EmployeeId,FirstName,MiddleName,Surname,PhoneNumber,EmailAddress,Identification,EmployeeNumber,EmployeeGender"
                )] EmployeeViewModel employee)
        {
            Employee employeeModel =
                await
                    db.Employees
                        .FirstOrDefaultAsync(z => z.EmployeeId == employee.EmployeeId);
            if (employeeModel != null)
            {
                if (employeeModel.EmployeeId == employee.EmployeeId)
                {
                    employeeModel.FirstName = employee.FirstName;
                    employeeModel.MiddleName = employee.MiddleName;
                    employeeModel.Surname = employee.Surname;
                    employeeModel.PhoneNumber = employee.PhoneNumber;
                    employeeModel.EmailAddress = employee.EmailAddress;
                    employeeModel.Identification = employee.Identification;
                    employeeModel.EmployeeGender = employee.EmployeeGender;
                    employeeModel.EmployeeNumber = employee.EmployeeNumber;
                    employeeModel.LastEditDate = DateTime.Now;
                    employeeModel.LastEditedBy = User.Identity.Name;
                    try
                    {
                        db.Entry(employeeModel).State = EntityState.Modified;
                        await db.SaveChangesAsync();
                        TempData.Add("EmployeeId", employeeModel.EmployeeId);
                        return RedirectToAction("Index");
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError("*", ex);
                        return View(employee);
                    }
                }
                return HttpNotFound();
            }
            return HttpNotFound();
        }

        // GET: Employees/Delete/5
        [HttpGet]
        public async Task<ActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Employee employee = await db.Employees.FirstOrDefaultAsync(z => z.EmployeeId == id);
            if (employee == null)
            {
                return HttpNotFound();
            }
            EmployeeViewModel employeeModel = Map<Employee, EmployeeViewModel>(employee);
            return View(employeeModel);
        }

        // POST: Employees/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(Guid id)
        {
            Employee employee = await db.Employees.FindAsync(id);
            db.Employees.Remove(employee);
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