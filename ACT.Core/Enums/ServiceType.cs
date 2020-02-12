
namespace ACT.Core.Enums
{
    using Attributes;

    [StringEnum]
    public enum ServiceType
    {
        [UiIgnoreEnumValue]
        [StringEnumDisplayText( "All" )]
        All = -1,

        [StringEnumDisplayText( "We would like to manage our own pallets" )]
        ManageOwnPallets = 1, // 1 = Client

        [StringEnumDisplayText( "We provide pallet management services to clients" )]
        ProvidePalletManagement = 2, // 2 = Client

        [StringEnumDisplayText( "We need a company to manage our own pallets" )]
        RequirePalletManagement = 3, // 3 = PSP

        [StringEnumDisplayText( "We have a company managing our services" )]
        HaveCompany = 4, // 4 = PSP
    }
}
