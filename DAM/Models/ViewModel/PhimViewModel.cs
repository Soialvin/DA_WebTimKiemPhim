using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using DAM.Validation;

namespace DAM.Models.ViewModel
{
    public class PhimViewModel
    {
        [DisplayName("Mã phim")]
        [Required(ErrorMessage = "{0} không được để trống")]
        [KTMaPhim]
        [KhoangTrangTiengVietMaPhim]
        public string MaPhim { get; set; }
        [DisplayName("Tên phim")]
        [Required(ErrorMessage = "{0} không được để trống")]
        public string TenPhim { get; set; }
        [DisplayName("Hình ảnh")]
        public string HinhAnh { get; set; }
        [DisplayName("Mã Trailer")]
        public string DaTaTrailer {  get; set; }
        [DisplayName("Thông tin phim")]
        public string ThongTinPhim { get; set; }
        [DisplayName("Nội dung")]
        public string NoiDung { get; set; }
        [DisplayName("Thời lượng")]
        [Required(ErrorMessage = "{0} không được để trống")]
        [Range(1,180,ErrorMessage = "{0} phải từ {1} đến {2}")]
        public Nullable<int> ThoiLuong { get; set; }
        [DisplayName("Trạng thái")]
        public string TrangThai { get; set; }
        [DisplayName("Ngày chiếu")]
        [Required(ErrorMessage = "{0} không được để trống")]
        [KTNgayChieu]
        public Nullable<System.DateTime> NgayChieu { get; set; }
        [DisplayName("Suất chiếu")]
        public List<string> MaSC { get; set; }
        [DisplayName("Thể loại")]
        public List<string> MaTL { get; set; }
    }
}