
namespace ACT.Core.Models.Custom
{
    using System;
    using System.Collections.Generic;
    using ACT.Data.Models;

    public partial class ClientCustomModel
    {
        public int Id { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public System.DateTime ModifiedOn { get; set; }
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
        public Nullable<System.DateTime> RegistrationDate { get; set; }
        public int ServiceRequired { get; set; }
        public int Status { get; set; }

        public string AdminPerson { get; set; }
        public string FinPersonEmail { get; set; }
        public string ChepReference { get; set; }


        public int UserCount { get; set; }
        public int BudgetCount { get; set; }
        public int ClientCount { get; set; }
        public int ProductCount { get; set; }
        public int InvoiceCount { get; set; }
        public int DocumentCount { get; set; }

        public string PSPName { get; set; }


        public List<Document> Documents { get; set; }
    }
}
