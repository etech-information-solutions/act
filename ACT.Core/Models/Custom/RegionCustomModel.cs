
namespace ACT.Core.Models.Custom
{
    using System;
    
    public partial class RegionCustomModel
    {
        public int Id { get; set; }
        public Nullable<int> RegionManagerId { get; set; }
        public int PSPId { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public System.DateTime ModifiedOn { get; set; }
        public string ModifiedBy { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Status { get; set; }
        public Nullable<int> Province { get; set; }


        public string PSPName { get; set; }
        public string RegionManagerEmail { get; set; }
        public string RegionManagerName { get; set; }
    }
}
