
namespace ACT.Core.Models.Custom
{
    using System;
    
    public partial class ProductPriceCustomModel
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public System.DateTime ModifiedOn { get; set; }
        public string ModifiedBy { get; set; }
        public decimal Rate { get; set; }
        public Nullable<int> RateUnit { get; set; }
        public Nullable<System.DateTime> FromDate { get; set; }
        public int Status { get; set; }
    }
}
