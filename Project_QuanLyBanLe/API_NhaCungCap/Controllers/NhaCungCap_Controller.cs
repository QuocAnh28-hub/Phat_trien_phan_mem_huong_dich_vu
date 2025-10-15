using BLL;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Data;

namespace API_NhaCungCap.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NhaCungCap_Controller : ControllerBase
    {
        private readonly NhaCungCap_BLL NCC_BLL;
        public NhaCungCap_Controller(IConfiguration configuration)
        {
            NCC_BLL = new NhaCungCap_BLL(configuration);
        }

        [Route("get-all-nhacungcap")]
        [HttpGet]
        public IActionResult getAllNCC()
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


        [Route("get-byid-nhacungcap")]
        [HttpGet]
        public IActionResult GetById(string ma)
        {
            try
            {
                DataTable dt = NCC_BLL.GetById(ma);
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
                return Ok(new { success = true, message = "Lấy thông tin nhà cung cấp thành công", data = list });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi: " + ex.Message });
            }
        }

        [Route("del-nhacungcap")]
        [HttpDelete]
        public IActionResult Delete(string ma)
        {
            try
            {
                DataTable dt = NCC_BLL.GetById(ma);
                if (dt.Rows.Count < 1)
                {
                    return Ok(new { success = false, message = "Không có thông tin nhà cung cấp có mã này" });

                }
                else
                {
                    dt = NCC_BLL.Delete(ma);
                    return Ok(new { success = true, message = "Xoá thông tin nhà cung cấp thành công" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi: " + ex.Message });
            }
        }

        [Route("update-khachhang")]
        [HttpPost]
        public IActionResult Update([FromBody] Models.NhaCungCap model)
        {
            try
            {
                DataTable dt = NCC_BLL.GetById(model.MaNCC);
                if (dt.Rows.Count < 1)
                {
                    return Ok(new { success = false, message = "Không có thông tin khách hàng có mã này" });

                }
                else
                {
                    dt = NCC_BLL.Update(model);
                    return Ok(new { success = true, message = "Thay đổi thông tin khách thành công" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi: " + ex.Message });
            }
        }


        [Route("create-nhacungcap")]
        [HttpPost]
        public IActionResult Create([FromBody] Models.NhaCungCap model)
        {
            try
            {
                DataTable dt = NCC_BLL.GetById(model.MaNCC);
                if (dt.Rows.Count == 1)
                {
                    return Ok(new { success = false, message = "Đã tồn tại nhà cung cấp có mã này" });

                }
                else
                {
                    dt = NCC_BLL.Create(model);
                    return Ok(new { success = true, message = "Thêm thông tin nhà cung cấp thành công" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi: " + ex.Message });
            }
        }
    }
}
