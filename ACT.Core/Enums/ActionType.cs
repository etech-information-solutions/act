
namespace ACT.Core.Enums
{
    using Attributes;

    [StringEnum]
    public enum ActionType
    {
        [UiIgnoreEnumValue]       

        [StringEnumDisplayText( "None" )]
        None = 0,

        [StringEnumDisplayText("Email")]
        Email = 1,

        [StringEnumDisplayText("Notify")]
        Notify = 2,

       [StringEnumDisplayText("Other")]
        Other = 3
    }
}
