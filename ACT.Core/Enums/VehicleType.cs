
namespace ACT.Core.Enums
{
    using Attributes;

    [StringEnum]
    public enum VehicleType
    {
        [UiIgnoreEnumValue]

        [StringEnumDisplayText( "Other" )]
        Other = 0,

        [StringEnumDisplayText( "Pickup" )]
        Pickup = 1,

        [StringEnumDisplayText( "Van" )]
        Van = 2,

        [StringEnumDisplayText( "HGV" )]
        HGV = 3,

        [StringEnumDisplayText( "Refrigerated Box Truck" )]
        RefrigeratedBoxTruck = 4,

        [StringEnumDisplayText( "Interlink" )]
        Interlink = 5
    }
}
