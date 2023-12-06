using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DAM.Models;
using DAM.Models.Salt_MH;
using DAM.Models.ViewModel;
using DocumentFormat.OpenXml.Spreadsheet;
using OfficeOpenXml;
using BCryptNet = BCrypt.Net.BCrypt;

namespace DAM.Controllers
{
    public class QLTaiKhoanController : BaseController
    {
        // GET: QLTaiKhoan
        DA_WebTimKiemPhimEntities db = new DA_WebTimKiemPhimEntities();
        public ActionResult QLTaiKhoan()
        {
            var result = db.TaiKhoans.Select(x => new TKViewModel
            {
                TenTK = x.TenTK,
                MatKhau = x.MatKhau,
                HoVaTen = x.HoVaTen,
                HinhAnh = x.HinhAnh,
                DiaChi = x.DiaChi,
                SoDienThoai = x.SoDienThoai,
                Email = x.Email,
                LoaiTK = x.LoaiTK,
                NgayDK = x.NgayDK,
                TrangThai = x.TrangThai
            }).ToList();
            return View(result);
        }
        [HttpGet]
        public ActionResult TaoMoi()
        {
            List<string> listLoaiTK = new List<string> { "Admin", "User" };
            ViewBag.LoaiTK = new SelectList(listLoaiTK);
            return View();
        }
        [HttpPost]
        public ActionResult TaoMoi(TKViewModel tk, HttpPostedFileBase HinhAnh)
        {
            List<string> listLoaiTK = new List<string> { "Admin", "User" };
            ViewBag.LoaiTK = new SelectList(listLoaiTK);
            if (ModelState.IsValid)
            {
                if (HinhAnh == null || HinhAnh.ContentLength == 0)
                {
                    // Ảnh không được chọn lưu hình mặc định
                    tk.HinhAnh = "systemusers_104569.png";
                }
                else
                {
                    //lấy tên ảnh
                    var fileName = Path.GetFileName(HinhAnh.FileName);
                    //Chuyển ảnh vào thư mục
                    var path = Path.Combine(Server.MapPath("~/Content/TKImages"), fileName);
                    if (System.IO.File.Exists(path))
                    {
                        ViewBag.upload = "Hình đã tồn tại";
                        return View(tk);
                    }
                    else
                    {
                        HinhAnh.SaveAs(path);
                        Session["TenHinh"] = HinhAnh.FileName;
                        ViewBag.TenHinh = "";
                        tk.HinhAnh = fileName;
                    }
                }
                if ((tk.MatKhau).Length > 15 || (tk.MatKhau).Length < 5)
                {
                    ViewBag.MatKhau = "Mật khẩu tối đa 15 kí tự, tối thiểu là 5 kí tự";
                    return View(tk);
                }
                if ((tk.MatKhau).Contains(" "))
                {
                    ViewBag.MatKhau = "Mật khẩu không chứa khoảng trắng";
                    return View(tk);
                }
                //string MH = BCryptNet.HashPassword(tk.MatKhau);
                //tk.MatKhau = MH;
                MH_GM mh = new MH_GM();
                tk.MatKhau = mh.MH(tk.MatKhau);
                tk.NgayDK = DateTime.Now;
                tk.TrangThai = "Đang hoạt động";
                var x = new TaiKhoan
                {
                    TenTK = tk.TenTK,
                    MatKhau = tk.MatKhau,
                    HoVaTen = tk.HoVaTen,
                    HinhAnh = tk.HinhAnh,
                    DiaChi = tk.DiaChi,
                    SoDienThoai = tk.SoDienThoai,
                    Email = tk.Email,
                    LoaiTK = tk.LoaiTK,
                    NgayDK = tk.NgayDK,
                    TrangThai = tk.TrangThai
                };
                db.TaiKhoans.Add(x);
                try
                {
                    if (Session["user"] != null)
                    {
                        TaiKhoan TKDN = (TaiKhoan)Session["user"];
                        LichSu ls = new LichSu
                        {
                            ThongTinTT = $"Đã tạo mới tài khoản: {tk.TenTK}",
                            NgayGioTT = DateTime.Now,
                            TenTK = TKDN.TenTK
                        };
                        db.LichSus.Add(ls);
                    }
                    db.SaveChanges();
                    // Nếu không có lỗi khi lưu dữ liệu, dòng sau sẽ được thực hiện
                    SetAlert("Thêm mới thành công", "sucsess");
                    return RedirectToAction("QLTaiKhoan");
                }
                catch (Exception ex)
                {
                    if (Session["user"] != null)
                    {
                        TaiKhoan TKDN = (TaiKhoan)Session["user"];
                        LichSu ls = new LichSu
                        {
                            ThongTinTT = $"Error tạo mới tài khoản. Mã lỗi: {ex.Message}",
                            NgayGioTT = DateTime.Now,
                            TenTK = TKDN.TenTK
                        };
                        db.LichSus.Add(ls);
                    }
                    db.SaveChanges();
                    // Nếu có lỗi khi lưu dữ liệu, bạn có thể xử lý lỗi ở đây
                    SetAlert($"Thêm mới không thành công. Lỗi đã được ghi nhận vui lòng liên hệ Admin", "danger");
                    return View("TaoMoi");
                }
            }
            return View("TaoMoi");
        }
        [HttpGet]
        public ActionResult Sua(string TenTK)
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
                LoaiTK = tk.LoaiTK,
                NgayDK = tk.NgayDK,
                TrangThai = tk.TrangThai
            };
            List<string> listLoaiTK = new List<string> { "Admin", "User" };
            ViewBag.LoaiTK = new SelectList(listLoaiTK);
            List<string> listTrangThai = new List<string> { "Đang hoạt động", "Ngừng hoạt động" };
            ViewBag.TrangThai = new SelectList(listTrangThai);
            return View(result);
        }
        [HttpPost]
        public ActionResult Sua(TKViewModel tk, HttpPostedFileBase HinhAnh)
        {
           
            try
            {
                List<string> listLoaiTK = new List<string> { "Admin", "User" };
                ViewBag.LoaiTK = new SelectList(listLoaiTK);
                List<string> listTrangThai = new List<string> { "Đang hoạt động", "Ngừng hoạt động" };
                ViewBag.TrangThai = new SelectList(listTrangThai);
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
                    var TKDN= (TaiKhoan)Session["user"];
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
                    x.LoaiTK = tk.LoaiTK;
                    x.TrangThai = tk.TrangThai;
                    if (Session["user"] != null)
                    {
                        LichSu ls = new LichSu
                        {
                            ThongTinTT = $"Đã cập nhật tài khoản: {tk.TenTK}",
                            NgayGioTT = DateTime.Now,
                            TenTK = TKDN.TenTK
                        };
                        db.LichSus.Add(ls);
                    }
                    db.SaveChanges();
                    Session.Remove("Anh");
                    SetAlert("Cập nhật thành công", "sucsess");
                    return RedirectToAction("QLTaiKhoan");
                }
                return View(tk);
            }
            catch (Exception ex)
            {
                if (Session["user"] != null)
                {
                    TaiKhoan TKDN = (TaiKhoan)Session["user"];
                    LichSu ls = new LichSu
                    {
                        ThongTinTT = $"Error cập nhật tài khoản. Mã lỗi: {ex.Message}",
                        NgayGioTT = DateTime.Now,
                        TenTK = TKDN.TenTK
                    };
                    db.LichSus.Add(ls);
                }
                db.SaveChanges();
                SetAlert($"Cập nhật không thành công. Lỗi đã được ghi nhận vui lòng liên hệ Admin", "danger");
                return View(tk);
            }

        }
        public ActionResult Xoa(string TenTK)
        {
            var TKDN = Session["user"] as TaiKhoan;
            if (TKDN.TenTK == TenTK)
            {
                SetAlert($"Không thể xóa tài khoản đang đăng nhập !!!!", "danger");
                return RedirectToAction("QLTaiKhoan");
            }
            var result = db.TaiKhoans.Find(TenTK);
            result.TrangThai = "Ngừng hoạt động";
            /*db.TaiKhoans.Remove(result);*/
            try
            {
                /*if (result.HinhAnh != null)
                {
                    var path = Path.Combine(Server.MapPath("~/Content/TKImages"), result.HinhAnh);
                    if (System.IO.File.Exists(path))
                    {
                        System.IO.File.Delete(path);
                    }
                }*/
                if (Session["user"] != null)
                {
                    LichSu ls = new LichSu
                    {
                        ThongTinTT = $"Đã xóa tài khoản: {result.TenTK}",
                        NgayGioTT = DateTime.Now,
                        TenTK = TKDN.TenTK
                    };
                    db.LichSus.Add(ls);
                }
                db.SaveChanges();
                SetAlert("Xóa thành công", "sucsess");
                return RedirectToAction("QLTaiKhoan");
            }
            catch (Exception ex)
            {
                if (Session["user"] != null)
                {
                    LichSu ls = new LichSu
                    {
                        ThongTinTT = $"Error xóa tài khoản. Mã lỗi: {ex.Message}",
                        NgayGioTT = DateTime.Now,
                        TenTK = TKDN.TenTK
                    };
                    db.LichSus.Add(ls);
                }
                db.SaveChanges();
                SetAlert($"Xóa không thành công. Lỗi đã được ghi nhận vui lòng liên hệ Admin", "danger");
                return RedirectToAction("QLTaiKhoan");
            }
        }
        public ActionResult TimKiem(string keyword)
        {
            var result = db.TaiKhoans.Select(x => new TKViewModel
            {
                TenTK = x.TenTK,
                MatKhau = x.MatKhau,
                HoVaTen = x.HoVaTen,
                HinhAnh = x.HinhAnh,
                DiaChi = x.DiaChi,
                SoDienThoai = x.SoDienThoai,
                Email = x.Email,
                LoaiTK = x.LoaiTK,
                NgayDK = x.NgayDK,
                TrangThai = x.TrangThai
            }).ToList();
            if (string.IsNullOrEmpty(keyword))
            {
                return View("QLTaiKhoan", result);
            }
            result = result.Where(x => (x.TenTK != null && x.TenTK.Contains(keyword)) || (x.HoVaTen != null && x.HoVaTen.Contains(keyword)) || (x.DiaChi != null && x.DiaChi.Contains(keyword)) || (x.SoDienThoai != null && x.SoDienThoai.Contains(keyword))).ToList();
            ViewBag.Search = keyword;
            return View("QLTaiKhoan", result);
        }
        public JsonResult GetMonthlyUserCounts()
        {
            using (var context = new DA_WebTimKiemPhimEntities())
            {
                var data = context.Database.SqlQuery<MonthlyUserCount>("DemTKTheoThangNam").ToList();
                return Json(data, JsonRequestBehavior.AllowGet);
            }
        }
        public class MonthlyUserCount
        {
            public int Năm { get; set; }
            public int Tháng { get; set; }
            public int SốTàiKhoản { get; set; }
        }
        public ActionResult ExportExcel()
        {
            var tk = db.TaiKhoans.ToList();
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("TaiKhoan");
                worksheet.Cells["A1"].Value = "Tên tài khoản";
                worksheet.Cells["B1"].Value = "Key";
                worksheet.Cells["C1"].Value = "Họ và tên";
                worksheet.Cells["D1"].Value = "Tên hình ảnh";
                worksheet.Cells["E1"].Value = "Địa chỉ";
                worksheet.Cells["F1"].Value = "SDT";
                worksheet.Cells["G1"].Value = "Email";
                worksheet.Cells["H1"].Value = "Loại";
                worksheet.Cells["I1"].Value = "Trạng thái";
                worksheet.Cells["J1"].Value = "Ngày đăng kí";
                int row = 2;
                foreach (var item in tk)
                {
                    worksheet.Cells["A" + row].Value = item.TenTK;
                    worksheet.Cells["B" + row].Value = item.MatKhau;
                    worksheet.Cells["C" + row].Value = item.HoVaTen;
                    worksheet.Cells["D" + row].Value = item.HinhAnh;
                    worksheet.Cells["E" + row].Value = item.DiaChi;
                    worksheet.Cells["F" + row].Value = item.SoDienThoai;
                    worksheet.Cells["G" + row].Value = item.Email;
                    worksheet.Cells["H" + row].Value = item.LoaiTK;
                    worksheet.Cells["I" + row].Value = item.TrangThai;
                    worksheet.Cells["J" + row].Value = item.NgayDK.ToString("dd/MM/yyyy");
                    row++;
                }
                // Tự động điều chỉnh cột cho đọc dễ nhìn hơn
                worksheet.Cells.AutoFitColumns();
                // Tạo mảng byte của file Excel
                var excelBytes = package.GetAsByteArray();
                // Đặt kiểu nội dung và các header trong response
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment;  filename=TaiKhoan.xlsx");
                // Ghi mảng byte của file Excel vào response stream
                Response.BinaryWrite(excelBytes);
                Response.Flush();
                Response.End();
            }
            return new EmptyResult();
        }
    }
}