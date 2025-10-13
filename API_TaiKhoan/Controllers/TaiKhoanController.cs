using BLL;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace API_TaiKhoan.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaiKhoan_Controller : ControllerBase
    {
        TaiKhoan_BLL TK_BLL = new TaiKhoan_BLL();

        // 🔹 Lấy tất cả tài khoản
        [Route("get-all-taikhoan")]
        [HttpGet]
        public IActionResult GetAllTaiKhoan()
        {
            try
            {
                DataTable dt = TK_BLL.GetAllTaiKhoan();
                var list = new List<object>();
                foreach (DataRow row in dt.Rows)
                {
                    list.Add(new
                    {
                        MaTaiKhoan = row["MaTaiKhoan"],
                        UserName = row["UserName"],
                        Password = row["Pass"],
                        Quyen = row["Quyen"]
                    });
                }
                return Ok(new { success = true, message = "Lấy danh sách tài khoản thành công", data = list });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi: " + ex.Message });
            }
        }

        // 🔹 Lấy tài khoản theo ID
        [Route("get-byid-taikhoan")]
        [HttpGet]
        public IActionResult GetByIdTaiKhoan(string mataikhoan)
        {
            try
            {
                DataTable dt = TK_BLL.GetByIdTaiKhoan(mataikhoan);
                var list = new List<object>();
                foreach (DataRow row in dt.Rows)
                {
                    list.Add(new
                    {
                        MaTaiKhoan = row["MaTaiKhoan"],
                        UserName = row["UserName"],
                        Password = row["Pass"],
                        Quyen = row["Quyen"]
                    });
                }
                return Ok(new { success = true, message = "Lấy thông tin tài khoản thành công", data = list });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi: " + ex.Message });
            }
        }

        // 🔹 Thêm tài khoản
        [Route("create-taikhoan")]
        [HttpPost]
        public IActionResult CreateTaiKhoan([FromBody] Models.TaiKhoan tk)
        {
            try
            {
                DataTable dt = TK_BLL.GetByIdTaiKhoan(tk.MATAIKHOAN);
                if (dt.Rows.Count == 1)
                {
                    return Ok(new { success = false, message = "Đã tồn tại tài khoản có mã này" });
                }
                else
                {
                    TK_BLL.CreateTaiKhoan(tk);
                    return Ok(new { success = true, message = "Thêm tài khoản thành công" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi: " + ex.Message });
            }
        }

        // 🔹 Cập nhật tài khoản
        [Route("update-byID-taikhoan")]
        [HttpPost]
        public IActionResult UpdateTaiKhoan([FromBody] Models.TaiKhoan tk)
        {
            try
            {
                DataTable dt = TK_BLL.GetByIdTaiKhoan(tk.MATAIKHOAN);
                if (dt.Rows.Count < 1)
                {
                    return Ok(new { success = false, message = "Không có tài khoản có mã này" });
                }
                else
                {
                    TK_BLL.UpdateTaiKhoan(tk);
                    return Ok(new { success = true, message = "Cập nhật tài khoản thành công" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi: " + ex.Message });
            }
        }

        // 🔹 Xóa tài khoản
        [Route("del-byID-taikhoan")]
        [HttpDelete]
        public IActionResult DeleteTaiKhoan(string mataikhoan)
        {
            try
            {
                DataTable dt = TK_BLL.GetByIdTaiKhoan(mataikhoan);
                if (dt.Rows.Count < 1)
                {
                    return Ok(new { success = false, message = "Không có tài khoản có mã này" });
                }
                else
                {
                    TK_BLL.DeleteTaiKhoan(mataikhoan);
                    return Ok(new { success = true, message = "Xóa tài khoản thành công" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi: " + ex.Message });
            }
        }

        // 🔹 Đăng nhập tài khoản
        [Route("login")]
        [HttpPost]
        public IActionResult Login([FromBody] Models.TaiKhoan tk)
        {
            try
            {
                DataTable dt = TK_BLL.Login(tk.USERNAME, tk.PASS);
                if (dt.Rows.Count < 1)
                {
                    return Ok(new { success = false, message = "Sai tên đăng nhập hoặc mật khẩu" });
                }
                else
                {
                    var list = new List<object>();
                    foreach (DataRow row in dt.Rows)
                    {
                        list.Add(new
                        {
                            MaTaiKhoan = row["MaTaiKhoan"],
                            UserName = row["UserName"],
                            Quyen = row["Quyen"]
                        });
                    }
                    return Ok(new { success = true, message = "Đăng nhập thành công", data = list });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi: " + ex.Message });
            }
        }
    }
}
