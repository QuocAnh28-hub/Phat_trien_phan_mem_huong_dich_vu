using BLL;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Task2_API_ThuKho.Controllers
{
    [Route("api/QuanLySanPham")]
    [ApiController]
    public class QuanLySanPham_Controller : ControllerBase
    {
        private readonly SanPham_BLL sp_bll;

        public QuanLySanPham_Controller(IConfiguration configuration)
        {
            sp_bll = new SanPham_BLL(configuration);
        }
        [HttpGet("get-all-sanpham")]
        public IActionResult GetAll()
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
        public IActionResult GetByID(string id)
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

        [Route("insert-sanpham")]
        [HttpPost]
        public IActionResult Create(Models.SanPham model)
        {
            try
            {
                bool result = sp_bll.ThemMoi(model);
                return result ? Ok("Thêm thành công") : BadRequest("Thêm thất bại");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi: " + ex.Message });
            }
        }

        [Route("update-sanpham")]
        [HttpPut]
        public IActionResult Update(Models.SanPham model)
        {
            try
            {
                bool result = sp_bll.Sua(model);
                return result ? Ok("Cập nhật thành công") : BadRequest("Cập nhật thất bại");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi: " + ex.Message });
            }
        }

        [Route("update-soluong-sanpham")]
        [HttpPatch]
        public IActionResult UpdateSoLuong(string maSP, int soLuongMoi)
        {
            try
            {
                bool result = sp_bll.SuaSoLuong(maSP, soLuongMoi);
                return result ? Ok("Cập nhật số lượng thành công") : BadRequest("Cập nhật thất bại");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi: " + ex.Message });
            }
        }

        [Route("delete-sanpham")]
        [HttpDelete]
        public IActionResult Delete(string maSP)
        {
            try
            {
                bool result = sp_bll.Xoa(maSP);
                return result ? Ok("Xóa sản phẩm thành công") : BadRequest("Xóa thất bại");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi: " + ex.Message });
            }
        }
    }
}
