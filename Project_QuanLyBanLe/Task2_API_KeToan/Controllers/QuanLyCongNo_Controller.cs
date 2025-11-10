using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using BLL;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Models;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Task2_API_KeToan.Controllers
{
    [Authorize]
    [Route("api/QuanLyCongNo")]
    [ApiController]
    public class QuanLyCongNo_Controller : ControllerBase
    {
        private readonly NhaCungCap_BLL NCC_BLL;
        private readonly KhachHang_BLL KH_BLL;
        private readonly ThanhToan_BLL TT_BLL;
        private readonly HoaDonBan_BLL hdb_bll;

        public QuanLyCongNo_Controller(IConfiguration configuration)
        {
            NCC_BLL = new NhaCungCap_BLL(configuration);
            KH_BLL = new KhachHang_BLL(configuration);
            TT_BLL = new ThanhToan_BLL(configuration);
            hdb_bll = new HoaDonBan_BLL(configuration);
        }

        [HttpGet("search-khachhang-chuathanhtoan")]
        public IActionResult SearchKhachHangChuaThanhToan(string tenKh)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(tenKh))
                    return BadRequest(new { success = false, message = "Thiếu tên khách hàng." });

                DataTable dt = TT_BLL.GetHoaDonChuaThanhToanTheoTen(tenKh);

                var list = new List<Dictionary<string, object>>();
                foreach (DataRow row in dt.Rows)
                {
                    var dict = new Dictionary<string, object>();
                    foreach (DataColumn col in dt.Columns)
                    {
                        dict[col.ColumnName] = row[col];
                    }
                    list.Add(dict);
                }

                return Ok(new
                {
                    success = true,
                    message = "Lấy danh sách hóa đơn chưa thanh toán theo tên khách hàng thành công!",
                    data = list
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Lỗi: " + ex.Message
                });
            }
        }

        [Route("update-trangthai-thanhtoan")]
        [HttpPut]
        public IActionResult UpdateTrangThaiThanhToan([FromQuery] string maHDBan, [FromQuery] string phuongThuc)
        {
            try
            {
                if (string.IsNullOrEmpty(maHDBan))
                    return BadRequest(new { success = false, message = "Mã hóa đơn không được để trống!" });

                if (string.IsNullOrEmpty(phuongThuc))
                    return BadRequest(new { success = false, message = "Phương thức thanh toán không được để trống!" });

                bool result = TT_BLL.UpdateTrangThaiThanhToan(maHDBan, phuongThuc);

                if (result)
                {
                    return Ok(new
                    {
                        success = true,
                        message = "Cập nhật trạng thái, phương thức, số tiền và ngày thanh toán thành công!"
                    });
                }
                else
                {
                    return Ok(new
                    {
                        success = false,
                        message = "⚠Không tìm thấy hóa đơn để cập nhật!"
                    });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Lỗi: " + ex.Message
                });
            }
        }

        [Route("get-hoadon-chuathanhtoan")]
        [HttpGet]
        public IActionResult GetHoaDonChuaThanhToan()
        {
            try
            {
                DataTable dt = TT_BLL.GetHoaDonChuaThanhToan();

                var list = new List<Dictionary<string, object>>();
                foreach (DataRow row in dt.Rows)
                {
                    var dict = new Dictionary<string, object>();
                    foreach (DataColumn col in dt.Columns)
                    {
                        dict[col.ColumnName] = row[col];
                    }
                    list.Add(dict);
                }

                return Ok(new
                {
                    success = true,
                    message = "Lấy danh sách hóa đơn chưa thanh toán thành công!",
                    data = list
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Lỗi: " + ex.Message
                });
            }
        }

        [Route("get-all-hoadonban")]
        [HttpGet]
        public IActionResult GetAll_HDB()
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
        public IActionResult Get_HDB_ByID(string maHoaDon)
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
                DataTable dt = TT_BLL.getAll();
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
                DataTable dt = TT_BLL.GetById(ma);
                return Ok(new { success = true, message = "Lấy thông tin thanh toán thành công", data = ChuyenThanhList(dt) });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi: " + ex.Message });
            }
        }

        [Route("update-thanhtoan")]
        [HttpPost]
        public IActionResult Update([FromBody] Models.ThanhToan model)
        {
            try
            {
                DataTable dt = TT_BLL.GetById(model.MaThanhToan);
                if (dt.Rows.Count < 1)
                {
                    return Ok(new { success = false, message = "Không tồn tại thanh toán có mã này" });

                }
                else
                {
                    dt = TT_BLL.Update(model);
                    return Ok(new { success = true, message = "Thay đổi thông tin thanh toán thành công" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi: " + ex.Message });
            }
        }

        [Route("get-all-nhacungcap")]
        [HttpGet]
        public IActionResult getAllNCC()
        {
            try
            {
                DataTable dt = NCC_BLL.getAllNCC();
                var list = new List<object>();
                foreach (DataRow row in dt.Rows)
                {
                    list.Add(new
                    {
                        MANCC = row["MANCC"],
                        TENNCC = row["TENNCC"],
                        DIACHI = row["DIACHI"],
                        SDT = row["SDT"],
                        EMAIL = row["EMAIL"]
                    });
                }
                return Ok(new { success = true, message = "Lấy danh sách nhà cung cấp thành công", data = list });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi: " + ex.Message });
            }
        }


        [Route("get-byid-nhacungcap")]
        [HttpGet]
        public IActionResult Get_NCC_ById(string ma)
        {
            try
            {
                DataTable dt = NCC_BLL.GetById(ma);
                var list = new List<object>();
                foreach (DataRow row in dt.Rows)
                {
                    list.Add(new
                    {
                        MANCC = row["MANCC"],
                        TENNCC = row["TENNCC"],
                        DIACHI = row["DIACHI"],
                        SDT = row["SDT"],
                        EMAIL = row["EMAIL"]
                    });
                }
                return Ok(new { success = true, message = "Lấy thông tin nhà cung cấp thành công", data = list });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi: " + ex.Message });
            }
        }


        [Route("get-all-khachhang")]
        [HttpGet]
        public IActionResult getAllKH()
        {
            try
            {
                DataTable dt = KH_BLL.getAllKH();
                var list = new List<object>();
                foreach (DataRow row in dt.Rows)
                {
                    list.Add(new
                    {
                        MaKH = row["MaKH"],
                        TenKH = row["TenKH"],
                        SDT = row["SDT"],
                        DiaChi = row["DiaChi"]
                    });
                }
                return Ok(new { success = true, message = "Lấy danh sách khách thành công", data = list });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi: " + ex.Message });
            }
        }


        [Route("get-byid-khachhang")]
        [HttpGet]
        public IActionResult GetByIdKH(string makh)
        {
            try
            {
                DataTable dt = KH_BLL.GetByIdKH(makh);
                var list = new List<object>();
                foreach (DataRow row in dt.Rows)
                {
                    list.Add(new
                    {
                        MaKH = row["MaKH"],
                        TenKH = row["TenKH"],
                        SDT = row["SDT"],
                        DiaChi = row["DiaChi"]
                    });
                }
                return Ok(new { success = true, message = "Lấy thông tin khách thành công", data = list });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi: " + ex.Message });
            }
        }
    }
}
