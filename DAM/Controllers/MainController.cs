using DAM.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DAM.Controllers
{
    public class MainController : BaseController
    {
        // GET: Main
        DA_WebTimKiemPhimEntities db = new DA_WebTimKiemPhimEntities();
        public ActionResult Main()
        {
            return View();
        }
        public ActionResult MenuTheLoaiPartial()
        {
            var listTheLoai = db.TheLoais;
            return PartialView(listTheLoai);
        }
        //Thông tin chi tiết
        public ActionResult PhimDangChieu_TT_Partial()
        {
            var listPhimDanngChieu_TT = db.Phims.Where(x => x.TrangThai == "Đang chiếu");
            return PartialView(listPhimDanngChieu_TT);
        }
        public ActionResult MenuTheLoai_TT_Partial()
        {
            var listTheLoai = db.TheLoais;
            return PartialView(listTheLoai);
        }
    }
}