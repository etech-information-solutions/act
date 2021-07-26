
namespace ACT.Core.Models.Custom
{
    using System;
    
    public partial class ChepAuditCustomModel
    {
        public int Id { get; set; }
        public int SiteId { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime ModifiedOn { get; set; }
        public string ModifiedBy { get; set; }
        public string Equipment { get; set; }
        public decimal? ChepStockBalance { get; set; }
        public decimal? NotInvoiceIn { get; set; }
        public decimal? NotInvoiceOut { get; set; }
        public decimal? ReqAdjustment { get; set; }
        public decimal? NotMCCIn { get; set; }
        public decimal? NotMCCOut { get; set; }
        public decimal? SuspendITL { get; set; }
        public decimal? SuspendMCC { get; set; }
        public decimal? AdjustedInvBalance { get; set; }
        public decimal? MccBalance { get; set; }

        public string SiteName { get; set; }

        public int? ReportDocumentId { get; set; }
    }
}
