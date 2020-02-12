
namespace ACT.Core.Models.Custom
{
    using System;
    
    public partial class PSPBillingCustomModel
    {
        public int Id { get; set; }
        public int PSPId { get; set; }
        public int ProductId { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public System.DateTime ModifiedOn { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<int> StatementNumber { get; set; }
        public Nullable<System.DateTime> StatementDate { get; set; }
        public Nullable<decimal> Rate { get; set; }
        public Nullable<int> Units { get; set; }
        public decimal InvoiceAmount { get; set; }
        public Nullable<decimal> PaymentAmount { get; set; }
        public Nullable<System.DateTime> PaymentDate { get; set; }
        public string ReferenceNumber { get; set; }
        public string NominatedAccount { get; set; }
        public int Status { get; set; }
    }
}
