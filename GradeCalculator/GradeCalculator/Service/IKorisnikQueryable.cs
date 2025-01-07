using GradeCalculator.ViewModels;

namespace GradeCalculator.Service
{
    public interface IKorisnikQueryable
    {
        IEnumerable<ShowKorisnikVM> GetAllUsers();
        ShowKorisnikVM GetUserDetails(int id);
        bool IsUsernameTaken(string username);
        bool IsEmailTaken(string email);
    }
}
