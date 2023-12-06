using DAM.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace DAM.Models.ViewModel
{
    public class ChiTietPhimViewModel
    {
        [DisplayName("Mã phim")]
        public string MaPhim { get; set; }
        [DisplayName("Tên phim")]
        public string TenPhim { get; set; }
        [DisplayName("Hình ảnh")]
        public string HinhAnh { get; set; }
        [DisplayName("Mã Trailer")]
        public string DaTaTrailer { get; set; }
        [DisplayName("Thông tin phim")]
        public string ThongTinPhim { get; set; }
        [DisplayName("Nội dung")]
        public string NoiDung { get; set; }
        [DisplayName("Thời lượng")]
        public Nullable<int> ThoiLuong { get; set; }
        [DisplayName("Trạng thái")]
        public string TrangThai { get; set; }
        [DisplayName("Ngày chiếu")]
        public Nullable<System.DateTime> NgayChieu { get; set; }
        [DisplayName("Mã rạp")]
        public List<string> MaRap { get; set; }
        [DisplayName("Tên rạp")]
        public List<string> TenRap { get; set; }
        [DisplayName("Mã thể loại")]
        public List<string> MaTL { get; set; }
        [DisplayName("Tên thể loại")]
        public List<string> TenTL { get; set; }
    }
}