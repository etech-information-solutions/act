
namespace ACT.Core.Enums
{
    using Attributes;

    [StringEnum]
    public enum AddressType
    {
        [UiIgnoreEnumValue]
        [StringEnumDisplayText( "All" )]
        All = 0,

        [StringEnumDisplayText( "Postal Address" )]
        Postal, // 1

        [StringEnumDisplayText( "Residential Address" )]
        Residential, // 2
    }
}
