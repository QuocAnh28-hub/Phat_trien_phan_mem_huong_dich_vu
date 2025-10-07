using DAL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class NhaCungCap_BLL
    {
        NhaCungCap_DAL NCC_DAL = new NhaCungCap_DAL();

        public DataTable getAllNCC()
        {
            return NCC_DAL.getAllNCC();
        }
    }
}
