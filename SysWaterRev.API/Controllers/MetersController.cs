using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Microsoft.Owin.Security.OAuth;
using SysWaterRev.BusinessLayer.Models;
using SysWaterRev.BusinessLayer.ViewModels;

namespace SysWaterRev.API.Controllers
{
    [HostAuthentication(OAuthDefaults.AuthenticationType)]
    public class MetersController : ApiController
    {
        private readonly ApplicationDbContext db;

        public MetersController()
        {
            db = new ApplicationDbContext();
            db.Configuration.ProxyCreationEnabled = false;
            db.Configuration.LazyLoadingEnabled = false;
        }

        // GET: api/Meters
        public IQueryable<Meter> GetMeters()
        {
            return db.Meters;
        }

        // GET: api/Meters/5
        [ResponseType(typeof(Meter))]
        public async Task<IHttpActionResult> GetMeter(Guid id)
        {
            var meter = await db.Meters.FindAsync(id);
            if (meter == null)
            {
                return NotFound();
            }
            return Ok(meter);
        }

        [HttpGet]
        [Route("meters/search/{searchQuery}")]
        public async Task<IHttpActionResult> Search(string searchQuery)
        {
            var searchResult =
                await db.Meters.Include(x => x.OwnerCustomer)
                    .Include(x => x.MeterReadings)
                    .Where(x => x.MeterNumber.Contains(searchQuery))
                    .Select(x => new MeterViewModel
                    {
                        CustomerId = x.CustomerId,
                        FirstName = x.OwnerCustomer.FirstName,
                        Surname = x.OwnerCustomer.Surname,
                        MiddleName = x.OwnerCustomer.MiddleName,
                        CustomerNumber = x.OwnerCustomer.CustomerNumber,
                        MeterId = x.MeterId,
                        MeterNumber = x.MeterNumber,
                        CreatedBy = x.CreatedBy,
                        DateCreated = x.DateCreated,
                        LastEditDate = x.LastEditDate,
                        LastEditedBy = x.LastEditedBy,
                        MeterSerialNumber = x.MeterSerialNumber,
                        ReadingsForMeter = x.MeterReadings.Count,
                    }).ToListAsync();
            if (searchResult != null && searchResult.Any())
            {
                return Ok(searchResult);
            }
            return NotFound();
        }

        // PUT: api/Meters/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutMeter(Guid id, Meter meter)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != meter.MeterId)
            {
                return BadRequest();
            }

            db.Entry(meter).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MeterExists(id))
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

        // POST: api/Meters
        [ResponseType(typeof(Meter))]
        public async Task<IHttpActionResult> PostMeter(Meter meter)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Meters.Add(meter);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (MeterExists(meter.MeterId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = meter.MeterId }, meter);
        }

        // DELETE: api/Meters/5
        [ResponseType(typeof(Meter))]
        public async Task<IHttpActionResult> DeleteMeter(Guid id)
        {
            var meter = await db.Meters.FindAsync(id);
            if (meter == null)
            {
                return NotFound();
            }

            db.Meters.Remove(meter);
            await db.SaveChangesAsync();

            return Ok(meter);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool MeterExists(Guid id)
        {
            return db.Meters.Count(e => e.MeterId == id) > 0;
        }
    }
}