using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ACT.Core.Interfaces
{
    public interface IIdentifyable
    {
        object ID { get; set; }
    }

    public interface IIdentifyable<TKey> : IIdentifyable
    {
        new TKey ID { get; set; }
    }
}
