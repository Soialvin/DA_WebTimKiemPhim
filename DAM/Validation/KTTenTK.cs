using DAM.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DAM.Validation
{
    public class KTTenTK : ValidationAttribute
    {
        DA_WebTimKiemPhimEntities db = new DA_WebTimKiemPhimEntities();
        public KTTenTK() => ErrorMessage = "Tên tài khoản đã được sử dụng";
        public override bool IsValid(object value)
        {
            if (value == null)
            {
                return false;
            }
            else
            {
                var httpContext = new HttpContextWrapper(HttpContext.Current);
                var TenTKGoc = httpContext.Request.Form["TenTKGoc"];
                string kt = value as string;
                if (string.IsNullOrEmpty(TenTKGoc) || kt != TenTKGoc)
                {
                    bool check = db.TaiKhoans.Any(x => x.TenTK == kt);
                    return !check;
                }
                return true;
            }
        }
    }
}