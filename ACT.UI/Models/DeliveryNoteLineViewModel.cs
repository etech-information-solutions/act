using System.ComponentModel.DataAnnotations;
using System;
using ACT.Core.Enums;

namespace ACT.UI.Models
{

    public partial class DeliveryNoteLineViewModel
    {
        public int Id { get; set; }

        public int DeliveryNoteId { get; set; }

        [Required]
        [Display( Name = "Product" )]
        [StringLength( 50, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 1 )]
        public string Product { get; set; }

        [Required]
        [Display( Name = "Product Description" )]
        [StringLength( 150, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 1 )]
        public string ProductDescription { get; set; }

        [Required]
        [Display( Name = "Order" )]
        public int Ordered { get; set; }

        [Display( Name = "Delivered" )]
        public int Delivered { get; set; }

        [Display( Name = "Returned" )]
        public int Returned { get; set; }

        public Status Status { get; set; }
    }
}
