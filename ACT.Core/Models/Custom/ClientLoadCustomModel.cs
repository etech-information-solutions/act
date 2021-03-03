
namespace ACT.Core.Models.Custom
{
    using System;
    using System.Collections.Generic;
    using ACT.Data.Models;

    public partial class ClientLoadCustomModel
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public Nullable<int> VehicleId { get; set; }
        public Nullable<int> ClientSiteId { get; set; }
        public Nullable<int> TransporterId { get; set; }
        public Nullable<int> ToClientSiteId { get; set; }
        public Nullable<int> OutstandingReasonId { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public System.DateTime ModifiedOn { get; set; }
        public string ModifiedBy { get; set; }
        public string LoadNumber { get; set; }
        public Nullable<System.DateTime> LoadDate { get; set; }
        public Nullable<System.DateTime> EffectiveDate { get; set; }
        public Nullable<System.DateTime> NotifyDate { get; set; }
        public string AccountNumber { get; set; }
        public string ClientDescription { get; set; }
        public string DeliveryNote { get; set; }
        public string ReferenceNumber { get; set; }
        public string ReceiverNumber { get; set; }
        public string Equipment { get; set; }
        public Nullable<decimal> OriginalQuantity { get; set; }
        public Nullable<decimal> NewQuantity { get; set; }
        public Nullable<bool> ReconcileInvoice { get; set; }
        public Nullable<System.DateTime> ReconcileDate { get; set; }
        public string PODNumber { get; set; }
        public string PCNNumber { get; set; }
        public string PRNNumber { get; set; }
        public int Status { get; set; }
        public Nullable<int> PostingType { get; set; }
        public string THAN { get; set; }
        public Nullable<decimal> ReturnQty { get; set; }
        public Nullable<decimal> OutstandingQty { get; set; }
        public Nullable<decimal> DebriefQty { get; set; }
        public Nullable<decimal> AdminMovement { get; set; }
        public string ChepInvoiceNo { get; set; }
        public string ChepCompensationNo { get; set; }
        public Nullable<int> InvoiceStatus { get; set; }
        public Nullable<int> PODStatus { get; set; }
        public Nullable<int> OrderStatus { get; set; }
        public string UID { get; set; }
        public Nullable<decimal> TransporterLiableQty { get; set; }
        public string ClientLoadComment { get; set; }
        public string ClientLoadNotes { get; set; }
        public string CancelledReason { get; set; }
        public Nullable<int> PODCommentId { get; set; }
        public bool ManuallyMatchedLoad { get; set; }
        public string ManuallyMatchedUID { get; set; }

        public int? ImageCount { get; set; }
        public int? DocumentCount { get; set; }
        public List<Image> Images { get; set; }
        public List<Document> Documents { get; set; }

        public string DocketNumber { get; set; }

        public int? SiteId { get; set; }
        public string SiteName { get; set; }
        public string ClientName { get; set; }
        public string SubSiteName { get; set; }
        public string OutstandingReason { get; set; }
        public string TransporterName { get; set; }
        public string VehicleRegistration { get; set; }

        public int? TaskCount { get; set; }
        public int? JournalCount { get; set; }

        public decimal? ChepNewQuantity { get; set; }


        // Mobile API
        public string Email { get; set; }

        public string APIKey { get; set; }
    }
}
