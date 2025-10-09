using DAL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
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
        public DataTable GetById(string ma)
        {
            return NCC_DAL.GetById(ma);
        }
        public DataTable Delete(string ma)
        {
            return NCC_DAL.Delete(ma);
        }
        public DataTable Update(Models.NhaCungCap model)
        {
            return NCC_DAL.Update(model);
        }
        public DataTable Create(Models.NhaCungCap model)
        {
            return NCC_DAL.Create(model);
        }
    }
}
