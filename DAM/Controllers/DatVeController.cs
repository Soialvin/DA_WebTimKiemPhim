using DAM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DAM.Controllers
{
    public class DatVeController : BaseController
    {
        DA_WebTimKiemPhimEntities db = new DA_WebTimKiemPhimEntities();
        // GET: DatVe
        public ActionResult Index()
        {
            return View(db.HoaDons);
        }
        [HttpGet]
        public ActionResult Sua()
        {
            List<string> listLoaiHoaDon = new List<string> { "Chưa thanh toán", "Đã thanh toán","Đã xóa" };
            ViewBag.TrangThai = new SelectList(listLoaiHoaDon);
            return View();
        }
        [HttpPost]
        public ActionResult Sua(HoaDon hd)
        {
            List<string> listLoaiHoaDon = new List<string> { "Chưa thanh toán", "Đã thanh toán", "Đã xóa" };
            ViewBag.TrangThai = new SelectList(listLoaiHoaDon);

            try
            {
                if (ModelState.IsValid)
                {
                    db.Entry(hd).State = System.Data.Entity.EntityState.Modified;
                    if (Session["user"] != null)
                    {
                        TaiKhoan TKDN = (TaiKhoan)Session["user"];
                        LichSu ls = new LichSu
                        {
                            ThongTinTT = $"Đã cập nhật hóa đơn mã: {hd.MaHD}",
                            NgayGioTT = DateTime.Now,
                            TenTK = TKDN.TenTK
                        };
                        db.LichSus.Add(ls);
                    }
                    db.SaveChanges();
                    SetAlert("Cập nhật thành công", "sucsess");
                    return RedirectToAction("Index");

                }
                return View(hd);
            }
            catch (Exception ex)
            {
                if (Session["user"] != null)
                {
                    TaiKhoan TKDN = (TaiKhoan)Session["user"];
                    LichSu ls = new LichSu
                    {
                        ThongTinTT = $"Error cập nhật hóa đơn. Mã lỗi: {ex.Message}",
                        NgayGioTT = DateTime.Now,
                        TenTK = TKDN.TenTK
                    };
                    db.LichSus.Add(ls);
                }
                db.SaveChanges();
                SetAlert($"Cập nhật không thành công. Lỗi đã được ghi nhận vui lòng liên hệ Admin", "danger");
                return View(hd);
            }
        }
        public ActionResult Xoa(string maHD)
        {
            var result = db.HoaDons.Find(maHD);
            /*db.HoaDons.Remove(result);*/
            result.TrangThai = "Đã xóa";
            try
            {
                if (Session["user"] != null)
                {
                    TaiKhoan TKDN = (TaiKhoan)Session["user"];
                    LichSu ls = new LichSu
                    {
                        ThongTinTT = $"Đã xóa hóa đơn mã: {result.MaHD}",
                        NgayGioTT = DateTime.Now,
                        TenTK = TKDN.TenTK
                    };
                    db.LichSus.Add(ls);
                }
                db.SaveChanges();
                SetAlert("Xóa thành công", "sucsess");
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                if (Session["user"] != null)
                {
                    TaiKhoan TKDN = (TaiKhoan)Session["user"];
                    LichSu ls = new LichSu
                    {
                        ThongTinTT = $"Error xóa hóa đơn. Mã lỗi: {ex.Message}",
                        NgayGioTT = DateTime.Now,
                        TenTK = TKDN.TenTK
                    };
                    db.LichSus.Add(ls);
                }
                db.SaveChanges();
                SetAlert($"Cập nhật không thành công. Lỗi đã được ghi nhận vui lòng liên hệ Admin", "danger");
                return RedirectToAction("Index");
            }
        }
        public ActionResult TimKiem(string keyword)
        {
            var result = db.HoaDons.ToList();
            if (string.IsNullOrEmpty(keyword))
            {
                return View("Index", result);
            }
            result = result.Where(x => (x.TenTK != null && x.TenTK.Contains(keyword)) ).ToList();
            ViewBag.Keyword = keyword;
            return View("Index", result);
        }
    }
}