
namespace ACT.Core.Models.Custom
{
    using System;
    using System.Collections.Generic;

    public partial class SiteCustomModel
    {
        public int Id { get; set; }
        public int? SiteId { get; set; }
        public int? RegionId { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime ModifiedOn { get; set; }
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
        public int? SiteType { get; set; }
        public string AccountCode { get; set; }
        public string Depot { get; set; }
        public string SiteCodeChep { get; set; }
        public int Status { get; set; }
        public string FinanceContact { get; set; }
        public string FinanceContactNo { get; set; }
        public string ReceivingContact { get; set; }
        public string ReceivingContactNo { get; set; }
        public string DepotManager { get; set; }
        public string DepotManagerContact { get; set; }
        public string FinanceEmail { get; set; }
        public string ReceivingEmail { get; set; }
        public string DepotManagerEmail { get; set; }
        public int? Province { get; set; }
        public string LocationNumber { get; set; }
        public int? ARPMSalesManagerId { get; set; }
        public string CLCode { get; set; }

        public string ClientName { get; set; }
        public string RegionName { get; set; }

        public int SubSiteCount { get; set; }

        public int ClientCount { get; set; }

        public int BudgetCount { get; set; }

        public List<string> Clients { get; set; }

        public List<string> SubSites { get; set; }
    }
}
