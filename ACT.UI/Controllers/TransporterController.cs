
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

using ACT.Core.Enums;
using ACT.Core.Models;
using ACT.Core.Models.Custom;
using ACT.Core.Services;
using ACT.Data.Models;
using ACT.UI.Models;
using ACT.UI.Mvc;

namespace ACT.UI.Controllers
{
    [Requires( PermissionTo.View, PermissionContext.Transporter )]
    public class TransporterController : BaseController
    {
        // GET: Finance
        public ActionResult Index()
        {
            return View();
        }

        //-------------------------------------------------------------------------------------

        #region Notifications

        //
        // POST: /Transporter/UpdateNotification/5
        [HttpPost]
        [Requires( PermissionTo.Delete )]
        public ActionResult UpdateNotification( BroadcastViewModel model )
        {
            using ( BroadcastService bservice = new BroadcastService() )
            using ( UserBroadcastService ubservice = new UserBroadcastService() )
            {
                Broadcast b = bservice.GetById( model.Id );

                if ( b == null )
                {
                    Notify( "Sorry, the requested resource could not be found. Please try again", NotificationType.Error );

                    return PartialView( "_AccessDenied" );
                }

                if ( b.UserBroadcasts.Any( ub => ub.UserId == CurrentUser.Id ) )
                {
                    int ubid = b.UserBroadcasts.FirstOrDefault( ub => ub.UserId == CurrentUser.Id ).Id;

                    ubservice.Query( $"DELETE FROM [dbo].[UserBroadcast] WHERE [Id]={ubid};" );
                }
                else
                {
                    ubservice.Create( new UserBroadcast()
                    {
                        BroadcastId = b.Id,
                        UserId = CurrentUser.Id,
                    } );
                }

                Notify( "The selected Notification was successfully updated.", NotificationType.Success );

                return MyNotifications( new PagingModel(), new CustomSearchModel() );
            }
        }

        #endregion

        //-------------------------------------------------------------------------------------

        #region Partial Views

        //
        // GET: /Transporter/SiteAudits
        public ActionResult SiteAudits( PagingModel pm, CustomSearchModel csm, bool givecsm = false )
        {
            using ( SiteAuditService service = new SiteAuditService() )
            {
                if ( givecsm )
                {
                    ViewBag.ViewName = "SiteAudits";

                    return PartialView( "_SiteAuditsCustomSearch", new CustomSearchModel( "SiteAudits" ) );
                }

                List<SiteAuditCustomModel> model = service.List1( pm, csm );

                int total = ( model.Count < pm.Take && pm.Skip == 0 ) ? model.Count : service.Total1( pm, csm );

                PagingExtension paging = PagingExtension.Create( model, total, pm.Skip, pm.Take, pm.Page );

                return PartialView( "_SiteAudits", paging );
            }
        }

        //
        // GET: /Transporter/OutstandingReport
        public ActionResult OutstandingReport( PagingModel pm, CustomSearchModel csm, bool givecsm = false )
        {
            if ( givecsm )
            {
                ViewBag.ViewName = "OutstandingPallets";

                return PartialView( "_OutstandingPalletsCustomSearch", new CustomSearchModel( "OutstandingPallets" ) );
            }

            List<OutstandingPalletsModel> model = GetOutstandingPallets( pm, csm );

            PagingExtension paging = PagingExtension.Create( model, model.Count, pm.Skip, pm.Take, pm.Page );

            return PartialView( "_OutstandingReport", paging );
        }

        //
        // GET: /Transporter/PODManagement
        public ActionResult PODManagement( PagingModel pm, CustomSearchModel csm, bool givecsm = false )
        {
            if ( givecsm )
            {
                ViewBag.ViewName = "PODManagement";

                return PartialView( "_PODManagementCustomSearch", new CustomSearchModel( "PODManagement" ) );
            }

            using ( ClientLoadService service = new ClientLoadService() )
            {
                List<ClientLoadCustomModel> model = service.ListOutstandingShipments( pm, csm );

                int total = ( model.Count < pm.Take && pm.Skip == 0 ) ? model.Count : service.Total1( pm, csm );

                PagingExtension paging = PagingExtension.Create( model, total, pm.Skip, pm.Take, pm.Page );

                return PartialView( "_PODManagement", paging );
            }
        }

        //
        // GET: /Transporter/MyNotifications
        public ActionResult MyNotifications( PagingModel pm, CustomSearchModel csm, bool givecsm = false )
        {
            if ( givecsm )
            {
                ViewBag.ViewName = "Notifications";

                return PartialView( "_MyNotificationsCustomSearch", new CustomSearchModel( "MyNotifications" ) );
            }

            using ( BroadcastService service = new BroadcastService() )
            {
                csm.IncludeUserBroadCasts = true;

                List<BroadcastCustomModel> model = service.List1( pm, csm );

                int total = ( model.Count < pm.Take && pm.Skip == 0 ) ? model.Count : service.Total1( pm, csm );

                PagingExtension paging = PagingExtension.Create( model, total, pm.Skip, pm.Take, pm.Page );

                return PartialView( "_MyNotifications", paging );
            }
        }

        #endregion
    }
}
