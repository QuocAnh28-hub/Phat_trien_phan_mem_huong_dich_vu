using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using BLL;
using System.Data;

namespace API_ThanhToan.Properties
{
    [Route("api/[controller]")]
    [ApiController]
    public class ThanhToan_Controller : ControllerBase
    {
        ThanhToan_BLL _BLL = new ThanhToan_BLL();


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
        public IActionResult GetAll()
        {
            try
            {
                DataTable dt = _BLL.getAll();
                return Ok(new { success = true, message = "Lấy danh sách thanh toán thành công", data = ChuyenThanhList(dt) });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi: " + ex.Message });
            }
        }

        [Route("get-byId-thanhtoan")]
        [HttpGet]
        public IActionResult GetById(string ma)
        {
            try
            {
                DataTable dt = _BLL.GetById(ma);
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
                DataTable dt = _BLL.GetById(model.MaThanhToan);
                if (dt.Rows.Count == 1)
                {
                    return Ok(new { success = false, message = "Đã tồn tại thanh toán có mã này" });

                }
                else
                {
                    dt = _BLL.Create(model);
                    return Ok(new { success = true, message = "Thêm thông tin thanh toán thành công" });
                }
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
                DataTable dt = _BLL.GetById(model.MaThanhToan);
                if (dt.Rows.Count < 1)
                {
                    return Ok(new { success = false, message = "Không tồn tại thanh toán có mã này" });

                }
                else
                {
                    dt = _BLL.Update(model);
                    return Ok(new { success = true, message = "Thay đổi thông tin thanh toán thành công" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi: " + ex.Message });
            }
        }


        [Route("del-thanhtoan")]
        [HttpDelete]
        public IActionResult Delete(string ma)
        {
            try
            {
                DataTable dt = _BLL.GetById(ma);
                if (dt.Rows.Count < 1)
                {
                    return Ok(new { success = false, message = "Không có thông tin thanh toán có mã này" });

                }
                else
                {
                    dt = _BLL.Delete(ma);
                    return Ok(new { success = true, message = "Xoá thông tin thanh toán thành công" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi: " + ex.Message });
            }
        }
    }
}
