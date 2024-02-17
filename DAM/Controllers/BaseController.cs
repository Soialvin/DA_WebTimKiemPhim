using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace DAM.Controllers
{
    public class BaseController : Controller
    {
        protected void SetAlert(string message, string type)
        {
            TempData["AlertMessage"] = message;
            switch (type)
            {
                case "sucsess":
                    TempData["AlertType"] = "alert-success"; break;
                case "warning":
                    TempData["AlertType"] = "alert-warning"; break;
                case "danger":
                    TempData["AlertType"] = "alert-danger"; break;
                default: TempData["AlertType"] = ""; break;
            }
        }
        public static string RemoveDiacritics(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return string.Empty;
            }
            string normalizedString = input.Normalize(NormalizationForm.FormD);
            Regex regex = new Regex("[^a-zA-Z0-9 đĐ]");
            string result = regex.Replace(normalizedString, "");
            result = result.Replace("đ", "d").Replace("Đ", "D");
            return result;
        }
    }
}