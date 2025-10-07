using Microsoft.Extensions.Configuration;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace DAL
{
    public class DanhMuc_DAL
    {
        private readonly DatabaseHelper _dbHelper;

        public DanhMuc_DAL(IConfiguration configuration)
        {
            _dbHelper = new DatabaseHelper(configuration);
        }

        public List<DanhMuc> GetAll()
        {
            List<DanhMuc> list = new List<DanhMuc>();
            string sql = "SELECT MADANHMUC, TENDANHMUC, MOTA FROM DANHMUC";
            var dt = _dbHelper.ExecuteQuery(sql);

            foreach (System.Data.DataRow row in dt.Rows)
            {
                list.Add(new DanhMuc
                {
                    MADANHMUC = row["MADANHMUC"].ToString(),
                    TENDANHMUC = row["TENDANHMUC"].ToString(),
                    MOTA = row["MOTA"].ToString()
                });
            }

            return list;
        }

        public List<DanhMuc> GetbyID(string madanhmuc)
        {
            List<DanhMuc> list = new List<DanhMuc>();
            string sql = "SELECT MADANHMUC, TENDANHMUC, MOTA FROM DANHMUC WHERE MADANHMUC ='" + madanhmuc + "'";
            var dt = _dbHelper.ExecuteQuery(sql);

            foreach (System.Data.DataRow row in dt.Rows)
            {
                list.Add(new DanhMuc
                {
                    MADANHMUC = row["MADANHMUC"].ToString(),
                    TENDANHMUC = row["TENDANHMUC"].ToString(),
                    MOTA = row["MOTA"].ToString()
                });
            }

            return list;
        }

        public bool Insert(DanhMuc dm)
        {
            string sql = "INSERT INTO DANHMUC (MADANHMUC, TENDANHMUC, MOTA) VALUES (@ma, @ten, @mota)";
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@ma", dm.MADANHMUC),
                new SqlParameter("@ten", dm.TENDANHMUC),
                new SqlParameter("@mota", dm.MOTA)
            };

            int rows = _dbHelper.ExecuteNonQuery(sql, parameters);
            return rows > 0;
        }
    }
}
