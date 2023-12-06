using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace DAM.Models.ViewModel
{
    public class PhanLoaiPhimViewModel
    {
        [DisplayName("Mã phim")]
        public string MaPhim { get; set; }
        [DisplayName("Tên phim")]
        public string TenPhim { get; set; }
        [DisplayName("Hình ảnh")]
        public string HinhAnh { get; set; }
        [DisplayName("Trạng thái")]
        public string TrangThai { get; set; }
        [DisplayName("Mã thể loại")]
        public string MaTL { get; set; }
        [DisplayName("Tên thể loại")]
        public string TenTL { get; set; }
    }
}