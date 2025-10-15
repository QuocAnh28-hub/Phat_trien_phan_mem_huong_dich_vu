using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using BLL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Task2_API_Admin.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuanLyKhuyenMai_Controller : ControllerBase
    {
        private readonly KhuyenMai_BLL _bll;

        public QuanLyKhuyenMai_Controller(IConfiguration configuration)
        {
            _bll = new KhuyenMai_BLL(configuration);
        }


        [HttpGet("get-all-khuyenmai")]
        public IActionResult GetAllKhuyenMai()
        {
            try
            {
                DataTable dt = _bll.getAll();
                var data = ToList(dt);
                return Ok(new { success = true, message = "Lấy danh sách khuyến mãi thành công", data });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi: " + ex.Message });
            }
        }


        [HttpGet("get-byid-khuyenmai")]
        public IActionResult GetByIdKhuyenMai([FromQuery] string ma)
        {
            try
            {
                DataTable dt = _bll.GetById(ma);
                var data = ToList(dt);
                if (data.Count == 0)
                    return Ok(new { success = false, message = "Không tìm thấy khuyến mãi." });

                return Ok(new { success = true, message = "Lấy thông tin khuyến mãi thành công", data });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi: " + ex.Message });
            }
        }


        [HttpDelete("del-khuyenmai")]
        public IActionResult DeleteKhuyenMai([FromQuery] string ma)
        {
            try
            {
                DataTable dt = _bll.GetById(ma);
                if (dt.Rows.Count < 1)
                    return Ok(new { success = false, message = "Không có thông tin khuyến mãi có mã này" });

                _bll.Delete(ma);
                return Ok(new { success = true, message = "Xoá khuyến mãi thành công" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi: " + ex.Message });
            }
        }


        [HttpPost("update-khuyenmai")]
        public IActionResult UpdateKhuyenMai([FromBody] Models.KhuyenMai model)
        {
            try
            {
                DataTable dt = _bll.GetById(model.MaKM);
                if (dt.Rows.Count < 1)
                    return Ok(new { success = false, message = "Không có thông tin khuyến mãi có mã này" });

                _bll.Update(model);
                return Ok(new { success = true, message = "Cập nhật khuyến mãi thành công" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi: " + ex.Message });
            }
        }


        [HttpPost("create-khuyenmai")]
        public IActionResult CreateKhuyenMai([FromBody] Models.KhuyenMai model)
        {
            try
            {
                DataTable dt = _bll.GetById(model.MaKM);
                if (dt.Rows.Count >= 1)
                    return Ok(new { success = false, message = "Đã tồn tại khuyến mãi có mã này" });

                _bll.Create(model);
                return Ok(new { success = true, message = "Thêm khuyến mãi thành công" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi: " + ex.Message });
            }
        }


        private static List<object> ToList(DataTable dt)
        {
            var list = new List<object>();
            foreach (DataRow row in dt.Rows)
            {
                list.Add(new
                {
                    MAKM = row["MAKM"]?.ToString()?.Trim(),
                    TENKM = row["TENKM"]?.ToString()?.Trim(),
                    MASP = row["MASP"]?.ToString()?.Trim(),
                    NGAYBATDAU = row["NGAYBATDAU"],
                    NGAYKETTHUC = row["NGAYKETTHUC"]
                });
            }
            return list;
        }
    }
}
