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

namespace Task2_API_Admin.Controllers
{
    [Authorize]
    [Route("api/QuanLyKhuyenMai")]
    [ApiController]
    public class QuanLyKhuyenMai_Controller : ControllerBase
    {
        private readonly KhuyenMai_BLL _BLL;   // đổi TaiKhoan_BLL -> KhuyenMai_BLL

        public QuanLyKhuyenMai_Controller(IConfiguration configuration)
        {
            _BLL = new KhuyenMai_BLL(configuration);  // khởi tạo đúng BLL khuyến mãi
        }

        [Route("get-all-khuyenmai")]
        [HttpGet]
        public IActionResult getAll()
        {
            try
            {
                DataTable dt = _BLL.getAll();
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
        public IActionResult GetById(string ma)
        {
            try
            {
                DataTable dt = _BLL.GetById(ma);
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

        [Route("del-khuyenmai")]
        [HttpDelete]
        public IActionResult Delete(string ma)
        {
            try
            {
                DataTable dt = _BLL.GetById(ma);
                if (dt.Rows.Count < 1)
                {
                    return Ok(new { success = false, message = "Không có thông tin khuyến mại có mã này" });
                }
                else
                {
                    dt = _BLL.Delete(ma);
                    return Ok(new { success = true, message = "Xoá thông tin khuyến mại thành công" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi: " + ex.Message });
            }
        }

        [Route("update-khuyenmai")]
        [HttpPost]
        public IActionResult Update([FromBody] Models.KhuyenMai model)
        {
            try
            {
                DataTable dt = _BLL.GetById(model.MaKM);
                if (dt.Rows.Count < 1)
                {
                    return Ok(new { success = false, message = "Không có thông tin khuyến mại có mã này" });
                }
                else
                {
                    dt = _BLL.Update(model);
                    return Ok(new { success = true, message = "Thay đổi thông tin khuyến mại thành công" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi: " + ex.Message });
            }
        }

        [Route("create-khuyenmai")]
        [HttpPost]
        public IActionResult Create([FromBody] Models.KhuyenMai model)
        {
            try
            {
                DataTable dt = _BLL.GetById(model.MaKM);
                if (dt.Rows.Count == 1)
                {
                    return Ok(new { success = false, message = "Đã tồn tại khuyến mại có mã này" });
                }
                else
                {
                    dt = _BLL.Create(model);
                    return Ok(new { success = true, message = "Thêm thông tin khuyến mại thành công" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi: " + ex.Message });
            }
        }
    }
}
