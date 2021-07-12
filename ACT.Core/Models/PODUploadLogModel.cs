using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACT.Core.Models
{
    public class PODUploadLogModel
    {
        public int Id { get; set; }

        public string UserName { get; set; }

        public string PODLinks { get; set; }

        public string LoadNumber { get; set; }

        public DateTime LoadDate { get; set; }

        public DateTime PODCommentDate { get; set; }

        public string ToSiteName { get; set; }

        public string DeliveryNote { get; set; }
    }
}
