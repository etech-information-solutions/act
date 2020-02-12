
namespace ACT.Core.Models.Custom
{
    using System;
    
    public partial class SiteBillingCustomModel
    {
        public int Id { get; set; }
        public int SiteId { get; set; }
        public int ProductId { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public System.DateTime ModifiedOn { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> BillingDate { get; set; }
        public Nullable<int> BillingPeriod { get; set; }
        public Nullable<decimal> ProductQuantity { get; set; }
        public Nullable<decimal> ProductAmount { get; set; }
        public string PastelCode { get; set; }
        public int Status { get; set; }
    }
}
