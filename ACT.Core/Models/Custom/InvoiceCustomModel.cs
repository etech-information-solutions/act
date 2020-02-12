
namespace ACT.Core.Models.Custom
{
    using System;
    
    public partial class InvoiceCustomModel
    {
        public int Id { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public System.DateTime ModifiedOn { get; set; }
        public string ModifiedBy { get; set; }
        public string Number { get; set; }
        public Nullable<System.DateTime> Date { get; set; }
        public Nullable<decimal> Quantity { get; set; }
        public Nullable<decimal> Amount { get; set; }
        public string LoadNumber { get; set; }
        public int Status { get; set; }
    }
}
