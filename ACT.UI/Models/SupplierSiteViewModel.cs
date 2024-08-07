using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;

using ACT.Core.Enums;
using ACT.Core.Models.Custom;
using ACT.Core.Services;
using ACT.Data.Models;

namespace ACT.UI.Models
{
    public class SupplierSiteViewModel
    {
        #region Properties

        public int Id { get; set; }

        [Display( Name = "Client" )]
        public int ClientId { get; set; }

        [Required]
        [Display( Name = "Main Site" )]
        public string MainSite { get; set; }

        [Required]
        [Display( Name = "Description" )]
        [StringLength( 500, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 3 )]
        public string Description { get; set; }

        [Display( Name = "Planning Point" )]
        [StringLength( 150, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 1 )]
        public string PlanningPoint { get; set; }

        [Required]
        public SiteType SiteType { get; set; }

        [Display( Name = "Account Code" )]
        [StringLength( 150, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 1 )]
        public string AccountCode { get; set; }

        [Display( Name = "Customer No/Debtor Code" )]
        [StringLength( 150, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 1 )]
        public string CustomerNoDebtorCode { get; set; }

        [Display( Name = "Depot" )]
        [StringLength( 150, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 1 )]
        public string Depot { get; set; }

        [Display( Name = "Chep GLID No" )]
        [StringLength( 150, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 1 )]
        public string SiteCodeChep { get; set; }

        [Display( Name = "Location Number" )]
        [StringLength( 150, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 1 )]
        public string LocationNumber { get; set; }

        [Required]
        [Display( Name = "Status" )]
        public Status Status { get; set; }

        public bool EditMode { get; set; }

        [Display( Name = "Contacts" )]
        public List<Contact> Contacts { get; set; }

        public ClientSiteCustomModel ClientSite { get; set; }

        public List<ClientCustomer> Clients { get; set; }

        public AddressViewModel Address { get; set; }

        public List<SiteBudget> SiteBudgets { get; set; }

        [Display( Name = "Import Sites" )]
        public HttpPostedFileBase SiteImportFile { get; set; }

        #endregion



        #region Model Options

        public Dictionary<int, string> SiteOptions
        {
            get
            {
                if ( !EditMode ) return null;

                using ( SiteService sservice = new SiteService() )
                {
                    return sservice.List( true );
                }
            }
        }

        public Dictionary<int, string> ClientOptions
        {
            get
            {
                if ( !EditMode ) return null;

                using ( ClientService cservice = new ClientService() )
                {
                    if ( cservice.SelectedClient != null )
                    {
                        ClientId = cservice.SelectedClient.Id;
                    }

                    return cservice.List( true );
                }
            }
        }


        #endregion
    }
}
