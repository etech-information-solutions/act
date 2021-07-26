using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACT.Core.Models.Custom
{
    public class ChepLoadChepCustomModel
    {
        public int Id { get; set; }
        public int ChepLoadId { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime ModifiedOn { get; set; }
        public string ModifiedBy { get; set; }
        public string PCN { get; set; }
        public string PRN { get; set; }
        public string DocketNumber { get; set; }
        public string Reference { get; set; }
        public int Quantity { get; set; }
        public int Status { get; set; }
    }
}
