
namespace ACT.Core.Enums
{
    using Attributes;

    [StringEnum]
    public enum RewardStatus
    {
        [UiIgnoreEnumValue]
        [StringEnumDisplayText( "Paid" )]
        All = -1,
        
        [StringEnumDisplayText( "Paid" )]
        Paid = 0,

        [StringEnumDisplayText( "In-Progress" )]
        InProgress = 1,

        [StringEnumDisplayText( "Available" )]
        Available = 2,

        [StringEnumDisplayText( "Cancelled" )]
        Cancelled = 3,
    }
}
