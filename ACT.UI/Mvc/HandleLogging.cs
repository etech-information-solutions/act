using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Filters;

namespace ACT.UI.Mvc
{
    public class HandleLogging : ActionFilterAttribute
    {
        public override void OnActionExecuting( System.Web.Http.Controllers.HttpActionContext actionContext )
        {
            if ( actionContext.Request.Method.Method.ToLower() == "post" )
            {

            }
        }
    }
}