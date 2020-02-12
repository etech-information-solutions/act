using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace System.Web.Mvc {
    public static class ControllerExtensions {

        public static string GetName(this ControllerBase controller) {
            return controller.GetType().Name.Replace("Controller", "");
        }

    }
}
