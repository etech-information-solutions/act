using System;
using ACT.Core.Attributes;

namespace ACT.Core.Enums
{
    // TODO:  Complete this list and keep in sync with DB script.
    [StringEnum]
    public enum PermissionContext
    {
        // Admin can see Admin (or Everything!)
        [PermissionContextSupports( PermissionTo.View )]
        SuperAdmin,

        [PermissionPrerequisite( PermissionTo.View, PermissionContext.DashBoard )]
        [PermissionContextSupports( PermissionTo.View | PermissionTo.Create | PermissionTo.Edit | PermissionTo.Delete )]
        DashBoard,

        [PermissionPrerequisite( PermissionTo.View, PermissionContext.Finance )]
        [PermissionContextSupports( PermissionTo.View | PermissionTo.Create | PermissionTo.Edit | PermissionTo.Delete )]
        Finance,

        [PermissionPrerequisite( PermissionTo.View, PermissionContext.Customer )]
        [PermissionContextSupports( PermissionTo.View | PermissionTo.Create | PermissionTo.Edit | PermissionTo.Delete )]
        Customer,

        [PermissionPrerequisite( PermissionTo.View, PermissionContext.Product )]
        [PermissionContextSupports( PermissionTo.View | PermissionTo.Create | PermissionTo.Edit | PermissionTo.Delete )]
        Product,

        [PermissionPrerequisite( PermissionTo.View, PermissionContext.Pallet )]
        [PermissionContextSupports( PermissionTo.View | PermissionTo.Create | PermissionTo.Edit | PermissionTo.Delete )]
        Pallet,

        [PermissionPrerequisite( PermissionTo.View, PermissionContext.Client )]
        [PermissionContextSupports( PermissionTo.View | PermissionTo.Create | PermissionTo.Edit | PermissionTo.Delete )]
        Client,

        [PermissionPrerequisite( PermissionTo.View, PermissionContext.Transporter )]
        [PermissionContextSupports( PermissionTo.View | PermissionTo.Create | PermissionTo.Edit | PermissionTo.Delete )]
        Transporter,

        [PermissionPrerequisite( PermissionTo.View, PermissionContext.Reports )]
        [PermissionContextSupports( PermissionTo.View | PermissionTo.Create | PermissionTo.Edit | PermissionTo.Delete )]
        Reports,

        // Admin can see Admin (or Everything!)
        [PermissionPrerequisite( PermissionTo.View, PermissionContext.Administration )]
        [PermissionContextSupports( PermissionTo.View | PermissionTo.Create | PermissionTo.Edit | PermissionTo.Delete )]
        Administration,

        [PermissionContextSupports( PermissionTo.View )]
        SuperFinance,

        [PermissionPrerequisite( PermissionTo.View, PermissionContext.Dummy )]
        [PermissionContextSupports( PermissionTo.View | PermissionTo.Create | PermissionTo.Edit | PermissionTo.Delete )]
        Dummy,

        // Whatever else.
    }



    #region Attributes

    [AttributeUsage( AttributeTargets.Field, Inherited = false, AllowMultiple = false )]
    public class PermissionContextSupportsAttribute : Attribute
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="permissionTo">Flagged enumerator indicated the supported Permissions</param>
        public PermissionContextSupportsAttribute( PermissionTo permissionTo )
        {
            this.PermissionTo = permissionTo;
        }

        public PermissionTo PermissionTo { get; set; }
    }

    [AttributeUsage( AttributeTargets.Field, Inherited = false, AllowMultiple = true )]
    public class PermissionPrerequisiteAttribute : Attribute
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="requiring">Requiring Permission.  Can be flagged.</param>
        /// <param name="required">Required Permission. Cannot be flagged.</param>
        public PermissionPrerequisiteAttribute( PermissionTo requiring, PermissionTo required )
        {
            this.Requiring = requiring;
            this.Required = required;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="requiring">Requiring Permission.  Can be flagged.</param>
        /// <param name="required">Required Permission. Cannot be flagged.</param>
        /// <param name="requiredWithin"></param>
        public PermissionPrerequisiteAttribute( PermissionTo requiring, PermissionTo required, PermissionContext requiredWithin )
        {
            this.Requiring = requiring;
            this.Required = required;
            this.RequiredWithin = requiredWithin;
        }

        public PermissionPrerequisiteAttribute( PermissionTo required, PermissionContext requiredWithin )
        {
            this.Required = required;
            this.RequiredWithin = requiredWithin;
        }

        public PermissionTo? Requiring { get; set; }

        public PermissionTo Required { get; set; }

        public PermissionContext? RequiredWithin { get; set; }
    }

    [AttributeUsage( AttributeTargets.Field, Inherited = false, AllowMultiple = false )]
    public class ForcePermissionAttribute : Attribute
    {
        public ForcePermissionAttribute( PermissionTo permissionTo )
        {
            this.PermissionTo = permissionTo;
        }

        public PermissionTo PermissionTo { get; set; }
    }

    #endregion

    #region Extensions



    #endregion
}
