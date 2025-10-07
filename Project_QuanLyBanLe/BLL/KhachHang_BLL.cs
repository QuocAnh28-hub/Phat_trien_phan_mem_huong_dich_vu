using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL;

namespace BLL
{
    public class KhachHang_BLL
    {
        KhachHang_DAL KH_DAL = new KhachHang_DAL();

        public DataTable getAllKH()
        {
            return KH_DAL.getAllKH();
        }
    }
}
