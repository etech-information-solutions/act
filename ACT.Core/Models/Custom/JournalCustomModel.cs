
namespace ACT.Core.Models.Custom
{
    using System;
    
    public partial class JournalCustomModel
    {
        public int Id { get; set; }
        public int? SiteAuditId { get; set; }
        public int? ClientLoadId { get; set; }
        public int? DocumetId { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime ModifiedOn { get; set; }
        public string ModifiedBy { get; set; }
        public string PostintDescription { get; set; }
        public decimal? PostingQuantity { get; set; }
        public bool? IsIn { get; set; }
        public string THANNumber { get; set; }
        public int Status { get; set; }
    }
}
