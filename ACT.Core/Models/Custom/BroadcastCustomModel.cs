
namespace ACT.Core.Models.Custom
{
    using System;
    
    public partial class BroadcastCustomModel
    {
        public int Id { get; set; }
        public int? ObjectId { get; set; }
        public string ObjectType { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime ModifiedOn { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime StartDate { get; set; }
        public Nullable<DateTime> EndDate { get; set; }
        public string Message { get; set; }
        public int Status { get; set; }

        public int? XRead { get; set; }

        public string PSPName { get; set; }
        public string ClientName { get; set; }

        public int ResponseCode { get; set; }

        public string Description { get; set; }
    }
}
