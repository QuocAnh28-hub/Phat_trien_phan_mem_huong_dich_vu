using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace DAL
{
    public class DataBase_Connect
    {
        protected string strCon = @"Data Source = HOANGLE\SQLEXPRESS;
                                    Initial Catalog = QUANLYBANLE;
                                    Integrated Security = True;
                                    TrustServerCertificate = True;";

        SqlDataAdapter sqlAdap;
        DataTable dt;
        SqlCommand cmd;

        //phi kết nối
        public DataTable GetDataTableFromSP(string spName, params SqlParameter[] parameters)
        {
            using (SqlConnection conn = new SqlConnection(strCon))
            {
                cmd = new SqlCommand(spName, conn);
                cmd.CommandType = CommandType.StoredProcedure;

                // Nếu có tham số thì thêm vào
                if (parameters != null)
                    cmd.Parameters.AddRange(parameters);

                sqlAdap = new SqlDataAdapter(cmd);
                dt = new DataTable();
                sqlAdap.Fill(dt);
                return dt;
            }
        }

        public int ExecuteSP(string spName, params SqlParameter[] parameters)
        {
            using (SqlConnection conn = new SqlConnection(strCon))
            {
                conn.Open();
                cmd = new SqlCommand(spName, conn);
                cmd.CommandType = CommandType.StoredProcedure;

                // Nếu có tham số thì thêm vào
                if (parameters != null)
                    cmd.Parameters.AddRange(parameters);

                int row = cmd.ExecuteNonQuery();
                return row; // Trả về số dòng bị ảnh hưởng
            }
        }
        


    }
}
