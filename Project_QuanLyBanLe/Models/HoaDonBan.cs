using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class HoaDonBan
    {
        public string MAHDBAN { get; set; }
        public string MANV { get; set; }
        public string MAKH { get; set; }
        public DateTime NGAYLAP { get; set; }
        public decimal TONGTIENHANG { get; set; }
        public decimal THUEVAT { get; set; }
        public decimal GIAMGIA { get; set; }
        public List<ChiTietBan> listjson_chitietban { get; set; }
    }
}
