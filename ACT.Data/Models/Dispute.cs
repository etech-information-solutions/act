//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ACT.Data.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Dispute
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
    
        public virtual Product Product1 { get; set; }
        public virtual User User { get; set; }
        public virtual User User1 { get; set; }
    }
}
