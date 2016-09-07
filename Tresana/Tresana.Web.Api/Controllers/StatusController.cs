using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using Tresana.Data.DataAccess;
using Tresana.Data.Entities;

namespace Tresana.Web.Api.Controllers
{
    public class StatusController : ApiController
    {
        private TresanaContext db = new TresanaContext();

        // GET: api/Status
        public IQueryable<Status> GetStatuses()
        {
            return db.Statuses;
        }

        // GET: api/Status/5
        [ResponseType(typeof(Status))]
        public IHttpActionResult GetStatus(int id)
        {
            Status status = db.Statuses.Find(id);
            if (status == null)
            {
                return NotFound();
            }

            return Ok(status);
        }

        // PUT: api/Status/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutStatus(int id, Status status)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != status.Id)
            {
                return BadRequest();
            }

            db.Entry(status).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!StatusExists(id))
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

        // POST: api/Status
        [ResponseType(typeof(Status))]
        public IHttpActionResult PostStatus(Status status)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Statuses.Add(status);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = status.Id }, status);
        }

        // DELETE: api/Status/5
        [ResponseType(typeof(Status))]
        public IHttpActionResult DeleteStatus(int id)
        {
            Status status = db.Statuses.Find(id);
            if (status == null)
            {
                return NotFound();
            }

            db.Statuses.Remove(status);
            db.SaveChanges();

            return Ok(status);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool StatusExists(int id)
        {
            return db.Statuses.Count(e => e.Id == id) > 0;
        }
    }
}