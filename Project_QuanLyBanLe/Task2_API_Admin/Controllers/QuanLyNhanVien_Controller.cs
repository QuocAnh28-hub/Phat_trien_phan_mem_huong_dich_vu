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
namespace Task2_API_Admin.Controllers
{      
    [Route("api/[controller]")]
    [ApiController]
    public class QuanLyNhanVien_Controller:ControllerBase
    {
            private readonly NhanVien_BLL _bll;
            public QuanLyNhanVien_Controller(IConfiguration configuration)
            {
                _bll = new NhanVien_BLL(configuration);
            }

            [HttpGet("get-all-nhanvien")]
            public IActionResult GetAllNhanVien()
            {
                try
                {
                    var data = _bll.LayTatCa()
                        .Select(x => new
                        {
                            MANV = x.MANV?.Trim(),
                            TENNV = x.TENNV?.Trim(),
                            SDT = x.SDT?.Trim(),
                            DIACHI = x.DIACHI?.Trim()
                        })
                        .ToList();

                    return Ok(new { success = true, message = "Lấy danh sách nhân viên thành công", data });
                }
                catch (Exception ex)
                {
                    return StatusCode(500, new { success = false, message = "Lỗi: " + ex.Message });
                }
            }

            [HttpGet("get-byid-nhanvien")]
            public IActionResult GetByIdNhanVien([FromQuery] string manv)
            {
                try
                {
                    var list = _bll.LayTheoID(manv);
                    if (list == null || list.Count == 0)
                        return Ok(new { success = false, message = "Không tìm thấy nhân viên" });

                    var data = list.Select(x => new
                    {
                        MANV = x.MANV?.Trim(),
                        TENNV = x.TENNV?.Trim(),
                        SDT = x.SDT?.Trim(),
                        DIACHI = x.DIACHI?.Trim()
                    });

                    return Ok(new { success = true, message = "Lấy thông tin nhân viên thành công", data });
                }
                catch (Exception ex)
                {
                    return StatusCode(500, new { success = false, message = "Lỗi: " + ex.Message });
                }
            }

            [HttpPost("create-nhanvien")]
            public IActionResult CreateNhanVien([FromBody] NhanVien nv)
            {
                try
                {
                    if (nv == null || string.IsNullOrWhiteSpace(nv.MANV) ||
                        string.IsNullOrWhiteSpace(nv.TENNV))
                        return Ok(new { success = false, message = "Thiếu thông tin bắt buộc" });

                    var ok = _bll.ThemMoi(nv);
                    if (!ok) return Ok(new { success = false, message = "Không thể thêm (mã đã tồn tại hoặc lỗi dữ liệu)" });

                    return Ok(new { success = true, message = "Thêm nhân viên thành công" });
                }
                catch (Exception ex)
                {
                    return StatusCode(500, new { success = false, message = "Lỗi: " + ex.Message });
                }
            }

            [HttpPost("update-nhanvien")]
            public IActionResult UpdateNhanVien([FromBody] NhanVien nv)
            {
                try
                {
                    if (nv == null || string.IsNullOrWhiteSpace(nv.MANV))
                        return Ok(new { success = false, message = "Thiếu mã nhân viên" });

                    var ok = _bll.CapNhat(nv);
                    if (!ok) return Ok(new { success = false, message = "Không thể cập nhật (mã không tồn tại hoặc lỗi dữ liệu)" });

                    return Ok(new { success = true, message = "Cập nhật nhân viên thành công" });
                }
                catch (Exception ex)
                {
                    return StatusCode(500, new { success = false, message = "Lỗi: " + ex.Message });
                }
            }

            [HttpDelete("delete-nhanvien")]
            public IActionResult DeleteNhanVien([FromQuery] string manv)
            {
                try
                {
                    var ok = _bll.Xoa(manv);
                    if (!ok) return Ok(new { success = false, message = "Không thể xoá (mã không tồn tại)" });

                    return Ok(new { success = true, message = "Xoá nhân viên thành công" });
                }
                catch (Exception ex)
                {
                    return StatusCode(500, new { success = false, message = "Lỗi: " + ex.Message });
                }
            }
        }

 }
