using FOS_WS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Http;

namespace FOS_WS.Filters
{
    public class RFilter : AuthorizeAttribute
    {
        public string Role { get; set; }
        private FOSWSDB db = new FOSWSDB();
            public override void OnAuthorization(System.Web.Http.Controllers.HttpActionContext actionContext)
            {
                if (AuthorizeRequest(actionContext))
                {
                    return;
                }
                HandleUnauthorizedRequest(actionContext);
            }
            protected override void HandleUnauthorizedRequest(System.Web.Http.Controllers.HttpActionContext actionContext)
            {
            //Code to handle unauthorized request
            String MS = "unauthorized request";
                actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized, MS );
                actionContext.Response.Headers.Add("WWW-Authenticate", "Basic Scheme='Data' location = 'http://localhost:");
            }
            private bool AuthorizeRequest(System.Web.Http.Controllers.HttpActionContext actionContext)
            {
            //Write your code here to perform authorization
                var authHeader = actionContext.Request.Headers.Authorization;

                if (authHeader != null)
                {
                    var authenticationToken = actionContext.Request.Headers.Authorization.Parameter;
                    var decodedAuthenticationToken = Encoding.UTF8.GetString(Convert.FromBase64String(authenticationToken));
                    var usernamePasswordArray = decodedAuthenticationToken.Split(':');
                    var userName = usernamePasswordArray[0];
                    var password = usernamePasswordArray[1];

                    // Replace this with your own system of security / means of validating credentials
                    var isValid = (from a in db.Users select a).Where(a => a.Username == userName && a.Password == password && a.Type == this.Role).SingleOrDefault();

                    if (isValid != null)
                    {
                        var principal = new GenericPrincipal(new GenericIdentity(userName), null);
                        Thread.CurrentPrincipal = principal;

                        return true;
                    }
                    return false;
                }
                else
                {
                    return false;
                }
            
            }
        
    }
}