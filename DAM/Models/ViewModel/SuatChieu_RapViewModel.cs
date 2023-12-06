using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace DAM.Models.ViewModel
{
    public class SuatChieu_RapViewModel
    {
        [DisplayName("Mã suất chiếu")]
        public string MaSC { get; set; }
        [DisplayName("Mã rạp")]
        public string MaRap { get; set; }
        [DisplayName("Ngày chiếu")]
        public System.DateTime NgayChieu { get; set; }
    }
}