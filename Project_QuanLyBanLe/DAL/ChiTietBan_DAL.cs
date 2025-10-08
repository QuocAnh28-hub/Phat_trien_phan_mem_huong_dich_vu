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
    public class ChiTietBan_DAL
    {
        private readonly DatabaseHelper _dbHelper;

        public ChiTietBan_DAL(IConfiguration configuration)
        {
            _dbHelper = new DatabaseHelper(configuration);
        }

        public List<ChiTietBan> GetAll()
        {
            List<ChiTietBan> list = new List<ChiTietBan>();
            string sql = "SELECT MAHDBAN, MASP, SOLUONG, DONGIA, TONGTIEN FROM CT_HDB";

            var dt = _dbHelper.ExecuteQuery(sql);

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

        public List<ChiTietBan> GetByHoaDon(string maHDB)
        {
            List<ChiTietBan> list = new List<ChiTietBan>();
            string sql = "SELECT MAHDBAN, MASP, SOLUONG, DONGIA, TONGTIEN FROM CT_HDB WHERE MAHDBAN = @MAHDBAN";

            SqlParameter[] parameters = new SqlParameter[]
            {
            new SqlParameter("@MAHDBAN", maHDB)
            };

            var dt = _dbHelper.ExecuteQuery(sql, parameters);

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

        public bool KiemTraTonTai(string maHDB, string maSP)
        {
            string sql = "SELECT COUNT(*) AS SoLuong FROM CT_HDB WHERE MAHDBAN = @MAHDBAN AND MASP = @MASP";
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@MAHDBAN", maHDB),
                new SqlParameter("@MASP", maSP)
            };

            var dt = _dbHelper.ExecuteQuery(sql, parameters);
            if (dt.Rows.Count > 0)
            {
                int count = Convert.ToInt32(dt.Rows[0]["SoLuong"]);
                return count > 0;
            }

            return false;
        }

        public bool Insert(ChiTietBan ct)
        {
            if (KiemTraTonTai(ct.MAHDBAN, ct.MASP))
                return false;

            string sql = @"INSERT INTO CT_HDB (MAHDBAN, MASP, SOLUONG, DONGIA, TONGTIEN) VALUES (@MAHDBAN, @MASP, @SOLUONG, @DONGIA, @TONGTIEN)";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@MAHDBAN", ct.MAHDBAN),
                new SqlParameter("@MASP", ct.MASP),
                new SqlParameter("@SOLUONG", ct.SOLUONG),
                new SqlParameter("@DONGIA", ct.DONGIA),
                new SqlParameter("@TONGTIEN", ct.TONGTIEN)
            };

            int rows = _dbHelper.ExecuteNonQuery(sql, parameters);
            return rows > 0;
        }

        public bool Update(ChiTietBan ct)
        {
            if (!KiemTraTonTai(ct.MAHDBAN, ct.MASP))
                return false;

            string sql = @"UPDATE CT_HDB
                       SET SOLUONG = @SOLUONG,
                           DONGIA = @DONGIA,
                           TONGTIEN = @TONGTIEN
                       WHERE MAHDBAN = @MAHDBAN AND MASP = @MASP";
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@MAHDBAN", ct.MAHDBAN),
                new SqlParameter("@MASP", ct.MASP),
                new SqlParameter("@SOLUONG", ct.SOLUONG),
                new SqlParameter("@DONGIA", ct.DONGIA),
                new SqlParameter("@TONGTIEN", ct.TONGTIEN)
            };

            int rows = _dbHelper.ExecuteNonQuery(sql, parameters);
            return rows > 0;
        }

        public bool Delete(string maHDB, string maSP)
        {
            if (!KiemTraTonTai(maHDB, maSP))
                return false;

            string sql = "DELETE FROM CT_HDB WHERE MAHDBAN = @MAHDBAN AND MASP = @MASP";
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@MAHDBAN", maHDB),
                new SqlParameter("@MASP", maSP)
            };

            int rows = _dbHelper.ExecuteNonQuery(sql, parameters);
            return rows > 0;
        }
    }
}
