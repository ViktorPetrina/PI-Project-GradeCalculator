using System.ComponentModel.DataAnnotations;

namespace GradeCalculator.ViewModels
{
    public class AutentikacijaVM
    {
        [Required(ErrorMessage = "User name is required")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [StringLength(256, MinimumLength = 8, ErrorMessage = "Password should be at least 8 characters long")]
        public string Password { get; set; }

        public string ReturnUrl { get; set; }
    }

}

