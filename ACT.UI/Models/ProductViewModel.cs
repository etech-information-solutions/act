using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ACT.Core.Enums;
using ACT.Core.Services;

namespace ACT.UI.Models
{
    public class ProductViewModel
    {
        #region Properties

        public int Id { get; set; }

        [Required]
        [Display( Name = "Name" )]
        [StringLength( 50, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 0 )]
        public string Name { get; set; }

        [Required]
        [Display( Name = "Description" )]
        [StringLength( 500, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 0 )]
        public string Description { get; set; }

        [Required]
        [Display( Name = "Status" )]
        public Status Status { get; set; }

        [Display( Name = "Select Product/Service Documentation" )]
        public FileViewModel File { get; set; }

        public List<ProductPriceViewModel> ProductPrices { get; set; }

        public bool EditMode { get; set; }
        public bool ContextualMode { get; set; }

        #endregion



        #region Model Options

        #endregion
    }

    public class ProductPriceViewModel
    {
        public int Id { get; set; }

        [Display( Name = "Product" )]
        public int ProductId { get; set; }

        [Required]
        [Display( Name = "Rate" )]
        public decimal? Rate { get; set; }

        [Display( Name = "Rate Unit" )]
        public int? RateUnit { get; set; }

        [Display( Name = "Start Date" )]
        public DateTime? StartDate { get; set; }

        [Required]
        [Display( Name = "Status" )]
        public Status Status { get; set; }

        [Display( Name = "Product Price Type" )]
        public ProductPriceType Type { get; set; }
    }
}