
namespace ACT.Core.Enums
{
    using Attributes;

    [StringEnum]
    public enum QAStatus
    {
        [UiIgnoreEnumValue]
        [StringEnumDisplayText( "All" )]
        All = -1,

        [StringEnumDisplayText( "Unqualified" )]
        Unqualified = 0,

        [StringEnumDisplayText( "Allocated" )]
        Allocated = 1,
        
        [StringEnumDisplayText( "Qualified" )]
        Qualified = 2,

        [UiIgnoreEnumValue]
        Mixed = 3
    }
}
