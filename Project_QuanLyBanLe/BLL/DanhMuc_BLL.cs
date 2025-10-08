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
    public class DanhMuc_BLL
    {
        private readonly DanhMuc_DAL dm_dal;

        public DanhMuc_BLL(IConfiguration configuration)
        {
            dm_dal = new DanhMuc_DAL(configuration);
        }

        public List<DanhMuc> LayTatCa()
        {
            var list = dm_dal.GetAll();

            if (list == null || list.Count == 0)
                return new List<DanhMuc>();

            return list;
        }

        public List<DanhMuc> LayTheoID(string madanhmuc)
        {
            if (string.IsNullOrEmpty(madanhmuc))
                return null;

            if (!dm_dal.KiemTraTonTai(madanhmuc))
                return null; 

            var danhmuc = dm_dal.GetbyID(madanhmuc);
            return danhmuc;
        }

        public bool ThemMoi(DanhMuc danhmuc)
        {
            if (danhmuc == null)
                return false;

            if (string.IsNullOrEmpty(danhmuc.MADANHMUC) || string.IsNullOrEmpty(danhmuc.TENDANHMUC))
                return false;

            if (dm_dal.KiemTraTonTai(danhmuc.MADANHMUC))
                return false;

            var result = dm_dal.Insert(danhmuc);
            return result;
        }

        public bool CapNhat(DanhMuc danhmuc)
        {
            if (danhmuc == null)
                return false;

            if (string.IsNullOrEmpty(danhmuc.MADANHMUC) || string.IsNullOrEmpty(danhmuc.TENDANHMUC))
                return false;

            var result = dm_dal.Update(danhmuc);
            return result;
        }

        public bool Xoa(string maDanhMuc)
        {
            if (string.IsNullOrEmpty(maDanhMuc))
                return false;

            if (!dm_dal.KiemTraTonTai(maDanhMuc))
                return false;

            if (dm_dal.CoSanPhamThuocDanhMuc(maDanhMuc))
                return false;

            var result = dm_dal.Delete(maDanhMuc);
            return result;
        }
    }
}
