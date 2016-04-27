using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Umbraco.Web.Mvc;
using UmbracoMembers.Models;


namespace UmbracoMembers.Controllers
{
    public class LogoutController : RenderMvcController
    {
        // Signs out the user and redirects to the site home page - not a surfacecontroller - just overrides the route using the doc type:
        [HttpGet]
        public ActionResult Index()
        {
            // clear the session redirect variable for future logins
            HttpContext.Session["redirectURL"] = null;
            Members.Logout();
            return Redirect("/");
        }
    }
}
