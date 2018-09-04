using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CasaDoCodigo.Status.Models;
using Microsoft.Extensions.HealthChecks;
using CasaDoCodigo.Status.ViewModels;

namespace CasaDoCodigo.Status.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHealthCheckService healthCheckService;

        public HomeController(IHealthCheckService healthCheckService)
        {
            this.healthCheckService = healthCheckService;
        }

        public async Task<IActionResult> Index()
        {
            var result = await healthCheckService.CheckHealthAsync();
            var data = new HealthStatusViewModel(result.CheckStatus);

            foreach (var item in result.Results)
            {
                data.AddResult(item.Key, item.Value);
            }

            return View(data);
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
