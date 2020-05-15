using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ACT.Core.Enums;
using ACT.Core.Services;
using ACT.Data.Models;

namespace ACT.UI.Models
{
    public class RegisterViewModel
    {
        #region Properties

        public int Id { get; set; }

        [Display( Name = "Name of Service Provider" )]
        public int? PSPId { get; set; }

        [Required]
        [Display( Name = "Company Name" )]
        [StringLength( 200, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 1 )]
        public string CompanyName { get; set; }

        [Required]
        [Display( Name = "Trading As" )]
        [StringLength( 200, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 1 )]
        public string TradingAs { get; set; }

        [Display( Name = "Company Registration Number" )]
        [StringLength( 50, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 1 )]
        public string CompanyRegistrationNumber { get; set; }

        [Display( Name = "VAT Number" )]
        [StringLength( 50, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 1 )]
        public string VATNumber { get; set; }

        [Display( Name = "Description of business" )]
        [StringLength( 500, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 1 )]
        public string Description { get; set; }

        [Display( Name = "Business Contact Number" )]
        [StringLength( 50, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 1 )]
        public string ContactNumber { get; set; }

        [Required]
        [Display( Name = "Your Administrator Email Address" )]
        [StringLength( 200, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 1 )]
        public string AdminEmail { get; set; }

        [Required]
        [Display( Name = "Email Address" )]
        [StringLength( 200, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 1 )]
        public string EmailAddress { get; set; }

        [Required]
        [Display( Name = "Contact Person" )]
        [StringLength( 200, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 1 )]
        public string ContactPerson { get; set; }

        [Required]
        [Display( Name = "Service Required" )]
        public ServiceType ServiceType { get; set; }

        [Display( Name = "Estimated Loads Per Month" )]
        public EstimatedLoadViewModel EstimatedLoad { get; set; }

        public AddressViewModel Address { get; set; }

        [Display( Name = "Company Registration Document" )]
        public FileViewModel RegistrationFile { get; set; }

        [Display( Name = "I accept the Terms and Conditions" )]
        public bool IsAccpetedTC { get; set; }

        #endregion

        #region Model Options

        public List<PSP> PSPOptions
        {
            get
            {
                using ( PSPService pservice = new PSPService() )
                {
                    return pservice.List();
                }
            }
            set
            {
            }
        }

        #endregion
    }
}
