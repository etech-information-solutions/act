using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACT.Core.Models
{
    public class PODPerUserModel
    {
        public string UserName { get; set; }

        public int PODsUploaded { get; set; }

        public DateTime UploadDate { get; set; }
    }
}
