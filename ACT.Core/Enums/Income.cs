
namespace ACT.Core.Enums
{
    using Attributes;

    [StringEnum]
    public enum Income
    {
        [UiIgnoreEnumValue]
        [StringEnumDisplayText( "All" )]
        All = -1,

        [StringEnumDisplayText( "R0.00 - R10,000.00" )]
        Low, // 0

        [StringEnumDisplayText( "R11,000.00 - R30,000.00" )]
        Middle, // 1

        [StringEnumDisplayText( "R31,000.00 - R50,000.00" )]
        High, // 2

        [StringEnumDisplayText( "R50,000.00+" )]
        Higher // 3
    }
}
