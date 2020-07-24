using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ACT.UI.Models
{
    public class EstimatedLoadViewModel
    {
        public int Id { get; set; }

        public Nullable<int> ObjectId { get; set; }

        public string ObjectType { get; set; }

        public int BudgetYear { get; set; }

        public Nullable<decimal> January { get; set; }

        public Nullable<decimal> February { get; set; }

        public Nullable<decimal> March { get; set; }

        public Nullable<decimal> April { get; set; }

        public Nullable<decimal> May { get; set; }

        public Nullable<decimal> June { get; set; }

        public Nullable<decimal> July { get; set; }

        public Nullable<decimal> August { get; set; }

        public Nullable<decimal> September { get; set; }

        public Nullable<decimal> October { get; set; }

        public Nullable<decimal> November { get; set; }

        public Nullable<decimal> December { get; set; }
    }
}