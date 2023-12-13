using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ClosedXML.Excel;
using DAM.Models;
using DAM.Models.ViewModel;
using DocumentFormat.OpenXml.EMMA;
using DocumentFormat.OpenXml.Math;
using DocumentFormat.OpenXml.Office2010.ExcelAc;
using DocumentFormat.OpenXml.Spreadsheet;
using OfficeOpenXml;

namespace DAM.Controllers
{
    public class RapChieuController : BaseController
    {
        DA_WebTimKiemPhimEntities db = new DA_WebTimKiemPhimEntities();
        // GET: RapChieu
        public ActionResult Index()
        {
            var rap = db.Raps.ToList();
            var result = rap.Where(x => x.TrangThai != "Ngừng hoạt động").Select(r => new RapPhimViewModel
            {
                MaRap = r.MaRap,
                TenRap = r.TenRap,
                DiaChi = r.DiaChi,
                GiaVe = r.GiaVe,
                TrangThai = r.TrangThai
            }).ToList();

            return View(result);
        }
        public ActionResult SCRap(string MaRap)
        {
            ViewBag.MaRap = MaRap;
            var scRap = db.SuatChieu_Rap.Where(x => x.MaRap == MaRap).ToList();
            var result = scRap.Select(scr => new SuatChieu_RapViewModel
            {
                MaSC = scr.MaSC,
                MaRap = scr.MaRap,
                NgayChieu = scr.NgayChieu
            }).ToList();
            return View(result);
        }
        [HttpGet]
        public ActionResult TaoMoi()
        {
            return View();
        }
        [HttpGet]
        public ActionResult TaoMoiSCRap(string MaRap)
        {
            var scRap = db.SuatChieus
                .Join(db.SuatChieu_Rap, sc => sc.MaSC, scr => scr.MaSC,
                (sc, scr) => new
                {
                    MaSC = sc.MaSC,
                    KhungGio = sc.KhungGio,
                    MaRap = scr.MaRap,
                    NgayChieu = scr.NgayChieu
                });
            var listSC = scRap.Where(x => x.MaRap == MaRap).Select(sc => sc.MaSC).ToList();
            ViewBag.MaSC = new SelectList(db.SuatChieus.OrderBy(x => x.MaSC), "MaSC", "KhungGio");
            return View();
        }
        [HttpPost]
        public ActionResult TaoMoi(RapPhimViewModel Model)
        {
            var r = new Rap
            {
                MaRap = Model.MaRap,
                TenRap = Model.TenRap,
                DiaChi = Model.DiaChi,
                GiaVe = Model.GiaVe,
                TrangThai = "Đang hoạt động"
            };
            db.Raps.Add(r);
            try
            {
                if (Session["user"] != null)
                {
                    TaiKhoan TKDN = (TaiKhoan)Session["user"];
                    LichSu ls = new LichSu
                    {
                        ThongTinTT = $"Đã tạo mới rạp mã: {r.MaRap}",
                        NgayGioTT = DateTime.Now,
                        TenTK = TKDN.TenTK
                    };
                    db.LichSus.Add(ls);
                }
                db.SaveChanges();
                // Nếu không có lỗi khi lưu dữ liệu, dòng sau sẽ được thực hiện
                SetAlert("Thêm mới thành công", "sucsess");
                return RedirectToAction("Index", "RapChieu");
            }
            catch (Exception ex)
            {
                if (Session["user"] != null)
                {
                    TaiKhoan TKDN = (TaiKhoan)Session["user"];
                    LichSu ls = new LichSu
                    {
                        ThongTinTT = $"Error tạo mới rạp. Mã lỗi: {ex.Message}",
                        NgayGioTT = DateTime.Now,
                        TenTK = TKDN.TenTK
                    };
                    db.LichSus.Add(ls);
                }
                db.SaveChanges();
                // Nếu có lỗi khi lưu dữ liệu, bạn có thể xử lý lỗi ở đây
                SetAlert($"Thêm mới không thành công. Lỗi: Lỗi đã được ghi nhận vui lòng liên hệ Admin", "danger");
                return View("TaoMoi");
            }

        }
        [HttpPost]
        public ActionResult TaoMoiSCRap(SuatChieu_RapViewModel Model)
        {
            ViewBag.MaSC = new SelectList(db.SuatChieus.OrderBy(x => x.MaSC), "MaSC", "KhungGio");
            var scRap = db.SuatChieus
                .Join(db.SuatChieu_Rap, sc => sc.MaSC, scr => scr.MaSC,
                (sc, scr) => new
                {
                    MaSC = sc.MaSC,
                    KhungGio = sc.KhungGio,
                    MaRap = scr.MaRap,
                    NgayChieu = scr.NgayChieu
                });
            var listSC = scRap.Where(x => x.MaRap == Model.MaRap && x.NgayChieu == Model.NgayChieu).Select(sc => sc.MaSC).ToList();
            if (!listSC.Any(x => x == Model.MaSC))
            {
                var scrAdd = new SuatChieu_Rap
                {
                    MaSC = Model.MaSC,
                    MaRap = Model.MaRap,
                    NgayChieu = Model.NgayChieu
                };
                db.SuatChieu_Rap.Add(scrAdd);
                if (Session["user"] != null)
                {
                    TaiKhoan TKDN = (TaiKhoan)Session["user"];
                    LichSu ls = new LichSu
                    {
                        ThongTinTT = $"Đã tạo thêm suất chiếu mã: {scrAdd.MaSC}, của rạp mã: {scrAdd.MaRap}, với ngày chiếu : {scrAdd.NgayChieu.ToString("dd/MM/yyyy")}",
                        NgayGioTT = DateTime.Now,
                        TenTK = TKDN.TenTK
                    };
                    db.LichSus.Add(ls);
                }
            }
            else
            {
                ViewBag.errMaSC = "Suất chiếu đã có trong này";
                ViewBag.errNgayChieu = "Ngày này đã có suất chiếu trên";
                return View(Model);
            }
            try
            {
                db.SaveChanges();
                // Nếu không có lỗi khi lưu dữ liệu, dòng sau sẽ được thực hiện
                SetAlert("Thêm thành công", "sucsess");
                return RedirectToAction("Index", "RapChieu");
            }
            catch (Exception ex)
            {
                if (Session["user"] != null)
                {
                    TaiKhoan TKDN = (TaiKhoan)Session["user"];
                    LichSu ls = new LichSu
                    {
                        ThongTinTT = $"Error thêm suất chiếu vào rạp. Mã lỗi: {ex.Message}",
                        NgayGioTT = DateTime.Now,
                        TenTK = TKDN.TenTK
                    };
                    db.LichSus.Add(ls);
                }
                db.SaveChanges();
                // Nếu có lỗi khi lưu dữ liệu, bạn có thể xử lý lỗi ở đây
                SetAlert($"Thêm suất chiếu vào rạp không thành công. Lỗi: Lỗi đã được ghi nhận vui lòng liên hệ Admin", "danger");
                return View("Index");
            }
        }
        [HttpGet]
        public ActionResult Sua(string MaRapPhim)
        {
            List<string> listTrangThai = new List<string> { "Đang hoạt động", "Ngừng hoạt động" };
            ViewBag.TrangThai = new SelectList(listTrangThai);
            var r = db.Raps.FirstOrDefault(x => x.MaRap == MaRapPhim);
            if (string.IsNullOrEmpty(MaRapPhim))
            {
                Response.StatusCode = 404;
                return null;
            }
            if (r == null)
            {
                return HttpNotFound();
            }
            var result = new RapPhimViewModel
            {
                MaRap = r.MaRap,
                TenRap = r.TenRap,
                DiaChi = r.DiaChi,
                GiaVe = r.GiaVe,
                TrangThai = r.TrangThai
            };
            return View(result);
        }
        [HttpGet]
        public ActionResult SuaSCRap(string MaRap, string MaSC, string NgayChieu)
        {
            DateTime ngayChieu = DateTime.ParseExact(NgayChieu, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            ViewBag.MaSC = new SelectList(db.SuatChieus.OrderBy(x => x.MaSC), "MaSC", "KhungGio");
            var scr = db.SuatChieu_Rap.FirstOrDefault(x => x.MaRap == MaRap &&  x.MaSC == MaSC && x.NgayChieu == ngayChieu);
            if (string.IsNullOrEmpty(MaRap) || string.IsNullOrEmpty(MaSC) || NgayChieu == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            if (scr == null)
            {
                return HttpNotFound();
            }
            var result = new SuatChieu_RapViewModel
            {
                MaSC = scr.MaSC,
                MaRap = scr.MaRap,
                NgayChieu = scr.NgayChieu
            };
            return View(result);
        }
        [HttpPost]
        public ActionResult Sua(RapPhimViewModel rp)
        {
            try
            {
                List<string> listTrangThai = new List<string> { "Đang hoạt động", "Ngừng hoạt động" };
                ViewBag.TrangThai = new SelectList(listTrangThai);
                if (ModelState.IsValid)
                {
                    var r = db.Raps.Find(rp.MaRap);
                    if (r == null)
                    {
                        return HttpNotFound();
                    }
                    r.MaRap = rp.MaRap;
                    r.TenRap = rp.TenRap;
                    r.DiaChi = rp.DiaChi;
                    r.GiaVe = rp.GiaVe;
                    r.TrangThai = rp.TrangThai;
                    if (Session["user"] != null)
                    {
                        TaiKhoan TKDN = (TaiKhoan)Session["user"];
                        LichSu ls = new LichSu
                        {
                            ThongTinTT = $"Đã cập nhật rạp mã: {r.MaRap}",
                            NgayGioTT = DateTime.Now,
                            TenTK = TKDN.TenTK
                        };
                        db.LichSus.Add(ls);
                    }
                    db.SaveChanges();
                    SetAlert("Cập nhật thành công", "sucsess");
                    return RedirectToAction("Index", "RapChieu");
                }
                return View(rp);
            }

            catch (Exception ex)
            {
                if (Session["user"] != null)
                {
                    TaiKhoan TKDN = (TaiKhoan)Session["user"];
                    LichSu ls = new LichSu
                    {
                        ThongTinTT = $"Error cập nhật rạp. Mã lỗi: {ex.Message}",
                        NgayGioTT = DateTime.Now,
                        TenTK = TKDN.TenTK
                    };
                    db.LichSus.Add(ls);
                }
                db.SaveChanges();
                // Nếu có lỗi khi lưu dữ liệu, bạn có thể xử lý lỗi ở đây
                SetAlert($"Cập nhật không thành công. Lỗi: Lỗi đã được ghi nhận vui lòng liên hệ Admin", "danger");
                return View(rp);
            }
        }
        [HttpPost]
        public ActionResult SuaSCRap(SuatChieu_RapViewModel model)
        {
            try
            {
                ViewBag.MaSC = new SelectList(db.SuatChieus.OrderBy(x => x.MaSC), "MaSC", "KhungGio");
                var scRap = db.SuatChieus
                .Join(db.SuatChieu_Rap, sc => sc.MaSC, scr => scr.MaSC,
                (sc, scr) => new
                {
                    MaSC = sc.MaSC,
                    KhungGio = sc.KhungGio,
                    MaRap = scr.MaRap,
                    NgayChieu = scr.NgayChieu
                });
                var listSC = scRap.Where(x => x.MaRap == model.MaRap && x.NgayChieu == model.NgayChieu).Select(sc => sc.MaSC).ToList();
                if (ModelState.IsValid)
                {
                    var scr = db.SuatChieu_Rap.FirstOrDefault(x => x.MaSC == model.MaSC && x.MaRap == model.MaRap && x.NgayChieu == model.NgayChieu);
                    if (scr == null)
                    {
                        return HttpNotFound();
                    }
                    if (!listSC.Any(x => x == model.MaSC))
                    {
                        scr.MaSC = model.MaSC;
                        scr.NgayChieu = model.NgayChieu;
                        if (Session["user"] != null)
                        {
                            TaiKhoan TKDN = (TaiKhoan)Session["user"];
                            LichSu ls = new LichSu
                            {
                                ThongTinTT = $"Đã tạo cập nhật suất chiếu mã: {scr.MaSC}, của rạp mã: {scr.MaRap}, với ngày chiếu : {scr.NgayChieu.ToString("dd/MM/yyyy")}",
                                NgayGioTT = DateTime.Now,
                                TenTK = TKDN.TenTK
                            };
                            db.LichSus.Add(ls);
                        }
                    }
                    else
                    {
                        ViewBag.errMaSC = "Suất chiếu đã có trong này";
                        ViewBag.errNgayChieu = "Ngày này đã có suất chiếu trên";
                        return View(model);
                    }
                    db.SaveChanges();
                    SetAlert("Cập nhật thành công", "sucsess");
                    return RedirectToAction("Index", "RapChieu");
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
                        ThongTinTT = $"Error cập nhật suất chiếu của rạp. Mã lỗi: {ex.Message}",
                        NgayGioTT = DateTime.Now,
                        TenTK = TKDN.TenTK
                    };
                    db.LichSus.Add(ls);
                }
                db.SaveChanges();
                // Nếu có lỗi khi lưu dữ liệu, bạn có thể xử lý lỗi ở đây
                SetAlert($"Cập nhật không thành công. Lỗi: Lỗi đã được ghi nhận vui lòng liên hệ Admin", "danger");
                return View(model);
            }
        }
        public ActionResult Xoa(string MaRapPhim)
        {
            var result = db.Raps.FirstOrDefault(x => x.MaRap == MaRapPhim);
            /*db.Raps.Remove(result);*/
            try
            {
                if (Session["user"] != null)
                {
                    TaiKhoan TKDN = (TaiKhoan)Session["user"];
                    result.TrangThai = "Ngừng hoạt động";
                    LichSu ls = new LichSu
                    {
                        ThongTinTT = $"Đã xóa rạp mã: {result.MaRap}",
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
                        ThongTinTT = $"Error xóa rạp. Mã lỗi: {ex.Message}",
                        NgayGioTT = DateTime.Now,
                        TenTK = TKDN.TenTK
                    };
                    db.LichSus.Add(ls);
                }
                db.SaveChanges();
                SetAlert($"Xóa không thành công. Lỗi: Lỗi đã được ghi nhận vui lòng liên hệ Admin", "danger");
                return RedirectToAction("Index");
            }
        }
        public ActionResult XoaSCRap(string MaRap, string MaSC, string NgayChieu)
        {
            DateTime ngayChieu = DateTime.ParseExact(NgayChieu, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            var scr = db.SuatChieu_Rap.FirstOrDefault(x => x.MaSC == MaSC && x.MaRap == MaRap && x.NgayChieu == ngayChieu);
            db.SuatChieu_Rap.Remove(scr);
            try
            {
                if (Session["user"] != null)
                {
                    TaiKhoan TKDN = (TaiKhoan)Session["user"];
                    LichSu ls = new LichSu
                    {
                        ThongTinTT = $"Đã xóa suất chiếu mã: {scr.MaSC}, của rạp mã: {scr.MaRap}",
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
                        ThongTinTT = $"Error xóa suất chiếu của rạp. Mã lỗi: {ex.Message}",
                        NgayGioTT = DateTime.Now,
                        TenTK = TKDN.TenTK
                    };
                    db.LichSus.Add(ls);
                }
                db.SaveChanges();
                SetAlert($"Xóa không thành công. Lỗi: Lỗi đã được ghi nhận vui lòng liên hệ Admin", "danger");
                return RedirectToAction("Index");
            }
        }
        public ActionResult TimKiem(string keyword)
        {
            var RapChieu = db.Raps.ToList();
            var result = RapChieu.Where(x => x.TrangThai != "Ngừng hoạt động").Select(p => new RapPhimViewModel
            {
                MaRap = p.MaRap,
                TenRap = p.TenRap,
                DiaChi = p.DiaChi,
                TrangThai = p.TrangThai
            }).ToList();
            if (string.IsNullOrEmpty(keyword))
            {
                return View("Index", result);
            }
            result = result.Where(x => (x.MaRap != null && x.MaRap.Contains(keyword)) || (x.TenRap != null && x.TenRap.Contains(keyword)) || (x.DiaChi != null && x.DiaChi.Contains(keyword))).ToList();
            ViewBag.Search = keyword;
            return View("Index", result);
        }
        public ActionResult TimKiemSCRap(string keyword, string MaRap)
        {
            var scr = db.SuatChieu_Rap.Where(x => x.MaRap == MaRap).ToList();
            var result = scr.Select(x => new SuatChieu_RapViewModel
            {
                MaSC = x.MaSC,
                MaRap = x.MaRap,
                NgayChieu = x.NgayChieu
            }).ToList();
            if (string.IsNullOrEmpty(keyword))
            {
                return View("SCRap", result);
            }
            result = result.Where(x => (x.MaSC != null && x.MaSC.Contains(keyword))).ToList();
            ViewBag.Search = keyword;
            return View("SCRap", result);
        }
        public ActionResult ExportExcel()
        {
            var rapPhims = db.Raps.ToList();
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            // Tạo một gói Excel mới
            using (ExcelPackage package = new ExcelPackage())
            {
                // Tạo một worksheet
                var worksheet = package.Workbook.Worksheets.Add("RapChieu");
                // Đặt tiêu đề cột
                worksheet.Cells["A1"].Value = "Mã Rạp";
                worksheet.Cells["B1"].Value = "Tên Rạp";
                worksheet.Cells["C1"].Value = "Địa Chỉ";
                worksheet.Cells["D1"].Value = "Giá vé";
                worksheet.Cells["E1"].Value = "Trạng thái";
                int row = 2;
                // Điền dữ liệu vào các hàng
                foreach (var rap in rapPhims)
                {
                    worksheet.Cells["A" + row].Value = rap.MaRap;
                    worksheet.Cells["B" + row].Value = rap.TenRap;
                    worksheet.Cells["C" + row].Value = rap.DiaChi;
                    worksheet.Cells["D" + row].Value = rap.GiaVe;
                    worksheet.Cells["E" + row].Value = rap.TrangThai;
                    // Mã Phim được nối thành một chuỗi và hiển thị
                    row++;
                }
                // Tự động điều chỉnh cột cho đọc dễ nhìn hơn
                worksheet.Cells.AutoFitColumns();
                // Tạo mảng byte của file Excel
                var excelBytes = package.GetAsByteArray();
                // Đặt kiểu nội dung và các header trong response
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment;  filename=RapChieu.xlsx");
                // Ghi mảng byte của file Excel vào response stream
                Response.BinaryWrite(excelBytes);
                Response.Flush();
                Response.End();
            }
            // Trả về kết quả rỗng (ngăn việc hiển thị view)
            return new EmptyResult();
        }
        public ActionResult ExportExcelSCRap(string MaRap)
        {
            var scr = db.SuatChieu_Rap.Where(x => x.MaRap == MaRap).ToList();
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            // Tạo một gói Excel mới
            using (ExcelPackage package = new ExcelPackage())
            {
                // Tạo một worksheet
                var worksheet = package.Workbook.Worksheets.Add("Suatchieucuarap");
                // Đặt tiêu đề cột
                worksheet.Cells["A1"].Value = "Mã Rạp";
                worksheet.Cells["B1"].Value = "Mã suất chiếu";
                worksheet.Cells["C1"].Value = "Ngày chiếu";
                int row = 2;
                // Điền dữ liệu vào các hàng
                foreach (var x in scr)
                {
                    worksheet.Cells["A" + row].Value = x.MaRap;
                    worksheet.Cells["B" + row].Value = x.MaSC;
                    worksheet.Cells["C" + row].Value = x.NgayChieu.ToString("dd/MM/yyyy");
                    // Mã Phim được nối thành một chuỗi và hiển thị
                    row++;
                }
                // Tự động điều chỉnh cột cho đọc dễ nhìn hơn
                worksheet.Cells.AutoFitColumns();
                // Tạo mảng byte của file Excel
                var excelBytes = package.GetAsByteArray();
                // Đặt kiểu nội dung và các header trong response
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment;  filename=Suatchieucuarap.xlsx");
                // Ghi mảng byte của file Excel vào response stream
                Response.BinaryWrite(excelBytes);
                Response.Flush();
                Response.End();
            }
            // Trả về kết quả rỗng (ngăn việc hiển thị view)
            return new EmptyResult();
        }
    }
}