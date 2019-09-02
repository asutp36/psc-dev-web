using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.Mvc;

using Inspinia_MVC5.Models;
using Inspinia_MVC5.ViewModels;

namespace Inspinia_MVC5.Controllers
{
    public class PagesController : Controller
    {

        public ActionResult SearchResults()
        {
            return View();
        }

        public ActionResult LockScreen()
        {
            return View();
        }

        public ActionResult Invoice()
        {
            return View();
        }

        public ActionResult InvoicePrint()
        {
            return View();
        }

        public ActionResult Login()
        {
            return View();
        }

        public ActionResult Login_2()
        {
            return View();
        }

        public ActionResult Register()
        {
            return View();
        }

        public ActionResult NotFoundError()
        {
            return View();
        }

        public ActionResult InternalServerError()
        {
            return View();
        }

        public ActionResult EmptyPage()
        {
            return View();
        }

        public ActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login_2(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                // Поиск пользователя в БД
                User user = null;
                using (ModelDb db = new ModelDb())
                {
                    user = db.Users.FirstOrDefault(u => u.Login == model.Name && u.Password == model.Password);
                }

                if (user != null)
                {
                    FormsAuthentication.SetAuthCookie(model.Name, true);

                    return RedirectToAction("IncashWashes", "Incash");
                }
                else
                {
                    ModelState.AddModelError("", "No user");
                }
            }

            return View(model);
        }

        public ActionResult Logoff()
        {
            FormsAuthentication.SignOut();

            return View("Login_2");
        }
    }
}