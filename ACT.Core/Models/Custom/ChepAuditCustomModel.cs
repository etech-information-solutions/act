
namespace ACT.Core.Models.Custom
{
    using System;
    
    public partial class ChepAuditCustomModel
    {
        public int Id { get; set; }
        public int SiteId { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public System.DateTime ModifiedOn { get; set; }
        public string ModifiedBy { get; set; }
        public string Equipment { get; set; }
        public Nullable<decimal> ChepStockBalance { get; set; }
        public Nullable<decimal> NotInvoiceIn { get; set; }
        public Nullable<decimal> NotInvoiceOut { get; set; }
        public Nullable<decimal> ReqAdjustment { get; set; }
        public Nullable<decimal> NotMCCIn { get; set; }
        public Nullable<decimal> NotMCCOut { get; set; }
        public Nullable<decimal> SuspendITL { get; set; }
        public Nullable<decimal> SuspendMCC { get; set; }
        public Nullable<decimal> AdjustedInvBalance { get; set; }
        public Nullable<decimal> MccBalance { get; set; }

        public string SiteName { get; set; }

        public int? ReportDocumentId { get; set; }
    }
}
