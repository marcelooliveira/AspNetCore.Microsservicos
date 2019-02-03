using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CasaDoCodigo.Status.Models;
using Microsoft.Extensions.Configuration;

namespace CasaDoCodigo.Status.Controllers
{
    public class HomeController : Controller
    {
        private readonly IConfiguration _configuration;

        public HomeController(IConfiguration configuration)
        {
            this._configuration = configuration;
        }

        public async Task<IActionResult> Index()
        {
            var basePath = _configuration["PATH_BASE"];
            return Redirect($"{basePath}/hc-ui");
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
