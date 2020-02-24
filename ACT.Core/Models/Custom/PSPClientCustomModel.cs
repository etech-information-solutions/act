
namespace ACT.Core.Models.Custom
{
    public partial class PSPClientCustomModel
    {
        public int Id { get; set; }
        public int PSPId { get; set; }
        public int ClientId { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public System.DateTime ModifiedOn { get; set; }
        public string ModifiedBy { get; set; }
        public int Status { get; set; }
    }
}
