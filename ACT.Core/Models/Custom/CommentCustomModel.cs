
namespace ACT.Core.Models.Custom
{
    public partial class CommentCustomModel
    {
        public int Id { get; set; }
        public int ObjectId { get; set; }
        public string ObjectType { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public System.DateTime ModifiedOn { get; set; }
        public string ModifiedBy { get; set; }
        public string Details { get; set; }
        public int Status { get; set; }
    }
}
