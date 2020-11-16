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
using FOS_WS.Filters;
using FOS_WS.Models;
using Newtonsoft.Json.Linq;

namespace FOS_WS.Services.Panel
{
    public class PanelController : ApiController
    {
        private FOSWSDB db = new FOSWSDB();

        // GET: api/menu
        public IQueryable<Food> GetFoods()
        {
            return db.Foods;
        }

        // GET: api/menu/5
        [ResponseType(typeof(Food))]
        public IHttpActionResult GetFood(int id)
        {
            Food food = db.Foods.Find(id);
            if (food == null)
            {
                return NotFound();
            }

            return Ok(food);
        }

        // PUT: api/menu/5
        [RFilter(Role ="Resturant")]
        [Route("~/api/update-resturant/{id}")]
        [HttpPut]
        public IHttpActionResult PutResturant(int id, [FromBody] Resturant resturant)
        {

            var q = (from a in db.Resturants where id == a.RID select a).SingleOrDefault(); 
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (RFilter.uid != q.UID)
            {
                return BadRequest("You Can't Access to This Section");
            }

            q.Rname = resturant.Rname;
            q.Rstatus = resturant.Rstatus;

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

            return Ok("Information Updated");
        }

        // POST: api/menu
        [RFilter(Role = "Resturant")]
        [Route("~/api/define-food")]
        [HttpPost]
        public IHttpActionResult PostFood(Food food)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                Resturant resturant = (from a in db.Resturants where a.UID == RFilter.uid select a).SingleOrDefault();
                food.RID = resturant.RID;
                db.Foods.Add(food);
                db.SaveChanges();
            }
            catch(Exception e)
            {
                return BadRequest(e.Message);
            }


            return Ok("Food Added To Your Resturant Menu");
        }



        // DELETE: api/menu/5
        [RFilter(Role = "Resturant")]
        [Route("~/api/del-resturant/{id}")]
        [HttpDelete]
        public IHttpActionResult DeleteRes(int id)
        {
            Resturant res = db.Resturants.Find(id);
            User user = db.Users.Find(res.UID);
            if (res == null || user == null)
            {
                return NotFound();
            }

            db.Users.Remove(user);
            db.Resturants.Remove(res);
            db.SaveChanges();

            return Ok("Account Deleted Successfully.");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool FoodExists(int id)
        {
            return db.Foods.Count(e => e.FID == id) > 0;
        }
        private bool ResturantExists(int id)
        {
            return db.Resturants.Count(e => e.RID == id) > 0;
        }
    }
}