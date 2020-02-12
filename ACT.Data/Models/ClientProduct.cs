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
    
    public partial class ClientProduct
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ClientProduct()
        {
            this.ClientProductMonthlies = new HashSet<ClientProductMonthly>();
        }
    
        public int Id { get; set; }
        public int ClientId { get; set; }
        public int ProductId { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public System.DateTime ModifiedOn { get; set; }
        public string ModifiedBy { get; set; }
        public string ProductDescription { get; set; }
        public Nullable<System.DateTime> ActiveDate { get; set; }
        public Nullable<decimal> HireRate { get; set; }
        public Nullable<decimal> LostRate { get; set; }
        public Nullable<decimal> IssueRate { get; set; }
        public Nullable<decimal> PassonRate { get; set; }
        public Nullable<int> PassonDays { get; set; }
        public int Status { get; set; }
    
        public virtual Client Client { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ClientProductMonthly> ClientProductMonthlies { get; set; }
        public virtual PSPProduct PSPProduct { get; set; }
    }
}
