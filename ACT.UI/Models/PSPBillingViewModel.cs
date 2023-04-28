using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using ACT.Core.Enums;
using ACT.Core.Services;

namespace ACT.UI.Models
{
    public class PSPBillingViewModel
    {
        #region Properties

        public int Id { get; set; }
        [Display(Name = "PSP")]
        public int PSPId { get; set; }

        [Display(Name = "Client")]
        public int ClientId { get; set; }
        //[Display(Name = "PSP Product")]
        //public int ProductId { get; set; }

        [Display(Name = "Product")]
        public int PSPProductId { get; set; }

       

        public int ProductId { get; set; }
        [Required]
        [Display(Name = "Status")]
        public Status Status { get; set; }


        [Display(Name = "Select Statement Date")]
        public DateTime? StatementDate { get; set; }

        public int? StatementNumber { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }

        [Display(Name = "Select Payment Date")]
        public DateTime? PaymentDate { get; set; }

        [Display(Name = "Rate")]
        public decimal? Rate { get; set; }
        public int? Units { get; set; }

        [Display(Name = "Invoice Money")]
        public decimal InvoiceAmount { get; set; }

        [Display(Name = "Payment Amount")]
        public decimal? PaymentAmount { get; set; }

        [Display(Name = "Tax Amount")]
        public decimal? TaxAmount { get; set; }

        [Display(Name = "Nominated Account")]
        public string NominatedAccount { get; set; }

        [Display(Name = "Modified By")]
        public string ModifiedBy { get; set; }

        [Display(Name = "Reference Number")]
        public string ReferenceNumber { get; set; }


        [Display(Name = "Equipment")]
        public string Equipment { get; set; }

        [Display(Name = "Accounting Code")]
        public string AccountingCode { get; set; }

        [Display(Name = "Rate Type")]
        public RateType RateType { get; set; }

       // public List<ProductPriceViewModel> ProductPrices { get; set; }

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
}