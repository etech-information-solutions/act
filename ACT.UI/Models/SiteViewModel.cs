using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using ACT.Core.Enums;
using ACT.Core.Services;
using ACT.Data.Models;
namespace ACT.UI.Models
{
    public class SiteViewModel
    {
        #region Properties

        public int Id { get; set; }

        [Display( Name = "Main Site" )]
        public int? SiteId { get; set; }

        [Required]
        [Display( Name = "Region" )]
        public int? RegionId { get; set; }

        [Required]
        [Display( Name = "Site Name" )]
        [StringLength( 50, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 3 )]
        public string Name { get; set; }

        [Required]
        [Display( Name = "Description" )]
        [StringLength( 500, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 3 )]
        public string Description { get; set; }

        [Display( Name = "Longitude" )]
        [StringLength( 50, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 1 )]
        public string Longitude { get; set; }

        [Display( Name = "Latitude" )]
        [StringLength( 50, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 1 )]
        public string Latitude { get; set; }

        [Display( Name = "Contact Number" )]
        [StringLength( 50, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 1 )]
        public string ContactNo { get; set; }

        [Display( Name = "Contact Name" )]
        [StringLength( 50, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 1 )]
        public string ContactName { get; set; }

        [Display( Name = "Planning Point" )]
        [StringLength( 50, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 1 )]
        public string PlanningPoint { get; set; }

        [Required]
        public SiteType SiteType { get; set; }

        [Display( Name = "Account Code" )]
        [StringLength( 50, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 1 )]
        public string AccountCode { get; set; }

        [Display( Name = "Depot" )]
        [StringLength( 50, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 1 )]
        public string Depot { get; set; }

        [Display( Name = "Chep Site Code" )]
        [StringLength( 50, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 1 )]
        public string SiteCodeChep { get; set; }

        [Required]
        [Display( Name = "Status" )]
        public Status Status { get; set; }

        [Display( Name = "Finance Contact Number" )]
        [StringLength( 50, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 1 )]
        public string FinanceContactNo { get; set; }

        [Display( Name = "Finance Contact Name" )]
        [StringLength( 50, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 1 )]
        public string FinanceContact { get; set; }

        [Display( Name = "Receiving Contact Number" )]
        [StringLength( 50, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 1 )]
        public string ReceivingContactNo { get; set; }

        [Display( Name = "Receiving Contact Name" )]
        [StringLength( 50, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 1 )]
        public string ReceivingContact { get; set; }

        public bool EditMode { get; set; }

        public List<ClientCustomer> Clients { get; set; }

        public AddressViewModel Address { get; set; }

        public List<SiteBudget> SiteBudgets { get; set; }

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

        #endregion
    }
}