using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class SanPham
    {
        public string MASP { get; set; }
        public string TENSP{ get; set; }
        public string MAVACH { get; set; }
        public string MOTA { get; set; }
        public string MADANHMUC { get; set; }
        public decimal? DONGIA { get; set; }
        public string THUOCTINH { get; set; }
        public decimal? THUE { get; set; }
        public int SOLUONGTON { get; set; }
    }
}
