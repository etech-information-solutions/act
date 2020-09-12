
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
    }
}
