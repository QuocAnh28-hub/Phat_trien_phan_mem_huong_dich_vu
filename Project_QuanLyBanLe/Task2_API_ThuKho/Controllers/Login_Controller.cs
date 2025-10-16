using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using BLL;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Models;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Task2_API_ThuKho.Controllers
{
    [Route("api/Login")]
    [ApiController]
    public class Login_Controller : ControllerBase
    {
        private readonly TaiKhoan_BLL _bll;
        public Login_Controller(IConfiguration configuration)
        {
            _bll = new TaiKhoan_BLL(configuration);
        }
        // 🔹 Đăng nhập
        [HttpPost("login")]
        public IActionResult Login([FromBody] TaiKhoan tk)
        {
            try
            {
                if (tk == null || string.IsNullOrWhiteSpace(tk.USERNAME) || string.IsNullOrWhiteSpace(tk.PASS))
                    return Ok(new { success = false, message = "Thiếu username/password" });

                var list = _bll.DangNhap(tk.USERNAME, tk.PASS);
                if (list == null || list.Count == 0)
                    return Ok(new { success = false, message = "Sai tên đăng nhập hoặc mật khẩu" });

                var data = list.Select(x => new {
                    MaTaiKhoan = x.MATAIKHOAN?.Trim(),
                    UserName = x.USERNAME?.Trim(),
                    Quyen = x.QUYEN
                })
                           .ToList();

                return Ok(new { success = true, message = "Đăng nhập thành công", data });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi: " + ex.Message });
            }
        }
    }
}
