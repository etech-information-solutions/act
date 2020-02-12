
namespace ACT.Core.Enums
{
    using Attributes;

    [StringEnum]
    public enum Status
    {
        [UiIgnoreEnumValue]
        [StringEnumDisplayText( "All" )]
        All = -1,

        [StringEnumDisplayText( "Inactive" )]
        Inactive,

        [StringEnumDisplayText( "Active" )]
        Active,

        [StringEnumDisplayText( "Pending" )]
        Pending,

        [StringEnumDisplayText( "Suspended" )]
        Suspended,

        [StringEnumDisplayText( "Rejected" )]
        Rejected,

        NA
    }
}
