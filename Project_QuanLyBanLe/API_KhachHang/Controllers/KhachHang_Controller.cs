using BLL;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;
namespace API_KhachHang.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KhachHang_Controller : ControllerBase
    {
        KhachHang_BLL KH_BLL = new KhachHang_BLL();

        [Route("get-all-khachhang")]
        [HttpGet]
        public IActionResult getAllKH()
        {
            try
            {
                DataTable dt = KH_BLL.getAllKH();
                var list = new List<object>();
                foreach (DataRow row in dt.Rows)
                {
                    list.Add(new
                    {
                        MaKH = row["MaKH"],
                        TenKH = row["TenKH"],
                        SDT = row["SDT"],
                        DiaChi = row["DiaChi"]
                    });
                }
                return Ok(new { success = true, message = "Lấy danh sách khách thành công", data = list });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi: " + ex.Message });
            }
        }


        [Route("get-byid-khachhang")]
        [HttpGet]
        public IActionResult GetByIdKH(string makh)
        {
            try
            {
                DataTable dt = KH_BLL.GetByIdKH(makh);
                var list = new List<object>();
                foreach (DataRow row in dt.Rows)
                {
                    list.Add(new
                    {
                        MaKH = row["MaKH"],
                        TenKH = row["TenKH"],
                        SDT = row["SDT"],
                        DiaChi = row["DiaChi"]
                    });
                }
                return Ok(new { success = true, message = "Lấy thông tin khách thành công", data = list });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi: " + ex.Message });
            }
        }

        [Route("del-byID-khachhang")]
        [HttpDelete]
        public IActionResult DeleteKH(string makh)
        {
            try
            {
                DataTable dt = KH_BLL.GetByIdKH(makh);  
                if (dt.Rows.Count < 1)
                {
                    return Ok(new { success = false, message = "Không có thông tin khách hàng có mã này" });

                }
                else
                {
                    dt = KH_BLL.DeleteByIdKH(makh);
                    return Ok(new { success = true, message = "Xoá thông tin khách thành công" });
                }                      
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi: " + ex.Message });
            }
        }

        [Route("update-byID-khachhang")]
        [HttpPost]
        public IActionResult UpdateByIdKH(Models.KhachHang kh)
        {
            try
            {
                DataTable dt = KH_BLL.GetByIdKH(kh.MaKH);
                if (dt.Rows.Count < 1)
                {
                    return Ok(new { success = false, message = "Không có thông tin khách hàng có mã này" });

                }
                else
                {
                    dt = KH_BLL.UpdateByIdKH(kh);
                    return Ok(new { success = true, message = "Thay đổi thông tin khách thành công" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi: " + ex.Message });
            }
        }


        [Route("create-khachhang")]
        [HttpPost]
        public IActionResult CreateKH([FromBody] Models.KhachHang kh)
        {
            try
            {
                DataTable dt = KH_BLL.GetByIdKH(kh.MaKH);
                if (dt.Rows.Count == 1)
                {
                    return Ok(new { success = false, message = "Đã tồn tại khách hàng có mã này" });

                }
                else
                {
                    dt = KH_BLL.CreateKH(kh);
                    return Ok(new { success = true, message = "Thêm thông tin khách thành công" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi: " + ex.Message });
            }
        }
    }
}
