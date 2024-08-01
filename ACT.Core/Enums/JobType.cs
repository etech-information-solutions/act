
namespace ACT.Core.Enums
{
    using Attributes;

    [StringEnum]
    public enum JobType
    {
        [UiIgnoreEnumValue]
        [StringEnumDisplayText( "None" )]
        None = -1,

        [StringEnumDisplayText( "General Account" )]
        General = 0,

        [StringEnumDisplayText( "Finance Manager" )]
        Finance = 1,

        [StringEnumDisplayText( "Key Account Manager" )]
        KeyAccount = 2,

        [StringEnumDisplayText( "Authorisation Contact" )]
        Authorisation = 3,

        [StringEnumDisplayText( "Depot Manager" )]
        DepotManager = 4,

        [StringEnumDisplayText( "Client Sales" )]
        ClientSales = 5,

        [StringEnumDisplayText( "Client Manager" )]
        ClientManager = 6,

        [StringEnumDisplayText( "Client Sales Rep" )]
        ClientSalesRep = 7,

        [StringEnumDisplayText( "Management" )]
        Management = 8,

        [StringEnumDisplayText( "Director" )]
        Director = 9,

        [StringEnumDisplayText( "Administration" )]
        Administration = 10,
        
    }
}
