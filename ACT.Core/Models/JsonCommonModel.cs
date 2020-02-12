using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACT.Core.Models
{
    class JsonCommonModel
    {
        public int Id { get; set; }

        public string ActionTable { get; set; }

        public string BeforeImage { get; set; }

        public string AfteImage { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime ModifiedOn { get; set; }
    }
}
