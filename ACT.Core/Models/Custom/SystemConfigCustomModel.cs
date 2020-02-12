
namespace ACT.Core.Models.Custom
{
    using System;
    
    public partial class SystemConfigCustomModel
    {
        public int Id { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public System.DateTime ModifiedOn { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<decimal> LogoffSeconds { get; set; }
        public Nullable<int> AutoLogoff { get; set; }
        public string ActivationEmail { get; set; }
        public Nullable<decimal> PasswordChange { get; set; }
        public Nullable<int> InvoiceRunDay { get; set; }
        public string SystemContactEmail { get; set; }
        public string FinancialContactEmail { get; set; }
        public string WelcomeEmailTemplate { get; set; }
        public string CorrespondenceEmail { get; set; }
        public string DeclineEmailTemplate { get; set; }
        public string ClientDocLocation { get; set; }
        public string BillingFileLocation { get; set; }
        public Nullable<System.DateTime> LastBillingRun { get; set; }
        public string PSPDocumentLocation { get; set; }
    }
}
