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
            this.ChepLoads = new HashSet<ChepLoad>();
            this.ChepLoadJournals = new HashSet<ChepLoadJournal>();
            this.ClientLoads = new HashSet<ClientLoad>();
            this.ClientLoads1 = new HashSet<ClientLoad>();
            this.DeliveryNotes = new HashSet<DeliveryNote>();
            this.ExtendedClientLoads = new HashSet<ExtendedClientLoad>();
        }
    
        public int Id { get; set; }
        public int ClientCustomerId { get; set; }
        public int SiteId { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public System.DateTime ModifiedOn { get; set; }
        public string ModifiedBy { get; set; }
        public string AccountingCode { get; set; }
        public int Status { get; set; }
        public string GLIDNo { get; set; }
        public Nullable<int> SiteType { get; set; }
        public string KAMName { get; set; }
        public string KAMContact { get; set; }
        public string KAMEmail { get; set; }
        public string ClientSalesManager { get; set; }
        public string ClientManagerContact { get; set; }
        public string ClientManagerEmail { get; set; }
        public string ClientSalesRep { get; set; }
        public string ClientSalesRepContact { get; set; }
        public string ClientSalesRegEmail { get; set; }
        public string ClientCustomerNumber { get; set; }
        public string LiquorLicenceNumber { get; set; }
        public string ClientSiteCode { get; set; }
        public string AuthorisationEmail1 { get; set; }
        public string AuthorisationEmail2 { get; set; }
        public string AuthorisationEmail3 { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ChepLoad> ChepLoads { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ChepLoadJournal> ChepLoadJournals { get; set; }
        public virtual ClientCustomer ClientCustomer { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ClientLoad> ClientLoads { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ClientLoad> ClientLoads1 { get; set; }
        public virtual Site Site { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DeliveryNote> DeliveryNotes { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ExtendedClientLoad> ExtendedClientLoads { get; set; }
    }
}
