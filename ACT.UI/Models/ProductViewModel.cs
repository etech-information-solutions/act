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

        [Display( Name = "Client" )]
        public int ClientId { get; set; }

        [Display( Name = "Product" )]
        public int ProductId { get; set; }

        [Required]
        [Display( Name = "Name" )]
        [StringLength( 50, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 0 )]
        public string Name { get; set; }

        [Display( Name = "Description" )]
        [StringLength( 500, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 0 )]
        public string Description { get; set; }

        [Required]
        [Display( Name = "Status" )]
        public Status Status { get; set; }

        [Display( Name = "Select Product/Service Documentation" )]
        public FileViewModel File { get; set; }



        [Display( Name = "Select Active Date" )]
        public DateTime? ActiveDate { get; set; }

        [Display( Name = "Hire Rate" )]
        public decimal? HireRate { get; set; }

        [Display( Name = "Lost Rate" )]
        public decimal? LostRate { get; set; }

        [Display( Name = "Issue Rate" )]
        public decimal? IssueRate { get; set; }

        [Display( Name = "Passon Rate" )]
        public decimal? PassonRate { get; set; }

        [Display( Name = "Passon Days" )]
        public int? PassonDays { get; set; }

        [Display( Name = "Equipment" )]
        public string Equipment { get; set; }

        [Display( Name = "Accounting Code" )]
        public string AccountingCode { get; set; }

        [Display( Name = "Rate Type" )]
        public RateType RateType { get; set; }

        public List<ProductPriceViewModel> ProductPrices { get; set; }

        public bool EditMode { get; set; }

        public bool LinkMode { get; set; }

        #endregion



        #region Model Options

        public Dictionary<int, string> ClientOptions
        {
            get
            {
                if ( !LinkMode ) return new Dictionary<int, string>();

                using ( ClientService cservice = new ClientService() )
                {
                    if ( cservice.SelectedClient != null )
                    {
                        ClientId = cservice.SelectedClient.Id;
                    }

                    return cservice.List( true );
                }
            }
        }

        public Dictionary<int, string> ProductOptions
        {
            get
            {
                if ( !LinkMode ) return new Dictionary<int, string>();

                using ( ProductService pservice = new ProductService() )
                {
                    return pservice.List( true );
                }
            }
        }

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