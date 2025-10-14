using BLL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Models;
using System.Linq;

namespace API_ChiTietNhap.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChiTietNhapController : ControllerBase
    {
        private readonly ChiTietNhap_BLL _bll;

        public ChiTietNhapController(IConfiguration configuration)
        {
            _bll = new ChiTietNhap_BLL(configuration);
        }

        // 🔹 Lấy tất cả
        [HttpGet("get-all-chitietnhap")]
        public IActionResult GetAll()
        {
            try
            {
                var data = _bll.LayTatCa()
                               .Select(x => new {
                                   MAPHIEUNHAP = x.MAPHIEUNHAP?.Trim(),
                                   MASP = x.MASP?.Trim(),
                                   SOLUONG = x.SOLUONG,
                                   DONGIANHAP = x.DONGIANHAP,
                                   THANHTIEN = x.THANHTIEN,
                                   NGAYNHAPKHO = x.NGAYNHAPKHO
                               })
                               .ToList();

                return Ok(new { success = true, message = "Lấy danh sách chi tiết nhập thành công", data });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi: " + ex.Message });
            }
        }

        // 🔹 Lấy theo phiếu
        [HttpGet("get-byphieu-chitietnhap")]
        public IActionResult GetByPhieu([FromQuery] string maphieunhap)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(maphieunhap))
                    return Ok(new { success = false, message = "Thiếu mã phiếu nhập" });

                var list = _bll.LayTheoPhieu(maphieunhap);
                var data = list.Select(x => new {
                    MAPHIEUNHAP = x.MAPHIEUNHAP?.Trim(),
                    MASP = x.MASP?.Trim(),
                    SOLUONG = x.SOLUONG,
                    DONGIANHAP = x.DONGIANHAP,
                    THANHTIEN = x.THANHTIEN,
                    NGAYNHAPKHO = x.NGAYNHAPKHO
                })
                               .ToList();

                return Ok(new { success = true, message = "Lấy chi tiết theo phiếu thành công", data });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi: " + ex.Message });
            }
        }

        // 🔹 Lấy theo (MAPHIEUNHAP, MASP)
        [HttpGet("get-byid-chitietnhap")]
        public IActionResult GetById([FromQuery] string maphieunhap, [FromQuery] string masp)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(maphieunhap) || string.IsNullOrWhiteSpace(masp))
                    return Ok(new { success = false, message = "Thiếu mã phiếu nhập hoặc mã sản phẩm" });

                var list = _bll.LayTheoID(maphieunhap, masp);
                if (list == null || list.Count == 0)
                    return Ok(new { success = false, message = "Không tìm thấy chi tiết nhập" });

                var x = list.First();
                var data = new
                {
                    MAPHIEUNHAP = x.MAPHIEUNHAP?.Trim(),
                    MASP = x.MASP?.Trim(),
                    SOLUONG = x.SOLUONG,
                    DONGIANHAP = x.DONGIANHAP,
                    THANHTIEN = x.THANHTIEN,
                    NGAYNHAPKHO = x.NGAYNHAPKHO
                };

                return Ok(new { success = true, message = "Lấy chi tiết nhập thành công", data });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi: " + ex.Message });
            }
        }

        // 🔹 Tạo mới
        [HttpPost("create-chitietnhap")]
        public IActionResult Create([FromBody] ChiTietNhap ct)
        {
            try
            {
                if (ct == null ||
                    string.IsNullOrWhiteSpace(ct.MAPHIEUNHAP) ||
                    string.IsNullOrWhiteSpace(ct.MASP) ||
                    ct.SOLUONG <= 0 || ct.DONGIANHAP < 0)
                    return Ok(new { success = false, message = "Thiếu/không hợp lệ thông tin bắt buộc" });

                var ok = _bll.ThemMoi(ct);
                if (!ok) return Ok(new { success = false, message = "Không thể thêm (dòng đã tồn tại hoặc dữ liệu không hợp lệ)" });

                return Ok(new { success = true, message = "Thêm chi tiết nhập thành công" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi: " + ex.Message });
            }
        }

        // 🔹 Cập nhật
        [HttpPost("update-chitietnhap")]
        public IActionResult Update([FromBody] ChiTietNhap ct)
        {
            try
            {
                if (ct == null ||
                    string.IsNullOrWhiteSpace(ct.MAPHIEUNHAP) ||
                    string.IsNullOrWhiteSpace(ct.MASP) ||
                    ct.SOLUONG <= 0 || ct.DONGIANHAP < 0)
                    return Ok(new { success = false, message = "Thiếu/không hợp lệ thông tin bắt buộc" });

                var ok = _bll.CapNhat(ct);
                if (!ok) return Ok(new { success = false, message = "Không thể cập nhật (không tìm thấy dòng hoặc dữ liệu không hợp lệ)" });

                return Ok(new { success = true, message = "Cập nhật chi tiết nhập thành công" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi: " + ex.Message });
            }
        }

        // 🔹 Xoá
        [HttpDelete("delete-chitietnhap")]
        public IActionResult Delete([FromQuery] string maphieunhap, [FromQuery] string masp)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(maphieunhap) || string.IsNullOrWhiteSpace(masp))
                    return Ok(new { success = false, message = "Thiếu mã phiếu nhập hoặc mã sản phẩm" });

                var ok = _bll.Xoa(maphieunhap, masp);
                if (!ok) return Ok(new { success = false, message = "Không thể xoá (không tìm thấy dòng)" });

                return Ok(new { success = true, message = "Xoá chi tiết nhập thành công" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi: " + ex.Message });
            }
        }
    }
}
