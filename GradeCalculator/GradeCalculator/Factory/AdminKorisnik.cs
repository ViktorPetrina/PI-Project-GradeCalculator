using GradeCalculator.Models;
using GradeCalculator.Security;
using GradeCalculator.ViewModels;

namespace GradeCalculator.Factory
{
    public class AdminKorisnik : IKorisnikFactory
    {
        public Korisnik CreateUser(KorisnikVM userVm)
        {
            var b64salt = PasswordProvider.Instance.GetSalt();
            var b64hash = PasswordProvider.Instance.GetHash(userVm.Password, b64salt);

            return new Korisnik
            {
                KorisnickoIme = userVm.UserName,
                Eposta = userVm.Email,
                LozinkaHash = b64hash,
                LozinkaSalt = b64salt,
                UlogaId = 2
            };
        }
    }
}
