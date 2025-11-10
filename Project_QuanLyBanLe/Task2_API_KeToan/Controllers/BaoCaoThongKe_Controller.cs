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
    [Route("api/BaoCaoThongKe")]
    [ApiController]
    public class BaoCaoThongKe_Controller : ControllerBase
    {
        private readonly DanhMuc_BLL dm_bll;
        private readonly SanPham_BLL sp_bll;
        private readonly KhuyenMai_BLL KM_BLL;
        private readonly PhieuNhapKho_BLL PNK_bll;
        private readonly ChiTietNhap_BLL CTN_bll;
        private readonly HoaDonBan_BLL hdb_bll;
        private readonly ChiTietBan_BLL ctb_bll;

        public BaoCaoThongKe_Controller(IConfiguration configuration)
        {
            dm_bll = new DanhMuc_BLL(configuration);
            sp_bll = new SanPham_BLL(configuration);
            KM_BLL = new KhuyenMai_BLL(configuration);
            PNK_bll = new PhieuNhapKho_BLL(configuration);
            CTN_bll = new ChiTietNhap_BLL(configuration);
            hdb_bll = new HoaDonBan_BLL(configuration);
            ctb_bll = new ChiTietBan_BLL(configuration);
        }

        [Route("get-all-danhmuc")]
        [HttpGet]
        public IActionResult GetAll_DM()
        {
            try
            {
                var list = dm_bll.LayTatCa();

                if (list == null || !list.Any())
                {
                    return NoContent();
                }

                return Ok(list);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi: " + ex.Message });
            }
        }

        [Route("get-byID-danhmuc")]
        [HttpGet]
        public IActionResult Get_DM_ByID(string madanhmuc)
        {
            try
            {
                var danhmuc = dm_bll.LayTheoID(madanhmuc);

                if (danhmuc == null || danhmuc.Count == 0)
                    return NotFound("Không tìm thấy danh mục.");

                return Ok(danhmuc);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi: " + ex.Message });
            }
        }

        [HttpGet("get-all-sanpham")]
        public IActionResult GetAll_SP()
        {
            try
            {
                var result = sp_bll.LayTatCa();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi: " + ex.Message });
            }
        }

        [HttpGet("get-sanpham-by-id")]
        public IActionResult Get_SP_ByID(string id)
        {
            try
            {
                var result = sp_bll.LayTheoID(id);
                if (result == null || result.Count == 0)
                    return NotFound("Không tìm thấy sản phẩm.");

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi: " + ex.Message });
            }
        }

        [Route("get-all-khuyenmai")]
        [HttpGet]
        public IActionResult getAll_KM()
        {
            try
            {
                DataTable dt = KM_BLL.getAll();
                var list = new List<object>();
                foreach (DataRow row in dt.Rows)
                {
                    list.Add(new
                    {
                        MAKM = row["MAKM"],
                        TENKM = row["TENKM"],
                        MASP = row["MASP"],
                        NGAYBATDAU = row["NGAYBATDAU"],
                        NGAYKETTHUC = row["NGAYKETTHUC"]
                    });
                }
                return Ok(new { success = true, message = "Lấy danh sách khuyến mại thành công", data = list });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi: " + ex.Message });
            }
        }


        [Route("get-byid-khuyenmai")]
        [HttpGet]
        public IActionResult Get_KM_ById(string ma)
        {
            try
            {
                DataTable dt = KM_BLL.GetById(ma);
                var list = new List<object>();
                foreach (DataRow row in dt.Rows)
                {
                    list.Add(new
                    {
                        MAKM = row["MAKM"],
                        TENKM = row["TENKM"],
                        MASP = row["MASP"],
                        NGAYBATDAU = row["NGAYBATDAU"],
                        NGAYKETTHUC = row["NGAYKETTHUC"]
                    });
                }
                return Ok(new { success = true, message = "Lấy thông tin khuyến mại thành công", data = list });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi: " + ex.Message });
            }
        }

        [HttpGet("get-all-phieunhapkho")]
        public IActionResult GetAllPhieuNhapKho()
        {
            try
            {
                var data = PNK_bll.LayTatCa();
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
                var list = PNK_bll.LayTheoID(maphieunhap);
                if (list == null || list.Count == 0)
                    return Ok(new { success = false, message = "Không tìm thấy phiếu nhập kho" });

                return Ok(new { success = true, message = "Lấy thông tin phiếu nhập kho thành công", data = list });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi: " + ex.Message });
            }
        }

        [HttpGet("get-all-chitietnhap")]
        public IActionResult GetAll_CTN()
        {
            try
            {
                var data = CTN_bll.LayTatCa()
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

        [HttpGet("get-byphieu-chitietnhap")]
        public IActionResult GetByPhieu([FromQuery] string maphieunhap)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(maphieunhap))
                    return Ok(new { success = false, message = "Thiếu mã phiếu nhập" });

                var list = CTN_bll.LayTheoPhieu(maphieunhap);
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

        [Route("get-all-chitietban")]
        [HttpGet]
        public IActionResult GetAll_CTB()
        {
            try
            {
                var result = ctb_bll.LayTatCa();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi: " + ex.Message });
            }
        }

        [Route("get-chitietban-by-IDhoadon")]
        [HttpGet]
        public IActionResult Get_CTB_ByHoaDon(string maHDB)
        {
            try
            {
                var result = ctb_bll.LayTheoHoaDon(maHDB);
                if (result == null || result.Count == 0)
                    return NotFound("Không tìm thấy chi tiết của hóa đơn.");

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi: " + ex.Message });
            }
        }
    }
}
