using GradeCalculator.Models;

namespace GradeCalculator.Repository
{
    public class GodinaRepository : IRepository<Godina>
    {
        private readonly PiGradeCalculatorContext _context;
        public GodinaRepository(PiGradeCalculatorContext context)
        {
            _context = context;
        }

        public Godina? Add(Godina value)
        {
            throw new NotImplementedException();
        }

        public Godina? Get(int id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Godina> GetAll()
        {
            return _context.Godinas;
        }

        public Godina? Modify(int id, Godina value)
        {
            throw new NotImplementedException();
        }

        public Godina? Remove(int id)
        {
            throw new NotImplementedException();
        }
    }
}
