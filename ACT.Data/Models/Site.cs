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
    
    public partial class Site
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Site()
        {
            this.ChepAudits = new HashSet<ChepAudit>();
            this.ClientSites = new HashSet<ClientSite>();
            this.SiteAudits = new HashSet<SiteAudit>();
            this.SiteBillings = new HashSet<SiteBilling>();
            this.SiteBudgets = new HashSet<SiteBudget>();
            this.Site1 = new HashSet<Site>();
        }
    
        public int Id { get; set; }
        public Nullable<int> SiteId { get; set; }
        public Nullable<int> RegionId { get; set; }
        public Nullable<int> ARPMSalesManagerId { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public System.DateTime ModifiedOn { get; set; }
        public string ModifiedBy { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string XCord { get; set; }
        public string YCord { get; set; }
        public string Address { get; set; }
        public string Town { get; set; }
        public string PostalCode { get; set; }
        public string ContactNo { get; set; }
        public string ContactName { get; set; }
        public string PlanningPoint { get; set; }
        public Nullable<int> SiteType { get; set; }
        public string AccountCode { get; set; }
        public string Depot { get; set; }
        public string SiteCodeChep { get; set; }
        public int Status { get; set; }
        public string FinanceContact { get; set; }
        public string FinanceContactNo { get; set; }
        public string ReceivingContact { get; set; }
        public string ReceivingContactNo { get; set; }
        public string DepotManager { get; set; }
        public string DepotManagerContact { get; set; }
        public string FinanceEmail { get; set; }
        public string ReceivingEmail { get; set; }
        public string DepotManagerEmail { get; set; }
        public Nullable<int> Province { get; set; }
        public string LocationNumber { get; set; }
        public string CLCode { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ChepAudit> ChepAudits { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ClientSite> ClientSites { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SiteAudit> SiteAudits { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SiteBilling> SiteBillings { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SiteBudget> SiteBudgets { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Site> Site1 { get; set; }
        public virtual Site Site2 { get; set; }
        public virtual User User { get; set; }
    }
}
