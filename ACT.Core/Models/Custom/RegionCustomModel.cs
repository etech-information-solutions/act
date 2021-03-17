
namespace ACT.Core.Models.Custom
{
    using System;
    
    public partial class RegionCustomModel
    {
        public int Id { get; set; }
        public int? RegionManagerId { get; set; }
        public int PSPId { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime ModifiedOn { get; set; }
        public string ModifiedBy { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Status { get; set; }
        public int? Province { get; set; }


        public string PSPName { get; set; }
        public string RegionManagerEmail { get; set; }
        public string RegionManagerName { get; set; }
    }
}
