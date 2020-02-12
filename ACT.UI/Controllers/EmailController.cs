
using System;
using System.Web.Mvc;
using ACT.Data.Models;

namespace ACT.UI.Controllers
{
    public class EmailController : BaseController
    {
        public ActionResult ResetPassword( Guid token, User user )
        {
            ViewBag.Token = token;

            return PartialView( "_ResetPassword", user );
        }
    }
}
