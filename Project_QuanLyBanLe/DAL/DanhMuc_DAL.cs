using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
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
            try
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
            catch (Exception ex)
            {
                throw new Exception("Lỗi: " + ex.Message);
            }
        }

        public List<DanhMuc> GetAll()
        {
            try
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
            catch (Exception ex)
            {
                throw new Exception("Lỗi: " + ex.Message);
            }
        }

        public List<DanhMuc> GetbyID(string madanhmuc)
        {
            try
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
            catch (Exception ex)
            {
                throw new Exception("Lỗi: " + ex.Message);
            }
        }

        public bool Insert(DanhMuc danhmuc)
        {
            try
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
            catch (Exception ex)
            {
                throw new Exception("Lỗi: " + ex.Message);
            }
        }

        public bool Update(DanhMuc danhmuc)
        {
            try
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
            catch (Exception ex)
            {
                throw new Exception("Lỗi: " + ex.Message);
            }
        }

        public bool CoSanPhamThuocDanhMuc(string maDanhMuc)
        {
            try
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
            catch (Exception ex)
            {
                throw new Exception("Lỗi: " + ex.Message);
            }
        }

        public bool Delete(string maDanhMuc)
        {
            try
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
            catch (Exception ex)
            {
                throw new Exception("Lỗi: " + ex.Message);
            }
        }
    }
}
