
namespace ACT.Core.Enums
{
    using Attributes;

    [StringEnum]
    public enum TypeOfPalletUse
    {
        [UiIgnoreEnumValue]

        [StringEnumDisplayText( "Pooling" )]
        Pooling = 0,

        [StringEnumDisplayText( "Own (Euro/Brown)" )]
        Own = 1,

        [StringEnumDisplayText( "Lug & Bins" )]
        LugBin = 2,

        [StringEnumDisplayText( "Other" )]
        Other = 3
    }
}
