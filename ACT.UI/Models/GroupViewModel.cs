using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using ACT.Core.Enums;
using ACT.Core.Services;

namespace ACT.UI.Models
{
    public class GroupViewModel
    {
        #region Properties

        public int Id { get; set; }

        [Required]
        [Display( Name = "Group Name" )]
        [StringLength( 50, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 0 )]
        public string Name { get; set; }

        [Required]
        [Display( Name = "Description" )]
        [StringLength( 500, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 1 )]
        public string Description { get; set; }

        [Required]
        [Display( Name = "Status" )]
        public Status Status { get; set; }

        public List<ClientGroupViewModel> Clients { get; set; }

        public bool EditMode { get; set; }

        #endregion



        #region Model Options

        public Dictionary<int, string> ClientOptions
        {
            get
            {
                if ( !EditMode ) return null;

                using ( ClientService cservice = new ClientService() )
                {
                    return cservice.List( true );
                }
            }
        }

        #endregion
    }
}