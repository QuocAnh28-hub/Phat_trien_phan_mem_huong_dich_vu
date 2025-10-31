using BLL;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Models;
using System;
using System.Collections.Generic;
using System.Data;

namespace Task2_API_ThuNgan.Controllers
{
    [Authorize]
    [Route("api/QuanLyBanHang")]
    [ApiController]
    public class QuanLyBanHang_Controller : ControllerBase
    {
        private readonly HoaDonBan_BLL hdb_bll;
        private readonly ChiTietBan_BLL ctb_bll;
        private readonly KhachHang_BLL KH_BLL;
        private readonly DanhMuc_BLL dm_bll;
        private readonly SanPham_BLL sp_bll;
        private readonly ThanhToan_BLL _BLL;

        public QuanLyBanHang_Controller(IConfiguration configuration)
        {
            hdb_bll = new HoaDonBan_BLL(configuration);
            ctb_bll = new ChiTietBan_BLL(configuration);
            KH_BLL = new KhachHang_BLL(configuration);
            dm_bll = new DanhMuc_BLL(configuration);
            sp_bll = new SanPham_BLL(configuration);
            _BLL = new ThanhToan_BLL(configuration);
        }

        [Route("insert-khachhang")]
        [HttpPost]
        public IActionResult Create([FromBody] KhachHang kh)
        {
            try
            {
                if (kh == null)
                    return BadRequest(new { success = false, message = "Dữ liệu gửi lên rỗng." });

                DataTable result = KH_BLL.CreateKH(kh);

                var list = result.AsEnumerable().Select(row =>
                    result.Columns.Cast<DataColumn>().ToDictionary(
                        col => col.ColumnName,
                        col => row[col]
                    )
                ).ToList();

                return Ok(new
                {
                    success = true,
                    message = "Thêm khách hàng thành công!",
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Lỗi khi thêm khách hàng: " + ex.Message
                });
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

        [Route("get-all-hoadonban")]
        [HttpGet]
        public IActionResult GetAll_HoaDon()
        {
            try
            {
                var result = hdb_bll.LayTatCa();
                return Ok(result);
            }
            catch (Exception ex) { return StatusCode(500, new { success = false, message = "Lỗi: " + ex.Message }); }
        }

        [Route("get-hoadonban-by-id")]
        [HttpGet]
        public IActionResult GetByID_HoaDon(string maHoaDon)
        {
            try
            {
                var result = hdb_bll.LayTheoID(maHoaDon);
                if (result == null || result.Count == 0)
                    return NotFound("Không tìm thấy hóa đơn.");
                return Ok(result);
            }
            catch (Exception ex) { return StatusCode(500, new { success = false, message = "Lỗi: " + ex.Message }); }
        }

        [Route("get-all-chitietban")]
        [HttpGet]
        public IActionResult GetAll_ChiTiet()
        {
            try
            {
                var result = ctb_bll.LayTatCa();
                return Ok(result);
            }
            catch (Exception ex) { return StatusCode(500, new { success = false, message = "Lỗi: " + ex.Message }); }
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
            catch (Exception ex) { return StatusCode(500, new { success = false, message = "Lỗi: " + ex.Message }); }
        }

        [Route("get-all-khachhang")]
        [HttpGet]
        public IActionResult getAllKH()
        {
            try
            {
                DataTable dt = KH_BLL.getAllKH();
                return Ok(new { success = true, message = "Lấy danh sách khách thành công", data = dt });
            }
            catch (Exception ex) { return StatusCode(500, new { success = false, message = "Lỗi: " + ex.Message }); }
        }

        [Route("get-byid-khachhang")]
        [HttpGet]
        public IActionResult GetByIdKH(string maKH)
        {
            try
            {
                if (string.IsNullOrEmpty(maKH))
                    return BadRequest(new { success = false, message = "Thiếu mã khách hàng." });

                DataTable dt = KH_BLL.GetByIdKH(maKH);

                if (dt == null || dt.Rows.Count == 0)
                    return NotFound(new { success = false, message = "Không tìm thấy khách hàng." });

                var list = dt.AsEnumerable()
                    .Select(row => dt.Columns.Cast<DataColumn>()
                        .ToDictionary(col => col.ColumnName, col => row[col])
                    ).ToList();

                var khach = (object)(list.Count == 1 ? list[0] : list);

                return Ok(new
                {
                    success = true,
                    message = "Lấy thông tin khách hàng thành công!",
                    data = khach
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Lỗi khi lấy thông tin khách hàng: " + ex.Message
                });
            }
        }


        [Route("get-all-danhmuc")]
        [HttpGet]
        public IActionResult GetAll_DanhMuc()
        {
            try
            {
                var list = dm_bll.LayTatCa();
                if (list == null || !list.Any()) return NoContent();
                return Ok(list);
            }
            catch (Exception ex) { return StatusCode(500, new { success = false, message = "Lỗi: " + ex.Message }); }
        }

        [Route("get-byID-danhmuc")]
        [HttpGet]
        public IActionResult GetByID_DanhMuc(string madanhmuc)
        {
            try
            {
                var danhmuc = dm_bll.LayTheoID(madanhmuc);
                if (danhmuc == null || danhmuc.Count == 0)
                    return NotFound("Không tìm thấy danh mục.");
                return Ok(danhmuc);
            }
            catch (Exception ex) { return StatusCode(500, new { success = false, message = "Lỗi: " + ex.Message }); }
        }

        [HttpGet("get-all-sanpham")]
        public IActionResult GetAll_SanPham()
        {
            try
            {
                var result = sp_bll.LayTatCa();
                return Ok(result);
            }
            catch (Exception ex) { return StatusCode(500, new { success = false, message = "Lỗi: " + ex.Message }); }
        }

        [HttpGet("get-sanpham-by-id")]
        public IActionResult GetByID_SanPham(string id)
        {
            try
            {
                var result = sp_bll.LayTheoID(id);
                if (result == null || result.Count == 0)
                    return NotFound("Không tìm thấy sản phẩm.");
                return Ok(result);
            }
            catch (Exception ex) { return StatusCode(500, new { success = false, message = "Lỗi: " + ex.Message }); }
        }
    }

}
