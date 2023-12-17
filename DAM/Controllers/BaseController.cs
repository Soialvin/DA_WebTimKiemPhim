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
            return ConvertToVietnameseWithoutDiacritics(input);
        }

        protected string ConvertToVietnameseWithoutDiacritics(string input)
        {
            Dictionary<char, string> diacriticMap = new Dictionary<char, string>
        {
            {'à', "a"}, {'á', "a"}, {'ả', "a"}, {'ã', "a"}, {'ạ', "a"},
            {'ă', "a"}, {'ằ', "a"}, {'ắ', "a"}, {'ẳ', "a"}, {'ẵ', "a"}, {'ặ', "a"},
            {'â', "a"}, {'ầ', "a"}, {'ấ', "a"}, {'ẩ', "a"}, {'ẫ', "a"}, {'ậ', "a"},
            {'è', "e"}, {'é', "e"}, {'ẻ', "e"}, {'ẽ', "e"}, {'ẹ', "e"},
            {'ê', "e"}, {'ề', "e"}, {'ế', "e"}, {'ể', "e"}, {'ễ', "e"}, {'ệ', "e"},
            {'ì', "i"}, {'í', "i"}, {'ỉ', "i"}, {'ĩ', "i"}, {'ị', "i"},
            {'ò', "o"}, {'ó', "o"}, {'ỏ', "o"}, {'õ', "o"}, {'ọ', "o"},
            {'ô', "o"}, {'ồ', "o"}, {'ố', "o"}, {'ổ', "o"}, {'ỗ', "o"}, {'ộ', "o"},
            {'ơ', "o"}, {'ờ', "o"}, {'ớ', "o"}, {'ở', "o"}, {'ỡ', "o"}, {'ợ', "o"},
            {'ù', "u"}, {'ú', "u"}, {'ủ', "u"}, {'ũ', "u"}, {'ụ', "u"},
            {'ư', "u"}, {'ừ', "u"}, {'ứ', "u"}, {'ử', "u"}, {'ữ', "u"}, {'ự', "u"},
            {'ỳ', "y"}, {'ý', "y"}, {'ỷ', "y"}, {'ỹ', "y"}, {'ỵ', "y"},
            {'đ', "d"}, {'Đ', "D"},
        };

            StringBuilder result = new StringBuilder();

            foreach (char c in input)
            {
                if (diacriticMap.ContainsKey(c))
                {
                    result.Append(diacriticMap[c]);
                }
                else
                {
                    result.Append(c);
                }
            }

            return result.ToString();
        }
    }
}