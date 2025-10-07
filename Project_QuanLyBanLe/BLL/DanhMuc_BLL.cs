using DAL;
using Microsoft.Extensions.Configuration;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class DanhMuc_BLL
    {
        private readonly DanhMuc_DAL dm_dal;

        public DanhMuc_BLL(IConfiguration configuration)
        {
            dm_dal = new DanhMuc_DAL(configuration);
        }

        public List<DanhMuc> LayTatCa() => dm_dal.GetAll();

        public bool ThemMoi(DanhMuc dm) => dm_dal.Insert(dm);
    }
}
