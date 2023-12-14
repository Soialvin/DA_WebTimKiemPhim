using DAM.Models;
using DAM.Models.ViewModel;
using DAM.Validation;
using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.EMMA;
using System;
using PagedList;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.Mvc;
using System.IO;
using Microsoft.Ajax.Utilities;
using DocumentFormat.OpenXml.Spreadsheet;

namespace DAM.Controllers
{
    public class VeController : BaseController
    {
        // GET: Ve
        DA_WebTimKiemPhimEntities db = new DA_WebTimKiemPhimEntities();
        public ActionResult Index()
        {
            return View(db.Ves.Where(x => x.TrangThaiVe != "Đã xóa"));
        }
        public ActionResult Xoa(int? MaVe)
        {
            var result = db.Ves.Find(MaVe);
            try
            {
                result.TrangThaiVe = "Đã xóa";
                if (Session["user"] != null)
                {
                    TaiKhoan TKDN = (TaiKhoan)Session["user"];
                    LichSu ls = new LichSu
                    {
                        ThongTinTT = $"Đã xóa vé mã: {result.MaVe}",
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
                        ThongTinTT = $"Error xóa vé. Mã lỗi: {ex.Message}",
                        NgayGioTT = DateTime.Now,
                        TenTK = TKDN.TenTK
                    };
                    db.LichSus.Add(ls);
                }
                db.SaveChanges();
                SetAlert($"Xóa không thành công. Lỗi đã được ghi nhận vui lòng liên hệ Admin", "danger");
                return RedirectToAction("Index");
            }
            
        }
        [HttpGet]
        public ActionResult TaoMoi()
        {
            ViewBag.MaPhim = new SelectList(db.Phims.Where(x => x.TrangThai != "Ngừng chiếu").OrderBy(x => x.TenPhim), "MaPhim", "TenPhim");
            ViewBag.MaRap = new SelectList(db.Raps.Where(x => x.TrangThai != "Ngừng hoạt động").OrderBy(x => x.TenRap), "MaRap", "TenRap");
            ViewBag.NgayChieu = new SelectList(db.SuatChieu_Rap.OrderBy(x => x.NgayChieu), "NgayChieu", "NgayChieu");
            ViewBag.MaSC = new SelectList(db.SuatChieus.Where(x => x.TrangThai != "Đã hủy").OrderBy(x => x.KhungGio), "MaSC", "KhungGio");
            return View();
        }
        [HttpGet]
        public ActionResult TaoMoiCDN()
        {
            RapSCPhimViewModel DatVeCDN = (RapSCPhimViewModel)Session["DatVeCDN"];
            var RapGheVe = db.Ves
           .Join(db.Ghe_Ve, ve => ve.MaVe, gv => gv.MaVe, (ve, gv) => new
           {
               MaRap = ve.MaRap,
               MaVe = ve.MaVe,
               MaGhe = gv.MaGhe,
               MaSC = ve.MaSC,
               MaPhim = ve.MaPhim,
               TrangThaiVe = ve.TrangThaiVe
           });
            var listGhe = RapGheVe.Where(x => x.MaRap == DatVeCDN.MaRap && x.MaSC == DatVeCDN.MaSC && x.MaPhim == DatVeCDN.MaPhim && x.TrangThaiVe == "Chưa xóa");
            var laySC = db.SuatChieus.FirstOrDefault(x => x.MaSC == DatVeCDN.MaSC);
            var layRap = db.Raps.FirstOrDefault(x => x.MaRap == DatVeCDN.MaRap);
            var layPhim = db.Phims.FirstOrDefault(x => x.MaPhim == DatVeCDN.MaPhim);
            var laydl = new RapSCPhimViewModel
            {
                MaPhim = layPhim.MaPhim,
                MaRap = layRap.MaRap,
                MaGhe = listGhe.Select(x => x.MaGhe).ToList(),
                MaSC = laySC.MaSC,
                TenPhim = layPhim.TenPhim,
                TenRap = layRap.TenRap,
                GiaVe = layRap.GiaVe,
                KhungGio = laySC.KhungGio
            };
            return View(laydl);
        }
        [HttpPost]
        public ActionResult SaveToSession(RapSCPhimViewModel model)
        {
            Session["DatVeCDN"] = model;
            return Json(new { success = true });
        }

        [HttpGet]
        public ActionResult DatVeNgay(string MaPhim, string TenPhim)
        {
            ViewBag.MaRap = new SelectList(db.Raps.Where(x => x.TrangThai != "Ngừng hoạt động").OrderBy(x => x.TenRap), "MaRap", "TenRap");
            ViewBag.NgayChieu = new SelectList(db.SuatChieu_Rap.OrderBy(x => x.NgayChieu), "NgayChieu", "NgayChieu");
            ViewBag.MaSC = new SelectList(db.SuatChieus.Where(x => x.TrangThai != "Đã hủy").OrderBy(x => x.KhungGio), "MaSC", "KhungGio");
            return View();
        }
        [HttpPost]
        public ActionResult loadRap(string selectedPhim)
        {
            var rap = db.SuatChieu_Phim
                .Join(db.SuatChieus, scp => scp.MaSC, sc => sc.MaSC,
                (scp, sc) => new
                {
                    MaPhim = scp.MaPhim,
                    MaSC = sc .MaSC
                })
                .Join(db.SuatChieu_Rap, x => x.MaSC, scr => scr.MaSC,
                (x, scr)=> new
                {
                    MaPhim = x.MaPhim,
                    MaSC = x.MaSC,
                    MaRap = scr.MaRap
                })
                .Join(db.Raps, y => y.MaRap, r => r.MaRap,
                (y, r) => new
                {
                    MaPhim = y.MaPhim,
                    MaSC = y.MaSC,
                    MaRap = y.MaRap,
                    TenRap = r.TenRap,
                    TrangThai = r.TrangThai
                }).ToList();
            var listRap = rap.Where(x => x.MaPhim == selectedPhim && x.TrangThai != "Ngừng hoạt động").ToList();
            return Json(listRap, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult loadNgayChieu(string selectedPhim, string selectedRap)
        {
            var ngayChieu = db.SuatChieu_Phim
                .Join(db.SuatChieus, scp => scp.MaSC, sc => sc.MaSC,
                (scp, sc) => new
                {
                    MaPhim = scp.MaPhim,
                    MaSC = sc.MaSC,
                    KhungGio = sc.KhungGio
                })
                .Join(db.SuatChieu_Rap, x => x.MaSC, scr => scr.MaSC,
                (x, scr) => new
                {
                    MaPhim = x.MaPhim,
                    MaSC = x.MaSC,
                    KhungGio = x.KhungGio,
                    MaRap = scr.MaRap,
                    NgayChieu = scr.NgayChieu
                }).ToList();
            var listNgayChieu = ngayChieu.Where(x => x.MaPhim == selectedPhim && x.MaRap == selectedRap && x.NgayChieu >= DateTime.Now).ToList();
            return Json(listNgayChieu, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult loadSC(string selectedRap, DateTime selectedNgayChieu)
        {
            var listSC = db.SuatChieus
                .Join(db.SuatChieu_Rap, sc => sc.MaSC, scr => scr.MaSC,
                (sc,scr) => new
                {
                    MaSC = sc.MaSC,
                    TrangThai = sc.TrangThai,
                    MaRap = scr.MaRap,
                    NgayChieu = scr.NgayChieu,
                    KhungGio = sc.KhungGio
                }).Where(x => x.MaRap == selectedRap && x.NgayChieu == selectedNgayChieu && x.TrangThai != "Đã hủy").ToList();
            return Json(listSC, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult ChonGhe(RapSCPhimViewModel RSCP)
        {
            var RapGheVe = db.Ves
            .Join(db.Ghe_Ve, ve => ve.MaVe, gv => gv.MaVe, (ve, gv) => new
            {
                MaRap = ve.MaRap,
                MaVe = ve.MaVe,
                MaGhe = gv.MaGhe,
                MaSC = ve.MaSC,
                MaPhim = ve.MaPhim,
                TrangThaiVe = ve.TrangThaiVe
            });
            var listGhe = RapGheVe.Where(x => x.MaRap == RSCP.MaRap && x.MaSC == RSCP.MaSC && x.MaPhim == RSCP.MaPhim && x.TrangThaiVe == "Chưa xóa");
            var laySC = db.SuatChieus.FirstOrDefault(x => x.MaSC == RSCP.MaSC);
            var layRap = db.Raps.FirstOrDefault(x => x.MaRap == RSCP.MaRap);
            var layPhim = db.Phims.FirstOrDefault(x => x.MaPhim == RSCP.MaPhim);
            var laydl = new RapSCPhimViewModel
            {
                MaPhim = layPhim.MaPhim,
                MaRap = layRap.MaRap,
                MaGhe = listGhe.Select(x => x.MaGhe).ToList(),
                MaSC = laySC.MaSC,
                TenPhim = layPhim.TenPhim,
                HinhAnh = layPhim.HinhAnh,
                TenRap = layRap.TenRap,
                GiaVe = layRap.GiaVe,
                KhungGio = laySC.KhungGio
            };
            return View(laydl);
        }
        [HttpPost]
        public ActionResult XuLyVe(RapSCPhimViewModel RSCP)
        {
            Session.Remove("DatVeCDN");
            List<string> listGheDaChon = new List<string>(RSCP.MaGheDaChon.Split(','));
            List<string> listPPThanhToan = new List<string> { "Thanh toán qua E-Banking", "Thanh toán qua MoMo", "Thanh toán qua VnPay" };
            ViewBag.PPThanhToan = new SelectList(listPPThanhToan);
            int soLuongGheDaChon = listGheDaChon.Count;
            var laydl = new HoaDonViewModel
            {
                MaPhim = RSCP.MaPhim,
                MaRap = RSCP.MaRap,
                TenPhim = RSCP.TenPhim,
                TenRap = RSCP.TenRap,
                MaGhe = listGheDaChon,
                MaSC = RSCP.MaSC,
                GiaVe = RSCP.GiaVe,
                TongTien = ((decimal)(RSCP.GiaVe * soLuongGheDaChon)),
                KhungGio = RSCP.KhungGio,
                HinhAnh = RSCP.HinhAnh
            };
            return View(laydl);
        }
        public ActionResult LichSuGiaoDich(string TenTK, int? page)
        {
            
            var ls = db.Ves
                .Join(db.HoaDons, v => v.MaVe, hd => hd.MaVe, (v, hd) => new
                {
                    MaHD = hd.MaHD,
                    TenTK = hd.TenTK,
                    MaVe = v.MaVe,
                    NgayDat = hd.NgayDat,
                    TongTien = hd.TongTien,
                    MaSC = v.MaSC,
                    MaPhim = v.MaPhim,
                    MaRap = v.MaRap
                }).Join(db.SuatChieus, y => y.MaSC, sc => sc.MaSC, (y, sc) => new
                {
                    KhungGio = sc.KhungGio,
                    MaHD = y.MaHD,
                    TenTK = y.TenTK,
                    MaVe = y.MaVe,
                    NgayDat = y.NgayDat,
                    TongTien = y.TongTien,
                    MaSC = y.MaSC,
                    MaPhim = y.MaPhim,
                    MaRap = y.MaRap
                }).Join(db.SuatChieu_Rap, w => w.MaSC, scr => scr.MaSC,(w,scr) => new
                {
                    NgayChieu = scr.NgayChieu,
                    MaHD = w.MaHD,
                    KhungGio = w.KhungGio,
                    TenTK = w.TenTK,
                    MaVe = w.MaVe,
                    NgayDat = w.NgayDat,
                    TongTien = w.TongTien,
                    MaSC = w.MaSC,
                    MaPhim = w.MaPhim,
                    MaRap = w.MaRap
                })
                .Join(db.Raps, z => z.MaRap, r => r.MaRap, (z, r) => new
                {
                    TenRap = r.TenRap,
                    MaHD = z.MaHD,
                    NgayChieu = z.NgayChieu,
                    KhungGio = z.KhungGio,
                    TenTK = z.TenTK,
                    MaVe = z.MaVe,
                    NgayDat = z.NgayDat,
                    TongTien = z.TongTien,
                    MaSC = z.MaSC,
                    MaPhim = z.MaPhim,
                    MaRap = z.MaRap
                }).Join(db.Phims, s => s.MaPhim, p => p.MaPhim, (s, p) => new LichSuViewModel
                {

                    TenPhim = p.TenPhim,
                    MaHD = s.MaHD,
                    NgayChieu = s.NgayChieu,
                    TenTK = s.TenTK,
                    TenRap = s.TenRap,
                    KhungGio = s.KhungGio,
                    MaGhe = db.Ghe_Ve.Where(k => k.MaVe == s.MaVe).Select(b => b.MaGhe).ToList(),
                    MaVe = (int)s.MaVe,
                    NgayDat = s.NgayDat,
                    TongTien = s.TongTien,
                    MaSC = s.MaSC,
                    MaPhim = s.MaPhim,
                    MaRap = s.MaRap,
                    HinhAnh = p.HinhAnh
                }).ToList();
            var lstk = ls.Where(x => x.TenTK == TenTK).DistinctBy(x => x.MaHD).ToList();
            int PageSize = 5;
            int PageNumber = (page ?? 1);
            ViewBag.TenTK = TenTK;
            return View(lstk.ToPagedList(PageNumber, PageSize));
        }
        public ActionResult TimKiem(string keyword)
        {
            int a;
            var result = db.Ves.ToList();
            if (string.IsNullOrEmpty(keyword))
            {
                return View("Index", result);
            }
            if (int.TryParse(keyword,out a) )
            {
                result = result.Where(x => (x.MaVe.ToString() != null && x.MaVe.ToString().Contains(a.ToString()))).ToList();
                ViewBag.Keyword = keyword;
                return View("Index", result);
            }
            else
            {
                result = result.Where(x => (x.MaVe.ToString() != null && x.MaVe.ToString().Contains(a.ToString()))).ToList();
                ViewBag.Keyword = keyword;
                return View("Index", result);
            }
        }
    }
}