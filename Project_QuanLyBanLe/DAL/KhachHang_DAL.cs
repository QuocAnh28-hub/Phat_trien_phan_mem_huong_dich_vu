using Microsoft.Data.SqlClient;
using Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace DAL
{
    public class KhachHang_DAL
    {
        private readonly DataBase_Connect db;
        public KhachHang_DAL(IConfiguration configuration)
        {
            db = new DataBase_Connect(configuration);
        }
        public DataTable getAllKH()
        {
            try
            {          
                DataTable dt = db.GetDataTableFromSP("sp_GetKhachHang");
                return dt;
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi lấy danh sách: " + ex.Message);
            }
        }

        public DataTable GetByIdKH(string makh)
        {
            try
            {
                SqlParameter[] para = { new SqlParameter("@MAKH", makh) };
                DataTable dt = db.GetDataTableFromSP("sp_GetByIDKhachHang", para);
                return dt;
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi lấy: " + ex.Message);
            }
        }

        public DataTable CreateKH(Models.KhachHang kh)
        {
            try
            {
                SqlParameter[] parameters = { new SqlParameter("@MAKH", kh.MaKH.Trim()),
                                             new SqlParameter("@TENKH", kh.TenKH.Trim()),
                                             new SqlParameter("@SDT", kh.SDT.Trim()),
                                             new SqlParameter("@DIACHI", kh.DiaChi.Trim())};
                DataTable dt = db.GetDataTableFromSP("SP_THEMKH", parameters);
                return dt;
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi thêm: " + ex.Message);
            }
        }

        public DataTable UpdateByIdKH(Models.KhachHang kh)
        {
            try
            {
                SqlParameter[] parameters = { new SqlParameter("@MAKH", kh.MaKH.Trim()),
                                             new SqlParameter("@TENKH", kh.TenKH.Trim()),
                                             new SqlParameter("@SDT", kh.SDT.Trim()),
                                             new SqlParameter("@DIACHI", kh.DiaChi.Trim())};
                DataTable dt = db.GetDataTableFromSP("SP_SUAKH", parameters);
                return dt;
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi sửa: " + ex.Message);
            }
        }
        public DataTable DeleteByIdKH(string makh)
        {
            try
            {
                SqlParameter[] parameters ={ new SqlParameter("@MAKH", makh) };
                DataTable dt = db.GetDataTableFromSP("SP_XOAKH", parameters);
                return dt;
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi xoá: " + ex.Message);
            }
        }
    }
}
