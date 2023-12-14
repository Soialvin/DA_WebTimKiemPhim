using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace DAM.Models.ViewModel
{
    public class SuatChieu_RapViewModel
    {
        public int ID { get; set; }
        [DisplayName("Mã suất chiếu")]
        public string MaSC { get; set; }
        public string MaSCGoc { get; set; }
        [DisplayName("Mã rạp")]
        public string MaRap { get; set; }
        [DisplayName("Ngày chiếu")]
        public System.DateTime NgayChieu { get; set; }
        public System.DateTime NgayChieuGoc { get; set; }
    }
}