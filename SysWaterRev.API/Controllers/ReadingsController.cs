using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security.OAuth;
using SysWaterRev.BusinessLayer.Framework;
using SysWaterRev.BusinessLayer.Models;
using SysWaterRev.BusinessLayer.ViewModels;

namespace SysWaterRev.API.Controllers
{
    [HostAuthentication(OAuthDefaults.AuthenticationType)]
    public class ReadingsController : ApiController
    {
        private readonly ApplicationDbContext db;

        public ReadingsController()
        {
            db = new ApplicationDbContext();
            db.Configuration.LazyLoadingEnabled = false;
            db.Configuration.ProxyCreationEnabled = false;
        }

        // GET: api/Readings
        public IQueryable<Reading> GetReadings()
        {
            return db.Readings;
        }

        private ApplicationUserManager userManager;
        public ApplicationUserManager UserManager
        {
            get
            {
                return userManager ??
                       ControllerContext.Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
        }

        // GET: api/Readings/5
        [ResponseType(typeof(Reading))]
        public async Task<IHttpActionResult> GetReading(Guid id)
        {
            var reading = await db.Readings.FindAsync(id);
            if (reading == null)
            {
                return NotFound();
            }

            return Ok(reading);
        }

        // PUT: api/Readings/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutReading(Guid id, Reading reading)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != reading.ReadingId)
            {
                return BadRequest();
            }

            db.Entry(reading).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ReadingExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Readings
        [ResponseType(typeof(ReadingViewModel))]
        [HttpPost]
        public async Task<IHttpActionResult> PostReading(ReadingViewModel reading)
        {
            var currentEmployee = db.Users.Include(x => x.EmployeeDetails).FirstOrDefault(x => x.UserName == reading.UserName);
            if (currentEmployee != null)
            {
                var readingModel = new Reading
                {
                    Accuracy = reading.Accuracy,
                    Altitude = reading.Altitude,
                    AltitudeAccuracy = reading.AltitudeAccuracy,
                    MeterId = reading.MeterId,
                    DateCreated = DateTime.Now,
                    CreatedBy = currentEmployee.UserName,
                    EmployeeId = currentEmployee.EmployeeDetails.EmployeeId,
                    Speed = reading.Speed,
                    ReadingValue = reading.ReadingValue,
                    ReadingId = IdentityGenerator.NewSequentialGuid(),
                    Longitude = reading.Longitude,
                    Latitude = reading.Latitude,
                    Heading = reading.Heading,
                    LocationDateTime = DateTime.Now,
                    IsConfirmed = false,
                };
                var previousReading = await db.Readings.OrderByDescending(x => x.DateCreated).FirstOrDefaultAsync();

                if (previousReading != null)
                {
                    if (readingModel.ReadingValue >= previousReading.ReadingValue)
                    {
                        readingModel.PreviousReading = previousReading;
                    }
                    else
                    {
                        return Conflict();
                    }
                }

                try
                {
                    db.Readings.Add(readingModel);
                    await db.SaveChangesAsync();
                    //Update ViewModel
                    reading.DateCreated = readingModel.DateCreated;
                    reading.ReadingId = readingModel.ReadingId;
                    reading.CreatedBy = readingModel.CreatedBy;
                    reading.EmployeeId = readingModel.EmployeeId;

                    return CreatedAtRoute("DefaultApi", new { id = reading.ReadingId }, reading);
                }
                catch (DbUpdateException ex)
                {
                    return Conflict();
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }
            return NotFound();
        }

        // DELETE: api/Readings/5
        [ResponseType(typeof(Reading))]
        public async Task<IHttpActionResult> DeleteReading(Guid id)
        {
            Reading reading = await db.Readings.FindAsync(id);
            if (reading == null)
            {
                return NotFound();
            }

            db.Readings.Remove(reading);
            await db.SaveChangesAsync();

            return Ok(reading);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ReadingExists(Guid id)
        {
            return db.Readings.Count(e => e.ReadingId == id) > 0;
        }
    }
}