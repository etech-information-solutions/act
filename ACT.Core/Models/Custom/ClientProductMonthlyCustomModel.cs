
namespace ACT.Core.Models.Custom
{
    using System;
    
    public partial class ClientProductMonthlyCustomModel
    {
        public int Id { get; set; }
        public int ClientProductId { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public System.DateTime ModifiedOn { get; set; }
        public string ModifiedBy { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public Nullable<decimal> OpeningBalance { get; set; }
        public Nullable<decimal> OpeningValue { get; set; }
        public Nullable<decimal> MonthIssues { get; set; }
        public Nullable<decimal> MonthReturns { get; set; }
        public Nullable<decimal> MonthTransferIn { get; set; }
        public Nullable<decimal> MonthTransferOut { get; set; }
        public Nullable<decimal> ClosingBalance { get; set; }
        public Nullable<decimal> ClosingValue { get; set; }
        public Nullable<decimal> NetRevenue { get; set; }
        public Nullable<decimal> TaxAmount { get; set; }
        public Nullable<decimal> GrossRevenue { get; set; }
        public int Status { get; set; }
    }
}
