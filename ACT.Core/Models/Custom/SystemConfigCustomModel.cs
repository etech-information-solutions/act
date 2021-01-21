
namespace ACT.Core.Models.Custom
{
    using System;
    
    public partial class SystemConfigCustomModel
    {
        public int Id { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime ModifiedOn { get; set; }
        public string ModifiedBy { get; set; }
        public decimal? LogoffSeconds { get; set; }
        public int? AutoLogoff { get; set; }
        public string ActivationEmail { get; set; }
        public decimal? PasswordChange { get; set; }
        public int? InvoiceRunDay { get; set; }
        public string SystemContactEmail { get; set; }
        public string FinancialContactEmail { get; set; }
        public string WelcomeEmailTemplate { get; set; }
        public string CorrespondenceEmail { get; set; }
        public string DeclineEmailTemplate { get; set; }
        public string ClientDocLocation { get; set; }
        public string BillingFileLocation { get; set; }
        public DateTime? LastBillingRun { get; set; }
        public string PSPDocumentLocation { get; set; }
    }
}
