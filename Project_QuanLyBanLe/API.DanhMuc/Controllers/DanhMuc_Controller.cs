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

namespace API.DanhMuc.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DanhMuc_Controller : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public DanhMuc_Controller(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]//Lấy danh sách tất cả DanhMuc
        public async Task<ActionResult<IEnumerable<Models.DanhMuc>>> GetAll()
        {
            var list = new List<Models.DanhMuc>();
            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (var conn = new SqlConnection(connectionString))
            {
                string sql = "SELECT MADANHMUC, TENDANHMUC, MOTA FROM DANHMUC";
                using (var cmd = new SqlCommand(sql, conn))
                {
                    await conn.OpenAsync();
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var d = new Models.DanhMuc
                            {
                                MADANHMUC = reader["MADANHMUC"] == DBNull.Value ? null : reader["MADANHMUC"].ToString(),
                                TENDANHMUC = reader["TENDANHMUC"] == DBNull.Value ? null : reader["TENDANHMUC"].ToString(),
                                MOTA = reader["MOTA"] == DBNull.Value ? null : reader["MOTA"].ToString()
                            };
                            list.Add(d);
                        }
                    }
                }
            }

            return Ok(list);
        }

        [HttpPost]//Thêm DanhMuc mới
        public async Task<IActionResult> Create([FromBody] Models.DanhMuc model)
        {
            if (model == null)
            {
                return BadRequest("Dữ liệu không hợp lệ.");
            }

            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (var conn = new SqlConnection(connectionString))
            {
                string sql = @"INSERT INTO DANHMUC (MADANHMUC, TENDANHMUC, MOTA)
                       VALUES (@MADANHMUC, @TENDANHMUC, @MOTA)";

                using (var cmd = new SqlCommand(sql, conn))
                {
                    // Tránh SQL Injection bằng tham số
                    cmd.Parameters.AddWithValue("@MADANHMUC", model.MADANHMUC ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@TENDANHMUC", model.TENDANHMUC ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@MOTA", model.MOTA ?? (object)DBNull.Value);

                    await conn.OpenAsync();
                    int rows = await cmd.ExecuteNonQueryAsync();

                    if (rows > 0)
                        return Ok(new { message = "Thêm danh mục thành công!" });
                    else
                        return StatusCode(500, new { message = "Không thể thêm danh mục." });
                }
            }
        }
    }
}