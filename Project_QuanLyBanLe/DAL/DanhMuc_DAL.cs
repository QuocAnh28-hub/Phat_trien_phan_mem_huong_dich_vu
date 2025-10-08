using Microsoft.Extensions.Configuration;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace DAL
{
    public class DanhMuc_DAL
    {
        private readonly DatabaseHelper _dbHelper;

        public DanhMuc_DAL(IConfiguration configuration)
        {
            _dbHelper = new DatabaseHelper(configuration);
        }

        public bool KiemTraTonTai(string maDanhMuc)
        {
            string sql = "SELECT COUNT(*) AS SoLuong FROM DANHMUC WHERE MADANHMUC = @MADANHMUC";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@MADANHMUC", maDanhMuc)
            };

            var dt = _dbHelper.ExecuteQuery(sql, parameters);

            if (dt.Rows.Count > 0)
            {
                int count = Convert.ToInt32(dt.Rows[0]["SoLuong"]);
                return count > 0;
            }

            return false;
        }

        public List<DanhMuc> GetAll()
        {
            List<DanhMuc> list = new List<DanhMuc>();
            string sql = "SELECT MADANHMUC, TENDANHMUC, MOTA FROM DANHMUC";
            var dt = _dbHelper.ExecuteQuery(sql);

            foreach (System.Data.DataRow row in dt.Rows)
            {
                list.Add(new DanhMuc
                {
                    MADANHMUC = row["MADANHMUC"].ToString(),
                    TENDANHMUC = row["TENDANHMUC"].ToString(),
                    MOTA = row["MOTA"].ToString()
                });
            }

            return list;
        }

        public List<DanhMuc> GetbyID(string madanhmuc)
        {
            List<DanhMuc> list = new List<DanhMuc>();
            string sql = "SELECT MADANHMUC, TENDANHMUC, MOTA FROM DANHMUC WHERE MADANHMUC ='" + madanhmuc + "'";
            var dt = _dbHelper.ExecuteQuery(sql);

            foreach (System.Data.DataRow row in dt.Rows)
            {
                list.Add(new DanhMuc
                {
                    MADANHMUC = row["MADANHMUC"].ToString(),
                    TENDANHMUC = row["TENDANHMUC"].ToString(),
                    MOTA = row["MOTA"].ToString()
                });
            }

            return list;
        }

        public bool Insert(DanhMuc danhmuc)
        {
            string sql = "INSERT INTO DANHMUC (MADANHMUC, TENDANHMUC, MOTA) VALUES (@ma, @ten, @mota)";
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@ma", danhmuc.MADANHMUC),
                new SqlParameter("@ten", danhmuc.TENDANHMUC),
                new SqlParameter("@mota", danhmuc.MOTA)
            };

            int rows = _dbHelper.ExecuteNonQuery(sql, parameters);
            return rows > 0;
        }

        public bool Update(DanhMuc danhmuc)
        {
            if (danhmuc == null || string.IsNullOrEmpty(danhmuc.MADANHMUC))
                return false;

            if (!KiemTraTonTai(danhmuc.MADANHMUC))
                return false;

            string sql = "UPDATE DANHMUC SET TENDANHMUC = @ten, MOTA = @mota WHERE MADANHMUC = @ma";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@ma", danhmuc.MADANHMUC),
                new SqlParameter("@ten", danhmuc.TENDANHMUC),
                new SqlParameter("@mota", danhmuc.MOTA)
            };

            int rows = _dbHelper.ExecuteNonQuery(sql, parameters);
            return rows > 0;
        }

        public bool CoSanPhamThuocDanhMuc(string maDanhMuc)
        {
            string sql = "SELECT COUNT(*) AS SoLuong FROM SANPHAM WHERE MADANHMUC = @MADANHMUC";

            SqlParameter[] parameters = new SqlParameter[]
            {
            new SqlParameter("@MADANHMUC", maDanhMuc)
            };

            var dt = _dbHelper.ExecuteQuery(sql, parameters);

            if (dt.Rows.Count > 0)
            {
                int count = Convert.ToInt32(dt.Rows[0]["SoLuong"]);
                return count > 0;
            }

            return false;
        }

        public bool Delete(string maDanhMuc)
        {
            if (string.IsNullOrEmpty(maDanhMuc))
                return false;

            if (!KiemTraTonTai(maDanhMuc))
                return false;

            string sql = "DELETE FROM DANHMUC WHERE MADANHMUC = @ma";
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@ma", maDanhMuc)
            };

            int rows = _dbHelper.ExecuteNonQuery(sql, parameters);
            return rows > 0;
        }
    }
}
