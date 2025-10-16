using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Models;
using System;
using System.Collections.Generic;
using System.Data;

namespace DAL
{
    public class ChiTietNhap_DAL
    {
        private readonly DatabaseHelper _dbHelper;

        public ChiTietNhap_DAL(IConfiguration configuration)
        {
            _dbHelper = new DatabaseHelper(configuration);
        }

        // ================== Helpers ==================
        public bool KiemTraTonTai(string maphieunhap, string masp)
        {
            try
            {
                string sql = @"
                    SELECT COUNT(*) AS SoLuong
                    FROM CHITIETNHAP
                    WHERE MAPHIEUNHAP = @MAPHIEUNHAP AND MASP = @MASP";

                SqlParameter[] p =
                {
                    new SqlParameter("@MAPHIEUNHAP", maphieunhap),
                    new SqlParameter("@MASP",        masp)
                };

                var dt = _dbHelper.ExecuteQuery(sql, p);
                if (dt.Rows.Count > 0)
                {
                    int count = Convert.ToInt32(dt.Rows[0]["SoLuong"]);
                    return count > 0;
                }
                return false;
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi kiểm tra tồn tại chi tiết nhập: " + ex.Message);
            }
        }

        // =============== SELECT ===============
        public List<ChiTietNhap> GetAll()
        {
            try
            {
                var list = new List<ChiTietNhap>();
                string sql = @"SELECT MAPHIEUNHAP, MASP, SOLUONG, DONGIANHAP, THANHTIEN, NGAYNHAPKHO
                               FROM CHITIETNHAP";
                var dt = _dbHelper.ExecuteQuery(sql);

                foreach (DataRow row in dt.Rows)
                {
                    list.Add(new ChiTietNhap
                    {
                        MAPHIEUNHAP = row["MAPHIEUNHAP"]?.ToString()?.Trim(),
                        MASP = row["MASP"]?.ToString()?.Trim(),
                        SOLUONG = row["SOLUONG"] == DBNull.Value ? 0 : Convert.ToInt32(row["SOLUONG"]),
                        DONGIANHAP = row["DONGIANHAP"] == DBNull.Value ? 0 : Convert.ToDecimal(row["DONGIANHAP"]),
                        THANHTIEN = row["THANHTIEN"] == DBNull.Value ? 0 : Convert.ToDecimal(row["THANHTIEN"]),
                        NGAYNHAPKHO = row["NGAYNHAPKHO"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(row["NGAYNHAPKHO"])
                    });
                }
                return list;
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi lấy danh sách chi tiết nhập: " + ex.Message);
            }
        }

        public List<ChiTietNhap> GetByPhieu(string maphieunhap)
        {
            try
            {
                var list = new List<ChiTietNhap>();
                string sql = @"SELECT MAPHIEUNHAP, MASP, SOLUONG, DONGIANHAP, THANHTIEN, NGAYNHAPKHO
                               FROM CHITIETNHAP
                               WHERE MAPHIEUNHAP = @MAPHIEUNHAP";
                SqlParameter[] p = { new SqlParameter("@MAPHIEUNHAP", maphieunhap) };
                var dt = _dbHelper.ExecuteQuery(sql, p);

                foreach (DataRow row in dt.Rows)
                {
                    list.Add(new ChiTietNhap
                    {
                        MAPHIEUNHAP = row["MAPHIEUNHAP"]?.ToString()?.Trim(),
                        MASP = row["MASP"]?.ToString()?.Trim(),
                        SOLUONG = row["SOLUONG"] == DBNull.Value ? 0 : Convert.ToInt32(row["SOLUONG"]),
                        DONGIANHAP = row["DONGIANHAP"] == DBNull.Value ? 0 : Convert.ToDecimal(row["DONGIANHAP"]),
                        THANHTIEN = row["THANHTIEN"] == DBNull.Value ? 0 : Convert.ToDecimal(row["THANHTIEN"]),
                        NGAYNHAPKHO = row["NGAYNHAPKHO"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(row["NGAYNHAPKHO"])
                    });
                }
                return list;
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi lấy chi tiết theo phiếu nhập: " + ex.Message);
            }
        }

        public List<ChiTietNhap> GetById(string maphieunhap, string masp)
        {
            try
            {
                var list = new List<ChiTietNhap>();
                string sql = @"SELECT MAPHIEUNHAP, MASP, SOLUONG, DONGIANHAP, THANHTIEN, NGAYNHAPKHO
                               FROM CHITIETNHAP
                               WHERE MAPHIEUNHAP = @MAPHIEUNHAP AND MASP = @MASP";
                SqlParameter[] p =
                {
                    new SqlParameter("@MAPHIEUNHAP", maphieunhap),
                    new SqlParameter("@MASP",        masp)
                };
                var dt = _dbHelper.ExecuteQuery(sql, p);

                foreach (DataRow row in dt.Rows)
                {
                    list.Add(new ChiTietNhap
                    {
                        MAPHIEUNHAP = row["MAPHIEUNHAP"]?.ToString()?.Trim(),
                        MASP = row["MASP"]?.ToString()?.Trim(),
                        SOLUONG = row["SOLUONG"] == DBNull.Value ? 0 : Convert.ToInt32(row["SOLUONG"]),
                        DONGIANHAP = row["DONGIANHAP"] == DBNull.Value ? 0 : Convert.ToDecimal(row["DONGIANHAP"]),
                        THANHTIEN = row["THANHTIEN"] == DBNull.Value ? 0 : Convert.ToDecimal(row["THANHTIEN"]),
                        NGAYNHAPKHO = row["NGAYNHAPKHO"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(row["NGAYNHAPKHO"])
                    });
                }
                return list;
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi lấy chi tiết nhập theo khóa: " + ex.Message);
            }
        }

        // =============== INSERT ===============
        public bool Insert(ChiTietNhap ctn)
        {
            try
            {
                string sql = @"
                    INSERT INTO CHITIETNHAP (MAPHIEUNHAP, MASP, SOLUONG, DONGIANHAP, THANHTIEN, NGAYNHAPKHO)
                    VALUES (@MAPHIEUNHAP, @MASP, @SOLUONG, @DONGIANHAP, (@SOLUONG * @DONGIANHAP), @NGAYNHAPKHO)";

                SqlParameter[] p =
                {
                    new SqlParameter("@MAPHIEUNHAP", ctn.MAPHIEUNHAP),
                    new SqlParameter("@MASP",        ctn.MASP),
                    new SqlParameter("@SOLUONG",     ctn.SOLUONG),
                    new SqlParameter("@DONGIANHAP",  ctn.DONGIANHAP),
                    new SqlParameter("@NGAYNHAPKHO", ctn.NGAYNHAPKHO)
                };

                int rows = _dbHelper.ExecuteNonQuery(sql, p);
                return rows > 0;
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi thêm chi tiết nhập: " + ex.Message);
            }
        }

        // =============== UPDATE ===============
        public bool Update(ChiTietNhap ctn)
        {
            try
            {
                if (ctn == null || string.IsNullOrEmpty(ctn.MAPHIEUNHAP) || string.IsNullOrEmpty(ctn.MASP))
                    return false;

                if (!KiemTraTonTai(ctn.MAPHIEUNHAP, ctn.MASP))
                    return false;

                string sql = @"
                    UPDATE CHITIETNHAP
                    SET SOLUONG = @SOLUONG,
                        DONGIANHAP = @DONGIANHAP,
                        THANHTIEN = (@SOLUONG * @DONGIANHAP),
                        NGAYNHAPKHO = @NGAYNHAPKHO
                    WHERE MAPHIEUNHAP = @MAPHIEUNHAP AND MASP = @MASP";

                SqlParameter[] p =
                {
                    new SqlParameter("@MAPHIEUNHAP", ctn.MAPHIEUNHAP),
                    new SqlParameter("@MASP",        ctn.MASP),
                    new SqlParameter("@SOLUONG",     ctn.SOLUONG),
                    new SqlParameter("@DONGIANHAP",  ctn.DONGIANHAP),
                    new SqlParameter("@NGAYNHAPKHO", ctn.NGAYNHAPKHO)
                };

                int rows = _dbHelper.ExecuteNonQuery(sql, p);
                return rows > 0;
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi cập nhật chi tiết nhập: " + ex.Message);
            }
        }

        // =============== DELETE ===============
        public bool Delete(string maphieunhap, string masp)
        {
            try
            {
                if (string.IsNullOrEmpty(maphieunhap) || string.IsNullOrEmpty(masp))
                    return false;

                if (!KiemTraTonTai(maphieunhap, masp))
                    return false;

                string sql = @"DELETE FROM CHITIETNHAP
                               WHERE MAPHIEUNHAP = @MAPHIEUNHAP AND MASP = @MASP";

                SqlParameter[] p =
                {
                    new SqlParameter("@MAPHIEUNHAP", maphieunhap),
                    new SqlParameter("@MASP",        masp)
                };

                int rows = _dbHelper.ExecuteNonQuery(sql, p);
                return rows > 0;
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi xóa chi tiết nhập: " + ex.Message);
            }
        }
    }
}
