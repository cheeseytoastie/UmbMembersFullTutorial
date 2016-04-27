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
            var memberService = Services.MemberService;
            var model = new MemberRegisterModel();
            var curMember = memberService.GetById(Members.GetCurrentMemberId());

            model.FirstName = curMember.GetValue<string>("firstname");
            model.LastName = curMember.GetValue<string>("lastname");
            model.MailingListInclude = curMember.GetValue<bool>("mailingListInclude");  // this is not doing anything - replace with your mailchimp integration!

            return PartialView("Account/MemberEditProfile", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken()]
        [ActionName("MemberEditProfile")]
        public ActionResult MemberEditProfile(MemberEditProfileModel model)
        {
            if (ModelState.IsValid)
            {
                var memberService = Services.MemberService;
                var member = Members.GetCurrentMember();

                // Get an editable curMember
                var curMember = memberService.GetById(member.Id);

                // save their password if they provide an update
                if (!(string.IsNullOrEmpty(model.Password)) && model.Password == model.Password2)
                {

                    if (model.Password.Length >= 8 && model.Password.Length <= 36)
                    {
                        memberService.SavePassword(curMember, model.Password);
                    }
                    else
                    {
                        TempData["Status"] = "Your new password must be between 8 and 36 characters long";
                        return RedirectToCurrentUmbracoPage();
                    }
                }

                curMember.SetValue("firstName", model.FirstName);
                curMember.SetValue("lastName", model.LastName);
                
                // if you were to change the members email address you'd want to validate it first
                // TODO

                // here you should do your Mailchimp integration / or whatever
                curMember.SetValue("mailingListInclude", model.MailingListInclude);

                // remember to save
                memberService.Save(curMember);

                @TempData["Success"] = true;
                TempData["Status"] = "Your account has been updated";
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