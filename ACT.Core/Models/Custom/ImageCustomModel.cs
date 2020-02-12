
namespace ACT.Core.Models.Custom
{
    public partial class ImageCustomModel
    {
        public int Id { get; set; }
        public int ObjectId { get; set; }
        public string ObjectType { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public int CreatedBy { get; set; }
        public System.DateTime ModifiedOn { get; set; }
        public int ModifiedBy { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public string Name { get; set; }
        public string Extension { get; set; }
        public decimal Size { get; set; }
        public string ExternalUrl { get; set; }
        public bool IsMain { get; set; }
        public string Mime { get; set; }
    }
}
