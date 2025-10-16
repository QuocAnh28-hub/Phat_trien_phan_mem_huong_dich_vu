using System;
using System.Collections.Generic;

namespace Models
{
    public class PhieuNhapKho
    {
        public string MAPHIEUNHAP { get; set; } = "";
        public string MASP { get; set; } = "";
        public string MANCC { get; set; } = "";
        public string MANV { get; set; } = "";
        public DateTime NGAYLAP { get; set; }

        public decimal THUEVAT { get; set; }   
        public List<ChiTietNhap>? listjson_chitietnhap { get; set; }
    }
}
