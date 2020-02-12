
namespace ACT.Core.Enums
{
    using Attributes;

    [StringEnum]
    public enum Months
    {
        [UiIgnoreEnumValue]
        [StringEnumDisplayText( "All" )]
        All = -1,

        [StringEnumDisplayText( "January" )]
        January,

        [StringEnumDisplayText( "February" )]
        February,

        [StringEnumDisplayText( "March" )]
        March,

        [StringEnumDisplayText( "April" )]
        April,

        [StringEnumDisplayText( "May" )]
        May,

        [StringEnumDisplayText( "June" )]
        June,

        [StringEnumDisplayText( "July" )]
        July,

        [StringEnumDisplayText( "August" )]
        August,

        [StringEnumDisplayText( "September" )]
        September,

        [StringEnumDisplayText( "October" )]
        October,

        [StringEnumDisplayText( "November" )]
        November,

        [StringEnumDisplayText( "December" )]
        December
    }
}
