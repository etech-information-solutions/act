
namespace ACT.Core.Models.Custom
{
    using System;
    using System.Collections.Generic;
    using ACT.Data.Models;

    public partial class ClientLoadCustomModel
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public int VehicleId { get; set; }
        public Nullable<int> ClientSiteId { get; set; }
        public Nullable<int> OutstandingReasonId { get; set; }
        public int TransporterId { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public System.DateTime ModifiedOn { get; set; }
        public string ModifiedBy { get; set; }
        public string LoadNumber { get; set; }
        public Nullable<System.DateTime> LoadDate { get; set; }
        public Nullable<System.DateTime> EffectiveDate { get; set; }
        public Nullable<System.DateTime> NotifyeDate { get; set; }
        public string AccountNumber { get; set; }
        public string ClientDescription { get; set; }
        public string DeliveryNote { get; set; }
        public string ReferenceNumber { get; set; }
        public string ReceiverNumber { get; set; }
        public string Equipment { get; set; }
        public Nullable<decimal> OriginalQuantity { get; set; }
        public Nullable<decimal> NewQuantity { get; set; }
        public Nullable<int> ReconcileInvoice { get; set; }
        public Nullable<System.DateTime> ReconcileDate { get; set; }
        public string PODNumber { get; set; }
        public string PCNNumber { get; set; }
        public string PRNNumber { get; set; }
        public int Status { get; set; } 

        public Nullable<decimal> RetQuantity { get; set; }
        public string ARPMComments { get; set; }
        public string ProvCode { get; set; }

        public int? ImageCount { get; set; }
        public int? DocumentCount { get; set; }
        public List<Image> Images { get; set; }
        public List<Document> Documents { get; set; }

        public int? PostingType { get; set; }

        public string DocketNumber { get; set; }
        public Nullable<int> InputInd { get; set; }
        public string THAN { get; set; }
        public Nullable<decimal> ReturnQty { get; set; }
        public Nullable<decimal> OutstandingQty { get; set; }

        public int? SiteId { get; set; }
        public string SiteName { get; set; }
        public string ClientName { get; set; }
        public string SubSiteName { get; set; }
        public string OutstandingReason { get; set; }

        public int? TaskCount { get; set; }
        public int? JournalCount { get; set; }

        public decimal? ChepNewQuantity { get; set; }


        // Mobile API
        public string Email { get; set; }

        public string APIKey { get; set; }
    }
}
