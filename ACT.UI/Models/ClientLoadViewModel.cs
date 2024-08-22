using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;

using ACT.Core.Enums;
using ACT.Core.Models;
using ACT.Core.Services;
using ACT.Data.Models;

using static iTextSharp.text.pdf.PdfDiv;

namespace ACT.UI.Models
{
    public class ClientLoadViewModel
    {
        #region Properties

        public int Id { get; set; }

        //[Required]
        [Display( Name = "CLIENT:" )]
        public int ClientId { get; set; }

        //[Required]
        [Display( Name = "VEHICLE REGISTRATION:" )]
        public int? VehicleId { get; set; }

        [Display( Name = "VEHICLE REGISTRATION:" )]
        public string VehicleRegistration { get; set; }

        [Display( Name = "SUPPLIER FROM:" )]
        public int? ClientSiteId { get; set; }
        public ClientSite FromClientSiteName { get; set; }

        [Display( Name = "CUSTOMER TO:" )]
        public int? ClientSiteIdTo { get; set; }

        [Display( Name = "REGION FROM:" )]
        public int? RegionFromId { get; set; }

        [Display( Name = "REGION TO:" )]
        public int? RegionToId { get; set; }


        [Display( Name = "TRANSPORTER NAME:" )]
        public int? TransporterId { get; set; }
        public string TransporterName { get; set; }

        [Display( Name = "OUTSTANDING REASON ID:" )]
        public int? OutstandingReasonId { get; set; }

        [Display( Name = "Load Comment:" )]
        public int? PODCommentId { get; set; }

        [Display( Name = "LOAD/SHIPMENT NUMBER:" )]
        [StringLength( 50, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 0 )]
        public string LoadNumber { get; set; }

        //[Required]
        [Display( Name = "LOAD DATE:" )]
        public DateTime? LoadDate { get; set; }

        //[Required]
        [Display( Name = "EFFECTIVE DATE:" )]
        public DateTime? EffectiveDate { get; set; }

        //[Required]
        [Display( Name = "NOTIFY DATE:" )]
        public DateTime? NotifyDate { get; set; }

        //[Required]
        [Display( Name = "ACCOUNT NUMBER:" )]
        [StringLength( 50, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 0 )]
        public string AccountNumber { get; set; }

        //[Required]
        [Display( Name = "CLIENT DESCRIPTION:" )]
        [StringLength( 50, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 0 )]
        public string ClientDescription { get; set; }

        //[Required]
        [Display( Name = "DELIVERY NOTE NUMBER:" )]
        [StringLength( 50, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 0 )]
        public string DeliveryNote { get; set; }

        //[Required]
        [Display( Name = "SALES ORDER NUMBER:" )]
        [StringLength( 50, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 0 )]
        public string ReferenceNumber { get; set; }

        //[Required]
        [Display( Name = "RECEIVER NUMBER:" )]
        [StringLength( 50, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 0 )]
        public string ReceiverNumber { get; set; }

        [Display( Name = "EQUIPMENT:" )]
        [StringLength( 50, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 0 )]
        public string Equipment { get; set; }

        //[Required]
        [Display( Name = "ORIGINAL QUANTITY:" )]
        public decimal? OriginalQuantity { get; set; }

        [Display( Name = "NEW QUANTITY:" )]
        public decimal? NewQuantity { get; set; }

        [Display( Name = "RECONCILE INVOICE:" )]
        public YesNo ReconcileInvoice { get; set; }

        [Display( Name = "RETURN DATE:" )]
        public DateTime? ReconcileDate { get; set; }

        [Display( Name = "POD NUMBER:" )]
        [StringLength( 50, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 0 )]
        public string PODNumber { get; set; }

        [Display( Name = "PCN:" )]
        [StringLength( 50, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 0 )]
        public string PCNNumber { get; set; }

        [Display( Name = "PRN:" )]
        [StringLength( 50, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 0 )]
        public string PRNNumber { get; set; }

        [Required]
        [Display( Name = "Status" )]
        public Status Status { get; set; }

        [Display( Name = "THAN:" )]
        [StringLength( 50, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 0 )]
        public string THAN { get; set; }

        [Display( Name = "RETURN QUANTITY:" )]
        public decimal? ReturnQty { get; set; }

        [Display( Name = "OUTSTANDING QUANTITY:" )]
        public decimal? OutstandingQty { get; set; }

        [Display( Name = "DEBRIEF QTY:" )]
        public decimal? DebriefQty { get; set; }

        [Display( Name = "AMIND MOVEMENT:" )]
        public decimal? AdminMovement { get; set; }

        [Display( Name = "TRANSPORT LIABLE:" )]
        public decimal TransporterLiableQty { get; set; }

        [Display( Name = "INVOICE NUMBER:" )]
        [StringLength( 50, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 0 )]
        public string ChepInvoiceNo { get; set; }

        [Display( Name = "CHEP COMPENSATION #:" )]
        [StringLength( 50, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 0 )]
        public string ChepCompensationNo { get; set; }

        [Display( Name = "Cancelled Reason:" )]
        [StringLength( 500, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 0 )]
        public string CancelledReason { get; set; }

        [Display( Name = "PCN Comments:" )]
        public string PCNComments { get; set; }

        [Display( Name = "PRN Comments:" )]
        public string PRNComments { get; set; }

        [Display( Name = "Pallet Notes:" )]
        [StringLength( 250, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 0 )]
        public string ClientLoadNotes { get; set; }

        [Display( Name = "DEBRIEF DOC NO:" )]
        [StringLength( 150, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 0 )]
        public string DebriefDocketNo { get; set; }

        [Display( Name = "Select a Date Format being used in the file your import file" )]
        public DateFormats DateFormats { get; set; }

        [Display( Name = "CHEP ACC NUM/GLID:" )]
        public string GLID { get; set; }

        [Display( Name = "GRV Number:" )]
        [StringLength( 250 )]
        public string GRVNumber { get; set; }

        [Display( Name = "ACT CONTROL DOC NO:" )]
        public string ActControlDocNo { get; set; }

        [Display( Name = "CHEP EXCHANGE DOC NO:" )]
        public string ChepExchangeDocNo { get; set; }

        [Display( Name = "DEBTORS CODE:" )]
        public string DebtorsCode { get; set; }

        [Display( Name = "FLEET NUMBER:" )]
        public string FleetNumber { get; set; }

        [Display( Name = "DEPOT STO NUMBER:" )]
        public string DepotStoNumber { get; set; }

        [Display( Name = "CHEP COMPENSATION DATE:" )]
        public DateTime? ChepCompensationDate { get; set; }



        [Display( Name = "Import Loads" )]
        public HttpPostedFileBase File { get; set; }

        public List<FileViewModel> Files { get; set; }

        public bool EditMode { get; set; }

        public bool HasPOD { get; set; }

        public bool HasDisputes { get; set; }

        #endregion



        #region Chepload Properties

        [Display( Name = "DELIVERY DATE:" )]
        public DateTime? DeliveryDate { get; set; }

        [Display( Name = "CHEP ACC NO/GLID:" )]
        public string ChepAccountNumberGlid { get; set; }

        [Display( Name = "Customer Order Number:" )]
        public string ChepRef { get; set; }

        [Display( Name = "Other Reference:" )]
        public string ChepOtherRef { get; set; }

        [Display( Name = "Chep Invoice Number:" )]
        public string ChepInvoiceNumber { get; set; }

        [Display( Name = "PALLET RETURN SLIP NO:" )]
        public string PalletReturnSlipNo { get; set; }

        public string DocketNumber { get; set; }

        [Display( Name = "THAN DOC NO:" )]
        public string ChepCustomerThanDocNo { get; set; }

        [Display( Name = "WAREHOUSE DOC NO:" )]
        public string WarehouseTransferDocNo { get; set; }

        [Display( Name = "PALLET RETURN DATE:" )]
        public DateTime? PalletReturnDate { get; set; }

        public DateTime? ChepEffectiveDate { get; set; }


        [Display( Name = "REGION FROM:" )]
        public string RegionFrom { get; set; }

        [Display( Name = "REGION TO:" )]
        public string RegionTo { get; set; }

        [Display( Name = "CUSTOMER GROUP:" )]
        public int? ClientGroupId { get; set; }

        [Display( Name = "SALES ORDER NUMBER:" )]
        public string OrderNumber { get; set; }

        [Required]
        [Display( Name = "PRIMARY SECONDARY" )]
        public LoadType? PrimarySecondary { get; set; }

        [Display( Name = "DEPOT STO NO:" )]
        public string DepoSTONo { get; set; }

        [Display( Name = "CUSTOMER ACCOUNT NUMBER:" )]
        public string CustomerAccountNumber { get; set; }

        [Display( Name = "CUSTOMER ORDER NUMBER:" )]
        public string CustomerOrderNumber { get; set; }

        [Display( Name = "LOAD SHEET NO:" )]
        public string LoadsheetNo { get; set; }

        [Display( Name = "ACT CONTROL DOC NO:" )]
        public string DocNumber { get; set; }

        [Required]
        [Display( Name = "CHEP EXCHANGE DOC NO:" )]
        public string ExchangeNo { get; set; }

        [Display( Name = "AUTHORIZATION CODE:" )]
        public string AuthorizationCode { get; set; }

        [Display( Name = "AUTHORIZED BY:" )]
        public string AuthorizedBy { get; set; }

        [Display( Name = "CHEP COMPENSATION DATE:" )]
        public DateTime? CompensationDate { get; set; }

        public virtual ClientGroup ClientGroup { get; set; }


        [Display( Name = "Document Type:" )]
        public DocumentType DocumentType { get; set; }

        public ExtendedClientLoad ExtendedClientLoad { get; set; }

        public List<ClientLoadQuantity> ClientLoadQuantities { get; set; }

        public string CustomerType { get; set; }

        public Dictionary<int, string> SupplierSiteOptions { get; set; } = new Dictionary<int, string>();

        public Dictionary<int, string> CustomerSiteOptions { get; set; } = new Dictionary<int, string>();

        public Dictionary<int, string> ClientGroupOptions { get; set; } = new Dictionary<int, string>();

        public Dictionary<int, string> TransporterOptions { get; set; } = new Dictionary<int, string>();

        public List<EquipmentDetailViewModel> EquipmentDetails { get; set; } = new List<EquipmentDetailViewModel>();

        public Dictionary<int, string> EquipmentCodeOptions { get; set; } = new Dictionary<int, string>();

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

        public Dictionary<int, string> VehicleOptions
        {
            //get; set;
            get
            {
                if ( !EditMode ) return null;

                using ( VehicleService service = new VehicleService() )
                {
                    return service.List( true );
                }
            }/**/
        }

        public Dictionary<int, string> ClientSiteOptions
        {
            //get; set;
            get
            {
                if ( !EditMode ) return null;

                using ( ClientSiteService service = new ClientSiteService() )
                {
                    return service.List( true, new PagingModel() { Sort = "ASC", SortBy = "s.Name" }, new CustomSearchModel() { ClientId = ClientId } );
                }
            }/**/
        }

        public Dictionary<int, string> OutstandingReasonOptions
        {
            get
            {
                if ( !EditMode ) return null;

                using ( OutstandingReasonService service = new OutstandingReasonService() )
                {
                    return service.List( true );
                }
            }
        }

        public Dictionary<int, string> PODCommentOptions
        {
            get
            {
                if ( !EditMode ) return null;

                using ( PODCommentService service = new PODCommentService() )
                {
                    return service.List( true );
                }
            }
        }

        #endregion
    }

    public class EquipmentDetailViewModel
    {
        [Display( Name = "Equipment" )]
        public int? ProductId { get; set; }

        [Display( Name = "Delivered Qty" )]
        public decimal DeliveredQty { get; set; }

        [Display( Name = "Returned/Transferred Qty" )]
        public decimal ReturnedTransferredQty { get; set; }

        [Display( Name = "Debrief Qty" )]
        public decimal DebriefQty { get; set; }

        [Display( Name = "Transporter Liable" )]
        public decimal TransporterLiable { get; set; }

        [Display( Name = "Admin Movement" )]
        public decimal AdminMovement { get; set; }

        [Display( Name = "Outstanding Qty" )]
        public decimal OutstandingQtyAtCustomer { get; set; }

        public Dictionary<int, string> ProductOptions { get; set; }
    }

    public enum LoadType
    {
        Primary = 1,
        Secondary = 2
    }
}