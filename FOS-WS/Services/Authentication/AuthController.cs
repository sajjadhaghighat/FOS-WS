using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using FOS_WS.Models;
using System.Web.Http.Description;
using Newtonsoft.Json.Linq;
using FOS_WS.Filters;

namespace FOS_WS.Services.Authentication
{
    public class AuthController : ApiController
    {
        private FOSWSDB db = new FOSWSDB();


        //[ResponseType(typeof(User))]
        [Route("~/api/register")]
        [HttpPost]
        public IHttpActionResult register([FromBody] JObject data)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                User user = data["udata"].ToObject<User>();
                var q = (from a in db.Users select a).Where(a => a.Username == user.Username).SingleOrDefault();
                if(q != null)
                {
                    return BadRequest("Error : Username Unavailable");
                }
                db.Users.Add(user);
                if (user.Type == "Resturant")
                {
                    Resturant resturant = data["rdata"].ToObject<Resturant>();
                    db.Resturants.Add(resturant);
                }
                //db.Resturants.Add(resturant);
                db.SaveChanges();
                return Ok("Registration successfully");
            }
            catch (Exception e)
            {
                return BadRequest(e.ToString());
            }



        }

        [RFilter(Role ="admin")]
        [Route("~/api/users")]
        [HttpGet]
        public IHttpActionResult getusers()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var q = (from a in db.Users select a);
                if (q != null)
                {
                    return Ok(q);
                }
                else
                {
                    return BadRequest("Error : Information is Incorrect!.");
                }

            }
            catch (Exception e)
            {
                return BadRequest(e.ToString());
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
    }
}
