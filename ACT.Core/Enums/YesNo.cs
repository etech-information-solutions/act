
namespace ACT.Core.Enums
{
    using Attributes;

    [StringEnum]
    public enum YesNo
    {
        [StringEnumDisplayText( "Yes" )]
        Yes = 1,

        [StringEnumDisplayText( "No" )]
        No = 0,
    }
}
