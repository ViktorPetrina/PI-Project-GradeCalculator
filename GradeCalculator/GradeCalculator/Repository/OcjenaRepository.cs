using GradeCalculator.Models;
using Microsoft.EntityFrameworkCore;

namespace GradeCalculator.Repository
{
    // Single Responsibility - samo dohvaca ocjene
    // Interface segregation principle - IReadAllRepository
    public class OcjenaRepository : IReadAllRepository<Ocjena>
    {
        protected readonly PiGradeCalculatorContext _context;

        public OcjenaRepository(PiGradeCalculatorContext context)
        {
            _context = context;
        }

        public IEnumerable<Ocjena> GetAll()
        {
            return _context.Ocjenas;
        }

        public virtual IEnumerable<Ocjena> GetBySubject(int id)
        {
            return _context.Ocjenas.Where(o => o.PredmetId == id);
        }
    }

    // Liskov substitution 
    public class ComplexOcjenaRepository : OcjenaRepository
    {
        public ComplexOcjenaRepository(PiGradeCalculatorContext context) : base(context)
        {
        }

        public override IEnumerable<Ocjena> GetBySubject(int id)
        {
            return _context.Ocjenas.Where(o => o.PredmetId == id);
        }
    }
}
