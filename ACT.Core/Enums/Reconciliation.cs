
namespace ACT.Core.Enums
{
    using Attributes;

    [StringEnum]
    public enum Reconciliation
    {
        [UiIgnoreEnumValue]
        [StringEnumDisplayText( "Unreconcilable" )]
        Unreconcilable = -1,

        [StringEnumDisplayText( "Unreconciled" )]
        Unreconciled = 0,

        [StringEnumDisplayText( "Reconciled" )]
        Reconciled = 1,
    }
}
