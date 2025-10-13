using System;
using System.Data;
using DAL;
using Models;

namespace BLL
{
    public class PhieuNhapKho_BLL
    {
        PhieuNhapKho_DAL PNK_DAL = new PhieuNhapKho_DAL();

        public DataTable GetAllPhieuNhapKho()
        {
            return PNK_DAL.GetAllPhieuNhapKho();
        }

        public DataTable GetByIdPhieuNhapKho(string maphieunhap)
        {
            return PNK_DAL.GetByIdPhieuNhapKho(maphieunhap);
        }

        public DataTable CreatePhieuNhapKho(PhieuNhapKho pnk)
        {
            return PNK_DAL.CreatePhieuNhapKho(pnk);
        }

        public DataTable UpdatePhieuNhapKho(PhieuNhapKho pnk)
        {
            return PNK_DAL.UpdatePhieuNhapKho(pnk);
        }

        public DataTable DeletePhieuNhapKho(string maphieunhap)
        {
            return PNK_DAL.DeletePhieuNhapKho(maphieunhap);
        }
    }
}
