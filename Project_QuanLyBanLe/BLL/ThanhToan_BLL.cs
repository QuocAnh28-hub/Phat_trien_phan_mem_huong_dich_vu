using DAL;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class ThanhToan_BLL
    {
        private readonly ThanhToan_DAL _DAL;

        public ThanhToan_BLL(IConfiguration configuration)
        {
            _DAL = new ThanhToan_DAL(configuration);
        }

        public DataTable getAll()
        {
            return _DAL.getAll();
        }
        public DataTable GetById(string ma)
        {
            return _DAL.GetById(ma);
        }
        public DataTable Delete(string ma)
        {
            return _DAL.Delete(ma);
        }
        public DataTable Update(Models.ThanhToan model)
        {
            return _DAL.Update(model);
        }
        public DataTable Create(Models.ThanhToan model)
        {
            return _DAL.Create(model);
        }
    }
}
