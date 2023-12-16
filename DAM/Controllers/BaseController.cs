using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
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
        protected string RemoveDiacritics(string input)
        {
            return new string(input
                .Normalize(NormalizationForm.FormD)
                .ToCharArray()
                .Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                .ToArray());
        }
    }
}