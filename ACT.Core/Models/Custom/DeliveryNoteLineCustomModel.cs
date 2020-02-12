
namespace ACT.Core.Models.Custom
{
    using System;
    
    public partial class DeliveryNoteLineCustomModel
    {
        public int Id { get; set; }
        public int DeliveryNoteId { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public System.DateTime ModifiedOn { get; set; }
        public string ModifiedBy { get; set; }
        public string Product { get; set; }
        public string ProductDescription { get; set; }
        public Nullable<decimal> OrderQuantity { get; set; }
        public Nullable<decimal> Delivered { get; set; }
        public int Status { get; set; }
    }
}
