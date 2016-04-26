using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Umbraco.Web;
using Umbraco.Web.Models;

namespace UmbracoMembers.Models
{
    public class MemberResetPasswordSetModel : MemberResetPasswordModel
    {
        [Required]
        [StringLength(36, MinimumLength = 8, ErrorMessage = "The password must be between 8 and 36 characters long.")]
    //    [RegularExpression(@"^(?=.{8})(?=.*[a-z])(?=.*[A-Z])(?=.*\d).+$", ErrorMessage = "The password must be between 8 and 36 characters long and contain at least one special character")]
        public string PasswordNew { get; set; }
        [Required]
        [System.ComponentModel.DataAnnotations.Compare("PasswordNew", ErrorMessage = "Passwords do not match")]
        public string PasswordNew2 { get; set; }
    }

}