using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Models;
using System;
using System.Collections.Generic;
using System.Data;

namespace DAL
{
    public class NhanVien_DAL
    {
        private readonly DatabaseHelper _dbHelper;

        public NhanVien_DAL(IConfiguration configuration)
        {
            _dbHelper = new DatabaseHelper(configuration);
        }

        // ===== Kiểm tra đã tồn tại MANV chưa
        public bool KiemTraTonTai(string manv)
        {
            try
            {
                string sql = "SELECT COUNT(*) AS SoLuong FROM NHANVIEN WHERE MANV = @MANV";
                SqlParameter[] p = { new SqlParameter("@MANV", manv) };

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
                throw new Exception("Lỗi khi kiểm tra tồn tại nhân viên: " + ex.Message);
            }
        }

        // ===== Lấy tất cả
        public List<NhanVien> GetAll()
        {
            try
            {
                var list = new List<NhanVien>();
                string sql = "SELECT MANV, TENNV, SDT, DIACHI FROM NHANVIEN";
                var dt = _dbHelper.ExecuteQuery(sql);

                foreach (DataRow r in dt.Rows)
                {
                    list.Add(new NhanVien
                    {
                        MANV = r["MANV"]?.ToString()?.Trim(),
                        TENNV = r["TENNV"]?.ToString()?.Trim(),
                        SDT = r["SDT"]?.ToString()?.Trim(),
                        DIACHI = r["DIACHI"]?.ToString()?.Trim()
                    });
                }
                return list;
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi lấy danh sách nhân viên: " + ex.Message);
            }
        }

        // ===== Lấy theo mã
        public List<NhanVien> GetByID(string manv)
        {
            try
            {
                var list = new List<NhanVien>();
                string sql = "SELECT MANV, TENNV, SDT, DIACHI FROM NHANVIEN WHERE MANV = @MANV";
                SqlParameter[] p = { new SqlParameter("@MANV", manv) };
                var dt = _dbHelper.ExecuteQuery(sql, p);

                foreach (DataRow r in dt.Rows)
                {
                    list.Add(new NhanVien
                    {
                        MANV = r["MANV"]?.ToString()?.Trim(),
                        TENNV = r["TENNV"]?.ToString()?.Trim(),
                        SDT = r["SDT"]?.ToString()?.Trim(),
                        DIACHI = r["DIACHI"]?.ToString()?.Trim()
                    });
                }
                return list;
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi lấy nhân viên theo mã: " + ex.Message);
            }
        }

        // ===== Thêm
        public bool Insert(NhanVien nv)
        {
            try
            {
                string sql = @"INSERT INTO NHANVIEN (MANV, TENNV, SDT, DIACHI)
                               VALUES (@MANV, @TENNV, @SDT, @DIACHI)";
                SqlParameter[] p =
                {
                    new SqlParameter("@MANV",   nv.MANV),
                    new SqlParameter("@TENNV",  nv.TENNV),
                    new SqlParameter("@SDT",    nv.SDT),
                    new SqlParameter("@DIACHI", nv.DIACHI)
                };

                int rows = _dbHelper.ExecuteNonQuery(sql, p);
                return rows > 0;
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi thêm nhân viên: " + ex.Message);
            }
        }

        // ===== Sửa
        public bool Update(NhanVien nv)
        {
            try
            {
                if (nv == null || string.IsNullOrWhiteSpace(nv.MANV))
                    return false;

                if (!KiemTraTonTai(nv.MANV))
                    return false;

                string sql = @"UPDATE NHANVIEN
                               SET TENNV = @TENNV, SDT = @SDT, DIACHI = @DIACHI
                               WHERE MANV = @MANV";
                SqlParameter[] p =
                {
                    new SqlParameter("@MANV",   nv.MANV),
                    new SqlParameter("@TENNV",  nv.TENNV),
                    new SqlParameter("@SDT",    nv.SDT),
                    new SqlParameter("@DIACHI", nv.DIACHI)
                };

                int rows = _dbHelper.ExecuteNonQuery(sql, p);
                return rows > 0;
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi sửa nhân viên: " + ex.Message);
            }
        }

        // ===== Xoá
        public bool Delete(string manv)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(manv))
                    return false;

                if (!KiemTraTonTai(manv))
                    return false;

                string sql = "DELETE FROM NHANVIEN WHERE MANV = @MANV";
                SqlParameter[] p = { new SqlParameter("@MANV", manv) };

                int rows = _dbHelper.ExecuteNonQuery(sql, p);
                return rows > 0;
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi xoá nhân viên: " + ex.Message);
            }
        }
    }
}
