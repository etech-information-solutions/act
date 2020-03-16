
namespace ACT.Core.Enums
{
    using Attributes;

    [StringEnum]
    public enum SiteType
    {
        [UiIgnoreEnumValue]
        [StringEnumDisplayText( "MainSite" )]
        MainSite = 1,
    
        [StringEnumDisplayText( "SubSite" )]
        SubSite = 2, // 1 = Client

        [StringEnumDisplayText("Depot")]
        Depot = 3, // 1 = Client

        [StringEnumDisplayText("WareHouse")]
        WareHouse = 4, // 1 = Client

        [StringEnumDisplayText("Shop")]
        Shop = 5, // 1 = Client
    }
}
