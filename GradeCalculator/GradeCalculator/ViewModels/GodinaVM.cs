using System.ComponentModel.DataAnnotations;

namespace GradeCalculator.ViewModels
{
    public class GodinaVM
    {
        public int Idgodina { get; set; }

        [Display(Name = "Naziv")]
        [Required(ErrorMessage = "Naziv godine/semestra je obavezan")]
        public string Naziv { get; set; } = null!;

        [Display(Name = "Prosjek ocijena")]
        public double? Prosjek { get; set; }

        public int? KorisnikId { get; set; }

        public virtual ICollection<PredmetVM> Predmets { get; set; } = new List<PredmetVM>();
    }
}
