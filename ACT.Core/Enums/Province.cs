
namespace ACT.Core.Enums
{
    using Attributes;

    [StringEnum]
    public enum Province
    {
        [UiIgnoreEnumValue]
        [StringEnumDisplayText( "All" )]
        All = -1,

        [StringEnumDisplayText( "Eastern Cape" )]
        EasternCape = 0,

        [StringEnumDisplayText( "Free State" )]
        FreeState = 1,

        [StringEnumDisplayText( "Gauteng" )]
        Gauteng = 2,

        [StringEnumDisplayText( "KwaZulu-Natal" )]
        KwaZuluNatal = 3,

        [StringEnumDisplayText( "Limpopo" )]
        Limpopo = 4,

        [StringEnumDisplayText( "Mpumalanga" )]
        Mpumalanga = 5,

        [StringEnumDisplayText( "Northern Cape" )]
        NorthernCape = 6,

        [StringEnumDisplayText( "North West" )]
        NorthWest = 7,

        [StringEnumDisplayText( "Western Cape" )]
        WesternCape = 8
    }
}
