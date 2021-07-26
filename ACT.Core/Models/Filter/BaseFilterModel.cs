using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ACT.Core.Enums;

namespace ACT.Core.Models.Filter
{
    public class BaseFilterModel
    {
        public string Showing { get; set; }

        public FilterType FilterType { get; set; }


        public int Total { get; set; }

        public int Active { get; set; }

        public int Inactive { get; set; }
    }
}
