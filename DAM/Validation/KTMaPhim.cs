using DAM.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DAM.Validation
{
    public class KTMaPhim : ValidationAttribute
    {
        DA_WebTimKiemPhimEntities db = new DA_WebTimKiemPhimEntities();
        public KTMaPhim() => ErrorMessage = "Mã phim đã tồn tại";
        public override bool IsValid(object value)
        {
            if (value == null)
            {
                return false;
            }
            else
            {
                var httpContext = new HttpContextWrapper(HttpContext.Current);
                var MaPhimGoc = httpContext.Request.Form["MaPhimGoc"];
                string kt = value as string;
                if (string.IsNullOrEmpty(MaPhimGoc) || kt != MaPhimGoc)
                {
                    bool check = db.Phims.Any(x => x.MaPhim == kt);
                    return !check;
                }
                return true;
            }
        }
    }
}