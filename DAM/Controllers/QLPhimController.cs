using DAM.Models;
using DAM.Models.ViewModel;
using DAM.Validation;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.Ajax.Utilities;
using OfficeOpenXml;
using PagedList;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Razor.Tokenizer.Symbols;

namespace DAM.Controllers
{
    public class QLPhimController : BaseController
    {
        // GET: QLPhim
        DA_WebTimKiemPhimEntities db = new DA_WebTimKiemPhimEntities();
        public ActionResult QLPhim()
        {
            var phims = db.Phims.ToList();
            var phimTL = db.Phim_TheLoai.ToList();
            var scPhim = db.SuatChieu_Phim.ToList();
            var result = phims.Select(p => new PhimViewModel
            {
                MaPhim = p.MaPhim,
                TenPhim = p.TenPhim,
                HinhAnh = p.HinhAnh,
                DaTaTrailer = p.DaTaTrailer,
                ThongTinPhim = p.ThongTinPhim,
                NoiDung = p.NoiDung,
                ThoiLuong = p.ThoiLuong,
                TrangThai = p.TrangThai,
                NgayChieu = p.NgayChieu,
                MaSC = scPhim.Where(x => x.MaPhim == p.MaPhim).Select(x => x.MaSC).ToList(),
                MaTL = phimTL.Where(tl => tl.MaPhim == p.MaPhim).Select(tl => tl.MaTL).ToList()
            }).ToList();
            return View(result);
        }
        [HttpGet]
        public ActionResult TaoMoi()
        {
            //Tạo dropdownlist
            ViewBag.MaSC = new SelectList(db.SuatChieus.OrderBy(x => x.MaSC), "MaSC", "MaSC");
            ViewBag.MaTL = new SelectList(db.TheLoais.OrderBy(x => x.TenTL), "MaTL", "TenTL");
            List<string> listTrangThai = new List<string> { "Đang chiếu", "Ngừng chiếu", "Sắp chiếu" };
            ViewBag.TrangThai = new SelectList(listTrangThai);
            return View();
        }
        [HttpPost]
        public ActionResult TaoMoi(PhimViewModel model, HttpPostedFileBase HinhAnh)
        {
            //Tạo dropdownlist
            ViewBag.MaSC = new SelectList(db.SuatChieus.OrderBy(x => x.MaSC), "MaSC", "MaSC");
            ViewBag.MaTL = new SelectList(db.TheLoais.OrderBy(x => x.TenTL), "MaTL", "TenTL");
            List<string> listTrangThai = new List<string> { "Đang chiếu", "Ngừng chiếu", "Sắp chiếu" };
            ViewBag.TrangThai = new SelectList(listTrangThai);
            //check ảnh tồn tại
            try
            {
                if (ModelState.IsValid)
                {
                    if (HinhAnh == null || HinhAnh.ContentLength == 0)
                    {
                        // Ảnh không được chọn lưu hình mặc định
                        model.HinhAnh = "LogoFilm.png";
                    }
                    else
                    {
                        //lấy tên ảnh
                        var fileName = Path.GetFileName(HinhAnh.FileName);
                        //Chuyển ảnh vào thư mục
                        var path = Path.Combine(Server.MapPath("~/Content/MovieImages"), fileName);
                        if (System.IO.File.Exists(path))
                        {
                            ViewBag.upload = "Hình đã tồn tại";
                            return View(model);
                        }
                        else
                        {
                            HinhAnh.SaveAs(path);
                            Session["TenHinh"] = HinhAnh.FileName;
                            ViewBag.TenHinh = "";
                            model.HinhAnh = fileName;
                        }
                    }
                    var p = new Phim
                    {
                        MaPhim = model.MaPhim,
                        TenPhim = model.TenPhim,
                        HinhAnh = model.HinhAnh,
                        DaTaTrailer = model.DaTaTrailer,
                        ThongTinPhim = model.ThongTinPhim,
                        NoiDung = model.NoiDung,
                        ThoiLuong = model.ThoiLuong,
                        TrangThai = model.TrangThai,
                        NgayChieu = (DateTime)model.NgayChieu
                    };
                    db.Phims.Add(p);
                    if (model.MaSC != null)
                    {
                        foreach (var item in model.MaSC)
                        {
                            var sc = new SuatChieu_Phim
                            {
                                MaPhim = model.MaPhim,
                                MaSC = item
                            };
                            db.SuatChieu_Phim.Add(sc);
                        }
                    }
                    if (model.MaTL != null)
                    {
                        foreach (var item in model.MaTL)
                        {
                            var tl = new Phim_TheLoai
                            {
                                MaTL = item,
                                MaPhim = model.MaPhim
                            };
                            db.Phim_TheLoai.Add(tl);
                        }
                    }
                    //Truy vết tạo mới
                    if (Session["user"] != null)
                    {
                        TaiKhoan TKDN = (TaiKhoan)Session["user"];
                        LichSu ls = new LichSu
                        {
                            ThongTinTT = $"Đã thêm mới phim mã: {p.MaPhim}",
                            NgayGioTT = DateTime.Now,
                            TenTK = TKDN.TenTK
                        };
                        db.LichSus.Add(ls);
                    }
                    db.SaveChanges();
                    // Nếu không có lỗi khi lưu dữ liệu, dòng sau sẽ được thực hiện
                    SetAlert("Thêm mới thành công", "sucsess");
                    return RedirectToAction("QLPhim", "QLPhim");
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
                        ThongTinTT = $"Error thêm mới phim. Mã lỗi: {ex.Message}",
                        NgayGioTT = DateTime.Now,
                        TenTK = TKDN.TenTK
                    };
                    db.LichSus.Add(ls);
                }
                db.SaveChanges();
                // Nếu có lỗi khi lưu dữ liệu, bạn có thể xử lý lỗi ở đây
                SetAlert($"Thêm mới không thành công. Lỗi đã được ghi nhận vui lòng liên hệ Admin", "danger");
                return View(model);
            }
        }
        [HttpGet]
        public ActionResult ChinhSua(string MaPhim)
        {
            ViewBag.MaSC = new SelectList(db.SuatChieus.OrderBy(x => x.MaSC), "MaSC", "MaSC");
            ViewBag.MaTL = new SelectList(db.TheLoais.OrderBy(x => x.TenTL), "MaTL", "TenTL");
            List<string> listTrangThai = new List<string> { "Đang chiếu", "Ngừng chiếu", "Sắp chiếu" };
            ViewBag.TrangThai = new SelectList(listTrangThai);
            var phims = db.Phims.ToList();
            var p = phims.FirstOrDefault(x => x.MaPhim == MaPhim);
            Session["AnhPhim"] = p;
            if (string.IsNullOrEmpty(MaPhim))
            {
                Response.StatusCode = 404;
                return null;
            }
            if (p == null)
            {
                return HttpNotFound();
            }
            var suatChieuPhims = db.SuatChieu_Phim.Where(sc => sc.MaPhim == MaPhim).ToList();
            var phimTheLoais = db.Phim_TheLoai.Where(tl => tl.MaPhim == MaPhim).ToList();
            var result = new PhimViewModel
            {
                MaPhim = p.MaPhim,
                TenPhim = p.TenPhim,
                HinhAnh = p.HinhAnh,
                DaTaTrailer = p.DaTaTrailer,
                ThongTinPhim = p.ThongTinPhim,
                NoiDung = p.NoiDung,
                ThoiLuong = p.ThoiLuong,
                TrangThai = p.TrangThai,
                NgayChieu = p.NgayChieu,
                MaSC = suatChieuPhims.Select(sc => sc.MaSC).ToList(),
                MaTL = phimTheLoais.Select(tl =>  tl.MaTL).ToList(),
            };
            return View(result);
        }
        [HttpPost]
        public ActionResult ChinhSua(PhimViewModel model, HttpPostedFileBase HinhAnh)
        {
            try
            {
                ViewBag.MaSC = new SelectList(db.SuatChieus.OrderBy(x => x.MaSC), "MaSC", "MaSC");
                ViewBag.MaTL = new SelectList(db.TheLoais.OrderBy(x => x.TenTL), "MaTL", "TenTL");
                List<string> listTrangThai = new List<string> { "Đang chiếu", "Ngừng chiếu", "Sắp chiếu" };
                ViewBag.TrangThai = new SelectList(listTrangThai);
                if (ModelState.IsValid)
                {
                    //check ảnh tồn tại
                    var p = db.Phims.Find(model.MaPhim);
                    if (p == null)
                    {
                        return HttpNotFound();
                    }
                    if (HinhAnh != null && HinhAnh.ContentLength != 0)
                    {
                        //lấy tên ảnh
                        var fileName = Path.GetFileName(HinhAnh.FileName);
                        //Chuyển ảnh vào thư mục
                        var path1 = Path.Combine(Server.MapPath("~/Content/MovieImages"), fileName);
                        if (System.IO.File.Exists(path1))
                        {
                            ViewBag.upload = "Hình đã tồn tại";
                            return View(model);
                        }
                        else
                        {
                            var anh = Session["AnhPhim"] as Phim;
                            if (anh.HinhAnh != null)
                            {
                                var path2 = Path.Combine(Server.MapPath("~/Content/MovieImages"), anh.HinhAnh);
                                if (System.IO.File.Exists(path2))
                                {
                                    System.IO.File.Delete(path2);
                                }
                            }
                            HinhAnh.SaveAs(path1);
                            Session["TenHinh"] = HinhAnh.FileName;
                            ViewBag.TenHinh = "";
                            model.HinhAnh = fileName;
                        }
                    }
                    else
                    {
                        var anh = Session["AnhPhim"] as Phim;
                        model.HinhAnh = anh.HinhAnh;
                    }
                    p.TenPhim = model.TenPhim;
                    p.HinhAnh = model.HinhAnh;
                    p.DaTaTrailer = model.DaTaTrailer;
                    p.ThongTinPhim = model.ThongTinPhim;
                    p.NoiDung = model.NoiDung;
                    p.ThoiLuong = model.ThoiLuong;
                    p.TrangThai = model.TrangThai;
                    p.NgayChieu = (DateTime)model.NgayChieu;
                    if (model.MaSC != null)
                    {
                        var scList = db.SuatChieu_Phim.Where(sc => sc.MaPhim == model.MaPhim).ToList();
                        foreach (var item in scList)
                        {
                            db.SuatChieu_Phim.Remove(item);
                        }
                        foreach (var item in model.MaSC)
                        {
                            var sc = new SuatChieu_Phim
                            {
                                MaPhim = model.MaPhim,
                                MaSC = item
                            };
                            db.SuatChieu_Phim.Add(sc);
                        }
                    }
                    if (model.MaTL != null)
                    {
                        var tlList = db.Phim_TheLoai.Where(tl => tl.MaPhim == model.MaPhim).ToList();
                        foreach (var item in tlList)
                        {
                            db.Phim_TheLoai.Remove(item);
                        }
                        foreach (var item in model.MaTL)
                        {
                            var tl = new Phim_TheLoai
                            {
                                MaPhim = model.MaPhim,
                                MaTL = item
                            };
                            db.Phim_TheLoai.Add(tl);
                        }
                    }
                    if (Session["user"] != null)
                    {
                        TaiKhoan TKDN = (TaiKhoan)Session["user"];
                        LichSu ls = new LichSu
                        {
                            ThongTinTT = $"Đã cập nhật phim mã: {p.MaPhim}",
                            NgayGioTT = DateTime.Now,
                            TenTK = TKDN.TenTK
                        };
                        db.LichSus.Add(ls);
                    }
                    db.SaveChanges();
                    Session.Remove("AnhPhim");
                    // Nếu không có lỗi khi lưu dữ liệu, dòng sau sẽ được thực hiện
                    SetAlert("Cập nhật thành công", "sucsess");
                    return RedirectToAction("QLPhim", "QLPhim");
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
                        ThongTinTT = $"Error cập nhật phim. Mã lỗi: {ex.Message}",
                        NgayGioTT = DateTime.Now,
                        TenTK = TKDN.TenTK
                    };
                    db.LichSus.Add(ls);
                }
                db.SaveChanges();
                // Nếu có lỗi khi lưu dữ liệu, bạn có thể xử lý lỗi ở đây
                SetAlert($"Cập nhật không thành công. Lỗi đã được ghi nhận vui lòng liên hệ Admin", "danger");
                return View(model);
            }
        }
        public ActionResult Xoa(string MaPhim)
        {
            var result = db.Phims.Find(MaPhim);
            /*db.Phims.Remove(result);*/
            try
            {
                /*if (result.HinhAnh != null)
                {
                    var path = Path.Combine(Server.MapPath("~/Content/MovieImages"), result.HinhAnh);
                    if (System.IO.File.Exists(path))
                    {
                        System.IO.File.Delete(path);
                    }
                }*/
                result.TrangThai = "Ngừng chiếu";
                if (Session["user"] != null)
                {
                    TaiKhoan TKDN = (TaiKhoan)Session["user"];
                    LichSu ls = new LichSu
                    {
                        ThongTinTT = $"Đã xóa phim mã: {result.MaPhim}",
                        NgayGioTT = DateTime.Now,
                        TenTK = TKDN.TenTK
                    };
                    db.LichSus.Add(ls);
                }
                db.SaveChanges();
                SetAlert("Xóa thành công", "sucsess");
                return RedirectToAction("QLPhim");
            }
            catch (Exception ex)
            {
                if (Session["user"] != null)
                {
                    TaiKhoan TKDN = (TaiKhoan)Session["user"];
                    LichSu ls = new LichSu
                    {
                        ThongTinTT = $"Error xóa phim. Mã lỗi: {ex.Message}",
                        NgayGioTT = DateTime.Now,
                        TenTK = TKDN.TenTK
                    };
                    db.LichSus.Add(ls);
                }
                db.SaveChanges();
                SetAlert($"Xóa không thành công. Lỗi đã được ghi nhận vui lòng liên hệ Admin", "danger");
                return RedirectToAction("QLPhim");
            }
        }
        public ActionResult TimKiem(string keyword)
        {
            var phims = db.Phims.ToList();
            var suatChieuPhims = db.SuatChieu_Phim.ToList();
            var phimTheLoais = db.Phim_TheLoai.ToList();

            var result = phims.Select(p => new PhimViewModel
            {
                MaPhim = p.MaPhim,
                TenPhim = p.TenPhim,
                HinhAnh = p.HinhAnh,
                DaTaTrailer = p.DaTaTrailer,
                ThongTinPhim = p.ThongTinPhim,
                NoiDung = p.NoiDung,
                ThoiLuong = p.ThoiLuong,
                TrangThai = p.TrangThai,
                NgayChieu = p.NgayChieu,
                MaSC = suatChieuPhims.Where(sc => sc.MaPhim == p.MaPhim).Select(sc => sc.MaSC).ToList(),
                MaTL = phimTheLoais.Where(tl => tl.MaPhim == p.MaPhim).Select(tl => tl.MaTL).ToList()
            }).ToList();
            if (string.IsNullOrEmpty(keyword))
            {
                return View("QLPhim", result);
            }
            result = result.Where(x => (x.MaPhim != null && x.MaPhim.Contains(keyword)) || (x.TenPhim != null && x.TenPhim.Contains(keyword)) || (x.TrangThai != null && x.TrangThai.Contains(keyword))).ToList();
            ViewBag.Search = keyword;
            return View("QLPhim", result);
        }
        public ActionResult ExportExcel()
        {
            var phims = db.Phims.ToList();
            var suatChieuPhims = db.SuatChieu_Phim.ToList();
            var phimTLs = db.Phim_TheLoai.ToList();

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Phim");
                worksheet.Cells["A1"].Value = "Mã phim";
                worksheet.Cells["B1"].Value = "Tên phim";
                worksheet.Cells["C1"].Value = "Tên ảnh";
                worksheet.Cells["D1"].Value = "Mã Trailer";
                worksheet.Cells["E1"].Value = "Thông tin";
                worksheet.Cells["F1"].Value = "Nội dung";
                worksheet.Cells["G1"].Value = "Thời lượng";
                worksheet.Cells["H1"].Value = "Trạng thái";
                worksheet.Cells["I1"].Value = "Ngày chiếu";
                worksheet.Cells["J1"].Value = "Mã suất chiếu";
                worksheet.Cells["K1"].Value = "Mã thể loại";
                int row = 2;
                foreach (var item in phims)
                {
                    var maSuatChieuList = suatChieuPhims.Where(x => x.MaPhim == item.MaPhim).Select(x => x.MaSC).ToList();
                    var maTlList = phimTLs.Where(x => x.MaPhim == item.MaPhim).Select(x => x.MaTL).ToList();
                    worksheet.Cells["A" + row].Value = item.MaPhim;
                    worksheet.Cells["B" + row].Value = item.TenPhim;
                    worksheet.Cells["C" + row].Value = item.HinhAnh;
                    worksheet.Cells["D" + row].Value = item.DaTaTrailer;
                    worksheet.Cells["E" + row].Value = item.ThongTinPhim;
                    worksheet.Cells["F" + row].Value = item.NoiDung;
                    worksheet.Cells["G" + row].Value = item.ThoiLuong;
                    worksheet.Cells["H" + row].Value = item.TrangThai;
                    worksheet.Cells["I" + row].Value = item.NgayChieu.ToString("dd/MM/yyyy");
                    worksheet.Cells["J" + row].Value = string.Join(",", maSuatChieuList);
                    worksheet.Cells["K" + row].Value = string.Join(",", maTlList);
                    row++;
                }
                worksheet.Cells.AutoFitColumns();
                var excelBytes = package.GetAsByteArray();
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment; filename=Phim.xlsx");
                Response.BinaryWrite(excelBytes);
                Response.Flush();
                Response.End();
            }
            return new EmptyResult();
        }
        [ChildActionOnly]
        public ActionResult PhimDangChieuPartial()
        {
            return PartialView();
        }
        [ChildActionOnly]
        public ActionResult PhimSapChieuPartial()
        {
            return PartialView();
        }
        public ActionResult ChiTiet(string MaPhim)
        {
            if (MaPhim == null)
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }
            Phim p = db.Phims.SingleOrDefault(x => x.MaPhim == MaPhim && x.TrangThai != "Ngừng chiếu");
            if (p == null)
            {
                return HttpNotFound();
            }
            var groupRap= db.SuatChieu_Phim
            .Join(
                db.SuatChieus,
                scp => scp.MaSC,
                sc => sc.MaSC,
                (scp, sc) => new {
                    MaPhim = scp.MaPhim,
                    MaSC = sc.MaSC
                })
            .Join(
                db.SuatChieu_Rap,
                sc => sc.MaSC,
                scr => scr.MaSC,
                (sc, scr) => new
                {
                    MaPhim = sc.MaPhim,
                    MaSC = sc.MaSC,
                    MaRap = scr.MaRap
                })
            .Join(
                db.Raps,
                scr => scr.MaRap,
                r => r.MaRap,
                (scr, r) => new
                {
                    MaPhim = scr.MaPhim,
                    MaSC = scr.MaSC,
                    MaRap = scr.MaRap,
                    TenRap = r.TenRap
                }
            );
            var listRap = groupRap.Where(x => x.MaPhim == MaPhim).ToList();
            var groupPhimTheLoai = db.Phim_TheLoai
            .Join(
                db.TheLoais,
                phim_TL => phim_TL.MaTL,
                TL => TL.MaTL,
                (phim_TL, TL) => new {
                    MaTL = phim_TL.MaTL,
                    TenTL = TL.TenTL,
                    MaPhim = phim_TL.MaPhim
                }
            );
            var listTL = groupPhimTheLoai.Where(x => x.MaPhim == MaPhim).ToList();
            var result = new ChiTietPhimViewModel
            {
                MaPhim = p.MaPhim,
                TenPhim = p.TenPhim,
                HinhAnh = p.HinhAnh,
                DaTaTrailer = p.DaTaTrailer,
                ThongTinPhim = p.ThongTinPhim,
                NoiDung = p.NoiDung,
                ThoiLuong = p.ThoiLuong,
                TrangThai = p.TrangThai,
                NgayChieu = p.NgayChieu,
                MaRap = listRap.Select(rp => rp.MaRap).ToList(),
                TenRap = listRap.Select(tr => tr.TenRap).ToList(),
                MaTL = listTL.Select(tl => tl.MaTL).ToList(),
                TenTL = listTL.Select(ttl => ttl.TenTL).ToList()
            };
            return View(result);
        }
        public ActionResult PhanLoaiPhim(string MaTL, int? page)
        {
            if (MaTL == null)
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }
            TheLoai tl = db.TheLoais.SingleOrDefault(x => x.MaTL == MaTL);
            var groupPhimTL = db.Phim_TheLoai
            .Join(
                db.Phims,
                phim_TL => phim_TL.MaPhim,
                phim => phim.MaPhim,
                (phim_TL, phim) => new PhanLoaiPhimViewModel
                {
                    MaTL = phim_TL.MaTL,
                    MaPhim = phim.MaPhim,
                    TenTL = tl.TenTL,
                    TenPhim = phim.TenPhim,
                    HinhAnh = phim.HinhAnh,
                    TrangThai = phim.TrangThai
                }
            );
            var ptl = groupPhimTL.Where(x => x.MaTL == MaTL && x.TrangThai != "Ngừng chiếu").ToList();
            if (ptl.Count == 0)
            {
                return HttpNotFound();
            }
            int PageSize = 18;
            int PageNumber = (page ?? 1);
            ViewBag.MaTL = MaTL;
            return View(ptl.ToPagedList(PageNumber, PageSize));
        }
        
    }
}