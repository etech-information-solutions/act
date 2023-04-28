using ACT.Core.Enums;
using ACT.Core.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ACT.UI.Models
{
    public class PSPProductViewModel
    {
        #region Properties

        public int Id { get; set; }

        [Display(Name = "Client")]
        public int ClientId { get; set; }

        [Display(Name = "Product")]
        public int ProductId { get; set; }

       // [Required]
        [Display(Name = "Name")]
        [StringLength(50, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 0)]
        public string Name { get; set; }

        [Display(Name = "Description")]
        [StringLength(500, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 0)]
        public string Description { get; set; }

        [Required]
        [Display(Name = "Status")]
        public Status Status { get; set; }

        [Display(Name = "Select Product/Service Documentation")]
        public FileViewModel File { get; set; }



        [Display(Name = "Select Start Date")]
        public DateTime? StartDate { get; set; }

        [Display(Name = "Select End Date")]
        public DateTime? EndDate { get; set; }

        [Display(Name = "Rate")]
        public decimal? Rate { get; set; }

        [Display(Name = "Rate Unit")]
        public RateUnit RateUnit { get; set; }

       

        [Display(Name = "Equipment")]
        public string Equipment { get; set; }

        [Display(Name = "Accounting Code")]
        public string AccountingCode { get; set; }

        [Display(Name = "Rate Type")]
        public RateType RateType { get; set; }

        public int PSPId { get; set; }
        public System.DateTime? CreatedOn { get; set; }
        public System.DateTime? ModifiedOn { get; set; }
        public string ModifiedBy { get; set; }

        public List<ProductPriceViewModel> ProductPrices { get; set; }

        public bool EditMode { get; set; }

        public bool LinkMode { get; set; }

        #endregion



        #region Model Options

        public Dictionary<int, string> ClientOptions
        {
            get
            {
                if (!LinkMode) return new Dictionary<int, string>();

                using (ClientService cservice = new ClientService())
                {
                    if (cservice.SelectedClient != null)
                    {
                        ClientId = cservice.SelectedClient.Id;
                    }

                    return cservice.List(true);
                }
            }
        }

        public Dictionary<int, string> ProductOptions
        {
            get
            {
                //if (!LinkMode) return new Dictionary<int, string>();

                using (ProductService pservice = new ProductService())
                {
                    return pservice.List(true);
                }
            }
        }
        public Dictionary<int, string> PSPOptions
        {
            get
            {
                using (PSPService pservice = new PSPService())
                {
                    return pservice.List(true);
                }
            }
        }

        #endregion
    }

    //public class ProductPriceViewModel
    //{
    //    public int Id { get; set; }

    //    [Display(Name = "Product")]
    //    public int ProductId { get; set; }

    //    [Required]
    //    [Display(Name = "Rate")]
    //    public decimal? Rate { get; set; }

    //    [Display(Name = "Rate Unit")]
    //    public int? RateUnit { get; set; }//

    //    [Display(Name = "Start Date")]
    //    public DateTime? StartDate { get; set; }

    //    [Required]
    //    [Display(Name = "Status")]
    //    public Status Status { get; set; }

    //    [Display(Name = "Product Price Type")]
    //    public ProductPriceType Type { get; set; }
    //}
}