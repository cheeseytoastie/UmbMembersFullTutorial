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
using UmbracoMembers.Helpers;


namespace UmbracoMembers.Controllers
{
    public class MemberRegisterSurfaceController : SurfaceController
    {
        [ChildActionOnly]
        [ActionName("MemberRegisterRenderForm")]
        public ActionResult MemberRegisterRenderForm()
        {
            return PartialView("Account/MemberRegister", new MemberRegisterModel());
        }

        [HttpPost]
        [ActionName("MemberRegister")]
        public ActionResult MemberRegisterForm(MemberRegisterModel model)
        {
            if (ModelState.IsValid)
            {
                var memberService = Services.MemberService;

                // Umbraco now uses the email as the members username so we only need to check this
                if (memberService.GetByEmail(model.Email) != null)
                {
                    TempData["Status"] = "Email is already in use - if you've forgotten your password or lost the account validation email please use the reset password functionality to recover your account.";
                    return CurrentUmbracoPage();
                }

                // create a GUID for the account validation
                string newUserGuid = System.Guid.NewGuid().ToString("N");

                // create the member using the MemberService
                var member = memberService.CreateMember(model.Email, model.Email, model.FirstName + " " + model.LastName, "Member");

                // set custom member data properties and set to NOT approved - careful with the alias case on your custom
                // properties! e.g. firstname won't work with this code - the Umbraco backoffice changes in v.7.4.2+ mean you can't set your own
                // alias - if you've "upgrading" following earlier tutorials note the case difference!

                // Set custom properties - we should check our custom properties exist first
                // if (member.HasProperty("firstName"))  - we'll let it bomb instead for learning / setup.
                member.SetValue("firstName", model.FirstName);
                member.SetValue("lastName", model.LastName);
                member.SetValue("validateGUID", newUserGuid);
                // set the expiry to be 24 hours.
                member.SetValue("validateGUIDExpiry", DateTime.Now.AddDays(1));
                member.SetValue("mailingListInclude", model.MailingListInclude);
                member.IsApproved = false;

                // remember to save
                memberService.Save(member);

                // save their password
                memberService.SavePassword(member, model.Password);

                // add to the WebsiteRegistrations group (this is so we can differentiate website members from any others you might have.)
                memberService.AssignRole(member.Id, "WebsiteRegistrations");

                // Set up the info for the validation email
                Dictionary<string, string> emailFields = new Dictionary<string, string>
                {
                    {"FIRSTNAME", model.FirstName},
                    {"LASTNAME", model.LastName},
                    {"EMAIL", model.Email},
                    {"VALIDATEGUID", newUserGuid},
                    {"DOMAIN", HttpContext.Request.Url.Authority}
                };

                // Send the validation email
                bool emailSent = EmailHelper.SendEmail("Validate Registration Email", "info@domain.com", model.Email, emailFields);

                // for testing I'm returning my guid to test the next bit without an email (e.g. copying and pasting into browser)
                // string host = HttpContext.Request.Url.Authority;
                // string validateURL = "http://" + host + "/account/validate?email=" + model.Email + "&guid=" + newUserGuid;
                // ONLY FOR TESTING - it outputs the validate link to the webpage which defeats the object! 
                //TempData["Status"] = "Member created! Test validate url = <a href=\"" + validateURL + "\">" + validateURL + "</a>";

                TempData["Success"] = emailSent;
                TempData["Status"] = "Your account has been created, before logging in please check your email and click on the list to valdiate your account and complete the registration process. ";
                return RedirectToCurrentUmbracoPage();
            }
            else
            {
                // model is invalid
                TempData["Status"] = "Please ensure you've provided all the required information.";
                return CurrentUmbracoPage();
            }
        }
    }
}