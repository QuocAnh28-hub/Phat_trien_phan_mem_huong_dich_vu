using DAL;
using Microsoft.Extensions.Configuration;
using Models;
using System.Collections.Generic;

namespace BLL
{
    public class PhieuNhapKho_BLL
    {
        private readonly PhieuNhapKho_DAL pnk_dal;

        public PhieuNhapKho_BLL(IConfiguration configuration)
        {
            pnk_dal = new PhieuNhapKho_DAL(configuration);
        }

        public List<PhieuNhapKho> LayTatCa()
        {
            var list = pnk_dal.GetAll();
            return (list == null || list.Count == 0) ? new List<PhieuNhapKho>() : list;
        }

        public List<PhieuNhapKho> LayTheoID(string maphieunhap)
        {
            if (string.IsNullOrWhiteSpace(maphieunhap)) return null;
            if (!pnk_dal.KiemTraTonTai(maphieunhap)) return null;

            return pnk_dal.GetByID(maphieunhap);
        }

        public bool ThemMoi(PhieuNhapKho pnk)
        {
            if (pnk == null) return false;
            if (string.IsNullOrWhiteSpace(pnk.MAPHIEUNHAP) ||
                string.IsNullOrWhiteSpace(pnk.MASP) ||
                string.IsNullOrWhiteSpace(pnk.MANCC) ||
                string.IsNullOrWhiteSpace(pnk.MANV))
                return false;

            if (pnk_dal.KiemTraTonTai(pnk.MAPHIEUNHAP)) return false;

            return pnk_dal.Insert(pnk);
        }

        public bool CapNhat(PhieuNhapKho pnk)
        {
            if (pnk == null) return false;
            if (string.IsNullOrWhiteSpace(pnk.MAPHIEUNHAP)) return false;

            return pnk_dal.Update(pnk);
        }

        public bool Xoa(string maphieunhap)
        {
            if (string.IsNullOrWhiteSpace(maphieunhap)) return false;
            if (!pnk_dal.KiemTraTonTai(maphieunhap)) return false;

            return pnk_dal.Delete(maphieunhap);
        }
    }
}
