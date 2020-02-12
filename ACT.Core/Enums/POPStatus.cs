
namespace ACT.Core.Enums
{
    using Attributes;

    [StringEnum]
    public enum POPStatus
    {
        [UiIgnoreEnumValue]
        [StringEnumDisplayText( "All" )]
        All = -1,

        [StringEnumDisplayText( "Outstanding" )]
        Outstanding,

        [StringEnumDisplayText( "Processing" )]
        Processing,

        [StringEnumDisplayText( "Complete" )]
        Complete
    }
}