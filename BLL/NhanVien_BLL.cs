using DAL;
using Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class NhanVien_BLL
    {
        NhanVien_DAL NV_DAL = new NhanVien_DAL();

        public DataTable GetAllNhanVien() => NV_DAL.GetAllNhanVien();
        public DataTable GetByIdNhanVien(string manv) => NV_DAL.GetByIdNhanVien(manv);
        public DataTable CreateNhanVien(NhanVien nv) => NV_DAL.CreateNhanVien(nv);
        public DataTable UpdateNhanVien(NhanVien nv) => NV_DAL.UpdateNhanVien(nv);
        public DataTable DeleteNhanVien(string manv) => NV_DAL.DeleteNhanVien(manv);
    }
}
