using System.ComponentModel.DataAnnotations;

namespace PortalOgloszeniowy.Models.ViewModels
{
    public class RegisterViewModel
    {
        [Required]
        [Display(Name = "Imię")]
        public string FirstName { get; set; }
        [Required]
        [Display(Name = "Nazwisko")]
        public string LastName { get; set; }
        [EmailAddress]
        [Required(ErrorMessage = "Adres e-mail jest wymagany.")]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Hasło ")]
        [StringLength(100, ErrorMessage = "{0} musi zawierać co najmniej {2} znaki.", MinimumLength = 6)]
        public string Password { get; set; }
        [DataType(DataType.Password)]
        [Display(Name = "Potwierdź hasło ")]
        [Compare("Password", ErrorMessage = "Hasła różnią się od siebie.")]
        public string ConfirmPassword { get; set; }

        [DataType(DataType.PhoneNumber)]
        [Display(Name = "Numer Telefonu ")]
        public string PhoneNumber { get; set; }
    }
}
