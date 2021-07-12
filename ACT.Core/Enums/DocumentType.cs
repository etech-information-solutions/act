
namespace ACT.Core.Enums
{
    using Attributes;

    [StringEnum]
    public enum DocumentType
    {
        [StringEnumDisplayText( "N/A" )]
        None = -1,

        [StringEnumDisplayText( "ACT Control Doc" )]
        ACTControlDoc = 0,

        [StringEnumDisplayText( "Production Plant" )]
        ProductionPlant = 1,

        [StringEnumDisplayText( "Exchange Customer" )]
        ExchangeCustomer = 2,

        [StringEnumDisplayText( "THAN" )]
        THAN = 3,

        [StringEnumDisplayText( "Export Customer" )]
        ExportCustomer = 4,

        [StringEnumDisplayText( "NPD Customer" )]
        NPDCustomer = 5,

        [StringEnumDisplayText( "Depot to Depot" )]
        DepotToDepot = 6,
    }
}
