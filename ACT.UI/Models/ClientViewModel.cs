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

        [Display( Name = "PSP" )]
        public int? PSPId { get; set; }

        public PSP PSP { get; set; }

        [Required]
        [Display( Name = "Company Registration Number" )]
        [StringLength( 15, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 0 )]
        public string CompanyRegistrationNumber { get; set; }

        [Required]
        [Display( Name = "Company Name" )]
        [StringLength( 200, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 1 )]
        public string CompanyName { get; set; }

        [Required]
        [Display( Name = "Trading As" )]
        [StringLength( 200, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 1 )]
        public string TradingAs { get; set; }

        [Display( Name = "Description" )]
        [StringLength( 500, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 1 )]
        public string Description { get; set; }

        [Display( Name = "VAT Number" )]
        [StringLength( 50, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 1 )]
        public string VATNumber { get; set; }

        [Display( Name = "Chep Reference" )]
        [StringLength( 50, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 1 )]
        public string ChepReference { get; set; }

        [Display( Name = "Contact Number" )]
        [StringLength( 50, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 1 )]
        public string ContactNumber { get; set; }

        [Required]
        [Display( Name = "Contact Person" )]
        [StringLength( 200, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 1 )]
        public string ContactPerson { get; set; }

        [Required]
        [Display( Name = " Contact Person Email" )]
        [StringLength( 200, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 1 )]
        public string Email { get; set; }

        [Required]
        [Display( Name = "Administrator Name" )]
        [StringLength( 200, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 1 )]
        public string AdminPerson { get; set; }

        [Required]
        [Display( Name = "Administrator Email" )]
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


        [Display( Name = "Select a reason why you're declining this Client" )]
        public string DeclinedReason { get; set; }

        [Display( Name = "Service Required" )]
        public ServiceType ServiceType { get; set; }

        [Display( Name = "Status" )]
        public PSPClientStatus Status { get; set; }

        public List<ClientBudget> ClientBudgets { get; set; }

        public AddressViewModel Address { get; set; }

        public List<FileViewModel> Files { get; set; }


        [Display( Name = "Select a reason why you're declining this Client" )]
        public string DeclineReason { get; set; }

        public bool EditMode { get; set; }

        #endregion


        #region Model Options

        public Dictionary<int, string> PSPOptions
        {
            get
            {
                using ( PSPService pservice = new PSPService() )
                {
                    return pservice.List( true );
                }
            }
        }

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