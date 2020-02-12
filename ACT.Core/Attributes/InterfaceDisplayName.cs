using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace System.ComponentModel
{

    /*
     * For some reason the DisplayName attribute cannot be added to an interface
     * */
    [AttributeUsage( AttributeTargets.Interface )]
    public class InterfaceDisplayName : DisplayNameAttribute
    {

        public InterfaceDisplayName() : base() { }

        public InterfaceDisplayName( string displayName ) : base( displayName ) { }
    }
}
