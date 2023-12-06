using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace DAM.Validation
{
    public class TiengVietMK : ValidationAttribute
    {
        public TiengVietMK() => ErrorMessage = "Mật khẩu không chứa khoảng trắng và chữ tiếng việt";
        public override bool IsValid(object value)
        {
            if (value == null)
            {
                return false;
            }
            string kt = value as string;
            string pattern = @"^[a-zA-Z0-9\W]+$";
            if (Regex.IsMatch(kt, pattern))
            {
                return true;
            }
            return false;
        }
    }
}