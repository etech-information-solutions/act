
namespace ACT.Core.Models.Custom
{
    public partial class ClientAuthorisationCustomModel
    {
        public int Id { get; set; }
        public int ClientSiteId { get; set; }
        public int TransporterId { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public System.DateTime ModifiedOn { get; set; }
        public string ModifiedBy { get; set; }
        public string Code { get; set; }
        public string LoadNumber { get; set; }
        public string DocketNumber { get; set; }
        public System.DateTime AuthorisationDate { get; set; }
        public int Status { get; set; }

        public string SiteName { get; set; }
        public string ClientName { get; set; }
        public string TransporterName { get; set; }
    }
}
