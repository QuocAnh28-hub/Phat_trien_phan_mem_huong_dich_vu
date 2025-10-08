using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration;
using Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class HoaDonBan_DAL
    {
        private readonly DatabaseHelper _dbHelper;

        public HoaDonBan_DAL(IConfiguration configuration)
        {
            _dbHelper = new DatabaseHelper(configuration);
        }

        public bool KiemTraTonTai(string maHDB)
        {
            string sql = "SELECT COUNT(*) AS SoLuong FROM HOADONBAN WHERE MAHDBAN = @MAHDBAN";
            SqlParameter[] parameters = new SqlParameter[]
            {
            new SqlParameter("@MAHDBAN", maHDB)
            };

            var dt = _dbHelper.ExecuteQuery(sql, parameters);

            if (dt.Rows.Count > 0)
            {
                int count = Convert.ToInt32(dt.Rows[0]["SoLuong"]);
                return count > 0;
            }

            return false;
        }

        public List<HoaDonBan> GetAll()
        {
            string sql = "SELECT * FROM HOADONBAN";
            DataTable dt = _dbHelper.ExecuteQuery(sql);
            List<HoaDonBan> list = new List<HoaDonBan>();

            foreach (DataRow row in dt.Rows)
            {
                list.Add(new HoaDonBan
                {
                    MAHDBAN = row["MAHDBAN"].ToString(),
                    MANV = row["MANV"].ToString(),
                    MAKH = row["MAKH"].ToString(),
                    NGAYLAP = Convert.ToDateTime(row["NGAYLAP"]),
                    TONGTIENHANG = Convert.ToDecimal(row["TONGTIENHANG"]),
                    THUEVAT = Convert.ToDecimal(row["THUEVAT"]),
                    GIAMGIA = Convert.ToDecimal(row["GIAMGIA"]),
                    listjson_chitietban = GetChiTietByMa(row["MAHDBAN"].ToString())
                });
            }

            return list;
        }

        public List<HoaDonBan> GetByID(string maHDB)
        {
            string sql = "SELECT * FROM HOADONBAN WHERE MAHDBAN = @MAHDBAN";
            SqlParameter[] parameters = { new SqlParameter("@MAHDBAN", maHDB) };
            DataTable dt = _dbHelper.ExecuteQuery(sql, parameters);
            List<HoaDonBan> list = new List<HoaDonBan>();

            foreach (DataRow row in dt.Rows)
            {
                list.Add(new HoaDonBan
                {
                    MAHDBAN = row["MAHDBAN"].ToString(),
                    MANV = row["MANV"].ToString(),
                    MAKH = row["MAKH"].ToString(),
                    NGAYLAP = Convert.ToDateTime(row["NGAYLAP"]),
                    TONGTIENHANG = Convert.ToDecimal(row["TONGTIENHANG"]),
                    THUEVAT = Convert.ToDecimal(row["THUEVAT"]),
                    GIAMGIA = Convert.ToDecimal(row["GIAMGIA"]),
                    listjson_chitietban = GetChiTietByMa(row["MAHDBAN"].ToString())
                });
            }

            return list;
        }

        private List<ChiTietBan> GetChiTietByMa(string maHDB)
        {
            string sql = "SELECT * FROM CT_HDB WHERE MAHDBAN = @MAHDBAN";
            SqlParameter[] parameters = { new SqlParameter("@MAHDBAN", maHDB) };
            DataTable dt = _dbHelper.ExecuteQuery(sql, parameters);
            List<ChiTietBan> list = new List<ChiTietBan>();

            foreach (DataRow row in dt.Rows)
            {
                list.Add(new ChiTietBan
                {
                    MAHDBAN = row["MAHDBAN"].ToString(),
                    MASP = row["MASP"].ToString(),
                    SOLUONG = Convert.ToInt32(row["SOLUONG"]),
                    DONGIA = Convert.ToDecimal(row["DONGIA"]),
                    TONGTIEN = Convert.ToDecimal(row["TONGTIEN"])
                });
            }

            return list;
        }

        public bool Insert(HoaDonBan hd)
        {
            if (KiemTraTonTai(hd.MAHDBAN))
                return false;

            decimal tongTienSanPham = 0;
            if (hd.listjson_chitietban != null && hd.listjson_chitietban.Count > 0)
            {
                foreach (var ct in hd.listjson_chitietban)
                {
                    ct.TONGTIEN = ct.SOLUONG * ct.DONGIA;
                    tongTienSanPham += ct.TONGTIEN;
                }
            }

            decimal tongTienSauTinh = tongTienSanPham + hd.THUEVAT - hd.GIAMGIA;
            if (tongTienSauTinh < 0)
                tongTienSauTinh = 0;

            string sql = @"INSERT INTO HOADONBAN (MAHDBAN, MANV, MAKH, NGAYLAP, TONGTIENHANG, THUEVAT, GIAMGIA) VALUES (@MAHDBAN, @MANV, @MAKH, @NGAYLAP, @TONGTIENHANG, @THUEVAT, @GIAMGIA)";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@MAHDBAN", hd.MAHDBAN),
                new SqlParameter("@MANV", hd.MANV ?? (object)DBNull.Value),
                new SqlParameter("@MAKH", hd.MAKH ?? (object)DBNull.Value),
                new SqlParameter("@NGAYLAP", hd.NGAYLAP),
                new SqlParameter("@TONGTIENHANG", tongTienSauTinh),
                new SqlParameter("@THUEVAT", hd.THUEVAT),
                new SqlParameter("@GIAMGIA", hd.GIAMGIA)
            };

            int rows = _dbHelper.ExecuteNonQuery(sql, parameters);

            if (rows > 0 && hd.listjson_chitietban != null)
            {
                foreach (var ct in hd.listjson_chitietban)
                {
                    string sqlCT = @"INSERT INTO CT_HDB (MAHDBAN, MASP, SOLUONG, DONGIA, TONGTIEN) VALUES (@MAHDBAN, @MASP, @SOLUONG, @DONGIA, @TONGTIEN)";
                    SqlParameter[] p = {
                        new SqlParameter("@MAHDBAN", hd.MAHDBAN),
                        new SqlParameter("@MASP", ct.MASP),
                        new SqlParameter("@SOLUONG", ct.SOLUONG),
                        new SqlParameter("@DONGIA", ct.DONGIA),
                        new SqlParameter("@TONGTIEN", ct.TONGTIEN)
                        };
                    _dbHelper.ExecuteNonQuery(sqlCT, p);
                }
            }

            return rows > 0;
        }

        public bool Update(HoaDonBan hd)
        {
            if (!KiemTraTonTai(hd.MAHDBAN))
                return false;

            string sqlDeleteCT = "DELETE FROM CT_HDB WHERE MAHDBAN = @MAHDBAN";
            _dbHelper.ExecuteNonQuery(sqlDeleteCT, new SqlParameter[] { new SqlParameter("@MAHDBAN", hd.MAHDBAN) });

            decimal tongTienSanPham = 0;
            if (hd.listjson_chitietban != null && hd.listjson_chitietban.Count > 0)
            {
                foreach (var ct in hd.listjson_chitietban)
                {
                    ct.TONGTIEN = ct.SOLUONG * ct.DONGIA;
                    tongTienSanPham += ct.TONGTIEN;
                }
            }

            decimal tongTienSauTinh = tongTienSanPham + hd.THUEVAT - hd.GIAMGIA;
            if (tongTienSauTinh < 0)
                tongTienSauTinh = 0;

            string sql = @"UPDATE HOADONBAN SET MANV=@MANV, MAKH=@MAKH, NGAYLAP=@NGAYLAP, TONGTIENHANG=@TONGTIENHANG, THUEVAT=@THUEVAT, GIAMGIA=@GIAMGIA WHERE MAHDBAN=@MAHDBAN";

            SqlParameter[] parameters = new SqlParameter[]
            {
            new SqlParameter("@MANV", hd.MANV ?? (object)DBNull.Value),
            new SqlParameter("@MAKH", hd.MAKH ?? (object)DBNull.Value),
            new SqlParameter("@NGAYLAP", hd.NGAYLAP),
            new SqlParameter("@TONGTIENHANG", tongTienSauTinh),
            new SqlParameter("@THUEVAT", hd.THUEVAT),
            new SqlParameter("@GIAMGIA", hd.GIAMGIA),
            new SqlParameter("@MAHDBAN", hd.MAHDBAN)
            };

            int rows = _dbHelper.ExecuteNonQuery(sql, parameters);

            if (rows > 0 && hd.listjson_chitietban != null)
            {
                foreach (var ct in hd.listjson_chitietban)
                {
                    string sqlCT = @"INSERT INTO CT_HDB (MAHDBAN, MASP, SOLUONG, DONGIA, TONGTIEN) VALUES (@MAHDBAN, @MASP, @SOLUONG, @DONGIA, @TONGTIEN)";
                    SqlParameter[] p = {
                    new SqlParameter("@MAHDBAN", hd.MAHDBAN),
                    new SqlParameter("@MASP", ct.MASP),
                    new SqlParameter("@SOLUONG", ct.SOLUONG),
                    new SqlParameter("@DONGIA", ct.DONGIA),
                    new SqlParameter("@TONGTIEN", ct.TONGTIEN)
                };
                    _dbHelper.ExecuteNonQuery(sqlCT, p);
                }
            }

            return rows > 0;
        }

        public bool Delete(string maHDB)
        {
            if (!KiemTraTonTai(maHDB))
                return false;

            string sqlCT = "DELETE FROM CT_HDB WHERE MAHDBAN = @MAHDBAN";
            _dbHelper.ExecuteNonQuery(sqlCT, new SqlParameter[] { new SqlParameter("@MAHDBAN", maHDB) });

            string sql = "DELETE FROM HOADONBAN WHERE MAHDBAN = @MAHDBAN";
            SqlParameter[] parameters = { new SqlParameter("@MAHDBAN", maHDB) };

            int rows = _dbHelper.ExecuteNonQuery(sql, parameters);
            return rows > 0;
        }
    }
}
