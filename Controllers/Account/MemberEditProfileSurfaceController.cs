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
    public class MemberEditProfileSurfaceController : SurfaceController
    {
        [ChildActionOnly]
        [ActionName("MemberEditProfileRenderForm")]
        public ActionResult MemberEditProfileRenderForm()
        {
            return PartialView("Account/MemberEditProfile", new MemberRegisterModel());
        }

    }
}