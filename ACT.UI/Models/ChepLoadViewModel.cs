using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace ACT.UI.Models
{
    public class ChepLoadViewModel
    {
        #region Properties

        public int Id { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public System.DateTime ModifiedOn { get; set; }
        public string ModifiedBy { get; set; }

        [Required]
        [Display(Name = "Load Date")]
        //[StringLength(50, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 0)]
        public Nullable<System.DateTime> LoadDate { get; set; }

        [Required]
        [Display(Name = "Notify Date")]
        //[StringLength(50, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 0)]
        public Nullable<System.DateTime> NotifyDate { get; set; }

        [Required]
        [Display(Name = "Effective Date")]
        //[StringLength(50, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 0)]
        public Nullable<System.DateTime> EffectiveDate { get; set; }

        public Nullable<int> PostingType { get; set; }

        [Required]
        [Display(Name = "Account Number")]
        [StringLength(50, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 0)]
        public string AccountNumber { get; set; }

        [Required]
        [Display(Name = "Client Description")]
        [StringLength(50, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 0)]
        public string ClientDescription { get; set; }

        [Required]
        [Display(Name = "Delivery Note")]
        [StringLength(50, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 0)]
        public string DeliveryNote { get; set; }

        [Required]
        [Display(Name = "Reference Number")]
        [StringLength(50, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 0)]
        public string ReferenceNumber { get; set; }

        [Required]
        [Display(Name = "Receiver Number")]
        [StringLength(50, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 0)]
        public string ReceiverNumber { get; set; }

        [Display(Name = "Equipment")]
        [StringLength(50, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 0)]
        public string Equipment { get; set; }

        [Display(Name = "Original Quantity")]
        //[StringLength(50, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 0)]
        public Nullable<decimal> OriginalQuantity { get; set; }

        [Display(Name = "New Quantity")]
        //[StringLength(50, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 0)]
        public Nullable<decimal> NewQuantity { get; set; }

        [Display(Name = "Docket Number")]
        [StringLength(50, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 0)]
        public string DocketNumber { get; set; }

        public int Status { get; set; }
        [Display(Name = "Load Files")]
        public ICollection<FileViewModel> Documents { get; set; }
        public int? DocumentCount { get; set; }
        //public List<Document> Documents { get; set; }

        public bool EditMode { get; set; }
        public bool ContextualMode { get; set; }
        public string DocsList { get; set; }

        #endregion


        #region Model Options


        #endregion
    }
}