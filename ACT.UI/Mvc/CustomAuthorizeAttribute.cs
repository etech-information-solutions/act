using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using System.Threading;
using ACT.UI.Controllers;
using ACT.Core.Enums;

namespace ACT.UI.Mvc
{
    [AttributeUsage( AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true )]
    public class CustomAuthorizeAttribute : AuthorizeAttribute
    {
        protected override void HandleUnauthorizedRequest( AuthorizationContext filterContext )
        {
            BaseController controller = filterContext.Controller as BaseController;

            if ( controller != null )
            {
                controller.HandleUnauthorizedUserRequest( filterContext );
            }
            else
            {
                base.HandleUnauthorizedRequest( filterContext );
            }
        }

        //Because MVS caches Attributes per action we need to be careful of
        //Threading issues
        //public BaseController Controller { get; set; }

        Dictionary<Thread, BaseController> _controllerDictionary = new Dictionary<Thread, BaseController>();

        /// <summary>
        /// Handle on the current Controller.  NB: Only available AuthorizeCore(). 
        /// </summary>
        public BaseController Controller
        {
            get
            {
                if ( _controllerDictionary.ContainsKey( Thread.CurrentThread ) )
                {
                    return _controllerDictionary[ Thread.CurrentThread ];
                }

                return null;
            }

        }

        public override void OnAuthorization( AuthorizationContext filterContext )
        {
            //It is not possible to get a handle on the Current Controller from within
            //AuthorizeCore.  

            _controllerDictionary.AddOrOverwrite( Thread.CurrentThread, filterContext.Controller as BaseController );


            base.OnAuthorization( filterContext );

            _controllerDictionary.Remove( Thread.CurrentThread );


        }

        protected override bool AuthorizeCore( HttpContextBase httpContext )
        {
            bool auth = base.AuthorizeCore( httpContext );

            if ( auth )
            {
                // Kick out disabled users on the fly
                if ( ( this.Controller != null ) && ( this.Controller.CurrentUser != null ) )
                {
                    auth = this.Controller.CurrentUser.Status == Status.Active;
                }
            }

            return auth;
        }

    }
}