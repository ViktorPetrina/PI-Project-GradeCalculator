using GradeCalculator.Models;

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
            _context.SaveChanges();

            return value;
        }

        public Godina? Get(int id)
        {
            var years = _context.Godinas;

            return years.FirstOrDefault(p => p.Idgodina == id);
        }

        public IEnumerable<Godina> GetAll()
        {
            return _context.Godinas;
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

            _context.SaveChanges();

            return year;
        }

        public Godina? Remove(int id)
        {
            var year = Get(id);

            if (year != null)
            {
                _context.Godinas.Remove(year);
                _context.SaveChanges();
            }

            return year;
        }
    }
}
