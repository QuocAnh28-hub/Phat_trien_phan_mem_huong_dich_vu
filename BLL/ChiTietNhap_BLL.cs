using System;
using System.Data;
using DAL;
using Models;

namespace BLL
{
    public class ChiTietNhap_BLL
    {
        ChiTietNhap_DAL CTN_DAL = new ChiTietNhap_DAL();

        public DataTable GetAllChiTietNhap()
        {
            return CTN_DAL.GetAllChiTietNhap();
        }

        public DataTable GetByIdChiTietNhap(string maphieunhap, string masp)
        {
            return CTN_DAL.GetByIdChiTietNhap(maphieunhap, masp);
        }

        public DataTable GetByPhieuNhapKho(string maphieunhap)
        {
            return CTN_DAL.GetByPhieuNhapKho(maphieunhap);
        }

        public DataTable CreateChiTietNhap(ChiTietNhap ctn)
        {
            return CTN_DAL.CreateChiTietNhap(ctn);
        }

        public DataTable UpdateChiTietNhap(ChiTietNhap ctn)
        {
            return CTN_DAL.UpdateChiTietNhap(ctn);
        }

        public DataTable DeleteChiTietNhap(string maphieunhap, string masp)
        {
            return CTN_DAL.DeleteChiTietNhap(maphieunhap, masp);
        }
    }
}
