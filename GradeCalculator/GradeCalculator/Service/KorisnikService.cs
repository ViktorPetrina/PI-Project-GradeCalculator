using GradeCalculator.Models;
using GradeCalculator.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace GradeCalculator.Service
{
    public class KorisnikService : IKorisnikService
    {
        private readonly PiGradeCalculatorContext _context;

        public KorisnikService() { }

        public KorisnikService(PiGradeCalculatorContext context) 
        {
            _context = context;
        }

        public IEnumerable<ShowKorisnikVM> GetAllUsers()
        {
            return _context.Korisniks.Include(u => u.Uloga)
            .Select(u => new ShowKorisnikVM
            {
                Id = u.Idkorisnik,
                UserName = u.KorisnickoIme,
                Email = u.Eposta,
                TotalGrade = u.UkupnaOcjena,
                RoleName = u.Uloga.Naziv
            }).ToList();
        }

        public Korisnik GetUser(int id)
        {
            var user = _context.Korisniks.Include(u => u.Uloga).FirstOrDefault(u => u.Idkorisnik == id);
            if (user == null) return null;

            return user;
        }

        public Korisnik GetUserByName(string name)
        {
            var user = _context.Korisniks.Include(u => u.Uloga).FirstOrDefault(u => u.KorisnickoIme == name);
            if (user == null) return null;

            return user;
        }

        public bool IsEmailTaken(string email)
        {
            return _context.Korisniks.Any(u => u.Eposta == email);
        }

        public bool IsUsernameTaken(string username)
        {
            return _context.Korisniks.Any(u => u.KorisnickoIme == username);
        }

        public void RemoveUser(int id)
        {
            var user = _context.Korisniks.Include(u => u.Uloga).FirstOrDefault(u => u.Idkorisnik == id);
            if (user != null)
            {
                RemoveUsersData(user);

                _context.Korisniks.Remove(user);
                _context.SaveChanges();
            }
        }

        private void RemoveUsersData(Korisnik user)
        {
            var relatedYears = _context.Godinas.Where(g => g.KorisnikId == user.Idkorisnik).ToList();
            foreach (var year in relatedYears)
            {
                var relatedSubjects = _context.Predmets.Where(p => p.GodinaId == year.Idgodina).ToList();
                foreach (var subject in relatedSubjects)
                {
                    var relatedGrades = _context.Ocjenas.Where(o => o.PredmetId == subject.Idpredmet).ToList();
                    _context.Ocjenas.RemoveRange(relatedGrades);
                    _context.Predmets.Remove(subject);
                }

                _context.Godinas.Remove(year);
            }
        }

    }
}
