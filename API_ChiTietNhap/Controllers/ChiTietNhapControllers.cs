using BLL;
using Microsoft.AspNetCore.Mvc;
using Models;
using System.Data;

namespace API_ChiTietNhap.Controllers
{
    [Route("api/[controller]")] // => /api/ChiTietNhap_/*
    [ApiController]
    public class ChiTietNhap_Controller : ControllerBase
    {
        ChiTietNhap_BLL CTN_BLL = new ChiTietNhap_BLL();

        [HttpGet("get-all-chitietnhap")]
        public IActionResult GetAll()
        {
            try
            {
                DataTable dt = CTN_BLL.GetAllChiTietNhap();
                var list = Map(dt);
                return Ok(new { success = true, message = "Lấy danh sách chi tiết nhập thành công", data = list });
            }
            catch (Exception ex) { return StatusCode(500, new { success = false, message = "Lỗi: " + ex.Message }); }
        }

        [HttpGet("get-byphieu-chitietnhap")]
        public IActionResult GetByPhieu([FromQuery] string maphieunhap)
        {
            try
            {
                DataTable dt = CTN_BLL.GetByPhieuNhapKho(maphieunhap);
                var list = Map(dt);
                return Ok(new { success = true, message = "Lấy chi tiết theo phiếu thành công", data = list });
            }
            catch (Exception ex) { return StatusCode(500, new { success = false, message = "Lỗi: " + ex.Message }); }
        }

        [HttpGet("get-byid-chitietnhap")]
        public IActionResult GetById([FromQuery] string maphieunhap, [FromQuery] string masp)
        {
            try
            {
                DataTable dt = CTN_BLL.GetByIdChiTietNhap(maphieunhap, masp);
                var list = Map(dt);
                if (list.Count == 0)
                    return Ok(new { success = false, message = "Không tìm thấy chi tiết nhập." });

                return Ok(new { success = true, message = "Lấy chi tiết nhập thành công", data = list[0] });
            }
            catch (Exception ex) { return StatusCode(500, new { success = false, message = "Lỗi: " + ex.Message }); }
        }

        [HttpPost("create-chitietnhap")]
        public IActionResult Create([FromBody] ChiTietNhap ct)
        {
            try
            {
                DataTable existed = CTN_BLL.GetByIdChiTietNhap(ct.MAPHIEUNHAP, ct.MASP);
                if (existed.Rows.Count > 0)
                    return Ok(new { success = false, message = "Đã tồn tại dòng chi tiết này." });

                CTN_BLL.CreateChiTietNhap(ct);
                return Ok(new { success = true, message = "Thêm chi tiết nhập thành công" });
            }
            catch (Exception ex) { return StatusCode(500, new { success = false, message = "Lỗi: " + ex.Message }); }
        }

        [HttpPost("update-chitietnhap")]
        public IActionResult Update([FromBody] ChiTietNhap ct)
        {
            try
            {
                DataTable existed = CTN_BLL.GetByIdChiTietNhap(ct.MAPHIEUNHAP, ct.MASP);
                if (existed.Rows.Count == 0)
                    return Ok(new { success = false, message = "Không tìm thấy dòng cần sửa." });

                CTN_BLL.UpdateChiTietNhap(ct);
                return Ok(new { success = true, message = "Cập nhật chi tiết nhập thành công" });
            }
            catch (Exception ex) { return StatusCode(500, new { success = false, message = "Lỗi: " + ex.Message }); }
        }

        [HttpDelete("delete-chitietnhap")]
        public IActionResult Delete([FromQuery] string maphieunhap, [FromQuery] string masp)
        {
            try
            {
                DataTable existed = CTN_BLL.GetByIdChiTietNhap(maphieunhap, masp);
                if (existed.Rows.Count == 0)
                    return Ok(new { success = false, message = "Không tìm thấy dòng để xoá." });

                CTN_BLL.DeleteChiTietNhap(maphieunhap.Trim(), masp.Trim());
                return Ok(new { success = true, message = "Xoá chi tiết nhập thành công" });
            }
            catch (Exception ex) { return StatusCode(500, new { success = false, message = "Lỗi: " + ex.Message }); }
        }

        // Helper
        private static List<object> Map(DataTable dt)
        {
            var list = new List<object>();
            foreach (DataRow r in dt.Rows)
            {
                list.Add(new
                {
                    MAPHIEUNHAP = r["MAPHIEUNHAP"]?.ToString()?.Trim(),
                    MASP = r["MASP"]?.ToString()?.Trim(),
                    SOLUONG = r["SOLUONG"],
                    DONGIANHAP = r["DONGIANHAP"],
                    THANHTIEN = r["THANHTIEN"],
                    NGAYNHAPKHO = r["NGAYNHAPKHO"]
                });
            }
            return list;
        }
    }
}
