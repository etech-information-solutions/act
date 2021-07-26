
namespace ACT.Core.Models.Custom
{
    public partial class ClientGroupCustomModel
    {
        public int Id { get; set; }
        public int GroupId { get; set; }
        public int ClientId { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public System.DateTime ModifiedOn { get; set; }
        public string ModifiedBy { get; set; }
        public int Status { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

    }
}
