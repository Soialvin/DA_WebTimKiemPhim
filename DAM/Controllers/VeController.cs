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
            ViewBag.MaSC = new SelectList(db.SuatChieus.OrderBy(x => x.KhungGio), "MaSC", "KhungGio");
            List<string> listTrangThai = new List<string> { "Chưa xóa", "Đã xóa" };
            return View();
        }
        [HttpPost]
        public ActionResult ChonGhe(RapSCPhimViewModel RSCP)
        {
            ViewBag.MaPhim = new SelectList(db.Phims.OrderBy(x => x.TenPhim), "MaPhim", "TenPhim");
            ViewBag.MaRap = new SelectList(db.Raps.OrderBy(x => x.TenRap), "MaRap", "TenRap");
            ViewBag.MaSC = new SelectList(db.SuatChieus.OrderBy(x => x.KhungGio), "MaSC", "KhungGio");
            
            List<string> listTrangThai = new List<string> { "Chưa xóa", "Đã xóa" };
            var RapGheVe = db.Raps
                .Join(db.Ves, rap => rap.MaRap, ve => ve.MaRap,
                (rap, ve) => new { MaRap = rap.MaRap, MaVe = ve.MaVe })
                .Join(db.Ghe_Ve, x => x.MaVe, y => y.MaVe, (x, y) => new
                {
                    MaRap = x.MaRap,
                    MaVe = x.MaVe,
                    MaGhe = y.MaGhe
                });
            var laySC = db.SuatChieus.FirstOrDefault(x => x.MaSC == RSCP.MaSC);
            var layRap = db.Raps.FirstOrDefault(x => x.MaRap == RSCP.MaRap);
            var layPhim = db.Phims.FirstOrDefault(x => x.MaPhim == RSCP.MaPhim);
            var laydl = new RapSCPhimViewModel
            {
                MaPhim = RSCP.MaPhim,
                MaRap = RSCP.MaRap,
                MaGhe = RapGheVe.Select(x => x.MaGhe).ToList(),
                MaSC = laySC.MaSC,
                TenPhim = layPhim.TenPhim,
                TenRap = layRap.TenRap,
                GiaVe = layRap.GiaVe.ToString(),
                KhungGio = laySC.KhungGio
            };
            return View(laydl);
        }
        [HttpPost]
        public ActionResult XuLyVe(RapSCPhimViewModel RSCP)
        {
            ViewBag.MaPhim = new SelectList(db.Phims.OrderBy(x => x.TenPhim), "MaPhim", "TenPhim");
            ViewBag.MaRap = new SelectList(db.Raps.OrderBy(x => x.TenRap), "MaRap", "TenRap");
            ViewBag.MaSC = new SelectList(db.SuatChieus.OrderBy(x => x.KhungGio), "MaSC", "KhungGio");
            List<string> listGheDaChon = new List<string>(RSCP.MaGheDaChon.Split(','));
            int soLuongGheDaChon = listGheDaChon.Count;
            decimal giaVe = decimal.Parse(RSCP.GiaVe);
            var laydl = new RapSCPhimViewModel
            {
                MaPhim = RSCP.MaPhim,
                MaRap = RSCP.MaRap,
                TenPhim = RSCP.TenPhim,
                TenRap = RSCP.TenRap,
                MaGhe = listGheDaChon,
                MaSC = RSCP.KhungGio.ToString(),
                GiaVe = (giaVe * soLuongGheDaChon).ToString(),
                KhungGio = RSCP.KhungGio
            };
            return View(laydl);
        }
    }
}