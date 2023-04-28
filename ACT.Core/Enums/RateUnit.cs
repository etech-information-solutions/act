
namespace ACT.Core.Enums
{

    using Attributes;

    [StringEnum]
    public  enum RateUnit
    {
        [UiIgnoreEnumValue]

        [StringEnumDisplayText("None")]
        None = -1,

        [StringEnumDisplayText("Transaction")]
        Weekly = 0,

        [StringEnumDisplayText("Monthly")]
        Monthly = 1,

        [StringEnumDisplayText("Annually")]
        Annually = 2,
    }
}
