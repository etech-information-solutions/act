using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using ACT.Core.Enums;
using ACT.Core.Helpers;

namespace ACT.UI.Mvc
{
    [AttributeUsage( AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true )]
    public class RequiresAttribute : CustomAuthorizeAttribute
    {

        /// <summary>
        /// Ensures that the current user has sufficient permissions within the Controller's default
        /// PermissionContext
        /// </summary>
        /// <param name="roleFor"></param>
        public RequiresAttribute( PermissionTo roleFor )
            : base()
        {

            this.RoleFor = roleFor;
            this.PermissionContext = null;

        }

        /// <summary>
        /// Ensures that the current user has sufficient permissions within the specified PermissionContext
        /// </summary>
        /// <param name="roleFor"></param>
        /// <param name="within"></param>
        public RequiresAttribute( PermissionTo roleFor, PermissionContext within )
            : base()
        {

            this.RoleFor = roleFor;
            this.PermissionContext = within;

        }

        void InitRoles()
        {
            List<string> roles = new List<string>();

            PermissionTo roleFor = this.RoleFor;
            PermissionContext within;

            if ( this.PermissionContext.HasValue )
            {
                within = this.PermissionContext.Value;
            }
            else
            {
                within = this.Controller.DefaultPermissionContext;
            }

            foreach ( PermissionTo sa in EnumHelper.GetOptions<PermissionTo>() )
            {
                if ( roleFor.MatchesFilter( sa ) )
                {
                    roles.Add( string.Format( "{0}_{1}", within.GetStringValue(), sa.GetStringValue() ) );
                }
            }

            base.Roles = roles.Delimit();

            _rolesInitialized = true;
        }


        protected override bool AuthorizeCore( HttpContextBase httpContext )
        {
            if ( !_rolesInitialized )
            {
                InitRoles();
            }

            return base.AuthorizeCore( httpContext );
        }

        protected bool _rolesInitialized = false;

        protected PermissionTo RoleFor { get; set; }

        protected PermissionContext? PermissionContext { get; set; }
    }


}