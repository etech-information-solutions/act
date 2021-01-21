
namespace ACT.Core.Models.Custom
{
    using System;
    
    public partial class InvoiceCustomModel
    {
        public int Id { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime ModifiedOn { get; set; }
        public string ModifiedBy { get; set; }
        public string Number { get; set; }
        public DateTime? Date { get; set; }
        public decimal? Quantity { get; set; }
        public decimal? Amount { get; set; }
        public string LoadNumber { get; set; }
        public int Status { get; set; }
    }
}
