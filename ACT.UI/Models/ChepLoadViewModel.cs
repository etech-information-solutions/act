using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;

using ACT.Core.Enums;
using ACT.Core.Services;

namespace ACT.UI.Models
{
    public class ChepLoadViewModel
    {
        #region Properties

        public int Id { get; set; }

        [Required]
        [Display( Name = "Client" )]
        public int ClientId { get; set; }

        [Display( Name = "Posting Type" )]
        public PostingType PostingType { get; set; }

        [Display( Name = "Chep Status" )]
        [StringLength( 50, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 0 )]
        public string ChepStatus { get; set; }

        [Display( Name = "Transaction Type" )]
        [StringLength( 50, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 0 )]
        public string TransactionType { get; set; }

        [Display( Name = "Docket Number" )]
        [StringLength( 50, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 0 )]
        public string DocketNumber { get; set; }

        [Display( Name = "Original Docket Number" )]
        [StringLength( 50, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 0 )]
        public string OriginalDocketNumber { get; set; }

        [Display( Name = "UMI" )]
        [StringLength( 50, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 0 )]
        public string UMI { get; set; }

        [Display( Name = "Location Id" )]
        [StringLength( 50, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 0 )]
        public string LocationId { get; set; }

        [Display( Name = "Location" )]
        [StringLength( 200, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 0 )]
        public string Location { get; set; }

        [Display( Name = "Other Party Id" )]
        [StringLength( 50, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 0 )]
        public string OtherPartyId { get; set; }

        [Display( Name = "Other Party" )]
        [StringLength( 50, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 0 )]
        public string OtherParty { get; set; }

        [Display( Name = "Other Party Country" )]
        [StringLength( 50, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 0 )]
        public string OtherPartyCountry { get; set; }

        [Display( Name = "Equipment Code" )]
        [StringLength( 10, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 0 )]
        public string EquipmentCode { get; set; }

        [Display( Name = "Equipment" )]
        [StringLength( 50, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 0 )]
        public string Equipment { get; set; }

        [Display( Name = "Quantity" )]
        public int? Quantity { get; set; }

        [Display( Name = "Ref #" )]
        [StringLength( 50, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 0 )]
        public string Reference { get; set; }

        [Display( Name = "Other Ref #" )]
        [StringLength( 50, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 0 )]
        public string OtherRef { get; set; }

        [Display( Name = "Batch Ref #" )]
        [StringLength( 50, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 0 )]
        public string BatchRef { get; set; }

        [Display( Name = "Shipment Date" )]
        public DateTime? ShipmentDate { get; set; }

        [Display( Name = "Delivery Date" )]
        public DateTime? DeliveryDate { get; set; }

        [Display( Name = "Effective Date" )]
        public DateTime? EffectiveDate { get; set; }

        [Display( Name = "Create Date" )]
        public DateTime? CreateDate { get; set; }

        [Display( Name = "Create By" )]
        [StringLength( 50, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 0 )]
        public string CreatedBy { get; set; }

        [Display( Name = "Invoice #" )]
        [StringLength( 50, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 0 )]
        public string InvoiceNumber { get; set; }

        [Display( Name = "Reason" )]
        [StringLength( 100, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 0 )]
        public string Reason { get; set; }

        [Display( Name = "Data Source" )]
        [StringLength( 10, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 0 )]
        public string DataSource { get; set; }

        [Display( Name = "Balance Status" )]
        public int? BalanceStatus { get; set; }

        public bool EditMode { get; set; }

        [Display( Name = "Import Loads" )]
        public HttpPostedFileBase File { get; set; }

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