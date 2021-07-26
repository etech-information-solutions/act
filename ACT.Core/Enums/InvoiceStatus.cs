
namespace ACT.Core.Enums
{
    using Attributes;

    [StringEnum]
    public enum InvoiceStatus
    {
        [UiIgnoreEnumValue]
        [StringEnumDisplayText( "All" )]
        All = -1,

        [StringEnumDisplayText( "N/A" )]
        NA = 0,

        [StringEnumDisplayText( "Updated" )]
        Updated = 1,
    }
}
