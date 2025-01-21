using System.ComponentModel.DataAnnotations;

namespace GradeCalculator.ViewModels
{
    public class ShowLogVM
    {
        public string Opis { get; set; } = null!;

        public DateTime Vrijeme { get; set; }
    }
}
