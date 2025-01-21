using GradeCalculator.Models;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using System.Xml.Linq;

namespace GradeCalculator.Service
{
    public class LogService : ILogService
    {
        
        private readonly IServiceProvider _serviceProvider;

        public LogService() { }

        public LogService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public void AddLog(string message)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<PiGradeCalculatorContext>();
                string prilagodeniOpis = "Log: " + message;
                var log = new Log
                {
                    Opis = prilagodeniOpis,
                    Vrijeme = DateTime.Now,
                };
                context.Logs.Add(log);
                context.SaveChanges();
            }
        }

        public List<Log> GetLogsAsXML()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<PiGradeCalculatorContext>();

                return context.Logs.ToList();
            }
        }
    }
}
