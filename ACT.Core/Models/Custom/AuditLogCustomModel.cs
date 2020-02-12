using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ACT.Data.Models;

namespace ACT.Core.Models.Custom
{
    public class AuditLogCustomModel
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int? ObjectId { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime ModifiedOn { get; set; }
        public string ModifiedBy { get; set; }
        public int Type { get; set; }
        public string ActionTable { get; set; }
        public bool IsAjaxRequest { get; set; }
        public string Parameters { get; set; }
        public string Controller { get; set; }
        public string Action { get; set; }
        public string Comments { get; set; }
        public string Browser { get; set; }
        public string BeforeImage { get; set; }
        public string AfterImage { get; set; }

        public User User { get; set; }
    }
}
