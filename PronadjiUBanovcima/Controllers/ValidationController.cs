using PronadjiUBanovcima.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PronadjiUBanovcima.Controllers
{
    public class ValidationController : Controller
    {
        //
        // GET: /Validation/
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public JsonResult DoesUserNameExist(string UserName)
        {
            ApplicationDbContext context = new ApplicationDbContext();
            ApplicationUser user = context.Users.SingleOrDefault(u => u.UserName == UserName);

            return Json(user == null);
        }
        [HttpPost]
        [AllowAnonymous]
        public JsonResult DoesEmailExist(string Email)
        {
            ApplicationDbContext context = new ApplicationDbContext();
            ApplicationUser user = context.Users.SingleOrDefault(u => u.Email == Email);

            return Json(user == null);
        }
	}
}