using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using FOS_WS.Models;
using System.Web.Http.Description;
using Newtonsoft.Json.Linq;

namespace FOS_WS.Services.Authentication
{
    public class AuthController : ApiController
    {
        private FOSWSDB db = new FOSWSDB();


        // POST: api/Register
        [ResponseType(typeof(User))]
        [Route("~/api/register")]
        [HttpPost]
        public IHttpActionResult PostUser([FromBody] JObject data)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                User user = data["udata"].ToObject<User>();
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
                return BadRequest("Fill All Properties");
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
