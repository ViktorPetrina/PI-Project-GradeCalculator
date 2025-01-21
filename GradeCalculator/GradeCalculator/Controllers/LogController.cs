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
        private readonly ILogAdapter _adapter;

        public LogController(PiGradeCalculatorContext context, ILogAdapter adapter)
        {
            _context = context;
            _adapter = adapter;
        }

        public IActionResult Details()
        {
            var logs = _context.Logs.ToList();
            var adaptedLogs = new List<ShowLogVM>();
            foreach (var log in logs)
            {
                adaptedLogs.Add(_adapter.Adapt(log));

            }
            return View(adaptedLogs);
            
        }
    }
}
