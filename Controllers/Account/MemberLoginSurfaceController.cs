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
    public class MemberLoginSurfaceController : SurfaceController
    {
        // change me if you're not going to use path "/account/login" - this stops a redirect loop if the user hits /account/login directly
        string membersLoginUrl = "/account/login/";

        // Handles the login
        [ChildActionOnly]
        [ActionName("MemberLoginRenderForm")]
        public ActionResult MemberLoginRenderForm()
        {
            MemberLoginModel model = new MemberLoginModel();

            string checkUrl = HttpContext.Request.Url.AbsolutePath.ToString();

            // add a trailing / if there isn't one (you can access the same page via http://mydomain.com/login or http://mydomain.com/login/)
            if (checkUrl[checkUrl.Length - 1] != '/')
            {
                checkUrl = checkUrl + "/";
            }

            // if we don't have a session variable and have a request URL then store it
            // we have to store it because if user tries an incorrect login then Current.Request.Url will show /umbraco/RenderMvc 
            // in MVC we won't have "/umbraco/RenderMvc" but I leave this in here just in case
            if (HttpContext.Request.Url != null && HttpContext.Request.Url.AbsolutePath.ToString() != "/umbraco/RenderMvc" && HttpContext.Session["redirectURL"] == null)
            {
                if (checkUrl.ToLower() != membersLoginUrl)
                {
                    HttpContext.Session["redirectURL"] = HttpContext.Request.Url.ToString();
                }
            }

            // set this to be checked by default - wish you could just pass checked=checked
            model.RememberMe = true;
            return PartialView("Account/MemberLogin", model);
        }

        // The MemberLoginPost Action checks the entered credentials using the member Umbraco stuff and redirects the user to the same page. Either as logged in, or with a message set in the TempData dictionary:
        [HttpPost]
        [ValidateAntiForgeryToken()]
        [ActionName("MemberLogin")]
        public ActionResult MemberLoginPost(MemberLoginModel model)
        {
            if (ModelState.IsValid)
            {
                var memberService = Services.MemberService;
                var member = memberService.GetByEmail(model.Email);

                if (member != null && model.Password != null)
                {
                    if (!member.IsApproved)
                    {
                        TempData["Status"] = "Before you can login you need to validate your email address - check your email for instructions on how to do this, if you can't find this email use the forgotten password function to receive a new email.";
                        return RedirectToCurrentUmbracoPage();
                    }

                    // helper method on Members to login
                    if (Members.Login(model.Email, model.Password))
                    {
                        return RedirectToCurrentUmbracoPage();
                    }
                    else
                    {
                        TempData["Status"] = "Invalid username or password";
                        return CurrentUmbracoPage();
                    }
                }
                else
                {
                    TempData["Status"] = "Invalid username or password";
                    return CurrentUmbracoPage();
                }
            }
            else
            {
                // model is invalid
                TempData["Status"] = "Invalid username or password";
                return CurrentUmbracoPage();
            }
        }
    }
}
