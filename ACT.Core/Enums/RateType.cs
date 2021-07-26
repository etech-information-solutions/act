
namespace ACT.Core.Enums
{
    using Attributes;

    [StringEnum]
    public enum RateType
    {
        [UiIgnoreEnumValue]

        [StringEnumDisplayText( "None" )]
        None = -1,

        [StringEnumDisplayText( "Weekly" )]
        Weekly = 0,

        [StringEnumDisplayText( "Monthly" )]
        Monthly = 1,

        [StringEnumDisplayText( "Annually" )]
        Annually = 2,

        [StringEnumDisplayText( "Daily" )]
        Daily = 3
    }
}
