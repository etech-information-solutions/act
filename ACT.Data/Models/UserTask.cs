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
    
    public partial class UserTask
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int TaskId { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public Nullable<System.DateTime> CompletedOn { get; set; }
        public int Status { get; set; }
    
        public virtual Task Task { get; set; }
        public virtual User User { get; set; }
    }
}
