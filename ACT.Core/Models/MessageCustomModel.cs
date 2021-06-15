using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACT.Core.Models
{
    public class MessageCustomModel
    {
        public int Id { get; set; }
        public int TicketId { get; set; }
        public int SenderUserId { get; set; }
        public int? ReceiverUserId { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public System.DateTime ModifiedOn { get; set; }
        public string ModifiedBy { get; set; }
        public string Details { get; set; }
        public System.Guid UID { get; set; }
        public bool IsClose { get; set; }
        public bool IsSupport { get; set; }
        public int Status { get; set; }


        public int Rating { get; set; }
        public int DepartmentId { get; set; }



        public string Email { get; set; }
        public string APIKey { get; set; }
    }
}
