
namespace ACT.Core.Enums
{
    using Attributes;

    [StringEnum]
    public enum ProductPriceType
    {
        [StringEnumDisplayText( "Hire Fee" )]
        Hire = 0,

        [StringEnumDisplayText( "Issue Fee" )]
        Issue = 1,

        [StringEnumDisplayText( "Compensation Fee" )]
        Lost = 2,

        [StringEnumDisplayText("Recovery Fee")]
        Recovery = 3,

        [StringEnumDisplayText("Transport Fee")]
        Transport = 4

    }
}
