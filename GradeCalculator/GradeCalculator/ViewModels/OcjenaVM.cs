using GradeCalculator.Models;
using System.ComponentModel.DataAnnotations;

namespace GradeCalculator.ViewModels
{
    public class OcjenaVM
    {
        public int Idocjena { get; set; }

        [Display(Name = "Ocjena")]
        [Required(ErrorMessage = "Vrijednost ocjene je obavezna")]
        [Range(1, 5, ErrorMessage = "Ocjena mora biti u rasponu 1 do 5")]
        public int Vrijednost { get; set; }

        public int PredmetId { get; set; }
    }
}
