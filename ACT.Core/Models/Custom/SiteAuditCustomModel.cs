
namespace ACT.Core.Models.Custom
{
    using System;
    using System.Collections.Generic;
    
    public partial class SiteAuditCustomModel
    {
        public int Id { get; set; }
        public int PSPId { get; set; }
        public int SiteId { get; set; }
        public int ClientId { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime ModifiedOn { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? AuditDate { get; set; }
        public string Equipment { get; set; }
        public decimal? PalletsOutstanding { get; set; }
        public decimal? PalletsCounted { get; set; }
        public decimal? WriteoffPallets { get; set; }
        public string CustomerName { get; set; }
        public byte[] CustomerSignature { get; set; }
        public string RepName { get; set; }
        public byte[] RepSignature { get; set; }
        public string PalletAuditor { get; set; }
        public byte[] PalletAuditorSign { get; set; }
        public string DocumentLocation { get; set; }
        public int Status { get; set; }
        public int? CustomerSignatureId { get; set; }
        public int? RepSignatureId { get; set; }
        public int? PalletAuditorSignatureId { get; set; }

        public string PSPName { get; set; }
        public string SiteName { get; set; }
        public string ClientName { get; set; }

        public int? ReportDocumentId { get; set; } 


        // Mobile API
        public string Email { get; set; }

        public string APIKey { get; set; }
    }
}
