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


    }
}
