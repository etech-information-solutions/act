
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

        [StringEnumDisplayText( "Director" )]
        Director = 2,

        [StringEnumDisplayText( "Key Account Manager" )]
        KeyAccount = 3,

        [StringEnumDisplayText( "Finance" )]
        Finance = 4,
    }
}
