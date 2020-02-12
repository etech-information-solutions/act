
namespace ACT.Core.Models.Custom
{
    public partial class NotificationCustomModel
    {
        public int Id { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public System.DateTime ModifiedOn { get; set; }
        public string ModifiedBy { get; set; }
        public string Notification1 { get; set; }
        public System.DateTime NotificationDate { get; set; }
        public bool Status { get; set; }
        public string SentBy { get; set; }
    }
}
