﻿
using GradeCalculator.Models;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;
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
             var ocjene = _context.Korisniks
                .Select(o=>o.UkupnaOcjena)
                .ToList();

            if (ocjene.IsNullOrEmpty() && !ocjene.Any())
            {
                return 0;
            }
            return ocjene.Average();
        }

        public Dictionary<int,double> KalkulacijaUkupnihProsjeka()
        {
            int brojOcjena = _context.Korisniks.Count();

            var ocjenePercentage = _context.Korisniks
                .GroupBy(o => (int)Math.Round(o.UkupnaOcjena, MidpointRounding.AwayFromZero))
                .ToDictionary(
                    o => o.Key,
                    o => Math.Round((o.Count() / (double)brojOcjena) * 100, 2) 
                     );
            return ocjenePercentage;

            
        }
    }
}