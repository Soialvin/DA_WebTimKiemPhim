using DAM.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DAM.Validation
{
    public class KTMaSC : ValidationAttribute
    {
        public KTMaSC() => ErrorMessage = "Mã suất chiếu đã tồn tại";
        DA_WebTimKiemPhimEntities db = new DA_WebTimKiemPhimEntities();
        public override bool IsValid(object value)
        {
            if (value == null)
            {
                return false;
            }
            else
            {
                var httpContext = new HttpContextWrapper(HttpContext.Current);
                var MaSCGoc = httpContext.Request.Form["MaSCGoc"];
                string kt = value as string;
                if (string.IsNullOrEmpty(MaSCGoc) || kt != MaSCGoc)
                {
                    bool check = db.SuatChieus.Any(x => x.MaSC == kt);
                    return !check;
                }
                return true;
            }
        }
    }
}