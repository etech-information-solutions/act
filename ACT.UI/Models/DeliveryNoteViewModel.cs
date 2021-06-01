using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using ACT.Core.Enums;
using ACT.Core.Models;
using ACT.Core.Services;

namespace ACT.UI.Models
{
    public class DeliveryNoteViewModel
    {
        #region Properties

        public int Id { get; set; }

        [Required]
        [Display( Name = "Site" )]
        public int ClientSiteId { get; set; }

        [Required]
        [Display( Name = "Client" )]
        public int ClientId { get; set; }

        [Required]
        [Display( Name = "Vehicle" )]
        public int VehicleId { get; set; }

        [Required]
        [Display( Name = "Customer Name" )]
        [StringLength( 150, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 1 )]
        public string CustomerName { get; set; }

        [Display( Name = "Customer Address" )]
        [StringLength( 75, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 1 )]
        public string CustomerAddress1 { get; set; }

        [StringLength( 75, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 1 )]
        public string CustomerAddress2 { get; set; }

        [Display( Name = "Town" )]
        public string CustomerAddressTown { get; set; }

        [Display( Name = "Postal Code" )]
        [StringLength( 10, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 1 )]
        public string CustomerPostalCode { get; set; }

        [Display( Name = "Province" )]
        public Province CustomerProvince { get; set; }

        [Display( Name = "Contact Number" )]
        public string ContactNumber { get; set; }


        [Display( Name = "Delivery Address" )]
        [StringLength( 75, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 1 )]
        public string DeliveryAddress1 { get; set; }

        [StringLength( 75, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 1 )]
        public string DeliveryAddress2 { get; set; }

        [Display( Name = "Town" )]
        public string DeliveryAddressTown { get; set; }

        [Display( Name = "Postal Code" )]
        [StringLength( 10, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 1 )]
        public string DeliveryPostalCode { get; set; }

        [Display( Name = "Province" )]
        public Province DeliveryProvince { get; set; }


        [Display( Name = "Billing Address" )]
        [StringLength( 75, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 1 )]
        public string BillingAddress1 { get; set; }

        [StringLength( 75, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 1 )]
        public string BillingAddress2 { get; set; }

        [Display( Name = "Town" )]
        public string BillingAddressTown { get; set; }

        [Display( Name = "Postal Code" )]
        [StringLength( 10, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 1 )]
        public string BillingPostalCode { get; set; }

        [Display( Name = "Province" )]
        public Province BillingProvince { get; set; }


        [Display( Name = "Invoice Number" )]
        [StringLength( 50, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 1 )]
        public string InvoiceNumber { get; set; }

        [Required]
        [Display( Name = "Order Number" )]
        [StringLength( 50, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 1 )]
        public string OrderNumber { get; set; }

        [Required]
        [DataType( DataType.Date )]
        [Display( Name = "Order Date" )]
        public DateTime? OrderDate { get; set; }


        [Display( Name = "Reference" )]
        [StringLength( 50, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 1 )]
        public string Reference306 { get; set; }

        [Display( Name = "Status" )]
        public Status Status { get; set; }

        [Display( Name = "Email Address" )]
        [StringLength( 50, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 1 )]
        public string EmailAddress { get; set; }

        public bool Email { get; set; }

        public string EmailTo { get; set; }

        public bool Reprint { get; set; }

        public List<DeliveryNoteLineViewModel> DeliveryNoteLines { get; set; }

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
                    if ( cservice.SelectedClient != null )
                    {
                        ClientId = cservice.SelectedClient.Id;
                    }

                    return cservice.List( true );
                }
            }
        }

        private Dictionary<int, string> siteOptions;
        public Dictionary<int, string> SiteOptions
        {
            get
            {
                if ( ClientId > 0 )
                {
                    using ( ClientSiteService sservice = new ClientSiteService() )
                    {
                        siteOptions = sservice.List( true, new PagingModel(), new CustomSearchModel() { ClientId = ClientId } );
                    }
                }

                return siteOptions;
            }
            set
            {
                siteOptions = value;
            }
        }

        private Dictionary<int, string> vehicleOptions;
        public Dictionary<int, string> VehicleOptions
        {
            get
            {
                if ( ClientId > 0 )
                {
                    using ( VehicleService vservice = new VehicleService() )
                    {
                        vehicleOptions = vservice.List( true, ClientId, "Client" );
                    }
                }

                return vehicleOptions;
            }
            set
            {
                vehicleOptions = value;
            }
        }

        #endregion
    }
}
