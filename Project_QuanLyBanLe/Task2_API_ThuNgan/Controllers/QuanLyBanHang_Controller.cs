using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

using BLL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Models;

namespace Task2_API_ThuNgan.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuanLyBanHang_Controller : ControllerBase
    {
        private readonly HoaDonBan_BLL hdb_bll;
        private readonly ChiTietBan_BLL ctb_bll;
        private readonly KhachHang_BLL kh_bll;
        private readonly DanhMuc_BLL dm_bll;
        private readonly SanPham_BLL sp_bll;
        private readonly ThanhToan_BLL tt_bll;

        public QuanLyBanHang_Controller(IConfiguration configuration)
        {
            hdb_bll = new HoaDonBan_BLL(configuration);
            ctb_bll = new ChiTietBan_BLL(configuration);
            kh_bll = new KhachHang_BLL(configuration);
            dm_bll = new DanhMuc_BLL(configuration);
            sp_bll = new SanPham_BLL(configuration);
            tt_bll = new ThanhToan_BLL(configuration);
        }

        // ===== HÓA ĐƠN BÁN =====
        [HttpGet("get-all-hoadonban")]
        public IActionResult GetAllHoaDonBan()
        {
            try
            {
                var data = hdb_bll.LayTatCa();
                return Ok(new { success = true, message = "Lấy danh sách hóa đơn thành công", data });
            }
            catch (Exception ex) { return StatusCode(500, new { success = false, message = "Lỗi: " + ex.Message }); }
        }

        [HttpGet("get-hoadonban-by-id")]
        public IActionResult GetHoaDonBanById([FromQuery] string maHoaDon)
        {
            try
            {
                var data = hdb_bll.LayTheoID(maHoaDon);
                if (data == null || data.Count == 0)
                    return Ok(new { success = false, message = "Không tìm thấy hóa đơn." });

                return Ok(new { success = true, message = "Lấy hóa đơn thành công", data });
            }
            catch (Exception ex) { return StatusCode(500, new { success = false, message = "Lỗi: " + ex.Message }); }
        }

        [HttpPost("insert-hoadonban")]
        public IActionResult CreateHoaDonBan([FromBody] HoaDonBan model)
        {
            try
            {
                var ok = hdb_bll.ThemMoi(model);
                return ok ? Ok(new { success = true, message = "Thêm hóa đơn thành công" })
                          : Ok(new { success = false, message = "Thêm hóa đơn thất bại" });
            }
            catch (Exception ex) { return StatusCode(500, new { success = false, message = "Lỗi: " + ex.Message }); }
        }

        [HttpPut("update-hoadonban")]
        public IActionResult UpdateHoaDonBan([FromBody] HoaDonBan model)
        {
            try
            {
                var ok = hdb_bll.Sua(model);
                return ok ? Ok(new { success = true, message = "Cập nhật hóa đơn thành công" })
                          : Ok(new { success = false, message = "Cập nhật hóa đơn thất bại" });
            }
            catch (Exception ex) { return StatusCode(500, new { success = false, message = "Lỗi: " + ex.Message }); }
        }

        [HttpDelete("delete-hoadonban")]
        public IActionResult DeleteHoaDonBan([FromQuery] string maHoaDon)
        {
            try
            {
                var ok = hdb_bll.Xoa(maHoaDon);
                return ok ? Ok(new { success = true, message = "Xóa hóa đơn thành công" })
                          : Ok(new { success = false, message = "Xóa hóa đơn thất bại" });
            }
            catch (Exception ex) { return StatusCode(500, new { success = false, message = "Lỗi: " + ex.Message }); }
        }

        // ===== CHI TIẾT BÁN =====
        [HttpGet("get-all-chitietban")]
        public IActionResult GetAllChiTietBan()
        {
            try
            {
                var data = ctb_bll.LayTatCa();
                return Ok(new { success = true, message = "Lấy danh sách chi tiết bán thành công", data });
            }
            catch (Exception ex) { return StatusCode(500, new { success = false, message = "Lỗi: " + ex.Message }); }
        }

        [HttpGet("get-chitietban-by-idhoadon")]
        public IActionResult GetChiTietBanByHoaDon([FromQuery] string maHDB)
        {
            try
            {
                var data = ctb_bll.LayTheoHoaDon(maHDB);
                if (data == null || data.Count == 0)
                    return Ok(new { success = false, message = "Không tìm thấy chi tiết của hóa đơn." });

                return Ok(new { success = true, message = "Lấy chi tiết thành công", data });
            }
            catch (Exception ex) { return StatusCode(500, new { success = false, message = "Lỗi: " + ex.Message }); }
        }

        [HttpPost("insert-chitietban")]
        public IActionResult CreateChiTietBan([FromBody] ChiTietBan model)
        {
            try
            {
                var ok = ctb_bll.ThemMoi(model);
                return ok ? Ok(new { success = true, message = "Thêm chi tiết thành công" })
                          : Ok(new { success = false, message = "Thêm chi tiết thất bại" });
            }
            catch (Exception ex) { return StatusCode(500, new { success = false, message = "Lỗi: " + ex.Message }); }
        }

        [HttpPut("update-chitietban")]
        public IActionResult UpdateChiTietBan([FromBody] ChiTietBan model)
        {
            try
            {
                var ok = ctb_bll.Sua(model);
                return ok ? Ok(new { success = true, message = "Cập nhật chi tiết thành công" })
                          : Ok(new { success = false, message = "Cập nhật chi tiết thất bại" });
            }
            catch (Exception ex) { return StatusCode(500, new { success = false, message = "Lỗi: " + ex.Message }); }
        }

        [HttpDelete("delete-chitietban")]
        public IActionResult DeleteChiTietBan([FromQuery] string maHDB, [FromQuery] string maSP)
        {
            try
            {
                var ok = ctb_bll.Xoa(maHDB, maSP);
                return ok ? Ok(new { success = true, message = "Xóa chi tiết thành công" })
                          : Ok(new { success = false, message = "Xóa chi tiết thất bại" });
            }
            catch (Exception ex) { return StatusCode(500, new { success = false, message = "Lỗi: " + ex.Message }); }
        }

        // ===== KHÁCH HÀNG (BLL cũ trả DataTable) =====
        [HttpGet("get-all-khachhang")]
        public IActionResult GetAllKhachHang()
        {
            try
            {
                DataTable dt = kh_bll.getAllKH();
                var data = new List<object>();
                foreach (DataRow row in dt.Rows)
                {
                    data.Add(new
                    {
                        MaKH = row["MaKH"]?.ToString()?.Trim(),
                        TenKH = row["TenKH"]?.ToString()?.Trim(),
                        SDT = row["SDT"]?.ToString()?.Trim(),
                        DiaChi = row["DiaChi"]?.ToString()?.Trim()
                    });
                }
                return Ok(new { success = true, message = "Lấy danh sách khách thành công", data });
            }
            catch (Exception ex) { return StatusCode(500, new { success = false, message = "Lỗi: " + ex.Message }); }
        }

        [HttpGet("get-byid-khachhang")]
        public IActionResult GetKhachHangById([FromQuery] string makh)
        {
            try
            {
                DataTable dt = kh_bll.GetByIdKH(makh);
                var data = new List<object>();
                foreach (DataRow row in dt.Rows)
                {
                    data.Add(new
                    {
                        MaKH = row["MaKH"]?.ToString()?.Trim(),
                        TenKH = row["TenKH"]?.ToString()?.Trim(),
                        SDT = row["SDT"]?.ToString()?.Trim(),
                        DiaChi = row["DiaChi"]?.ToString()?.Trim()
                    });
                }
                if (data.Count == 0)
                    return Ok(new { success = false, message = "Không tìm thấy khách hàng." });

                return Ok(new { success = true, message = "Lấy thông tin khách thành công", data });
            }
            catch (Exception ex) { return StatusCode(500, new { success = false, message = "Lỗi: " + ex.Message }); }
        }

        // ===== DANH MỤC =====
        [HttpGet("get-all-danhmuc")]
        public IActionResult GetAllDanhMuc()
        {
            try
            {
                var data = dm_bll.LayTatCa();
                if (data == null || !data.Any()) return NoContent();
                return Ok(new { success = true, message = "Lấy danh sách danh mục thành công", data });
            }
            catch (Exception ex) { return StatusCode(500, new { success = false, message = "Lỗi: " + ex.Message }); }
        }

        [HttpGet("get-byid-danhmuc")]
        public IActionResult GetDanhMucById([FromQuery] string madanhmuc)
        {
            try
            {
                var data = dm_bll.LayTheoID(madanhmuc);
                if (data == null || data.Count == 0)
                    return Ok(new { success = false, message = "Không tìm thấy danh mục." });

                return Ok(new { success = true, message = "Lấy danh mục thành công", data });
            }
            catch (Exception ex) { return StatusCode(500, new { success = false, message = "Lỗi: " + ex.Message }); }
        }

        // ===== SẢN PHẨM =====
        [HttpGet("get-all-sanpham")]
        public IActionResult GetAllSanPham()
        {
            try
            {
                var data = sp_bll.LayTatCa();
                return Ok(new { success = true, message = "Lấy danh sách sản phẩm thành công", data });
            }
            catch (Exception ex) { return StatusCode(500, new { success = false, message = "Lỗi: " + ex.Message }); }
        }

        [HttpGet("get-sanpham-by-id")]
        public IActionResult GetSanPhamById([FromQuery] string id)
        {
            try
            {
                var data = sp_bll.LayTheoID(id);
                if (data == null || data.Count == 0)
                    return Ok(new { success = false, message = "Không tìm thấy sản phẩm." });

                return Ok(new { success = true, message = "Lấy sản phẩm thành công", data });
            }
            catch (Exception ex) { return StatusCode(500, new { success = false, message = "Lỗi: " + ex.Message }); }
        }

        // ===== THANH TOÁN (BLL cũ trả DataTable) =====
        [HttpGet("get-all-thanhtoan")]
        public IActionResult GetAllThanhToan()
        {
            try
            {
                DataTable dt = tt_bll.getAll();
                return Ok(new { success = true, message = "Lấy danh sách thanh toán thành công", data = ChuyenThanhList(dt) });
            }
            catch (Exception ex) { return StatusCode(500, new { success = false, message = "Lỗi: " + ex.Message }); }
        }

        [HttpGet("get-byid-thanhtoan")]
        public IActionResult GetThanhToanById([FromQuery] string ma)
        {
            try
            {
                DataTable dt = tt_bll.GetById(ma);
                return Ok(new { success = true, message = "Lấy thông tin thanh toán thành công", data = ChuyenThanhList(dt) });
            }
            catch (Exception ex) { return StatusCode(500, new { success = false, message = "Lỗi: " + ex.Message }); }
        }

        [HttpPost("insert-thanhtoan")]
        public IActionResult CreateThanhToan([FromBody] Models.ThanhToan model)
        {
            try
            {
                DataTable dt = tt_bll.GetById(model.MaThanhToan);
                if (dt.Rows.Count == 1)
                    return Ok(new { success = false, message = "Đã tồn tại thanh toán có mã này" });

                tt_bll.Create(model);
                return Ok(new { success = true, message = "Thêm thông tin thanh toán thành công" });
            }
            catch (Exception ex) { return StatusCode(500, new { success = false, message = "Lỗi: " + ex.Message }); }
        }

        [HttpPost("update-thanhtoan")]
        public IActionResult UpdateThanhToan([FromBody] Models.ThanhToan model)
        {
            try
            {
                DataTable dt = tt_bll.GetById(model.MaThanhToan);
                if (dt.Rows.Count < 1)
                    return Ok(new { success = false, message = "Không tồn tại thanh toán có mã này" });

                tt_bll.Update(model);
                return Ok(new { success = true, message = "Thay đổi thông tin thanh toán thành công" });
            }
            catch (Exception ex) { return StatusCode(500, new { success = false, message = "Lỗi: " + ex.Message }); }
        }

        [HttpDelete("del-thanhtoan")]
        public IActionResult DeleteThanhToan([FromQuery] string ma)
        {
            try
            {
                DataTable dt = tt_bll.GetById(ma);
                if (dt.Rows.Count < 1)
                    return Ok(new { success = false, message = "Không có thông tin thanh toán có mã này" });

                tt_bll.Delete(ma);
                return Ok(new { success = true, message = "Xoá thông tin thanh toán thành công" });
            }
            catch (Exception ex) { return StatusCode(500, new { success = false, message = "Lỗi: " + ex.Message }); }
        }

        // Helper chuyển DataTable => List<object> (giữ nguyên như bạn đang dùng)
        private static List<object> ChuyenThanhList(DataTable dt)
        {
            var list = new List<object>();
            foreach (DataRow r in dt.Rows)
            {
                list.Add(new
                {
                    MaThanhToan = r["MaThanhToan"]?.ToString()?.Trim(),
                    MaHDBan = r["MaHDBan"]?.ToString()?.Trim(),
                    PhuongThuc = r["PhuongThuc"]?.ToString()?.Trim(),
                    SoTienThanhToan = r["SoTienThanhToan"],
                    NgayThanhToan = r["NgayThanhToan"],
                    TrangThai = r["TrangThai"]?.ToString()?.Trim()
                });
            }
            return list;
        }
    }
}
