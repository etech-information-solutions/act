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
    
    public partial class Journal
    {
        public int Id { get; set; }
        public Nullable<int> SiteAuditId { get; set; }
        public Nullable<int> ClientLoadId { get; set; }
        public Nullable<int> DocumetId { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public System.DateTime ModifiedOn { get; set; }
        public string ModifiedBy { get; set; }
        public string PostintDescription { get; set; }
        public Nullable<decimal> PostingQuantity { get; set; }
        public Nullable<bool> IsIn { get; set; }
        public string THANNumber { get; set; }
        public int Status { get; set; }
    
        public virtual ClientLoad ClientLoad { get; set; }
        public virtual SiteAudit SiteAudit { get; set; }
        public virtual Document Document { get; set; }
    }
}
