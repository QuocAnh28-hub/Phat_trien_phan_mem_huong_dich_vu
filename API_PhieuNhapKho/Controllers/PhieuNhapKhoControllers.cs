using BLL;
using Microsoft.AspNetCore.Mvc;
using Models;
using System.Data;

namespace API_PhieuNhapKho.Controllers
{
    [Route("api/[controller]")] // => /api/PhieuNhapKho_/*
    [ApiController]
    public class PhieuNhapKho_Controller : ControllerBase
    {
        PhieuNhapKho_BLL PNK_BLL = new PhieuNhapKho_BLL();

        [HttpGet("get-all-phieunhap")]
        public IActionResult GetAll()
        {
            try
            {
                DataTable dt = PNK_BLL.GetAllPhieuNhapKho();
                var list = new List<object>();
                foreach (DataRow r in dt.Rows)
                {
                    list.Add(new
                    {
                        MAPHIEUNHAP = r["MAPHIEUNHAP"]?.ToString()?.Trim(),
                        MASP = r["MASP"]?.ToString()?.Trim(),
                        MANCC = r["MANCC"]?.ToString()?.Trim(),
                        MANV = r["MANV"]?.ToString()?.Trim(),
                        NGAYLAP = r["NGAYLAP"],
                        THUEVAT = r["THUEVAT"]
                    });
                }
                return Ok(new { success = true, message = "Lấy danh sách phiếu nhập thành công", data = list });
            }
            catch (Exception ex) { return StatusCode(500, new { success = false, message = "Lỗi: " + ex.Message }); }
        }

        [HttpGet("get-byid-phieunhap")]
        public IActionResult GetById([FromQuery] string maphieunhap)
        {
            try
            {
                DataTable dt = PNK_BLL.GetByIdPhieuNhapKho(maphieunhap);
                if (dt.Rows.Count == 0)
                    return Ok(new { success = false, message = "Không tìm thấy phiếu nhập." });

                var r = dt.Rows[0];
                var data = new
                {
                    MAPHIEUNHAP = r["MAPHIEUNHAP"]?.ToString()?.Trim(),
                    MASP = r["MASP"]?.ToString()?.Trim(),
                    MANCC = r["MANCC"]?.ToString()?.Trim(),
                    MANV = r["MANV"]?.ToString()?.Trim(),
                    NGAYLAP = r["NGAYLAP"],
                    THUEVAT = r["THUEVAT"]
                };
                return Ok(new { success = true, message = "Lấy thông tin phiếu nhập thành công", data });
            }
            catch (Exception ex) { return StatusCode(500, new { success = false, message = "Lỗi: " + ex.Message }); }
        }

        [HttpPost("create-phieunhap")]
        public IActionResult Create([FromBody] PhieuNhapKho pn)
        {
            try
            {
                DataTable existed = PNK_BLL.GetByIdPhieuNhapKho(pn.MAPHIEUNHAP);
                if (existed.Rows.Count > 0)
                    return Ok(new { success = false, message = "Đã tồn tại phiếu nhập có mã này." });

                PNK_BLL.CreatePhieuNhapKho(pn);
                return Ok(new { success = true, message = "Thêm phiếu nhập thành công" });
            }
            catch (Exception ex) { return StatusCode(500, new { success = false, message = "Lỗi: " + ex.Message }); }
        }

        [HttpPost("update-phieunhap")]
        public IActionResult Update([FromBody] PhieuNhapKho pn)
        {
            try
            {
                DataTable existed = PNK_BLL.GetByIdPhieuNhapKho(pn.MAPHIEUNHAP);
                if (existed.Rows.Count == 0)
                    return Ok(new { success = false, message = "Không tìm thấy phiếu nhập cần sửa." });

                PNK_BLL.UpdatePhieuNhapKho(pn);
                return Ok(new { success = true, message = "Cập nhật phiếu nhập thành công" });
            }
            catch (Exception ex) { return StatusCode(500, new { success = false, message = "Lỗi: " + ex.Message }); }
        }

        [HttpDelete("delete-phieunhap")]
        public IActionResult Delete([FromQuery] string maphieunhap)
        {
            try
            {
                DataTable existed = PNK_BLL.GetByIdPhieuNhapKho(maphieunhap);
                if (existed.Rows.Count == 0)
                    return Ok(new { success = false, message = "Không tìm thấy phiếu nhập để xoá." });

                PNK_BLL.DeletePhieuNhapKho(maphieunhap.Trim());
                return Ok(new { success = true, message = "Xoá phiếu nhập thành công" });
            }
            catch (Exception ex) { return StatusCode(500, new { success = false, message = "Lỗi: " + ex.Message }); }
        }
    }
}
