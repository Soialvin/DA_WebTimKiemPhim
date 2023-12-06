using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using DAM.Models.ViewModel;
using Microsoft.Ajax.Utilities;

namespace DAM.Validation
{
    public class KTNgayChieu : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var trangThai = validationContext.ObjectType.GetProperty("TrangThai");
            var trangThaiValue = trangThai.GetValue(validationContext.ObjectInstance);

            if (trangThaiValue.ToString() == "Đang chiếu" || trangThaiValue.ToString() == "Ngừng chiếu")
            {
                var ngayChieu = (DateTime?)value;
                if (ngayChieu.HasValue && ngayChieu.Value.Date > DateTime.Now.Date)
                {
                    return new ValidationResult(ErrorMessage ?? "Ngày chiếu phải nhỏ hơn hoặc bằng ngày hiện tại");
                }
                return ValidationResult.Success;
            }
            else
            {
                var ngayChieu = (DateTime?)value;
                if (ngayChieu.HasValue && ngayChieu.Value.Date < DateTime.Now.Date)
                {
                    return new ValidationResult(ErrorMessage ?? "Ngày chiếu phải lớn hơn hoặc bằng ngày hiện tại");
                }
                return ValidationResult.Success;
            }
        }
    }
}