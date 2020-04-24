using BlazorBoilerplate.Localization;
using System.ComponentModel.DataAnnotations;

namespace BlazorBoilerplate.Shared.Dto.Account
{
    public class RegisterDto
    {
        [Required]
        [StringLength(64, ErrorMessageResourceType = typeof(Strings), ErrorMessageResourceName = "ErrorInvalidLength", MinimumLength = 2)]
        [RegularExpression(@"[^\s]+", ErrorMessageResourceType = typeof(Strings), ErrorMessageResourceName = "SpacesNotPermitted")]
        [Display(Name = "UserName", ResourceType = typeof(Strings))]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessageResourceType = typeof(Strings), ErrorMessageResourceName = "ErrorInvalidLength", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Required]
        [Display(Name = "ConfirmPassword", ResourceType = typeof(Strings))]
        [Compare("Password", ErrorMessageResourceType = typeof(Strings), ErrorMessageResourceName = "PasswordConfirmationFailed")]
        public string PasswordConfirm { get; set; }

        [DataType(DataType.PhoneNumber)]
        [Phone]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        public string VerificationCode { get; set; }
    }
}