using System.ComponentModel.DataAnnotations;

namespace PortalOgloszeniowy.Models.ViewModels
{
    public class LoginViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Hasło")]
        public string Password { get; set; }
        [Display(Name = "Nie wylogowuj mnie.")]
        public bool RememberMe { get; set; }
    }
}
