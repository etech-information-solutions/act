
namespace ACT.Core.Models.Custom
{
    public partial class ClientSiteCustomModel
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public int SiteId { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public System.DateTime ModifiedOn { get; set; }
        public string ModifiedBy { get; set; }
        public string AccountingCode { get; set; }
        public int Status { get; set; }
    }
}
