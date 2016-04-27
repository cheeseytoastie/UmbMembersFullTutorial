using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Umbraco.Web.Mvc;
using UmbracoMembers.Helpers;
using UmbracoMembers.Models;


namespace UmbracoMembers.Controllers
{
    public class MemberResetPasswordSurfaceController : SurfaceController
    {
        [ChildActionOnly]
        [ActionName("MemberResetPasswordRenderForm")]
        public ActionResult MemberResetPasswordRenderForm()
        {
            var model = new MemberResetPasswordModel();
            model.ValidateGUID = "dummy"; // dummy 

            return PartialView("Account/MemberResetPassword", model);
        }

        // The MemberResetPasswordPost Action checks the email address supplied, if found sets a guid, expiry time and sends an email link
        [HttpPost]
        [ValidateAntiForgeryToken()]
        [ActionName("MemberResetPassword")]
        public ActionResult MemberResetPasswordPost(MemberResetPasswordModel model)
        {
            if (ModelState.IsValid)
            {
                var memberService = Services.MemberService;
                var member = memberService.GetByEmail(model.Email);

                if (member != null)
                {
                    string validateGuid = System.Guid.NewGuid().ToString("N");

                    // set the expiry to be 24 hours.
                    member.SetValue("validateGUID", validateGuid);
                    member.SetValue("validateGUIDExpiry", DateTime.Now.AddHours(2));

                    // remember to save
                    memberService.Save(member);

                    // Set up the info for the valdiation email
                    Dictionary<string, string> emailFields = new Dictionary<string, string>
                    {
                        {"FIRSTNAME", member.GetValue<string>("firstName")},
                        {"LASTNAME", member.GetValue<string>("lastName")},
                        {"EMAIL", model.Email},
                        {"VALIDATEGUID", validateGuid},
                        {"DOMAIN", HttpContext.Request.Url.Authority}
                    };

                    // Send the password reset email
                    bool emailSent = EmailHelper.SendEmail("Password Reset Email", "info@domain.com", model.Email, emailFields);

                    TempData["Status"] = "A password reset email has been sent to the email address.";
                    return CurrentUmbracoPage();

                }
                else
                {
                    // Security decision here - you can either inform the user the email address supplied is not valid or pretend it is (to mask wether there is an account).
                    TempData["Status"] = "A password reset email has been sent to the email address.";
                    return CurrentUmbracoPage();
                }
            }
            else
            {
                // model is invalid
                TempData["Status"] = "Invalid email address supplied.";
                return CurrentUmbracoPage();
            }
        }


        [ChildActionOnly]
        [ActionName("MemberResetPasswordSetRenderForm")]
        public ActionResult MemberResetPasswordSetRenderForm()
        {
            var model = new MemberResetPasswordSetModel();
            model.Email = Request.QueryString["email"] ?? string.Empty;
            model.ValidateGUID = Request.QueryString["validateGUID"] ?? string.Empty;
            var memberService = Services.MemberService;
            var member = memberService.GetByEmail(model.Email);

            if (member != null)
            {
                string resetPasswordGuid = member.GetValue<string>("validateGUID");
                DateTime resetPasswordGuidExpiry = member.GetValue<DateTime>("validateGUIDExpiry");

                if (model.ValidateGUID != String.Empty && resetPasswordGuid == model.ValidateGUID && DateTime.Now < resetPasswordGuidExpiry && model.PasswordNew == model.PasswordNew2)
                {
                    TempData["Success"] = "False";
                    return PartialView("Account/MemberResetPasswordSet", model);
                }
                else
                {
                    TempData["Status"] = "Your password link is invalid - please request a new link.";
                    return PartialView("Account/MemberResetPassword", model);
                }
            } 
            else
            {
                TempData["Status"] = "Your password link is invalid - please request a new link.";
                return PartialView("Account/MemberResetPassword", model);
            }
        }


        // The MemberResetPasswordSetPost Action sets the new password
        [HttpPost]
        [ValidateAntiForgeryToken()]
        [ActionName("MemberResetPasswordSet")]
        public ActionResult MemberResetPasswordSetPost(MemberResetPasswordSetModel model)
        {
            if (ModelState.IsValid)
            {
                var memberService = Services.MemberService;
                var member = memberService.GetByEmail(model.Email);

                if (member != null)
                {
                    string resetPasswordGuid = member.GetValue<string>("validateGUID");
                    DateTime resetPasswordGuidExpiry = member.GetValue<DateTime>("validateGUIDExpiry");

                    if (model.ValidateGUID != String.Empty && resetPasswordGuid == model.ValidateGUID && DateTime.Now < resetPasswordGuidExpiry && model.PasswordNew == model.PasswordNew2)
                    {
                        member.IsLockedOut = false;  // if the user has tried repeatedly they might have locked their account
                        member.SetValue("validateGUIDExpiry", DateTime.Now.AddHours(-1));
                        
                        // remember to save
                        memberService.Save(member);

                        // save their password
                        memberService.SavePassword(member, model.PasswordNew);

                        TempData["Success"] = "True";
                        TempData["Status"] = "Your password has been changed - you can now login.";
                        return CurrentUmbracoPage();
                    }
                    else
                    {
                        TempData["Status"] = "Your link has expired - please try requesting the new password again.";
                        return CurrentUmbracoPage();
                    }
                }
                else
                {
                    // model is invalid
                    TempData["Status"] = "Invalid information supplied.";
                    return CurrentUmbracoPage();
                }
            }
            else
            {
                // model is invalid
                TempData["Status"] = "Invalid information supplied.";
                return CurrentUmbracoPage();
            }
        }
    }
}