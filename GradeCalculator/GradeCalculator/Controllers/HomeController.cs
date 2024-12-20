using GradeCalculator.Models;
using GradeCalculator.Service;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace GradeCalculator.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly LogService _logService;

        public HomeController(ILogger<HomeController> logger, LogService logService)
        {
            _logger = logger;
            _logService = logService;
        }

        public IActionResult Index()
        {
            _logService.AddLog("Korisnik ušao u stranicu");
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
