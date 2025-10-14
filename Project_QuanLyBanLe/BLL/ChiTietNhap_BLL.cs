using DAL;
using Microsoft.Extensions.Configuration;
using Models;
using System.Collections.Generic;

namespace BLL
{
    public class ChiTietNhap_BLL
    {
        private readonly ChiTietNhap_DAL ctn_dal;

        public ChiTietNhap_BLL(IConfiguration configuration)
        {
            ctn_dal = new ChiTietNhap_DAL(configuration);
        }

        public List<ChiTietNhap> LayTatCa()
        {
            var list = ctn_dal.GetAll();
            return (list == null || list.Count == 0) ? new List<ChiTietNhap>() : list;
        }

        public List<ChiTietNhap> LayTheoPhieu(string maphieunhap)
        {
            if (string.IsNullOrWhiteSpace(maphieunhap)) return new List<ChiTietNhap>();
            return ctn_dal.GetByPhieu(maphieunhap);
        }

        public List<ChiTietNhap> LayTheoID(string maphieunhap, string masp)
        {
            if (string.IsNullOrWhiteSpace(maphieunhap) || string.IsNullOrWhiteSpace(masp)) return null;
            if (!ctn_dal.KiemTraTonTai(maphieunhap, masp)) return null;

            return ctn_dal.GetById(maphieunhap, masp);
        }

        public bool ThemMoi(ChiTietNhap ctn)
        {
            if (ctn == null) return false;
            if (string.IsNullOrWhiteSpace(ctn.MAPHIEUNHAP) || string.IsNullOrWhiteSpace(ctn.MASP))
                return false;
            if (ctn.SOLUONG <= 0 || ctn.DONGIANHAP < 0) return false;

            if (ctn_dal.KiemTraTonTai(ctn.MAPHIEUNHAP, ctn.MASP)) return false;

            return ctn_dal.Insert(ctn);
        }

        public bool CapNhat(ChiTietNhap ctn)
        {
            if (ctn == null) return false;
            if (string.IsNullOrWhiteSpace(ctn.MAPHIEUNHAP) || string.IsNullOrWhiteSpace(ctn.MASP))
                return false;
            if (ctn.SOLUONG <= 0 || ctn.DONGIANHAP < 0) return false;

            return ctn_dal.Update(ctn);
        }

        public bool Xoa(string maphieunhap, string masp)
        {
            if (string.IsNullOrWhiteSpace(maphieunhap) || string.IsNullOrWhiteSpace(masp)) return false;
            if (!ctn_dal.KiemTraTonTai(maphieunhap, masp)) return false;

            return ctn_dal.Delete(maphieunhap, masp);
        }
    }
}
