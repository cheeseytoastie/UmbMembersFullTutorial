using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Umbraco.Web;
using Umbraco.Web.Models;

namespace UmbracoMembers.Models
{
    public class MemberValidateModel
    {
        public string Email { get; set; }
        public string ValidateGUID { get; set; }
        public DateTime ValidateGUIDExpiry { get; set; }
    }

}