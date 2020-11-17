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
    [RFilter(Role = "Resturant")]
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
//=================================================================================================
        // PUT: api/update-resturant/5
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

            return Ok("Resturant Information Updated");
        }


        // DELETE: api/del-resturant/5
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

        //===============================================================================================
        // POST: api/define-food
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

        // PUT: api/update-food/5
        [Route("~/api/update-food/{id}")]
        [HttpPut]
        public IHttpActionResult PutFood(int id, [FromBody] Food food)
        {

            var q = (from a in db.Foods where id == a.FID select a).SingleOrDefault();
            var access = (from a in db.Resturants where q.RID == a.RID select a).SingleOrDefault();
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (RFilter.uid != access.UID)
            {
                return BadRequest("You Can't Access to This Section");
            }

            q.Fname = food.Fname;
            q.Description = food.Description;
            q.Fqty = food.Fqty;
            q.Price = food.Price;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FoodExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok("Food Information Updated");
        }


        // DELETE: api/del-food/5
        [Route("~/api/del-food/{id}")]
        [HttpDelete]
        public IHttpActionResult DeleteFood(int id)
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

        // GET: api/menu
        [Route("~/api/menu")]
        [HttpGet]
        public IHttpActionResult GetMenu()
        {
            try
            {
                Resturant res = (from a in db.Resturants where a.UID == RFilter.uid select a).SingleOrDefault();
                var menu = (from a in db.Foods where a.RID == res.RID select new {a.Fname,a.Description,a.Fqty,a.Price});
                return Ok(menu);
            }
            catch(Exception e)
            {
                return BadRequest(e.Message);
            }
        }


//=================================================================================================

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