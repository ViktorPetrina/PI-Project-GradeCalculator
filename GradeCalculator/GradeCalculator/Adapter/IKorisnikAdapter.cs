using GradeCalculator.ViewModels;
using GradeCalculator.Models;

namespace GradeCalculator.Adapter
{
    public interface IKorisnikAdapter
    {
        ShowKorisnikVM Adapt(Korisnik user);
    }
}
