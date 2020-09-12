
namespace ACT.Core.Models.Custom
{
    using System;
    using System.Collections.Generic;

    public partial class SiteCustomModel
    {
        public int Id { get; set; }
        public Nullable<int> SiteId { get; set; }
        public Nullable<int> RegionId { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public System.DateTime ModifiedOn { get; set; }
        public string ModifiedBy { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string XCord { get; set; }
        public string YCord { get; set; }
        public string Address { get; set; }
        public string PostalCode { get; set; }
        public string ContactNo { get; set; }
        public string ContactName { get; set; }
        public string PlanningPoint { get; set; }
        public Nullable<int> SiteType { get; set; }
        public string AccountCode { get; set; }
        public string Depot { get; set; }
        public string SiteCodeChep { get; set; }
        public int Status { get; set; }
        public string FinanceContact { get; set; }
        public string FinanceContactNo { get; set; }
        public string ReceivingContact { get; set; }
        public string ReceivingContactNo { get; set; }

        public string RegionName { get; set; }

        public int SubSiteCount { get; set; }

        public int ClientCount { get; set; }

        public int BudgetCount { get; set; }

        public List<string> Clients { get; set; }

        public List<string> SubSites { get; set; }
    }
}
