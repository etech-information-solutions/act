
namespace ACT.Core.Enums
{
    using Attributes;

    [StringEnum]
    public enum BalanceStatus
    {
        [UiIgnoreEnumValue]
        [StringEnumDisplayText( "None" )]
        None = -1,

        [StringEnumDisplayText( "Not Balanced" )]
        NotBalanced = 0,

        [StringEnumDisplayText( "Balanced" )]
        Balanced = 1,
    }
}
