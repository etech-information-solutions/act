
namespace ACT.Core.Models.Custom
{
    using System;
    
    public partial class DisputeCustomModel
    {
        public int Id { get; set; }
        public Nullable<int> ChepLoadId { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public System.DateTime ModifiedOn { get; set; }
        public string ModifiedBy { get; set; }
        public string DocketNumber { get; set; }
        public string DisputeReason { get; set; }
        public string DisputeEmail { get; set; }
        public string TDNNumber { get; set; }
        public Nullable<int> Status { get; set; }
        public string Reference { get; set; }
        public string Equipment { get; set; }
        public string OtherParty { get; set; }
        public string Sender { get; set; }
        public string Receiver { get; set; }
        public string Declarer { get; set; }
        public string Product { get; set; }
        public Nullable<decimal> Quantity { get; set; }
        public Nullable<int> ActionedById { get; set; }
        public Nullable<System.DateTime> ResolvedOn { get; set; }
        public Nullable<int> ResolvedById { get; set; }
        public Nullable<int> ProductId { get; set; }
        public bool Imported { get; set; }

        public string ActionUser { get; set; }
        public string ResolvedUser { get; set; }

        public string ChepLoadAccountNumber { get; set; }
    }
}
