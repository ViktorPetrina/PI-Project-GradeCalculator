
using GradeCalculator.Models;
using Microsoft.IdentityModel.Tokens;

namespace GradeCalculator.Service
{
    public class StatistikaService : IStatistikaService
    {
        private readonly PiGradeCalculatorContext _context;

        public StatistikaService(PiGradeCalculatorContext context)
        {
            _context = context;
        }

        public double KalkulacijaProsjeka(int id)
        {
             var ocjene = _context.Godinas
                .Where(o=> o.KorisnikId==id)
                .Select(o=> (double)o.Prosjek)
                .ToList();
            if (ocjene.IsNullOrEmpty() && !ocjene.Any())
            {
                return 0;
            }
            return ocjene.Average();
        }

        public Dictionary<int,double> KalkulacijaUkupnihProsjeka()
        {
            
        }
    }
}
