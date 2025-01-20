using GradeCalculator.Models;
using GradeCalculator.ViewModels;

namespace GradeCalculator.Adapter
{
    public class KorisnikAdapter : IKorisnikAdapter
    {
        public ShowKorisnikVM Adapt(Korisnik user)
        {
            if (user == null)
                return null;

            return new ShowKorisnikVM
            {
                Id = user.Idkorisnik,
                UserName = user.KorisnickoIme,
                Email = user.Eposta,
                TotalGrade = user.UkupnaOcjena,
                RoleName = user.Uloga.Naziv
            };
        }
    }
}
