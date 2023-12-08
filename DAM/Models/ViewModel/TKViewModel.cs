using DAM.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace DAM.Models.ViewModel
{
    public class TKViewModel
    {
        [DisplayName("Tên tài khoản")]
        [Required(ErrorMessage = "{0} không được để trống")]
        [KTTenTK]
        [KhoangTrangTiengVietTenTK]
        public string TenTK { get; set; }
        [DisplayName("Mật khẩu")]
        [Required(ErrorMessage = "{0} không được để trống")]
        [TiengVietMK]
        public string MatKhau { get; set; }
        [DisplayName("Họ và tên")]
        [Required(ErrorMessage = "{0} không được để trống")]
        public string HoVaTen { get; set; }
        [DisplayName("Hình ảnh")]
        public string HinhAnh { get; set; }
        [DisplayName("Địa chỉ")]
        public string DiaChi { get; set; }
        [DisplayName("Số điện thoại")]
        [RegularExpression(@"^[0-9]*$", ErrorMessage = "Số điện thoại chỉ được nhập số.")]
        [Required(ErrorMessage = "{0} không được để trống")]
        [KTSDT]
        public string SoDienThoai { get; set; }
        [DisplayName("Email")]
        [Required(ErrorMessage = "{0} không được để trống")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ.")]
        [KTEmail]
        public string Email { get; set; }
        [DisplayName("Loại Tài khoản")]
        public string LoaiTK { get; set; }
        [DisplayName("Ngày đăng kí")]
        public System.DateTime NgayDK { get; set; }
        [DisplayName("Trạng thái")]
        public string TrangThai { get; set; }
    }
}