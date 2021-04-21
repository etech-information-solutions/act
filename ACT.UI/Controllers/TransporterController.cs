
using System;
using System.Collections.Generic;
using System.IO;
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
        // GET: Transporter
        public ActionResult Index()
        {
            return View();
        }

        //-------------------------------------------------------------------------------------

        // GET: Transporter/EditClientLoad/5
        [Requires( PermissionTo.Edit )]
        public ActionResult EditClientLoad( int id )
        {
            using ( ImageService iservice = new ImageService() )
            using ( ClientLoadService clservice = new ClientLoadService() )
            {
                ClientLoad load = clservice.GetById( id );

                if ( load == null )
                {
                    Notify( "Sorry, the requested resource could not be found. Please try again", NotificationType.Error );

                    return PartialView( "_AccessDenied" );
                }

                ClientLoadViewModel model = new ClientLoadViewModel()
                {
                    Id = load.Id,
                    EditMode = true,
                    PODNumber = load.PODNumber,
                    PCNNumber = load.PCNNumber,
                    PRNNumber = load.PRNNumber,
                    PODCommentId = load.PODCommentId,
                    PCNComments = load.PCNComments,
                    PRNComments = load.PRNComments,
                    Files = new List<FileViewModel>()
                };

                Image podImg = iservice.Get( model.Id, "PODNumber", true );
                Image pcnImg = iservice.Get( model.Id, "PCNNumber", true );
                Image prnImg = iservice.Get( model.Id, "PRNNumber", true );

                if ( podImg != null )
                {
                    model.Files.Add( new FileViewModel()
                    {
                        Id = podImg.Id,
                        Name = "PODNumber",
                        Extension = podImg.Extension
                    } );
                }

                if ( pcnImg != null )
                {
                    model.Files.Add( new FileViewModel()
                    {
                        Id = pcnImg.Id,
                        Name = "PCNNumber",
                        Extension = pcnImg.Extension
                    } );
                }

                if ( prnImg != null )
                {
                    model.Files.Add( new FileViewModel()
                    {
                        Id = prnImg.Id,
                        Name = "PRNNumber",
                        Extension = prnImg.Extension
                    } );
                }

                return View( model );
            }
        }

        // POST: Pallet/EditClientLoad/5
        [HttpPost]
        [Requires( PermissionTo.Edit )]
        public ActionResult EditClientLoad( ClientLoadViewModel model, PagingModel pm, CustomSearchModel csm )
        {
            using ( ImageService iservice = new ImageService() )
            using ( ClientLoadService clservice = new ClientLoadService() )
            {
                ClientLoad load = clservice.GetById( model.Id );

                if ( load == null )
                {
                    Notify( "Sorry, the requested resource could not be found. Please try again", NotificationType.Error );

                    return PartialView( "_AccessDenied" );
                }

                #region Update Client Load

                load.PODNumber = model.PODNumber;
                load.PCNNumber = model.PCNNumber;
                load.PRNNumber = model.PRNNumber;

                load.PODCommentDate = DateTime.Now;
                load.PCNCommentDate = DateTime.Now;
                load.PRNCommentDate = DateTime.Now;

                load.PODCommentId = model.PODCommentId;
                load.PCNComments = model.PCNComments;
                load.PRNComments = model.PRNComments;

                load.PODCommentById = CurrentUser.Id;
                load.PCNCommentById = CurrentUser.Id;
                load.PRNCommentById = CurrentUser.Id;

                clservice.Update( load );

                #endregion

                #region Any Uploads?

                if ( model.Files.NullableAny( f => f.File != null ) )
                {
                    // Create folder
                    string path = Server.MapPath( $"~/{VariableExtension.SystemRules.ImagesLocation}/ClientLoad/{load.LoadNumber?.Trim()}/" );

                    string now = DateTime.Now.ToString( "yyyyMMddHHmmss" );

                    foreach ( FileViewModel f in model.Files.Where( f => f.File != null ) )
                    {
                        string ext = Path.GetExtension( f.File.FileName ),
                               name = f.File.FileName.Replace( ext, "" );

                        // Check if a logo already exists?
                        Image img = iservice.Get( load.Id, f.Name, true );

                        if ( img != null )
                        {
                            DeleteImage( img.Id );
                        }

                        if ( !Directory.Exists( path ) )
                        {
                            Directory.CreateDirectory( path );
                        }

                        Image image = new Image()
                        {
                            Name = name,
                            IsMain = true,
                            Extension = ext,
                            ObjectId = load.Id,
                            ObjectType = f.Name,
                            Size = f.File.ContentLength,
                            Location = $"ClientLoad/{load.LoadNumber?.Trim()}/{now}-{f.File.FileName}"
                        };

                        iservice.Create( image );

                        string fullpath = Path.Combine( path, $"{now}-{f.File.FileName}" );

                        f.File.SaveAs( fullpath );
                    }
                }

                #endregion
            }

            Notify( "The selected Client Load details were successfully updated.", NotificationType.Success );

            return PODManagement( pm, csm );
        }

        //-------------------------------------------------------------------------------------



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
