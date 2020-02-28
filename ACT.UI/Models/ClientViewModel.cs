﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ACT.Core.Enums;
using ACT.Core.Services;
using ACT.Data.Models;
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
        [Display(Name = "Company Name")]
        [StringLength(200, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 1)]
        public string CompanyName { get; set; }

        [Required]
        [Display(Name = "Trading As")]
        [StringLength(200, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 1)]
        public string TradingAs { get; set; }
        [Display(Name = "Description")]
        [StringLength(500, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 1)]
        public string Description { get; set; }
        [Display(Name = "VAT Number")]
        [StringLength(50, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 1)]
        public string VATNumber { get; set; }
        [Display(Name = "Business Contact Number")]
        [StringLength(50, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 1)]
        public string ContactNumber { get; set; }
        [Required]
        [Display(Name = "Contact Person")]
        [StringLength(200, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 1)]
        public string ContactPerson { get; set; }
        public string FinancialPerson { get; set; }
        [Required]
        [Display(Name = "Administrator Email Address")]
        [StringLength(200, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 1)]
        public string AdminEmail { get; set; }

        [Required]
        [Display(Name = "Email Address")]
        [StringLength(200, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 1)]
        public string Email { get; set; }

        [Display(Name = "Select a reason why you're declining this PSP")]
        public string DeclinedReason { get; set; }

        [Required]
        [Display(Name = "Service Required")]
        public int ServiceType { get; set; }

        [Display(Name = "Status")]
        public int Status { get; set; }

        [Display(Name = "Estimated Loads Per Month")]
        public EstimatedLoadViewModel ClientBudget { get; set; }

        public AddressViewModel Address { get; set; }

        [Display(Name = "Company Documents")]
        public ICollection<FileViewModel> CompanyFile { get; set; }

        //[Display(Name = "Select a reason why you're declining this Client")]
        //public string DeclineReason { get; set; }

        public bool EditMode { get; set; }

        #endregion


        #region Model Options


        #endregion
    }
}