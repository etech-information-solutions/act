using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;

using ACT.Core.Enums;
using ACT.Core.Services;
using ACT.Data.Models;

namespace ACT.UI.Models
{
    public class SiteViewModel
    {
        #region Properties

        [Required]
        [Display( Name = "Client" )]
        public int ClientId { get; set; }

        public int Id { get; set; }

        [Display( Name = "Main Site (Optional: Only make a selection if you're adding a subsite)" )]
        public int? SiteId { get; set; }

        [Required]
        [Display( Name = "Region" )]
        public int? RegionId { get; set; }

        [Display( Name = "ARPM Sales Manager" )]
        public int? ARPMSalesManagerId { get; set; }

        [Required]
        [Display( Name = "Site Name" )]
        [StringLength( 150, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 3 )]
        public string Name { get; set; }

        [Required]
        [Display( Name = "Description" )]
        [StringLength( 500, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 3 )]
        public string Description { get; set; }

        [Display( Name = "Longitude" )]
        [StringLength( 150, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 1 )]
        public string Longitude { get; set; }

        [Display( Name = "Latitude" )]
        [StringLength( 150, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 1 )]
        public string Latitude { get; set; }

        [Display( Name = "Contact Number" )]
        [StringLength( 150, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 1 )]
        public string ContactNo { get; set; }

        [Display( Name = "Contact Name" )]
        [StringLength( 150, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 1 )]
        public string ContactName { get; set; }

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

        [Required]
        [Display( Name = "Status" )]
        public Status Status { get; set; }

        [Display( Name = "Finance Contact Number" )]
        [StringLength( 150, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 1 )]
        public string FinanceContactNo { get; set; }

        [Display( Name = "Finance Contact Name" )]
        [StringLength( 150, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 1 )]
        public string FinanceContact { get; set; }

        [Display( Name = "Receiving Contact Number" )]
        [StringLength( 150, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 1 )]
        public string ReceivingContactNo { get; set; }

        [Display( Name = "Receiving Contact Name" )]
        [StringLength( 150, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 1 )]
        public string ReceivingContact { get; set; }

        [Display( Name = "Receiving Email Address" )]
        [StringLength( 150, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 1 )]
        public string ReceivingEmail { get; set; }

        [Display( Name = "Depot Manager" )]
        [StringLength( 150, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 1 )]
        public string DepotManager { get; set; }

        [Display( Name = "Depot Manager Email" )]
        [StringLength( 150, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 1 )]
        public string DepotManagerEmail { get; set; }

        [Display( Name = "Depot Manager Contact" )]
        [StringLength( 150, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 1 )]
        public string DepotManagerContact { get; set; }

        [Display( Name = "Finance Email" )]
        [StringLength( 150, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 1 )]
        public string FinanceEmail { get; set; }

        [Display( Name = "Location Number" )]
        [StringLength( 150, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 1 )]
        public string LocationNumber { get; set; }

        [Display( Name = "CL Code" )]
        [StringLength( 150, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 1 )]
        public string CLCode { get; set; }

        

        [Display( Name = "Client Sales Manager" )]
        [StringLength( 150, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 1 )]
        public string ClientSalesManager { get; set; }

        [Display( Name = "Client Manager Contact" )]
        [StringLength( 150, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 1 )]
        public string ClientManagerContact { get; set; }

        [Display( Name = "Client Manager Email" )]
        [StringLength( 150, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 1 )]
        public string ClientManagerEmail { get; set; }

        [Display( Name = "Client Sales Rep Contact" )]
        [StringLength( 150, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 1 )]
        public string ClientSalesRepContact { get; set; }

        [Display( Name = "Client Sales Rep Email" )]
        [StringLength( 150, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 1 )]
        public string ClientSalesRegEmail { get; set; }

        [Display( Name = "Authorisation Email 1" )]
        [StringLength( 150, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 1 )]
        public string AuthorisationEmail1 { get; set; }

        [Display( Name = "Authorisation Email 2" )]
        [StringLength( 150, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 1 )]
        public string AuthorisationEmail2 { get; set; }

        [Display( Name = "Authorisation Email 3" )]
        [StringLength( 150, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 1 )]
        public string AuthorisationEmail3 { get; set; }



        public bool EditMode { get; set; }

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

        public Dictionary<int, string> ClientCustomerOptions
        {
            get
            {
                if ( !EditMode ) return null;

                using ( ClientCustomerService cservice = new ClientCustomerService() )
                {
                    return cservice.List( true );
                }
            }
        }

        public Dictionary<int, string> RegionOptions
        {
            get
            {
                if ( !EditMode ) return null;

                using ( RegionService rservice = new RegionService() )
                {
                    return rservice.List( true );
                }
            }
        }

        public Dictionary<int, string> ARPMSalesManagerOptions
        {
            get
            {
                if ( !EditMode ) return null;

                using ( UserService rservice = new UserService() )
                {
                    return rservice.List( true, RoleType.ARPMSalesManager );
                }
            }
        }


        #endregion
    }
}
