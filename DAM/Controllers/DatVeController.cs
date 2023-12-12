using DAM.Models;
using DAM.Models.ViewModel;
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
            result = result.Where(x => (x.TenTK != null && x.TenTK.Contains(keyword))).ToList();
            ViewBag.Keyword = keyword;
            return View("Index", result);
        }
        [HttpPost]
        public ActionResult XuLyHD(HoaDonViewModel model)
        {
            List<string> listPPThanhToan = new List<string> { "Thanh toán qua E-Banking", "Thanh toán qua MoMo", "Thanh toán qua VnPay" };
            ViewBag.PPThanhToan = new SelectList(listPPThanhToan);
            if (Session["user"] != null)
            {
                TaiKhoan TKDN = (TaiKhoan)Session["user"];
                var v = new Ve
                {
                    MaSC = model.MaSC,
                    MaPhim = model.MaPhim,
                    MaRap = model.MaRap,
                    NgayTao = DateTime.Now,
                    TrangThaiVe = "Chưa xóa"
                };
                db.Ves.Add(v);
                foreach (var item in model.MaGhe)
                {
                    var g_v = new Ghe_Ve
                    {
                        MaVe = v.MaVe,
                        MaGhe = item
                    };
                    db.Ghe_Ve.Add(g_v);
                }
                var hd = new HoaDon
                {
                    MaVe = v.MaVe,
                    TenTK = TKDN.TenTK,
                    NgayDat = DateTime.Now,
                    TongTien = model.TongTien,
                    PPThanhToan = model.PPThanhToan,
                    TrangThai = "Đã thanh toán"
                };
                db.HoaDons.Add(hd);
                try
                {
                    db.SaveChanges();
                    return Json(new { success = true });
                }
                catch (Exception)
                {
                    return Json(new { success = false });
                }
            }
            else
            {
                Response.StatusCode = 404;
                return null;
            }
        }
    }
}