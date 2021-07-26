
namespace ACT.Core.Models.Custom
{
    using System;
    
    public partial class VehicleCustomModel
    {
        public int Id { get; set; }
        public int ObjectId { get; set; }
        public string ObjectType { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime ModifiedOn { get; set; }
        public string ModifiedBy { get; set; }
        public string Make { get; set; }
        public string Model { get; set; }
        public int? Year { get; set; }
        public string EngineNumber { get; set; }
        public string VINNumber { get; set; }
        public string Registration { get; set; }
        public string Descriptoin { get; set; }
        public int Type { get; set; }
        public int Status { get; set; }
    }
}
