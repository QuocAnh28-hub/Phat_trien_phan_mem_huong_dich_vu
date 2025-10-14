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
    public class KhuyenMai_DAL
    {
        private readonly DataBase_Connect db;
        public KhuyenMai_DAL(IConfiguration configuration)
        {
            db = new DataBase_Connect(configuration);
        }


        public DataTable getAll()
        {
            try
            {
                DataTable dt = db.GetDataTableFromSP("sp_GetKhuyenMai");
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
                SqlParameter[] para = { new SqlParameter("@MAKM", ma) };
                DataTable dt = db.GetDataTableFromSP("sp_GetByIDKM", para);
                return dt;
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi lấy: " + ex.Message);
            }
        }

        public DataTable Create(Models.KhuyenMai model)
        {
            try
            {
                SqlParameter[] parameters = { new SqlParameter("@MAKM", model.MaKM.Trim()),
                                             new SqlParameter("@TENKM",  model.TenKM.Trim()),
                                             new SqlParameter("@MASP",  model.MaSP.Trim()),
                                                new SqlParameter("@NGAYBATDAU",  model.NgayBD),
                                                new SqlParameter("@NGAYKETTHUC",  model.NgayKT)};
                DataTable dt = db.GetDataTableFromSP("SP_THEMKM", parameters);
                return dt;
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi thêm: " + ex.Message);
            }
        }

        public DataTable Update(Models.KhuyenMai model)
        {
            try
            {
                SqlParameter[] parameters = { new SqlParameter("@MAKM", model.MaKM.Trim()),
                                             new SqlParameter("@TENKM",  model.TenKM.Trim()),
                                             new SqlParameter("@MASP",  model.MaSP.Trim()),
                                                new SqlParameter("@NGAYBATDAU",  model.NgayBD),
                                                new SqlParameter("@NGAYKETTHUC",  model.NgayKT)};
                DataTable dt = db.GetDataTableFromSP("SP_SUAKM", parameters);
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
                SqlParameter[] parameters = { new SqlParameter("@MAKM", ma) };
                DataTable dt = db.GetDataTableFromSP("SP_XOAKM", parameters);
                return dt;
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi xoá: " + ex.Message);
            }
        }
    }
}
