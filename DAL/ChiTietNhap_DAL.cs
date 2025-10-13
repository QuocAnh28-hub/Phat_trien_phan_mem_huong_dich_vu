using Microsoft.Data.SqlClient;
using Models;
using System;
using System.Data;

namespace DAL
{
    public class ChiTietNhap_DAL
    {
        DataBase_Connect db = new DataBase_Connect();

        // Lấy tất cả chi tiết nhập
        public DataTable GetAllChiTietNhap()
        {
            try
            {
                return db.GetDataTableFromSP("SP_GETCHITIETNHAP");
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi lấy danh sách chi tiết nhập: " + ex.Message);
            }
        }

        // Lấy chi tiết theo mã phiếu nhập
        public DataTable GetByPhieuNhapKho(string maphieunhap)
        {
            try
            {
                SqlParameter[] para = { new SqlParameter("@MAPHIEUNHAP", maphieunhap) };
                return db.GetDataTableFromSP("SP_GETCHITIETNHAP_BYPN", para);
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi lấy chi tiết theo phiếu nhập: " + ex.Message);
            }
        }

        // Lấy 1 dòng chi tiết theo (MAPHIEUNHAP, MASP)
        public DataTable GetByIdChiTietNhap(string maphieunhap, string masp)
        {
            try
            {
                SqlParameter[] para = {
                    new SqlParameter("@MAPHIEUNHAP", maphieunhap),
                    new SqlParameter("@MASP",        masp)
                };
                return db.GetDataTableFromSP("SP_GETBYIDCHITIETNHAP", para);
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi lấy chi tiết nhập: " + ex.Message);
            }
        }

        // Thêm chi tiết nhập
        public DataTable CreateChiTietNhap(ChiTietNhap ctn)
        {
            try
            {
                SqlParameter[] para = {
                    new SqlParameter("@MAPHIEUNHAP", ctn.MAPHIEUNHAP),
                    new SqlParameter("@MASP",        ctn.MASP),
                    new SqlParameter("@SOLUONG",     ctn.SOLUONG),
                    new SqlParameter("@DONGIANHAP",  ctn.DONGIANHAP),
                    new SqlParameter("@NGAYNHAPKHO", ctn.NGAYNHAPKHO)
                };
                return db.GetDataTableFromSP("SP_THEMCHITIETNHAP", para);
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi thêm chi tiết nhập: " + ex.Message);
            }
        }

        // Sửa chi tiết nhập
        public DataTable UpdateChiTietNhap(ChiTietNhap ctn)
        {
            try
            {
                SqlParameter[] para = {
                    new SqlParameter("@MAPHIEUNHAP", ctn.MAPHIEUNHAP),
                    new SqlParameter("@MASP",        ctn.MASP),
                    new SqlParameter("@SOLUONG",     ctn.SOLUONG),
                    new SqlParameter("@DONGIANHAP",  ctn.DONGIANHAP),
                    new SqlParameter("@NGAYNHAPKHO", ctn.NGAYNHAPKHO)
                };
                return db.GetDataTableFromSP("SP_SUACHITIETNHAP", para);
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi sửa chi tiết nhập: " + ex.Message);
            }
        }

        // Xoá chi tiết nhập
        public DataTable DeleteChiTietNhap(string maphieunhap, string masp)
        {
            try
            {
                SqlParameter[] para = {
                    new SqlParameter("@MAPHIEUNHAP", maphieunhap),
                    new SqlParameter("@MASP",        masp)
                };
                return db.GetDataTableFromSP("SP_XOACHITIETNHAP", para);
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi xoá chi tiết nhập: " + ex.Message);
            }
        }
    }
}
