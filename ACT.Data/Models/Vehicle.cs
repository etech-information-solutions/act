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
    
    public partial class Vehicle
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Vehicle()
        {
            this.ClientLoads = new HashSet<ClientLoad>();
        }
    
        public int Id { get; set; }
        public int ObjectId { get; set; }
        public string ObjectType { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public System.DateTime ModifiedOn { get; set; }
        public string ModifiedBy { get; set; }
        public string Make { get; set; }
        public string Model { get; set; }
        public Nullable<int> Year { get; set; }
        public string EngineNumber { get; set; }
        public string VINNumber { get; set; }
        public string Registration { get; set; }
        public string Descriptoin { get; set; }
        public Nullable<int> Type { get; set; }
        public int Status { get; set; }
        public string FleetNumber { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ClientLoad> ClientLoads { get; set; }
    }
}
