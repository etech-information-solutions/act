
using System.Collections.Generic;
using ACT.Data.Models;

namespace ACT.Core.Models.Custom
{
    public partial class ProductCustomModel
    {
        public int Id { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public System.DateTime ModifiedOn { get; set; }
        public string ModifiedBy { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Status { get; set; }

        public int? ProductPriceCount { get; set; }

        public int? DocumentCount { get; set; }
       
        public List<Document> Documents { get; set; }
    }
}
