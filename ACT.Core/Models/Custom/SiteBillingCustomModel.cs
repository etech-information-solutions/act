
namespace ACT.Core.Models.Custom
{
    using System;
    
    public partial class SiteBillingCustomModel
    {
        public int Id { get; set; }
        public int SiteId { get; set; }
        public int ProductId { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime ModifiedOn { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? BillingDate { get; set; }
        public int? BillingPeriod { get; set; }
        public decimal? ProductQuantity { get; set; }
        public decimal? ProductAmount { get; set; }
        public string PastelCode { get; set; }
        public int Status { get; set; }
    }
}
