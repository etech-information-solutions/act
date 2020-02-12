
namespace ACT.Core.Models.Custom
{
    public partial class ClientAuthorisationCustomModel
    {
        public int Id { get; set; }
        public int ClientSiteId { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public System.DateTime ModifiedOn { get; set; }
        public string ModifiedBy { get; set; }
        public string AuthorisationCode { get; set; }
        public string LoadNumber { get; set; }
        public int Status { get; set; }
    }
}
