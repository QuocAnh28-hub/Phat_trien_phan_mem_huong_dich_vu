using DAL;
using Microsoft.Extensions.Configuration;
using Models;
using System.Collections.Generic;

namespace BLL
{
    public class NhanVien_BLL
    {
        private readonly NhanVien_DAL nv_dal;

        public NhanVien_BLL(IConfiguration configuration)
        {
            nv_dal = new NhanVien_DAL(configuration);
        }

        public List<NhanVien> LayTatCa()
        {
            var list = nv_dal.GetAll();
            return (list == null || list.Count == 0) ? new List<NhanVien>() : list;
        }

        public List<NhanVien> LayTheoID(string manv)
        {
            if (string.IsNullOrWhiteSpace(manv)) return null;
            if (!nv_dal.KiemTraTonTai(manv)) return null;

            return nv_dal.GetByID(manv);
        }

        public bool ThemMoi(NhanVien nv)
        {
            if (nv == null) return false;
            if (string.IsNullOrWhiteSpace(nv.MANV) || string.IsNullOrWhiteSpace(nv.TENNV)) return false;
            if (nv_dal.KiemTraTonTai(nv.MANV)) return false;

            return nv_dal.Insert(nv);
        }

        public bool CapNhat(NhanVien nv)
        {
            if (nv == null) return false;
            if (string.IsNullOrWhiteSpace(nv.MANV) || string.IsNullOrWhiteSpace(nv.TENNV)) return false;

            return nv_dal.Update(nv);
        }

        public bool Xoa(string manv)
        {
            if (string.IsNullOrWhiteSpace(manv)) return false;
            if (!nv_dal.KiemTraTonTai(manv)) return false;

            return nv_dal.Delete(manv);
        }
    }
}
