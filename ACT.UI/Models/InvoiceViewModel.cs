using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using ACT.Core.Models;
using ACT.Core.Services;

namespace ACT.UI.Models
{
    public class InvoiceViewModel
    {
        #region Properties

        public int Id { get; set; }

        [Required]
        [Display( Name = "Invoice #" )]
        [StringLength( 50, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 0 )]
        public string Number { get; set; }

        [Required]
        [Display( Name = "Invoice Date" )]
        public DateTime? Date { get; set; }

        [Required]
        [Display( Name = "Invoice Qty" )]
        public decimal? Quantity { get; set; }

        [Required]
        [Display( Name = "Invoice Amount" )]
        public decimal? Amount { get; set; }

        [Required]
        [Display( Name = "Load Number" )]
        [StringLength( 50, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 0 )]
        public string LoadNumber { get; set; }

        [Display( Name = "File" )]
        public HttpPostedFileBase File { get; set; }

        public bool EditMode { get; set; }

        #endregion

        #region Model Options


        #endregion
    }
}