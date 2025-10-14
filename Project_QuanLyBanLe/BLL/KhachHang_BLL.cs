using DAL;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class KhachHang_BLL
    {
        private readonly KhachHang_DAL KH_DAL;

        public KhachHang_BLL(IConfiguration configuration)
        {
            KH_DAL = new KhachHang_DAL(configuration);
        }
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
