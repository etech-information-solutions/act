
namespace ACT.Core.Models.Custom
{
    using System;
    
    public partial class DeliveryNoteLineCustomModel
    {
        public int Id { get; set; }
        public int DeliveryNoteId { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime ModifiedOn { get; set; }
        public string ModifiedBy { get; set; }
        public string Product { get; set; }
        public string ProductDescription { get; set; }
        public decimal? OrderQuantity { get; set; }
        public decimal? Delivered { get; set; }
        public int Status { get; set; }
    }
}
