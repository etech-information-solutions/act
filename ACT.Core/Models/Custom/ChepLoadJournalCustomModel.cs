
namespace ACT.Core.Models.Custom
{
    using System;
    using System.Collections.Generic;
    using ACT.Data.Models;
    public partial class ChepLoadJournalCustomModel
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public int? ClientSiteId { get; set; }
        public int? OutstandingReasonId { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime ModifiedOn { get; set; }
        public string ModifiedBy { get; set; }
        public string ChepStatus { get; set; }
        public string TransactionType { get; set; }
        public string DocketNumber { get; set; }
        public string OriginalDocketNumber { get; set; }
        public string UMI { get; set; }
        public string LocationId { get; set; }
        public string Location { get; set; }
        public string OtherPartyId { get; set; }
        public string OtherParty { get; set; }
        public string OtherPartyCountry { get; set; }
        public string EquipmentCode { get; set; }
        public string Equipment { get; set; }
        public int? Quantity { get; set; }
        public string Ref { get; set; }
        public string OtherRef { get; set; }
        public string BatchRef { get; set; }
        public DateTime? ShipmentDate { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public DateTime? EffectiveDate { get; set; }
        public DateTime? CreateDate { get; set; }
        public string CreatedBy { get; set; }
        public string InvoiceNumber { get; set; }
        public string Reason { get; set; }
        public string DataSource { get; set; }
        public int? BalanceStatus { get; set; }
        public int Status { get; set; }
        public int PostingType { get; set; }
        public bool IsExchange { get; set; }
        public bool IsPSPPickup { get; set; }
        public bool TransporterLiable { get; set; }
        public bool ManuallyMatchedLoad { get; set; }
        public string ManuallyMatchedUID { get; set; }
        public string CorrectedRef { get; set; }
        public string CorrectedOtherRef { get; set; }
        public bool IsExtra { get; set; }
        public int? ChepLoadId { get; set; }

        public virtual Client Client { get; set; }
        public int VersionCount { get; set; }
        public int DocumentCount { get; set; }
        public int CommentCount { get; set; }
        public List<Comment> Comments { get; set; }
        public List<Document> Documents { get; set; }

        public string ClientName { get; set; }

        public string OutstandingReason { get; set; }

        public string AuthorisationCode { get; set; }
        public string LoadNumber { get; set; }
        public string TransporterName { get; set; }
        public string VehicleRegistration { get; set; }

        // Reports

        public string SiteName { get; set; }
        public string RegionName { get; set; }
    }
}
