using ACT.Core.Services;
using ACT.Data.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ACT.UI.Models
{
    public class BillingInvoiceViewModel
    {

        public BillingInvoiceViewModel()
        {
            billings = new List<PSPBillingViewModel>();


        }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string PostalCode { get; set; }
        public Province Province { get; set; }

        public string Town { get; set; }
        public string AccountNumber { get; set; }
        public string  Bank { get; set; }

        public int AccountType { get; set; }
        public string Branch { get; set; }

        public string VATNumber { get; set; }
        public string ContactNumber { get; set; }
        public string ContactEmail { get; set; }
        public string ContactPerson { get; set; }
        public string CompanyName { get; set; }

        public string InvoiceNumber { get; set; }

        public string StatementDate { get; set; }

        public string DueDate { get; set; }
        public decimal InvoiceAmount { get; set; }

        public decimal Total { get; set; }
        public string CreatedOn { get; set; }
        public int PSPId { get; set; }
        public string PSPName { get; set; }
       public string ClientName { get; set; }

        public List<PSPBillingViewModel> billings { get; set; }
      //  public PSPBillingViewModel pSPBilling { get; set; }
      //  public BankDetailViewModel bankView { get; set; }
      //  public AddressViewModel addressView { get; set; }
       // public PSPViewModel pSPView { get; set; }
        [Display(Name = "Client")]
        public int ClientId { get; set; }
        public bool EditMode { get; set; }

        public bool LinkMode { get; set; }


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