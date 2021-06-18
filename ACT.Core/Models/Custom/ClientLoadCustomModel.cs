
namespace ACT.Core.Models.Custom
{
    using System;
    using System.Collections.Generic;
    using ACT.Data.Models;

    public partial class ClientLoadCustomModel
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public int? VehicleId { get; set; }
        public int? ClientSiteId { get; set; }
        public int? TransporterId { get; set; }
        public int? ToClientSiteId { get; set; }
        public int? PODCommentId { get; set; }
        public int? OutstandingReasonId { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime ModifiedOn { get; set; }
        public string ModifiedBy { get; set; }
        public string LoadNumber { get; set; }
        public DateTime? LoadDate { get; set; }
        public DateTime? EffectiveDate { get; set; }
        public DateTime? NotifyDate { get; set; }
        public string AccountNumber { get; set; }
        public string ClientDescription { get; set; }
        public string DeliveryNote { get; set; }
        public string ReferenceNumber { get; set; }
        public string ReceiverNumber { get; set; }
        public string Equipment { get; set; }
        public decimal? OriginalQuantity { get; set; }
        public decimal? NewQuantity { get; set; }
        public bool ReconcileInvoice { get; set; }
        public DateTime? ReconcileDate { get; set; }
        public string PODNumber { get; set; }
        public string PCNNumber { get; set; }
        public string PRNNumber { get; set; }
        public int Status { get; set; }
        public int? PostingType { get; set; }
        public string THAN { get; set; }
        public decimal? ReturnQty { get; set; }
        public decimal? OutstandingQty { get; set; }
        public decimal? DebriefQty { get; set; }
        public decimal? AdminMovement { get; set; }
        public string ChepInvoiceNo { get; set; }
        public string ChepCompensationNo { get; set; }
        public int? InvoiceStatus { get; set; }
        public int? PODStatus { get; set; }
        public int? OrderStatus { get; set; }
        public string UID { get; set; }
        public decimal? TransporterLiableQty { get; set; }
        public string ClientLoadComment { get; set; }
        public string ClientLoadNotes { get; set; }
        public string CancelledReason { get; set; }
        public bool ManuallyMatchedLoad { get; set; }
        public string ManuallyMatchedUID { get; set; }
        public string PODComments { get; set; }
        public string PCNComments { get; set; }
        public string PRNComments { get; set; }
        public int? PODCommentById { get; set; }
        public int? PCNCommentById { get; set; }
        public int? PRNCommentById { get; set; }
        public DateTime? PODCommentDate { get; set; }
        public DateTime? PCNCommentDate { get; set; }
        public DateTime? PRNCommentDate { get; set; }
        public string DebriefDocketNo { get; set; }
        public string PalletReturnSlipNo { get; set; }
        public string ChepCustomerThanDocNo { get; set; }
        public string WarehouseTransferDocNo { get; set; }
        public DateTime? PalletReturnDate { get; set; }
        public string EquipmentCode { get; set; }
        public string GLID { get; set; }
        public string CustomerType { get; set; }
        public string OrderNumber { get; set; }

        public int? ImageCount { get; set; }
        public int? DocumentCount { get; set; }
        public List<Image> Images { get; set; }
        public List<Document> Documents { get; set; }

        public string DocketNumber { get; set; }
        public string InvoiceNumber { get; set; }
        public string ChepAccountGLIDNo { get; set; }

        public int? SiteId { get; set; }
        public int? ToSiteId { get; set; }
        public string SiteName { get; set; }
        public string ToSiteName { get; set; }
        public string ClientName { get; set; }
        public string SubSiteName { get; set; }
        public string OutstandingReason { get; set; }
        public string TransporterName { get; set; }
        public string VehicleRegistration { get; set; }
        public string PODComment { get; set; }

        public string PODCommentBy { get; set; }
        public string PCNCommentBy { get; set; }
        public string PRNCommentBy { get; set; }

        public int? TaskCount { get; set; }
        public int? JournalCount { get; set; }

        public decimal? ChepNewQuantity { get; set; }


        // Mobile API
        public string Email { get; set; }

        public string APIKey { get; set; }
    }
}
