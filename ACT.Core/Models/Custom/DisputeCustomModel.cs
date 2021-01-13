
namespace ACT.Core.Models.Custom
{
    using System;
    
    public partial class DisputeCustomModel
    {
        public int Id { get; set; }
        public Nullable<int> ChepLoadId { get; set; }
        public Nullable<int> ActionedById { get; set; }
        public Nullable<int> ResolvedById { get; set; }
        public Nullable<int> ProductId { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public System.DateTime ModifiedOn { get; set; }
        public string ModifiedBy { get; set; }
        public string DocketNumber { get; set; }
        public string DisputeReason { get; set; }
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
        public Nullable<decimal> Quantity { get; set; }
        public Nullable<System.DateTime> ResolvedOn { get; set; }
        public string ActionBy { get; set; }
        public bool Imported { get; set; }
        public int Status { get; set; }
        public string Location { get; set; }
        public string LocationId { get; set; }
        public string Action { get; set; }
        public string OriginalDocketNumber { get; set; }
        public string OtherReference { get; set; }
        public Nullable<System.DateTime> EffectiveDate { get; set; }
        public Nullable<System.DateTime> ShipDate { get; set; }
        public Nullable<System.DateTime> DelilveryDate { get; set; }
        public Nullable<decimal> DaysLeft { get; set; }
        public string CorrectionRequestNumber { get; set; }
        public Nullable<System.DateTime> CorrectionRequestDate { get; set; }
        public string TransactionType { get; set; }
        public string DisputeComment { get; set; }
        public string DataSource { get; set; }
        public Nullable<int> HasDocket { get; set; }

        public string ActionUser { get; set; }
        public string ResolvedUser { get; set; }

        public string ChepLoadAccountNumber { get; set; }
    }
}
