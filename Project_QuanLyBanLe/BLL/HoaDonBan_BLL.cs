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
    public class HoaDonBan_BLL
    {
        private readonly HoaDonBan_DAL hdb_dal;

        public HoaDonBan_BLL(IConfiguration configuration)
        {
            hdb_dal = new HoaDonBan_DAL(configuration);
        }

        public List<HoaDonBan> LayTatCa()
        {
            return hdb_dal.GetAll();
        }

        public List<HoaDonBan> LayTheoID(string maHDB)
        {
            if (string.IsNullOrEmpty(maHDB))
                return null;

            if (!hdb_dal.KiemTraTonTai(maHDB))
                return null;

            return hdb_dal.GetByID(maHDB);
        }

        public bool ThemMoi(HoaDonBan hd)
        {
            if (hd == null)
                return false;

            if (string.IsNullOrEmpty(hd.MAHDBAN))
                return false;

            if (hdb_dal.KiemTraTonTai(hd.MAHDBAN))
                return false;

            if (hd.listjson_chitietban == null || hd.listjson_chitietban.Count == 0)
                return false;

            foreach (var ct in hd.listjson_chitietban)
            {
                if (string.IsNullOrEmpty(ct.MASP) || ct.SOLUONG <= 0 || ct.DONGIA <= 0)
                    return false;
            }

            return hdb_dal.Insert(hd);
        }

        public bool Sua(HoaDonBan hd)
        {
            if (hd == null)
                return false;

            if (string.IsNullOrEmpty(hd.MAHDBAN))
                return false;

            if (!hdb_dal.KiemTraTonTai(hd.MAHDBAN))
                return false;

            if (hd.listjson_chitietban == null || hd.listjson_chitietban.Count == 0)
                return false;

            foreach (var ct in hd.listjson_chitietban)
            {
                if (string.IsNullOrEmpty(ct.MASP) || ct.SOLUONG <= 0 || ct.DONGIA <= 0)
                    return false;
            }

            return hdb_dal.Update(hd);
        }

        public bool Xoa(string maHDB)
        {
            if (string.IsNullOrEmpty(maHDB))
                return false;

            if (!hdb_dal.KiemTraTonTai(maHDB))
                return false;

            return hdb_dal.Delete(maHDB);
        }
        public bool ResetTongTienHangByHoaDon(string maHDBan, decimal tongTienMoi)
        {
            if (string.IsNullOrEmpty(maHDBan)) return false;
            return hdb_dal.ResetTongTienHang(maHDBan, tongTienMoi);
        }

    }
}
