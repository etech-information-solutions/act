
namespace ACT.Core.Models.Custom
{
    using System;
    
    public partial class PSPProductCustomModel
    {
        public int Id { get; set; }
        public int PSPId { get; set; }
        public int ProductId { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime ModifiedOn { get; set; }
        public string ModifiedBy { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public decimal? Rate { get; set; }
        public int? RateUnit { get; set; }
        public int Status { get; set; }

        public string PSPName { get; set; }
        public string ProductName { get; set; }
    }
}
