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
    
    public partial class EstimatedLoad
    {
        public int Id { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public System.DateTime ModifiedOn { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<int> ObjectId { get; set; }
        public string ObjectType { get; set; }
        public Nullable<int> BudgetYear { get; set; }
        public Nullable<decimal> January { get; set; }
        public Nullable<decimal> February { get; set; }
        public Nullable<decimal> March { get; set; }
        public Nullable<decimal> April { get; set; }
        public Nullable<decimal> May { get; set; }
        public Nullable<decimal> June { get; set; }
        public Nullable<decimal> July { get; set; }
        public Nullable<decimal> August { get; set; }
        public Nullable<decimal> September { get; set; }
        public Nullable<decimal> October { get; set; }
        public Nullable<decimal> November { get; set; }
        public Nullable<decimal> December { get; set; }
        public Nullable<decimal> Total { get; set; }
    }
}
