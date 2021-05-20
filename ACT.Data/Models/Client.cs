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
    
    public partial class Client
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Client()
        {
            this.ClientBudgets = new HashSet<ClientBudget>();
            this.ClientChepAccounts = new HashSet<ClientChepAccount>();
            this.ClientCustomers = new HashSet<ClientCustomer>();
            this.ClientGroups = new HashSet<ClientGroup>();
            this.ClientKPIs = new HashSet<ClientKPI>();
            this.ClientProducts = new HashSet<ClientProduct>();
            this.ClientUsers = new HashSet<ClientUser>();
            this.DeliveryNotes = new HashSet<DeliveryNote>();
            this.PSPClients = new HashSet<PSPClient>();
            this.SiteAudits = new HashSet<SiteAudit>();
            this.Transporters = new HashSet<Transporter>();
            this.Tasks = new HashSet<Task>();
            this.ClientLoads = new HashSet<ClientLoad>();
            this.ClientLoads1 = new HashSet<ClientLoad>();
            this.ChepLoads = new HashSet<ChepLoad>();
            this.ChepLoadJournals = new HashSet<ChepLoadJournal>();
        }
    
        public int Id { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public System.DateTime ModifiedOn { get; set; }
        public string ModifiedBy { get; set; }
        public string CompanyRegistrationNumber { get; set; }
        public string CompanyName { get; set; }
        public string TradingAs { get; set; }
        public string Description { get; set; }
        public string VATNumber { get; set; }
        public string ContactNumber { get; set; }
        public string ContactPerson { get; set; }
        public string FinancialPerson { get; set; }
        public string Email { get; set; }
        public string AdminEmail { get; set; }
        public string DeclinedReason { get; set; }
        public int ServiceRequired { get; set; }
        public int Status { get; set; }
        public string AdminPerson { get; set; }
        public string FinPersonEmail { get; set; }
        public string ChepReference { get; set; }
        public Nullable<int> PalletType { get; set; }
        public string PalletTypeOther { get; set; }
        public string BBBEELevel { get; set; }
        public Nullable<int> CompanyType { get; set; }
        public string PSPName { get; set; }
        public Nullable<int> NumberOfLostPallets { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ClientBudget> ClientBudgets { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ClientChepAccount> ClientChepAccounts { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ClientCustomer> ClientCustomers { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ClientGroup> ClientGroups { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ClientKPI> ClientKPIs { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ClientProduct> ClientProducts { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ClientUser> ClientUsers { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DeliveryNote> DeliveryNotes { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PSPClient> PSPClients { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SiteAudit> SiteAudits { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Transporter> Transporters { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Task> Tasks { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ClientLoad> ClientLoads { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ClientLoad> ClientLoads1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ChepLoad> ChepLoads { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ChepLoadJournal> ChepLoadJournals { get; set; }
    }
}
