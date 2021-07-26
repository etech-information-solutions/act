using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ACT.UI.Models
{
    public class EstimatedLoadViewModel
    {
        public int Id { get; set; }

        public int? ObjectId { get; set; }

        public string ObjectType { get; set; }

        public int BudgetYear { get; set; }

        public int Total { get; set; }

        public decimal? January { get; set; }

        public decimal? February { get; set; }

        public decimal? March { get; set; }

        public decimal? April { get; set; }

        public decimal? May { get; set; }

        public decimal? June { get; set; }

        public decimal? July { get; set; }

        public decimal? August { get; set; }

        public decimal? September { get; set; }

        public decimal? October { get; set; }

        public decimal? November { get; set; }

        public decimal? December { get; set; }
    }
}