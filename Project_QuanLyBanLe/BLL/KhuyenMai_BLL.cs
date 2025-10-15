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
    public class KhuyenMai_BLL
    {
        private readonly KhuyenMai_DAL _DAL;

        public KhuyenMai_BLL(IConfiguration configuration)
        {
            _DAL = new KhuyenMai_DAL(configuration);
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
        public DataTable Update(Models.KhuyenMai model)
        {
            return _DAL.Update(model);
        }
        public DataTable Create(Models.KhuyenMai model)
        {
            return _DAL.Create(model);
        }
    }
}
