
using System.Collections.Generic;
using ACT.Data.Models;
namespace ACT.Core.Models.Custom
{
    using System;
    
    public partial class ClientProductCustomModel
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public int ProductId { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public System.DateTime ModifiedOn { get; set; }
        public string ModifiedBy { get; set; }
        public string ProductDescription { get; set; }
        public Nullable<System.DateTime> ActiveDate { get; set; }
        public Nullable<decimal> HireRate { get; set; }
        public Nullable<decimal> LostRate { get; set; }
        public Nullable<decimal> IssueRate { get; set; }
        public Nullable<decimal> PassonRate { get; set; }
        public Nullable<int> PassonDays { get; set; }
        public int Status { get; set; }
    
        public string Name { get; set; }
        public string CompanyName { get; set; }
        public int? ProductPriceCount { get; set; }

        public int? DocumentCount { get; set; }

        public List<Document> Documents { get; set; }
    }
}
