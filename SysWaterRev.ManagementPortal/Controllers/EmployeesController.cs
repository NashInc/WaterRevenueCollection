using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using SysWaterRev.BusinessLayer.Framework;
using SysWaterRev.BusinessLayer.Models;
using SysWaterRev.BusinessLayer.ViewModels;
using SysWaterRev.ManagementPortal.Framework;

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
        public  ActionResult Create(EmployeeViewModel employee, params string[] selectedRoles)
        {
            try
            {
                var appUser = new ApplicationUser
                {
                    IsActive = true,
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
                var userResult = UserManager.Create(appUser);
                if (!userResult.Succeeded) return View(employee);
                var addToRoleResult = UserManager.AddToRoles(appUser.Id, selectedRoles);
                if (!addToRoleResult.Succeeded) return View(employee);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("*", ex);
                return View(employee);
            }
        }

        // GET: Employees/Edit/5
        [HttpGet]
        public async Task<ActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var applicationUser = await db.Users.Include(x=>x.EmployeeDetails).FirstOrDefaultAsync(x => x.EmployeeDetails.EmployeeId == id);
            if (applicationUser == null)
            {
                return HttpNotFound();
            }
            var employeeViewModel = Map<Employee, EmployeeViewModel>(applicationUser.EmployeeDetails);
            return View(employeeViewModel);
        }

        // POST: Employees/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(EmployeeViewModel employee)
        {
            var applicationUser =
                await
                    db.Users.Include(x => x.EmployeeDetails)
                        .FirstOrDefaultAsync(x => x.EmployeeDetails.EmployeeId == employee.EmployeeId);
            if (applicationUser != null)
            {
                //Employee Detail Edits
                applicationUser.EmployeeDetails.FirstName = employee.FirstName;
                applicationUser.EmployeeDetails.MiddleName = employee.MiddleName;
                applicationUser.EmployeeDetails.Surname = employee.Surname;
                applicationUser.EmployeeDetails.EmailAddress = employee.EmailAddress;
                applicationUser.EmployeeDetails.EmployeeGender = employee.EmployeeGender;
                applicationUser.EmployeeDetails.EmployeeNumber = employee.EmployeeNumber;                   
                applicationUser.EmployeeDetails.Identification = employee.Identification;
                //User Detail Edits
                applicationUser.Email = employee.EmailAddress;
                applicationUser.UserName = employee.EmailAddress;
                applicationUser.PhoneNumber = employee.PhoneNumber;
                //base
                applicationUser.EmployeeDetails.LastEditDate = DateTime.Now;
                applicationUser.EmployeeDetails.LastEditedBy = User.Identity.Name;
                //Save the mods
                db.Entry(applicationUser).State=EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return HttpNotFound("Entity with the State ID Could not be found!");
        }

        // GET: Employees/Delete/5
        [HttpGet]
        public async Task<ActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var applicationUser = await db.Users.Include(x => x.EmployeeDetails).FirstOrDefaultAsync(z => z.EmployeeDetails.EmployeeId == id);
            if (applicationUser == null)
            {
                return HttpNotFound();
            }
            var employeeModel = Map<Employee, EmployeeViewModel>(applicationUser.EmployeeDetails);
            return View(employeeModel);
        }

        // POST: Employees/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(Guid id)
        {
            var applicationUser = await db.Users.Include(x => x.EmployeeDetails).FirstOrDefaultAsync(z => z.EmployeeDetails.EmployeeId == id);
            db.Users.Remove(applicationUser);
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