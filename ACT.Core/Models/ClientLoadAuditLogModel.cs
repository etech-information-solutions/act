using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ACT.Core.Models.Custom;

namespace ACT.Core.Models
{
    public class ClientLoadAuditLogModel
    {
        public ClientLoadCustomModel ClientLoad { get; set; }

        public ExtendedClientLoadCustomModel ExtendedClientLoad { get; set; }

        public int? ToSiteId { get; set; }
        public int? AuditLogId { get; set; }
        public int? FromSiteId { get; set; }
        public string ClientName { get; set; }
        public string ToSiteName { get; set; }
        public string PODComment { get; set; }
        public string DebtorsCode { get; set; }
        public string FromSiteName { get; set; }
        public string ToRegionCode { get; set; }
        public string ToRegionName { get; set; }
        public string AuditUserName { get; set; }
        public string AuditUserRole { get; set; }
        public string AuthorizerName { get; set; }
        public string FromRegionName { get; set; }
        public string FromRegionCode { get; set; }
        public string TransporterName { get; set; }
        public string TransporterNumber { get; set; }
        public string AuthorizationCode { get; set; }
        public string VehicleFleetNumber { get; set; }
        public string VehicleRegistration { get; set; }
        public string ClientLoadAfterImage { get; set; }
        public DateTime? AuditLogCreatedOn { get; set; }
        public DateTime? AuthorizationDate { get; set; }
        public string CustomerAccountNumber { get; set; }
        public DateTime? ClientLoadAuditDateTime { get; set; }
        public string ExtendedClientLoadAfterImage { get; set; }
        public DateTime? ExtendedClientLoadAuditDateTime { get; set; }

    }
}
