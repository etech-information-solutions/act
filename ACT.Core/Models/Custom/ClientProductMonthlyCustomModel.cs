
namespace ACT.Core.Models.Custom
{
    using System;
    
    public partial class ClientProductMonthlyCustomModel
    {
        public int Id { get; set; }
        public int ClientProductId { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime ModifiedOn { get; set; }
        public string ModifiedBy { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public decimal? OpeningBalance { get; set; }
        public decimal? OpeningValue { get; set; }
        public decimal? MonthIssues { get; set; }
        public decimal? MonthReturns { get; set; }
        public decimal? MonthTransferIn { get; set; }
        public decimal? MonthTransferOut { get; set; }
        public decimal? ClosingBalance { get; set; }
        public decimal? ClosingValue { get; set; }
        public decimal? NetRevenue { get; set; }
        public decimal? TaxAmount { get; set; }
        public decimal? GrossRevenue { get; set; }
        public int Status { get; set; }
    }
}
