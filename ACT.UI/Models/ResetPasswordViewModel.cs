using System;
using System.ComponentModel.DataAnnotations;

namespace ACT.UI.Models
{
    public class ResetPasswordViewModel
    {
        [Required]
        [DataType( DataType.Password )]
        [Display( Name = "Password" )]
        public string Password { get; set; }

        [Required]
        [DataType( DataType.Password )]
        [Display( Name = "Confirm Password" )]
        public string ConfirmPassword { get; set; }

        public Guid UID { get; set; }

        public string ReturnUrl { get; set; }
    }
}
