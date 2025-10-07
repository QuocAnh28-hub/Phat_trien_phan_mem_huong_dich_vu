using BLL;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace API_NhaCungCap.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NhaCungCap_Controller : ControllerBase
    {
        NhaCungCap_BLL NCC_BLL = new NhaCungCap_BLL();

        [HttpGet]
        public IActionResult getAllKH()
        {
            try
            {
                DataTable dt = NCC_BLL.getAllNCC();
                var list = new List<object>();
                foreach (DataRow row in dt.Rows)
                {
                    list.Add(new
                    {
                        MANCC = row["MANCC"],
                        TENNCC = row["TENNCC"],
                        DIACHI = row["DIACHI"],
                        SDT = row["SDT"],
                        EMAIL = row["EMAIL"]
                    });
                }
                return Ok(new { success = true, message = "Lấy danh sách nhà cung cấp thành công", data = list });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi: " + ex.Message });
            }
        }
    }
}
