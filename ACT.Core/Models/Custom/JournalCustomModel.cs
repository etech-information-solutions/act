
namespace ACT.Core.Models.Custom
{
    using System;
    
    public partial class JournalCustomModel
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
    }
}
