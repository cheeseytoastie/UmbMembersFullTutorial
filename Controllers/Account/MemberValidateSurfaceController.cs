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
    public class MemberValidateSurfaceController : SurfaceController
    {
        [ChildActionOnly]
        [HttpGet]
        [ActionName("MemberValidate")]
        public ActionResult MemberValidateGet(string email, string validateGUID)
        {
            if (email == null || validateGUID == null)
            {
                TempData["Status"] = "Error validating your email address";
                return PartialView("Account/MemberValidate", new MemberValidateModel());
            }

            var memberService = Services.MemberService;
            var member = memberService.GetByEmail(email);

            if (member.GetValue("validateGUID").ToString().ToLower() == validateGUID && member.GetValue<DateTime>("validateGUIDExpiry") > DateTime.Now)
            {
                member.IsApproved = true;
                // expire the guid
                member.SetValue("validateGUIDExpiry", DateTime.Now.AddDays(-1));
                memberService.Save(member);
                TempData["Status"] = "Your account has now been validated - you can now login.";
                return PartialView("Account/MemberValidate", new MemberValidateModel());
            }
            else
            {
                TempData["Status"] = "Sorry - we can't seem to validate your email address, please try using the forgotten password function to reset your account.";
                return PartialView("Account/MemberValidate", new MemberValidateModel());
            }
        }
    }
}