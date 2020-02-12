
namespace ACT.Core.Models.Custom
{
    public partial class ContactCustomModel
    {
        public int Id { get; set; }
        public int ObjectId { get; set; }
        public string ObjectType { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public System.DateTime ModifiedOn { get; set; }
        public string ModifiedBy { get; set; }
        public string ContactName { get; set; }
        public string ContactTitle { get; set; }
        public string ContactIdNo { get; set; }
        public string ContactCell { get; set; }
        public string ContactEmail { get; set; }
        public int Status { get; set; }
    }
}
