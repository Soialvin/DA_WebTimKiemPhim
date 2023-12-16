using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DAM.Models.ViewModel
{
    public class VeViewModel
    {
        public int MaVe { get; set; }
        public string MaSC { get; set; }
        public System.TimeSpan KhungGio { get; set; }
        public string MaPhim { get; set; }
        public string TenPhim { get; set; }
        public string MaRap { get; set; }
        public string TenRap { get; set; }
        public Nullable<System.DateTime> NgayTao { get; set; }
        public string TrangThaiVe { get; set; }
    }
}