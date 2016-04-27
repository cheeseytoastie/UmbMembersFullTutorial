using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Umbraco.Web;
using Umbraco.Web.Models;

namespace UmbracoMembers.Models
{
    public class MemberEditProfileModel
    {
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        public bool MailingListInclude { get; set; }
        [Required(AllowEmptyStrings = true)]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        // [StringLength(36, MinimumLength = 8, ErrorMessage = "The password must be between 8 and 36 characters long.")]
        //    [RegularExpression(@"^(?=.{8})(?=.*[a-z])(?=.*[A-Z])(?=.*\d).+$", ErrorMessage = "The password must be between 8 and 36 characters long and contain at least one special character")]
        public string Password { get; set; }
        [Required(AllowEmptyStrings = true)]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "Passwords do not match")]
        public string Password2 { get; set; }
    }
}