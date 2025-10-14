using BLL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Models;

namespace API_TaiKhoan.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaiKhoanController : ControllerBase
    {
        private readonly TaiKhoan_BLL _bll;
        public TaiKhoanController(IConfiguration configuration)
        {
            _bll = new TaiKhoan_BLL(configuration);
        }

        // 🔹 Lấy tất cả tài khoản
        [HttpGet("get-all-taikhoan")]
        public IActionResult GetAllTaiKhoan()
        {
            try
            {
                var data = _bll.LayTatCa()
                               .Select(x => new {
                                   MaTaiKhoan = x.MATAIKHOAN?.Trim(),
                                   UserName = x.USERNAME?.Trim(),
                                   Quyen = x.QUYEN
                               })
                               .ToList();

                return Ok(new { success = true, message = "Lấy danh sách tài khoản thành công", data });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi: " + ex.Message });
            }
        }

        // 🔹 Lấy tài khoản theo ID
        [HttpGet("get-byid-taikhoan")]
        public IActionResult GetByIdTaiKhoan([FromQuery] string mataikhoan)
        {
            try
            {
                var list = _bll.LayTheoID(mataikhoan);
                if (list == null || list.Count == 0)
                    return Ok(new { success = false, message = "Không tìm thấy tài khoản" });

                var data = list.Select(x => new {
                    MaTaiKhoan = x.MATAIKHOAN?.Trim(),
                    UserName = x.USERNAME?.Trim(),
                    Quyen = x.QUYEN
                })
                           .ToList();

                return Ok(new { success = true, message = "Lấy thông tin tài khoản thành công", data });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi: " + ex.Message });
            }
        }

        // 🔹 Thêm tài khoản
        [HttpPost("create-taikhoan")]
        public IActionResult CreateTaiKhoan([FromBody] TaiKhoan tk)
        {
            try
            {
                if (tk == null || string.IsNullOrWhiteSpace(tk.MATAIKHOAN) ||
                    string.IsNullOrWhiteSpace(tk.USERNAME) || string.IsNullOrWhiteSpace(tk.PASS))
                    return Ok(new { success = false, message = "Thiếu thông tin bắt buộc" });

                var ok = _bll.ThemMoi(tk);
                if (!ok) return Ok(new { success = false, message = "Không thể thêm (có thể mã đã tồn tại)" });

                return Ok(new { success = true, message = "Thêm tài khoản thành công" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi: " + ex.Message });
            }
        }

        // 🔹 Cập nhật tài khoản
        [HttpPost("update-byID-taikhoan")]
        public IActionResult UpdateTaiKhoan([FromBody] TaiKhoan tk)
        {
            try
            {
                if (tk == null || string.IsNullOrWhiteSpace(tk.MATAIKHOAN) ||
                    string.IsNullOrWhiteSpace(tk.USERNAME) || string.IsNullOrWhiteSpace(tk.PASS))
                    return Ok(new { success = false, message = "Thiếu thông tin bắt buộc" });

                var ok = _bll.CapNhat(tk);
                if (!ok) return Ok(new { success = false, message = "Không thể cập nhật (không tìm thấy mã hoặc dữ liệu không hợp lệ)" });

                return Ok(new { success = true, message = "Cập nhật tài khoản thành công" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi: " + ex.Message });
            }
        }

        // 🔹 Xóa tài khoản
        [HttpDelete("del-byID-taikhoan")]
        public IActionResult DeleteTaiKhoan([FromQuery] string mataikhoan)
        {
            try
            {
                var ok = _bll.Xoa(mataikhoan);
                if (!ok) return Ok(new { success = false, message = "Không thể xoá (không tìm thấy mã)" });

                return Ok(new { success = true, message = "Xóa tài khoản thành công" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi: " + ex.Message });
            }
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
