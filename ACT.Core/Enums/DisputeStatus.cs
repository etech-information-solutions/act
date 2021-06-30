
namespace ACT.Core.Enums
{
    using Attributes;

    [StringEnum]
    public enum DisputeStatus
    {
        [UiIgnoreEnumValue]
        [StringEnumDisplayText( "All" )]
        All = -1,

        [StringEnumDisplayText( "Active" )]
        Active = 1, 

        [StringEnumDisplayText( "Resolved" )]
        Resolved = 2,

        [StringEnumDisplayText( "Cancelled" )]
        Cancelled = 3,
    }
}
