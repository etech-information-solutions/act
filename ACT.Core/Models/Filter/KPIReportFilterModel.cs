using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACT.Core.Models.Filter
{
    public class KPIReportFilterModel : BaseFilterModel
    {
        public int PODPerUser { get; set; }

        public int PODUploadLog { get; set; }

        public int AuthorizationCodeAudit { get; set; }

        public int AuditLogPerUser { get; set; }
    }
}
