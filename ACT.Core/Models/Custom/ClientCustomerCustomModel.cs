namespace ACT.Core.Models.Custom
{
    using System;

    public partial class ClientCustomerCustomModel
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime ModifiedOn { get; set; }
        public string ModifiedBy { get; set; }
        public string CustomerName { get; set; }
        public string CustomerNumber { get; set; }
        public string ContactNumber { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public string KeyAccountManager { get; set; }
        public string CustomerAddress1 { get; set; }
        public string CustomerTown { get; set; }
        public string CustomerPostalCode { get; set; }
        public int Status { get; set; }
        public string ClientName { get; set; }
    }
}