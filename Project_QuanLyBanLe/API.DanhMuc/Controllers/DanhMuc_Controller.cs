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
        private readonly DanhMuc_BLL dm_bll;

        public DanhMuc_Controller(IConfiguration configuration)
        {
            dm_bll = new DanhMuc_BLL(configuration);
        }

        [HttpGet]
        public IActionResult GetAll() => Ok(dm_bll.LayTatCa());

        [HttpPost]
        public IActionResult Create(Models.DanhMuc model)
        {
            bool result = dm_bll.ThemMoi(model);
            return result ? Ok("Thêm thành công") : BadRequest("Thêm thất bại");
        }
    }
}