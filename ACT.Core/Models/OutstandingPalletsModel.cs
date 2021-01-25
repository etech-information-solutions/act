using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ACT.Core.Models.Custom;

namespace ACT.Core.Models
{
    public class OutstandingPalletsModel
    {
        public int Total { get; set; }

        public ChepLoadCustomModel ClientLoad { get; set; }

        public OutstandingReasonModel GrandTotal { get; set; }

        public List<OutstandingRegionModel> Regions { get; set; }

        public List<OutstandingReasonModel> OutstandingReasons { get; set; }
    }

    public class OutstandingRegionModel
    {
        public int? Id { get; set; }

        public string Name { get; set; }

        public int Total { get; set; }

        public List<OutstandingSiteModel> Sites { get; set; }
    }

    public class OutstandingSiteModel
    {
        public int? Id { get; set; }

        public string Name { get; set; }

        public int Total { get; set; }

        public List<OutstandingReasonModel> OutstandingReasons { get; set; }
    }

    public class OutstandingReasonModel
    {
        public int Total { get; set; }

        public string Description { get; set; }

        public decimal? GrandTotal { get; set; }

        public decimal? To30Days { get; set; }

        public decimal? To60Days { get; set; }

        public decimal? To90Days { get; set; }

        public decimal? To120Days { get; set; }

        public decimal? To183Days { get; set; }

        public decimal? To270Days { get; set; }

        public decimal? To365Days { get; set; }
    }
}
