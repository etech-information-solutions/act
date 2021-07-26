
namespace ACT.Core.Enums
{
    using Attributes;

    [StringEnum]
    public enum CommonStatus
    {
        [UiIgnoreEnumValue]
        [StringEnumDisplayText( "None" )]
        None = -1,

        [StringEnumDisplayText( "Inactive" )]
        Inactive = 0,

        [StringEnumDisplayText( "Active" )]
        Active = 1,
    }
}
