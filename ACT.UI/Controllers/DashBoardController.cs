using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ACT.Core.Enums;
using ACT.UI.Mvc;

namespace ACT.UI.Controllers
{
    [Requires( PermissionTo.View, PermissionContext.Administration )]
    public class DashBoardController : BaseController
    {
        // GET: DashBoard
        public ActionResult Index()
        {
            return View();
        }
    }
}