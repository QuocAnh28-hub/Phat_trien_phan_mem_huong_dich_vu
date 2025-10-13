using System;
using System.Data;
using DAL;
using Models;

namespace BLL
{
    public class TaiKhoan_BLL
    {
        TaiKhoan_DAL TK_DAL = new TaiKhoan_DAL();

        public DataTable GetAllTaiKhoan()
        {
            return TK_DAL.GetAllTaiKhoan();
        }

        public DataTable GetByIdTaiKhoan(string mataikhoan)
        {
            return TK_DAL.GetByIdTaiKhoan(mataikhoan);
        }

        public DataTable CreateTaiKhoan(TaiKhoan tk)
        {
            return TK_DAL.CreateTaiKhoan(tk);
        }

        public DataTable UpdateTaiKhoan(TaiKhoan tk)
        {
            return TK_DAL.UpdateTaiKhoan(tk);
        }

        public DataTable DeleteTaiKhoan(string mataikhoan)
        {
            return TK_DAL.DeleteTaiKhoan(mataikhoan);
        }

        public DataTable Login(string username, string password)
        {
            return TK_DAL.Login(username, password);
        }
    }
}
