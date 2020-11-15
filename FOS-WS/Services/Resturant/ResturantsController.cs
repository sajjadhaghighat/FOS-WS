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
using FOS_WS.Models;

namespace FOS_WS.Services.Resturant
{
    public class ResturantsController : ApiController
    {
        private FOSWSDB db = new FOSWSDB();

        // GET: api/Resturants
        public IQueryable<Resturant> GetResturants()
        {
            return db.Resturants;
        }

        // GET: api/Resturants/5
        [ResponseType(typeof(Resturant))]
        public IHttpActionResult GetResturant(int id)
        {
            Resturant resturant = db.Resturants.Find(id);
            if (resturant == null)
            {
                return NotFound();
            }

            return Ok(resturant);
        }

        // PUT: api/Resturants/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutResturant(int id, Resturant resturant)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != resturant.RID)
            {
                return BadRequest();
            }

            db.Entry(resturant).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ResturantExists(id))
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

        // POST: api/Resturants
        [ResponseType(typeof(Resturant))]
        public IHttpActionResult PostResturant(Resturant resturant)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Resturants.Add(resturant);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = resturant.RID }, resturant);
        }

        // DELETE: api/Resturants/5
        [ResponseType(typeof(Resturant))]
        public IHttpActionResult DeleteResturant(int id)
        {
            Resturant resturant = db.Resturants.Find(id);
            if (resturant == null)
            {
                return NotFound();
            }

            db.Resturants.Remove(resturant);
            db.SaveChanges();

            return Ok(resturant);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ResturantExists(int id)
        {
            return db.Resturants.Count(e => e.RID == id) > 0;
        }
    }
}