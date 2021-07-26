
namespace ACT.Core.Enums
{
    using Attributes;

    [StringEnum]
    public enum SiteType
    {
        [UiIgnoreEnumValue]
        [StringEnumDisplayText( "Transfer Customer" )]
        All = -1,

        [StringEnumDisplayText( "Depot" )]
        Depot = 0,

        [StringEnumDisplayText( "Manufacturer" )]
        Manufacturer = 1,

        [StringEnumDisplayText( "Retailer" )]
        Retailer = 2,

        [StringEnumDisplayText( "Transporter Depot" )]
        TransporterDepot = 3,

        [StringEnumDisplayText( "Grains" )]
        Grains = 4,

        [StringEnumDisplayText( "Groceries" )]
        Groceries = 5,

        [StringEnumDisplayText( "Mills" )]
        Mills = 6
    }
}
