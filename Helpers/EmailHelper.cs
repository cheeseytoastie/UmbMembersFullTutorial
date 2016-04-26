using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using Umbraco;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Core.Logging;
using Umbraco.Web;

namespace UmbracoMembers.Helpers
{
    public class EmailHelper
    {

        public static bool SendEmail(string EmailTemplateName, string FromEmail, string ToEmail, Dictionary<string, string> EmailFields)
        {
            // get the email master outer template
            int siteSettingsID = 1074;
            var uh = new Umbraco.Web.UmbracoHelper(Umbraco.Web.UmbracoContext.Current);
            var siteSettingsNode = uh.TypedContent(siteSettingsID);
            string emailMasterTemplate = siteSettingsNode.GetPropertyValue<string>("emailMasterTemplate");
            var emailNode = siteSettingsNode.Children().Where(x => x.Name == EmailTemplateName).First();
            string emailBodyTemplate = emailNode.GetPropertyValue<string>("emailBody");
            string EmailSubject = emailNode.GetPropertyValue<string>("emailSubject");

            string EmailBody = emailMasterTemplate.Replace("[[CONTENT]]", emailBodyTemplate);

            try
            {
                // send email 
                MailMessage message = new MailMessage();

                message.To.Add(ToEmail);
                message.From = new System.Net.Mail.MailAddress(FromEmail, "UMBRACOMEMBERS : WEBSITE");  // Change me :)
                message.Subject = ReplaceFields(EmailSubject, EmailFields);

                message.IsBodyHtml = true;
                message.Body = ReplaceFields(EmailBody, EmailFields);
                SmtpClient smtp = new SmtpClient();
                smtp.Send(message);
                return true;
            }
            catch (Exception e)
            {
                LogHelper.Info(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "Error trying to Send Email" + e);
                return false;
            }
        }

        public static string ReplaceFields(string textIn, Dictionary<string, string> EmailFields)
        {
            string textOut = textIn;

            // replace the email template fields with values
            foreach (var emailField in EmailFields)
            {
                textOut = textOut.Replace(string.Concat("[[", emailField.Key.ToUpper(), "]]"), emailField.Value);
            }

            return textOut;
        }
    }
}