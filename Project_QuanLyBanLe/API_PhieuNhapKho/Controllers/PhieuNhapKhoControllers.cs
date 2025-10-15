using BLL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Models;

namespace API_PhieuNhapKho.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PhieuNhapKhoController : ControllerBase
    {
        private readonly PhieuNhapKho_BLL _bll;
        public PhieuNhapKhoController(IConfiguration configuration)
        {
            _bll = new PhieuNhapKho_BLL(configuration);
        }

        [HttpGet("get-all-phieunhapkho")]
        public IActionResult GetAllPhieuNhapKho()
        {
            try
            {
                var data = _bll.LayTatCa();
                return Ok(new { success = true, message = "Lấy danh sách phiếu nhập kho thành công", data });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi: " + ex.Message });
            }
        }

        [HttpGet("get-byid-phieunhapkho")]
        public IActionResult GetByIdPhieuNhapKho([FromQuery] string maphieunhap)
        {
            try
            {
                var list = _bll.LayTheoID(maphieunhap);
                if (list == null || list.Count == 0)
                    return Ok(new { success = false, message = "Không tìm thấy phiếu nhập kho" });

                return Ok(new { success = true, message = "Lấy thông tin phiếu nhập kho thành công", data = list });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi: " + ex.Message });
            }
        }

        [HttpPost("create-phieunhapkho")]
        public IActionResult CreatePhieuNhapKho([FromBody] PhieuNhapKho pnk)
        {
            try
            {
                if (pnk == null || string.IsNullOrWhiteSpace(pnk.MAPHIEUNHAP))
                    return Ok(new { success = false, message = "Thiếu thông tin bắt buộc" });

                var ok = _bll.ThemMoi(pnk);
                if (!ok) return Ok(new { success = false, message = "Không thể thêm (mã đã tồn tại hoặc lỗi dữ liệu)" });

                return Ok(new { success = true, message = "Thêm phiếu nhập kho thành công" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi: " + ex.Message });
            }
        }

        [HttpPost("update-phieunhapkho")]
        public IActionResult UpdatePhieuNhapKho([FromBody] PhieuNhapKho pnk)
        {
            try
            {
                if (pnk == null || string.IsNullOrWhiteSpace(pnk.MAPHIEUNHAP))
                    return Ok(new { success = false, message = "Thiếu mã phiếu nhập" });

                var ok = _bll.CapNhat(pnk);
                if (!ok) return Ok(new { success = false, message = "Không thể cập nhật (mã không tồn tại hoặc lỗi dữ liệu)" });

                return Ok(new { success = true, message = "Cập nhật phiếu nhập kho thành công" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi: " + ex.Message });
            }
        }

        [HttpDelete("delete-phieunhapkho")]
        public IActionResult DeletePhieuNhapKho([FromQuery] string maphieunhap)
        {
            try
            {
                var ok = _bll.Xoa(maphieunhap);
                if (!ok) return Ok(new { success = false, message = "Không thể xoá (mã không tồn tại)" });

                return Ok(new { success = true, message = "Xoá phiếu nhập kho thành công" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi: " + ex.Message });
            }
        }
    }
}
