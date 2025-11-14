using BLL;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Task2_API_ThuNgan.Controllers
{
    [Authorize]
    [Route("api/QuanLyDoiTra")]
    [ApiController]
    public class QuanLyDoiTra_Controller:ControllerBase
    {
            private readonly HoaDonBan_BLL hdb_bll;
            private readonly ChiTietBan_BLL ctb_bll;
            private readonly KhachHang_BLL kh_bll;
            private readonly DanhMuc_BLL dm_bll;
            private readonly SanPham_BLL sp_bll;
            private readonly ThanhToan_BLL tt_bll;

            public QuanLyDoiTra_Controller(IConfiguration configuration)
            {
                hdb_bll = new HoaDonBan_BLL(configuration);
                ctb_bll = new ChiTietBan_BLL(configuration);
                kh_bll = new KhachHang_BLL(configuration);
                dm_bll = new DanhMuc_BLL(configuration);
                sp_bll = new SanPham_BLL(configuration);
                tt_bll = new ThanhToan_BLL(configuration);
            }
        public class ResetThanhToanRequest
        {
            public string MaHDBan { get; set; }
            public decimal SoTienMoi { get; set; } = 0;
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
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi: " + ex.Message });
            }
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

        private List<object> ChuyenThanhList(DataTable dt)
        {
            //tạo danh sách chứa đối tượng
            var list = new List<object>();
            //duyệt từng dòng trong datatable
            foreach (DataRow row in dt.Rows)
            {
                list.Add(new
                {
                    MATHANHTOAN = row["MATHANHTOAN"],
                    MAHDBan = row["MAHDBan"],
                    PhuongThuc = row["PhuongThuc"],
                    SoTienThanhToan = row["SoTienThanhToan"],
                    NGAYTHANHTOAN = row["NGAYTHANHTOAN"],
                    TrangThai = row["TrangThai"]
                });
            }
            return list;
        }

        [Route("get-all-thanhtoan")]
        [HttpGet]
        public IActionResult GetAll_ThanhToan()
        {
            try
            {
                DataTable dt = tt_bll.getAll();
                return Ok(new { success = true, message = "Lấy danh sách thanh toán thành công", data = ChuyenThanhList(dt) });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi: " + ex.Message });
            }
        }

        [Route("get-byId-thanhtoan")]
        [HttpGet]
        public IActionResult Get_ThanhToan_ById(string ma)
        {
            try
            {
                DataTable dt = tt_bll.GetById(ma);
                return Ok(new { success = true, message = "Lấy thông tin thanh toán thành công", data = ChuyenThanhList(dt) });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi: " + ex.Message });
            }
        }

        [Route("insert-thanhtoan")]
        [HttpPost]
        public IActionResult Create([FromBody] Models.ThanhToan model)
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

        [Route("update-thanhtoan")]
        [HttpPost]
        public IActionResult Update([FromBody] Models.ThanhToan model)
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

        [Route("del-thanhtoan")]
        [HttpDelete]
        public IActionResult Deletett(string ma)
        {
            try
            {
                DataTable dt = tt_bll.GetById(ma);
                if (dt.Rows.Count < 1)
                {
                    return Ok(new { success = false, message = "Không có thông tin thanh toán có mã này" });

                }
                else
                {
                    dt = tt_bll.Delete(ma);
                    return Ok(new { success = true, message = "Xoá thông tin thanh toán thành công" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi: " + ex.Message });
            }
        }

        [Route("get-thanhtoan-by-mahdban")]
        [HttpGet]
        public IActionResult GetThanhToanByHoaDon(string maHDBan)
        {
            try
            {
                // dùng BLL, BLL gọi DAL.GetByHoaDon
                DataTable dt = tt_bll.GetByHoaDon(maHDBan);
                return Ok(new
                {
                    success = true,
                    message = "Lấy danh sách thanh toán theo hóa đơn thành công",
                    data = ChuyenThanhList(dt)   // dùng lại hàm map DataTable → list<object>
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi: " + ex.Message });
            }
        }


        [Route("reset-sotienthanhtoan-by-mahdban")]
        [HttpPost]
        public IActionResult ResetSoTienThanhToanByHoaDon(
        [FromQuery] string maHDBan,
        [FromQuery] decimal soTienMoi = 0)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(maHDBan))
                    return BadRequest(new { success = false, message = "Thiếu mã hóa đơn bán" });

                // dt bây giờ là danh sách THANHTOAN đã được reset tiền
                DataTable dt = tt_bll.ResetSoTienByHoaDon(maHDBan, soTienMoi);

                return Ok(new
                {
                    success = true,
                    message = "Đã reset số tiền thanh toán theo hóa đơn.",
                    data = ChuyenThanhList(dt)   // giờ dt có đủ cột nên không còn lỗi
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi: " + ex.Message });
            }
        }

        [Route("reset-tongtienhang-by-mahdban")]
        [HttpPost]
        public IActionResult ResetTongTienHangByHoaDon(
        [FromQuery] string maHDBan,
        [FromQuery] decimal tongTienMoi = 0)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(maHDBan))
                    return BadRequest(new { success = false, message = "Thiếu mã hóa đơn bán" });

                bool ok = hdb_bll.ResetTongTienHangByHoaDon(maHDBan, tongTienMoi);

                return Ok(new
                {
                    success = ok,
                    message = ok
                        ? "Đã cập nhật TONGTIENHANG cho hóa đơn."
                        : "Không cập nhật được TONGTIENHANG (kiểm tra mã hóa đơn)."
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi: " + ex.Message });
            }
        }



    }
}
