
namespace ACT.Core.Enums
{
    using Attributes;

    [StringEnum]
    public enum DocumentType
    {
        [UiIgnoreEnumValue]
        [StringEnumDisplayText( "All" )]
        All = -1,

        [StringEnumDisplayText( "Email" )]
        Email, // 0

        [StringEnumDisplayText( "Quotation" )]
        Quotation, // 1

        [StringEnumDisplayText( "Invoice/Receipt" )]
        Invoice, // 2

        [StringEnumDisplayText( "Contract" )]
        Contract, // 3

        [StringEnumDisplayText( "Statement" )]
        Statement, // 4

        //[StringEnumDisplayText( "Receipt" )]
        //Receipt,

        [StringEnumDisplayText( "Refund on Advancement (Proof of Payment)" )]
        Refund, // 5

        [StringEnumDisplayText( "Budget" )]
        Budget, // 6

        [UiIgnoreEnumValue]
        [StringEnumDisplayText( "Provincial Refund" )]
        ProvincialRefund, // 7

        [UiIgnoreEnumValue]
        [StringEnumDisplayText( "Write Off" )]
        WriteOff, // 8
    }
}
