using DAM.Models;
using DAM.Models.ViewModel;
using DAM.Validation;
using DocumentFormat.OpenXml.Wordprocessing;
using PagedList;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DAM.Controllers
{
    public class KhuyenMaiController : BaseController
    {
        // GET: KhuyenMai
        DA_WebTimKiemPhimEntities db = new DA_WebTimKiemPhimEntities();
        public ActionResult Index()
        {
            var result = db.KhuyenMais.Select(x => new ChiTietKMViewModel
            {
                MaKM = x.MaKM,
                MaRap = x.MaRap,
                TenKM = x.TenKM,
                HinhAnh = x.HinhAnh,
                NoiDung = x.NoiDung,
                NgayBD = x.NgayBD,
                NgayKT = x.NgayKT
            }).ToList();
            return View(result);
        }
        [HttpGet]
        public ActionResult TaoMoi()
        {
            ViewBag.MaRap = new SelectList(db.Raps.OrderBy(x => x.MaRap), "MaRap", "TenRap");
            return View();
        }
        [HttpPost]
        public ActionResult TaoMoi(ChiTietKMViewModel km, HttpPostedFileBase HinhAnh)
        {
            ViewBag.MaRap = new SelectList(db.Raps.OrderBy(x => x.MaRap), "MaRap", "TenRap");
            if (ModelState.IsValid)
            {
                if (HinhAnh == null || HinhAnh.ContentLength == 0)
                {
                    km.HinhAnh = "LogoFilm.png";
                }
                else
                {
                    var filename = Path.GetFileName(HinhAnh.FileName);
                    var path = Path.Combine(Server.MapPath("~/Content/KMImages"), filename);
                    if (System.IO.File.Exists(path))
                    {
                        ViewBag.upload = "Hình ảnh đã tồn tại";
                        return View(km);
                    }
                    else
                    {
                        HinhAnh.SaveAs(path);
                        Session["TenHinh"] = HinhAnh.FileName;
                        ViewBag.TenHinh = "";
                        km.HinhAnh = filename;
                    }
                }
                km.NgayBD = DateTime.Now;
                km.NgayKT = DateTime.Now.AddYears(1);
                var x = new KhuyenMai
                {
                    MaKM = km.MaRap,
                    MaRap = km.MaRap,
                    TenKM = km.TenKM,
                    HinhAnh = km.HinhAnh,
                    NoiDung = km.NoiDung,
                    NgayBD = km.NgayBD,
                    NgayKT = km.NgayKT
                };
                db.KhuyenMais.Add(x);
                try
                {
                    if (Session["user"] != null)
                    {
                        TaiKhoan TKDN = (TaiKhoan)Session["user"];
                        LichSu ls = new LichSu
                        {
                            ThongTinTT = $"Đã tạo mới khuyến mãi: {km.MaKM}",
                            NgayGioTT = DateTime.Now,
                            TenTK = TKDN.TenTK
                        };
                        db.LichSus.Add(ls);
                    }
                    db.SaveChanges();
                    SetAlert("Thêm mới thành công", "sucsess");
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    if (Session["user"] != null)
                    {
                        TaiKhoan TKDN = (TaiKhoan)Session["user"];
                        LichSu ls = new LichSu
                        {
                            ThongTinTT = $"Error tạo mới khuyến mãi. Mã lỗi: {ex.Message}",
                            NgayGioTT = DateTime.Now,
                            TenTK = TKDN.TenTK
                        };
                        db.LichSus.Add(ls);
                    }
                    db.SaveChanges();
                    SetAlert($"Thêm mới không thành công. Lỗi đã được ghi nhận vui lòng liên hệ Admin", "danger");
                    return View("TaoMoi");
                }
            }
            return View("TaoMoi");
        }
        [HttpGet]
        public ActionResult Sua(String maKM)
        {
            if (maKM == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            KhuyenMai km = db.KhuyenMais.SingleOrDefault(x => x.MaKM == maKM);
            ViewBag.MaRap = new SelectList(db.Raps.OrderBy(x => x.MaRap), "MaRap", "TenRap",km.MaRap);
            Session["Anh"] = km;
            if (km == null)
            {
                return HttpNotFound();
            }
            var result = new ChiTietKMViewModel
            {
                MaKM = km.MaKM,
                MaRap = km.MaRap,
                TenKM = km.TenKM,
                HinhAnh = km.HinhAnh,
                NoiDung = km.NoiDung,
                NgayBD = km.NgayBD,
                NgayKT = km.NgayKT
            };
            return View(result);
        }
        [HttpPost]
        public ActionResult Sua(KhuyenMai km, HttpPostedFileBase HinhAnh)
        {
            ViewBag.MaRap = new SelectList(db.Raps.OrderBy(x => x.MaRap), "MaRap", "TenRap", km.MaRap);
            try
            {
                if (ModelState.IsValid)
                {
                    var x = db.KhuyenMais.Find(km.MaKM);
                    if (x == null)
                    {
                        return HttpNotFound();
                    }
                    if (HinhAnh != null && HinhAnh.ContentLength != 0)
                    {
                        var fileName = Path.GetFileName(HinhAnh.FileName);
                        var path1 = Path.Combine(Server.MapPath("~/Content/KMImages"), fileName);
                        if (System.IO.File.Exists(path1))
                        {
                            ViewBag.upload = "Hình đã tồn tại";
                            return View(km);
                        }
                        else
                        {
                            var anh = Session["Anh"] as KhuyenMai;
                            if (anh.HinhAnh != null)
                            {
                                var path2 = Path.Combine(Server.MapPath("~/Content/KMImages"), anh.HinhAnh);
                                if (System.IO.File.Exists(path2))
                                {
                                    System.IO.File.Delete(path2);
                                }
                            }
                            HinhAnh.SaveAs(path1);
                            Session["TenHinh"] = HinhAnh.FileName;
                            ViewBag.TenHinh = "";
                            km.HinhAnh = fileName;
                        }
                    }
                    else
                    {
                        var anh = Session["Anh"] as KhuyenMai;
                        km.HinhAnh = anh.HinhAnh;
                    }
                    x.TenKM = km.TenKM;
                    x.MaRap = km.MaRap;
                    x.HinhAnh = km.HinhAnh;
                    x.NoiDung = km.NoiDung;
                    x.NgayBD = km.NgayBD;
                    x.NgayKT = km.NgayKT;
                    if (Session["user"] != null)
                    {
                        TaiKhoan TKDN = (TaiKhoan)Session["user"];
                        LichSu ls = new LichSu
                        {
                            ThongTinTT = $"Đã cập nhật khuyến mãi: {km.MaKM}",
                            NgayGioTT = DateTime.Now,
                            TenTK = TKDN.TenTK
                        };
                        db.LichSus.Add(ls);
                    }
                    db.SaveChanges();
                    Session.Remove("Anh");
                    SetAlert("Cập nhật thành công", "sucsess");
                    return RedirectToAction("Index");

                }
                return View(km);
            }
            catch (Exception ex)
            {
                if (Session["user"] != null)
                {
                    TaiKhoan TKDN = (TaiKhoan)Session["user"];
                    LichSu ls = new LichSu
                    {
                        ThongTinTT = $"Error cập nhật khuyến mãi. Mã lỗi: {ex.Message}",
                        NgayGioTT = DateTime.Now,
                        TenTK = TKDN.TenTK
                    };
                    db.LichSus.Add(ls);
                }
                db.SaveChanges();
                SetAlert($"Cập nhật không thành công. Lỗi đã được ghi nhận vui lòng liên hệ Admin", "danger");
                return View(km);
            }
        }
        public ActionResult Xoa(string maKM)
        {
            var result = db.KhuyenMais.Find(maKM);
            db.KhuyenMais.Remove(result);
            db.KhuyenMais.Remove(result);
            try
            {
                if (result.HinhAnh != null)
                {
                    var path = Path.Combine(Server.MapPath("~/Content/KMImages"), result.HinhAnh);
                    if (System.IO.File.Exists(path))
                    {
                        System.IO.File.Delete(path);
                    }
                }
                if (Session["user"] != null)
                {
                    TaiKhoan TKDN = (TaiKhoan)Session["user"];
                    LichSu ls = new LichSu
                    {
                        ThongTinTT = $"Đã xóa khuyến mãi: {result.MaKM}",
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
                        ThongTinTT = $"Error xóa khuyến mãi. Mã lỗi: {ex.Message}",
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
        public ActionResult TimKiem(string keyword)
        {
            var result = db.KhuyenMais.Select(x => new ChiTietKMViewModel
            {
                MaKM = x.MaKM,
                MaRap = x.MaRap,
                TenKM = x.TenKM,
                HinhAnh = x.HinhAnh,
                NoiDung = x.NoiDung,
                NgayBD = x.NgayBD,
                NgayKT = x.NgayKT,
            }).ToList();
            if (string.IsNullOrEmpty(keyword))
            {
                return View("Index", result);
            }
            result = result.Where(x => (x.MaKM != null && RemoveDiacritics(x.MaKM).ToLower().Contains(RemoveDiacritics(keyword).ToLower())) || (x.TenKM != null && RemoveDiacritics(x.TenKM).ToLower().Contains(RemoveDiacritics(keyword).ToLower())) || (x.MaRap != null && RemoveDiacritics(x.MaRap).ToLower().Contains(RemoveDiacritics(keyword).ToLower()))).ToList();
            ViewBag.Keyword = keyword;
            return View("Index", result);
        }
        public ActionResult KhuyenMaiPartial()
        {
            DateTime ngayHienTai = DateTime.ParseExact(DateTime.Now.ToString("yyyy/MM/dd"), "yyyy/MM/dd", CultureInfo.InvariantCulture);
            var listKM = db.KhuyenMais.Where(x => x.NgayKT >= ngayHienTai);
            return PartialView(listKM);
        }
        public ActionResult AllKhuyenMai(int? page)
        {
            DateTime ngayHienTai = DateTime.ParseExact(DateTime.Now.ToString("yyyy/MM/dd"), "yyyy/MM/dd", CultureInfo.InvariantCulture);
            var listKM = db.KhuyenMais.Where(x => x.NgayKT >= ngayHienTai).ToList();
            if (listKM.Count == 0)
            {
                return HttpNotFound();
            }
            int PageSize = 18;
            int PageNumber = (page ?? 1);
            return View(listKM.ToPagedList(PageNumber, PageSize));
        }
        public ActionResult ChiTietKM(string MaKM)
        {
            if (MaKM == null)
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }
            KhuyenMai km = db.KhuyenMais.SingleOrDefault(x => x.MaKM == MaKM);
            if (km == null)
            {
                return HttpNotFound();
            }
            var groupKMRap = db.KhuyenMais
            .Join(
                db.Raps,
                KM => KM.MaRap,
                rap => rap.MaRap,
                (KM, rap) => new ChiTietKMViewModel
                {
                    MaKM = KM.MaKM,
                    TenKM = KM.TenKM,
                    HinhAnh = KM.HinhAnh,
                    NoiDung = KM.NoiDung,
                    NgayBD = KM.NgayBD,
                    NgayKT = KM.NgayKT,
                    MaRap = rap.MaRap,
                    TenRap = rap.TenRap,
                    DiaChi = rap.DiaChi
                }
                ).SingleOrDefault(x => x.MaKM == MaKM);
            return View(groupKMRap);
        }
    }
}