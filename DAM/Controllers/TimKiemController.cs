using DAM.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace DAM.Controllers
{
    public class TimKiemController : BaseController
    {
        // GET: TimKiem
        DA_WebTimKiemPhimEntities db = new DA_WebTimKiemPhimEntities();
        private string RemoveDiacritics(string input)
        {
            return new string(input
                .Normalize(NormalizationForm.FormD)
                .ToCharArray()
                .Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                .ToArray());
        }
        [HttpGet]
        public ActionResult KQTimKiem(string TuKhoa, int? page)
        {
            if (Request.HttpMethod != "GET")
            {
                page = 1;
            }
            ViewBag.TuKhoa = TuKhoa;
            var listPhim = db.Phims.Where(x => RemoveDiacritics(x.TenPhim).ToLower().Contains(RemoveDiacritics(TuKhoa).ToLower()) || RemoveDiacritics(x.ThongTinPhim).ToLower().Contains(RemoveDiacritics(TuKhoa).ToLower())).ToList();
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