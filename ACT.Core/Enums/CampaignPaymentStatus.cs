
namespace ACT.Core.Enums
{
    using Attributes;

    [StringEnum]
    public enum CampaignPaymentStatus
    {
        [UiIgnoreEnumValue]
        [StringEnumDisplayText( "All" )]
        All = -1,

        [StringEnumDisplayText( "Unpaid" )]
        Unpaid = 0,

        [StringEnumDisplayText( "Paid" )]
        Paid = 1
    }
}
