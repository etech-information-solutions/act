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
        public int UID { get; set; }

        //[Required]
        [Display( Name = "CLIENT:" )]
        public int ClientId { get; set; }

        //[Required]
        [Display( Name = "VEHICLE ID:" )]
        public int? VehicleId { get; set; }

        [Required]
        [Display( Name = "VEHICLE REG#:" )]
        public string VehicleRegistration { get; set; }

        [Display( Name = "SUPPLIER FROM:" )]
        public int? ClientSiteId { get; set; }
        public ClientSite FromClientSiteName { get; set; }

        [Display( Name = "CUSTOMER TO:" )]
        public int? ClientSiteIdTo { get; set; }

        [Display( Name = "REGION FROM:" )]
        public int? RegionFromId { get; set; }

        [Display( Name = "REGION FROM:" )]
        public string RegionFromName { get; set; }

        [Display( Name = "REGION TO:" )]
        public int? RegionToId { get; set; }

        [Display( Name = "REGION TO:" )]
        public string RegionToName { get; set; }


        [Display( Name = "TRANSPORTER:" )]
        public int? TransporterId { get; set; }
        public string TransporterName { get; set; }

        [Display( Name = "OUT REASON ID:" )]
        public int? OutstandingReasonId { get; set; }

        [Display( Name = "LOAD COMMENT:" )]
        public int? PODCommentId { get; set; }

        [Required]
        [Display( Name = "LOAD NUMBER:" )]
        [StringLength( 50, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 0 )]
        public string LoadNumber { get; set; }

        [Required]
        [Display( Name = "LOAD DATE:" )]
        public DateTime? LoadDate { get; set; }

        //[Required]
        [Display( Name = "GRV DATE:" )]
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
        [Display( Name = "DEL NOTE #:" )]
        [StringLength( 50, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 0 )]
        public string DeliveryNote { get; set; }

        //[Required]
        [Display( Name = "OTHER REF #:" )]
        [StringLength( 50, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 0 )]
        public string ReferenceNumber { get; set; }

        //[Required]
        [Display( Name = "RECEIVER #:" )]
        [StringLength( 50, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 0 )]
        public string ReceiverNumber { get; set; }

        [Display( Name = "EQUIPMENT:" )]
        [StringLength( 50, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 0 )]
        public string Equipment { get; set; }

        //[Required]
        [Display( Name = "ORIGINAL QTY:" )]
        public decimal? OriginalQuantity { get; set; }

        [Display( Name = "NEW QTY:" )]
        public decimal? NewQuantity { get; set; }

        [Display( Name = "RECONCILE INV:" )]
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
        [Display( Name = "STATUS" )]
        public LoadStatus Status { get; set; }

        [Display( Name = "THAN:" )]
        [StringLength( 50, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 0 )]
        public string THAN { get; set; }

        [Display( Name = "INVOICE #:" )]
        [StringLength( 50, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 0 )]
        public string ChepInvoiceNo { get; set; }

        [Display( Name = "CHEP COMP #:" )]
        [StringLength( 50, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 0 )]
        public string ChepCompensationNo { get; set; }

        [Display( Name = "CANCELLED REASON:" )]
        [StringLength( 500, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 0 )]
        public string CancelledReason { get; set; }

        [Display( Name = "PCN COMMENTS:" )]
        public string PCNComments { get; set; }

        [Display( Name = "PRN COMMENTS:" )]
        public string PRNComments { get; set; }

        [Display( Name = "PALLET NOTES:" )]
        [StringLength( 250, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 0 )]
        public string ClientLoadNotes { get; set; }

        [Display( Name = "DEBRIEF DOC #:" )]
        [StringLength( 150, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 0 )]
        public string DebriefDocketNo { get; set; }

        [Display( Name = "Select a Date Format being used in the file your import file" )]
        public DateFormats DateFormats { get; set; }

        [Display( Name = "CHEP NUM/GLID:" )]
        public string GLID { get; set; }

        [Display( Name = "GRV NUMBER:" )]
        [StringLength( 250 )]
        public string GRVNumber { get; set; }

        [Display( Name = "ACT DOC NO:" )]
        public string ActControlDocNo { get; set; }

        [Display( Name = "CHEP DOC NO:" )]
        public string ChepExchangeDocNo { get; set; }

        [Display( Name = "DEBTORS CODE:" )]
        public string DebtorsCode { get; set; }

        [Display( Name = "FLEET NUMBER:" )]
        public string FleetNumber { get; set; }

        [Display( Name = "STO NUMBER:" )]
        public string DepotStoNumber { get; set; }

        [Display( Name = "CHEP COMP DATE:" )]
        public DateTime? ChepCompensationDate { get; set; }



        [Display( Name = "IMPORT LOADS" )]
        public HttpPostedFileBase File { get; set; }

        public List<FileViewModel> Files { get; set; }

        public bool EditMode { get; set; }

        public bool HasPOD { get; set; }

        public bool HasDisputes { get; set; }

        #endregion



        #region Chepload Properties

        [Display( Name = "DELIVERY DATE:" )]
        public DateTime? DeliveryDate { get; set; }

        [Display( Name = "CHEP NO/GLID:" )]
        public string ChepAccountNumberGlid { get; set; }

        [Display( Name = "ORDER #:" )]
        public string ChepRef { get; set; }

        [Display( Name = "OTHER REFERENCE:" )]
        public string ChepOtherRef { get; set; }

        [Display( Name = "CHEP INV #:" )]
        public string ChepInvoiceNumber { get; set; }

        [Display( Name = "RETURN SLIP #:" )]
        public string PalletReturnSlipNo { get; set; }

        public string DocketNumber { get; set; }

        [Display( Name = "THAN DOC #:" )]
        public string ChepCustomerThanDocNo { get; set; }

        [Display( Name = "WAREHOUSE #:" )]
        public string WarehouseTransferDocNo { get; set; }

        [Display( Name = "RETURN DATE:" )]
        public DateTime? PalletReturnDate { get; set; }

        public DateTime? ChepEffectiveDate { get; set; }


        [Display( Name = "REGION FROM:" )]
        public string RegionFrom { get; set; }

        [Display( Name = "REGION TO:" )]
        public string RegionTo { get; set; }

        [Display( Name = "CUST GROUP:" )]
        public int? ClientGroupId { get; set; }

        [Display( Name = "SALES ORD #:" )]
        public string OrderNumber { get; set; }

        [Required]
        [Display( Name = "PRIM/SEC" )]
        public LoadType? PrimarySecondary { get; set; }

        [Display( Name = "LOAD TYPE:" )]
        public CommentLoadType? LoadCategory { get; set; }

        public CommentLoadType ComputedLoadCategory
        {
            get
            {
                if ( CustomerType?.ToUpper()?.Contains( "EXCHANGE" ) == true ||
                    ReceiverNumber?.StartsWith( "50000" ) == true ||
                    ReceiverNumber?.StartsWith( "52" ) == true ||
                    ReceiverNumber?.StartsWith( "51" ) == true )
                {
                    return CommentLoadType.PCN;
                }
                else if ( DocumentType == DocumentType.ACTControlDoc )
                {
                    return CommentLoadType.Other;
                }
                return CommentLoadType.THAN;
            }
        }

        [Display( Name = "DEP STO NO:" )]
        public string DepoSTONo { get; set; }

        [Display( Name = "CUST ACC #:" )]
        public string CustomerAccountNumber { get; set; }

        [Display( Name = "CUST ORDER #:" )]
        public string CustomerOrderNumber { get; set; }

        [Display( Name = "LOAD SHEET #:" )]
        public string LoadsheetNo { get; set; }

        [Display( Name = "ACT DOC #:" )]
        public string DocNumber { get; set; }

        [Required]
        [Display( Name = "CHEP DOC #:" )]
        public string ExchangeNo { get; set; }

        [Display( Name = "AUTH CODE:" )]
        public string AuthorizationCode { get; set; }

        [Display( Name = "AUTH BY:" )]
        public string AuthorizedBy { get; set; }

        [Display( Name = "CHEP COMP DATE:" )]
        public DateTime? CompensationDate { get; set; }

        public virtual ClientGroup ClientGroup { get; set; }


        [Display( Name = "DOCUMENT TYPE:" )]
        public DocumentType DocumentType { get; set; }

        public ExtendedClientLoad ExtendedClientLoad { get; set; }

        public List<ClientLoadQuantity> ClientLoadQuantities { get; set; }

        public string CustomerType { get; set; }

        public Dictionary<int, string> SupplierSiteOptions { get; set; }

        public Dictionary<int, string> CustomerSiteOptions { get; set; }

        public Dictionary<int, string> ClientGroupOptions { get; set; }

        public Dictionary<int, string> TransporterOptions { get; set; }

        public List<EquipmentDetailViewModel> EquipmentDetails { get; set; }

        public Dictionary<int, string> EquipmentCodeOptions { get; set; }

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
        [Display( Name = "EQUIPMENT: " )]
        public int? ProductId { get; set; }

        [Display( Name = "DELIVERY QTY: " )]
        public decimal DeliveredQty { get; set; }

        [Display( Name = "RETURNED QTY: " )]
        public decimal ReturnedTransferredQty { get; set; }

        [Display( Name = "DEBRIEF QTY: " )]
        public decimal DebriefQty { get; set; }

        [Display( Name = "TRANSPORTER: " )]
        public decimal TransporterLiable { get; set; }

        [Display( Name = "ADM MOVEMENT: " )]
        public decimal AdminMovement { get; set; }

        [Display( Name = "OUT QTY: " )]
        public decimal OutstandingQtyAtCustomer { get; set; }

        public Dictionary<int, string> ProductOptions { get; set; }

        public string ProductName { get; set; }
    }

    public enum LoadType
    {
        Primary = 1,
        Secondary = 2
    }

    public enum CommentLoadType
    {
        Other = 0,
        PCN = 1,
        THAN = 2
    }
}