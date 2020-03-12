using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ACT.Core.Enums;
using ACT.Core.Services;
using ACT.Data.Models;
namespace ACT.UI.Models
{
    public class TransporterViewModel
    {
        #region Properties

        public int Id { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public System.DateTime ModifiedOn { get; set; }
        public string ModifiedBy { get; set; }
        [Required]
        [Display(Name = "Name")]
        [StringLength(500, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 1)]
        public string Name { get; set; }
        public string ContactNumber { get; set; }
        [Required]
        [Display(Name = "Email")]
        [StringLength(100, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 1)]
        public string Email { get; set; }
        [Required]
        [Display(Name = "Trading Name")]
        [StringLength(100, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 1)]
        public string TradingName { get; set; }
        [Required]
        [Display(Name = "Registration Number")]
        [StringLength(20, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 1)]
        public string RegistrationNumber { get; set; }
        public int Status { get; set; }



        public bool EditMode { get; set; }

        #endregion


        #region Model Options


        #endregion
    }
}