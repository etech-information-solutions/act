
namespace ACT.Core.Enums
{
    using Attributes;

    [StringEnum]
    public enum InvoiceStatus
    {
        [UiIgnoreEnumValue]
        [StringEnumDisplayText( "All" )]
        All = -1,

        [StringEnumDisplayText( "Declined" )]
        Declined = 0,

        [StringEnumDisplayText( "Paid" )]
        Paid = 1,

        [StringEnumDisplayText( "Processing" )]
        Processing = 2,

        [UiIgnoreEnumValue]
        Processed = 3
    }
}
