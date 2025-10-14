using DAL;
using Microsoft.Extensions.Configuration;
using Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class ChiTietBan_BLL
    {
        private readonly ChiTietBan_DAL ctb_dal;

        public ChiTietBan_BLL(IConfiguration configuration)
        {
            ctb_dal = new ChiTietBan_DAL(configuration);
        }

        public List<ChiTietBan> LayTatCa()
        {
            return ctb_dal.GetAll();
        }

        public List<ChiTietBan> LayTheoHoaDon(string maHDB)
        {
            if (string.IsNullOrEmpty(maHDB))
                return null;

            return ctb_dal.GetByHoaDon(maHDB);
        }

        public bool ThemMoi(ChiTietBan ct)
        {
            if (ct == null)
                return false;

            if (string.IsNullOrEmpty(ct.MAHDBAN) || string.IsNullOrEmpty(ct.MASP))
                return false;

            if (ctb_dal.KiemTraTonTai(ct.MAHDBAN, ct.MASP))
                return false;

            return ctb_dal.Insert(ct);
        }

        public bool Sua(ChiTietBan ct)
        {
            if (ct == null)
                return false;

            if (string.IsNullOrEmpty(ct.MAHDBAN) || string.IsNullOrEmpty(ct.MASP))
                return false;

            if (!ctb_dal.KiemTraTonTai(ct.MAHDBAN, ct.MASP))
                return false;

            return ctb_dal.Update(ct);
        }

        public bool Xoa(string maHDB, string maSP)
        {
            if (string.IsNullOrEmpty(maHDB) || string.IsNullOrEmpty(maSP))
                return false;

            if (!ctb_dal.KiemTraTonTai(maHDB, maSP))
                return false;

            return ctb_dal.Delete(maHDB, maSP);
        }
    }
}
