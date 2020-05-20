//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ACT.Data.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class DeliveryNote
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public DeliveryNote()
        {
            this.DeliveryNoteLines = new HashSet<DeliveryNoteLine>();
        }
    
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
        public Nullable<int> ClientSiteId { get; set; }
    
        public virtual Client Client { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DeliveryNoteLine> DeliveryNoteLines { get; set; }
    }
}
