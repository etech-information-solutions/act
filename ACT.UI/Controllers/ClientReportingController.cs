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
            string csv = string.Empty;
            string filename = string.Format( "{0}-{1}.csv", type.ToUpperInvariant(), DateTime.Now.ToString( "yyyy_MM_dd_HH_mm" ) );

            pm.Skip = 0;
            pm.Take = int.MaxValue;

            int count = 0;

            switch ( type )
            {
                case "disputes":

                    #region Disputes

                    csv = string.Format( "Effective Date,Account Number,TDN Number,Raised Date,Docket Number,Reference,Action By,Resolved On,Resolved By,Other Party,Sender,Receiver,Declarer,Dispute Email,Product,Dispute Status,Equipment,Quantity,Reason fo Dispute {0}", Environment.NewLine );

                    using ( DisputeService dservice = new DisputeService() )
                    {
                        List<DisputeCustomModel> disputes = dservice.List1( pm, csm );

                        if ( disputes.NullableAny() )
                        {
                            foreach ( DisputeCustomModel item in disputes )
                            {
                                csv = string.Format( "{0} {1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17},{18},{19} {20}",
                                                    csv,
                                                    "\"" + item.EffectiveDate + "\"",
                                                    "\"" + item.ChepLoadAccountNumber + "\"",
                                                    "\"" + item.TDNNumber + "\"",
                                                    "\"" + item.CreatedOn + "\"",
                                                    "\"" + item.DocketNumber + "\"",
                                                    "\"" + item.Reference + "\"",
                                                    "\"" + item.ActionUser + "\"",
                                                    "\"" + item.ResolvedOn + "\"",
                                                    "\"" + item.ResolvedUser + "\"",
                                                    "\"" + item.OtherParty + "\"",
                                                    "\"" + item.Sender + "\"",
                                                    "\"" + item.Receiver + "\"",
                                                    "\"" + item.Declarer + "\"",
                                                    "\"" + item.DisputeEmail + "\"",
                                                    "\"" + item.Product + "\"",
                                                    "\"" + item.Status + "\"",
                                                    "\"" + item.Equipment + "\"",
                                                    "\"" + item.Quantity + "\"",
                                                    "\"" + item.DisputeReasonDetails + "\"",
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
                                                    "\"" + item.CreatedOn + "\"",
                                                    "\"" + item.SiteName + "\"",
                                                    "\"" + item.Equipment + "\"",
                                                    "\"" + item.ChepStockBalance + "\"",
                                                    "\"" + item.NotInvoiceIn + "\"",
                                                    "\"" + item.NotInvoiceOut + "\"",
                                                    "\"" + item.ReqAdjustment + "\"",
                                                    "\"" + item.NotMCCIn + "\"",
                                                    "\"" + item.NotMCCOut + "\"",
                                                    "\"" + item.MccBalance + "\"",
                                                    "\"" + item.SuspendITL + "\"",
                                                    "\"" + item.SuspendMCC + "\"",
                                                    "\"" + item.AdjustedInvBalance + "\"",
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
                        List<SiteAuditCustomModel> audits = dservice.List1( pm, csm );

                        if ( audits.NullableAny() )
                        {
                            foreach ( SiteAuditCustomModel item in audits )
                            {
                                csv = string.Format( "{0} {1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11} {12}",
                                                    csv,
                                                    "\"" + item.CreatedOn + "\"",
                                                    "\"" + item.AuditDate + "\"",
                                                    "\"" + item.SiteName + "\"",
                                                    "\"" + item.Equipment + "\"",
                                                    "\"" + item.PalletsOutstanding + "\"",
                                                    "\"" + item.PalletsCounted + "\"",
                                                    "\"" + item.WriteoffPallets + "\"",
                                                    "\"" + item.CustomerName + "\"",
                                                    "\"" + item.RepName + "\"",
                                                    "\"" + item.PalletAuditor + "\"",
                                                    "\"" + item.Status + "\"",
                                                    Environment.NewLine );
                            }
                        }
                    }

                    #endregion

                    break;

                case "outstandingpallets":

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

                case "movementreport":

                    #region Movement Report

                    csv = string.Format( "Movement Date,Effective Date,Date of Notification,Docket Number,Posting Type,Trading Partner,Customer Reference Number,Equipment,POP Number,PCN Number,PRN Number {0}", Environment.NewLine );

                    using ( ClientLoadService dservice = new ClientLoadService() )
                    {
                        if ( !csm.FromDate.HasValue )
                        {
                            csm.FromDate = DateTime.Now.AddMonths( -1 );
                        }
                        if ( !csm.ToDate.HasValue )
                        {
                            csm.ToDate = DateTime.Now;
                        }

                        List<ClientLoadCustomModel> clientload = dservice.List1( pm, csm );

                        if ( clientload.NullableAny() )
                        {
                            foreach ( ClientLoadCustomModel item in clientload )
                            {
                                csv = string.Format( "{0} {1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11} {12}",
                                                    csv,
                                                    "\"" + item.LoadDate + "\"",
                                                    "\"" + item.EffectiveDate + "\"",
                                                    "\"" + item.NotifyDate + "\"",
                                                    "\"" + item.DocketNumber + "\"",
                                                    "\"" + item.PostingType + "\"",
                                                    "\"" + item.ClientDescription + "\"",
                                                    "\"" + item.ReferenceNumber + "\"",
                                                    "\"" + item.Equipment + "\"",
                                                    "\"" + item.PODNumber + "\"",
                                                    "\"" + item.PCNNumber + "\"",
                                                    "\"" + item.PRNNumber + "\"",
                                                    Environment.NewLine );
                            }
                        }
                    }

                    #endregion

                    break;

                case "topoustandingcustomers":

                    #region Top Oustanding Customers

                    csv = $"Client, Region, Site, Reason, Total {Environment.NewLine}";

                    csm.BalanceStatus = BalanceStatus.NotBalanced;
                    csm.ReconciliationStatus = ReconciliationStatus.Reconciled;

                    List<OutstandingPalletsModel> toc = GetOustandingCustomers( pm, csm );

                    if ( toc.NullableAny() )
                    {
                        foreach ( OutstandingPalletsModel item in toc )
                        {
                            csv = $"{csv} {item.ClientLoad.ClientName},,,,{item.Total} {Environment.NewLine}";

                            foreach ( OutstandingRegionModel r in item.Regions )
                            {
                                csv = $"{csv},{r.Name},,,{r.Total} {Environment.NewLine}";

                                foreach ( OutstandingSiteModel s in r.Sites )
                                {
                                    csv = $"{csv},{s.Name},,,{s.Total} {Environment.NewLine}";

                                    foreach ( OutstandingReasonModel o in s.OutstandingReasons )
                                    {
                                        csv = $"{csv},,,{o.Description},{o.Total} {Environment.NewLine}";
                                    }
                                }
                            }

                            csv = $"{csv}{Environment.NewLine}TOTAL,,,,{item.Total} {Environment.NewLine}{Environment.NewLine}";
                        }
                    }

                    #endregion

                    break;

                case "podoutstanding":

                    #region POD Outstanding

                    csv = $"Load Date,Created,Client,Transporter,Vehicle,Docket Number,Ref,Other Ref,Equipment,Invoice Number,Transaction Type, Quantity,Outstanding Reason {Environment.NewLine}";

                    using ( ChepLoadService chservice = new ChepLoadService() )
                    {
                        csm.IsPODOutstanding = true;
                        csm.BalanceStatus = BalanceStatus.NotBalanced;
                        csm.ReconciliationStatus = ReconciliationStatus.Reconciled;

                        List<ChepLoadCustomModel> cheploads = chservice.List1( pm, csm );

                        if ( cheploads.NullableAny() )
                        {
                            foreach ( ChepLoadCustomModel item in cheploads )
                            {
                                csv = $"{csv}\"{item.ShipmentDate}\",\"{item.CreateDate}\",\"{item.ClientName}\",\"{item.TransporterName}\",\"{item.VehicleRegistration}\",\"{item.DocketNumber}\",\"{item.CorrectedRef ?? item.Ref}\",\"{item.CorrectedOtherRef ?? item.OtherRef}\",\"{item.Equipment}\",\"{item.InvoiceNumber}\",\"{item.TransactionType}\",\"{item.Quantity}\",\"{item.OutstandingReason}\" {Environment.NewLine}";
                            }
                        }
                    }

                    #endregion

                    break;

                case "transporterliablereport":

                    #region Transporter Liable Report

                    csv = $"Load Date,Created,Client,Transporter,Vehicle,Docket Number,Ref,Other Ref,Equipment,Invoice Number,Transaction Type, Quantity,Outstanding Reason {Environment.NewLine}";

                    using ( ChepLoadService chservice = new ChepLoadService() )
                    {
                        csm.IsPODOutstanding = true;
                        csm.IsTransporterLiable = true;
                        csm.BalanceStatus = BalanceStatus.NotBalanced;
                        csm.ReconciliationStatus = ReconciliationStatus.Reconciled;

                        List<ChepLoadCustomModel> cheploads = chservice.List1( pm, csm );

                        if ( cheploads.NullableAny() )
                        {
                            foreach ( ChepLoadCustomModel item in cheploads )
                            {
                                csv = $"{csv}\"{item.ShipmentDate}\",\"{item.CreateDate}\",\"{item.ClientName}\",\"{item.TransporterName}\",\"{item.VehicleRegistration}\",\"{item.DocketNumber}\",\"{item.CorrectedRef ?? item.Ref}\",\"{item.CorrectedOtherRef ?? item.OtherRef}\",\"{item.Equipment}\",\"{item.InvoiceNumber}\",\"{item.TransactionType}\",\"{item.Quantity}\",\"{item.OutstandingReason}\" {Environment.NewLine}";
                            }
                        }
                    }

                    #endregion

                    break;

                case "disputesresolved":

                    #region Disputes Resolved

                    csv = $"Effective Date,Original Docket Number,Docket Number,Ref,Other Ref,TDN,Quantity,Action By, Status,Reason {Environment.NewLine}";

                    using ( DisputeService dservice = new DisputeService() )
                    {
                        csm.DisputeStatus = DisputeStatus.Resolved;

                        List<DisputeCustomModel> disputes = dservice.List1( pm, csm );

                        if ( disputes.NullableAny() )
                        {
                            foreach ( DisputeCustomModel item in disputes )
                            {
                                //csv = $"{csv}\"{item.ShipmentDate}\",\"{item.CreateDate}\",\"{item.ClientName}\",\"{item.TransporterName}\",\"{item.VehicleRegistration}\",\"{item.DocketNumber}\",\"{item.CorrectedRef ?? item.Ref}\",\"{item.CorrectedOtherRef ?? item.OtherRef}\",\"{item.Equipment}\",\"{item.InvoiceNumber}\",\"{item.TransactionType}\",\"{item.Quantity}\",\"{item.OutstandingReason}\" {Environment.NewLine}";
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
            if ( givecsm )
            {
                ViewBag.ViewName = "OutstandingPallets";

                return PartialView( "_OutstandingPalletsCustomSearch", new CustomSearchModel( "OutstandingPallets" ) );
            }

            List<OutstandingPalletsModel> model = GetOutstandingPallets( pm, csm );

            PagingExtension paging = PagingExtension.Create( model, model.Count, pm.Skip, pm.Take, pm.Page );

            return PartialView( "_OutstandingPallets", paging );
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
        // POST || GET: /ClientReporting/TopOustandingCustomers
        public ActionResult TopOustandingCustomers( PagingModel pm, CustomSearchModel csm, bool givecsm = false )
        {
            if ( givecsm )
            {
                ViewBag.ViewName = "TopOustandingCustomers";

                return PartialView( "_TopOustandingCustomersCustomSearch", new CustomSearchModel( "TopOustandingCustomers" ) );
            }

            pm.Take = int.MaxValue;
            csm.BalanceStatus = BalanceStatus.NotBalanced;
            csm.ReconciliationStatus = ReconciliationStatus.Reconciled;

            List<OutstandingPalletsModel> model = GetOustandingCustomers( pm, csm );

            PagingExtension paging = PagingExtension.Create( model, model.Count, pm.Skip, pm.Take, pm.Page );

            return PartialView( "_TopOustandingCustomers", paging );
        }

        //
        // POST || GET: /ClientReporting/PODOutstanding
        public ActionResult PODOutstanding( PagingModel pm, CustomSearchModel csm, bool givecsm = false )
        {
            using ( ChepLoadService service = new ChepLoadService() )
            {
                if ( givecsm )
                {
                    ViewBag.ViewName = "PODOutstanding";

                    return PartialView( "_OutstandingPallets1CustomSearch", new CustomSearchModel( "PODOutstanding" ) );
                }

                csm.IsPODOutstanding = true;
                csm.BalanceStatus = BalanceStatus.NotBalanced;
                csm.ReconciliationStatus = ReconciliationStatus.Reconciled;

                List<ChepLoadCustomModel> model = service.List1( pm, csm );
                int total = ( model.Count < pm.Take && pm.Skip == 0 ) ? model.Count : service.Total1( pm, csm );

                PagingExtension paging = PagingExtension.Create( model, total, pm.Skip, pm.Take, pm.Page );

                return PartialView( "_PODOutstanding", paging );
            }
        }

        //
        // POST || GET: /ClientReporting/TransporterLiableReport
        public ActionResult TransporterLiableReport( PagingModel pm, CustomSearchModel csm, bool givecsm = false )
        {
            using ( ChepLoadService service = new ChepLoadService() )
            {
                if ( givecsm )
                {
                    ViewBag.ViewName = "TransporterLiableReport";

                    return PartialView( "_OutstandingPallets1CustomSearch", new CustomSearchModel( "TransporterLiableReport" ) );
                }

                csm.IsPODOutstanding = true;
                csm.IsTransporterLiable = true;
                csm.BalanceStatus = BalanceStatus.NotBalanced;
                csm.ReconciliationStatus = ReconciliationStatus.Reconciled;

                List<ChepLoadCustomModel> model = service.List1( pm, csm );
                int total = ( model.Count < pm.Take && pm.Skip == 0 ) ? model.Count : service.Total1( pm, csm );

                PagingExtension paging = PagingExtension.Create( model, total, pm.Skip, pm.Take, pm.Page );

                return PartialView( "_TransporterLiableReport", paging );
            }
        }

        //
        // POST || GET: /ClientReporting/DisputesResolved
        public ActionResult DisputesResolved( PagingModel pm, CustomSearchModel csm, bool givecsm = false )
        {
            using ( DisputeService service = new DisputeService() )
            {
                if ( givecsm )
                {
                    ViewBag.ViewName = "DisputesResolved";

                    return PartialView( "_DisputesCustomSearch", new CustomSearchModel( "Disputes" ) );
                }
;
                csm.DisputeStatus = DisputeStatus.Resolved;

                List<DisputeCustomModel> model = service.List1( pm, csm );
                int total = ( model.Count < pm.Take && pm.Skip == 0 ) ? model.Count : service.Total1( pm, csm );

                PagingExtension paging = PagingExtension.Create( model, total, pm.Skip, pm.Take, pm.Page );

                return PartialView( "_DisputesResolved", paging );
            }
        }

        //
        // POST || GET: /ClientReporting/CollectionsReport
        public ActionResult CollectionsReport( PagingModel pm, CustomSearchModel csm, bool givecsm = false )
        {
            using ( ChepLoadService service = new ChepLoadService() )
            {
                if ( givecsm )
                {
                    ViewBag.ViewName = "CollectionsReport";

                    return PartialView( "_OutstandingPallets1CustomSearch", new CustomSearchModel( "CollectionsReport" ) );
                }

                csm.IsPSPPickUp = true;
                csm.BalanceStatus = BalanceStatus.NotBalanced;
                csm.ReconciliationStatus = ReconciliationStatus.Reconciled;

                List<ChepLoadCustomModel> model = service.List1( pm, csm );
                int total = ( model.Count < pm.Take && pm.Skip == 0 ) ? model.Count : service.Total1( pm, csm );

                PagingExtension paging = PagingExtension.Create( model, total, pm.Skip, pm.Take, pm.Page );

                return PartialView( "_CollectionsReport", paging );
            }
        }

        //
        // POST || GET: /ClientReporting/AuthorisationStats
        public ActionResult AuthorisationStats( PagingModel pm, CustomSearchModel csm, bool givecsm = false )
        {
            using ( ClientAuthorisationService service = new ClientAuthorisationService() )
            {
                if ( givecsm )
                {
                    ViewBag.ViewName = "AuthorisationStats";

                    return PartialView( "_AuthorisationCodesCustomSearch", new CustomSearchModel( "AuthorisationCodes" ) );
                }

                pm.Sort = pm.SortBy == "CreatedOn" ? "ASC" : pm.Sort;
                pm.SortBy = pm.SortBy == "CreatedOn" ? "c.CompanyName" : pm.SortBy;

                List<CountModel> model = service.ListStats( pm, csm );

                PagingExtension paging = PagingExtension.Create( model, model.Count, pm.Skip, pm.Take, pm.Page );

                return PartialView( "_AuthorisationStats", paging );
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
