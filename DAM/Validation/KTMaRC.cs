using DAM.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DAM.Validation
{
    public class KTMaRC : ValidationAttribute
    {
        DA_WebTimKiemPhimEntities db = new DA_WebTimKiemPhimEntities();
        public KTMaRC() => ErrorMessage = "Mã Rạp chiếu đã tồn tại";
        public override bool IsValid(object value)
        {
            if (value == null)
            {
                return false;
            }
            else
            {
                var httpContext = new HttpContextWrapper(HttpContext.Current);
                var MaRCGoc = httpContext.Request.Form["MaRCGoc"];
                string kt = value as string;
                if (string.IsNullOrEmpty(MaRCGoc) || kt != MaRCGoc)
                {
                    bool check = db.Raps.Any(x => x.MaRap == kt);
                    return !check;
                }
                return true;
            }
        }
    }
}