using GradeCalculator.Models;
using System.ComponentModel.DataAnnotations;

namespace GradeCalculator.ViewModels
{
    public class PredmetVM
    {
        // ovo maknuti kasnije - ne pokazujemo id-jeve
        [Display(Name = "ID")]
        public int Idpredmet { get; set; }

        [Display(Name = "Naziv predmeta")]
        [Required(ErrorMessage = "Naziv je obavezan!")]
        public string Naziv { get; set; } = null!;

        [Display(Name = "Prosjek ocijena")]
        public double? Prosjek { get; set; }

        [Display(Name = "Godina/semestar")]
        public int? GodinaId { get; set; }

        public virtual ICollection<Ocjena> Ocjenas { get; set; } = new List<Ocjena>();
    }
}
