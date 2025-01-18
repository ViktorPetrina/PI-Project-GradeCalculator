using GradeCalculator.Models;
using GradeCalculator.ViewModels;

namespace GradeCalculator.Service
{
    public interface IKorisnikQueryable
    {
        IEnumerable<ShowKorisnikVM> GetAllUsers();
        Korisnik GetUser(int id);
        bool IsUsernameTaken(string username);
        bool IsEmailTaken(string email);
    }
}
