using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using ACT.Core.Enums;
using ACT.Core.Services;

namespace ACT.UI.Models
{
    public class AuthorisationCodeViewModel
    {
        #region Properties

        public int Id { get; set; }

        [Required]
        [Display( Name = "Client Site" )]
        public int ClientSiteId { get; set; }

        [Required]
        [Display( Name = "Transporter" )]
        public int TransporterId { get; set; }

        [Display( Name = "Code" )]
        [StringLength( 50, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 0 )]
        public string Code { get; set; }

        [Required]
        [Display( Name = "Authorisation Date" )]
        public DateTime? AuthorisationDate { get; set; }

        [Required]
        [Display( Name = "Load Number" )]
        [StringLength( 50, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 0 )]
        public string LoadNumber { get; set; }

        [Required]
        [Display( Name = "Docket Number" )]
        [StringLength( 150, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 0 )]
        public string DocketNumber { get; set; }

        [Required]
        [Display( Name = "Status" )]
        public Status Status { get; set; }

        public bool EditMode { get; set; }

        #endregion



        #region Model Options

        public Dictionary<int, string> ClientSiteOptions
        {
            get
            {
                using ( ClientSiteService sservice = new ClientSiteService() )
                {
                    return sservice.List( true );
                }
            }
        }

        public Dictionary<int, string> TransporterOptions
        {
            get
            {
                using ( TransporterService tservice = new TransporterService() )
                {
                    return tservice.List( true );
                }
            }
        }

        #endregion
    }
}