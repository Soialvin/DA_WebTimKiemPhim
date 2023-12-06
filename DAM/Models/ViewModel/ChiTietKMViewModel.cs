using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DAM.Models.ViewModel
{
    public class ChiTietKMViewModel
    {
        public string MaKM {  get; set; }
        public string TenKM { get; set; }
        public string HinhAnh { get; set; }
        public string NoiDung { get; set; }
        public System.DateTime NgayBD { get; set; }
        public System.DateTime NgayKT { get; set; }
        public string MaRap { get; set; }
        public string TenRap { get; set; }
        public string DiaChi { get; set; }
    }
}