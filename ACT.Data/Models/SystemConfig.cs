//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ACT.Data.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class SystemConfig
    {
        public int Id { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public System.DateTime ModifiedOn { get; set; }
        public string ModifiedBy { get; set; }
        public decimal LogoffSeconds { get; set; }
        public bool AutoLogoff { get; set; }
        public string ActivationEmail { get; set; }
        public Nullable<decimal> PasswordChange { get; set; }
        public Nullable<int> InvoiceRunDay { get; set; }
        public string ContactNumber { get; set; }
        public string SystemContactEmail { get; set; }
        public string FinancialContactEmail { get; set; }
        public string CorrespondenceEmail { get; set; }
        public string DocumentsLocation { get; set; }
        public string ImagesLocation { get; set; }
        public Nullable<System.DateTime> LastBillingRun { get; set; }
        public string Address { get; set; }
        public string AppDownloadUrl { get; set; }
        public string WebsiteUrl { get; set; }
        public string PlatformTnCDocumentUrl { get; set; }
        public string ClientTnCDocumentUrl { get; set; }
    }
}
