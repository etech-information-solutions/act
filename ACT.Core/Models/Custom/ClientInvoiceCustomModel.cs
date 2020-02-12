
namespace ACT.Core.Models.Custom
{
    public partial class ClientInvoiceCustomModel
    {
        public int Id { get; set; }
        public int ClientLoadId { get; set; }
        public int InvoiceId { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public System.DateTime ModifiedOn { get; set; }
        public string ModifiedBy { get; set; }
        public int Status { get; set; }
    }
}
