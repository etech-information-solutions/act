
namespace ACT.Core.Enums
{
    using Attributes;

    [StringEnum]
    public enum PSPClientStatus
    {
        [UiIgnoreEnumValue]
        [StringEnumDisplayText( "All" )]
        All = -1,

        [StringEnumDisplayText( "Unverified" )]
        Unverified = 0,

        [StringEnumDisplayText( "Verified" )]
        Verified = 1,

        [StringEnumDisplayText( "Rejected" )]
        Rejected = 2,

        [StringEnumDisplayText( "Inactive" )]
        Inactive = 3,

        [UiIgnoreEnumValue]
        NA
    }
}
