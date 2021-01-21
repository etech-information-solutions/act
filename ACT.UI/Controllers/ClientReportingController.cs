using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Transactions;
using System.Web;
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
    [Requires( PermissionTo.View, PermissionContext.Client )]
    public class ClientReportingController : BaseController
    {
        // GET: Index
        public ActionResult Index()
        {
            return View();
        }



        #region Exports

        //
        // GET: /Administration/Export
        public FileContentResult Export( PagingModel pm, CustomSearchModel csm, string type = "disputes" )
        {
            string csv = "";
            string filename = string.Format( "{0}-{1}.csv", type.ToUpperInvariant(), DateTime.Now.ToString( "yyyy_MM_dd_HH_mm" ) );

            pm.Skip = 0;
            pm.Take = int.MaxValue;

            switch ( type )
            {
                case "disputes":

                    #region Disputes

                    csv = string.Format( "Account Number,TDN Number,Raised Date,Docket Number,Reference,Action By,Resolved On,Resolved By,Other Party,Sender,Receiver,Declarer,Dispute Email,Product,Dispute Status,Equipment,Quantity,Reason fo Dispute {0}", Environment.NewLine );

                    using ( DisputeService dservice = new DisputeService() )
                    {
                        List<DisputeCustomModel> disputes = dservice.List1( pm, csm );

                        if ( disputes != null && disputes.Any() )
                        {
                            foreach ( DisputeCustomModel item in disputes )
                            {
                                csv = string.Format( "{0} {1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17},{18} {19}",
                                                    csv,
                                                    item.ChepLoadAccountNumber,
                                                    item.TDNNumber,
                                                    item.CreatedOn,
                                                    item.DocketNumber,
                                                    item.Reference,
                                                    item.ActionUser,
                                                    item.ResolvedOn,
                                                    item.ResolvedUser,
                                                    item.OtherParty,
                                                    item.Sender,
                                                    item.Receiver,
                                                    item.Declarer,
                                                    item.DisputeEmail,
                                                    item.Product,
                                                    item.Status,
                                                    item.Equipment,
                                                    item.Quantity,
                                                    item.DisputeReasonDetails,
                                                    Environment.NewLine );
                            }
                        }
                    }

                    #endregion

                    break;

                case "chepaudit":

                    #region Chep Audit

                    csv = string.Format( "Date Created,Site,Equipment,Stock Balance,Invoice In,Invoice Out,Req Adjustment,MCC In,MCC Out,MCC Balance,Suspend ITL,Suspend MCC,Adjusted Invoice Balance {0}", Environment.NewLine );

                    using ( ChepAuditService dservice = new ChepAuditService() )
                    {
                        List<ChepAuditCustomModel> audits = dservice.List1( pm, csm );

                        if ( audits != null && audits.Any() )
                        {
                            foreach ( ChepAuditCustomModel item in audits )
                            {
                                csv = string.Format( "{0} {1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13} {14}",
                                                    csv,
                                                    item.CreatedOn,
                                                    item.SiteName,
                                                    item.Equipment,
                                                    item.ChepStockBalance,
                                                    item.NotInvoiceIn,
                                                    item.NotInvoiceOut,
                                                    item.ReqAdjustment,
                                                    item.NotMCCIn,
                                                    item.NotMCCOut,
                                                    item.MccBalance,
                                                    item.SuspendITL,
                                                    item.SuspendMCC,
                                                    item.AdjustedInvBalance,
                                                    Environment.NewLine );
                            }
                        }
                    }

                    #endregion

                    break;

                case "clientaudit":

                    #region Client Audit

                    csv = string.Format( "Date Created,Audit Date,Site,Equipment,Pallets Outstanding,Pallets Counted,Writeoff Pallets,Customer Name,Rep Name,Pallet Auditor,Status {0}", Environment.NewLine );

                    using ( SiteAuditService dservice = new SiteAuditService() )
                    {
                        List<SiteAuditCustomModel> siteAudits = dservice.List1( pm, csm );

                        if ( siteAudits != null && siteAudits.Any() )
                        {
                            foreach ( SiteAuditCustomModel item in siteAudits )
                            {
                                csv = string.Format( "{0} {1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11} {12}",
                                                    csv,
                                                    item.CreatedOn,
                                                    item.AuditDate,
                                                    item.SiteName,
                                                    item.Equipment,
                                                    item.PalletsOutstanding,
                                                    item.PalletsCounted,
                                                    item.WriteoffPallets,
                                                    item.CustomerName,
                                                    item.RepName,
                                                    item.PalletAuditor,
                                                    item.Status,
                                                    Environment.NewLine );
                            }
                        }
                    }

                    #endregion

                    break;

                case "movementreport":

                    #region Movement Report

                    csv = string.Format( "Movement Date,Effective Date,Date of Notification,Docket Number,Posting Type,Trading Partner,Customer Reference Number,Equipment,POP Number,PCN Number,PRN Number {0}", Environment.NewLine );

                    using ( ClientLoadService dservice = new ClientLoadService() )
                    {
                        List<ClientLoadCustomModel> clientload = dservice.List1( pm, csm );

                        if ( clientload != null && clientload.Any() )
                        {
                            foreach ( ClientLoadCustomModel item in clientload )
                            {
                                csv = string.Format( "{0} {1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11} {12}",
                                                    csv,
                                                    item.LoadDate,
                                                    item.EffectiveDate,
                                                    item.NotifyDate,
                                                    item.DocketNumber,
                                                    item.PostingType,
                                                    item.ClientDescription,
                                                    item.ReferenceNumber,
                                                    item.Equipment,
                                                    item.PODNumber,
                                                    item.PCNNumber,
                                                    item.PRNNumber,
                                                    Environment.NewLine );
                            }
                        }
                    }

                    #endregion

                    break;

                case "exceptions":

                    #region Exceptions

                    csv = string.Format( "Site,Sub-site,Load Date,Exception {0}", Environment.NewLine );

                    using ( ClientLoadService dservice = new ClientLoadService() )
                    {
                        csm.ReconciliationStatus = ReconciliationStatus.Unreconciled;

                        List<ClientLoadCustomModel> clientload = dservice.List1( pm, csm );

                        if ( clientload != null && clientload.Any() )
                        {
                            foreach ( ClientLoadCustomModel item in clientload )
                            {
                                csv = string.Format( "{0} {1},{2},{3} {12}",
                                                    csv,
                                                    item.SiteName,
                                                    item.SubSiteName,
                                                    item.LoadDate,
                                                    item.OutstandingReason,
                                                    Environment.NewLine );
                            }
                        }
                    }

                    #endregion

                    break;
            }

            return File( new System.Text.UTF8Encoding().GetBytes( csv ), "text/csv", filename );
        }

        #endregion



        #region Disputes

        //
        // GET: /ClientReporting/DisputeDetails/5
        public ActionResult DisputeDetails( int id, bool layout = true )
        {
            using ( DisputeService dservice = new DisputeService() )
            {
                Dispute model = dservice.GetById( id );

                if ( model == null )
                {
                    Notify( "Sorry, the requested resource could not be found. Please try again", NotificationType.Error );

                    return RedirectToAction( "Index" );
                }

                if ( layout )
                {
                    ViewBag.IncludeLayout = true;
                }

                return View( model );
            }
        }

        #endregion



        #region Chep Audit

        //
        // GET: /ClientReporting/ChepAuditDetails/5
        public ActionResult ChepAuditDetails( int id, bool layout = true )
        {
            using ( DocumentService dservice = new DocumentService() )
            using ( ChepAuditService cservice = new ChepAuditService() )
            {
                ChepAudit model = cservice.GetById( id );

                if ( model == null )
                {
                    Notify( "Sorry, the requested resource could not be found. Please try again", NotificationType.Error );

                    return RedirectToAction( "Index" );
                }

                Document document = dservice.Get( model.Id, "ChepAudit", "AuditReport" );

                if ( document != null )
                {
                    ViewBag.AuditReport = document;
                }

                if ( layout )
                {
                    ViewBag.IncludeLayout = true;
                }

                return View( model );
            }
        }

        //
        // GET: /ClientReporting/AddChepAudit/5 
        //[Requires( PermissionTo.Create )]
        public ActionResult AddChepAudit()
        {
            ChepAuditViewModel model = new ChepAuditViewModel() { EditMode = true };

            return View( model );
        }

        //
        // POST: /ClientReporting/AddChepAudit/5
        [HttpPost]
        //[Requires( PermissionTo.Create )]
        public ActionResult AddChepAudit( ChepAuditViewModel model )
        {
            if ( !ModelState.IsValid )
            {
                Notify( "Sorry, the Chep Audit Report was not created. Please correct all errors and try again.", NotificationType.Error );

                return View( model );
            }

            using ( TransactionScope scope = new TransactionScope() )
            using ( DocumentService dservice = new DocumentService() )
            using ( ChepAuditService cservice = new ChepAuditService() )
            {
                #region Create ChepAudit

                // Create ChepAudit
                ChepAudit àudit = new ChepAudit()
                {
                    SiteId = model.SiteId,
                    NotMCCIn = model.NotMCCIn,
                    Equipment = model.Equipment,
                    NotMCCOut = model.NotMCCOut,
                    SuspendITL = model.SuspendITL,
                    SuspendMCC = model.SuspendMCC,
                    MccBalance = model.MccBalance,
                    NotInvoiceIn = model.NotInvoiceIn,
                    ReqAdjustment = model.ReqAdjustment,
                    NotInvoiceOut = model.NotInvoiceOut,
                    ChepStockBalance = model.ChepStockBalance,
                    AdjustedInvBalance = model.AdjustedInvBalance,
                };

                cservice.Create( àudit );

                #endregion

                #region Any Uploads

                if ( model.AuditReportFile != null )
                {
                    // Create folder
                    string path = Server.MapPath( $"~/{VariableExtension.SystemRules.DocumentsLocation}/ChepAuditReports/Site-{model.SiteId}-id/" );

                    if ( !Directory.Exists( path ) )
                    {
                        Directory.CreateDirectory( path );
                    }

                    string now = DateTime.Now.ToString( "yyyyMMddHHmmss" );

                    Document doc = new Document()
                    {
                        ObjectId = àudit.Id,
                        ObjectType = "ChepAudit",
                        Status = ( int ) Status.Active,
                        Name = model.AuditReportFile.Name,
                        Category = model.AuditReportFile.Name,
                        Title = model.AuditReportFile.File.FileName,
                        Size = model.AuditReportFile.File.ContentLength,
                        Description = model.AuditReportFile.Description,
                        Type = Path.GetExtension( model.AuditReportFile.File.FileName ),
                        Location = $"ChepAuditReports/Site-{model.SiteId}-id/{now}-{model.AuditReportFile.File.FileName}"
                    };

                    dservice.Create( doc );

                    string fullpath = Path.Combine( path, $"{now}-{model.AuditReportFile.File.FileName}" );
                    model.AuditReportFile.File.SaveAs( fullpath );
                }

                #endregion

                // We're done here..

                scope.Complete();
            }

            Notify( "The Chep Audit Report was successfully created.", NotificationType.Success );

            return ChepAudit( new PagingModel(), new CustomSearchModel() );
        }

        //
        // GET: /ClientReporting/EditChepAudit/5
        //[Requires( PermissionTo.Edit )]
        public ActionResult EditChepAudit( int id )
        {
            using ( DocumentService dservice = new DocumentService() )
            using ( ChepAuditService service = new ChepAuditService() )
            {
                ChepAudit audit = service.GetById( id );

                if ( audit == null )
                {
                    Notify( "Sorry, the requested resource could not be found. Please try again", NotificationType.Error );

                    return PartialView( "_AccessDenied" );
                }

                Document document = dservice.Get( audit.Id, "ChepAudit", "AuditReport" );

                ChepAuditViewModel model = new ChepAuditViewModel()
                {
                    Id = audit.Id,
                    EditMode = true,
                    SiteId = audit.SiteId,
                    NotMCCIn = audit.NotMCCIn,
                    NotMCCOut = audit.NotMCCOut,
                    Equipment = audit.Equipment,
                    SuspendMCC = audit.SuspendMCC,
                    SuspendITL = audit.SuspendITL,
                    MccBalance = audit.MccBalance,
                    NotInvoiceIn = audit.NotInvoiceIn,
                    NotInvoiceOut = audit.NotInvoiceOut,
                    ReqAdjustment = audit.ReqAdjustment,
                    ChepStockBalance = audit.ChepStockBalance,
                    AdjustedInvBalance = audit.AdjustedInvBalance,
                    AuditReportFile = new FileViewModel()
                    {
                        Id = document.Id,
                        Name = document.Name,
                        Extension = document.Type,
                        Size = ( decimal ) document.Size,
                        Description = document.Description,
                    },
                };

                return View( model );
            }
        }

        //
        // POST: /ClientReporting/EditChepAudit/5
        [HttpPost]
        //[Requires( PermissionTo.Edit )]
        public ActionResult EditChepAudit( ChepAuditViewModel model )
        {
            if ( !ModelState.IsValid )
            {
                Notify( "Sorry, the selected Chep Audit Report was not updated. Please correct all errors and try again.", NotificationType.Error );

                return View( model );
            }

            using ( TransactionScope scope = new TransactionScope() )
            using ( DocumentService dservice = new DocumentService() )
            using ( ChepAuditService rservice = new ChepAuditService() )
            {
                ChepAudit audit = rservice.GetById( model.Id );

                if ( audit == null )
                {
                    Notify( "Sorry, that Chep Audit Report does not exist! Please specify a valid Chep Audit Report Id and try again.", NotificationType.Error );

                    return View( model );
                }

                #region Update Chep Audit

                // Update ChepAudit
                audit.SiteId = model.SiteId;
                audit.NotMCCIn = model.NotMCCIn;
                audit.Equipment = model.Equipment;
                audit.NotMCCOut = model.NotMCCOut;
                audit.SuspendITL = model.SuspendITL;
                audit.SuspendMCC = model.SuspendMCC;
                audit.MccBalance = model.MccBalance;
                audit.NotInvoiceIn = model.NotInvoiceIn;
                audit.ReqAdjustment = model.ReqAdjustment;
                audit.NotInvoiceOut = model.NotInvoiceOut;
                audit.ChepStockBalance = model.ChepStockBalance;
                audit.AdjustedInvBalance = model.AdjustedInvBalance;

                rservice.Update( audit );

                #endregion

                #region Any Uploads

                if ( model.AuditReportFile != null )
                {
                    // Create folder
                    string path = Server.MapPath( $"~/{VariableExtension.SystemRules.DocumentsLocation}/ChepAuditReports/Site-{model.SiteId}-id/" );

                    if ( !Directory.Exists( path ) )
                    {
                        Directory.CreateDirectory( path );
                    }

                    string now = DateTime.Now.ToString( "yyyyMMddHHmmss" );

                    Document doc = dservice.GetById( model.AuditReportFile.Id );

                    if ( doc != null )
                    {
                        // Disable this file...
                        doc.Status = ( int ) Status.Inactive;

                        dservice.Update( doc );
                    }

                    doc = new Document()
                    {
                        ObjectId = audit.Id,
                        ObjectType = "ChepAudit",
                        Status = ( int ) Status.Active,
                        Name = model.AuditReportFile.Name,
                        Category = model.AuditReportFile.Name,
                        Title = model.AuditReportFile.File.FileName,
                        Size = model.AuditReportFile.File.ContentLength,
                        Description = model.AuditReportFile.Description,
                        Type = Path.GetExtension( model.AuditReportFile.File.FileName ),
                        Location = $"ChepAuditReports/Site-{model.SiteId}-id/{now}-{model.AuditReportFile.File.FileName}"
                    };

                    dservice.Create( doc );

                    string fullpath = Path.Combine( path, $"{now}-{model.AuditReportFile.File.FileName}" );
                    model.AuditReportFile.File.SaveAs( fullpath );
                }

                #endregion

                scope.Complete();
            }

            Notify( "The selected Chep Audit Report details were successfully updated.", NotificationType.Success );

            return ChepAudit( new PagingModel(), new CustomSearchModel() );
        }

        //
        // POST: /ClientReporting/DeleteChepAudit/5
        [HttpPost]
        //[Requires( PermissionTo.Delete )]
        public ActionResult DeleteChepAudit( ChepAuditViewModel model, PagingModel pm, CustomSearchModel csm )
        {
            ChepAudit role;

            using ( ChepAuditService service = new ChepAuditService() )
            {
                role = service.GetById( model.Id );

                if ( role == null )
                {
                    Notify( "Sorry, the requested resource could not be found. Please try again", NotificationType.Error );

                    return PartialView( "_AccessDenied" );
                }

                service.Delete( role );

                Notify( "The selected Chep Audit Report was successfully Deleted.", NotificationType.Success );
            }

            return ChepAudit( pm, csm );
        }

        #endregion



        #region Client Audit

        //
        // GET: /ClientReporting/ClientAuditDetails/5
        public ActionResult ClientAuditDetails( int id, bool layout = true )
        {
            using ( DocumentService dservice = new DocumentService() )
            using ( SiteAuditService sservice = new SiteAuditService() )
            {
                SiteAudit model = sservice.GetById( id );

                if ( model == null )
                {
                    Notify( "Sorry, the requested resource could not be found. Please try again", NotificationType.Error );

                    return RedirectToAction( "Index" );
                }

                Document document = dservice.Get( model.Id, "SiteAudit", "AuditReport" );

                if ( document != null )
                {
                    ViewBag.AuditReport = document;
                }

                if ( layout )
                {
                    ViewBag.IncludeLayout = true;
                }

                return View( model );
            }
        }

        #endregion



        #region Exceptions

        //
        // GET: /ClientReporting/ExceptionDetails/5
        public ActionResult ExceptionDetails( int id, bool layout = true )
        {
            using ( DocumentService dservice = new DocumentService() )
            using ( ClientLoadService clservice = new ClientLoadService() )
            {
                ClientLoad model = clservice.GetById( id );

                if ( model == null )
                {
                    Notify( "Sorry, the requested resource could not be found. Please try again", NotificationType.Error );

                    return RedirectToAction( "Index" );
                }

                List<Document> documents = dservice.List( model.Id, "ClientLoad" );

                if ( documents != null )
                {
                    ViewBag.Documents = documents;
                }

                if ( layout )
                {
                    ViewBag.IncludeLayout = true;
                }

                return View( model );
            }
        }

        #endregion



        #region Partial Views

        //
        // POST || GET: /ClientReporting/Disputes
        public ActionResult Disputes( PagingModel pm, CustomSearchModel csm, bool givecsm = false )
        {
            using ( DisputeService service = new DisputeService() )
            {
                if ( givecsm )
                {
                    ViewBag.ViewName = "Disputes";

                    return PartialView( "_DisputesCustomSearch", new CustomSearchModel( "Disputes" ) );
                }

                List<DisputeCustomModel> model = service.List1( pm, csm );

                int total = ( model.Count < pm.Take && pm.Skip == 0 ) ? model.Count : service.Total1( pm, csm );

                PagingExtension paging = PagingExtension.Create( model, total, pm.Skip, pm.Take, pm.Page );

                return PartialView( "_Disputes", paging );
            }
        }

        //
        // POST || GET: /ClientReporting/ChepAudit
        public ActionResult ChepAudit( PagingModel pm, CustomSearchModel csm, bool givecsm = false )
        {
            using ( ChepAuditService service = new ChepAuditService() )
            {
                if ( givecsm )
                {
                    ViewBag.ViewName = "ChepAudit";

                    return PartialView( "_ChepAuditCustomSearch", new CustomSearchModel( "ChepAudit" ) );
                }

                List<ChepAuditCustomModel> model = service.List1( pm, csm );

                int total = ( model.Count < pm.Take && pm.Skip == 0 ) ? model.Count : service.Total1( pm, csm );

                PagingExtension paging = PagingExtension.Create( model, total, pm.Skip, pm.Take, pm.Page );

                return PartialView( "_ChepAudit", paging );
            }
        }

        //
        // POST || GET: /ClientReporting/ClientAudit
        public ActionResult ClientAudit( PagingModel pm, CustomSearchModel csm, bool givecsm = false )
        {
            using ( SiteAuditService service = new SiteAuditService() )
            {
                if ( givecsm )
                {
                    ViewBag.ViewName = "ClientAudit";

                    return PartialView( "_ClientAuditCustomSearch", new CustomSearchModel( "ClientAudit" ) );
                }

                List<SiteAuditCustomModel> model = service.List1( pm, csm );

                int total = ( model.Count < pm.Take && pm.Skip == 0 ) ? model.Count : service.Total1( pm, csm );

                PagingExtension paging = PagingExtension.Create( model, total, pm.Skip, pm.Take, pm.Page );

                return PartialView( "_ClientAudit", paging );
            }
        }

        //
        // POST || GET: /ClientReporting/OutstandingPallets
        public ActionResult OutstandingPallets( PagingModel pm, CustomSearchModel csm, bool givecsm = false )
        {
            using ( ClientLoadService service = new ClientLoadService() )
            {
                if ( givecsm )
                {
                    ViewBag.ViewName = "OutstandingPallets";

                    return PartialView( "_OutstandingPalletsCustomSearch", new CustomSearchModel( "OutstandingPallets" ) );
                }

                List<OutstandingPalletsModel> model = GetOutstandingPallets( pm, csm );

                PagingExtension paging = PagingExtension.Create( model, model.Count, pm.Skip, pm.Take, pm.Page );

                return PartialView( "_OutstandingPallets", paging );
            }
        }

        //
        // POST || GET: /ClientReporting/MovementReport
        public ActionResult MovementReport( PagingModel pm, CustomSearchModel csm, bool givecsm = false )
        {
            using ( ClientLoadService service = new ClientLoadService() )
            {
                if ( givecsm )
                {
                    ViewBag.ViewName = "MovementReport";

                    return PartialView( "_MovementReportCustomSearch", new CustomSearchModel( "MovementReport" ) );
                }

                if ( !csm.FromDate.HasValue )
                {
                    csm.FromDate = DateTime.Now.AddMonths( -1 );
                }
                if ( !csm.ToDate.HasValue )
                {
                    csm.ToDate = DateTime.Now;
                }

                List<ClientLoadCustomModel> model = service.List1( pm, csm );

                int total = ( model.Count < pm.Take && pm.Skip == 0 ) ? model.Count : service.Total1( pm, csm );

                PagingExtension paging = PagingExtension.Create( model, total, pm.Skip, pm.Take, pm.Page );

                return PartialView( "_MovementReport", paging );
            }
        }

        //
        // POST || GET: /ClientReporting/Exceptions
        public ActionResult Exceptions( PagingModel pm, CustomSearchModel csm, bool givecsm = false )
        {
            using ( ClientLoadService service = new ClientLoadService() )
            {
                if ( givecsm )
                {
                    ViewBag.ViewName = "Exceptions";

                    return PartialView( "_ExceptionsCustomSearch", new CustomSearchModel( "Exceptions" ) );
                }

                csm.ReconciliationStatus = ReconciliationStatus.Unreconciled;

                List<ClientLoadCustomModel> model = service.List1( pm, csm );

                int total = ( model.Count < pm.Take && pm.Skip == 0 ) ? model.Count : service.Total1( pm, csm );

                if ( model.NullableAny() && pm.SortBy == "CreatedOn" )
                {
                    model = model.OrderBy( o => o.SiteId ).ThenBy( t => t.LoadDate ).ToList();
                }

                PagingExtension paging = PagingExtension.Create( model, total, pm.Skip, pm.Take, pm.Page );

                return PartialView( "_Exceptions", paging );
            }
        }

        #endregion
    }
}
