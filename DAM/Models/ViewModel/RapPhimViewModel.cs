using DAM.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DAM.Models.ViewModel
{
    public class RapPhimViewModel
    {
        [DisplayName("Mã Rạp")]
        
        [Required(ErrorMessage = "{0} không được để trống")]
        [KTMaRC]
        public string MaRap { get; set; }
        [DisplayName("Tên rạp")]
        [Required(ErrorMessage = "{0} không được để trống")]
        public string TenRap { get; set; }
        [DisplayName("Địa chỉ")]
        [Required(ErrorMessage = "{0} không được để trống")]
        public string DiaChi { get; set; }

        [DisplayName("Giá vé")]
        
        public Nullable<decimal> GiaVe { get; set; }
        [DisplayName("Trạng thái")]
        public string TrangThai { get; set; }
    }
}