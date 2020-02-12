using ACT.Core.Attributes;

namespace ACT.Core.Enums
{
    [StringEnum]
    public enum CancellationReason
    {
        [UiIgnoreEnumValue]
        [StringEnumDisplayText( "All" )]
        All = -1,

        [StringEnumDisplayText( "Not Interested" )]
        NotInterested = 1, // 1

        [StringEnumDisplayText( "Insufficient Funds" )]
        InsufficientFunds = 2, // 2

        [StringEnumDisplayText( "Bank Account details invalid" )]
        InvalidBank = 3, // 3

        [StringEnumDisplayText( "Other" )]
        Other = 99, // 4
    }
}
