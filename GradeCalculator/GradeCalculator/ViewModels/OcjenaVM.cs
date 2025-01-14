using GradeCalculator.Models;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace GradeCalculator.ViewModels
{
    public class OcjenaVM
    {
        [JsonIgnore]
        public int Idocjena { get; set; }

        [Display(Name = "Ocjena")]
        [Required(ErrorMessage = "Vrijednost ocjene je obavezna")]
        [Range(1, 5, ErrorMessage = "Ocjena mora biti u rasponu 1 do 5")]
        public int Vrijednost { get; set; }

        [JsonIgnore]
        public int PredmetId { get; set; }
    }
}
