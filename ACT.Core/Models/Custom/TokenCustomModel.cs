
namespace ACT.Core.Models.Custom
{
    public partial class TokenCustomModel
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public System.DateTime ModifiedOn { get; set; }
        public string ModifiedBy { get; set; }
        public System.Guid UID { get; set; }
        public int Status { get; set; }
    }
}
