using DAM.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DAM.Validation
{
    public class KTSDT : ValidationAttribute
    {
        DA_WebTimKiemPhimEntities db = new DA_WebTimKiemPhimEntities();
        public KTSDT() => ErrorMessage = "SDT đã được sử dụng";
        public override bool IsValid(object value)
        {
            if (value == null)
            {
                return false;
            }
            else
            {
                string std = value as string;

                // Lấy SDT ban đầu từ trường ẩn trong trường hợp chỉnh sửa
                var httpContext = new HttpContextWrapper(HttpContext.Current);
                var SDTGoc = httpContext.Request.Form["SDTGoc"]; // Tên trường ẩn

                if (string.IsNullOrEmpty(SDTGoc) || std != SDTGoc)
                {
                    // Kiểm tra trùng lặp khi tạo mới hoặc SDT thay đổi
                    bool kt = db.TaiKhoans.Any(x => x.SoDienThoai == std);
                    return !kt;
                }
                return true;
            }
        }
    }
}