using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DAM.Controllers
{
    public class YeuThichController : Controller
    {
        // GET: YeuThich
        /*public ActionResult YeuThichTrue(string MaPhim)
        {
            var MaPhimLayVe = MaPhim;
            return View();
        }*/
        public ActionResult YeuThichFalse(string MaPhim)
        {
            var MaPhimLayVe = MaPhim;
            return View();
        }
    }
}