using DAM.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace DAM.Validation
{
    public class KTEmail : ValidationAttribute
    {
        DA_WebTimKiemPhimEntities db = new DA_WebTimKiemPhimEntities();
        public KTEmail() => ErrorMessage = "Email đã được sử dụng";
        public override bool IsValid(object value)
        {
            if (value == null)
            {
                return true;
            }
            else
            {
                var httpContext = new HttpContextWrapper(HttpContext.Current);
                var EmailGoc = httpContext.Request.Form["EmailGoc"];
                string kt = value as string;
                if (string.IsNullOrEmpty(EmailGoc) || kt != EmailGoc)
                {
                    bool check = db.TaiKhoans.Any(x => x.Email == kt);
                    return !check;
                }
                return true;
            }
        }
    }
}