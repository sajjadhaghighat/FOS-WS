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

namespace FOS_WS.Services.Ordering
{
    public class OrdersController : ApiController
    {
        private FOSWSDB db = new FOSWSDB();

        // GET: api/resturant-menu/5
        [AllowAnonymous]
        [Route("~/api/resturant-menu/{id}")]
        [HttpGet]
        public IHttpActionResult GetMenu(int id)
        {
            Resturant res = (from a in db.Resturants where id == a.RID select a).SingleOrDefault();
            if (res == null)
            {
                return NotFound();
            }
            var menu = (from a in db.Foods where id == a.RID select new { a.FID,a.Fname,a.Description,a.Fqty,a.Price,a.RID });
            return Ok(menu);
        }

        // GET: api/resList
        [AllowAnonymous]
        [Route("~/api/resturant-list")]
        [HttpGet]
        public IHttpActionResult GetRlist()
        {
            var resList = db.Resturants.Select(a => new { a.RID, a.Rname, a.Rstatus, a.UID });
            if (resList == null)
            {
                return NotFound();
            }
            var query =
            from a in db.Resturants
            join b in db.Users on a.UID equals b.UID
            select new { Resturant_name = a.Rname, Resturant_status = a.Rstatus,  Manager_Fname = b.Firstname, Manager_Lname = b.Lastname };
            return Ok(query);
        }

//===================================================================================================

        [Route("~/api/order")]
        [RFilter(Role = "Customer")]
        [HttpPost]
        public IHttpActionResult PostOrder(Order order)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                Food food = db.Foods.Find(order.FID);
                if(order.Oqty <= food.Fqty)
                {
                    order.UID = RFilter.uid;
                    order.Price = (int.Parse(food.Price) * order.Oqty).ToString();
                    order.Timestamp = (DateTime.Now).ToString();
                    order.Status = "Waiting for Confirm...";
                    db.Foods.Add(food);
                    db.SaveChanges();
                }
                else
                {
                    return BadRequest("Error : Food stock is not enough!.");
                }
                
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

            return Ok("Order Send to Resturant Successfully. Waiting for Confirm...");
        }



        [Route("~/api/change_order_status/{id}")]
        [RFilter(Role = "Resturant")]
        [HttpPut]
        public IHttpActionResult PutOstatus(int id,[FromBody] Order order)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            User user = db.Users.Find(db.Resturants.Find(db.Foods.Find(db.Orders.Find(id).FID).RID).UID);
            if (RFilter.uid != user.UID)
            {
                return BadRequest("You Can't Access to This Section");
            }
            Order newS = db.Orders.Find(id);
            newS.Status = order.Status;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrderExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok("Order Status Information Updated");
        }


        [Route("~/api/orders_list")]
        [RFilter(Role = "Resturant")]
        [HttpGet]
        public IHttpActionResult GetOlist()
        {
            try
            {
                var query = from a in db.Orders
                            join b in db.Foods on a.FID equals b.FID
                            join c in db.Resturants on b.RID equals c.RID
                            join d in db.Users on c.UID equals d.UID
                            where d.UID == RFilter.uid
                            select new { a.OID, b.Fname, a.Oqty, a.Price, a.Phone, a.User.Firstname, a.Status };
                return Ok(query);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }









        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool OrderExists(int id)
        {
            return db.Orders.Count(e => e.OID == id) > 0;
        }

    }
}