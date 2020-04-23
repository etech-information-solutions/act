
namespace ACT.Core.Models.Custom
{
    using System;
    using System.Collections.Generic;

    public partial class DeliveryNoteCustomModel
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public System.DateTime ModifiedOn { get; set; }
        public string ModifiedBy { get; set; }
        public string CustomerName { get; set; }
        public string CustomerAddress { get; set; }
        public string CustomerPostalCode { get; set; }
        public Nullable<int> CustomerProvince { get; set; }
        public string DeliveryAddress { get; set; }
        public string DeliveryPostalCode { get; set; }
        public Nullable<int> DeliveryProvince { get; set; }
        public string InvoiceNumber { get; set; }
        public string OrderNumber { get; set; }
        public Nullable<System.DateTime> OrderDate { get; set; }
        public string BillingAddress { get; set; }
        public string BililngPostalCode { get; set; }
        public Nullable<int> BillingProvince { get; set; }
        public string EmailAddress { get; set; }
        public string ContactNumber { get; set; }
        public string Reference306 { get; set; }
        public int Status { get; set; }

        public List<DeliveryNoteLineCustomModel> DeliveryNoteLines { get; set; }
        public int CountNoteLines { get; set; }
    }
}
