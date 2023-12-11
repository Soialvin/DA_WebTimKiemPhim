using DAM.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DAM.Controllers
{
    public class TimKiemController : BaseController
    {
        // GET: TimKiem
        DA_WebTimKiemPhimEntities db = new DA_WebTimKiemPhimEntities();
        [HttpGet]
        public ActionResult KQTimKiem(string TuKhoa, int? page)
        {
            if (Request.HttpMethod != "GET")
            {
                page = 1;
            }
            ViewBag.TuKhoa = TuKhoa;
            var listPhim = db.Phims.Where(x => x.TenPhim.Contains(TuKhoa) || x.ThongTinPhim.Contains(TuKhoa));
            int PageSize = 18;
            int PageNumber = (page ?? 1);
            return View(listPhim.OrderBy(x => x.TenPhim).ToPagedList(PageNumber, PageSize));
        }
        [HttpPost]
        public ActionResult LayTuKhoaTimKiem(string TuKhoa)
        {
            return RedirectToAction("KQTimKiem", new {@TuKhoa = TuKhoa});
        }
    }
}