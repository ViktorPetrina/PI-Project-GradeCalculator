using System.ComponentModel.DataAnnotations;

namespace GradeCalculator.ViewModels
{
    public class ShowKorisnikVM
    {
        public int Id { get; set; }

        [Display(Name = "Korisničko ime")]
        [Required(ErrorMessage = "User name is required")]
        public string UserName { get; set; }

        [Display(Name = "E-pošta")]
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Provide a correct e-mail address")]
        public string Email { get; set; }

        [Display(Name = "Ukupna ocjena")]
        public double TotalGrade { get; set; }

        [Display(Name = "Uloga")]
        public string RoleName { get; set; }
    }
}
