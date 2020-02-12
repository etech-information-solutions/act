
namespace ACT.Core.Models.Custom
{
    using System;
    
    public partial class ClientBudgetCustomModel
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public System.DateTime ModifiedOn { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<int> BudgetYear { get; set; }
        public Nullable<int> January { get; set; }
        public Nullable<int> February { get; set; }
        public Nullable<int> March { get; set; }
        public Nullable<int> April { get; set; }
        public Nullable<int> May { get; set; }
        public Nullable<int> June { get; set; }
        public Nullable<int> July { get; set; }
        public Nullable<int> August { get; set; }
        public Nullable<int> September { get; set; }
        public Nullable<int> October { get; set; }
        public Nullable<int> November { get; set; }
        public Nullable<int> December { get; set; }
        public int Status { get; set; }
    }
}
