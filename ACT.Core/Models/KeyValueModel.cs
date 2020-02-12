using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACT.Core.Models
{
    class KeyValueModel
    {
        public string TKey { get; set; }

        public string TValue { get; set; }
    }
    class IntKeyValueModel
    {
        public int TKey { get; set; }

        public int TValue { get; set; }
    }
    class IntStringKeyValueModel
    {
        public int TKey { get; set; }

        public string TValue { get; set; }
    }
}
