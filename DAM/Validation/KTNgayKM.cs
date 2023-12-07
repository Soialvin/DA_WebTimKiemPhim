using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using DAM.Models.ViewModel;
using Microsoft.Ajax.Utilities;

namespace DAM.Validation
{
    public class KTNgayKM : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var ngayBD = (DateTime?)value;
            var ngayKT = (DateTime?)validationContext.ObjectType.GetProperty("NgayKT").GetValue(validationContext.ObjectInstance);

            if (ngayBD.HasValue && ngayKT.HasValue && ngayBD.Value.Date > ngayKT.Value.Date)
            {
                return new ValidationResult(ErrorMessage ?? "Ngày bắt đầu phải nhỏ hơn ngày kết thúc khuyến mãi");
            }

            return ValidationResult.Success;
        }
    }
}