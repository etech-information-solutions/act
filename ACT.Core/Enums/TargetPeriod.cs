
namespace ACT.Core.Enums
{
    using Attributes;
    [StringEnum]
    public enum TargetPeriod
    {
        [StringEnumDisplayText("Daily")]
        Daily,

        [StringEnumDisplayText("Weekly")]
        Weekly,

        [StringEnumDisplayText("Monthly")]
        Monthly,

        [StringEnumDisplayText("Annually")]
        Annually
    }
}
