using DAM.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DAM.Models.ViewModel
{
    public class LichSuViewModel
    {
        public int MaHD {  get; set; }
        public string TenTK { get; set; }
        public System.DateTime NgayChieu { get; set; }
        public string HinhAnh { get; set; }
        public int MaVe { get; set; }
        public List<string> MaGhe { get; set; }
        public string MaPhim { get; set; }
        public string TenPhim { get; set; }
        public string MaRap { get; set; }
        public string TenRap { get;set; }
        public string MaSC { get; set; }
        public Nullable<System.TimeSpan> KhungGio { get; set; }
        public decimal TongTien { get; set; }
        public System.DateTime NgayDat { get; set; }
        
    }
}