using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Models;
using System;
using System.Collections.Generic;
using System.Data;

namespace DAL
{
    public class PhieuNhapKho_DAL
    {
        private readonly DatabaseHelper _dbHelper;

        public PhieuNhapKho_DAL(IConfiguration configuration)
        {
            _dbHelper = new DatabaseHelper(configuration);
        }

        // ===== Helpers =====
        public bool KiemTraTonTai(string maPN)
        {
            try
            {
                string sql = "SELECT COUNT(*) AS SoLuong FROM PHIEUNHAPKHO WHERE MAPHIEUNHAP = @MAPHIEUNHAP";
                SqlParameter[] p = { new SqlParameter("@MAPHIEUNHAP", maPN) };
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
                throw new Exception("Lỗi kiểm tra tồn tại phiếu nhập: " + ex.Message);
            }
        }

        // ===== SELECT =====
        public List<PhieuNhapKho> GetAll()
        {
            try
            {
                var list = new List<PhieuNhapKho>();
                string sql = @"SELECT MAPHIEUNHAP, MASP, MANCC, MANV, NGAYLAP, THUEVAT
                               FROM PHIEUNHAPKHO";
                var dt = _dbHelper.ExecuteQuery(sql);

                foreach (DataRow r in dt.Rows)
                {
                    list.Add(new PhieuNhapKho
                    {
                        MAPHIEUNHAP = r["MAPHIEUNHAP"]?.ToString()?.Trim(),
                        MASP = r["MASP"]?.ToString()?.Trim(),
                        MANCC = r["MANCC"]?.ToString()?.Trim(),
                        MANV = r["MANV"]?.ToString()?.Trim(),
                        NGAYLAP = r["NGAYLAP"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(r["NGAYLAP"]),
                        THUEVAT = r["THUEVAT"] == DBNull.Value ? 0 : Convert.ToDouble(r["THUEVAT"])
                    });
                }
                return list;
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi lấy danh sách phiếu nhập: " + ex.Message);
            }
        }

        public List<PhieuNhapKho> GetByID(string maPN)
        {
            try
            {
                var list = new List<PhieuNhapKho>();
                string sql = @"SELECT MAPHIEUNHAP, MASP, MANCC, MANV, NGAYLAP, THUEVAT
                               FROM PHIEUNHAPKHO
                               WHERE MAPHIEUNHAP = @MAPHIEUNHAP";
                SqlParameter[] p = { new SqlParameter("@MAPHIEUNHAP", maPN) };
                var dt = _dbHelper.ExecuteQuery(sql, p);

                foreach (DataRow r in dt.Rows)
                {
                    list.Add(new PhieuNhapKho
                    {
                        MAPHIEUNHAP = r["MAPHIEUNHAP"]?.ToString()?.Trim(),
                        MASP = r["MASP"]?.ToString()?.Trim(),
                        MANCC = r["MANCC"]?.ToString()?.Trim(),
                        MANV = r["MANV"]?.ToString()?.Trim(),
                        NGAYLAP = r["NGAYLAP"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(r["NGAYLAP"]),
                        THUEVAT = r["THUEVAT"] == DBNull.Value ? 0 : Convert.ToDouble(r["THUEVAT"])
                    });
                }
                return list;
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi lấy phiếu nhập theo mã: " + ex.Message);
            }
        }

        // ===== INSERT =====
        public bool Insert(PhieuNhapKho pnk)
        {
            try
            {
                string sql = @"
                    INSERT INTO PHIEUNHAPKHO (MAPHIEUNHAP, MASP, MANCC, MANV, NGAYLAP, THUEVAT)
                    VALUES (@MAPHIEUNHAP, @MASP, @MANCC, @MANV, @NGAYLAP, @THUEVAT)";

                SqlParameter[] p =
                {
                    new SqlParameter("@MAPHIEUNHAP", pnk.MAPHIEUNHAP),
                    new SqlParameter("@MASP",        pnk.MASP),
                    new SqlParameter("@MANCC",       pnk.MANCC),
                    new SqlParameter("@MANV",        pnk.MANV),
                    new SqlParameter("@NGAYLAP",     pnk.NGAYLAP),
                    new SqlParameter("@THUEVAT",     pnk.THUEVAT)
                };

                int rows = _dbHelper.ExecuteNonQuery(sql, p);
                return rows > 0;
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi thêm phiếu nhập: " + ex.Message);
            }
        }

        // ===== UPDATE =====
        public bool Update(PhieuNhapKho pnk)
        {
            try
            {
                if (pnk == null || string.IsNullOrWhiteSpace(pnk.MAPHIEUNHAP))
                    return false;

                if (!KiemTraTonTai(pnk.MAPHIEUNHAP))
                    return false;

                string sql = @"
                    UPDATE PHIEUNHAPKHO
                    SET MASP = @MASP,
                        MANCC = @MANCC,
                        MANV = @MANV,
                        NGAYLAP = @NGAYLAP,
                        THUEVAT = @THUEVAT
                    WHERE MAPHIEUNHAP = @MAPHIEUNHAP";

                SqlParameter[] p =
                {
                    new SqlParameter("@MAPHIEUNHAP", pnk.MAPHIEUNHAP),
                    new SqlParameter("@MASP",        pnk.MASP),
                    new SqlParameter("@MANCC",       pnk.MANCC),
                    new SqlParameter("@MANV",        pnk.MANV),
                    new SqlParameter("@NGAYLAP",     pnk.NGAYLAP),
                    new SqlParameter("@THUEVAT",     pnk.THUEVAT)
                };

                int rows = _dbHelper.ExecuteNonQuery(sql, p);
                return rows > 0;
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi cập nhật phiếu nhập: " + ex.Message);
            }
        }

        // ===== DELETE =====
        public bool Delete(string maPN)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(maPN))
                    return false;

                if (!KiemTraTonTai(maPN))
                    return false;

                string sql = "DELETE FROM PHIEUNHAPKHO WHERE MAPHIEUNHAP = @MAPHIEUNHAP";
                SqlParameter[] p = { new SqlParameter("@MAPHIEUNHAP", maPN) };

                int rows = _dbHelper.ExecuteNonQuery(sql, p);
                return rows > 0;
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi xoá phiếu nhập: " + ex.Message);
            }
        }
    }
}
