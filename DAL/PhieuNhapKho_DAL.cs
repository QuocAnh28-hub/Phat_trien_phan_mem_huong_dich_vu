using Microsoft.Data.SqlClient;
using Models;
using System;
using System.Data;

namespace DAL
{
    public class PhieuNhapKho_DAL
    {
        DataBase_Connect db = new DataBase_Connect();

        // Lấy danh sách phiếu nhập kho
        public DataTable GetAllPhieuNhapKho()
        {
            try
            {
                return db.GetDataTableFromSP("SP_GETPHIEUNHAPKHO");
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi lấy danh sách phiếu nhập kho: " + ex.Message);
            }
        }

        // Lấy 1 phiếu nhập kho theo mã
        public DataTable GetByIdPhieuNhapKho(string maphieunhap)
        {
            try
            {
                SqlParameter[] para = { new SqlParameter("@MAPHIEUNHAP", maphieunhap) };
                return db.GetDataTableFromSP("SP_GETBYIDPHIEUNHAPKHO", para);
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi lấy phiếu nhập kho: " + ex.Message);
            }
        }

        // Thêm phiếu nhập kho
        public DataTable CreatePhieuNhapKho(PhieuNhapKho pnk)
        {
            try
            {
                SqlParameter[] para = {
                    new SqlParameter("@MAPHIEUNHAP", pnk.MAPHIEUNHAP),
                    new SqlParameter("@MASP",        pnk.MASP),
                    new SqlParameter("@MANCC",       pnk.MANCC),
                    new SqlParameter("@MANV",        pnk.MANV),
                    new SqlParameter("@NGAYLAP",     pnk.NGAYLAP),
                    new SqlParameter("@THUEVAT",     pnk.THUEVAT)
                };
                return db.GetDataTableFromSP("SP_THEMPHIEUNHAPKHO", para);
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi thêm phiếu nhập kho: " + ex.Message);
            }
        }

        // Sửa phiếu nhập kho
        public DataTable UpdatePhieuNhapKho(PhieuNhapKho pnk)
        {
            try
            {
                SqlParameter[] para = {
                    new SqlParameter("@MAPHIEUNHAP", pnk.MAPHIEUNHAP),
                    new SqlParameter("@MASP",        pnk.MASP),
                    new SqlParameter("@MANCC",       pnk.MANCC),
                    new SqlParameter("@MANV",        pnk.MANV),
                    new SqlParameter("@NGAYLAP",     pnk.NGAYLAP),
                    new SqlParameter("@THUEVAT",     pnk.THUEVAT)
                };
                return db.GetDataTableFromSP("SP_SUAPHIEUNHAPKHO", para);
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi sửa phiếu nhập kho: " + ex.Message);
            }
        }

        // Xoá phiếu nhập kho
        public DataTable DeletePhieuNhapKho(string maphieunhap)
        {
            try
            {
                SqlParameter[] para = { new SqlParameter("@MAPHIEUNHAP", maphieunhap) };
                return db.GetDataTableFromSP("SP_XOAPHIEUNHAPKHO", para);
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi xoá phiếu nhập kho: " + ex.Message);
            }
        }
    }
}
