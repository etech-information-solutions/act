
namespace ACT.Core.Models.Custom
{
    public partial class ChepClientCustomModel
    {
        public int Id { get; set; }
        public int ChepLoadsId { get; set; }
        public int ClientLoadsId { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public System.DateTime ModifiedOn { get; set; }
        public string ModifiedBy { get; set; }
        public int Status { get; set; }
    }
}
