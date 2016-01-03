using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ActionMailerNext;
using ActionMailerNext.Mvc5;
using PronadjiUBanovcima.Models;
using System.Net.Mail;

namespace PronadjiUBanovcima.Controllers
{
    public class MailController : MailerBase
    {
        public EmailResult VerificationEmail(ApplicationUser model)
        {

            //SetMailMethod(MailMethod.SMTP);

            MailAttributes.From = new System.Net.Mail.MailAddress("npetrovic789@yahoo.com", "Shtrik IT");
            MailAttributes.To.Add(new System.Net.Mail.MailAddress(model.Email));
            MailAttributes.Subject = "Account Confirmation";
            MailAttributes.Priority = MailPriority.High;

            return Email("EmailConfirmationView", model);


        }
        //
        // GET: /Mail/
        public ActionResult Index()
        {
            return View();
        }
	}
}