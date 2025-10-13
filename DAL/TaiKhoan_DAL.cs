using Microsoft.Data.SqlClient;
using Models;
using System;
using System.Data;

namespace DAL
{
    public class TaiKhoan_DAL
    {
        DataBase_Connect db = new DataBase_Connect();

        // Lấy danh sách tất cả tài khoản
        public DataTable GetAllTaiKhoan()
        {
            try
            {
                return db.GetDataTableFromSP("SP_GETTAIKHOAN");
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi lấy danh sách tài khoản: " + ex.Message);
            }
        }

        // Lấy tài khoản theo mã
        public DataTable GetByIdTaiKhoan(string mataikhoan)
        {
            try
            {
                SqlParameter[] para = { new SqlParameter("@MATAIKHOAN", mataikhoan) };
                return db.GetDataTableFromSP("SP_GETBYIDTAIKHOAN", para);
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi lấy tài khoản: " + ex.Message);
            }
        }

        // Đăng nhập (kiểm tra username/password)
        public DataTable Login(string username, string password)
        {
            try
            {
                SqlParameter[] para = {
                    new SqlParameter("@USERNAME", username),
                    new SqlParameter("@PASS", password)
                };
                return db.GetDataTableFromSP("SP_DANGNHAP", para);
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi đăng nhập: " + ex.Message);
            }
        }

        // Thêm tài khoản
        public DataTable CreateTaiKhoan(TaiKhoan tk)
        {
            try
            {
                SqlParameter[] para = {
                    new SqlParameter("@MATAIKHOAN", tk.MATAIKHOAN),
                    new SqlParameter("@USERNAME", tk.USERNAME),
                    new SqlParameter("@PASS", tk.PASS),
                    new SqlParameter("@QUYEN", tk.QUYEN)
                };
                return db.GetDataTableFromSP("SP_THEMTAIKHOAN", para);
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi thêm tài khoản: " + ex.Message);
            }
        }

        // Sửa tài khoản
        public DataTable UpdateTaiKhoan(TaiKhoan tk)
        {
            try
            {
                SqlParameter[] para = {
                    new SqlParameter("@MATAIKHOAN", tk.MATAIKHOAN),
                    new SqlParameter("@USERNAME", tk.USERNAME),
                    new SqlParameter("@PASS", tk.PASS),
                    new SqlParameter("@QUYEN", tk.QUYEN)
                };
                return db.GetDataTableFromSP("SP_SUATAIKHOAN", para);
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi sửa tài khoản: " + ex.Message);
            }
        }

        // Xoá tài khoản
        public DataTable DeleteTaiKhoan(string mataikhoan)
        {
            try
            {
                SqlParameter[] para = { new SqlParameter("@MATAIKHOAN", mataikhoan) };
                return db.GetDataTableFromSP("SP_XOATAIKHOAN", para);
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi xoá tài khoản: " + ex.Message);
            }
        }
    }
}
