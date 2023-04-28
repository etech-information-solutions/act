
using System;
using System.ComponentModel.DataAnnotations;
using ACT.Core.Enums;

namespace ACT.UI.Models
{
    public class PSPConfigViewModel
    {
        #region Properties

        public int Id { get; set; }

        [Required]
        public int PSPId { get; set; }

        [Required]
        [Display( Name = "Invoice Run Day" )]
        public int? InvoiceRunDay { get; set; }

        [Required]
        [Display( Name = "Billing File Location" )]
        [StringLength( 50, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 0 )]
        public string BillingFileLocation { get; set; }

        [Required]
        [Display( Name = "Documents Location" )]
        [StringLength( 150, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 0 )]
        public string DocumentLocation { get; set; }

        [Required]
        [Display( Name = "System Contact Email" )]
        [StringLength( 150, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 0 )]
        public string SystemContactEmail { get; set; }

        [Required]
        [Display( Name = "Import Email" )]
        [StringLength( 150, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 0 )]
        public string ImportEmail { get; set; }

        [Required]
        [Display( Name = "Financial Contact Email" )]
        [StringLength( 150, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 0 )]
        public string FinancialEmail { get; set; }

        [Required]
        [Display( Name = "Receiving Manager Email" )]
        [StringLength( 50, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 0 )]
        public string ReceivingManagerEmail { get; set; }

        [Required]
        [Display( Name = "OPs Manager Email" )]
        [StringLength( 50, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 0 )]
        public string OpsManagerEmail { get; set; }

        [Required]
        [Display( Name = "Admin Manager Email" )]
        [StringLength( 50, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 0 )]
        public string AdminManagerEmail { get; set; }

        [Required]
        [Display( Name = "Welcome Email Name" )]
        [StringLength( 50, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 0 )]
        public string WelcomeEmailName { get; set; }

        [Required]
        [Display( Name = "Decline Email Name" )]
        [StringLength( 50, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 0 )]
        public string DeclineEmailName { get; set; }

        [Required]
        [Display( Name = "Client Correspondence Name" )]
        [StringLength( 150, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 0 )]
        public string ClientCorrespondenceName { get; set; }

        [Display(Name = "Import Email Host")]
        [StringLength(100, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 0)]
        public string ImportEmailHost { get; set; }

        [Display(Name = "Import Email Username")]
        [StringLength(100, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 0)]
        public string ImportEmailUsername { get; set; }

        [Display(Name = "Import Email Password")]
        [StringLength(50, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 0)]
        public string ImportEmailPassword { get; set; }

        [Display(Name = "Import Email Port")]
        public int? ImportEmailPort { get; set; }

        [Display( Name = "Import Use SSL" )]
        public YesNo ImportUseSSL { get; set; }
        [Display(Name = "Collection Run Time")]
        public TimeSpan? CollectionRunTime { get; set; }

        #endregion



        #region Model Options



        #endregion
    }
}
