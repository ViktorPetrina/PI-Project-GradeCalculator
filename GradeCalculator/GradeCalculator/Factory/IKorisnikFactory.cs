using GradeCalculator.Models;
using GradeCalculator.ViewModels;

namespace GradeCalculator.Factory
{
    public interface IKorisnikFactory
    {
        Korisnik CreateUser(KorisnikVM userVm);
    }
}
