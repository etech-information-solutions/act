
namespace ACT.Core.Models.Custom
{
    using System;
    
    public partial class PSPConfigCustomModel
    {
        public int Id { get; set; }
        public int PSPId { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime ModifiedOn { get; set; }
        public string ModifiedBy { get; set; }
        public decimal? PasswordChange { get; set; }
        public int? InvoiceRunDay { get; set; }
        public string SystemEmail { get; set; }
        public string FinancialEmail { get; set; }
        public string WelcomeEmailName { get; set; }
        public string DeclineEmailName { get; set; }
        public string ClientCorrespondenceName { get; set; }
        public string DocumentLocation { get; set; }
        public string BillingFileLocation { get; set; }
        public DateTime? LastBillingRun { get; set; }
        public string ImportEmail { get; set; }
        public string ReceivingManagerEmail { get; set; }
        public string OpsManagerEmail { get; set; }
        public string AdminManagerEmail { get; set; }
    }
}
