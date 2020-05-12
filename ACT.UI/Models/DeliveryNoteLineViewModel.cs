using System.ComponentModel.DataAnnotations;
using System;

namespace ACT.UI.Models
{

    public partial class DeliveryNoteLineViewModel
    {
        public int Id { get; set; }
        public int DeliveryNoteId { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public System.DateTime ModifiedOn { get; set; }
        public string ModifiedBy { get; set; }
        [Required]
        [Display(Name = "Product")]
        [StringLength(50, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 1)]
        public string Product { get; set; }
        [Required]
        [Display(Name = "Product Description")]
        [StringLength(150, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 1)]
        public string ProductDescription { get; set; }
        [Required]
        [Range(-9999999, 9999999)]
        [Display(Name = "Order Quantity")]
        public Nullable<decimal> OrderQuantity { get; set; }
        [Required]
        [Range(-9999999, 9999999)]
        [Display(Name = "Delivered")]
        public Nullable<decimal> Delivered { get; set; }
        [Required]
        [Range(-9999999, 9999999)]
        [Display(Name = "Outstanding")]
        public Nullable<decimal> Outstanding { get; set; }
        public int Status { get; set; }
    }
}
