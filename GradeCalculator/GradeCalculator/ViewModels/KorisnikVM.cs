using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace GradeCalculator.ViewModels
{
    public class KorisnikVM
    {
        public int Id { get; set; }

        [Display(Name = "Korisničko ime")]
        [Required(ErrorMessage = "User name is required")]
        public string UserName { get; set; }

        [Display(Name = "E-pošta")]
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Provide a correct e-mail address")]
        public string Email { get; set; }

        [Display(Name = "Lozinka")]
        [Required(ErrorMessage = "Password is required")]
        [StringLength(256, MinimumLength = 8, ErrorMessage = "Password should be at least 8 characters long")]
        public string Password { get; set; }

        [Display(Name = "Ukupna ocjena")]
        public double TotalGrade { get; set; }

        [Display(Name = "Uloga")]
        public int RoleId { get; set; }
    }
}
