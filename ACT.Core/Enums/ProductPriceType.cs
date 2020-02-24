
namespace ACT.Core.Enums
{
    using Attributes;
    [StringEnum]
    public enum ProductPriceType
    {
        [StringEnumDisplayText( "Hire" )]
        Hire = 0,

        [StringEnumDisplayText( "Issue" )]
        Issue = 1,

        [StringEnumDisplayText( "Lost Pallets" )]
        Lost = 2,
    }
}
