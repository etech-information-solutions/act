
namespace ACT.Core.Enums
{
    using Attributes;

    [StringEnum]
    public enum JobType
    {
        [UiIgnoreEnumValue]
        [StringEnumDisplayText( "None" )]
        None = -1,

        [StringEnumDisplayText( "Administration" )]
        Administration = 0,

        [StringEnumDisplayText( "Management" )]
        Management = 1,
    }
}
