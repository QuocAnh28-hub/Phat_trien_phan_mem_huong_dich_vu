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
        public IActionResult GetByIdKH(string makh)
        {
            try
            {
                DataTable dt = KH_BLL.GetByIdKH(makh);
                return Ok(new { success = true, message = "Lấy thông tin khách thành công", data = dt });
            }
            catch (Exception ex) { return StatusCode(500, new { success = false, message = "Lỗi: " + ex.Message }); }
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
