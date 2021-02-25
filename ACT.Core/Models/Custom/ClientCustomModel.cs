
namespace ACT.Core.Models.Custom
{
    using System;
    using System.Collections.Generic;
    using ACT.Data.Models;

    public partial class ClientCustomModel
    {
        public int Id { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime ModifiedOn { get; set; }
        public string ModifiedBy { get; set; }
        public string CompanyRegistrationNumber { get; set; }
        public string CompanyName { get; set; }
        public string TradingAs { get; set; }
        public string Description { get; set; }
        public string VATNumber { get; set; }
        public string ContactNumber { get; set; }
        public string ContactPerson { get; set; }
        public string FinancialPerson { get; set; }
        public string Email { get; set; }
        public string AdminEmail { get; set; }
        public string DeclinedReason { get; set; }
        public DateTime? RegistrationDate { get; set; }
        public int ServiceRequired { get; set; }
        public int Status { get; set; }

        public string AdminPerson { get; set; }
        public string FinPersonEmail { get; set; }
        public string ChepReference { get; set; }
        public int? PalletType { get; set; }
        public string PalletTypeOther { get; set; }
        public string BBBEELevel { get; set; }
        public int? CompanyType { get; set; }
        public string PSPName { get; set; }
        public int? NumberOfLostPallets { get; set; }


        public int UserCount { get; set; }
        public int BudgetCount { get; set; }
        public int ClientCount { get; set; }
        public int ProductCount { get; set; }
        public int InvoiceCount { get; set; }
        public int DocumentCount { get; set; }

        public int EstimatedLoadCount { get; set; }

        public string PSPCompanyName { get; set; }

        public DateTime? ContractRenewalDate { get; set; }


        public List<Document> Documents { get; set; }
    }
}
