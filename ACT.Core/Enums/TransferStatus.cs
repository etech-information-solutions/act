
namespace ACT.Core.Enums
{
    using Attributes;

    [StringEnum]
    public enum TransferStatus
    {
        [UiIgnoreEnumValue]
        [StringEnumDisplayText( "All" )]
        All = -1,

        [StringEnumDisplayText( "Awaiting Transfer" )]
        Awaiting = 0,

        [StringEnumDisplayText( "Transferring in Progress" )]
        Transferring = 1,

        [StringEnumDisplayText( "Transfer completed" )]
        Transferred = 2,
    }
}
