
namespace ACT.Core.Enums
{
    using Attributes;

    [StringEnum]
    public enum BankAccountType
    {
        [UiIgnoreEnumValue]
        [StringEnumDisplayText( "All" )]
        All = -1,

        [StringEnumDisplayText("None")]
        None = 0,

        [StringEnumDisplayText("Current/Cheque")]
        Current = 1,

        [StringEnumDisplayText("Savings")]
        Savings = 2,

        [StringEnumDisplayText( "Transmission" )]
        Transmission = 3
    }
}
