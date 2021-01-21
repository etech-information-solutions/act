
namespace ACT.Core.Models.Custom
{
    using System;
    
    public partial class TaskCustomModel
    {
        public int Id { get; set; }
        public int? ClientId { get; set; }
        public int? ChepLoadId { get; set; }
        public int? ClientLoadId { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime ModifiedOn { get; set; }
        public string ModifiedBy { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime? Date { get; set; }
        public int? Action { get; set; }
        public int Status { get; set; }
    }
}
