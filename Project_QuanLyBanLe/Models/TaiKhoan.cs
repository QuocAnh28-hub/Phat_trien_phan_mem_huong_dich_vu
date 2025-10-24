using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class TaiKhoan
    {
        public string MATAIKHOAN { get; set; }
        public string USERNAME { get; set; }
        public string PASS { get; set; }
        public int QUYEN { get; set; }
    }
}
