
using System;
using System.ComponentModel.DataAnnotations;

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

        #endregion



        #region Model Options



        #endregion
    }
}
