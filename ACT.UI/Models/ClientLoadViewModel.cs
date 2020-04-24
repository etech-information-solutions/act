using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace ACT.UI.Models
{
    public class ClientLoadViewModel
    {
        #region Properties

        public int Id { get; set; }

        public int ClientId { get; set; }
        public int VehicleId { get; set; }
        public int TransporterId { get; set; }

        public System.DateTime CreatedOn { get; set; }
        public System.DateTime ModifiedOn { get; set; }
        public string ModifiedBy { get; set; }
        public string LoadNumber { get; set; }
        [Required]
        [Display(Name = "Load Date")]
        //[StringLength(50, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 0)]
        public Nullable<System.DateTime> LoadDate { get; set; }
        [Required]
        [Display(Name = "Effective Date")]
        //[StringLength(50, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 0)]
        public Nullable<System.DateTime> EffectiveDate { get; set; }
        [Required]
        [Display(Name = "Notify Date")]
        //[StringLength(50, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 0)]
        public Nullable<System.DateTime> NotifyeDate { get; set; }
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

        [Required]
        [Display(Name = "Original Quantity")]
        //[StringLength(50, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 0)]
        public Nullable<decimal> OriginalQuantity { get; set; }

        [Display(Name = "New Quantity")]
        //[StringLength(50, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 0)]
        public Nullable<decimal> NewQuantity { get; set; }
        public Nullable<int> ReconcileInvoice { get; set; }
        public Nullable<System.DateTime> ReconcileDate { get; set; }
        
        [Display(Name = "POD Number")]
        [StringLength(50, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 0)]
        public string PODNumber { get; set; }
        
        [Display(Name = "PCN Number")]
        [StringLength(50, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 0)]
        public string PCNNumber { get; set; }
        
        [Display(Name = "PRN Number")]
        [StringLength(50, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 0)]
        public string PRNNumber { get; set; }

        public int Status { get; set; }
        [Display(Name = "Return Quantity")]
        //[StringLength(18, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 0)]
        public Nullable<decimal> RetQuantity { get; set; }
        [Display(Name = "ARPM Comments")]
        [StringLength(150, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 0)]
        public string ARPMComments { get; set; }
        [Display(Name = "Province Code")]
        [StringLength(30, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 0)]
        public string ProvCode { get; set; }

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