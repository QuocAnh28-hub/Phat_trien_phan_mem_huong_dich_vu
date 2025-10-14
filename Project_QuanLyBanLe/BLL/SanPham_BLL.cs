using DAL;
using Microsoft.Extensions.Configuration;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class SanPham_BLL
    {
        private readonly SanPham_DAL sp_dal;

        public SanPham_BLL(IConfiguration configuration)
        {
            sp_dal = new SanPham_DAL(configuration);
        }

        public List<SanPham> LayTatCa()
        {
            return sp_dal.GetAll();
        }

        public List<SanPham> LayTheoID(string maSP)
        {
            if (string.IsNullOrEmpty(maSP))
                return null;

            if (!sp_dal.KiemTraTonTai(maSP))
                return null;

            return sp_dal.GetByID(maSP);
        }

        public bool ThemMoi(SanPham sp)
        {
            if (sp == null)
                return false;

            if (string.IsNullOrEmpty(sp.MASP) || string.IsNullOrEmpty(sp.TENSP))
                return false;

            if (sp_dal.KiemTraTonTai(sp.MASP))
                return false;

            return sp_dal.Insert(sp);
        }

        public bool Sua(SanPham sp)
        {
            if (sp == null)
                return false;

            if (string.IsNullOrEmpty(sp.MASP))
                return false;

            if (!sp_dal.KiemTraTonTai(sp.MASP))
                return false;

            return sp_dal.Update(sp);
        }

        public bool SuaSoLuong(string maSP, int soLuongMoi)
        {
            if (string.IsNullOrEmpty(maSP))
                return false;

            if (!sp_dal.KiemTraTonTai(maSP))
                return false;

            return sp_dal.UpdateSoLuong(maSP, soLuongMoi);
        }

        public bool Xoa(string maSP)
        {
            if (string.IsNullOrEmpty(maSP))
                return false;

            if (!sp_dal.KiemTraTonTai(maSP))
                return false;

            return sp_dal.Delete(maSP);
        }
    }
}
