using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ACT.Core.Enums;
using ACT.Core.Models;
using ACT.Core.Models.Custom;
using ACT.Core.Services;
using ACT.UI.Mvc;

namespace ACT.UI.Controllers
{
    [Requires( PermissionTo.View, PermissionContext.Client )]
    public class ClientReportingController : BaseController
    {
        // GET: Index
        public ActionResult Index()
        {
            return View();
        }

        #region Partial Views

        //
        // POST || GET: /ClientReporting/Disputes
        public ActionResult Disputes( PagingModel pm, CustomSearchModel csm, bool givecsm = false )
        {
            if ( givecsm )
            {
                ViewBag.ViewName = "Disputes";

                return PartialView( "_DisputesCustomSearch", new CustomSearchModel( "Disputes" ) );
            }

            int total = 0;

            List<DisputeCustomModel> model;

            using ( DisputeService service = new DisputeService() )
            {
                model = service.List1( pm, csm );
                total = ( model.Count < pm.Take && pm.Skip == 0 ) ? model.Count : service.Total1( pm, csm );
            }

            PagingExtension paging = PagingExtension.Create( model, total, pm.Skip, pm.Take, pm.Page );

            return PartialView( "_Disputes", paging );
        }



        #endregion
    }
}