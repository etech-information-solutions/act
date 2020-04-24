using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ACT.Core.Enums;
using ACT.Core.Services;
using ACT.Data.Models;
using System.Collections.Generic;
using ACT.Core.Models.Custom;
namespace ACT.UI.Models
{
    public class DeliveryNoteViewModel
    {
        #region Properties


        public int Id { get; set; }
        public int ClientId { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public System.DateTime ModifiedOn { get; set; }
        public string ModifiedBy { get; set; }
        [Required]
        [Display(Name = "Customer Name")]
        [StringLength(50, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 1)]
        public string CustomerName { get; set; }
        [Required]
        [Display(Name = "Customer Address")]
        [StringLength(50, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 1)]
        public string CustomerAddress { get; set; }

        public string CustomerAddress2 { get; set; }
        public string CustomerAddressTown { get; set; }
        [Required]
        [Display(Name = "CustomerPostalCode ")]
        [StringLength(50, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 1)]
        public string CustomerPostalCode { get; set; }
        [Required]
        [Display(Name = "Customer Province")]
        [StringLength(50, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 1)]
        public Nullable<int> CustomerProvince { get; set; }
        [Required]
        [Display(Name = "Delivery Address")]
        [StringLength(50, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 1)]
        public string DeliveryAddress { get; set; }
        public string DeliveryAddress2 { get; set; }
        public string DeliveryAddressTown { get; set; }
        [Required]
        [Display(Name = "DeliveryPostalCode ")]
        [StringLength(50, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 1)]
        public string DeliveryPostalCode { get; set; }
        [Required]
        [Display(Name = "Delivery Province")]
        //[StringLength(50, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 1)]
        public Nullable<int> DeliveryProvince { get; set; }
        [Required]
        [Display(Name = "Invoice Number")]
        [StringLength(50, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 1)]
        public string InvoiceNumber { get; set; }
        [Required]
        [Display(Name = "Order Number")]
        [StringLength(50, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 1)]
        public string OrderNumber { get; set; }
        [Display(Name = "Order Date")]
        //[StringLength(50, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 1)]
        public Nullable<System.DateTime> OrderDate { get; set; }
        [Required]
        [Display(Name = "Billing Address")]
        [StringLength(50, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 1)]
        public string BillingAddress { get; set; }
        public string BillingAddress2 { get; set; }
        public string BillingAddressTown { get; set; }
        [Required]
        [Display(Name = "Billing PostalCode")]
        [StringLength(50, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 1)]
        public string BillingPostalCode { get; set; }
        [Required]
        [Display(Name = "Billing Province")]
        //[StringLength(50, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 1)]
        public Nullable<int> BillingProvince { get; set; }
        [Required]
        [Display(Name = "Reference")]
        [StringLength(50, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 1)]
        public string Reference306 { get; set; }
        [Display(Name = "Status")]
        public int Status { get; set; }

        [Display(Name = "Delivery Note Lines")]
        public List<DeliveryNoteLineCustomModel> DeliveryNoteLines { get; set; }
        [Display(Name = "Line Count")]
        public int CountNoteLines { get; set; }

        public string DeliveryEmail { get; set; }

        #endregion

        #region Model Options
        #endregion
    }
}
