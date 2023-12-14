using CaptchaMvc.HtmlHelpers;
using DAM.Models;
using DAM.Models.Salt_MH;
using DAM.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using BCryptNet = BCrypt.Net.BCrypt;

namespace DAM.Controllers
{
    public class DangNhapDangKyController : BaseController
    {
        // GET: Log
        DA_WebTimKiemPhimEntities db = new DA_WebTimKiemPhimEntities();
        // GET: Admin
        public ActionResult DangNhap()
        {
            return View();
        }
        [HttpPost]
        public ActionResult DangNhap(string user, string password)
        {
            //Checkdb

            var TKKH = db.TaiKhoans.SingleOrDefault(m => m.TenTK.ToLower() == user.ToLower() && m.TrangThai == "Đang hoạt động");
            MH_GM mh = new MH_GM();
            //check code
            if (TKKH != null && TKKH.MatKhau == mh.MH(password))
            {
                Session["user"] = TKKH;
                var tk1 = TKKH.LoaiTK;
                if (tk1 == "User")
                {
                    if(Session["DatVeCDN"] == null)
                    {
                        return RedirectToAction("Main", "Main");
                    }
                    else
                    {
                        return RedirectToAction("TaoMoiCDN", "Ve");
                    }
                }
                return RedirectToAction("Index", "ThongKe");

            }
            else
            {
                TempData["error"] = "Tài khoản đăng nhập không đúng";
                return View();
            }
        }

        public ActionResult DangXuat()
        {
            //Xóa session
            Session.Remove("user");
            //xoa session form
            FormsAuthentication.SignOut();

            return RedirectToAction("DangNhap");

        }

        public ActionResult DangXuatKH()
        {
            //Xóa session
            Session.Remove("user");
            //xoa session form
            FormsAuthentication.SignOut();

            return RedirectToAction("Main","Main");

        }

        //Dăng ký

        [HttpGet]
        public ActionResult DangKy()
        {
            return View();
        }
        [HttpPost]
        public ActionResult DangKy(TKViewModel tk)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if ((tk.MatKhau).Length > 15 || (tk.MatKhau).Length < 5)
                    {
                        ViewBag.loimk = "Mật khẩu tối đa 15 kí tự, tối thiểu là 5 kí tự";
                        return View(tk);
                    }
                    if ((tk.MatKhau).Contains(" "))
                    {
                        ViewBag.loimk = "Mật khẩu không chứa khoảng trắng";
                        return View(tk);
                    }
                    if (!this.IsCaptchaValid("captcha sai"))
                    {
                        ViewBag.thongbao = "Sai mã captcha";
                        return View(tk);
                    }
                    
                    tk.LoaiTK = "User";
                    tk.TrangThai = "Đang hoạt động";
                    tk.NgayDK = DateTime.Now;
                    tk.HinhAnh = "systemusers_104569.png";
                    // Mã hóa mật khẩu trước khi lưu vào cơ sở dữ liệu  
                    //tk.MatKhau = BCryptNet.HashPassword(tk.MatKhau);
                    MH_GM mh = new MH_GM();
                    tk.MatKhau = mh.MH(tk.MatKhau);
                    TaiKhoan tkkh = new TaiKhoan
                    {
                        TenTK =tk.TenTK,
                        HoVaTen = tk.HoVaTen,
                        MatKhau = tk.MatKhau,
                        HinhAnh =tk.HinhAnh,
                        DiaChi = tk.DiaChi,
                        SoDienThoai = tk.SoDienThoai,
                        Email = tk.Email,
                        LoaiTK = tk.LoaiTK,
                        NgayDK = tk.NgayDK,
                        TrangThai = tk.TrangThai
                    };
                    db.TaiKhoans.Add(tkkh);
                    db.SaveChanges();
                    SetAlert("Đăng ký thành công", "sucsess");
                    return RedirectToAction("Dangnhap", "DangNhapDangKy");
                }
                catch (Exception)
                {
                    SetAlert($"Đăng ký không thành công", "danger");
                    return View(tk);
                }
            }
            return View(tk);
        }
    }
}