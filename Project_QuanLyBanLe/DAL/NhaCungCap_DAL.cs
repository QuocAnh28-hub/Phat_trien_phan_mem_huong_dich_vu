using Microsoft.Data.SqlClient;
using Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class NhaCungCap_DAL
    {
        DataBase_Connect db = new DataBase_Connect();
        public DataTable getAllNCC()
        {
            try
            {
                DataTable dt = db.GetDataTableFromSP("sp_GetNhaCungCap");
                return dt;
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi lấy danh sách: " + ex.Message);
            }
        }
    }
}
