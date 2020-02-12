
namespace ACT.Core.Enums
{
    using Attributes;
    [StringEnum]
    public enum Title
    {
        [StringEnumDisplayText( "Mr" )]
        Mr,

        [StringEnumDisplayText( "Mrs" )]
        Mrs,

        [StringEnumDisplayText( "Ms" )]
        Ms,

        [StringEnumDisplayText( "Miss" )]
        Miss,

        [StringEnumDisplayText( "Mx" )]
        Mx,

        [StringEnumDisplayText( "Master" )]
        Master,

        [StringEnumDisplayText( "Maid" )]
        Maid,

        [StringEnumDisplayText( "Madam" )]
        Madam
    }
}
