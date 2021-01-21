
namespace ACT.Core.Models.Custom
{
    using System;
    
    public partial class ProductPriceCustomModel
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime ModifiedOn { get; set; }
        public string ModifiedBy { get; set; }
        public decimal Rate { get; set; }
        public int? RateUnit { get; set; }
        public DateTime? FromDate { get; set; }
        public int Status { get; set; }
    }
}
