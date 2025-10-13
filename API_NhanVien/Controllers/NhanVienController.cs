using BLL;
using Microsoft.AspNetCore.Mvc;
using Models;
using System;
using System.Collections.Generic;
using System.Data;

namespace API_NhanVien.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NhanVien_Controller : ControllerBase
    {
        NhanVien_BLL NV_BLL = new NhanVien_BLL();

        [Route("get-all-nhanvien")]
        [HttpGet]
        public IActionResult GetAllNhanVien()
        {
            try
            {
                DataTable dt = NV_BLL.GetAllNhanVien();
                var list = new List<object>();
                foreach (DataRow row in dt.Rows)
                {
                    list.Add(new
                    {
                        MANV = row["MANV"].ToString().Trim(),
                        TENNV = row["TENNV"].ToString().Trim(),
                        SDT = row["SDT"].ToString().Trim(),
                        DIACHI = row["DIACHI"].ToString().Trim()
                    });
                }
                return Ok(new { success = true, message = "Lấy danh sách nhân viên thành công", data = list });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi: " + ex.Message });
            }
        }

        [Route("get-byid-nhanvien")]
        [HttpGet]
        public IActionResult GetByIdNhanVien(string manv)
        {
            try
            {
                DataTable dt = NV_BLL.GetByIdNhanVien(manv);
                if (dt.Rows.Count == 0)
                    return Ok(new { success = false, message = "Không tìm thấy nhân viên này" });

                var nv = dt.Rows[0];
                return Ok(new
                {
                    success = true,
                    message = "Lấy thông tin nhân viên thành công",
                    data = new
                    {
                        MANV = nv["MANV"].ToString().Trim(),
                        TENNV = nv["TENNV"].ToString().Trim(),
                        SDT = nv["SDT"].ToString().Trim(),
                        DIACHI = nv["DIACHI"].ToString().Trim()
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi: " + ex.Message });
            }
        }

        [Route("create-nhanvien")]
        [HttpPost]
        public IActionResult CreateNhanVien([FromBody] NhanVien nv)
        {
            try
            {
                DataTable check = NV_BLL.GetByIdNhanVien(nv.MANV);
                if (check.Rows.Count > 0)
                    return Ok(new { success = false, message = "Đã tồn tại nhân viên có mã này" });

                NV_BLL.CreateNhanVien(nv);
                return Ok(new { success = true, message = "Thêm nhân viên thành công" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi: " + ex.Message });
            }
        }

        [Route("update-nhanvien")]
        [HttpPost]
        public IActionResult UpdateNhanVien([FromBody] NhanVien nv)
        {
            try
            {
                DataTable check = NV_BLL.GetByIdNhanVien(nv.MANV);
                if (check.Rows.Count == 0)
                    return Ok(new { success = false, message = "Không tìm thấy nhân viên cần sửa" });

                NV_BLL.UpdateNhanVien(nv);
                return Ok(new { success = true, message = "Cập nhật nhân viên thành công" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi: " + ex.Message });
            }
        }

        [Route("delete-nhanvien")]
        [HttpDelete]
        public IActionResult DeleteNhanVien(string manv)
        {
            try
            {
                DataTable check = NV_BLL.GetByIdNhanVien(manv);
                if (check.Rows.Count == 0)
                    return Ok(new { success = false, message = "Không tìm thấy nhân viên để xoá" });

                NV_BLL.DeleteNhanVien(manv);
                return Ok(new { success = true, message = "Xoá nhân viên thành công" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi: " + ex.Message });
            }
        }
    }
}
