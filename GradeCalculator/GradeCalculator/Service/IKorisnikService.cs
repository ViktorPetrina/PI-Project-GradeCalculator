using GradeCalculator.Models;
using GradeCalculator.ViewModels;

namespace GradeCalculator.Service
{
    public interface IKorisnikService
    {
        void RemoveUser(int id);
        IEnumerable<ShowKorisnikVM> GetAllUsers();
        Korisnik GetUser(int id);
        bool IsUsernameTaken(string username);
        bool IsEmailTaken(string email);
    }
}
