using DAM.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace DAM.Validation
{
    public class KhoangTrangTiengVietTenTK : ValidationAttribute
    {
        public KhoangTrangTiengVietTenTK() => ErrorMessage = "Tên tài khoản không chứa khoảng trắng, tiếng việt, kí tự đặc biệt";
        public override bool IsValid(object value)
        {
            if (value == null)
            {
                return false;
            }
            string kt = value as string;
            string pattern = "^[a-zA-Z0-9]+$";
            if (Regex.IsMatch(kt, pattern))
            {
                return true;
            }
            return false;
        }
    }
}