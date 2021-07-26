
namespace ACT.Core.Enums
{
    using Attributes;

    [StringEnum]
    public enum OnlineStatus
    {
        [UiIgnoreEnumValue]
        [StringEnumDisplayText( "None" )]
        None = -1,

        [StringEnumDisplayText( "Offline" )]
        Offline = 0,

        [StringEnumDisplayText( "Online" )]
        Online = 1,

        [StringEnumDisplayText( "Away" )]
        Away = 2,

        [StringEnumDisplayText( "Busy" )]
        Busy = 3,
    }
}
