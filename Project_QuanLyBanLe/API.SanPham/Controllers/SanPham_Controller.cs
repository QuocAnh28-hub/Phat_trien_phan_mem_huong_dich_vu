using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BLL;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Models;

namespace API.SanPham.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SanPham_Controller : ControllerBase
    {
    }
}
