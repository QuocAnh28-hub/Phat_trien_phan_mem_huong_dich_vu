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

        public List<HoaDonBan> GetAll()
        {
            List<HoaDonBan> list = new List<HoaDonBan>();
            string sql = "SELECT MAHDBAN, MANV, MAKH, NGAYLAP, TONGTIENHANG, THUEVAT, GIAMGIA FROM HOADONBAN";

            var dt = _dbHelper.ExecuteQuery(sql);

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
                    GIAMGIA = Convert.ToDecimal(row["GIAMGIA"])
                });
            }

            return list;
        }

        public List<HoaDonBan> GetByID(string maHDB)
        {
            List<HoaDonBan> list = new List<HoaDonBan>();
            string sql = @"SELECT MAHDBAN, MANV, MAKH, NGAYLAP, TONGTIENHANG, THUEVAT, GIAMGIA FROM HOADONBAN WHERE MAHDBAN = @MAHDBAN";

            SqlParameter[] parameters = new SqlParameter[]
            {
            new SqlParameter("@MAHDBAN", maHDB)
            };

            var dt = _dbHelper.ExecuteQuery(sql, parameters);

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
                    GIAMGIA = Convert.ToDecimal(row["GIAMGIA"])
                });
            }

            return list;
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

        public bool Insert(HoaDonBan hd)
        {
            if (KiemTraTonTai(hd.MAHDBAN))
                return false;

            string sql = @"INSERT INTO HOADONBAN (MAHDBAN, MANV, MAKH, NGAYLAP, TONGTIENHANG, THUEVAT, GIAMGIA) VALUES (@MAHDBAN, @MANV, @MAKH, @NGAYLAP, @TONGTIENHANG, @THUEVAT, @GIAMGIA)";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@MAHDBAN", hd.MAHDBAN),
                new SqlParameter("@MANV", hd.MANV ?? (object)DBNull.Value),
                new SqlParameter("@MAKH", hd.MAKH ?? (object)DBNull.Value),
                new SqlParameter("@NGAYLAP", hd.NGAYLAP),
                new SqlParameter("@TONGTIENHANG", hd.TONGTIENHANG),
                new SqlParameter("@THUEVAT", hd.THUEVAT),
                new SqlParameter("@GIAMGIA", hd.GIAMGIA)
            };

            int rows = _dbHelper.ExecuteNonQuery(sql, parameters);
            return rows > 0;
        }

        public bool Update(HoaDonBan hd)
        {
            if (!KiemTraTonTai(hd.MAHDBAN))
                return false;

            string sql = @"UPDATE HOADONBAN
                       SET MANV = @MANV,
                           MAKH = @MAKH,
                           NGAYLAP = @NGAYLAP,
                           TONGTIENHANG = @TONGTIENHANG,
                           THUEVAT = @THUEVAT,
                           GIAMGIA = @GIAMGIA
                       WHERE MAHDBAN = @MAHDBAN";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@MAHDBAN", hd.MAHDBAN),
                new SqlParameter("@MANV", hd.MANV ?? (object)DBNull.Value),
                new SqlParameter("@MAKH", hd.MAKH ?? (object)DBNull.Value),
                new SqlParameter("@NGAYLAP", hd.NGAYLAP),
                new SqlParameter("@TONGTIENHANG", hd.TONGTIENHANG),
                new SqlParameter("@THUEVAT", hd.THUEVAT),
                new SqlParameter("@GIAMGIA", hd.GIAMGIA)
            };

            int rows = _dbHelper.ExecuteNonQuery(sql, parameters);
            return rows > 0;
        }

        public bool Delete(string maHDB)
        {
            if (!KiemTraTonTai(maHDB))
                return false;

            string sql = "DELETE FROM HOADONBAN WHERE MAHDBAN = @MAHDBAN";
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@MAHDBAN", maHDB)
            };

            int rows = _dbHelper.ExecuteNonQuery(sql, parameters);
            return rows > 0;
        }
    }
}
