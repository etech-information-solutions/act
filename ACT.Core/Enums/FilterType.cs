using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ACT.Core.Attributes;

namespace ACT.Core.Enums
{
    [StringEnum]
    public enum FilterType
    {
        [StringEnumDisplayText( "None" )]
        None = -1,

        [StringEnumDisplayText( "Status" )]
        Status = 1,

        [StringEnumDisplayText( "Role" )]
        Role = 2,
    }
}
