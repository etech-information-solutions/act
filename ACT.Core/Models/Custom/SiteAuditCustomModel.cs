
namespace ACT.Core.Models.Custom
{
    using System;
    using System.Collections.Generic;
    
    public partial class SiteAuditCustomModel
    {
        public int Id { get; set; }
        public int SiteId { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public System.DateTime ModifiedOn { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> AuditDate { get; set; }
        public string Equipment { get; set; }
        public Nullable<decimal> PalletsOutstanding { get; set; }
        public Nullable<decimal> PalletsCounted { get; set; }
        public Nullable<decimal> WriteoffPallets { get; set; }
        public string CustomerName { get; set; }
        public byte[] CustomerSignature { get; set; }
        public string RepName { get; set; }
        public byte[] RepSignature { get; set; }
        public string PalletAuditor { get; set; }
        public byte[] PalletAuditorSign { get; set; }
        public string DocumentLocation { get; set; }
        public int Status { get; set; }

        public string SiteName { get; set; }

        public int? ReportDocumentId { get; set; }
    }
}
