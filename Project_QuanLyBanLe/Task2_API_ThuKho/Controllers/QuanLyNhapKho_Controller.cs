using BLL;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models;
using System.Data;

namespace Task2_API_ThuKho.Controllers
{
    [Authorize]
    [Route("api/QuanLyNhapKho")]
    [ApiController]
    public class QuanLyNhapKho_Controller : ControllerBase
    {
        private readonly PhieuNhapKho_BLL _bll;
        private readonly ChiTietNhap_BLL ctn_bll;
        private readonly NhaCungCap_BLL NCC_BLL;
        public QuanLyNhapKho_Controller(IConfiguration configuration)
        {
            _bll = new PhieuNhapKho_BLL(configuration);
            ctn_bll = new ChiTietNhap_BLL(configuration);
            NCC_BLL = new NhaCungCap_BLL(configuration);
        }

        //PhieuNhapKho
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



        //ChiTietNhap

        // 🔹 Lấy tất cả
        [HttpGet("get-all-chitietnhap")]
        public IActionResult GetAll()
        {
            try
            {
                var data = ctn_bll.LayTatCa()
                               .Select(x => new {
                                   MAPHIEUNHAP = x.MAPHIEUNHAP?.Trim(),
                                   MASP = x.MASP?.Trim(),
                                   SOLUONG = x.SOLUONG,
                                   DONGIANHAP = x.DONGIANHAP,
                                   THANHTIEN = x.THANHTIEN,
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

                var list = ctn_bll.LayTheoPhieu(maphieunhap);
                var data = list.Select(x => new {
                    MAPHIEUNHAP = x.MAPHIEUNHAP?.Trim(),
                    MASP = x.MASP?.Trim(),
                    SOLUONG = x.SOLUONG,
                    DONGIANHAP = x.DONGIANHAP,
                    THANHTIEN = x.THANHTIEN,
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

                var list = ctn_bll.LayTheoID(maphieunhap, masp);
                if (list == null || list.Count == 0)
                    return Ok(new { success = false, message = "Không tìm thấy chi tiết nhập" });

                var x = list.First();
                var data = new
                {
                    MAPHIEUNHAP = x.MAPHIEUNHAP?.Trim(),
                    MASP = x.MASP?.Trim(),
                    SOLUONG = x.SOLUONG,
                    DONGIANHAP = x.DONGIANHAP,
                    THANHTIEN = x.THANHTIEN
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

                var ok = ctn_bll.ThemMoi(ct);
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

                var ok = ctn_bll.CapNhat(ct);
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

                var ok = ctn_bll.Xoa(maphieunhap, masp);
                if (!ok) return Ok(new { success = false, message = "Không thể xoá (không tìm thấy dòng)" });

                return Ok(new { success = true, message = "Xoá chi tiết nhập thành công" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi: " + ex.Message });
            }
        }


        //NhaCungCap
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
        public IActionResult GetById(string ma)
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

        [Route("del-nhacungcap")]
        [HttpDelete]
        public IActionResult Delete(string ma)
        {
            try
            {
                DataTable dt = NCC_BLL.GetById(ma);
                if (dt.Rows.Count < 1)
                {
                    return Ok(new { success = false, message = "Không có thông tin nhà cung cấp có mã này" });

                }
                else
                {
                    dt = NCC_BLL.Delete(ma);
                    return Ok(new { success = true, message = "Xoá thông tin nhà cung cấp thành công" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi: " + ex.Message });
            }
        }

        [Route("update-khachhang")]
        [HttpPost]
        public IActionResult Update([FromBody] Models.NhaCungCap model)
        {
            try
            {
                DataTable dt = NCC_BLL.GetById(model.MaNCC);
                if (dt.Rows.Count < 1)
                {
                    return Ok(new { success = false, message = "Không có thông tin khách hàng có mã này" });

                }
                else
                {
                    dt = NCC_BLL.Update(model);
                    return Ok(new { success = true, message = "Thay đổi thông tin khách thành công" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi: " + ex.Message });
            }
        }


        [Route("create-nhacungcap")]
        [HttpPost]
        public IActionResult Create([FromBody] Models.NhaCungCap model)
        {
            try
            {
                DataTable dt = NCC_BLL.GetById(model.MaNCC);
                if (dt.Rows.Count == 1)
                {
                    return Ok(new { success = false, message = "Đã tồn tại nhà cung cấp có mã này" });

                }
                else
                {
                    dt = NCC_BLL.Create(model);
                    return Ok(new { success = true, message = "Thêm thông tin nhà cung cấp thành công" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi: " + ex.Message });
            }
        }

    }
}
