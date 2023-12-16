using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DAM.Models;
using DAM.Models.ViewModel;
using Irony.Parsing;
using Microsoft.Ajax.Utilities;
using OfficeOpenXml;

namespace DAM.Controllers
{
    public class XuatChieuController : BaseController
    {
        DA_WebTimKiemPhimEntities db = new DA_WebTimKiemPhimEntities();
        // GET: XuatChieu
        public ActionResult Index()
        {
            var suat = db.SuatChieus.ToList();
            var result = suat.Where(x => x.TrangThai != "Đã hủy").Select(p => new SuatChieuPhimModel
            {
                MaSC = p.MaSC,
                KhungGio = p.KhungGio,
                LoaiChieu = p.LoaiChieu,
                TrangThai = p.TrangThai,
            }).ToList();
            return View(result);
        }
        public ActionResult TaoMoi()
        {
            List<string> lstloaichieu = new List<string> { "2D", "3D", "4DX" };
            ViewBag.LoaiChieu = new SelectList(lstloaichieu);
            return View();
        }
        [HttpPost]
        public ActionResult TaoMoi(SuatChieuPhimModel Model)
        {
            List<string> lstloaichieu = new List<string> { "2D", "3D", "4DX" };
            ViewBag.LoaiChieu = new SelectList(lstloaichieu);
           
            var sc = new SuatChieu
            {
                MaSC = Model.MaSC,
                KhungGio = (TimeSpan)Model.KhungGio,
                LoaiChieu = Model.LoaiChieu
            };
            db.SuatChieus.Add(sc);
            try
            {
                if (Session["user"] != null)
                {
                    TaiKhoan TKDN = (TaiKhoan)Session["user"];
                    LichSu ls = new LichSu
                    {
                        ThongTinTT = $"Đã thêm mới suất chiếu mã: {sc.MaSC}",
                        NgayGioTT = DateTime.Now,
                        TenTK = TKDN.TenTK
                    };
                    db.LichSus.Add(ls);
                }
                db.SaveChanges();
                // Nếu không có lỗi khi lưu dữ liệu, dòng sau sẽ được thực hiện
                SetAlert("Thêm mới thành công", "sucsess");
                return RedirectToAction("Index", "XuatChieu");
            }
            catch (Exception ex)
            {
                if (Session["user"] != null)
                {
                    TaiKhoan TKDN = (TaiKhoan)Session["user"];
                    LichSu ls = new LichSu
                    {
                        ThongTinTT = $"Error thêm mới suất chiếu. Mã lỗi: {ex.Message}",
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
        public ActionResult Sua(string maSuatChieu)
        {
            var sc = db.SuatChieus.ToList();
            var suatchieu = sc.FirstOrDefault(x => x.MaSC == maSuatChieu);
            List<string> lstloaichieu = new List<string> { "2D", "3D", "4DX" };
            ViewBag.LoaiChieu = new SelectList(lstloaichieu,suatchieu.LoaiChieu);
            List<string> lstTrangThai = new List<string> { "Đang chiếu", "Đã hủy" };
            ViewBag.TrangThai = new SelectList(lstTrangThai,suatchieu.TrangThai);
            if (string.IsNullOrEmpty(maSuatChieu))
            {
                Response.StatusCode = 404;
                return null;
            }
            if (suatchieu == null)
            {
                return HttpNotFound();
            }
            var result = new SuatChieuPhimModel
            {
                MaSC = suatchieu.MaSC,
                KhungGio = suatchieu.KhungGio,
                LoaiChieu = suatchieu.LoaiChieu,
                TrangThai = suatchieu.TrangThai
            };
            return View(result);
        }
        [HttpPost]
        public ActionResult Sua(SuatChieuPhimModel model)
        {
            try
            {
                List<string> lstloaichieu = new List<string> { "2D", "3D", "4DX" };
                ViewBag.LoaiChieu = new SelectList(lstloaichieu,model.LoaiChieu);
                List<string> lstTrangThai = new List<string> { "Đang chiếu", "Đã hủy" };
                ViewBag.TrangThai = new SelectList(lstTrangThai,model.TrangThai);
                if (ModelState.IsValid)
                {
                    var sc = db.SuatChieus.Find(model.MaSC);
                    if (sc == null)
                    {
                        return HttpNotFound();
                    }
                    sc.MaSC = model.MaSC;
                    sc.KhungGio = (TimeSpan)model.KhungGio;
                    sc.LoaiChieu = model.LoaiChieu;
                    sc.TrangThai = model.TrangThai;
                    if (Session["user"] != null)
                    {
                        TaiKhoan TKDN = (TaiKhoan)Session["user"];
                        LichSu ls = new LichSu
                        {
                            ThongTinTT = $"Đã cập nhật suất chiếu mã: {sc.MaSC}",
                            NgayGioTT = DateTime.Now,
                            TenTK = TKDN.TenTK
                        };
                        db.LichSus.Add(ls);
                    }
                    db.SaveChanges();
                    SetAlert("Cập nhật thành công", "sucsess");
                    return RedirectToAction("Index", "XuatChieu");
                }
                return View(model);
            }

            catch (Exception ex)
            {
                if (Session["user"] != null)
                {
                    TaiKhoan TKDN = (TaiKhoan)Session["user"];
                    LichSu ls = new LichSu
                    {
                        ThongTinTT = $"Error cập nhật suất chiếu. Mã lỗi: {ex.Message}",
                        NgayGioTT = DateTime.Now,
                        TenTK = TKDN.TenTK
                    };
                    db.LichSus.Add(ls);
                }
                db.SaveChanges();
                // Nếu có lỗi khi lưu dữ liệu, bạn có thể xử lý lỗi ở đây
                SetAlert($"Cập nhật không thành công. Lỗi đã được ghi nhận vui lòng liên hệ Admin ", "danger");
                return View(model);
            }
        }
        public ActionResult Xoa(string maSuatChieu)
        {
            var result = db.SuatChieus.Find(maSuatChieu);
            /*db.SuatChieus.Remove(result);*/
            result.TrangThai = "Đã hủy";
            try
            {
                if (Session["user"] != null)
                {
                    TaiKhoan TKDN = (TaiKhoan)Session["user"];
                    LichSu ls = new LichSu
                    {
                        ThongTinTT = $"Đã xóa suất chiếu mã: {result.MaSC}",
                        NgayGioTT = DateTime.Now,
                        TenTK = TKDN .TenTK
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
                        ThongTinTT = $"Error xóa suất chiếu. Mã lỗi: {ex.Message}",
                        NgayGioTT = DateTime.Now,
                        TenTK = TKDN.TenTK
                    };
                    db.LichSus.Add(ls);
                }
                db.SaveChanges();
                // Nếu có lỗi khi lưu dữ liệu, bạn có thể xử lý lỗi ở đây
                SetAlert($"Xóa không thành công. Lỗi đã được ghi nhận vui lòng liên hệ Admin ", "danger");
                return RedirectToAction("Index");
            }
        }
        public ActionResult TimKiem(string keyword)
        {
            var suatchieu = db.SuatChieus.ToList();
            var result = suatchieu.Where(x => x.TrangThai != "Đã hủy").Select(p => new SuatChieuPhimModel
            {
               MaSC = p.MaSC,
               KhungGio = p.KhungGio,
               LoaiChieu = p.LoaiChieu,
                TrangThai = p.TrangThai
            }).ToList();
            if (string.IsNullOrEmpty(keyword))
            {
                return View("Index", result);
            }
            result = result.Where(x => (x.MaSC != null && x.MaSC.ToLower().Contains(keyword.ToLower())) || (x.LoaiChieu != null && x.LoaiChieu.ToLower().Contains(keyword.ToLower())) || (x.KhungGio != null && x.KhungGio.ToString().Contains(keyword)) ).ToList();
            ViewBag.Search = keyword;
            return View("Index", result);
        }
        public ActionResult ExportExcel()
        {
            var suatchieu = db.SuatChieus.ToList();

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (ExcelPackage package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("SuatChieu");
                worksheet.Cells["A1"].Value = "Mã Suất Chiếu";
                worksheet.Cells["B1"].Value = "Khung Giờ";
                worksheet.Cells["C1"].Value = "Loại Chiếu";
                worksheet.Cells["D1"].Value = "Trạng thái";
                int row = 2;
                foreach (var sc in suatchieu)
                {
                    worksheet.Cells["A" + row].Value = sc.MaSC;
                    worksheet.Cells["B" + row].Value = sc.KhungGio.ToString();
                    worksheet.Cells["C" + row].Value = sc.LoaiChieu;
                    worksheet.Cells["D" + row].Value = sc.TrangThai;
                    row++;
                }
                worksheet.Cells.AutoFitColumns();
                var excelBytes = package.GetAsByteArray();
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment; filename=SuatChieu.xlsx");
                Response.BinaryWrite(excelBytes);
                Response.Flush();
                Response.End();
            }
            return new EmptyResult();
        }
    }
}