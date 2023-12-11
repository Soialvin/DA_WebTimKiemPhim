using DAM.Models;
using DAM.Models.Salt_MH;
using DAM.Models.ViewModel;
using PagedList;
using System;
using System.Collections.Generic;
using System.IO;
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
        [HttpGet]
        public ActionResult TTTaiKhoan(string TenTK)
        {
            if (TenTK == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            var tk = db.TaiKhoans.SingleOrDefault(x => x.TenTK == TenTK);
            Session["Anh"] = tk;
            if (tk == null)
            {
                return HttpNotFound();
            }
            var result = new TKViewModel
            {
                TenTK = tk.TenTK,
                MatKhau = tk.MatKhau,
                HoVaTen = tk.HoVaTen,
                HinhAnh = tk.HinhAnh,
                DiaChi = tk.DiaChi,
                SoDienThoai = tk.SoDienThoai,
                Email = tk.Email,
            };

            return View(result);
        }
        [HttpPost]
        public ActionResult TTTaiKhoan(TKViewModel tk, HttpPostedFileBase HinhAnh)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var x = db.TaiKhoans.Find(tk.TenTK);
                    if (x == null)
                    {
                        return HttpNotFound();
                    }
                    if (HinhAnh != null && HinhAnh.ContentLength != 0)
                    {
                        //lấy tên ảnh
                        var fileName = Path.GetFileName(HinhAnh.FileName);
                        //Chuyển ảnh vào thư mục
                        var path1 = Path.Combine(Server.MapPath("~/Content/TKImages"), fileName);
                        if (System.IO.File.Exists(path1))
                        {
                            ViewBag.upload = "Hình đã tồn tại";
                            return View(tk);
                        }
                        else
                        {
                            var anh = Session["Anh"] as TaiKhoan;
                            if (anh.HinhAnh != null)
                            {
                                var path2 = Path.Combine(Server.MapPath("~/Content/TKImages"), anh.HinhAnh);
                                if (System.IO.File.Exists(path2))
                                {
                                    System.IO.File.Delete(path2);
                                }
                            }
                            HinhAnh.SaveAs(path1);
                            Session["TenHinh"] = HinhAnh.FileName;
                            ViewBag.TenHinh = "";
                            tk.HinhAnh = fileName;
                        }
                    }
                    else
                    {
                        var anh = (TaiKhoan)Session["Anh"];
                        tk.HinhAnh = anh.HinhAnh;
                    }
                    var TKDN = (TaiKhoan)Session["user"];
                    if (TKDN != null)
                    {
                        if (TKDN.TenTK == tk.TenTK)
                        {
                            Session["user"] = tk;
                        }
                    }
                    var httpContext = ControllerContext.HttpContext;
                    var MKGoc = httpContext.Request.Form["MKGoc"];
                    if (string.IsNullOrEmpty(MKGoc) || tk.MatKhau != MKGoc)
                    {
                        if (tk.MatKhau.Length > 15 || tk.MatKhau.Length < 5)
                        {
                            ViewBag.errMK = "Mật khẩu không quá 15 kí tự, tối thiểu là 5 kí tự";
                            return View(tk);
                        }
                        if (tk.MatKhau.Contains(" "))
                        {
                            ViewBag.errMK = "Mật khẩu không chứa khoảng trắng";
                            return View(tk);
                        }
                        //tk.MatKhau = BCryptNet.HashPassword(tk.MatKhau);
                        MH_GM mh = new MH_GM();
                        tk.MatKhau = mh.MH(tk.MatKhau);
                    }
                    x.MatKhau = tk.MatKhau;
                    x.HoVaTen = tk.HoVaTen;
                    x.HinhAnh = tk.HinhAnh;
                    x.DiaChi = tk.DiaChi;
                    x.SoDienThoai = tk.SoDienThoai;
                    x.Email = tk.Email;
                    db.SaveChanges();
                    Session["user"] = x;
                    Session.Remove("Anh");
                    return Json(new { success = true });
                }
                return Json(new { success = false });
            }
            catch (Exception ex)
            {
                if (Session["user"] != null)
                {
                    TaiKhoan TKDN = (TaiKhoan)Session["user"];
                    LichSu ls = new LichSu
                    {
                        ThongTinTT = $"Error cập nhật tài khoản của khách hàng. Mã lỗi: {ex.Message}",
                        NgayGioTT = DateTime.Now,
                        TenTK = TKDN.TenTK
                    };
                    db.LichSus.Add(ls);
                }
                db.SaveChanges();
                Response.StatusCode = 404;
                return null;
            }

        }
    }
}