using System;
using System.ComponentModel.DataAnnotations;
using ACT.Core.Enums;

namespace ACT.UI.Models
{
    public class ClientViewModel
    {
        #region Properties

        public int Id { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public System.DateTime ModifiedOn { get; set; }
        public string ModifiedBy { get; set; }

        [Required]
        [Display(Name = "Company Registration Number")]
        [StringLength(15, ErrorMessage = "Only {15} characters are allowed for this field.", MinimumLength = 0)]
        public string CompanyRegistrationNumber { get; set; }

        [Required]
        [Display(Name = "Company name")]
        [StringLength(100, ErrorMessage = "Only {15} characters are allowed for this field.", MinimumLength = 0)]
        public string CompanyName { get; set; }
        public string TradingAs { get; set; }
        public string Description { get; set; }
        public string VATNumber { get; set; }
        public string ContactNumber { get; set; }
        public string ContactPerson { get; set; }
        public string FinancialPerson { get; set; }
        public string Email { get; set; }
        public string AdminEmail { get; set; }
        public string DeclinedReason { get; set; }

        [Required]
        [Display( Name = "Service Required")]
        public int ServiceRequired { get; set; }
        public int Status { get; set; }

        public bool EditMode { get; set; }

        #endregion

        #region Model Options



        #endregion
    }
}