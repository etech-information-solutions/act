
namespace ACT.Core.Models.Custom
{
    using System;
    
    public partial class DisputeCustomModel
    {
        public int Id { get; set; }
        public int? ChepLoadId { get; set; }
        public int? ActionedById { get; set; }
        public int? ResolvedById { get; set; }
        public int? DisputeReasonId { get; set; }
        public int? ProductId { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime ModifiedOn { get; set; }
        public string ModifiedBy { get; set; }
        public string DocketNumber { get; set; }
        public string DisputeEmail { get; set; }
        public string TDNNumber { get; set; }
        public string Reference { get; set; }
        public string Equipment { get; set; }
        public string EquipmentCode { get; set; }
        public string OtherParty { get; set; }
        public string Sender { get; set; }
        public string Receiver { get; set; }
        public string Declarer { get; set; }
        public string Product { get; set; }
        public decimal? Quantity { get; set; }
        public DateTime? ResolvedOn { get; set; }
        public string ActionBy { get; set; }
        public bool Imported { get; set; }
        public int Status { get; set; }
        public string Location { get; set; }
        public string LocationId { get; set; }
        public string Action { get; set; }
        public string OriginalDocketNumber { get; set; }
        public string OtherReference { get; set; }
        public DateTime? EffectiveDate { get; set; }
        public DateTime? ShipDate { get; set; }
        public DateTime? DelilveryDate { get; set; }
        public decimal? DaysLeft { get; set; }
        public string CorrectionRequestNumber { get; set; }
        public DateTime? CorrectionRequestDate { get; set; }
        public string TransactionType { get; set; }
        public string DisputeComment { get; set; }
        public string DataSource { get; set; }
        public int? HasDocket { get; set; }

        public string ActionUser { get; set; }
        public string ResolvedUser { get; set; }

        public string ChepLoadAccountNumber { get; set; }

        public string DisputeReasonDetails { get; set; }

        public decimal? Disputes { get; set; }
    }
}
