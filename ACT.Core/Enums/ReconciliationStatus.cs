
namespace ACT.Core.Enums
{
    using Attributes;

    [StringEnum]
    public enum ReconciliationStatus
    {
        [UiIgnoreEnumValue]
        [StringEnumDisplayText( "Unreconcilable" )]
        All = -1,

        [StringEnumDisplayText( "Unreconciled" )]
        Unreconciled = 0,

        [StringEnumDisplayText( "Reconciled" )]
        Reconciled = 1,

        [StringEnumDisplayText( "PCN Found" )]
        PCNFound = 3,
    }
}
