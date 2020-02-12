
namespace ACT.Core.Models.Custom
{
    public partial class AddressCustomModel
    {
        public int Id { get; set; }
        public int ObjectId { get; set; }
        public string ObjectType { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public int CreatedBy { get; set; }
        public System.DateTime ModifiedOn { get; set; }
        public int ModifiedBy { get; set; }
        public string Addressline1 { get; set; }
        public string Addressline2 { get; set; }
        public string Town { get; set; }
        public string PostalCode { get; set; }
        public string Longitude { get; set; }
        public string Latitude { get; set; }
        public int Province { get; set; }
        public int Type { get; set; }
        public int Status { get; set; }
    }
}
