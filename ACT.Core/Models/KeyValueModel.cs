using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACT.Core.Models
{
    public class KeyValueModel
    {
        public string TKey { get; set; }

        public string TValue { get; set; }
    }
    public class IntKeyValueModel
    {
        public int TKey { get; set; }

        public int TValue { get; set; }
    }
    public class IntStringKeyValueModel
    {
        public int TKey { get; set; }

        public string TValue { get; set; }
    }
    public class CommonModel
    {
        public int Id { get; set; }

        public string Name { get; set; }
    }
}
