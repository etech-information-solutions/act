
namespace ACT.Core.Enums
{
    using Attributes;

    [StringEnum]
    public enum ActivityTypes
    {
        [UiIgnoreEnumValue]
        All = -1,

        Login,

        Logout,

        //List,

        //View,

        Delete,

        //CreateRequest,

        Create,

        //EditRequest,

        Edit,

        Download,

        Other
    }
}

