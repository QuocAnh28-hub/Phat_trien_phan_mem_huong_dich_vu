using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class NhaCungCap_DAL
    {
        private readonly DataBase_Connect db;
        public NhaCungCap_DAL(IConfiguration configuration)
        {
            db = new DataBase_Connect(configuration);
        }


        public DataTable getAllNCC()
        {
            try
            {
                DataTable dt = db.GetDataTableFromSP("sp_GetNhaCungCap");
                return dt;
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi lấy danh sách: " + ex.Message);
            }
        }
        public DataTable GetById(string ma)
        {
            try
            {
                SqlParameter[] para = { new SqlParameter("@MANCC", ma) };
                DataTable dt = db.GetDataTableFromSP("sp_GetByIDNhaCungCap", para);
                return dt;
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi lấy: " + ex.Message);
            }
        }

        public DataTable Create(Models.NhaCungCap model)
        {
            try
            {
                SqlParameter[] parameters = { new SqlParameter("@MANCC", model.MaNCC.Trim()),
                                             new SqlParameter("@TENNCC",  model.TenNCC.Trim()),
                                             new SqlParameter("@DIACHI",  model.DiaChi.Trim()),
                                                new SqlParameter("@SDT",  model.SDT.Trim()),
                                                new SqlParameter("@EMAIL",  model.EMAIL.Trim())};
                DataTable dt = db.GetDataTableFromSP("SP_THEMNCC", parameters);
                return dt;
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi thêm: " + ex.Message);
            }
        }

        public DataTable Update(Models.NhaCungCap model)
        {
            try
            {
                SqlParameter[] parameters = { new SqlParameter("@MANCC", model.MaNCC.Trim()),
                                             new SqlParameter("@TENNCC",  model.TenNCC.Trim()),
                                             new SqlParameter("@DIACHI",  model.DiaChi.Trim()),
                                                new SqlParameter("@SDT",  model.SDT.Trim()),
                                                new SqlParameter("@EMAIL",  model.EMAIL.Trim())};
                DataTable dt = db.GetDataTableFromSP("SP_SUANCC", parameters);
                return dt;
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi sửa: " + ex.Message);
            }
        }
        public DataTable Delete(string ma)
        {
            try
            {
                SqlParameter[] parameters = { new SqlParameter("@MANCC", ma) };
                DataTable dt = db.GetDataTableFromSP("SP_XOANCC", parameters);
                return dt;
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi xoá: " + ex.Message);
            }
        }
    }
}
