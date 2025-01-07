using GradeCalculator.Models;
using GradeCalculator.ViewModels;

namespace GradeCalculator.Service
{
    public interface IKorisnikManagable
    {
        void RemoveUser(int id);
        void AddUser(Korisnik user);
    }
}
