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
    public class ThanhToan_DAL
    {
        private readonly DataBase_Connect db;
        public ThanhToan_DAL(IConfiguration configuration)
        {
            db = new DataBase_Connect(configuration);
        }


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

        public DataTable GetHoaDonChuaThanhToan()
        {
            try
            {
                DataTable dt = db.GetDataTableFromSP("SP_LAY_HOADON_CHUA_THANHTOAN");
                return dt;
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi lấy danh sách hóa đơn chưa thanh toán: " + ex.Message);
            }
        }

        public bool UpdateTrangThaiThanhToan(string maHDBan, string phuongThuc)
        {
            try
            {
                SqlParameter[] parameters =
                {
                    new SqlParameter("@MAHDBAN", maHDBan.Trim()),
                    new SqlParameter("@PHUONGTHUC", phuongThuc.Trim())
                };

                db.GetDataTableFromSP("SP_CAPNHAT_TRANGTHAI_THANHTOANTHANHCONG", parameters);

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi cập nhật trạng thái thanh toán: " + ex.Message);
            }
        }

        public DataTable GetHoaDonChuaThanhToanTheoTen(string tenKh)
        {
            try
            {
                SqlParameter[] parameters = {
                    new SqlParameter("@TenKhachHang", tenKh.Trim())
                };

                DataTable dt = db.GetDataTableFromSP("SP_LAY_HOADON_CHUA_THANHTOAN_THEO_TENKH", parameters);
                return dt;
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi lấy danh sách hóa đơn chưa thanh toán theo tên khách hàng: " + ex.Message);
            }
        }

        public DataTable GetByHoaDon(string maHDBan)
        {
            try
            {
                SqlParameter[] parameters =
                {
            new SqlParameter("@MAHDBAN", maHDBan.Trim())
        };

                // dùng đúng tên proc ở bước 1
                DataTable dt = db.GetDataTableFromSP("SP_GET_THANHTOAN_BY_MAHDBAN", parameters);
                return dt;
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi lấy danh sách thanh toán theo hóa đơn: " + ex.Message);
            }
        }

        public DataTable ResetSoTienByHoaDon(string maHDBan, decimal soTienMoi = 0)
        {
            try
            {
                SqlParameter[] parameters =
                {
            new SqlParameter("@MAHDBAN", maHDBan.Trim()),
            new SqlParameter("@SoTienMoi", soTienMoi)
        };

                // chỉ cần chạy proc update, không quan tâm DataTable trả về
                db.GetDataTableFromSP("SP_RESET_SOTIENTHANHTOAN_BY_MAHDBAN", parameters);

                // sau đó lấy lại danh sách thanh toán của hóa đơn để trả cho FE
                return GetByHoaDon(maHDBan);
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi reset số tiền thanh toán: " + ex.Message);
            }
        }
    }
}
