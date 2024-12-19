using System.ComponentModel.DataAnnotations;

namespace GradeCalculator.ViewModels
{
    public class ChangePasswordVM
    {
        [Display(Name = "Nova lozinka")]
        [Required(ErrorMessage = "Password is required")]
        [StringLength(256, MinimumLength = 8, ErrorMessage = "Password should be at least 8 characters long")]
        public string NewPassword { get; set; }

        [Display(Name = "Potvrdi lozinku")]
        [Required(ErrorMessage = "Password is required")]
        [StringLength(256, MinimumLength = 8, ErrorMessage = "Password should be at least 8 characters long")]
        public string ConfirmPassword { get; set; }
    }
}
