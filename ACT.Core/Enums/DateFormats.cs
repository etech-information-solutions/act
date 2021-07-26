
namespace ACT.Core.Enums
{
    using Attributes;
    
    [StringEnum]
    public enum DateFormats
    {
        [StringEnumDisplayText( "yyyy-MM-dd", true )]
        Date1,

        [StringEnumDisplayText( "yyyy/MM/dd", true )]
        Date2,

        [StringEnumDisplayText( "yyyy MM dd", true )]
        Date15,

        [StringEnumDisplayText( "yy/MM/dd", true )]
        Date3,

        [StringEnumDisplayText( "yy-MM-dd", true )]
        Date4,

        [StringEnumDisplayText( "yy MM dd", true )]
        Date16,

        [StringEnumDisplayText( "dd/MM/yy", true )]
        Date5,

        [StringEnumDisplayText( "dd-MM-yy", true )]
        Date6,

        [StringEnumDisplayText( "dd MM yy", true )]
        Date17,

        [StringEnumDisplayText( "dd-MM-yyyy", true )]
        Date7,

        [StringEnumDisplayText( "dd/MM/yyyy", true )]
        Date8,

        [StringEnumDisplayText( "dd MM yyyy", true )]
        Date18,

        [StringEnumDisplayText( "MM-dd-yyyy", true )]
        Date9,

        [StringEnumDisplayText( "MM/dd/yyyy", true )]
        Date10,

        [StringEnumDisplayText( "MM dd yyyy", true )]
        Date19,

        [StringEnumDisplayText( "MM/dd/yy", true )]
        Date11,

        [StringEnumDisplayText( "MM-dd-yy", true )]
        Date12,

        [StringEnumDisplayText( "MM dd yy", true )]
        Date20,


        [StringEnumDisplayText( "dd/MMM/yy", true )]
        Date13,

        [StringEnumDisplayText( "dd-MMM-yy", true )]
        Date22,

        [StringEnumDisplayText( "dd MMM yy", true )]
        Date21,


        [StringEnumDisplayText( "dd/MMM/yyyy", true )]
        Date14,

        [StringEnumDisplayText( "dd-MMM-yyyy", true )]
        Date23,

        [StringEnumDisplayText( "dd MMM yyyy", true )]
        Date24,
    }
}
