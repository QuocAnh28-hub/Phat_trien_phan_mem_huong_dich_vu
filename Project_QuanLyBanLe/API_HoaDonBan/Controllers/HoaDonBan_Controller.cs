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

namespace API_HoaDonBan.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HoaDonBan_Controller : ControllerBase
    {
        private readonly HoaDonBan_BLL hdb_bll;

        public HoaDonBan_Controller(IConfiguration configuration)
        {
            hdb_bll = new HoaDonBan_BLL(configuration);
        }

        [Route("get-all-hoadonban")]
        [HttpGet]
        public IActionResult GetAll()
        {
            try
            {
                var result = hdb_bll.LayTatCa();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi: " + ex.Message });
            }
        }

        [Route("get-hoadonban-by-id")]
        [HttpGet]
        public IActionResult GetByID(string maHoaDon)
        {
            try
            {
                var result = hdb_bll.LayTheoID(maHoaDon);
                if (result == null || result.Count == 0)
                    return NotFound("Không tìm thấy hóa đơn.");

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi: " + ex.Message });
            }
        }

        [Route("insert-hoadonban")]
        [HttpPost]
        public IActionResult Create([FromBody] HoaDonBan model)
        {
            try
            {
                bool result = hdb_bll.ThemMoi(model);
                return result ? Ok("Thêm hóa đơn thành công") : BadRequest("Thêm hóa đơn thất bại");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi: " + ex.Message });
            }
        }

        [Route("update-hoadonban")]
        [HttpPut]
        public IActionResult Update([FromBody] HoaDonBan model)
        {
            try
            {
                bool result = hdb_bll.Sua(model);
                return result ? Ok("Cập nhật hóa đơn thành công") : BadRequest("Cập nhật hóa đơn thất bại");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi: " + ex.Message });
            }
        }

        [Route("delete-hoadonban")]
        [HttpDelete]
        public IActionResult Delete(string maHoaDon)
        {
            try
            {
                bool result = hdb_bll.Xoa(maHoaDon);
                return result ? Ok("Xóa hóa đơn thành công") : BadRequest("Xóa hóa đơn thất bại");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi: " + ex.Message });
            }
        }
    }
}
