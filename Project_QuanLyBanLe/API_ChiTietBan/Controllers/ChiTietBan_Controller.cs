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

namespace API_ChiTietBan.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChiTietBan_Controller : ControllerBase
    {
        private readonly ChiTietBan_BLL ctb_bll;

        public ChiTietBan_Controller(IConfiguration configuration)
        {
            ctb_bll = new ChiTietBan_BLL(configuration);
        }

        [Route("get-all-chitietban")]
        [HttpGet]
        public IActionResult GetAll()
        {
            try
            {
                var result = ctb_bll.LayTatCa();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi: " + ex.Message });
            }
        }

        [Route("get-chitietban-by-IDhoadon")]
        [HttpGet]
        public IActionResult GetByHoaDon(string maHDB)
        {
            try
            {
                var result = ctb_bll.LayTheoHoaDon(maHDB);
                if (result == null || result.Count == 0)
                    return NotFound("Không tìm thấy chi tiết của hóa đơn.");

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi: " + ex.Message });
            }
        }

        [Route("insert-chitietban")]
        [HttpPost]
        public IActionResult Create(ChiTietBan model)
        {
            try
            {
                bool result = ctb_bll.ThemMoi(model);
                return result ? Ok("Thêm chi tiết thành công") : BadRequest("Thêm thất bại");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi: " + ex.Message });
            }
        }

        [Route("update-chitietban")]
        [HttpPut]
        public IActionResult Update(ChiTietBan model)
        {
            try
            {
                bool result = ctb_bll.Sua(model);
                return result ? Ok("Cập nhật chi tiết thành công") : BadRequest("Cập nhật thất bại");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi: " + ex.Message });
            }
        }

        [Route("delete-chitietban")]
        [HttpDelete]
        public IActionResult Delete(string maHDB, string maSP)
        {
            try
            {
                bool result = ctb_bll.Xoa(maHDB, maSP);
                return result ? Ok("Xóa chi tiết thành công") : BadRequest("Xóa thất bại");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi: " + ex.Message });
            }
        }
    }
}
