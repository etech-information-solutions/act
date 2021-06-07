
using System;

namespace ACT.Core.Models.Custom
{
    public partial class ClientAuthorisationCustomModel
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int ClientLoadId { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime ModifiedOn { get; set; }
        public string ModifiedBy { get; set; }
        public string Code { get; set; }
        public DateTime? AuthorisationDate { get; set; }
        public int? Status { get; set; }

        public int? ClientAuthorisationId { get; set; }

        #region Client Load

        public int ClientId { get; set; }
        public int? VehicleId { get; set; }
        public int? ClientSiteId { get; set; }
        public int? TransporterId { get; set; }
        public int? ToClientSiteId { get; set; }
        public int? OutstandingReasonId { get; set; }
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
        public bool? ReconcileInvoice { get; set; }
        public DateTime? ReconcileDate { get; set; }
        public string PODNumber { get; set; }
        public string PCNNumber { get; set; }
        public string PRNNumber { get; set; }
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
        public int? PODCommentId { get; set; }
        public bool ManuallyMatchedLoad { get; set; }
        public string ManuallyMatchedUID { get; set; }

        #endregion


        public string ToSiteName { get; set; }
        public string FromSiteName { get; set; }
        public string ClientName { get; set; }
        public string AuthoriserName { get; set; }
        public string AuthorisationComment { get; set; }
        public string TransporterName { get; set; }
        public string VehicleRegistration { get; set; }
    }
}
