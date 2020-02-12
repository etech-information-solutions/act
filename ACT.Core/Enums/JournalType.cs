
namespace ACT.Core.Enums
{
    using Attributes;

    [StringEnum]
    public enum JournalType
    {
        [UiIgnoreEnumValue]
        [StringEnumDisplayText( "All" )]
        All = -1,

        [StringEnumDisplayText( "Original" )]
        Original, // 0

        [StringEnumDisplayText( "Credit" )]
        Credit, // 1

        [StringEnumDisplayText( "Debit" )]
        Debit, // 2
    }
}
