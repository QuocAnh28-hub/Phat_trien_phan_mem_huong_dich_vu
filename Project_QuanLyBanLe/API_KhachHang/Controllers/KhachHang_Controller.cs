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
    }
}
