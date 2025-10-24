using DAL;
using Microsoft.Extensions.Configuration;
using Models;
using System.Collections.Generic;

namespace BLL
{
    public class TaiKhoan_BLL
    {
        private readonly TaiKhoan_DAL tk_dal;

        public TaiKhoan_BLL(IConfiguration configuration)
        {
            tk_dal = new TaiKhoan_DAL(configuration);
        }

        public List<TaiKhoan> LayTatCa()
        {
            var list = tk_dal.GetAll();
            return (list == null || list.Count == 0) ? new List<TaiKhoan>() : list;
        }

        public List<TaiKhoan> LayTheoID(string mataikhoan)
        {
            if (string.IsNullOrWhiteSpace(mataikhoan)) return null;
            if (!tk_dal.KiemTraTonTai(mataikhoan)) return null;

            return tk_dal.GetByID(mataikhoan);
        }

        public bool ThemMoi(TaiKhoan tk)
        {
            if (tk == null) return false;
            if (string.IsNullOrWhiteSpace(tk.MATAIKHOAN) ||
                string.IsNullOrWhiteSpace(tk.USERNAME) ||
                string.IsNullOrWhiteSpace(tk.PASS)) return false;

            if (tk_dal.KiemTraTonTai(tk.MATAIKHOAN)) return false;

            return tk_dal.Insert(tk);
        }

        public bool CapNhat(TaiKhoan tk)
        {
            if (tk == null) return false;
            if (string.IsNullOrWhiteSpace(tk.MATAIKHOAN) ||
                string.IsNullOrWhiteSpace(tk.USERNAME) ||
                string.IsNullOrWhiteSpace(tk.PASS)) return false;

            return tk_dal.Update(tk);
        }

        public bool Xoa(string mataikhoan)
        {
            if (string.IsNullOrWhiteSpace(mataikhoan)) return false;
            if (!tk_dal.KiemTraTonTai(mataikhoan)) return false;

            return tk_dal.Delete(mataikhoan);
        }

        // Đăng nhập
        public List<TaiKhoan> DangNhap(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                return new List<TaiKhoan>();

            return tk_dal.Login(username, password);
        }

        // Lấy quyền theo username
        public int LayQuyen(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                return 0;

            return tk_dal.GetRoleByUsername(username);
        }
    }
}
