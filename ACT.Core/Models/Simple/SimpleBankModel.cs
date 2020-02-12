using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ACT.Core.Enums;

namespace ACT.Core.Models.Simple
{
    public class SimpleBankModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public Status Status { get; set; }
    }
}
