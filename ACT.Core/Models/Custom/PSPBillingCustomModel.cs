
namespace ACT.Core.Models.Custom
{
    using System;
    
    public partial class PSPBillingCustomModel
    {
        public int Id { get; set; }
        public int PSPId { get; set; }
        public int PSPProductId { get; set; }
        public int ClientId { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime ModifiedOn { get; set; }
        public string ModifiedBy { get; set; }
        public int? StatementNumber { get; set; }
        public DateTime? StatementDate { get; set; }
        public decimal? Rate { get; set; }
        public int? Units { get; set; }
        public decimal InvoiceAmount { get; set; }
        public decimal? TaxAmount { get; set; }
        public decimal? PaymentAmount { get; set; }
        public DateTime? PaymentDate { get; set; }
        public string ReferenceNumber { get; set; }
        public string NominatedAccount { get; set; }
        public int Status { get; set; }

        public string ClientName { get; set; }
        public string PSPName { get; set; }
        public string ProductName { get; set; }
    }
}
