using System;
using System.Collections.Generic;
using ACT.Core.Models.Custom;

namespace ACT.Core.Models
{
    public class OutstandingPalletsModel
    {
        public int Total { get; set; }

        public DateTime MinYear { get; set; }

        public ClientLoadCustomModel ClientLoad { get; set; }

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

        public int GrandTotal { get; set; }

        public int To30Days { get; set; }

        public int To60Days { get; set; }

        public int To90Days { get; set; }

        public int To120Days { get; set; }

        public int To183Days { get; set; }

        public int To270Days { get; set; }

        public int To365Days { get; set; }

        public List<PreviousYear> PreviousYears { get; set; }
    }

    public class PreviousYear
    {
        public int Year { get; set; }

        public int Total { get; set; }
    }
}
