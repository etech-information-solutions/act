
namespace ACT.Core.Enums
{
    using Attributes;

    [StringEnum]
    public enum PostingType
    {
        [UiIgnoreEnumValue]
        [StringEnumDisplayText( "Other" )]
        Other = 0,

        [StringEnumDisplayText( "Email" )]
        Email = 1,

        [StringEnumDisplayText( "Import" )]
        Import = 2,

        [StringEnumDisplayText( "Manual" )]
        Manual = 3,
    }
}
