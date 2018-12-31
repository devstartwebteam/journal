using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using PapeAuthentication;
using System.Web;

namespace journalentry.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}