
namespace ACT.Core.Enums
{
    using Attributes;

    [StringEnum]
    public enum SiteType
    {
        [UiIgnoreEnumValue]
        [StringEnumDisplayText("Transfer Customer")]
        TransferCustomer = 1,
    
        [StringEnumDisplayText("Exchange Customer")]
        ExchangeCustomer = 2,

        [StringEnumDisplayText("Depot")]
        Depot = 3, 
    }
}
