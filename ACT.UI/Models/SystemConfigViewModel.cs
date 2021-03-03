
using System;
using System.ComponentModel.DataAnnotations;
using ACT.Core.Enums;

namespace ACT.UI.Models
{
    public class SystemConfigViewModel
    {
        #region Properties

        public int Id { get; set; }

        [Display( Name = "Log off Seconds" )]
        public decimal LogoffSeconds { get; set; }

        [Display( Name = "Auto Log off" )]
        public bool AutoLogoff { get; set; }

        [Display( Name = "Password Change" )]
        public decimal? PasswordChange { get; set; }

        [Display( Name = "Invoice Run Day" )]
        public int? InvoiceRunDay { get; set; }

        [Display( Name = "Images Location" )]
        [StringLength( 200, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 0 )]
        public string ImagesLocation { get; set; }

        [Display( Name = "Documents Location" )]
        [StringLength( 200, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 0 )]
        public string DocumentsLocation { get; set; }

        [Display( Name = "System Contact Email" )]
        [StringLength( 250, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 0 )]
        public string SystemContactEmail { get; set; }

        [Display( Name = "Activation Email" )]
        [StringLength( 250, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 0 )]
        public string ActivationEmail { get; set; }

        [Display( Name = "Financial Contact Email" )]
        [StringLength( 250, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 0 )]
        public string FinancialEmail { get; set; }

        [Display( Name = "Correspondence Email" )]
        [StringLength( 250, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 0 )]
        public string CorrespondenceEmail { get; set; }

        [Display( Name = "Contact Number" )]
        [StringLength( 150, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 0 )]
        public string ContactNumber { get; set; }

        [Display( Name = "Address" )]
        [StringLength( 500, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 0 )]
        public string Address { get; set; }

        [Display( Name = "App Download Url" )]
        [StringLength( 500, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 0 )]
        public string AppDownloadUrl { get; set; }

        [Display( Name = "Website Url" )]
        [StringLength( 250, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 0 )]
        public string WebsiteUrl { get; internal set; }



        [Display( Name = "Dispute Monitor Path" )]
        [StringLength( 250, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 0 )]
        public string DisputeMonitorPath { get; set; }

        [Display( Name = "Dispute Monitor Interval" )]
        public string DisputeMonitorInterval { get; set; }

        [Display( Name = "Dispute Monitor Time" )]
        public TimeSpan? DisputeMonitorTime { get; set; }

        [Display( Name = "Dispute Monitor Enabled" )]
        public YesNo DisputeMonitorEnabled { get; set; }



        [Display( Name = "Client Monitor Path" )]
        [StringLength( 250, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 0 )]
        public string ClientMonitorPath { get; set; }

        [Display( Name = "Client Monitor Interval" )]
        public string ClientMonitorInterval { get; set; }

        [Display( Name = "Client Monitor Time" )]
        public TimeSpan? ClientMonitorTime { get; set; }

        [Display( Name = "Client Monitor Enabled" )]
        public YesNo ClientMonitorEnabled { get; set; }

        

        [Display( Name = "Client Contract Renewal Reminder (in months)" )]
        public int? ClientContractRenewalReminderMonths { get; set; }

        [Display( Name = "Dispute Resolve Time (in days)" )]
        public int? DisputeDaysToResolve { get; set; }


        #endregion



        #region Model Options



        #endregion
    }
}
