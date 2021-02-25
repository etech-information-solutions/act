using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using ACT.Core.Enums;
using ACT.Core.Services;

namespace ACT.UI.Models
{
    public class RegionViewModel
    {
        #region Properties

        public int Id { get; set; }

        [Required]
        [Display( Name = "PSP" )]
        public int PSPId { get; set; }

        [Required]
        [Display( Name = "Province" )]
        public Province Province { get; set; }

        [Display( Name = "Regional Manager" )]
        public int? RegionManagerId { get; set; }

        [Required]
        [Display( Name = "Name" )]
        [StringLength( 150, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 0 )]
        public string Name { get; set; }

        [Required]
        [Display( Name = "Description" )]
        [StringLength( 500, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 0 )]
        public string Description { get; set; }
        [Required]
        [Display( Name = "Status" )]
        public Status Status { get; set; }

        public bool EditMode { get; set; }

        #endregion



        #region Model Options

        public Dictionary<int, string> PSPOptions
        {
            get
            {
                if ( !EditMode ) return null;

                using ( PSPService pservice = new PSPService() )
                {
                    return pservice.List( true );
                }
            }
        }

        public Dictionary<int, string> RegionManagerOptions
        {
            get
            {
                if ( !EditMode ) return null;

                using ( UserService uservice = new UserService() )
                {
                    return uservice.List( true, RoleType.PSP );
                }
            }
        }

        #endregion
    }
}