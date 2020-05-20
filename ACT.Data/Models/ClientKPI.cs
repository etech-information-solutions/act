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
    
    public partial class ClientKPI
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public System.DateTime ModifiedOn { get; set; }
        public string ModifiedBy { get; set; }
        public string KPIDescription { get; set; }
        public Nullable<decimal> Disputes { get; set; }
        public Nullable<decimal> OutstandingPallets { get; set; }
        public Nullable<decimal> Passons { get; set; }
        public Nullable<decimal> MonthlyCost { get; set; }
        public int Status { get; set; }
        public Nullable<int> OutstandingDays { get; set; }
        public Nullable<int> ResolveDays { get; set; }
        public Nullable<decimal> Weight { get; set; }
        public Nullable<decimal> TargetAmount { get; set; }
        public Nullable<int> TargetPeriod { get; set; }
    
        public virtual Client Client { get; set; }
    }
}
