using GradeCalculator.Models;
using Microsoft.EntityFrameworkCore;

namespace GradeCalculator.Repository
{
    // Open / Close principle - mozemo raditi nove repozitorije bez mjenjanja postojecih
    public class GodinaRepository : IRepository<Godina>
    {
        private readonly PiGradeCalculatorContext _context;
        public GodinaRepository(PiGradeCalculatorContext context)
        {
            _context = context;
        }

        public Godina? Add(Godina value)
        {
            _context.Godinas.Add(value);
            CommitChanges();

            return value;
        }

        public Godina? Get(int id)
        {
            var years = GetGodinas();

            return years.FirstOrDefault(p => p.Idgodina == id);
        }

        public IEnumerable<Godina> GetAll()
        {
            return GetGodinas();
        }

        public Godina? Modify(int id, Godina value)
        {
            var year = Get(id);

            if (year != null)
            {
                year.Naziv = value.Naziv;
                year.Prosjek = value.Prosjek;
                year.KorisnikId = value.KorisnikId;
            }

            CommitChanges();

            return year;
        }

        public Godina? Remove(int id)
        {
            var year = Get(id);

            if (year != null)
            {
                _context.Godinas.Remove(year);
                CommitChanges();
            }

            return year;
        }

        private IEnumerable<Godina> GetGodinas()
        {
            return _context.Godinas
                .Include(g => g.Predmets)
                .ThenInclude(p => p.Ocjenas);
        }

        private void CommitChanges()
        {
            _context.SaveChanges();
        }
    }
}
