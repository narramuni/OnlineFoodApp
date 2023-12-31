    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;

    namespace OnlineFood.Controllers
    {
        public class HomeController : Controller
        {
            OnlineFoodDeliveryAPPDBEntities1 dbContext = new OnlineFoodDeliveryAPPDBEntities1();
            // GET: Home
            public ActionResult Index()
            {
                return View();
            }

            public ActionResult AdminLogin()
            {
                return View();
            }

            [HttpPost]
            public ActionResult AdminLogin(string adminUsername, string adminPassword)
            {
                var admin = dbContext.Admins
                    .FirstOrDefault(a => a.AdminUsername == adminUsername && a.AdminPassword == adminPassword);

                if (admin != null)
                {
                    // Admin authentication successful

                    // Store admin information in session or cookie
                    Session["AdminId"] = admin.AdminId;
                    Session["AdminUsername"] = admin.AdminUsername;

                    return RedirectToAction("Index", "Admin");
                }
                else
                {
                    // Admin authentication failed
                    ModelState.AddModelError("", "Invalid login attempt");
                    return View();
                }
            }
        }
    }
