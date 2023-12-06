using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DAM.Models.ViewModel
{
    public class RapSCPhimViewModel
    {
        public string MaPhim { get; set; }
        public string TenPhim { get; set; }
        public string MaRap { get; set; }
        public string TenRap { get; set; }
        public string MaSC {  get; set; }
        public Nullable<System.TimeSpan> KhungGio { get; set; }
        public List<string> MaGhe { get; set; }
        public string MaGheDaChon {  get; set; }
        public string GiaVe { get; set; }
    }
}