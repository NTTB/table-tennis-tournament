using System.ComponentModel.DataAnnotations;

namespace TTT.Web.Services.Identity.AccountViewModels
{
    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
