using GradeCalculator.Models;
using Microsoft.EntityFrameworkCore;

namespace GradeCalculator.Repository
{
    // repository
    public class PredmetRepository : IRepository<Predmet>
    {
        private readonly PiGradeCalculatorContext _context;
        public PredmetRepository(PiGradeCalculatorContext context)
        {
            _context = context;
        }

        public Predmet? Get(int id)
        {
            var subjects = _context.Predmets.Include(p => p.Ocjenas);

            return subjects.FirstOrDefault(p => p.Idpredmet == id);
        }

        public IEnumerable<Predmet> GetAll()
        {
            return _context.Predmets;
        }

        public Predmet Add(Predmet value)
        {
            _context.Predmets.Add(value);
            _context.SaveChanges();

            return value;
        }

        public Predmet? Modify(int id, Predmet value)
        {
            var subject = Get(id);

            if (subject != null)
            {
                subject.Naziv = value.Naziv;
                subject.Prosjek = value.Prosjek;
                subject.GodinaId = value.GodinaId;
            }

            _context.SaveChanges();

            return subject;
        }

        public Predmet? Remove(int id)
        {
            var subject = Get(id);

            if (subject != null)
            {
                _context.Predmets.Remove(subject);
                _context.SaveChanges();
            }

            return subject;
        }
    }
}
