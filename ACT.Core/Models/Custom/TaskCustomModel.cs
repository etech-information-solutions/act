
namespace ACT.Core.Models.Custom
{
    using System;
    
    public partial class TaskCustomModel
    {
        public int Id { get; set; }
        public Nullable<int> ClientId { get; set; }
        public Nullable<int> ChepLoadId { get; set; }
        public Nullable<int> ClientLoadId { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public System.DateTime ModifiedOn { get; set; }
        public string ModifiedBy { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Nullable<System.DateTime> Date { get; set; }
        public Nullable<int> Action { get; set; }
        public int Status { get; set; }
    }
}
