﻿using System;
using System.ComponentModel.DataAnnotations;
using ACT.Core.Enums;

namespace ACT.UI.Models
{
    public class AddressViewModel
    {
        #region Properties

        public int Id { get; set; }

        public int ObjectId { get; set; }

        [Display( Name = "Same As Postal" )]
        public bool SameAsPostal { get; set; }

        [Required]
        [Display( Name = "Address line 1" )]
        [StringLength( 100, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 0 )]
        public string AddressLine1 { get; set; }
        
        [Display( Name = "Address line 2" )]
        [StringLength( 100, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 0 )]
        public string AddressLine2 { get; set; }

        [Display( Name = "Town" )]
        [StringLength( 50, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 0 )]
        public string Town { get; set; }

        [Display( Name = "Postal Code" )]
        [StringLength( 50, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 0 )]
        public string PostCode { get; set; }

        [Required]
        [Display( Name = "Select Province" )]
        public Province Province { get; set; }

        [Required]
        [Display( Name = "Address Type" )]
        public AddressType AddressType { get; set; }
        
        [Display( Name = "Status" )]
        public Status Status { get; set; }

        public bool EditMode { get; set; }

        #endregion

        #region Model Options



        #endregion
    }
}