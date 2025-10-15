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
    public class SanPham_DAL
    {
        private readonly DatabaseHelper _dbHelper;

        public SanPham_DAL(IConfiguration configuration)
        {
            _dbHelper = new DatabaseHelper(configuration);
        }

        public List<SanPham> GetAll()
        {
            try
            {
                List<SanPham> list = new List<SanPham>();
                string sql = "SELECT MASP, TENSP, MAVACH, MOTA, MADANHMUC, DONGIA, THUOCTINH, THUEVAT, SOLUONGTON FROM SANPHAM";

                var dt = _dbHelper.ExecuteQuery(sql);

                foreach (System.Data.DataRow row in dt.Rows)
                {
                    list.Add(new SanPham
                    {
                        MASP = row["MASP"].ToString(),
                        TENSP = row["TENSP"].ToString(),
                        MAVACH = row["MAVACH"].ToString(),
                        MOTA = row["MOTA"].ToString(),
                        MADANHMUC = row["MADANHMUC"].ToString(),
                        DONGIA = row["DONGIA"] == DBNull.Value ? null : (decimal?)Convert.ToDecimal(row["DONGIA"]),
                        THUOCTINH = row["THUOCTINH"].ToString(),
                        THUE = row["THUEVAT"] == DBNull.Value ? null : (decimal?)Convert.ToDecimal(row["THUEVAT"]),
                        SOLUONGTON = Convert.ToInt32(row["SOLUONGTON"])
                    });
                }

                return list;
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi: " + ex.Message);
            }
        }

        public List<SanPham> GetByID(string maSP)
        {
            try
            {
                List<SanPham> list = new List<SanPham>();
                string sql = "SELECT MASP, TENSP, MAVACH, MOTA, MADANHMUC, DONGIA, THUOCTINH, THUEVAT, SOLUONGTON FROM SANPHAM WHERE MASP = @ma";
                SqlParameter[] parameters = new SqlParameter[]
                {
                new SqlParameter("@ma", maSP)
                };

                var dt = _dbHelper.ExecuteQuery(sql, parameters);

                foreach (System.Data.DataRow row in dt.Rows)
                {
                    list.Add(new SanPham
                    {
                        MASP = row["MASP"].ToString(),
                        TENSP = row["TENSP"].ToString(),
                        MAVACH = row["MAVACH"].ToString(),
                        MOTA = row["MOTA"].ToString(),
                        MADANHMUC = row["MADANHMUC"].ToString(),
                        DONGIA = row["DONGIA"] == DBNull.Value ? null : (decimal?)Convert.ToDecimal(row["DONGIA"]),
                        THUOCTINH = row["THUOCTINH"].ToString(),
                        THUE = row["THUEVAT"] == DBNull.Value ? null : (decimal?)Convert.ToDecimal(row["THUEVAT"]),
                        SOLUONGTON = Convert.ToInt32(row["SOLUONGTON"])
                    });
                }

                return list;
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi: " + ex.Message);
            }
        }

        public bool KiemTraTonTai(string maSanPham)
        {
            try
            {
                string sql = "SELECT COUNT(*) AS SoLuong FROM SANPHAM WHERE MASP = @MASP";

                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@MASP", maSanPham)
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

        public bool Insert(SanPham sp)
        {
            try
            {
                if (KiemTraTonTai(sp.MASP))
                    return false;

                string sql = @"INSERT INTO SANPHAM (MASP, TENSP, MAVACH, MOTA, MADANHMUC, DONGIA, THUOCTINH, THUEVAT, SOLUONGTON) VALUES (@MASP, @TENSP, @MAVACH, @MOTA, @MADANHMUC, @DONGIA, @THUOCTINH, @THUEVAT, @SOLUONGTON)";

                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@MASP", sp.MASP),
                    new SqlParameter("@TENSP", sp.TENSP ?? (object)DBNull.Value),
                    new SqlParameter("@MAVACH", sp.MAVACH ?? (object)DBNull.Value),
                    new SqlParameter("@MOTA", sp.MOTA ?? (object)DBNull.Value),
                    new SqlParameter("@MADANHMUC", sp.MADANHMUC ?? (object)DBNull.Value),
                    new SqlParameter("@DONGIA", sp.DONGIA ?? (object)DBNull.Value),
                    new SqlParameter("@THUOCTINH", sp.THUOCTINH ?? (object)DBNull.Value),
                    new SqlParameter("@THUEVAT", sp.THUE ?? (object)DBNull.Value),
                    new SqlParameter("@SOLUONGTON", sp.SOLUONGTON)
                };

                int rows = _dbHelper.ExecuteNonQuery(sql, parameters);
                return rows > 0;
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi: " + ex.Message);
            }
        }

        public bool Update(SanPham sp)
        {
            try
            {
                if (!KiemTraTonTai(sp.MASP))
                    return false;

                string sql = @"UPDATE SANPHAM
                       SET TENSP = @TENSP,
                           MAVACH = @MAVACH,
                           MOTA = @MOTA,
                           MADANHMUC = @MADANHMUC,
                           DONGIA = @DONGIA,
                           THUOCTINH = @THUOCTINH,
                           THUEVAT = @THUEVAT,
                           SOLUONGTON = @SOLUONGTON
                       WHERE MASP = @MASP";

                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@MASP", sp.MASP),
                    new SqlParameter("@TENSP", sp.TENSP ?? (object)DBNull.Value),
                    new SqlParameter("@MAVACH", sp.MAVACH ?? (object)DBNull.Value),
                    new SqlParameter("@MOTA", sp.MOTA ?? (object)DBNull.Value),
                    new SqlParameter("@MADANHMUC", sp.MADANHMUC ?? (object)DBNull.Value),
                    new SqlParameter("@DONGIA", sp.DONGIA ?? (object)DBNull.Value),
                    new SqlParameter("@THUOCTINH", sp.THUOCTINH ?? (object)DBNull.Value),
                    new SqlParameter("@THUEVAT", sp.THUE ?? (object)DBNull.Value),
                    new SqlParameter("@SOLUONGTON", sp.SOLUONGTON)
                };

                int rows = _dbHelper.ExecuteNonQuery(sql, parameters);
                return rows > 0;
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi: " + ex.Message);
            }
        }

        public bool UpdateSoLuong(string maSP, int soLuongMoi)
        {
            try
            {
                if (!KiemTraTonTai(maSP))
                    return false;

                string sql = "UPDATE SANPHAM SET SOLUONGTON = @SoLuong WHERE MASP = @MASP";

                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@SoLuong", soLuongMoi),
                    new SqlParameter("@MASP", maSP)
                };

                int rows = _dbHelper.ExecuteNonQuery(sql, parameters);
                return rows > 0;
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi: " + ex.Message);
            }
        }

        public bool Delete(string maSP)
        {
            try
            {
                if (!KiemTraTonTai(maSP))
                    return false;

                string sql = "DELETE FROM SANPHAM WHERE MASP = @MASP";

                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@MASP", maSP)
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
