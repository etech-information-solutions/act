
namespace ACT.Core.Models.Custom
{
    using System;
    using System.Collections.Generic;
    using ACT.Data.Models;

    public partial class ClientCustomerCustomModel
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public System.DateTime ModifiedOn { get; set; }
        public string Modifiedby { get; set; }
        public string CustomerName { get; set; }
        public int Status { get; set; }

        public string ClientName { get; set; }
    }
}
