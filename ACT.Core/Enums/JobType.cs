
namespace ACT.Core.Enums
{
    using Attributes;

    [StringEnum]
    public enum JobType
    {
        [UiIgnoreEnumValue]
        [StringEnumDisplayText( "None" )]
        None = -1,

        [StringEnumDisplayText( "General Account" )]
        General = 0,

        [StringEnumDisplayText( "Finance" )]
        Finance = 1,

        [StringEnumDisplayText( "Key Account Manager" )]
        KeyAccount = 2,

        [StringEnumDisplayText( "Management" )]
        Management = 3,

        [StringEnumDisplayText( "Director" )]
        Director = 4,

        [StringEnumDisplayText( "Administration" )]
        Administration = 5,
        
    }
}
