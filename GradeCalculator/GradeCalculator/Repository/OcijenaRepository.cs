using GradeCalculator.Models;

namespace GradeCalculator.Repository
{
    public class OcijenaRepository : IRepository<Ocjena>
    {
        private readonly PiGradeCalculatorContext _context;
        public OcijenaRepository(PiGradeCalculatorContext context)
        {
            _context = context;
        }

        public Ocjena? Add(Ocjena value)
        {
            throw new NotImplementedException();
        }

        public Ocjena? Get(int id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Ocjena> GetAll()
        {
            return _context.Ocjenas;
        }

        public IEnumerable<Ocjena> GetBySubject(int id)
        {
            return _context.Ocjenas.Where(o => o.PredmetId == id);
        }

        public Ocjena? Modify(int id, Ocjena value)
        {
            throw new NotImplementedException();
        }

        public Ocjena? Remove(int id)
        {
            throw new NotImplementedException();
        }
    }
}
