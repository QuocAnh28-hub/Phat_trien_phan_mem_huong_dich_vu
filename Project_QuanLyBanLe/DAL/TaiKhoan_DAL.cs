using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Models;
using System;
using System.Collections.Generic;
using System.Data;

namespace DAL
{
    public class TaiKhoan_DAL
    {
        private readonly DatabaseHelper _dbHelper;

        public TaiKhoan_DAL(IConfiguration configuration)
        {
            _dbHelper = new DatabaseHelper(configuration);
        }

        // ===== Helpers =====
        public bool KiemTraTonTai(string maTK)
        {
            try
            {
                string sql = "SELECT COUNT(*) AS SoLuong FROM TAIKHOAN WHERE MATAIKHOAN = @MATAIKHOAN";
                SqlParameter[] p = { new SqlParameter("@MATAIKHOAN", maTK) };
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
                throw new Exception("Lỗi kiểm tra tồn tại tài khoản: " + ex.Message);
            }
        }

        // ===== SELECT =====
        public List<TaiKhoan> GetAll()
        {
            try
            {
                var list = new List<TaiKhoan>();
                string sql = @"SELECT MATAIKHOAN, USERNAME, PASS, QUYEN
                               FROM TAIKHOAN";
                var dt = _dbHelper.ExecuteQuery(sql);

                foreach (DataRow r in dt.Rows)
                {
                    list.Add(new TaiKhoan
                    {
                        MATAIKHOAN = r["MATAIKHOAN"]?.ToString()?.Trim(),
                        USERNAME = r["USERNAME"]?.ToString()?.Trim(),
                        PASS = r["PASS"]?.ToString()?.Trim(),
                        QUYEN = r["QUYEN"] == DBNull.Value ? 0 : Convert.ToInt32(r["QUYEN"])
                    });
                }
                return list;
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi lấy danh sách tài khoản: " + ex.Message);
            }
        }

        public List<TaiKhoan> GetByID(string maTK)
        {
            try
            {
                var list = new List<TaiKhoan>();
                string sql = @"SELECT MATAIKHOAN, USERNAME, PASS, QUYEN
                               FROM TAIKHOAN
                               WHERE MATAIKHOAN = @MATAIKHOAN";
                SqlParameter[] p = { new SqlParameter("@MATAIKHOAN", maTK) };
                var dt = _dbHelper.ExecuteQuery(sql, p);

                foreach (DataRow r in dt.Rows)
                {
                    list.Add(new TaiKhoan
                    {
                        MATAIKHOAN = r["MATAIKHOAN"]?.ToString()?.Trim(),
                        USERNAME = r["USERNAME"]?.ToString()?.Trim(),
                        PASS = r["PASS"]?.ToString()?.Trim(),
                        QUYEN = r["QUYEN"] == DBNull.Value ? 0 : Convert.ToInt32(r["QUYEN"])
                    });
                }
                return list;
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi lấy tài khoản theo mã: " + ex.Message);
            }
        }

        // ===== LOGIN =====
        // Trả về 0 hoặc 1 bản ghi phù hợp với username/password
        public List<TaiKhoan> Login(string username, string password)
        {
            try
            {
                var list = new List<TaiKhoan>();
                string sql = @"
                    SELECT TOP 1 MATAIKHOAN, USERNAME, PASS, QUYEN
                    FROM TAIKHOAN
                    WHERE USERNAME = @USERNAME AND PASS = @PASS";

                SqlParameter[] p =
                {
                    new SqlParameter("@USERNAME", username),
                    new SqlParameter("@PASS", password)
                };

                DataTable dt = _dbHelper.ExecuteQuery(sql, p);
                foreach (DataRow r in dt.Rows)
                {
                    list.Add(new TaiKhoan
                    {
                        MATAIKHOAN = r["MATAIKHOAN"].ToString().Trim(),
                        USERNAME = r["USERNAME"].ToString().Trim(),
                        PASS = r["PASS"].ToString().Trim(),
                        QUYEN = r["QUYEN"] == DBNull.Value ? 0 : Convert.ToInt32(r["QUYEN"])
                    });
                }

                return list;
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi đăng nhập: " + ex.Message);
            }
        }

        public int GetRoleByUsername(string username)
        {
            try
            {
                string sql = "SELECT TOP 1 QUYEN FROM TAIKHOAN WHERE USERNAME = @USERNAME";
                SqlParameter[] p =
                {
                    new SqlParameter("@USERNAME", username)
                };

                DataTable dt = _dbHelper.ExecuteQuery(sql, p);
                if (dt.Rows.Count > 0)
                    return Convert.ToInt32(dt.Rows[0]["QUYEN"]);

                return 0;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        // ===== INSERT =====
        public bool Insert(TaiKhoan tk)
        {
            try
            {
                string sql = @"INSERT INTO TAIKHOAN (MATAIKHOAN, USERNAME, PASS, QUYEN)
                               VALUES (@MATAIKHOAN, @USERNAME, @PASS, @QUYEN)";
                SqlParameter[] p =
                {
                    new SqlParameter("@MATAIKHOAN", tk.MATAIKHOAN),
                    new SqlParameter("@USERNAME",   tk.USERNAME),
                    new SqlParameter("@PASS",       tk.PASS),
                    new SqlParameter("@QUYEN",      tk.QUYEN)
                };
                int rows = _dbHelper.ExecuteNonQuery(sql, p);
                return rows > 0;
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi thêm tài khoản: " + ex.Message);
            }
        }

        // ===== UPDATE =====
        public bool Update(TaiKhoan tk)
        {
            try
            {
                if (tk == null || string.IsNullOrWhiteSpace(tk.MATAIKHOAN))
                    return false;

                if (!KiemTraTonTai(tk.MATAIKHOAN))
                    return false;

                string sql = @"UPDATE TAIKHOAN
                               SET USERNAME = @USERNAME,
                                   PASS = @PASS,
                                   QUYEN = @QUYEN
                               WHERE MATAIKHOAN = @MATAIKHOAN";
                SqlParameter[] p =
                {
                    new SqlParameter("@MATAIKHOAN", tk.MATAIKHOAN),
                    new SqlParameter("@USERNAME",   tk.USERNAME),
                    new SqlParameter("@PASS",       tk.PASS),
                    new SqlParameter("@QUYEN",      tk.QUYEN)
                };
                int rows = _dbHelper.ExecuteNonQuery(sql, p);
                return rows > 0;
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi sửa tài khoản: " + ex.Message);
            }
        }

        // ===== DELETE =====
        public bool Delete(string maTK)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(maTK))
                    return false;

                if (!KiemTraTonTai(maTK))
                    return false;

                string sql = "DELETE FROM TAIKHOAN WHERE MATAIKHOAN = @MATAIKHOAN";
                SqlParameter[] p = { new SqlParameter("@MATAIKHOAN", maTK) };
                int rows = _dbHelper.ExecuteNonQuery(sql, p);
                return rows > 0;
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi xoá tài khoản: " + ex.Message);
            }
        }
    }
}
