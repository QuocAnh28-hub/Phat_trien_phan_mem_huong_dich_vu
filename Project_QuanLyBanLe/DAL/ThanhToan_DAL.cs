using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models;

namespace DAL
{
    public class ThanhToan_DAL
    {
        DataBase_Connect db = new DataBase_Connect();


        public DataTable getAll()
        {
            try
            {
                DataTable dt = db.GetDataTableFromSP("sp_GetThanhToan");
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
                SqlParameter[] para = { new SqlParameter("@MAThanhToan", ma) };
                DataTable dt = db.GetDataTableFromSP("sp_GetByIDTT", para);
                return dt;
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi lấy: " + ex.Message);
            }
        }

        public DataTable Create(Models.ThanhToan model)
        {
            try
            {
                SqlParameter[] parameters = { new SqlParameter("@MaThanhToan", model.MaThanhToan.Trim()),
                                             new SqlParameter("@MaHDBan",  model.MaHDBan.Trim()),
                                             new SqlParameter("@PhuongThuc",  model.PhuongThuc.Trim()),
                                                new SqlParameter("@SoTienThanhToan",  model.SoTienThanhToan),
                                                new SqlParameter("@NgayThanhToan",  model.NgayThanhToan),
                                                new SqlParameter("@TrangThai",  model.TrangThai)};
                DataTable dt = db.GetDataTableFromSP("SP_THEMTT", parameters);
                return dt;
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi thêm: " + ex.Message);
            }
        }

        public DataTable Update(Models.ThanhToan model)
        {
            try
            {
                SqlParameter[] parameters = { new SqlParameter("@MaThanhToan", model.MaThanhToan.Trim()),
                                             new SqlParameter("@PhuongThuc",  model.PhuongThuc.Trim()),
                                                new SqlParameter("@TrangThai",  model.TrangThai)};
                DataTable dt = db.GetDataTableFromSP("SP_SUATT", parameters);
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
                SqlParameter[] parameters = { new SqlParameter("@MAThanhToan", ma) };
                DataTable dt = db.GetDataTableFromSP("SP_XOAtt", parameters);
                return dt;
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi xoá: " + ex.Message);
            }
        }
    }
}
