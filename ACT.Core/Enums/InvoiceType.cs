
namespace ACT.Core.Enums
{
    using Attributes;

    [StringEnum]
    public enum InvoiceType
    {
        [StringEnumDisplayText("PSP")]
        PSP, // 1

        [StringEnumDisplayText("Client")]
        Client, // 2
    }
}
