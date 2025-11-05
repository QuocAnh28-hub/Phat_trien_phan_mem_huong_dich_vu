using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Models;
using System;
using System.Collections.Generic;
using System.Data;

namespace DAL
{
    public class PhieuNhapKho_DAL
    {
        private readonly DatabaseHelper _dbHelper;

        public PhieuNhapKho_DAL(IConfiguration configuration)
        {
            _dbHelper = new DatabaseHelper(configuration);
        }

        // ===== Helpers =====
        public bool KiemTraTonTai(string maPN)
        {
            try
            {
                string sql = "SELECT COUNT(*) AS SoLuong FROM PHIEUNHAPKHO WHERE MAPHIEUNHAP = @MAPHIEUNHAP";
                SqlParameter[] p = { new SqlParameter("@MAPHIEUNHAP", (maPN ?? string.Empty).Trim()) };
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
                throw new Exception("Lỗi kiểm tra tồn tại phiếu nhập: " + ex.Message);
            }
        }

        // ===== SELECT =====
        public List<PhieuNhapKho> GetAll()
        {
            try
            {
                string sql = @"SELECT MAPHIEUNHAP, MANCC, MANV, NGAYLAP
                               FROM PHIEUNHAPKHO";
                var dt = _dbHelper.ExecuteQuery(sql);
                var list = new List<PhieuNhapKho>();

                foreach (DataRow r in dt.Rows)
                {
                    list.Add(new PhieuNhapKho
                    {
                        MAPHIEUNHAP = r["MAPHIEUNHAP"]?.ToString()?.Trim(),
                        MANCC = r["MANCC"]?.ToString()?.Trim(),
                        MANV = r["MANV"]?.ToString()?.Trim(),
                        NGAYLAP = r["NGAYLAP"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(r["NGAYLAP"]),
                        listjson_chitietnhap = GetChiTietByMa(r["MAPHIEUNHAP"]?.ToString()?.Trim())
                    });
                }

                return list;
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi lấy danh sách phiếu nhập: " + ex.Message);
            }
        }

        public List<PhieuNhapKho> GetByID(string maPN)
        {
            try
            {
                string sql = @"SELECT MAPHIEUNHAP, MANCC, MANV, NGAYLAP
                               FROM PHIEUNHAPKHO
                               WHERE MAPHIEUNHAP = @MAPHIEUNHAP";
                SqlParameter[] p = { new SqlParameter("@MAPHIEUNHAP", (maPN ?? string.Empty).Trim()) };
                var dt = _dbHelper.ExecuteQuery(sql, p);
                var list = new List<PhieuNhapKho>();

                foreach (DataRow r in dt.Rows)
                {
                    list.Add(new PhieuNhapKho
                    {
                        MAPHIEUNHAP = r["MAPHIEUNHAP"]?.ToString()?.Trim(),
                        MANCC = r["MANCC"]?.ToString()?.Trim(),
                        MANV = r["MANV"]?.ToString()?.Trim(),
                        NGAYLAP = r["NGAYLAP"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(r["NGAYLAP"]),
                        listjson_chitietnhap = GetChiTietByMa(r["MAPHIEUNHAP"]?.ToString()?.Trim())
                    });
                }

                return list;
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi lấy phiếu nhập theo mã: " + ex.Message);
            }
        }

        private List<ChiTietNhap> GetChiTietByMa(string maPN)
        {
            try
            {
                string sql = @"SELECT MAPHIEUNHAP, MASP, SOLUONG, DONGIANHAP, THANHTIEN
                               FROM CHITIETNHAP WHERE MAPHIEUNHAP = @MAPHIEUNHAP";
                SqlParameter[] p = { new SqlParameter("@MAPHIEUNHAP", (maPN ?? string.Empty).Trim()) };
                var dt = _dbHelper.ExecuteQuery(sql, p);
                var list = new List<ChiTietNhap>();

                foreach (DataRow r in dt.Rows)
                {
                    list.Add(new ChiTietNhap
                    {
                        MAPHIEUNHAP = r["MAPHIEUNHAP"]?.ToString()?.Trim(),
                        MASP = r["MASP"]?.ToString()?.Trim(),
                        SOLUONG = r["SOLUONG"] == DBNull.Value ? 0 : Convert.ToInt32(r["SOLUONG"]),
                        DONGIANHAP = r["DONGIANHAP"] == DBNull.Value ? 0 : Convert.ToDecimal(r["DONGIANHAP"]),
                        THANHTIEN = r["THANHTIEN"] == DBNull.Value ? 0 : Convert.ToDecimal(r["THANHTIEN"]),
                    });
                }
                return list;
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi lấy chi tiết phiếu nhập: " + ex.Message);
            }
        }

        // ===== INSERT =====
        public bool Insert(PhieuNhapKho pnk)
        {
            try
            {
                if (KiemTraTonTai(pnk.MAPHIEUNHAP))
                    return false;

                decimal tongTienHang = 0;
                if (pnk.listjson_chitietnhap != null && pnk.listjson_chitietnhap.Count > 0)
                {
                    foreach (var ct in pnk.listjson_chitietnhap)
                    {
                        ct.THANHTIEN = ct.SOLUONG * ct.DONGIANHAP;
                        tongTienHang += ct.THANHTIEN;
                    }
                }

                decimal tongSauTinh = tongTienHang;
                if (tongSauTinh < 0) tongSauTinh = 0;

                string sql = @"INSERT INTO PHIEUNHAPKHO (MAPHIEUNHAP, MANCC, MANV, NGAYLAP)
                               VALUES (@MAPHIEUNHAP, @MANCC, @MANV, @NGAYLAP)";

                SqlParameter[] p =
                {
                    new SqlParameter("@MAPHIEUNHAP", (pnk.MAPHIEUNHAP ?? string.Empty).Trim()),
                    new SqlParameter("@MANCC", (object?)pnk.MANCC ?? DBNull.Value),
                    new SqlParameter("@MANV", (object?)pnk.MANV ?? DBNull.Value),
                    new SqlParameter("@NGAYLAP", pnk.NGAYLAP),
                };

                int rows = _dbHelper.ExecuteNonQuery(sql, p);

                if (rows > 0 && pnk.listjson_chitietnhap != null)
                {
                    foreach (var ct in pnk.listjson_chitietnhap)
                    {
                        string sqlCT = @"INSERT INTO CHITIETNHAP (MAPHIEUNHAP, MASP, SOLUONG, DONGIANHAP, THANHTIEN)
                                         VALUES (@MAPHIEUNHAP, @MASP, @SOLUONG, @DONGIANHAP, @THANHTIEN)";
                        SqlParameter[] pCT =
                        {
                            new SqlParameter("@MAPHIEUNHAP", pnk.MAPHIEUNHAP),
                            new SqlParameter("@MASP", ct.MASP),
                            new SqlParameter("@SOLUONG", ct.SOLUONG),
                            new SqlParameter("@DONGIANHAP", ct.DONGIANHAP),
                            new SqlParameter("@THANHTIEN", ct.THANHTIEN),
                        };
                        _dbHelper.ExecuteNonQuery(sqlCT, pCT);
                    }
                }

                return rows > 0;
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi thêm phiếu nhập: " + ex.Message);
            }
        }

        // ===== UPDATE =====
        public bool Update(PhieuNhapKho pnk)
        {
            try
            {
                if (!KiemTraTonTai(pnk.MAPHIEUNHAP))
                    return false;

                string sqlDelCT = "DELETE FROM CHITIETNHAP WHERE MAPHIEUNHAP = @MAPHIEUNHAP";
                _dbHelper.ExecuteNonQuery(sqlDelCT, new SqlParameter[] { new SqlParameter("@MAPHIEUNHAP", pnk.MAPHIEUNHAP) });

                decimal tongTienHang = 0;
                if (pnk.listjson_chitietnhap != null && pnk.listjson_chitietnhap.Count > 0)
                {
                    foreach (var ct in pnk.listjson_chitietnhap)
                    {
                        ct.THANHTIEN = ct.SOLUONG * ct.DONGIANHAP;
                        tongTienHang += ct.THANHTIEN;
                    }
                }

                decimal tongSauTinh = tongTienHang;
                if (tongSauTinh < 0) tongSauTinh = 0;

                string sql = @"UPDATE PHIEUNHAPKHO
                               SET MANCC=@MANCC, MANV=@MANV, NGAYLAP=@NGAYLAP
                               WHERE MAPHIEUNHAP=@MAPHIEUNHAP";

                SqlParameter[] p =
                {
                    new SqlParameter("@MANCC", (object?)pnk.MANCC ?? DBNull.Value),
                    new SqlParameter("@MANV", (object?)pnk.MANV ?? DBNull.Value),
                    new SqlParameter("@NGAYLAP", pnk.NGAYLAP),
                    new SqlParameter("@MAPHIEUNHAP", (pnk.MAPHIEUNHAP ?? string.Empty).Trim())
                };

                int rows = _dbHelper.ExecuteNonQuery(sql, p);

                if (rows > 0 && pnk.listjson_chitietnhap != null)
                {
                    foreach (var ct in pnk.listjson_chitietnhap)
                    {
                        string sqlCT = @"INSERT INTO CHITIETNHAP (MAPHIEUNHAP, MASP, SOLUONG, DONGIANHAP, THANHTIEN)
                                         VALUES (@MAPHIEUNHAP, @MASP, @SOLUONG, @DONGIANHAP, @THANHTIEN)";
                        SqlParameter[] pCT =
                        {
                            new SqlParameter("@MAPHIEUNHAP", pnk.MAPHIEUNHAP),
                            new SqlParameter("@MASP", ct.MASP),
                            new SqlParameter("@SOLUONG", ct.SOLUONG),
                            new SqlParameter("@DONGIANHAP", ct.DONGIANHAP),
                            new SqlParameter("@THANHTIEN", ct.THANHTIEN),
                        };
                        _dbHelper.ExecuteNonQuery(sqlCT, pCT);
                    }
                }

                return rows > 0;
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi cập nhật phiếu nhập: " + ex.Message);
            }
        }

        // ===== DELETE =====
        public bool Delete(string maPN)
        {
            try
            {
                if (!KiemTraTonTai(maPN))
                    return false;

                string sqlCT = "DELETE FROM CHITIETNHAP WHERE MAPHIEUNHAP = @MAPHIEUNHAP";
                _dbHelper.ExecuteNonQuery(sqlCT, new SqlParameter[] { new SqlParameter("@MAPHIEUNHAP", maPN) });

                string sql = "DELETE FROM PHIEUNHAPKHO WHERE MAPHIEUNHAP = @MAPHIEUNHAP";
                SqlParameter[] p = { new SqlParameter("@MAPHIEUNHAP", maPN) };

                int rows = _dbHelper.ExecuteNonQuery(sql, p);
                return rows > 0;
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi xoá phiếu nhập: " + ex.Message);
            }
        }
    }
}
