using ADMIN.Getways.Interface;
using ADMIN.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace ADMIN.Controllers
{
    [Authorize(Roles ="login")]
    public class HomeController : Controller
    {  
        private readonly ILogger<HomeController> _logger;
        private readonly IResponseGetaway _responseGetaway;

        public HomeController(ILogger<HomeController> logger, IResponseGetaway responseGetaway)
        {
            _logger = logger;
            _responseGetaway = responseGetaway;
        }

        public IActionResult Index()
        {
            var response = _responseGetaway.GetTAsync("WeatherForecast").Result;
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
