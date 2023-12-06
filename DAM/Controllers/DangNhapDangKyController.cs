using DAM.Models;
using DAM.Models.Salt_MH;
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
        public ActionResult DangNhap(string user, string password, TaiKhoan tk)
        {
            //Checkdb

            var TKKH = db.TaiKhoans.SingleOrDefault(m => m.TenTK.ToLower() == user.ToLower());
            MH_GM mh = new MH_GM();
            //check code
            if (TKKH != null && TKKH.MatKhau == mh.MH(password))
            {
                Session["user"] = TKKH;
                var tk1 = TKKH.LoaiTK;
                if (tk1 == "User")
                {
                    return RedirectToAction("Main", "Main");
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
        public ActionResult DangKy(TaiKhoan tk)
        {
            if (ModelState.IsValid)
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
                tk.LoaiTK = "User";
                tk.TrangThai = "Đang hoạt động";
                tk.NgayDK = DateTime.Now;
                tk.HinhAnh = "systemusers_104569.png";
                // Mã hóa mật khẩu trước khi lưu vào cơ sở dữ liệu  
                //tk.MatKhau = BCryptNet.HashPassword(tk.MatKhau);
                MH_GM mh = new MH_GM();
                tk.MatKhau = mh.MH(tk.MatKhau);
                db.TaiKhoans.Add(tk);
                db.SaveChanges();
                return RedirectToAction("Dangnhap", "DangNhapDangKy");
            }
            return View(tk);
        }
    }
}