using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Models;
using System.Data;

namespace DAL
{
    public class NhanVien_DAL
    {
        private readonly DataBase_Connect db = new DataBase_Connect();

        // Lấy toàn bộ nhân viên
        public DataTable GetAllNhanVien()
        {
            try
            {
                return db.GetDataTableFromSP("dbo.SP_GETNHANVIEN");
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi lấy danh sách nhân viên: " + ex.Message, ex);
            }
        }

        // Lấy theo mã nhân viên
        public DataTable GetByIdNhanVien(string manv)
        {
            try
            {
                var para = new[] { new SqlParameter("@MANV", manv.Trim()) };
                return db.GetDataTableFromSP("dbo.SP_GETBYIDNHANVIEN", para);
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi lấy nhân viên: " + ex.Message, ex);
            }
        }

        // Thêm nhân viên
        public DataTable CreateNhanVien(NhanVien nv)
        {
            try
            {
                var para = new[]
                {
                    new SqlParameter("@MANV", nv.MANV.Trim()),
                    new SqlParameter("@TENNV", nv.TENNV.Trim()),
                    new SqlParameter("@SDT", nv.SDT.Trim()),
                    new SqlParameter("@DIACHI", nv.DIACHI.Trim())
                };
                return db.GetDataTableFromSP("dbo.SP_THEMNHANVIEN", para);
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi thêm nhân viên: " + ex.Message, ex);
            }
        }

        // Sửa nhân viên
        public DataTable UpdateNhanVien(NhanVien nv)
        {
            try
            {
                var para = new[]
                {
                    new SqlParameter("@MANV", nv.MANV.Trim()),
                    new SqlParameter("@TENNV", nv.TENNV.Trim()),
                    new SqlParameter("@SDT", nv.SDT.Trim()),
                    new SqlParameter("@DIACHI", nv.DIACHI.Trim())
                };
                return db.GetDataTableFromSP("dbo.SP_SUANHANVIEN", para);
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi sửa nhân viên: " + ex.Message, ex);
            }
        }

        // Xoá nhân viên
        public DataTable DeleteNhanVien(string manv)
        {
            try
            {
                var para = new[] { new SqlParameter("@MANV", manv.Trim()) };
                return db.GetDataTableFromSP("dbo.SP_XOANHANVIEN", para);
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi xoá nhân viên: " + ex.Message, ex);
            }
        }
    }
}

