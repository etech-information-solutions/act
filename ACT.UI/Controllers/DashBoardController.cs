using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ACT.Core.Enums;
using ACT.Core.Models;
using ACT.Core.Services;
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



        #region Partial Views

        //
        // POST || GET: /DashBoard/AgeOfOutstandingPallets
        public ActionResult AgeOfOutstandingPallets( PagingModel pm, CustomSearchModel csm )
        {
            using ( ClientLoadService service = new ClientLoadService() )
            {


                return PartialView( "_AgeOfOutstandingPallets" );
            }
        }

        //
        // POST || GET: /DashBoard/LoadsPerMonth
        public ActionResult LoadsPerMonth( PagingModel pm, CustomSearchModel csm )
        {

            return PartialView( "_LoadsPerMonth" );
        }

        //
        // POST || GET: /DashBoard/AuthorisationCodes
        public ActionResult AuthorisationCodes( PagingModel pm, CustomSearchModel csm )
        {

            return PartialView( "_AuthorisationCodes" );
        }

        //
        // POST || GET: /DashBoard/NumberOfPalletsManaged
        public ActionResult NumberOfPalletsManaged( PagingModel pm, CustomSearchModel csm )
        {

            return PartialView( "_NumberOfPalletsManaged" );
        }

        //
        // POST || GET: /DashBoard/NumberOfDisputes
        public ActionResult NumberOfDisputes( PagingModel pm, CustomSearchModel csm )
        {

            return PartialView( "_NumberOfDisputes" );
        }

        //
        // POST || GET: /DashBoard/KPIMeasurement
        public ActionResult KPIMeasurement( PagingModel pm, CustomSearchModel csm )
        {

            return PartialView( "_KPIMeasurement" );
        }

        #endregion
    }
}