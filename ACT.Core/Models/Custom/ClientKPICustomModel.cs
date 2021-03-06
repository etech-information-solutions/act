
namespace ACT.Core.Models.Custom
{
    using System;
    
    public partial class ClientKPICustomModel
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public System.DateTime ModifiedOn { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<decimal> Disputes { get; set; }
        public Nullable<decimal> OutstandingPallets { get; set; }
        public Nullable<decimal> Passons { get; set; }
        public Nullable<decimal> MonthlyCost { get; set; }
        public int Status { get; set; }
    }
}
