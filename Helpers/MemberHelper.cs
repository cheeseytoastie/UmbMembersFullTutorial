using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Umbraco;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Core.Services;
using Umbraco.Web.Mvc;

namespace UmbracoMembers.Helpers
{
    public class MemberHelper
    {
        public static bool EmailIsInUse(string email)
        {
            var memberService = ApplicationContext.Current.Services.MemberService;

            // See if there is a member already using the entered email address.
            var member = memberService.GetByEmail(email);

            if (member != null)
            {
                return true;
            }

            return false;
        }
    }
}