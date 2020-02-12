using System.ComponentModel.DataAnnotations;

namespace ACT.UI.Models
{
    public class ForgotPasswordViewModel
    {
        [Required]
        [Display( Name = "Email Address" )]
        public string Email { get; set; }

        public string ReturnUrl { get; set; }
    }
}
