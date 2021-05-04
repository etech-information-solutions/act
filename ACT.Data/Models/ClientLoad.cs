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
    
    public partial class ClientLoad
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ClientLoad()
        {
            this.ChepClients = new HashSet<ChepClient>();
            this.ClientAuthorisations = new HashSet<ClientAuthorisation>();
            this.ClientInvoices = new HashSet<ClientInvoice>();
            this.Journals = new HashSet<Journal>();
            this.Tasks = new HashSet<Task>();
        }
    
        public int Id { get; set; }
        public int ClientId { get; set; }
        public Nullable<int> VehicleId { get; set; }
        public Nullable<int> ClientSiteId { get; set; }
        public Nullable<int> TransporterId { get; set; }
        public Nullable<int> ToClientSiteId { get; set; }
        public Nullable<int> PODCommentId { get; set; }
        public Nullable<int> OutstandingReasonId { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public System.DateTime ModifiedOn { get; set; }
        public string ModifiedBy { get; set; }
        public string LoadNumber { get; set; }
        public Nullable<System.DateTime> LoadDate { get; set; }
        public Nullable<System.DateTime> EffectiveDate { get; set; }
        public Nullable<System.DateTime> NotifyDate { get; set; }
        public string AccountNumber { get; set; }
        public string ClientDescription { get; set; }
        public string DeliveryNote { get; set; }
        public string ReferenceNumber { get; set; }
        public string ReceiverNumber { get; set; }
        public string Equipment { get; set; }
        public Nullable<decimal> OriginalQuantity { get; set; }
        public Nullable<decimal> NewQuantity { get; set; }
        public bool ReconcileInvoice { get; set; }
        public Nullable<System.DateTime> ReconcileDate { get; set; }
        public string PODNumber { get; set; }
        public string PCNNumber { get; set; }
        public string PRNNumber { get; set; }
        public int Status { get; set; }
        public Nullable<int> PostingType { get; set; }
        public string THAN { get; set; }
        public Nullable<decimal> ReturnQty { get; set; }
        public Nullable<decimal> OutstandingQty { get; set; }
        public Nullable<decimal> DebriefQty { get; set; }
        public Nullable<decimal> AdminMovement { get; set; }
        public string ChepInvoiceNo { get; set; }
        public string ChepCompensationNo { get; set; }
        public Nullable<int> InvoiceStatus { get; set; }
        public Nullable<int> PODStatus { get; set; }
        public Nullable<int> OrderStatus { get; set; }
        public string UID { get; set; }
        public Nullable<decimal> TransporterLiableQty { get; set; }
        public string ClientLoadComment { get; set; }
        public string ClientLoadNotes { get; set; }
        public string CancelledReason { get; set; }
        public bool ManuallyMatchedLoad { get; set; }
        public string ManuallyMatchedUID { get; set; }
        public string PODComments { get; set; }
        public string PCNComments { get; set; }
        public string PRNComments { get; set; }
        public Nullable<int> PODCommentById { get; set; }
        public Nullable<int> PCNCommentById { get; set; }
        public Nullable<int> PRNCommentById { get; set; }
        public Nullable<System.DateTime> PODCommentDate { get; set; }
        public Nullable<System.DateTime> PCNCommentDate { get; set; }
        public Nullable<System.DateTime> PRNCommentDate { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ChepClient> ChepClients { get; set; }
        public virtual Client Client { get; set; }
        public virtual Client Client1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ClientAuthorisation> ClientAuthorisations { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ClientInvoice> ClientInvoices { get; set; }
        public virtual ClientSite ClientSite { get; set; }
        public virtual ClientSite ClientSite1 { get; set; }
        public virtual OutstandingReason OutstandingReason { get; set; }
        public virtual PODComment PODComment { get; set; }
        public virtual Transporter Transporter { get; set; }
        public virtual User User { get; set; }
        public virtual User User1 { get; set; }
        public virtual User User2 { get; set; }
        public virtual Vehicle Vehicle { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Journal> Journals { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Task> Tasks { get; set; }
    }
}
