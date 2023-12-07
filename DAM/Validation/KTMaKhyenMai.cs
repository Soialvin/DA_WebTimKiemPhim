using DAM.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DAM.Validation
{
    public class KTMaKhyenMai : ValidationAttribute
    {
        DA_WebTimKiemPhimEntities db = new DA_WebTimKiemPhimEntities();
        public KTMaKhyenMai() => ErrorMessage = "Mã khuyến mãi đã tồn tại";
        public override bool IsValid(object value)
        {
            if (value == null)
            {
                return false;
            }
            else
            {
                var httpContext = new HttpContextWrapper(HttpContext.Current);
                var MaKMGoc = httpContext.Request.Form["MaKMGoc"];
                string kt = value as string;
                if (string.IsNullOrEmpty(MaKMGoc) || kt != MaKMGoc)
                {
                    bool check = db.KhuyenMais.Any(x => x.MaKM == kt);
                    return !check;
                }
                return true;
            }
        }
    }
}