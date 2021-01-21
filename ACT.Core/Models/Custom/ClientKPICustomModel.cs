
namespace ACT.Core.Models.Custom
{
    using System;
    
    public partial class ClientKPICustomModel
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime ModifiedOn { get; set; }
        public string ModifiedBy { get; set; }
        public string KPIDescription { get; set; }
        public decimal? Disputes { get; set; }
        public decimal? OutstandingPallets { get; set; }
        public decimal? Passons { get; set; }
        public decimal? MonthlyCost { get; set; }
        public int Status { get; set; }
        public int? OutstandingDays { get; set; }
        public int? ResolveDays { get; set; }
        public decimal? Weight { get; set; }
        public decimal? TargetAmount { get; set; }
        public int? TargetPeriod { get; set; }

        public string ClientName { get; set; }
    }
}
