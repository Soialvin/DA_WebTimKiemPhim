using DAM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DAM.Controllers
{
    public class YeuThichController : BaseController
    {
        // GET: YeuThich
        DA_WebTimKiemPhimEntities db = new DA_WebTimKiemPhimEntities();
        [HttpPost]
        public ActionResult YeuThich(int value, string MaPhim)
        {
            try
            {
                if (Session["user"] != null)
                {
                    TaiKhoan TKDN = (TaiKhoan)Session["user"];
                    if (value == 1)
                    {
                        var tkyt = db.YeuThichs.FirstOrDefault(x => x.TenTK == TKDN.TenTK);
                        if (tkyt != null)
                        {
                            var ytp = new YeuThich_Phim
                            {
                                MaYT = tkyt.MaYT,
                                MaPhim = MaPhim
                            };
                            db.YeuThich_Phim.Add(ytp);
                        }
                        else
                        {
                            var yt = new YeuThich
                            {
                                TenTK = TKDN.TenTK
                            };
                            db.YeuThichs.Add(yt);
                            var ytp = new YeuThich_Phim
                            {
                                MaYT = yt.MaYT,
                                MaPhim = MaPhim
                            };
                            db.YeuThich_Phim.Add(ytp);
                        }
                    }
                    else
                    {
                        var tkyt = db.YeuThichs.FirstOrDefault(x => x.TenTK == TKDN.TenTK);
                        if (tkyt != null)
                        {
                            var pyt = db.YeuThich_Phim.FirstOrDefault(x => x.MaPhim == MaPhim && x.MaYT == tkyt.MaYT);
                            db.YeuThich_Phim.Remove(pyt);
                        }
                    }
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                if (Session["user"] != null)
                {
                    TaiKhoan TKDN = (TaiKhoan)Session["user"];
                    LichSu ls = new LichSu
                    {
                        ThongTinTT = $"Error chức năng yêu thích. Mã lỗi: {ex.Message}",
                        NgayGioTT = DateTime.Now,
                        TenTK = TKDN.TenTK
                    };
                    db.LichSus.Add(ls);
                }
                db.SaveChanges();
            }
            return RedirectToAction("Main","Main");
        }
        [HttpGet]
        public ActionResult Top7YTPartial()
        {
            var top = db.YeuThich_Phim
                .GroupBy(a => a.MaPhim)
                .Select(b => new
                {
                    MaPhim = b.Key,
                    LuotThich = b.Count()
                })
                .OrderByDescending(c => c.LuotThich).ToList();
            var listMaPhim = top.Select(x => x.MaPhim).ToList();
            var phim = db.Phims.Where(x => listMaPhim.Contains(x.MaPhim) && x.TrangThai != "Ngừng chiếu").ToList();
            return PartialView(phim);
        }
        [HttpGet]
        public ActionResult TKPYTPartial()
        {
            if (Session["user"] != null)
            {
                TaiKhoan TKDN = (TaiKhoan)Session["user"];
                var tkyt = db.YeuThichs.FirstOrDefault(x => x.TenTK == TKDN.TenTK);
                var listPhim = db.YeuThich_Phim.Where(x => x.MaYT == tkyt.MaYT).Select(x => x.MaPhim).ToList();
                var phim = db.Phims.Where(x => listPhim.Contains(x.MaPhim) && x.TrangThai != "Ngừng chiếu").ToList();
                return PartialView(phim);
            }
            return new EmptyResult();
        }
    }
}