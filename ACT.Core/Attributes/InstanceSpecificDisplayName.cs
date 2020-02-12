using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace ACT.Core.Attributes
{

    /*
    * It is a pity that .Net does not give us a handle on the Object on which the Attribute
    * was added somewhere along the line.  This implies that we cannot just override some method.
    * Instead we will need to do a custom implementation an make sure that get's used by UI :-(
    * */


    [AttributeUsage( AttributeTargets.Property )]
    public class InstanceSpecificDisplayName : DisplayNameAttribute
    {

        public InstanceSpecificDisplayName() : base() { }

        public virtual string GetDisplayName( object obj )
        {
            return DisplayName;
        }

    }
}
