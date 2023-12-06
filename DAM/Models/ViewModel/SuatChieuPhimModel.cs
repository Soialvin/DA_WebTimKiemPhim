using DAM.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DAM.Models.ViewModel
{
    public class SuatChieuPhimModel
    {
        [DisplayName("Mã Suất Chiếu")]
        [Required(ErrorMessage = "{0} không được để trống")]
        [KTMaSC]
        public string MaSC { get; set; }
        [DisplayName("Khung giờ")]
        //chưa fix dc lỗi nhập kí tự vào giờ
        [Required(ErrorMessage = "Giờ phút là bắt buộc.")]
        [RegularExpression(@"^([01]?[0-9]|2[0-3]):[0-5][0-9]:[0-5][0-9]$", ErrorMessage = "Nhập giờ, phút, giây theo định dạng HH:mm:ss")]
        public Nullable<System.TimeSpan> KhungGio { get; set; }
        [DisplayName("Loại chiếu")]
        public string LoaiChieu { get; set; }
        [DisplayName("Trạng thái")]
        public string TrangThai { get; set; }
    }
}