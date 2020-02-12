
namespace ACT.Core.Enums
{
    using Attributes;

    [StringEnum]
    public enum Timers
    {
        [UiIgnoreEnumValue]
        [StringEnumDisplayText( "All" )]
        All = -1,

        [StringEnumDisplayText( "Member Monitor Timer" )]
        MemberMonitorTimer = 1,

        [StringEnumDisplayText( "Billing Monitor Timer" )]
        BillingMonitorTimer = 2,

        [StringEnumDisplayText( "Refund Monitor Timer" )]
        RefundMonitorTimer = 3,
    }
}
