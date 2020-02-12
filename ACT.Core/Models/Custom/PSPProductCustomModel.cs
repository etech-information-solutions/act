
namespace ACT.Core.Models.Custom
{
    using System;
    
    public partial class PSPProductCustomModel
    {
        public int Id { get; set; }
        public int PSPId { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public System.DateTime ModifiedOn { get; set; }
        public string ModifiedBy { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Nullable<System.DateTime> StartDate { get; set; }
        public Nullable<System.DateTime> EndDate { get; set; }
        public Nullable<decimal> Rate { get; set; }
        public Nullable<int> RateUnit { get; set; }
        public int Status { get; set; }
    }
}
