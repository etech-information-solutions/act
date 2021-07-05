
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


        #region Exports

        //
        // GET: /Finance/Export
        public FileContentResult Export( PagingModel pm, CustomSearchModel csm, string type = "outstandingreport" )
        {
            string csv = "";
            string filename = string.Format( "{0}-{1}.csv", type.ToUpperInvariant(), DateTime.Now.ToString( "yyyy_MM_dd_HH_mm" ) );

            pm.Skip = 0;
            pm.Take = int.MaxValue;

            int count = 0;

            switch ( type )
            {
                case "outstandingreports":

                    #region Outstanding Pallets

                    List<OutstandingPalletsModel> items = GetOutstandingPallets( pm, csm );

                    DateTime minYear = items.NullableAny() ? items.Min( m => m.MinYear ) : DateTime.Now;

                    csv = "Client,Reason,0-30 Days,31-60 Days,61-90 Days,91-120 Days,121-183 Days,184-270 Days,271-365 Days";

                    if ( minYear.Year != DateTime.Now.Year )
                    {
                        for ( int i = DateTime.Now.AddYears( -1 ).Year; i >= minYear.Year; i-- )
                        {
                            csv = $"{csv},{i}";
                        }
                    }

                    csv = $"{csv},Grand Total {Environment.NewLine}";

                    if ( items.NullableAny() )
                    {
                        count = 0;

                        foreach ( OutstandingPalletsModel item in items )
                        {
                            csv = $"{csv} {item.ClientLoad.ClientName},,,,,,,";

                            if ( minYear.Year != DateTime.Now.Year )
                            {
                                for ( int i = DateTime.Now.AddYears( -1 ).Year; i >= minYear.Year; i-- )
                                {
                                    csv = $"{csv},";
                                }
                            }

                            csv = $"{csv},{Environment.NewLine}";

                            foreach ( OutstandingReasonModel osr in item.OutstandingReasons )
                            {
                                csv = $"{csv},{osr.Description},{osr.To30Days},{osr.To60Days},{osr.To90Days},{osr.To120Days},{osr.To183Days},{osr.To270Days},{osr.To365Days}";

                                if ( minYear.Year != DateTime.Now.Year )
                                {
                                    for ( int i = DateTime.Now.AddYears( -1 ).Year; i >= minYear.Year; i-- )
                                    {
                                        csv = $"{csv},{osr.PreviousYears?.FirstOrDefault( y => y.Year == i )?.Total}";
                                    }
                                }

                                csv = $"{csv},{osr.GrandTotal} {Environment.NewLine}";
                            }

                            csv = $"{csv}{Environment.NewLine},Total,{item.GrandTotal.To30Days},{item.GrandTotal.To60Days},{item.GrandTotal.To90Days},{item.GrandTotal.To120Days},{item.GrandTotal.To183Days},{item.GrandTotal.To270Days},{item.GrandTotal.To365Days}";

                            if ( minYear.Year != DateTime.Now.Year )
                            {
                                for ( int i = DateTime.Now.AddYears( -1 ).Year; i >= minYear.Year; i-- )
                                {
                                    csv = $"{csv},{item.GrandTotal?.PreviousYears?.FirstOrDefault( y => y.Year == i )?.Total}";
                                }
                            }

                            csv = $"{csv},{item.GrandTotal.GrandTotal} {Environment.NewLine}{Environment.NewLine}";

                            count++;
                        }

                        csv = $"{csv}{Environment.NewLine}{Environment.NewLine},Grand Total,{items.Sum( s => s.GrandTotal.To30Days )},{items.Sum( s => s.GrandTotal.To60Days )},{items.Sum( s => s.GrandTotal.To90Days )},{items.Sum( s => s.GrandTotal.To120Days )},{items.Sum( s => s.GrandTotal.To183Days )},{items.Sum( s => s.GrandTotal.To270Days )},{items.Sum( s => s.GrandTotal.To365Days )}";

                        if ( minYear.Year != DateTime.Now.Year )
                        {
                            for ( int i = DateTime.Now.AddYears( -1 ).Year; i >= minYear.Year; i-- )
                            {
                                csv = $"{csv},{items.SelectMany( m => m.GrandTotal?.PreviousYears?.Where( w => w.Year == i ) ).Sum( s => s.Total )}";
                            }
                        }

                        csv = $"{csv},{items.Sum( s => s.GrandTotal.GrandTotal )}";
                    }

                    #endregion

                    break;
            }

            return File( new System.Text.UTF8Encoding().GetBytes( csv ), "text/csv", filename );
        }

        #endregion



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
