using GradeCalculator.Models;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;

namespace GradeCalculator.Service
{
    public class LogService : ILogService
    {
        private readonly PiGradeCalculatorContext _context;
        public LogService(PiGradeCalculatorContext context)
        {
            _context = context;
        }
        public void AddLog(string message)
        {
            string prilagodeniOpis = "Log:" + message;
            var log = new Log
            {
                Opis = prilagodeniOpis,
                Vrijeme = DateTime.Now,
            };
            _context.Logs.Add(log);
            _context.SaveChanges();
        }
    }
}
