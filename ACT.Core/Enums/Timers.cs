
namespace ACT.Core.Enums
{
    using Attributes;

    [StringEnum]
    public enum Timers
    {
        [UiIgnoreEnumValue]
        [StringEnumDisplayText( "All" )]
        All = -1,

        [StringEnumDisplayText( "Dispute Monitor Timer" )]
        DisputeMonitorTimer = 1,

        [StringEnumDisplayText( "Client Monitor Timer" )]
        ClientMonitorTimer = 2,
    }
}
