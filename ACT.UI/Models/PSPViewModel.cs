using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ACT.Core.Enums;
using ACT.Core.Services;
using ACT.Data.Models;

namespace ACT.UI.Models
{
    public class PSPViewModel
    {
        #region Properties

        public int Id { get; set; }

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
        [Display( Name = "Administrator Name" )]
        [StringLength( 200, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 1 )]
        public string AdminPerson { get; set; }

        [Required]
        [Display( Name = "Administrator Email Address" )]
        [StringLength( 200, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 1 )]
        public string AdminEmail { get; set; }


        [Required]
        [Display( Name = " Financial Person Name" )]
        [StringLength( 200, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 1 )]
        public string FinancialPerson { get; set; }

        [Required]
        [Display( Name = "Financial Person Email" )]
        [StringLength( 200, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 1 )]
        public string FinPersonEmail { get; set; }

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

        [Required]
        [Display( Name = "Type of Pallet Use" )]
        public TypeOfPalletUse TypeOfPalletUse { get; set; }

        [Display( Name = "Your Type of Pallet Use" )]
        public string OtherTypeOfPalletUse { get; set; }

        [Required]
        [Display( Name = "Company Type" )]
        public CompanyType CompanyType { get; set; }

        [Display( Name = "Status" )]
        public PSPClientStatus Status { get; set; }

        public List<PSPBudget> PSPBudgets { get; set; }

        public AddressViewModel Address { get; set; }

        public List<FileViewModel> Files { get; set; }

        [Display( Name = "BBBEE Level" )]
        [StringLength( 50, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 1 )]
        public string BBBEELevel { get; set; }

        [Display( Name = "Number Of Pallets Lost" )]
        public int? NumberOfLostPallets { get; set; }

        [Display( Name = "Select a reason why you're declining this PSP" )]
        public string DeclineReason { get; set; }

        public UserViewModel User { get; set; }

        #endregion

        #region Model Options

        public List<string> DeclineReasonOptions
        {
            get
            {
                if ( Status == PSPClientStatus.Unverified )
                {
                    using ( DeclineReasonService dservice = new DeclineReasonService() )
                    {
                        return dservice.ListByColumn( "Description", "Status", ( int ) Core.Enums.Status.Active );
                    }
                }

                return new List<string>();
            }
        }

        #endregion
    }
}
