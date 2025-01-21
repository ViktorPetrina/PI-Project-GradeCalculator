using GradeCalculator.Adapter;
using GradeCalculator.Models;
using GradeCalculator.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GradeCalculator.Controllers
{
    public class LogController : Controller
    {
        private readonly PiGradeCalculatorContext _context;


        public LogController(PiGradeCalculatorContext context, )
        {
            _context = context;
        }

        public IActionResult Details()
        {
            var logs = _context.Logs.ToList();
            var adaptedLogs = new List<ShowLogVM>();
            foreach (var log in logs)
            {
                LogAdapter.Instance.Adapt(log);

            }
            return View(adaptedLogs);
            
        }
    }
}
