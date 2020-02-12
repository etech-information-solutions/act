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

        public Nullable<int> January { get; set; }

        public Nullable<int> February { get; set; }

        public Nullable<int> March { get; set; }

        public Nullable<int> April { get; set; }

        public Nullable<int> May { get; set; }

        public Nullable<int> June { get; set; }

        public Nullable<int> July { get; set; }

        public Nullable<int> August { get; set; }

        public Nullable<int> September { get; set; }

        public Nullable<int> October { get; set; }

        public Nullable<int> November { get; set; }

        public Nullable<int> December { get; set; }
    }
}