
namespace ACT.Core.Enums
{
    using Attributes;

    [StringEnum]
    public enum PaymentStatus
    {
        [UiIgnoreEnumValue]
        [StringEnumDisplayText( "All" )]
        All = -1,

        [StringEnumDisplayText( "Outstanding" )]
        Outstanding = 0,

        [StringEnumDisplayText( "In Progress" )]
        InProgress = 1,

        [StringEnumDisplayText( "Payment Successful" )]
        PaymentSuccessful = 2,

        [StringEnumDisplayText( "Payment Unsuccessful" )]
        PaymentUnsuccessful = 3,

        [StringEnumDisplayText( "Journalised" )]
        Journalised = 4
    }
}
