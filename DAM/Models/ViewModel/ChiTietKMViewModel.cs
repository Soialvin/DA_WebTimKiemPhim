using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using DAM.Validation;
namespace DAM.Models.ViewModel
{
    public class ChiTietKMViewModel
    {
        [DisplayName("Mã khuyến mãi")]
        [Required(ErrorMessage = "{0} không được để trống")]
        [KhoangTrangTiengVietKhuyenMai]
        public string MaKM { get; set; }
        [DisplayName("Tên khuyến mãi")]
        [Required(ErrorMessage = "{0} không được để trống")]
        public string TenKM { get; set; }
        [DisplayName("Hình ảnh")]
        public string HinhAnh { get; set; }
        [DisplayName("Nội dung")]
        public string NoiDung { get; set; }
        [DisplayName("Ngày bắt đầu")]
        [KTNgayKM(ErrorMessage = "Ngày bắt đầu không được lớn hơn ngày kết thúc khuyến mãi")]
        public System.DateTime NgayBD { get; set; }
        [DisplayName("Ngày kết thúc")]
        public System.DateTime NgayKT { get; set; }
        [DisplayName("Tên rạp")]
        public string MaRap { get; set; }
        [DisplayName("Tên rạp")]
        public string TenRap { get; set; }
        [DisplayName("Địa chỉ")]
        public string DiaChi { get; set; }
    }
}