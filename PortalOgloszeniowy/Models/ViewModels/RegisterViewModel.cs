using System.ComponentModel.DataAnnotations;

namespace PortalOgloszeniowy.Models.ViewModels
{
    public class RegisterViewModel
    {
        //[Required]
       // public string Name { get; set; }
        [EmailAddress]
        [Required(ErrorMessage = "Adres e-mail jest wymagany.")]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Hasło ")]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        public string Password { get; set; }
        [DataType(DataType.Password)]
        [Display(Name = "Potwierdź hasło ")]
        [Compare("Password", ErrorMessage = "Hasła różnią się od siebie.")]
        public string ConfirmPassword { get; set; }

    }
}
