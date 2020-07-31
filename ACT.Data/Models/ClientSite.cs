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
    
    public partial class ClientSite
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ClientSite()
        {
            this.ClientAuthorisations = new HashSet<ClientAuthorisation>();
            this.ClientLoads = new HashSet<ClientLoad>();
            this.DeliveryNotes = new HashSet<DeliveryNote>();
        }
    
        public int Id { get; set; }
        public int ClientId { get; set; }
        public int SiteId { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public System.DateTime ModifiedOn { get; set; }
        public string ModifiedBy { get; set; }
        public string AccountingCode { get; set; }
        public int Status { get; set; }
    
        public virtual Client Client { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ClientAuthorisation> ClientAuthorisations { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ClientLoad> ClientLoads { get; set; }
        public virtual Site Site { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DeliveryNote> DeliveryNotes { get; set; }
    }
}
