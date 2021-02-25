
namespace ACT.Core.Enums
{
    using Attributes;

    [StringEnum]
    public enum RoleType
    {
        [UiIgnoreEnumValue]
        [StringEnumDisplayText( "All / NA" )]
        All = -1,

        [UiIgnoreEnumValue]
        [StringEnumDisplayText( "Client" )]
        Client = 1,

        [StringEnumDisplayText( "Pooling Service Provider (PSP)" )]
        PSP = 2,

        [StringEnumDisplayText( "Operator" )]
        Operator = 3,

        //[StringEnumDisplayText( "Administrator" )]
        //Administrator = 4,

        [StringEnumDisplayText( "Super Admin" )]
        SuperAdmin = 5,

        [StringEnumDisplayText( "Transporter" )]
        Transporter = 6,

        [StringEnumDisplayText( "ARPM Sales Manager" )]
        ARPMSalesManager = 7,
    }
}
