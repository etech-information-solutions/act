
namespace ACT.Core.Models.Custom
{
    using System;
    
    public partial class DocumentCustomModel
    {
        public int Id { get; set; }
        public Nullable<int> ObjectId { get; set; }
        public string ObjectType { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public int CreatedBy { get; set; }
        public System.DateTime ModifiedOn { get; set; }
        public int ModifiedBy { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public string Category { get; set; }
        public string Type { get; set; }
        public int Status { get; set; }
        public double Size { get; set; }
        public string Url { get; set; }
    }
}
