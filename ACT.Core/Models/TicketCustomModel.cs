using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ACT.Data.Models;

namespace ACT.Core.Models
{
    public class TicketCustomModel
    {
        public int Id { get; set; }
        public int OwnerUserId { get; set; }
        public Nullable<int> SupportUserId { get; set; }
        public int DepartmentId { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public System.DateTime ModifiedOn { get; set; }
        public string ModifiedBy { get; set; }
        public int Rating { get; set; }
        public Nullable<System.DateTime> DateClosed { get; set; }
        public Nullable<int> ClosedByUserId { get; set; }
        public System.Guid UID { get; set; }
        public string Number { get; set; }
        public int Status { get; set; }



        public string Message { get; set; }
        public int MessageStatus { get; set; }
        public int MessageSenderUserId { get; set; }
        public int MessageReceiverUserId { get; set; }
        public DateTime? MessageCreatedOn { get; set; }



        public string OwnerName { get; set; }
        public string SupportName { get; set; }



        public string DepartmentName { get; set; }


        public List<Message> Messages { get; set; }
    }
}
