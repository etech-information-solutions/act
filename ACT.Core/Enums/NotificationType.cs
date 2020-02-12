
namespace ACT.Core.Enums
{
    using Attributes;

    [StringEnum]
    public enum NotificationType
    {
        [StringEnumDisplayText( "Success" )]
        Success,

        [StringEnumDisplayText( "Warn" )]
        Warn,

        [StringEnumDisplayText( "Error" )]
        Error,

        [StringEnumDisplayText( "Critical" )]
        Critical
    }
}
