
namespace ACT.Core.Enums
{
    using Attributes;

    [StringEnum]
    public enum JournalType
    {
        [UiIgnoreEnumValue]
        [StringEnumDisplayText( "None" )]
        None = -1,

        [StringEnumDisplayText( "Transfer In" )]
        TransferIn = 0,

        [StringEnumDisplayText( "Transfer Out" )]
        TransferOut = 1,

        [StringEnumDisplayText( "Return" )]
        Return = 2,

        [StringEnumDisplayText( "Issue" )]
        Issue = 3,

        [StringEnumDisplayText( "Correction" )]
        Correction = 4,
    }
}
