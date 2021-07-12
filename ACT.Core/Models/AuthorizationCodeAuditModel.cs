using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACT.Core.Models
{
    public class AuthorizationCodeAuditModel
    {
        public int Id { get; set; }

        public string LoadNumber { get; set; }

        public DateTime LoadDate { get; set; }

        public string ToSiteName { get; set; }

        public string DeliveryNote { get; set; }

        public string CustomerName { get; set; }

        public string AuthorizerName { get; set; }

        public string TransporterName { get; set; }

        public string LastEditedByName { get; set; }

        public string AuthorisationCode { get; set; }

        public DateTime AuthorisationDate { get; set; }
    }
}
