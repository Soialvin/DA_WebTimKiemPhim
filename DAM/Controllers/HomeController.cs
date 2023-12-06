using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DAM.Controllers
{
    public class HomeController : BaseController
    {
        // GET: Home
        public ActionResult Home()
        {
            return View();
        }
    }
}