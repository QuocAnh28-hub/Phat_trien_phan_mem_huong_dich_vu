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
        public DataTable GetByIdKH(string makh)
        {
            return KH_DAL.GetByIdKH(makh);
        }
        public DataTable DeleteByIdKH(string makh)
        {
            return KH_DAL.DeleteByIdKH(makh);
        }
        public DataTable UpdateByIdKH(Models.KhachHang kh)
        {
            return KH_DAL.UpdateByIdKH(kh);
        }
        public DataTable CreateKH(Models.KhachHang kh)
        {
            return KH_DAL.CreateKH(kh);
        }
    }
}
