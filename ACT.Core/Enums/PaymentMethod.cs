
namespace ACT.Core.Enums
{
    using Attributes;

    [StringEnum]
    public enum PaymentMethod
    {
        [StringEnumDisplayText( "Cellphone Wallet" )]
        Cell = 1, // 1

        [StringEnumDisplayText( "Bank Account" )]
        Bank = 2, // 2
    }
}
