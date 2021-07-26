using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using ACT.Core.Enums;
using ACT.Core.Services;
using ACT.Data.Models;

namespace ACT.UI.Models
{
    public class AddressViewModel
    {
        #region Properties

        public int Id { get; set; }

        public int ObjectId { get; set; }

        [Display( Name = "Select Province" )]
        public int ProvinceId { get; set; }
        public Province Province { get; set; }

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
        [Display( Name = "Address Type" )]
        public AddressType AddressType { get; set; }

        [Display( Name = "Status" )]
        public Status Status { get; set; }

        [Display( Name = "Longitude" )]
        [StringLength( 30, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 0 )]
        public string Longitude { get; set; }
        [Display( Name = "Latitude" )]
        [StringLength( 30, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 0 )]
        public string Latitude { get; set; }


        public bool EditMode { get; set; }

        #endregion

        #region Model Options

        public Dictionary<int, string> ProvinceOptions
        {
            get
            {
                using ( ProvinceService pservice = new ProvinceService() )
                {
                    return pservice.List( true );
                }
            }
        }

        #endregion
    }
}