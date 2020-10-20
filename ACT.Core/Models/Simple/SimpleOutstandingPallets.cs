using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ACT.Core.Models.Simple
{
    public class SimpleOutstandingPallets
    {
        public int? SiteId { get; set; }
        public int? ClientId { get; set; }

        public string SiteName { get; set; }
        public string ClientName { get; set; }

        public decimal? Total { get; set; }

        public decimal? Month1 { get; set; }

        public decimal? Month2 { get; set; }

        public decimal? Month3 { get; set; }

        public decimal? Month4 { get; set; }

        public decimal? Past30 { get; set; }
    }
}