using DAM.Models;
using DAM.Models.ViewModel;
using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.EMMA;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.Mvc;

namespace DAM.Controllers
{
    public class VeController : BaseController
    {
        // GET: Ve
        DA_WebTimKiemPhimEntities db = new DA_WebTimKiemPhimEntities();
        public ActionResult Index()
        {
            return View(db.Ves);
        }
        [HttpGet]
        public ActionResult TaoMoi()
        {
            ViewBag.MaPhim = new SelectList(db.Phims.OrderBy(x => x.TenPhim), "MaPhim", "TenPhim");
            ViewBag.MaRap = new SelectList(db.Raps.OrderBy(x => x.TenRap), "MaRap", "TenRap");
            ViewBag.NgayChieu = new SelectList(db.SuatChieu_Rap.OrderBy(x => x.NgayChieu), "NgayChieu", "NgayChieu");
            ViewBag.MaSC = new SelectList(db.SuatChieus.OrderBy(x => x.KhungGio), "MaSC", "KhungGio");
            List<string> listTrangThai = new List<string> { "Chưa xóa", "Đã xóa" };
            return View();
        }
        [HttpGet]
        public ActionResult DatVeNgay(string MaPhim, string TenPhim)
        {
            ViewBag.MaRap = new SelectList(db.Raps.OrderBy(x => x.TenRap), "MaRap", "TenRap");
            ViewBag.NgayChieu = new SelectList(db.SuatChieu_Rap.OrderBy(x => x.NgayChieu), "NgayChieu", "NgayChieu");
            ViewBag.MaSC = new SelectList(db.SuatChieus.OrderBy(x => x.KhungGio), "MaSC", "KhungGio");
            List<string> listTrangThai = new List<string> { "Chưa xóa", "Đã xóa" };
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
                    TenRap = r.TenRap
                }).ToList();
            var listRap = rap.Where(x => x.MaPhim == selectedPhim).ToList();
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
            var listNgayChieu = ngayChieu.Where(x => x.MaPhim == selectedPhim && x.MaRap == selectedRap).ToList();
            return Json(listNgayChieu, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult loadSC(string selectedRap, DateTime selectedNgayChieu)
        {
            var listSC = db.SuatChieu_Rap.Where(x => x.MaRap == selectedRap && x.NgayChieu == selectedNgayChieu).ToList();
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
                TenRap = layRap.TenRap,
                GiaVe = layRap.GiaVe,
                KhungGio = laySC.KhungGio
            };
            return View(laydl);
        }
        [HttpPost]
        public ActionResult XuLyVe(RapSCPhimViewModel RSCP)
        {
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
                KhungGio = RSCP.KhungGio
            };
            return View(laydl);
        }
    }
}