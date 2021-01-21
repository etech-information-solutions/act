using ACT.Core.Enums;
using ACT.Core.Models;
using ACT.Core.Models.Custom;
using ACT.Core.Services;
using ACT.Data.Models;
using ACT.UI.Models;
using ACT.UI.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Transactions;
using System.Web.Mvc;
using System.Web;
using Newtonsoft.Json;
using OpenPop;
using ACT.Core.Helpers;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;
using ACT.Mailer;
using Microsoft.VisualBasic.FileIO;
using System.Globalization;

namespace ACT.UI.Controllers
{
    [Requires( PermissionTo.View, PermissionContext.Pallet )]
    public class PalletController : BaseController
    {
        // GET: Pallet
        public ActionResult Index()
        {
            return View();
        }

        #region Exports

        //
        // GET: /Administration/Export
        public FileContentResult Export( PagingModel pm, CustomSearchModel csm, string type = "poolingagentdata" )
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

            }

            return File( new System.Text.UTF8Encoding().GetBytes( csv ), "text/csv", filename );
        }

        #endregion



        #region Pooling Agent Data

        //
        // GET: /Pallet/PoolingAgentDataDetails/5
        public ActionResult PoolingAgentDataDetails( int id, bool layout = true, bool canEdit = true )
        {
            using ( ChepLoadService chservice = new ChepLoadService() )
            using ( ClientLoadService clservice = new ClientLoadService() )
            {
                ChepLoad model = chservice.GetById( id );

                if ( model == null )
                {
                    Notify( "Sorry, the requested resource could not be found. Please try again", NotificationType.Error );

                    return PartialView( "_AccessDenied" );
                }

                if ( layout )
                {
                    ViewBag.IncludeLayout = true;
                }

                ViewBag.CanEdit = canEdit;

                ViewBag.ClientLoads = clservice.ListByChepRefOtherRef( model.Ref, model.OtherRef );

                return View( model );
            }
        }

        // GET: Pallet/AddPoolingAgentData
        [Requires( PermissionTo.Create )]
        public ActionResult AddPoolingAgentData()
        {
            ChepLoadViewModel model = new ChepLoadViewModel() { EditMode = true };

            return View( model );
        }

        // POST: Pallet/AddPoolingAgentData
        [HttpPost]
        [Requires( PermissionTo.Create )]
        public ActionResult AddPoolingAgentData( ChepLoadViewModel model )
        {
            if ( !ModelState.IsValid )
            {
                Notify( "Sorry, the item was not created. Please correct all errors and try again.", NotificationType.Error );

                return View( model );
            }

            using ( ChepLoadService clservice = new ChepLoadService() )
            {
                #region Create Chep Load

                ChepLoad clientload = new ChepLoad()
                {
                    UMI = model.UMI,
                    Reason = model.Reason,
                    Ref = model.Reference,
                    ClientId = model.ClientId,
                    BatchRef = model.BatchRef,
                    Location = model.Location,
                    OtherRef = model.OtherRef,
                    Quantity = model.Quantity,
                    CreatedBy = model.CreatedBy,
                    Equipment = model.Equipment,
                    ChepStatus = model.ChepStatus,
                    CreateDate = model.CreateDate,
                    LocationId = model.LocationId,
                    OtherParty = model.OtherParty,
                    DataSource = model.DataSource,
                    Status = ( int ) Status.Active,
                    OtherPartyId = model.OtherPartyId,
                    ShipmentDate = model.ShipmentDate,
                    DeliveryDate = model.DeliveryDate,
                    DocketNumber = model.DocketNumber,
                    BalanceStatus = model.BalanceStatus,
                    EffectiveDate = model.EffectiveDate,
                    EquipmentCode = model.EquipmentCode,
                    InvoiceNumber = model.InvoiceNumber,
                    PostingType = ( int ) model.PostingType,
                    TransactionType = model.TransactionType,
                    OtherPartyCountry = model.OtherPartyCountry,
                    OriginalDocketNumber = model.OriginalDocketNumber,
                };

                clservice.Create( clientload );

                #endregion
            }

            Notify( "The item was successfully created.", NotificationType.Success );

            return PoolingAgentData( new PagingModel(), new CustomSearchModel() );
        }

        // GET: Pallet/EditPoolingAgentData/5
        [Requires( PermissionTo.Edit )]
        public ActionResult EditPoolingAgentData( int id )
        {
            using ( ChepLoadService service = new ChepLoadService() )
            {
                ChepLoad load = service.GetById( id );

                if ( load == null )
                {
                    Notify( "Sorry, the requested resource could not be found. Please try again", NotificationType.Error );

                    return PartialView( "_AccessDenied" );
                }

                ChepLoadViewModel model = new ChepLoadViewModel()
                {
                    Id = load.Id,
                    UMI = load.UMI,
                    EditMode = true,
                    Reason = load.Reason,
                    Reference = load.Ref,
                    ClientId = load.ClientId,
                    BatchRef = load.BatchRef,
                    Location = load.Location,
                    OtherRef = load.OtherRef,
                    Quantity = load.Quantity,
                    CreatedBy = load.CreatedBy,
                    Equipment = load.Equipment,
                    ChepStatus = load.ChepStatus,
                    CreateDate = load.CreateDate,
                    LocationId = load.LocationId,
                    OtherParty = load.OtherParty,
                    DataSource = load.DataSource,
                    OtherPartyId = load.OtherPartyId,
                    ShipmentDate = load.ShipmentDate,
                    DeliveryDate = load.DeliveryDate,
                    DocketNumber = load.DocketNumber,
                    BalanceStatus = load.BalanceStatus,
                    EffectiveDate = load.EffectiveDate,
                    EquipmentCode = load.EquipmentCode,
                    InvoiceNumber = load.InvoiceNumber,
                    TransactionType = load.TransactionType,
                    OtherPartyCountry = load.OtherPartyCountry,
                    PostingType = ( PostingType ) load.PostingType,
                    OriginalDocketNumber = load.OriginalDocketNumber,
                };

                return View( model );
            }
        }

        // POST: Pallet/EditPoolingAgentData/5
        [HttpPost]
        [Requires( PermissionTo.Edit )]
        public ActionResult EditPoolingAgentData( ChepLoadViewModel model )
        {
            if ( !ModelState.IsValid )
            {
                Notify( "Sorry, the selected item was not updated. Please correct all errors and try again.", NotificationType.Error );

                return View( model );
            }

            using ( ChepLoadService service = new ChepLoadService() )
            {
                ChepLoad load = service.GetById( model.Id );

                if ( load == null )
                {
                    Notify( "Sorry, that item does not exist! Please specify a valid item Id and try again.", NotificationType.Error );

                    return View( model );
                }

                #region Update Chep Load

                if ( CurrentUser.IsAdmin )
                {
                    load.ClientId = model.ClientId;
                }

                load.UMI = model.UMI;
                load.Reason = model.Reason;
                load.Ref = model.Reference;
                load.BatchRef = model.BatchRef;
                load.Location = model.Location;
                load.OtherRef = model.OtherRef;
                load.Quantity = model.Quantity;
                load.CreatedBy = model.CreatedBy;
                load.Equipment = model.Equipment;
                load.ChepStatus = model.ChepStatus;
                load.CreateDate = model.CreateDate;
                load.LocationId = model.LocationId;
                load.OtherParty = model.OtherParty;
                load.DataSource = model.DataSource;
                load.OtherPartyId = model.OtherPartyId;
                load.ShipmentDate = model.ShipmentDate;
                load.DeliveryDate = model.DeliveryDate;
                load.DocketNumber = model.DocketNumber;
                load.BalanceStatus = model.BalanceStatus;
                load.EffectiveDate = model.EffectiveDate;
                load.EquipmentCode = model.EquipmentCode;
                load.InvoiceNumber = model.InvoiceNumber;
                load.TransactionType = model.TransactionType;
                load.OtherPartyCountry = model.OtherPartyCountry;
                load.OriginalDocketNumber = model.OriginalDocketNumber;

                service.Update( load );

                #endregion
            }

            Notify( "The selected Item details were successfully updated.", NotificationType.Success );

            return PoolingAgentData( new PagingModel(), new CustomSearchModel() );
        }

        // GET: Client/AddSite
        [Requires( PermissionTo.Create )]
        public ActionResult ImportPoolingAgentData()
        {
            ChepLoadViewModel model = new ChepLoadViewModel() { EditMode = true };

            return View( model );
        }

        // POST: Client/AddSite
        [HttpPost]
        [Requires( PermissionTo.Create )]
        public ActionResult ImportPoolingAgentData( ChepLoadViewModel model )
        {
            if ( model.File == null )
            {
                Notify( "Please select a file to upload and try again.", NotificationType.Error );

                return View( model );
            }

            int line = 0,
                count = 0,
                errors = 0,
                skipped = 0,
                created = 0,
                updated = 0,
                errorDocId = 0;

            string cQuery, uQuery;

            List<string> errs = new List<string>();

            using ( SiteService sservice = new SiteService() )
            using ( ChepLoadService clservice = new ChepLoadService() )
            using ( ClientSiteService csservice = new ClientSiteService() )
            using ( ClientCustomerService ccservice = new ClientCustomerService() )
            using ( TextFieldParser parser = new TextFieldParser( model.File.InputStream ) )
            {
                parser.Delimiters = new string[] { "," };

                while ( true )
                {
                    string[] load = parser.ReadFields();

                    if ( load == null )
                    {
                        break;
                    }

                    line++;

                    if ( line == 1 ) continue;

                    cQuery = uQuery = string.Empty;

                    count++;

                    if ( load.NullableCount() < 2 )
                    {
                        skipped++;

                        continue;
                    }

                    load = load.ToSQLSafe();

                    ChepLoad l = clservice.Get( model.ClientId, load[ 4 ] );

                    int.TryParse( load[ 13 ].Trim(), out int qty );

                    string shipmentDate1 = string.Empty,
                           deliveryDate1 = string.Empty,
                           effectiveDate1 = string.Empty,
                           createDate1 = string.Empty;

                    #region Dates

                    if ( !DateTime.TryParse( load[ 17 ], out DateTime shipmentDate ) )
                    {
                        if ( !DateTime.TryParseExact( load[ 17 ], "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out shipmentDate ) )
                        {
                            shipmentDate1 = "NULL";
                        }
                        else
                        {
                            shipmentDate1 = $"'{shipmentDate}'";
                        }
                    }
                    else
                    {
                        shipmentDate1 = $"'{shipmentDate}'";
                    }

                    if ( !DateTime.TryParse( load[ 18 ], out DateTime deliveryDate ) )
                    {
                        if ( !DateTime.TryParseExact( load[ 18 ], "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out deliveryDate ) )
                        {
                            deliveryDate1 = "NULL";
                        }
                        else
                        {
                            deliveryDate1 = $"'{deliveryDate}'";
                        }
                    }
                    else
                    {
                        deliveryDate1 = $"'{deliveryDate}'";
                    }

                    if ( !DateTime.TryParse( load[ 19 ], out DateTime effectiveDate ) )
                    {
                        if ( !DateTime.TryParseExact( load[ 19 ], "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out effectiveDate ) )
                        {
                            effectiveDate1 = "NULL";
                        }
                        else
                        {
                            effectiveDate1 = $"'{effectiveDate}'";
                        }
                    }
                    else
                    {
                        effectiveDate1 = $"'{effectiveDate}'";
                    }

                    if ( !DateTime.TryParse( load[ 20 ], out DateTime createDate ) )
                    {
                        if ( !DateTime.TryParseExact( load[ 20 ], "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out createDate ) )
                        {
                            createDate1 = "NULL";
                        }
                        else
                        {
                            createDate1 = $"'{createDate}'";
                        }
                    }
                    else
                    {
                        createDate1 = $"'{createDate}'";
                    }

                    #endregion

                    #region Client Site

                    ClientSite cs = new ClientSite();
                    Site site = sservice.GetBySiteCode( load[ 6 ].Trim() );

                    if ( site == null )
                    {
                        site = new Site()
                        {
                            Name = load[ 7 ].Trim(),
                            Description = load[ 7 ].Trim(),
                            SiteCodeChep = load[ 6 ].Trim(),
                            Status = ( int ) Status.Active,
                        };

                        site = sservice.Create( site );
                    }

                    if ( !site.ClientSites.NullableAny() )
                    {
                        ClientCustomer cc = ccservice.GetByClient( model.ClientId );

                        if ( cc == null )
                        {
                            cc = new ClientCustomer()
                            {
                                ClientId = model.ClientId,
                                Status = ( int ) Status.Active,
                                CustomerName = load[ 7 ].Trim(),
                                CustomerNumber = load[ 6 ].Trim(),
                            };

                            cc = ccservice.Create( cc );
                        }

                        cs = new ClientSite()
                        {
                            SiteId = site.Id,
                            ClientCustomerId = cc.Id,
                            Status = ( int ) Status.Active,
                            AccountingCode = load[ 6 ].Trim(),
                            ClientSiteCode = load[ 6 ].Trim(),
                        };

                        cs = csservice.Create( cs );
                    }
                    else
                    {
                        cs = site.ClientSites.FirstOrDefault();
                    }

                    #endregion

                    if ( l == null )
                    {
                        #region Create Chep Load

                        cQuery = $" {cQuery} INSERT INTO [dbo].[ChepLoad] ([ClientId],[ClientSiteId],[CreatedOn],[ModifiedOn],[ModifiedBy],[ChepStatus],[TransactionType],[DocketNumber],[OriginalDocketNumber],[UMI],[LocationId],[Location],[OtherPartyId],[OtherParty],[OtherPartyCountry],[EquipmentCode],[Equipment],[Quantity],[Ref],[OtherRef],[BatchRef],[ShipmentDate],[DeliveryDate],[EffectiveDate],[CreateDate],[CreatedBy],[InvoiceNumber],[Reason],[DataSource],[BalanceStatus],[Status],[PostingType]) ";
                        cQuery = $" {cQuery} VALUES ({model.ClientId},{cs.Id},'{DateTime.Now}','{DateTime.Now}','{CurrentUser.Email}','{load[ 0 ]}','{load[ 2 ]}','{load[ 3 ]}','{load[ 4 ]}','{load[ 5 ]}','{load[ 6 ]}','{load[ 7 ]}','{load[ 8 ]}','{load[ 9 ]}','{load[ 10 ]}','{load[ 11 ]}','{load[ 12 ]}',{qty},'{load[ 14 ]}','{load[ 15 ]}','{load[ 16 ]}',{shipmentDate1},{deliveryDate1},{effectiveDate1},{createDate1},'{load[ 21 ]}','{load[ 22 ]}','{load[ 23 ]}','{load[ 24 ]}',{( int ) ReconciliationStatus.Unreconciled},{( int ) ReconciliationStatus.Unreconciled},{( int ) PostingType.Import}) ";

                        #endregion

                        try
                        {
                            clservice.Query( cQuery );

                            created++;
                        }
                        catch ( Exception ex )
                        {
                            errors++;

                            errs.Add( ex.ToString() );
                        }
                    }
                    else
                    {
                        #region Update ChepLoad

                        uQuery = $@"{uQuery} UPDATE [dbo].[ChepLoad] SET
                                                    [ClientSiteId]={cs.Id},
                                                    [ModifiedOn]='{DateTime.Now}',
                                                    [ModifiedBy]='{CurrentUser.Email}',
                                                    [ChepStatus]='{load[ 0 ]}',
                                                    [TransactionType]='{load[ 2 ]}',
                                                    [OriginalDocketNumber]='{load[ 4 ]}',
                                                    [UMI]='{load[ 5 ]}',
                                                    [LocationId]='{load[ 6 ]}',
                                                    [Location]='{load[ 7 ]}',
                                                    [OtherPartyId]='{load[ 8 ]}',
                                                    [OtherParty]='{load[ 9 ]}',
                                                    [OtherPartyCountry]='{load[ 10 ]}',
                                                    [EquipmentCode]='{load[ 11 ]}',
                                                    [Equipment]='{load[ 12 ]}',
                                                    [Quantity]={qty},
                                                    [Ref]='{load[ 14 ]}',
                                                    [OtherRef]='{load[ 15 ]}',
                                                    [BatchRef]='{load[ 16 ]}',
                                                    [ShipmentDate]={shipmentDate1},
                                                    [DeliveryDate]={deliveryDate1},
                                                    [EffectiveDate]={effectiveDate1},
                                                    [CreateDate]={createDate1},
                                                    [CreatedBy]='{load[ 21 ]}',
                                                    [InvoiceNumber]='{load[ 22 ]}',
                                                    [Reason]='{load[ 23 ]}',
                                                    [DataSource]='{load[ 24 ]}'
                                                WHERE
                                                    [Id]={l.Id}";

                        #endregion

                        try
                        {
                            clservice.Query( uQuery );

                            updated++;
                        }
                        catch ( Exception ex )
                        {
                            errors++;

                            errs.Add( ex.ToString() );
                        }
                    }
                }

                cQuery = string.Empty;
                uQuery = string.Empty;

                if ( errs.NullableAny() )
                {
                    errorDocId = LogImportErrors( errs, model.ClientId );
                }
            }

            AutoReconcileLoads();

            string resp = $"{created} loads were successfully created, {updated} were updated, {skipped} were skipped and there were {errors} errors.";

            if ( errs.NullableAny() && errorDocId > 0 )
            {
                resp = $"{resp} <a href='/Pallet/ViewDocument/{errorDocId}' target='_blank'>Click here</a> to view the erros.";
            }

            Notify( resp, NotificationType.Success );

            return PoolingAgentData( new PagingModel(), new CustomSearchModel() );
        }

        // POST: Pallet/DeletePoolingAgentData/5
        [HttpPost]
        [Requires( PermissionTo.Delete )]
        public ActionResult DeletePoolingAgentData( ChepLoadViewModel model )
        {
            using ( ChepLoadService service = new ChepLoadService() )
            {
                ChepLoad load = service.GetById( model.Id );

                if ( load == null )
                {
                    Notify( "Sorry, the requested resource could not be found. Please try again", NotificationType.Error );

                    return PartialView( "_AccessDenied" );
                }

                load.Status = ( ( ( Status ) load.Status ) == Status.Active ) ? ( int ) Status.Inactive : ( int ) Status.Active;

                service.Update( load );

                Notify( "The selected item was successfully updated.", NotificationType.Success );

                return PoolingAgentData( new PagingModel(), new CustomSearchModel() );
            }
        }

        // GET: Pallet/AddPoolingAgentData
        [Requires( PermissionTo.Create )]
        public ActionResult AllocatePoolingAgentData( int id )
        {
            ChepLoadChepViewModel model = new ChepLoadChepViewModel() { ChepLoadId = id };

            return PartialView( "_AllocatePoolingAgentData", model );
        }

        // POST: Pallet/AllocatePoolingAgentData
        [HttpPost]
        [Requires( PermissionTo.Create )]
        public ActionResult AllocatePoolingAgentData( ChepLoadChepViewModel model, int chepLoadId )
        {
            if ( !model.ChepLoadAllocations.NullableAny() )
            {
                Notify( "Sorry, the items were not created. Please correct all errors and try again.", NotificationType.Error );

                return PartialView( "_AllocatePoolingAgentData", model );
            }

            using ( ChepLoadService chservice = new ChepLoadService() )
            using ( ChepLoadChepService clcservice = new ChepLoadChepService() )
            {
                List<ChepLoad> dLoads = new List<ChepLoad>();
                List<ChepLoad> refLoads = chservice.ListByReference( model.ChepLoadAllocations.FirstOrDefault().Reference );

                int rSum = refLoads.Sum( s => s.Quantity ?? 0 );
                int aSum = model.ChepLoadAllocations.Sum( s => s.Quantity );

                if ( ( aSum + rSum ) > 0 )
                {
                    Notify( "Sorry, the sum of the allocated quantities (positive) may not exceed the difference of the sum of quantities of a ref/otherref.", NotificationType.Error );

                    return PartialView( "_AllocatePoolingAgentData", model );
                }

                foreach ( ChepLoadChepViewModel item in model.ChepLoadAllocations )
                {
                    dLoads.AddRange( chservice.ListByDocketNumber( item.DocketNumber ) );

                    List<ChepLoad> loads = chservice.ListDocketNumberAndReference( item.DocketNumber, item.Reference );

                    ChepLoadChep load = clcservice.GetById( item.Id );

                    Status status = ( loads.NullableAny() ) ? Status.Active : Status.Inactive;

                    if ( load == null )
                    {
                        #region Chep Load Chep

                        load = new ChepLoadChep()
                        {
                            PCN = item.PCN,
                            PRN = item.PRN,
                            ChepLoadId = chepLoadId,
                            Quantity = item.Quantity,
                            Reference = item.Reference,
                            DocketNumber = item.DocketNumber,
                            EffectiveDate = item.EffectiveDate,
                        };

                        clcservice.Create( load );

                        #endregion
                    }
                    else
                    {
                        #region Update Chep Load Chep

                        load.PCN = item.PCN;
                        load.PRN = item.PRN;
                        load.Status = ( int ) status;
                        load.ChepLoadId = chepLoadId;
                        load.Quantity = item.Quantity;
                        load.Reference = item.Reference;
                        load.DocketNumber = item.DocketNumber;
                        load.EffectiveDate = item.EffectiveDate;

                        clcservice.Update( load );

                        #endregion
                    }
                }

                List<int> uIds = new List<int>();

                uIds.AddRange( dLoads.Select( s => s.Id ) );
                uIds.AddRange( refLoads.Select( s => s.Id ) );

                if ( ( aSum - rSum ) == 0 && dLoads.Count == model.ChepLoadAllocations.Count && refLoads.NullableAny() )
                {
                    string q = $"UPDATE [dbo].[ChepLoad] SET [Status]={( int ) ReconciliationStatus.Reconciled},[BalanceStatus]={( int ) BalanceStatus.Balanced},[ModifiedOn]='{DateTime.Now}',[ModifiedBy]='{CurrentUser?.Email}' WHERE Id IN({string.Join( ",", uIds )});";

                    chservice.Query( q );
                }
                else
                {
                    string q = $"UPDATE [dbo].[ChepLoad] SET [BalanceStatus]={( int ) BalanceStatus.NotBalanced},[ModifiedOn]='{DateTime.Now}',[ModifiedBy]='{CurrentUser?.Email}' WHERE Id IN({string.Join( ",", uIds )});";

                    chservice.Query( q );
                }
            }

            Notify( "The items were successfully updated.", NotificationType.Success );

            return AllocatePoolingAgentData( chepLoadId );
        }

        #endregion

        //-------------------------------------------------------------------------------------

        #region Client Data

        //
        // GET: /Pallet/ClientDataDetails/5
        public ActionResult ClientDataDetails( int id, bool layout = true, bool canEdit = true )
        {
            using ( ChepLoadService chservice = new ChepLoadService() )
            using ( ClientLoadService clservice = new ClientLoadService() )
            {
                ClientLoad model = clservice.GetById( id );

                if ( model == null )
                {
                    Notify( "Sorry, the requested resource could not be found. Please try again", NotificationType.Error );

                    return PartialView( "_AccessDenied" );
                }

                if ( layout )
                {
                    ViewBag.IncludeLayout = true;
                }

                ViewBag.CanEdit = canEdit;

                ViewBag.ChepLoads = chservice.ListByReference( model.ReceiverNumber );

                return View( model );
            }
        }

        // GET: Pallet/AddClientLoad
        [Requires( PermissionTo.Create )]
        public ActionResult AddClientData()
        {
            ClientLoadViewModel model = new ClientLoadViewModel() { EditMode = true };

            return View( model );
        }

        // POST: Pallet/AddClientData
        [HttpPost]
        [Requires( PermissionTo.Create )]
        public ActionResult AddClientData( ClientLoadViewModel model )
        {
            if ( !ModelState.IsValid )
            {
                Notify( "Sorry, the item was not created. Please correct all errors and try again.", NotificationType.Error );

                return View( model );
            }

            using ( ClientLoadService clservice = new ClientLoadService() )
            using ( DeliveryNoteService dnservice = new DeliveryNoteService() )
            using ( DeliveryNoteLineService dnlservice = new DeliveryNoteLineService() )
            using ( AddressService aservice = new AddressService() )
            using ( ClientService cservice = new ClientService() )
            using ( TransactionScope scope = new TransactionScope() )
            {
                #region Create Client Load

                ClientLoad load = new ClientLoad()
                {
                    THAN = model.THAN,
                    ClientId = model.ClientId,
                    LoadDate = model.LoadDate,
                    Equipment = model.Equipment,
                    ReturnQty = model.ReturnQty,
                    PODNumber = model.PODNumber,
                    PCNNumber = model.PCNNumber,
                    PRNNumber = model.PRNNumber,
                    VehicleId = model.VehicleId,
                    DebriefQty = model.DebriefQty,
                    LoadNumber = model.LoadNumber,
                    NotifyDate = model.NotifyDate,
                    Status = ( int ) model.Status,
                    NewQuantity = model.NewQuantity,
                    DeliveryNote = model.DeliveryNote,
                    ClientSiteId = model.ClientSiteId,
                    ReconcileDate = model.ReconcileDate,
                    TransporterId = model.TransporterId,
                    AccountNumber = model.AccountNumber,
                    EffectiveDate = model.EffectiveDate,
                    ChepInvoiceNo = model.ChepInvoiceNo,
                    AdminMovement = model.AdminMovement,
                    ReceiverNumber = model.ReceiverNumber,
                    OutstandingQty = model.OutstandingQty,
                    ReferenceNumber = model.ReferenceNumber,
                    OriginalQuantity = model.OriginalQuantity,
                    ClientDescription = model.ClientDescription,
                    ChepCompensationNo = model.ChepCompensationNo,
                    OutstandingReasonId = model.OutstandingReasonId,
                    ReconcileInvoice = model.ReconcileInvoice.GetBoolValue(),
                };

                load = clservice.Create( load );

                #endregion

                Address address = aservice.Get( model.ClientId, "Client" );

                string customerAddress = $"{address?.Addressline1} {address?.Addressline2} {address?.Town} {address?.PostalCode}";

                #region Create Delivery Note

                Client client = cservice.GetById( model.ClientId );

                DeliveryNote deliveryNote = new DeliveryNote()
                {
                    ClientId = model.ClientId,
                    ClientSiteId = model.ClientSiteId,

                    OrderDate = model.LoadDate,
                    EmailAddress = client.Email,
                    Status = ( int ) Status.Active,
                    Reference306 = model.LoadNumber,
                    CustomerName = client.CompanyName,
                    InvoiceNumber = model.DeliveryNote,
                    OrderNumber = model.ReferenceNumber,
                    ContactNumber = model.ReceiverNumber,

                    CustomerAddress = customerAddress,
                    CustomerProvince = address?.Province,
                    CustomerPostalCode = address?.PostalCode,

                    DeliveryAddress = customerAddress,
                    DeliveryProvince = address?.Province,
                    DeliveryPostalCode = address?.PostalCode,

                    BillingAddress = customerAddress,
                    BillingProvince = address?.Province,
                    BililngPostalCode = address?.PostalCode,
                };

                deliveryNote = dnservice.Create( deliveryNote );

                #endregion

                #region Delivery Note Line

                DeliveryNoteLine dnl = new DeliveryNoteLine
                {
                    Returned = 0,
                    Product = model.Equipment,
                    DeliveryNoteId = deliveryNote.Id,
                    Equipment = model.Equipment,
                    Delivered = model.NewQuantity,
                    Status = ( int ) Status.Active,
                    ProductDescription = model.Equipment,
                    OrderQuantity = model.OriginalQuantity,
                };

                dnlservice.Create( dnl );

                #endregion

                #region Delivery Note Address

                // Create Invoice Customer Address
                Address invoiceCustomerAddress = new Address()
                {
                    Type = 1,
                    Town = address?.Town,
                    ObjectId = deliveryNote.Id,
                    ObjectType = "DeliveryNote",
                    Status = ( int ) Status.Active,
                    PostalCode = address?.PostalCode,
                    Addressline1 = address?.Addressline1,
                    Addressline2 = address?.Addressline2,
                    Province = ( int ) address?.Province,
                };

                aservice.Create( invoiceCustomerAddress );

                #endregion

                scope.Complete();
            }

            Notify( "The item was successfully created.", NotificationType.Success );

            return ClientData( new PagingModel(), new CustomSearchModel() );
        }

        // GET: Pallet/EditClientData/5
        [Requires( PermissionTo.Edit )]
        public ActionResult EditClientData( int id )
        {
            using ( ClientLoadService service = new ClientLoadService() )
            {
                ClientLoad load = service.GetById( id );

                if ( load == null )
                {
                    Notify( "Sorry, the requested resource could not be found. Please try again", NotificationType.Error );

                    return PartialView( "_AccessDenied" );
                }

                ClientLoadViewModel model = new ClientLoadViewModel()
                {
                    Id = load.Id,
                    EditMode = true,
                    THAN = load.THAN,
                    LoadDate = load.LoadDate,
                    ClientId = load.ClientId,
                    VehicleId = load.VehicleId,
                    Equipment = load.Equipment,
                    ReturnQty = load.ReturnQty,
                    PODNumber = load.PODNumber,
                    PCNNumber = load.PCNNumber,
                    PRNNumber = load.PRNNumber,
                    DebriefQty = load.DebriefQty,
                    LoadNumber = load.LoadNumber,
                    NotifyDate = load.NotifyDate,
                    NewQuantity = load.NewQuantity,
                    DeliveryNote = load.DeliveryNote,
                    ClientSiteId = load.ClientSiteId,
                    TransporterId = load.TransporterId,
                    ReconcileDate = load.ReconcileDate,
                    AccountNumber = load.AccountNumber,
                    EffectiveDate = load.EffectiveDate,
                    AdminMovement = load.AdminMovement,
                    ChepInvoiceNo = load.ChepInvoiceNo,
                    OutstandingQty = load.OutstandingQty,
                    ReceiverNumber = load.ReceiverNumber,
                    ReferenceNumber = load.ReferenceNumber,
                    OriginalQuantity = load.OriginalQuantity,
                    ClientDescription = load.ClientDescription,
                    ChepCompensationNo = load.ChepCompensationNo,
                    Status = ( ReconciliationStatus ) load.Status,
                    OutstandingReasonId = load.OutstandingReasonId,
                    ReconcileInvoice = load.ReconcileInvoice == true ? YesNo.Yes : YesNo.No,
                };

                return View( model );
            }
        }

        // POST: Pallet/EditClientData/5
        [HttpPost]
        [Requires( PermissionTo.Edit )]
        public ActionResult EditClientData( ClientLoadViewModel model )
        {
            if ( !ModelState.IsValid )
            {
                Notify( "Sorry, the selected Client Load was not updated. Please correct all errors and try again.", NotificationType.Error );

                return View( model );
            }

            using ( ClientLoadService clservice = new ClientLoadService() )
            using ( DeliveryNoteLineService dnlservice = new DeliveryNoteLineService() )
            {
                ClientLoad load = clservice.GetById( model.Id );

                if ( load == null )
                {
                    Notify( "Sorry, the requested resource could not be found. Please try again", NotificationType.Error );

                    return PartialView( "_AccessDenied" );
                }

                bool updateDeliveryNote = load.NewQuantity != model.NewQuantity || load.OriginalQuantity != model.OriginalQuantity;

                #region Update Client Load

                load.THAN = model.THAN;
                load.ClientId = model.ClientId;
                load.LoadDate = model.LoadDate;
                load.Equipment = model.Equipment;
                load.ReturnQty = model.ReturnQty;
                load.PODNumber = model.PODNumber;
                load.PCNNumber = model.PCNNumber;
                load.PRNNumber = model.PRNNumber;
                load.VehicleId = model.VehicleId;
                load.DebriefQty = model.DebriefQty;
                load.LoadNumber = model.LoadNumber;
                load.NotifyDate = model.NotifyDate;
                load.Status = ( int ) model.Status;
                load.NewQuantity = model.NewQuantity;
                load.DeliveryNote = model.DeliveryNote;
                load.ClientSiteId = model.ClientSiteId;
                load.ReconcileDate = model.ReconcileDate;
                load.TransporterId = model.TransporterId;
                load.AccountNumber = model.AccountNumber;
                load.EffectiveDate = model.EffectiveDate;
                load.ChepInvoiceNo = model.ChepInvoiceNo;
                load.AdminMovement = model.AdminMovement;
                load.ReceiverNumber = model.ReceiverNumber;
                load.OutstandingQty = model.OutstandingQty;
                load.ReferenceNumber = model.ReferenceNumber;
                load.OriginalQuantity = model.OriginalQuantity;
                load.ClientDescription = model.ClientDescription;
                load.ChepCompensationNo = model.ChepCompensationNo;
                load.OutstandingReasonId = model.OutstandingReasonId;
                load.ReconcileInvoice = model.ReconcileInvoice.GetBoolValue();

                clservice.Update( load );

                #endregion

                if ( updateDeliveryNote )
                {
                    #region Update Delivery Note Lines

                    List<DeliveryNoteLine> deliveryNoteLines = dnlservice.ListByLoadNumber( model.LoadNumber );

                    if ( deliveryNoteLines.NullableAny() )
                    {
                        foreach ( DeliveryNoteLine dnl in deliveryNoteLines )
                        {
                            dnl.Product = model.Equipment;
                            dnl.Equipment = model.Equipment;
                            dnl.Delivered = model.NewQuantity;
                            dnl.ProductDescription = model.Equipment;
                            dnl.OrderQuantity = model.OriginalQuantity;

                            dnlservice.Update( dnl );
                        }
                    }

                    #endregion
                }
            }

            Notify( "The selected Client Load details were successfully updated.", NotificationType.Success );

            return ClientData( new PagingModel(), new CustomSearchModel() );
        }

        // GET: Pallet/ImportClientData
        [Requires( PermissionTo.Create )]
        public ActionResult ImportClientData()
        {
            ClientLoadViewModel model = new ClientLoadViewModel() { EditMode = true };

            return View( model );
        }

        // POST: Pallet/ImportClientData
        [HttpPost]
        [Requires( PermissionTo.Create )]
        public ActionResult ImportClientData( ClientLoadViewModel model )
        {
            if ( model.File == null )
            {
                Notify( "Please select a file to upload and try again.", NotificationType.Error );

                return View( model );
            }

            int line = 0,
                count = 0,
                errors = 0,
                skipped = 0,
                created = 0,
                updated = 0,
                errorDocId = 0;

            string cQuery, uQuery;

            List<string> errs = new List<string>();

            using ( SiteService sservice = new SiteService() )
            using ( VehicleService vservice = new VehicleService() )
            using ( ClientSiteService csservice = new ClientSiteService() )
            using ( ClientLoadService clservice = new ClientLoadService() )
            using ( TransporterService tservice = new TransporterService() )
            using ( ClientCustomerService ccservice = new ClientCustomerService() )
            using ( TextFieldParser parser = new TextFieldParser( model.File.InputStream ) )
            {
                parser.Delimiters = new string[] { "," };

                while ( true )
                {
                    string[] load = parser.ReadFields();

                    if ( load == null )
                    {
                        break;
                    }

                    line++;

                    if ( line == 1 ) continue;

                    cQuery = uQuery = string.Empty;

                    count++;

                    if ( load.NullableCount() < 2 )
                    {
                        skipped++;

                        continue;
                    }

                    load = load.ToSQLSafe();

                    string[] podPieces = load[ 11 ].Split( ';' );

                    int podStatus = podPieces.Count() > 0 ? 1 : 0;

                    string pod = podPieces.Count() > 0 ? podPieces[ 0 ] : string.Empty,
                           pcn = podPieces.Count() > 1 ? podPieces[ 1 ] : string.Empty,
                           prn = load[ 20 ],
                           reg = load[ 13 ];

                    int.TryParse( load[ 16 ], out int qty );

                    int? returnQty = !string.IsNullOrWhiteSpace( load[ 20 ] ) ? qty : 0;

                    ReconciliationStatus status = ReconciliationStatus.Unreconciled;

                    string uid = clservice.GetSha1Md5String( $"{model.ClientId}{load[ 0 ]}{load[ 2 ]}" );

                    if ( uid.Length > 150 )
                    {
                        uid = uid.Substring( 0, 150 );
                    }

                    if ( !DateTime.TryParse( load[ 3 ], out DateTime loadDate ) )
                    {
                        DateTime.TryParseExact( load[ 3 ], "yyyy-MM-dd h:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime loadDate1 );

                        loadDate = loadDate1;
                    }

                    if ( !DateTime.TryParse( load[ 3 ], out DateTime deliveryDate ) )
                    {
                        DateTime.TryParseExact( load[ 3 ], "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime deliveryDate1 );

                        deliveryDate = deliveryDate1;
                    }

                    ClientLoad l = clservice.GetByUID( uid );

                    #region Transporter

                    Transporter t = tservice.GetByName( load[ 15 ] );

                    if ( t == null )
                    {
                        t = new Transporter()
                        {
                            Name = load[ 15 ],
                            TradingName = load[ 15 ],
                            RegistrationNumber = load[ 14 ],
                            Status = ( int ) Status.Active,
                        };

                        t = tservice.Create( t );
                    }

                    #endregion

                    #region Vehicle

                    Vehicle v = vservice.GetByRegistrationNumber( reg, "Transporter" );

                    if ( v == null )
                    {
                        v = new Vehicle()
                        {
                            ObjectId = t.Id,
                            Descriptoin = reg,
                            Registration = reg,
                            ObjectType = "Transporter",
                            Status = ( int ) Status.Active,
                            Type = ( int ) VehicleType.Pickup,
                        };

                        v = vservice.Create( v );
                    }

                    #endregion

                    #region Client Site 1

                    ClientSite cs1 = new ClientSite();
                    Site site = sservice.GetBySiteCode( load[ 0 ].Trim() );

                    if ( site == null )
                    {
                        site = new Site()
                        {
                            Name = load[ 1 ].Trim(),
                            Description = load[ 1 ].Trim(),
                            SiteCodeChep = load[ 0 ].Trim(),
                            Status = ( int ) Status.Active,
                        };

                        site = sservice.Create( site );
                    }

                    if ( !site.ClientSites.NullableAny() )
                    {
                        ClientCustomer cc = ccservice.GetByClient( model.ClientId );

                        if ( cc == null )
                        {
                            cc = new ClientCustomer()
                            {
                                ClientId = model.ClientId,
                                Status = ( int ) Status.Active,
                                CustomerName = load[ 6 ].Trim(),
                                CustomerNumber = load[ 0 ].Trim(),
                                CustomerTown = load[ 22 ].Trim(),
                            };

                            cc = ccservice.Create( cc );
                        }

                        cs1 = new ClientSite()
                        {
                            SiteId = site.Id,
                            ClientCustomerId = cc.Id,
                            Status = ( int ) Status.Active,
                            AccountingCode = load[ 0 ].Trim(),
                            ClientSiteCode = load[ 0 ].Trim(),
                        };

                        cs1 = csservice.Create( cs1 );
                    }
                    else
                    {
                        cs1 = site.ClientSites.FirstOrDefault();
                    }

                    #endregion

                    #region Client Site 2

                    ClientSite cs2 = new ClientSite();
                    Site site2 = sservice.GetBySiteCode( load[ 5 ].Trim() );

                    if ( site2 == null )
                    {
                        site2 = new Site()
                        {
                            Name = load[ 1 ].Trim(),
                            Description = load[ 1 ].Trim(),
                            SiteCodeChep = load[ 5 ].Trim(),
                            Status = ( int ) Status.Active,
                        };

                        site2 = sservice.Create( site2 );
                    }

                    if ( !site2.ClientSites.NullableAny() )
                    {
                        ClientCustomer cc = ccservice.GetByClient( model.ClientId );

                        if ( cc == null )
                        {
                            cc = new ClientCustomer()
                            {
                                ClientId = model.ClientId,
                                Status = ( int ) Status.Active,
                                CustomerName = load[ 6 ].Trim(),
                                CustomerNumber = load[ 5 ].Trim(),
                                CustomerTown = load[ 22 ].Trim(),
                            };

                            cc = ccservice.Create( cc );
                        }

                        cs2 = new ClientSite()
                        {
                            SiteId = site2.Id,
                            ClientCustomerId = cc.Id,
                            Status = ( int ) Status.Active,
                            AccountingCode = load[ 5 ].Trim(),
                            ClientSiteCode = load[ 5 ].Trim(),
                        };

                        cs2 = csservice.Create( cs2 );
                    }
                    else
                    {
                        cs2 = site2.ClientSites.FirstOrDefault();
                    }

                    #endregion

                    if ( l == null )
                    {
                        #region Create Client Load

                        cQuery = $" {cQuery} INSERT INTO [dbo].[ClientLoad]([ClientId],[ClientSiteId],[ToClientSiteId],[VehicleId],[TransporterId],[CreatedOn],[ModifiedOn],[ModifiedBy],[LoadNumber],[LoadDate],[EffectiveDate],[NotifyDate],[AccountNumber],[ClientDescription],[DeliveryNote],[ReferenceNumber],[ReceiverNumber],[OriginalQuantity],[NewQuantity],[PODNumber],[PCNNumber],[PRNNumber],[Status],[PostingType],[THAN],[ReturnQty],[PODStatus],[InvoiceStatus],[UID]) ";
                        cQuery = $" {cQuery} VALUES ({model.ClientId},{cs1.Id},{cs2.Id},{v.Id},{t.Id},'{DateTime.Now}','{DateTime.Now}','{CurrentUser.Email}','{load[ 2 ]}','{loadDate}','{deliveryDate}','{deliveryDate}','{load[ 5 ]}','{load[ 6 ]}','{load[ 12 ]}','{load[ 10 ]}','{load[ 12 ]}',{qty},{qty},'{pod}','{pcn}','{prn}',{( int ) status},{( int ) PostingType.Import},'{load[ 17 ]}',{returnQty},{podStatus},0,'{uid}') ";

                        #endregion

                        try
                        {
                            clservice.Query( cQuery );

                            created++;
                        }
                        catch ( Exception ex )
                        {
                            errors++;

                            errs.Add( ex.ToString() );
                        }
                    }
                    else
                    {
                        #region Update Client Load

                        uQuery = $@"{uQuery} UPDATE [dbo].[ClientLoad] SET
                                                    [ModifiedOn]='{DateTime.Now}',
                                                    [ModifiedBy]='{CurrentUser.Email}',
                                                    [VehicleId]={v.Id},
                                                    [ClientSiteId]={cs1.Id},
                                                    [ToClientSiteId]={cs2.Id},
                                                    [TransporterId]={t.Id},
                                                    [LoadNumber]='{load[ 2 ]}',
                                                    [LoadDate]='{loadDate}',
                                                    [EffectiveDate]='{deliveryDate}',
                                                    [NotifyDate]='{deliveryDate}',
                                                    [AccountNumber]='{load[ 5 ]}',
                                                    [ClientDescription]='{load[ 6 ]}',
                                                    [DeliveryNote]='{load[ 12 ]}',
                                                    [ReferenceNumber]='{load[ 10 ]}',
                                                    [ReceiverNumber]='{load[ 12 ]}',
                                                    [OriginalQuantity]={qty},
                                                    [NewQuantity]={qty},
                                                    [PODNumber]='{pod}',
                                                    [PCNNumber]='{pcn}',
                                                    [PRNNumber]='{prn}',
                                                    [THAN]='{load[ 17 ]}',
                                                    [ReturnQty]={returnQty},
                                                    [PODStatus]={podStatus},
                                                    [UID]={( int ) Status.Active}
                                                WHERE
                                                    [Id]={l.Id}";

                        #endregion

                        try
                        {
                            clservice.Query( uQuery );

                            updated++;
                        }
                        catch ( Exception ex )
                        {
                            errors++;

                            errs.Add( ex.ToString() );
                        }
                    }
                }

                cQuery = string.Empty;
                uQuery = string.Empty;

                if ( errs.NullableAny() )
                {
                    errorDocId = LogImportErrors( errs, model.ClientId );
                }
            }

            AutoReconcileLoads();

            string resp = $"{created} loads were successfully created, {updated} were updated, {skipped} were skipped and there were {errors} errors.";

            if ( errs.NullableAny() && errorDocId > 0 )
            {
                resp = $"{resp} <a href='/Pallet/ViewDocument/{errorDocId}' target='_blank'>Click here</a> to view the erros.";
            }

            Notify( resp, NotificationType.Success );

            return ClientData( new PagingModel(), new CustomSearchModel() );
        }

        // POST: Pallet/DeleteClientData/5
        [HttpPost]
        [Requires( PermissionTo.Delete )]
        public ActionResult DeleteClientData( ClientLoadViewModel model )
        {
            ClientLoad activeLoad;
            try
            {

                using ( ClientLoadService service = new ClientLoadService() )
                using ( TransactionScope scope = new TransactionScope() )
                {
                    activeLoad = service.GetById( model.Id );

                    if ( activeLoad == null )
                    {
                        Notify( "Sorry, the requested resource could not be found. Please try again", NotificationType.Error );

                        return PartialView( "_AccessDenied" );
                    }

                    activeLoad.Status = ( ( ( Status ) activeLoad.Status ) == Status.Active ) ? ( int ) Status.Inactive : ( int ) Status.Active;
                    service.Update( activeLoad );
                    scope.Complete();

                }
                Notify( "The selected item was successfully updated.", NotificationType.Success );
                return RedirectToAction( "ClientData" );
            }
            catch ( Exception ex )
            {
                ViewBag.Message = ex.Message;
                return View();
            }
        }

        #endregion

        //-------------------------------------------------------------------------------------

        #region Reconcile Loads



        //
        // GET: /Pallet/ChepLoads
        public ActionResult ChepLoads( PagingModel pm, CustomSearchModel csm )
        {
            using ( ChepLoadService chservice = new ChepLoadService() )
            {
                csm.ReconciliationStatus = ReconciliationStatus.Unreconciled;

                pm.Sort = "ASC";

                pm.SortBy = "cl.[ShipmentDate]";

                List<ChepLoadCustomModel> chModel = chservice.List1( pm, csm );
                int chTotal = ( chModel.Count < pm.Take && pm.Skip == 0 ) ? chModel.Count : chservice.Total1( pm, csm );

                PagingExtension paging = PagingExtension.Create( chModel, chTotal, pm.Skip, pm.Take, pm.Page );

                return PartialView( "_ChepLoads", paging );
            }
        }

        //
        // GET: /Pallet/ClientLoads
        public ActionResult ClientLoads( PagingModel pm, CustomSearchModel csm )
        {
            using ( ClientLoadService chservice = new ClientLoadService() )
            {
                csm.ReconciliationStatus = ReconciliationStatus.Unreconciled;

                pm.Sort = "ASC";

                pm.SortBy = "cl.[LoadDate]";

                List<ClientLoadCustomModel> model = chservice.List1( pm, csm );
                int total = ( model.Count < pm.Take && pm.Skip == 0 ) ? model.Count : chservice.Total1( pm, csm );

                PagingExtension paging = PagingExtension.Create( model, total, pm.Skip, pm.Take, pm.Page );

                return PartialView( "_ClientLoads", paging );
            }
        }

        //
        // GET: /Pallet/ReconcileLoads
        public ActionResult ReconcileLoad( int chid, int clid, string product )
        {
            using ( TransactionScope scope = new TransactionScope() )
            using ( ChepLoadService chservice = new ChepLoadService() )
            using ( ClientLoadService clservice = new ClientLoadService() )
            using ( ChepClientService ccservice = new ChepClientService() )
            {
                ChepLoad ch = chservice.GetById( chid );

                if ( ch == null )
                {
                    Notify( "Sorry, the specified Pooling Agent Load could not be found. Please try again", NotificationType.Error );

                    return PartialView( "_AccessDenied" );
                }

                ClientLoad cl = clservice.GetById( clid );

                if ( cl == null )
                {
                    Notify( "Sorry, the specified Client Load could not be found. Please try again", NotificationType.Error );

                    return PartialView( "_AccessDenied" );
                }

                ChepClient cc = new ChepClient()
                {
                    ChepLoadsId = ch.Id,
                    ClientLoadsId = cl.Id,
                    Status = ( int ) ReconciliationStatus.Reconciled,
                };

                ccservice.Create( cc );

                ch.BalanceStatus = ( int ) ReconciliationStatus.Reconciled;

                chservice.Update( ch );

                cl.Status = ( int ) ReconciliationStatus.Reconciled;
                cl.OrderStatus = ( int ) ReconciliationStatus.Reconciled;

                if ( string.IsNullOrWhiteSpace( cl.Equipment ) )
                {
                    cl.Equipment = product;
                }

                clservice.Update( cl );

                scope.Complete();

                Notify( "The selected loads were successfully reconcilled.", NotificationType.Success );
            }

            return PartialView( "_Notification" );
        }



        #region Chep Load Documents

        //
        // GET: /Pallet/ChepLoadDocuments
        public ActionResult ChepLoadDocuments( int id )
        {
            using ( DocumentService dservice = new DocumentService() )
            {
                List<Document> docs = dservice.List( id, "ChepLoad" );

                ViewBag.Id = id;

                return PartialView( "_ChepLoadDocuments", docs );
            }
        }

        //
        // GET: /Pallet/UpdateChepLoadDocuments
        [HttpPost]
        [Requires( PermissionTo.Create )]
        public ActionResult UpdateChepLoadDocuments( List<DocumentsViewModel> chepLoadDocuments, int ChepLoadId )
        {
            if ( chepLoadDocuments.NullableAny( f => f.File != null ) )
            {
                using ( DocumentService dservice = new DocumentService() )
                {
                    foreach ( DocumentsViewModel item in chepLoadDocuments.Where( f => f.File != null ) )
                    {
                        string path = Server.MapPath( $"~/{VariableExtension.SystemRules.DocumentsLocation}/ChepLoads/{ChepLoadId}/" );

                        if ( !Directory.Exists( path ) )
                        {
                            Directory.CreateDirectory( path );
                        }

                        string now = DateTime.Now.ToString( "yyyyMMddHHmmss" );

                        Document d = new Document()
                        {
                            ObjectId = ChepLoadId,
                            Category = "ChepLoad",
                            ObjectType = "ChepLoad",
                            Description = "Chep Load Document",
                            Name = item.File.FileName,
                            Title = item.File.FileName,
                            Size = item.File.ContentLength,
                            Status = ( int ) Status.Active,
                            Type = Path.GetExtension( item.File.FileName ),
                            Location = $"ChepLoads/{ChepLoadId}/{now}-{item.File.FileName}"
                        };

                        dservice.Create( d );

                        string fullpath = Path.Combine( path, $"{now}-{item.File.FileName}" );

                        item.File.SaveAs( fullpath );
                    }
                }
            }

            Notify( "The Chep Load Documents were successfully updated...", NotificationType.Success );

            return ChepLoadDocuments( ChepLoadId );
        }

        //
        // GET: /Pallet/DeleteChepDocument
        [HttpPost]
        [Requires( PermissionTo.Delete )]
        public ActionResult DeleteChepLoadDocument( int id )
        {
            int ChepLoadId;

            using ( DocumentService dservice = new DocumentService() )
            {
                Document doc = dservice.GetById( id );

                if ( doc == null )
                {
                    Notify( "Sorry, the specified Chep Load Document could not be found. Please try again", NotificationType.Error );

                    return PartialView( "_AccessDenied" );
                }

                ChepLoadId = doc.ObjectId.Value;

                string path = Server.MapPath( string.Format( "{0}/{1}", VariableExtension.SystemRules.DocumentsLocation, doc.Location ) );
                string folder = Path.GetDirectoryName( path );

                if ( System.IO.File.Exists( path ) )
                {
                    @System.IO.File.Delete( path );
                }

                if ( Directory.Exists( folder ) && Directory.GetFiles( folder )?.Length <= 0 )
                {
                    Directory.Delete( folder );
                }

                dservice.Delete( doc );

                Notify( "The selected Chep Load Document was successfully deleted.", NotificationType.Success );
            }

            return ChepLoadDocuments( ChepLoadId );
        }

        #endregion



        #region Chep Load Comments

        //
        // GET: /Pallet/ChepLoadComments
        public ActionResult ChepLoadComments( int id )
        {
            using ( CommentService dservice = new CommentService() )
            {
                List<Comment> comments = dservice.List( id, "ChepLoad" );

                ViewBag.Id = id;

                return PartialView( "_ChepLoadComments", comments );
            }
        }

        //
        // GET: /Pallet/UpdateChepLoadComments
        [HttpPost]
        [Requires( PermissionTo.Create )]
        public ActionResult UpdateChepLoadComments( List<Comment> chepLoadComments, int chepLoadId )
        {
            if ( chepLoadComments.NullableAny( f => !string.IsNullOrEmpty( f.Details ) ) )
            {
                using ( CommentService cservice = new CommentService() )
                {
                    foreach ( Comment item in chepLoadComments.Where( f => !string.IsNullOrEmpty( f.Details ) ) )
                    {
                        Comment c = cservice.GetById( item.Id );

                        if ( c == null )
                        {
                            c = new Comment()
                            {
                                ObjectId = chepLoadId,
                                Details = item.Details,
                                ObjectType = "ChepLoad",
                                UserId = CurrentUser.Id,
                                Status = ( int ) Status.Active,
                            };

                            cservice.Create( c );
                        }
                        else
                        {
                            c.Details = item.Details;

                            cservice.Update( c );
                        }
                    }
                }
            }

            Notify( "The Chep Load Comments were successfully updated...", NotificationType.Success );

            return ChepLoadComments( chepLoadId );
        }

        //
        // GET: /Pallet/DeleteChepComment
        [HttpPost]
        [Requires( PermissionTo.Delete )]
        public ActionResult DeleteChepLoadComment( int id )
        {
            int chepLoadId;

            using ( CommentService cservice = new CommentService() )
            {
                Comment c = cservice.GetById( id );

                if ( c == null )
                {
                    Notify( "Sorry, the specified Chep Load Comment could not be found. Please try again", NotificationType.Error );

                    return PartialView( "_AccessDenied" );
                }

                chepLoadId = c.ObjectId;

                cservice.Delete( c );

                Notify( "The selected Chep Load Comment was successfully deleted.", NotificationType.Success );
            }

            return ChepLoadComments( chepLoadId );
        }

        #endregion



        #region Client Load Documents

        //
        // GET: /Pallet/ClientLoadDocuments
        public ActionResult ClientLoadDocuments( int id )
        {
            using ( DocumentService dservice = new DocumentService() )
            {
                List<Document> docs = dservice.List( id, "ClientLoad" );

                ViewBag.Id = id;

                return PartialView( "_ClientLoadDocuments", docs );
            }
        }

        //
        // GET: /Pallet/UpdateClientLoadDocuments
        [HttpPost]
        [Requires( PermissionTo.Create )]
        public ActionResult UpdateClientLoadDocuments( List<DocumentsViewModel> clientLoadDocuments, int clientLoadId )
        {
            if ( clientLoadDocuments.NullableAny( f => f.File != null ) )
            {
                using ( DocumentService dservice = new DocumentService() )
                {
                    foreach ( DocumentsViewModel item in clientLoadDocuments.Where( f => f.File != null ) )
                    {
                        string path = Server.MapPath( $"~/{VariableExtension.SystemRules.DocumentsLocation}/ClientLoads/{clientLoadId}/" );

                        if ( !Directory.Exists( path ) )
                        {
                            Directory.CreateDirectory( path );
                        }

                        string now = DateTime.Now.ToString( "yyyyMMddHHmmss" );

                        Document d = new Document()
                        {
                            ObjectId = clientLoadId,
                            Category = "ClientLoad",
                            ObjectType = "ClientLoad",
                            Description = "Client Load Document",
                            Name = item.File.FileName,
                            Title = item.File.FileName,
                            Size = item.File.ContentLength,
                            Status = ( int ) Status.Active,
                            Type = Path.GetExtension( item.File.FileName ),
                            Location = $"ClientLoads/{clientLoadId}/{now}-{item.File.FileName}"
                        };

                        dservice.Create( d );

                        string fullpath = Path.Combine( path, $"{now}-{item.File.FileName}" );

                        item.File.SaveAs( fullpath );
                    }
                }
            }

            Notify( "The Client Load Documents were successfully updated...", NotificationType.Success );

            return ClientLoadDocuments( clientLoadId );
        }

        //
        // GET: /Pallet/DeleteClientDocument
        [HttpPost]
        [Requires( PermissionTo.Delete )]
        public ActionResult DeleteClientLoadDocument( int id )
        {
            int clientLoadId;

            using ( DocumentService dservice = new DocumentService() )
            {
                Document doc = dservice.GetById( id );

                if ( doc == null )
                {
                    Notify( "Sorry, the specified Client Load Document could not be found. Please try again", NotificationType.Error );

                    return PartialView( "_AccessDenied" );
                }

                clientLoadId = doc.ObjectId.Value;

                string path = Server.MapPath( string.Format( "{0}/{1}", VariableExtension.SystemRules.DocumentsLocation, doc.Location ) );
                string folder = Path.GetDirectoryName( path );

                if ( System.IO.File.Exists( path ) )
                {
                    @System.IO.File.Delete( path );
                }

                if ( Directory.Exists( folder ) && Directory.GetFiles( folder )?.Length <= 0 )
                {
                    Directory.Delete( folder );
                }

                dservice.Delete( doc );

                Notify( "The selected Client Load Document was successfully deleted.", NotificationType.Success );
            }

            return ClientLoadDocuments( clientLoadId );
        }

        #endregion



        #region Client Load Comments

        //
        // GET: /Pallet/ClientLoadComments
        public ActionResult ClientLoadComments( int id )
        {
            using ( CommentService cservice = new CommentService() )
            {
                List<Comment> comments = cservice.List( id, "ClientLoad" );

                ViewBag.Id = id;

                return PartialView( "_ClientLoadComments", comments );
            }
        }

        //
        // GET: /Pallet/UpdateClientLoadComments
        [HttpPost]
        [Requires( PermissionTo.Create )]
        public ActionResult UpdateClientLoadComments( List<Comment> clientLoadComments, int clientLoadId )
        {
            if ( clientLoadComments.NullableAny( f => !string.IsNullOrEmpty( f.Details ) ) )
            {
                using ( CommentService cservice = new CommentService() )
                {
                    foreach ( Comment item in clientLoadComments.Where( f => !string.IsNullOrEmpty( f.Details ) ) )
                    {
                        Comment c = cservice.GetById( item.Id );

                        if ( c == null )
                        {
                            c = new Comment()
                            {
                                Details = item.Details,
                                ObjectId = clientLoadId,
                                UserId = CurrentUser.Id,
                                ObjectType = "ClientLoad",
                                Status = ( int ) Status.Active,
                            };

                            cservice.Create( c );
                        }
                        else
                        {
                            c.Details = item.Details;

                            cservice.Update( c );
                        }
                    }
                }
            }

            Notify( "The Client Load Comments were successfully updated...", NotificationType.Success );

            return ClientLoadComments( clientLoadId );
        }

        //
        // GET: /Pallet/DeleteClientComment
        [HttpPost]
        [Requires( PermissionTo.Delete )]
        public ActionResult DeleteClientLoadComment( int id )
        {
            int clientLoadId;

            using ( CommentService cservice = new CommentService() )
            {
                Comment c = cservice.GetById( id );

                if ( c == null )
                {
                    Notify( "Sorry, the specified Client Load Comment could not be found. Please try again", NotificationType.Error );

                    return PartialView( "_AccessDenied" );
                }

                clientLoadId = c.ObjectId;

                cservice.Delete( c );

                Notify( "The selected Client Load Comment was successfully deleted.", NotificationType.Success );
            }

            return ClientLoadComments( clientLoadId );
        }

        #endregion



        #region Client Load Journals

        //
        // GET: /Pallet/ClientLoadJournals
        public ActionResult ClientLoadJournals( int id )
        {
            using ( JournalService jservice = new JournalService() )
            using ( DocumentService dservice = new DocumentService() )
            {
                List<Journal> journals = jservice.ListByClientLoad( id );

                ViewBag.Id = id;

                List<JournalViewModel> model = new List<JournalViewModel>();

                foreach ( Journal item in journals )
                {
                    List<Document> docs = dservice.List( item.Id, "Journal" );

                    model.Add( new JournalViewModel()
                    {
                        Id = item.Id,
                        EditMode = true,
                        THAN = item.THAN,
                        Documents = docs,
                        InOutInd = item.InOutInd,
                        ClientLoadId = item.ClientLoadId,
                        Status = ( Status ) item.Status,
                        PostingQuantity = item.PostingQuantity,
                        PostingDescription = item.PostingDescription,
                        JournalType = ( JournalType ) item.JournalType,
                    } );
                }

                return PartialView( "_ClientLoadJournals", model );
            }
        }

        //
        // GET: /Pallet/UpdateClientLoadJournals
        [HttpPost]
        [Requires( PermissionTo.Create )]
        public ActionResult UpdateClientLoadJournals( List<JournalViewModel> clientLoadJournals )
        {
            int clientLoadId = clientLoadJournals?.FirstOrDefault()?.ClientLoadId ?? 0;

            using ( JournalService jservice = new JournalService() )
            using ( TransactionScope scope = new TransactionScope() )
            using ( DocumentService dservice = new DocumentService() )
            {
                foreach ( JournalViewModel item in clientLoadJournals )
                {
                    Journal j = jservice.GetById( item.Id );

                    if ( j == null )
                    {
                        #region Create Journal

                        j = new Journal()
                        {
                            THAN = item.THAN,
                            InOutInd = item.InOutInd,
                            ClientLoadId = clientLoadId,
                            Status = ( int ) Status.Active,
                            JournalType = ( int ) item.JournalType,
                            PostingQuantity = item.PostingQuantity,
                            PostingDescription = item.PostingDescription,
                        };

                        jservice.Create( j );

                        #endregion
                    }
                    else
                    {
                        #region Update Journal

                        j.THAN = item.THAN;
                        j.InOutInd = item.InOutInd;
                        j.Status = ( int ) Status.Active;
                        j.JournalType = ( int ) item.JournalType;
                        j.PostingQuantity = item.PostingQuantity;
                        j.PostingDescription = item.PostingDescription;

                        jservice.Update( j );

                        #endregion
                    }

                    #region Any Document Upload

                    if ( item.File != null )
                    {
                        string path = Server.MapPath( $"~/{VariableExtension.SystemRules.DocumentsLocation}/Journals/{j.Id}/" );

                        if ( !Directory.Exists( path ) )
                        {
                            Directory.CreateDirectory( path );
                        }

                        string now = DateTime.Now.ToString( "yyyyMMddHHmmss" );

                        Document d = dservice.Get( j.Id, "Journal", "Journal" );

                        if ( d != null )
                        {
                            try
                            {
                                string p = Server.MapPath( $"~/{VariableExtension.SystemRules.DocumentsLocation}/{d.Location}" );

                                if ( System.IO.File.Exists( p ) )
                                {
                                    System.IO.File.Delete( p );
                                }
                            }
                            catch ( Exception ex )
                            {

                            }

                            dservice.Delete( d );
                        }

                        d = new Document()
                        {
                            ObjectId = j.Id,
                            Category = "Journal",
                            ObjectType = "Journal",
                            Description = "Journal",
                            Name = item.File.FileName,
                            Title = item.File.FileName,
                            Size = item.File.ContentLength,
                            Status = ( int ) Status.Active,
                            Type = Path.GetExtension( item.File.FileName ),
                            Location = $"Journals/{j.Id}/{now}-{item.File.FileName}"
                        };

                        dservice.Create( d );

                        string fullpath = Path.Combine( path, $"{now}-{item.File.FileName}" );

                        item.File.SaveAs( fullpath );
                    }

                    #endregion
                }

                scope.Complete();

                Notify( "The Client Load Journals were successfully updated...", NotificationType.Success );
            }

            return ClientLoadJournals( clientLoadId );
        }

        //
        // GET: /Pallet/DeleteClientJournal
        [HttpPost]
        [Requires( PermissionTo.Delete )]
        public ActionResult DeleteClientLoadJournal( int id )
        {
            using ( JournalService jservice = new JournalService() )
            using ( TransactionScope scope = new TransactionScope() )
            using ( DocumentService dservice = new DocumentService() )
            {
                Journal j = jservice.GetById( id );

                if ( j == null )
                {
                    Notify( "Sorry, the specified Journal could not be found. Please try again", NotificationType.Error );

                    return PartialView( "_AccessDenied" );
                }

                jservice.Delete( j );

                List<Document> docs = dservice.List( id, "Journal" );

                if ( docs.NullableAny() )
                {
                    foreach ( Document doc in docs )
                    {
                        string path = Server.MapPath( string.Format( "{0}/{1}", VariableExtension.SystemRules.DocumentsLocation, doc.Location ) );
                        string folder = Path.GetDirectoryName( path );

                        if ( System.IO.File.Exists( path ) )
                        {
                            @System.IO.File.Delete( path );
                        }

                        if ( Directory.Exists( folder ) && Directory.GetFiles( folder )?.Length <= 0 )
                        {
                            Directory.Delete( folder );
                        }

                        dservice.Delete( doc );
                    }
                }

                scope.Complete();

                Notify( "The selected Client Load Journal was successfully deleted.", NotificationType.Success );
            }

            return ClientLoadJournals( id );
        }

        #endregion



        #endregion

        //-------------------------------------------------------------------------------------

        #region Reconcile Invoices

        // GET: Pallet/AddInvoice
        [Requires( PermissionTo.Create )]
        public ActionResult AddInvoice()
        {
            InvoiceViewModel model = new InvoiceViewModel() { EditMode = true };

            return View( model );
        }

        // POST: Pallet/AddInvoice
        [HttpPost]
        [Requires( PermissionTo.Create )]
        public ActionResult AddInvoice( InvoiceViewModel model )
        {
            if ( !ModelState.IsValid )
            {
                Notify( "Sorry, the item was not created. Please correct all errors and try again.", NotificationType.Error );

                return View( model );
            }

            using ( InvoiceService iservice = new InvoiceService() )
            using ( ClientLoadService clservice = new ClientLoadService() )
            using ( ClientInvoiceService ciservice = new ClientInvoiceService() )
            {
                #region Create Invoice

                Invoice i = new Invoice()
                {
                    Date = model.Date,
                    Number = model.Number,
                    Amount = model.Amount,
                    Quantity = model.Quantity,
                    LoadNumber = model.LoadNumber,
                    Status = ( int ) Status.Active,
                };

                i = iservice.Create( i );

                #endregion

                #region Client Load Invoice Recon?

                List<ClientLoad> clientLoads = clservice.ListByColumnWhere( "LoadNumber", i.LoadNumber );

                if ( clientLoads.NullableAny() )
                {
                    foreach ( ClientLoad item in clientLoads )
                    {
                        item.ReconcileInvoice = true;
                        item.ChepInvoiceNo = i.Number;
                        item.InvoiceStatus = ( int ) InvoiceStatus.Updated;

                        clservice.Update( item );

                        ClientInvoice ci = new ClientInvoice()
                        {
                            ClientLoadId = item.Id,
                            InvoiceId = i.Id,
                            Status = ( int ) Status.Active,
                        };

                        ciservice.Create( ci );
                    }
                }

                #endregion
            }

            Notify( "The invoice was successfully created.", NotificationType.Success );

            return ReconcileInvoices( new PagingModel(), new CustomSearchModel() );
        }

        // GET: Pallet/ImportInvoices
        [Requires( PermissionTo.Create )]
        public ActionResult ImportInvoices()
        {
            InvoiceViewModel model = new InvoiceViewModel() { EditMode = true };

            return View( model );
        }

        // POST: Pallet/ImportInvoices
        [HttpPost]
        [Requires( PermissionTo.Create )]
        public ActionResult ImportInvoices( InvoiceViewModel model )
        {
            if ( model.File == null )
            {
                Notify( "Please select a file to upload and try again.", NotificationType.Error );

                return View( model );
            }

            int count = 0,
                errors = 0,
                skipped = 0,
                created = 0,
                updated = 0;

            string cQuery, uQuery;

            using ( InvoiceService iservice = new InvoiceService() )
            using ( TextFieldParser parser = new TextFieldParser( model.File.InputStream ) )
            {
                parser.Delimiters = new string[] { "," };

                while ( true )
                {
                    string[] inv = parser.ReadFields();

                    if ( inv == null )
                    {
                        break;
                    }

                    cQuery = uQuery = string.Empty;

                    count++;

                    if ( inv.NullableCount() < 2 )
                    {
                        skipped++;

                        continue;
                    }

                    inv = inv.ToSQLSafe();

                    Invoice i = iservice.GetByInvoiceNumber( inv[ 0 ] );

                    if ( i == null )
                    {
                        #region Create Invoice

                        cQuery = $" {cQuery} INSERT INTO [dbo].[Invoice] ([CreatedOn],[ModfiedOn],[ModifiedBy],[Number],[Date],[Quantity],[Amount],[LoadNumber],[Status]) ";
                        cQuery = $" {cQuery} VALUES ('{DateTime.Now}','{DateTime.Now}','{CurrentUser.Email}','{inv[ 0 ]}','{inv[ 1 ]}',{inv[ 2 ]},{inv[ 3 ]},'{inv[ 4 ]}',{( int ) Status.Active}) ";

                        #endregion

                        try
                        {
                            iservice.Query( cQuery );

                            created++;
                        }
                        catch ( Exception ex )
                        {
                            errors++;
                        }
                    }
                    else
                    {
                        #region Update Invoice

                        uQuery = $@"{uQuery} UPDATE [dbo].[Invoice] SET
                                                    [ModifiedOn]='{DateTime.Now}',
                                                    [ModifiedBy]='{CurrentUser.Email}',
                                                    [Number]='{inv[ 0 ]}',
                                                    [Date]='{inv[ 1 ]}',
                                                    [Quantity]={inv[ 2 ]},
                                                    [Amount]={inv[ 3 ]},
                                                    [LoadNumber]='{inv[ 4 ]}',
                                                    [Status]={( int ) Status.Active}
                                                WHERE
                                                    [Id]={i.Id}";

                        #endregion

                        try
                        {
                            iservice.Query( uQuery );

                            updated++;
                        }
                        catch ( Exception ex )
                        {
                            errors++;
                        }
                    }
                }

                cQuery = string.Empty;
                uQuery = string.Empty;
            }

            AutoReconcileInvoices();

            Notify( $"{created} invoices were successfully created, {updated} were updated, {skipped} were skipped and there were {errors} errors.", NotificationType.Success );

            return ReconcileInvoices( new PagingModel(), new CustomSearchModel() );
        }

        //
        // GET: /Pallet/ReconcileInvoice
        public ActionResult ReconcileInvoice( int iid, int clid )
        {
            using ( TransactionScope scope = new TransactionScope() )
            using ( InvoiceService iservice = new InvoiceService() )
            using ( ClientLoadService clservice = new ClientLoadService() )
            using ( ClientInvoiceService ciservice = new ClientInvoiceService() )
            {
                Invoice i = iservice.GetById( iid );

                if ( i == null )
                {
                    Notify( "Sorry, the specified Invoice could not be found. Please try again", NotificationType.Error );

                    return PartialView( "_AccessDenied" );
                }

                ClientLoad cl = clservice.GetById( clid );

                if ( cl == null )
                {
                    Notify( "Sorry, the specified Client Load could not be found. Please try again", NotificationType.Error );

                    return PartialView( "_AccessDenied" );
                }

                ClientInvoice ci = new ClientInvoice()
                {
                    InvoiceId = i.Id,
                    ClientLoadId = clid,
                    Status = ( int ) Status.Active,
                };

                ciservice.Create( ci );

                cl.ReconcileInvoice = true;
                cl.ChepInvoiceNo = i.Number;
                cl.InvoiceStatus = ( int ) InvoiceStatus.Updated;

                clservice.Update( cl );

                scope.Complete();

                Notify( "The selected loads were successfully reconcilled.", NotificationType.Success );
            }

            return PartialView( "_Notification" );
        }



        #region Invoice Comments

        //
        // GET: /Pallet/InvoiceComments
        public ActionResult InvoiceComments( int id )
        {
            using ( CommentService cservice = new CommentService() )
            {
                List<Comment> comments = cservice.List( id, "Invoice" );

                ViewBag.Id = id;

                return PartialView( "_InvoiceComments", comments );
            }
        }

        //
        // GET: /Pallet/UpdateClientLoadComments
        [HttpPost]
        [Requires( PermissionTo.Create )]
        public ActionResult UpdateInvoiceComments( List<Comment> invoiceComments, int invoiceId )
        {
            if ( invoiceComments.NullableAny( f => !string.IsNullOrEmpty( f.Details ) ) )
            {
                using ( CommentService cservice = new CommentService() )
                {
                    foreach ( Comment item in invoiceComments.Where( f => !string.IsNullOrEmpty( f.Details ) ) )
                    {
                        Comment c = cservice.GetById( item.Id );

                        if ( c == null )
                        {
                            c = new Comment()
                            {
                                ObjectId = invoiceId,
                                ObjectType = "Invoice",
                                Details = item.Details,
                                Status = ( int ) Status.Active,
                            };

                            cservice.Create( c );
                        }
                        else
                        {
                            c.Details = item.Details;

                            cservice.Update( c );
                        }
                    }
                }
            }

            Notify( "The Invoice Comments were successfully updated...", NotificationType.Success );

            return InvoiceComments( invoiceId );
        }

        //
        // GET: /Pallet/DeleteInvoiceComment
        [HttpPost]
        [Requires( PermissionTo.Delete )]
        public ActionResult DeleteInvoiceComment( int id )
        {
            int invoiceId;

            using ( CommentService cservice = new CommentService() )
            {
                Comment c = cservice.GetById( id );

                if ( c == null )
                {
                    Notify( "Sorry, the specified Client Load Comment could not be found. Please try again", NotificationType.Error );

                    return PartialView( "_AccessDenied" );
                }

                invoiceId = c.ObjectId;

                cservice.Delete( c );

                Notify( "The selected Invoice Comment was successfully deleted.", NotificationType.Success );
            }

            return InvoiceComments( invoiceId );
        }

        #endregion



        #region Invoice Documents

        //
        // GET: /Pallet/InvoiceDocuments
        public ActionResult InvoiceDocuments( int id )
        {
            using ( DocumentService dservice = new DocumentService() )
            {
                List<Document> docs = dservice.List( id, "Invoice" );

                ViewBag.Id = id;

                return PartialView( "_InvoiceDocuments", docs );
            }
        }

        //
        // GET: /Pallet/UpdateInvoiceDocuments
        [HttpPost]
        [Requires( PermissionTo.Create )]
        public ActionResult UpdateInvoiceDocuments( List<DocumentsViewModel> invoiceDocuments, int invoiceId )
        {
            if ( invoiceDocuments.NullableAny( f => f.File != null ) )
            {
                using ( DocumentService dservice = new DocumentService() )
                {
                    foreach ( DocumentsViewModel item in invoiceDocuments.Where( f => f.File != null ) )
                    {
                        string path = Server.MapPath( $"~/{VariableExtension.SystemRules.DocumentsLocation}/Invoices/{invoiceId}/" );

                        if ( !Directory.Exists( path ) )
                        {
                            Directory.CreateDirectory( path );
                        }

                        string now = DateTime.Now.ToString( "yyyyMMddHHmmss" );

                        Document d = new Document()
                        {
                            ObjectId = invoiceId,
                            Category = "Invoice",
                            ObjectType = "Invoice",
                            Name = item.File.FileName,
                            Title = item.File.FileName,
                            Size = item.File.ContentLength,
                            Status = ( int ) Status.Active,
                            Description = "Invoice Document",
                            Type = Path.GetExtension( item.File.FileName ),
                            Location = $"Invoices/{invoiceId}/{now}-{item.File.FileName}"
                        };

                        dservice.Create( d );

                        string fullpath = Path.Combine( path, $"{now}-{item.File.FileName}" );

                        item.File.SaveAs( fullpath );
                    }
                }
            }

            Notify( "The Invoice Documents were successfully updated...", NotificationType.Success );

            return InvoiceDocuments( invoiceId );
        }

        //
        // GET: /Pallet/DeleteClientDocument
        [HttpPost]
        [Requires( PermissionTo.Delete )]
        public ActionResult DeleteInvoiceDocument( int id )
        {
            int invoiceId;

            using ( DocumentService dservice = new DocumentService() )
            {
                Document doc = dservice.GetById( id );

                if ( doc == null )
                {
                    Notify( "Sorry, the specified Invoice Document could not be found. Please try again", NotificationType.Error );

                    return PartialView( "_AccessDenied" );
                }

                invoiceId = doc.ObjectId.Value;

                string path = Server.MapPath( string.Format( "{0}/{1}", VariableExtension.SystemRules.DocumentsLocation, doc.Location ) );
                string folder = Path.GetDirectoryName( path );

                if ( System.IO.File.Exists( path ) )
                {
                    @System.IO.File.Delete( path );
                }

                if ( Directory.Exists( folder ) && Directory.GetFiles( folder )?.Length <= 0 )
                {
                    Directory.Delete( folder );
                }

                dservice.Delete( doc );

                Notify( "The selected Invoice Document was successfully deleted.", NotificationType.Success );
            }

            return InvoiceDocuments( invoiceId );
        }

        #endregion



        //
        // GET: /Pallet/Invoices
        public ActionResult Invoices( PagingModel pm, CustomSearchModel csm )
        {
            using ( InvoiceService iservice = new InvoiceService() )
            {
                pm.Sort = "ASC";

                pm.SortBy = "i.[Date]";

                List<InvoiceCustomModel> chModel = iservice.List1( pm, csm );
                int chTotal = ( chModel.Count < pm.Take && pm.Skip == 0 ) ? chModel.Count : iservice.Total1( pm, csm );

                PagingExtension paging = PagingExtension.Create( chModel, chTotal, pm.Skip, pm.Take, pm.Page );

                return PartialView( "_Invoices", paging );
            }
        }

        //
        // GET: /Pallet/InvoiceClientLoads
        public ActionResult InvoiceClientLoads( PagingModel pm, CustomSearchModel csm )
        {
            using ( ClientLoadService chservice = new ClientLoadService() )
            {
                csm.InvoiceStatus = InvoiceStatus.NA;

                pm.Sort = "ASC";

                pm.SortBy = "cl.[LoadDate]";

                List<ClientLoadCustomModel> model = chservice.List1( pm, csm );
                int total = ( model.Count < pm.Take && pm.Skip == 0 ) ? model.Count : chservice.Total1( pm, csm );

                PagingExtension paging = PagingExtension.Create( model, total, pm.Skip, pm.Take, pm.Page );

                return PartialView( "_InvoiceClientLoads", paging );
            }
        }

        #endregion

        //-------------------------------------------------------------------------------------

        #region Outstanding Pallets

        // GET: Pallet/EditOutstandingPallet/5
        [Requires( PermissionTo.Edit )]
        public ActionResult EditOutstandingPallet( int id )
        {
            using ( CommentService cservice = new CommentService() )
            using ( ChepLoadService chservice = new ChepLoadService() )
            {
                ChepLoad load = chservice.GetById( id );

                if ( load == null )
                {
                    Notify( "Sorry, the requested resource could not be found. Please try again", NotificationType.Error );

                    return PartialView( "_AccessDenied" );
                }

                ChepLoadViewModel model = new ChepLoadViewModel()
                {
                    Id = load.Id,
                    EditMode = true,
                    OutstandingReasonId = load.OutstandingReasonId,
                };

                model.ChepLoadComments = cservice.List( id, "ChepLoad" );

                return View( model );
            }
        }

        // POST: Pallet/EditOutstandingPallet/5
        [HttpPost]
        [Requires( PermissionTo.Edit )]
        public ActionResult EditOutstandingPallet( ChepLoadViewModel model )
        {
            //if ( !ModelState.IsValid )
            //{
            //    Notify( "Sorry, the selected item was not updated. Please correct all errors and try again.", NotificationType.Error );

            //    return View( model );
            //}

            using ( CommentService cservice = new CommentService() )
            using ( ChepLoadService chservice = new ChepLoadService() )
            {
                ChepLoad load = chservice.GetById( model.Id );

                if ( load == null )
                {
                    Notify( "Sorry, that item does not exist! Please specify a valid item Id and try again.", NotificationType.Error );

                    return View( model );
                }

                #region Update Chep Load

                load.OutstandingReasonId = model.OutstandingReasonId;

                chservice.Update( load );

                #endregion

                #region Comments

                if ( model.ChepLoadComments.NullableAny() )
                {
                    foreach ( Comment item in model.ChepLoadComments.Where( f => !string.IsNullOrEmpty( f.Details ) ) )
                    {
                        Comment c = cservice.GetById( item.Id );

                        if ( c == null )
                        {
                            c = new Comment()
                            {
                                ObjectId = model.Id,
                                Details = item.Details,
                                UserId = CurrentUser.Id,
                                ObjectType = "ChepLoad",
                                Status = ( int ) Status.Active,
                            };

                            cservice.Create( c );
                        }
                        else
                        {
                            c.Details = item.Details;

                            cservice.Update( c );
                        }
                    }
                }

                #endregion
            }

            Notify( "The selected Item details were successfully updated.", NotificationType.Success );

            return OutstandingPallets( new PagingModel(), new CustomSearchModel() );
        }

        #endregion

        //-------------------------------------------------------------------------------------

        #region Generate Delivery Note

        //
        // GET: /Pallet/DeliveryNoteDetails/5
        public ActionResult DeliveryNoteDetails( int id, bool layout = true )
        {
            using ( DeliveryNoteService cservice = new DeliveryNoteService() )
            {
                DeliveryNote model = cservice.GetById( id );

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

        //
        // GET: /Pallet/AddDeliveryNote/5 
        [Requires( PermissionTo.Create )]
        public ActionResult AddDeliveryNote()
        {
            DeliveryNoteViewModel model = new DeliveryNoteViewModel() { EditMode = true };

            return View( model );
        }

        //
        // POST: /Pallet/AddDeliveryNote/5
        [HttpPost]
        [Requires( PermissionTo.Create )]
        public ActionResult AddDeliveryNote( DeliveryNoteViewModel model )
        {
            if ( !ModelState.IsValid )
            {
                Notify( "Sorry, the Delivery Note was not created. Please correct all errors and try again.", NotificationType.Error );

                return View( model );
            }

            using ( TransactionScope scope = new TransactionScope() )
            using ( ClientLoadService clservice = new ClientLoadService() )
            using ( DeliveryNoteService dnservice = new DeliveryNoteService() )
            using ( DeliveryNoteLineService dnlservice = new DeliveryNoteLineService() )
            {
                string number = ( dnservice.Max( "InvoiceNumber" ) as string ) ?? "0";

                int.TryParse( number.Trim().Replace( "DN", "" ), out int n );

                model.InvoiceNumber = dnservice.FormatNumber( "DN", ( n + 1 ) );

                #region Create Delivery Note

                // Create Delivery Note
                DeliveryNote note = new DeliveryNote()
                {
                    ClientId = model.ClientId,
                    OrderDate = model.OrderDate,
                    Status = ( int ) Status.Active,
                    OrderNumber = model.OrderNumber,
                    ClientSiteId = model.ClientSiteId,
                    CustomerName = model.CustomerName,
                    EmailAddress = model.EmailAddress,
                    Reference306 = model.Reference306,
                    InvoiceNumber = model.InvoiceNumber,
                    ContactNumber = model.ContactNumber,
                    BililngPostalCode = model.BillingPostalCode,
                    CustomerPostalCode = model.CustomerPostalCode,
                    DeliveryPostalCode = model.DeliveryPostalCode,
                    BillingProvince = ( int ) model.BillingProvince,
                    CustomerProvince = ( int ) model.CustomerProvince,
                    DeliveryProvince = ( int ) model.DeliveryProvince,
                    BillingAddress = $"{model.BillingAddress1}~{model.BillingAddress2}~{model.BillingAddressTown}",
                    DeliveryAddress = $"{model.DeliveryAddress1}~{model.DeliveryAddress2}~{model.DeliveryAddressTown}",
                    CustomerAddress = $"{model.CustomerAddress1}~{model.CustomerAddress2}~{model.CustomerAddressTown}",
                };

                note = dnservice.Create( note );

                #endregion

                #region Create Delivery Note Lines

                if ( model.DeliveryNoteLines.NullableAny() )
                {
                    foreach ( DeliveryNoteLineViewModel l in model.DeliveryNoteLines )
                    {
                        DeliveryNoteLine line = new DeliveryNoteLine()
                        {
                            Product = l.Product,
                            Equipment = l.Product,
                            Returned = l.Returned,
                            Delivered = l.Delivered,
                            DeliveryNoteId = note.Id,
                            OrderQuantity = l.Ordered,
                            Status = ( int ) Status.Active,
                            ProductDescription = l.ProductDescription,
                        };

                        dnlservice.Create( line );
                    }
                }

                #endregion

                #region Create Client Load

                ClientLoad load = new ClientLoad()
                {
                    NotifyDate = DateTime.Now,
                    ClientId = model.ClientId,
                    LoadDate = model.OrderDate,
                    VehicleId = model.VehicleId,
                    EffectiveDate = DateTime.Now,
                    LoadNumber = model.OrderNumber,
                    ClientSiteId = model.ClientSiteId,
                    DeliveryNote = model.InvoiceNumber,
                    AccountNumber = model.Reference306,
                    ReceiverNumber = model.ContactNumber,
                    ReferenceNumber = model.InvoiceNumber,
                    ClientDescription = model.CustomerName,
                    Status = ( int ) ReconciliationStatus.Unreconciled,
                    ReturnQty = model.DeliveryNoteLines?.Sum( s => s.Returned ),
                    NewQuantity = model.DeliveryNoteLines?.Sum( s => s.Ordered ),
                    Equipment = model.DeliveryNoteLines?.FirstOrDefault()?.Product,
                    OriginalQuantity = model.DeliveryNoteLines?.Sum( s => s.Ordered ),
                };

                clservice.Create( load );

                #endregion

                // We're done here..

                scope.Complete();
            }

            Notify( "The Delivery Note was successfully created.", NotificationType.Success );

            return DeliveryNotes( new PagingModel(), new CustomSearchModel() );
        }

        //
        // GET: /Pallet/EditDeliveryNote/5
        [Requires( PermissionTo.Edit )]
        public ActionResult EditDeliveryNote( int id )
        {
            using ( ClientLoadService clservice = new ClientLoadService() )
            using ( DeliveryNoteService service = new DeliveryNoteService() )
            {
                DeliveryNote note = service.GetById( id );

                if ( note == null )
                {
                    Notify( "Sorry, the requested resource could not be found. Please try again", NotificationType.Error );

                    return PartialView( "_AccessDenied" );
                }

                ClientLoad load = clservice.GetByRefernce( note.InvoiceNumber );

                string[] bAddress = note.BillingAddress?.Split( '~' );
                string[] dAddress = note.DeliveryAddress?.Split( '~' );
                string[] cAddress = note.CustomerAddress?.Split( '~' );

                #region Delivery Note

                DeliveryNoteViewModel model = new DeliveryNoteViewModel()
                {
                    Id = note.Id,
                    EditMode = true,
                    ClientId = note.ClientId,
                    OrderDate = note.OrderDate,
                    OrderNumber = note.OrderNumber,
                    Status = ( Status ) note.Status,
                    VehicleId = load?.VehicleId ?? 0,
                    Reference306 = note.Reference306,
                    CustomerName = note.CustomerName,
                    EmailAddress = note.EmailAddress,
                    ClientSiteId = note.ClientSite.Id,
                    ContactNumber = note.ContactNumber,
                    InvoiceNumber = note.InvoiceNumber,
                    BillingPostalCode = note.BililngPostalCode,
                    CustomerPostalCode = note.CustomerPostalCode,
                    DeliveryPostalCode = note.DeliveryPostalCode,
                    BillingProvince = ( Province ) note.BillingProvince,
                    CustomerProvince = ( Province ) note.CustomerProvince,
                    DeliveryProvince = ( Province ) note.DeliveryProvince,

                    BillingAddress1 = bAddress.Length >= 1 ? bAddress[ 0 ] : string.Empty,
                    BillingAddress2 = bAddress.Length >= 2 ? bAddress[ 1 ] : string.Empty,
                    BillingAddressTown = bAddress.Length >= 3 ? bAddress[ 2 ] : string.Empty,

                    CustomerAddress1 = cAddress.Length >= 1 ? cAddress[ 0 ] : string.Empty,
                    CustomerAddress2 = cAddress.Length >= 2 ? cAddress[ 1 ] : string.Empty,
                    CustomerAddressTown = cAddress.Length >= 3 ? cAddress[ 2 ] : string.Empty,

                    DeliveryAddress1 = cAddress.Length >= 1 ? cAddress[ 0 ] : string.Empty,
                    DeliveryAddress2 = cAddress.Length >= 2 ? cAddress[ 1 ] : string.Empty,
                    DeliveryAddressTown = cAddress.Length >= 3 ? cAddress[ 2 ] : string.Empty,

                    DeliveryNoteLines = new List<DeliveryNoteLineViewModel>(),
                };

                #endregion

                #region Delivery Note Lines

                foreach ( DeliveryNoteLine line in note.DeliveryNoteLines )
                {
                    model.DeliveryNoteLines.Add( new DeliveryNoteLineViewModel()
                    {
                        Id = line.Id,
                        Product = line.Product,
                        Returned = ( int ) line.Returned,
                        Status = ( Status ) line.Status,
                        Delivered = ( int ) line.Delivered,
                        Ordered = ( int ) line.OrderQuantity,
                        DeliveryNoteId = line.DeliveryNoteId,
                        ProductDescription = line.ProductDescription,
                    } );
                }

                #endregion

                return View( model );
            }
        }

        //
        // POST: /Pallet/EditDeliveryNote/5
        [HttpPost]
        [Requires( PermissionTo.Edit )]
        public ActionResult EditDeliveryNote( DeliveryNoteViewModel model )
        {
            using ( TransactionScope scope = new TransactionScope() )
            using ( ClientLoadService clservice = new ClientLoadService() )
            using ( DeliveryNoteService dnservice = new DeliveryNoteService() )
            using ( DeliveryNoteLineService dnlservice = new DeliveryNoteLineService() )
            {
                DeliveryNote note = dnservice.GetById( model.Id );

                if ( note == null )
                {
                    Notify( "Sorry, that Delivery Note does not exist! Please specify a valid Delivery Note Id and try again.", NotificationType.Error );

                    return View( model );
                }

                #region Update Delivery Note

                // Update Delivery Note
                note.ClientId = model.ClientId;
                note.OrderDate = model.OrderDate;
                note.Status = ( int ) model.Status;
                note.OrderNumber = model.OrderNumber;
                note.ClientSiteId = model.ClientSiteId;
                note.CustomerName = model.CustomerName;
                note.EmailAddress = model.EmailAddress;
                note.Reference306 = model.Reference306;
                note.ContactNumber = model.ContactNumber;
                note.BililngPostalCode = model.BillingPostalCode;
                note.CustomerPostalCode = model.CustomerPostalCode;
                note.DeliveryPostalCode = model.DeliveryPostalCode;
                note.BillingProvince = ( int ) model.BillingProvince;
                note.CustomerProvince = ( int ) model.CustomerProvince;
                note.DeliveryProvince = ( int ) model.DeliveryProvince;
                note.BillingAddress = $"{model.BillingAddress1}~{model.BillingAddress2}~{model.BillingAddressTown}";
                note.DeliveryAddress = $"{model.DeliveryAddress1}~{model.DeliveryAddress2}~{model.DeliveryAddressTown}";
                note.CustomerAddress = $"{model.CustomerAddress1}~{model.CustomerAddress2}~{model.CustomerAddressTown}";

                note = dnservice.Update( note );

                #endregion

                #region Update Delivery Note Lines

                if ( model.DeliveryNoteLines.NullableAny() )
                {
                    foreach ( DeliveryNoteLineViewModel l in model.DeliveryNoteLines )
                    {
                        DeliveryNoteLine line = dnlservice.GetById( l.Id );

                        if ( line != null )
                        {
                            line.Product = l.Product;
                            line.Equipment = l.Product;
                            line.Returned = l.Returned;
                            line.Delivered = l.Delivered;
                            line.DeliveryNoteId = note.Id;
                            line.OrderQuantity = l.Ordered;
                            line.ProductDescription = l.ProductDescription;

                            dnlservice.Update( line );
                        }
                        else
                        {
                            line = new DeliveryNoteLine()
                            {
                                Product = l.Product,
                                Equipment = l.Product,
                                Returned = l.Returned,
                                Delivered = l.Delivered,
                                DeliveryNoteId = note.Id,
                                OrderQuantity = l.Ordered,
                                Status = ( int ) Status.Active,
                                ProductDescription = l.ProductDescription,
                            };

                            dnlservice.Create( line );
                        }
                    }
                }

                #endregion

                #region Update Client Load

                ClientLoad load = clservice.GetByRefernce( note.InvoiceNumber );

                if ( load != null )
                {
                    load.ClientId = model.ClientId;
                    load.LoadDate = model.OrderDate;
                    load.VehicleId = model.VehicleId;
                    load.LoadNumber = model.OrderNumber;
                    load.ClientSiteId = model.ClientSiteId;
                    load.AccountNumber = model.Reference306;
                    load.ReceiverNumber = model.ContactNumber;
                    load.ClientDescription = model.CustomerName;
                    load.Status = ( int ) ReconciliationStatus.Unreconciled;
                    load.ReturnQty = model.DeliveryNoteLines?.Sum( s => s.Returned );
                    load.NewQuantity = model.DeliveryNoteLines?.Sum( s => s.Ordered );
                    load.Equipment = model.DeliveryNoteLines?.FirstOrDefault()?.Product;
                    load.OriginalQuantity = model.DeliveryNoteLines?.Sum( s => s.Ordered );

                    clservice.Update( load );
                }
                else
                {
                    load = new ClientLoad()
                    {
                        NotifyDate = DateTime.Now,
                        ClientId = model.ClientId,
                        LoadDate = model.OrderDate,
                        VehicleId = model.VehicleId,
                        EffectiveDate = DateTime.Now,
                        LoadNumber = model.OrderNumber,
                        ClientSiteId = model.ClientSiteId,
                        DeliveryNote = model.InvoiceNumber,
                        AccountNumber = model.Reference306,
                        ReceiverNumber = model.ContactNumber,
                        ReferenceNumber = model.InvoiceNumber,
                        ClientDescription = model.CustomerName,
                        Status = ( int ) ReconciliationStatus.Unreconciled,
                        ReturnQty = model.DeliveryNoteLines?.Sum( s => s.Returned ),
                        NewQuantity = model.DeliveryNoteLines?.Sum( s => s.Ordered ),
                        Equipment = model.DeliveryNoteLines?.FirstOrDefault()?.Product,
                        OriginalQuantity = model.DeliveryNoteLines?.Sum( s => s.Ordered ),
                    };

                    clservice.Create( load );
                }

                #endregion

                scope.Complete();
            }

            Notify( "The selected Delivery Note details were successfully updated.", NotificationType.Success );

            return DeliveryNotes( new PagingModel(), new CustomSearchModel() );
        }

        //
        // POST: /Pallet/DeleteDeliveryNote/5
        [HttpPost]
        [Requires( PermissionTo.Delete )]
        public ActionResult DeleteDeliveryNote( DeliveryNoteViewModel model, PagingModel pm, CustomSearchModel csm )
        {
            using ( DeliveryNoteService service = new DeliveryNoteService() )
            {
                DeliveryNote note = service.GetById( model.Id );

                if ( note == null )
                {
                    Notify( "Sorry, the requested resource could not be found. Please try again", NotificationType.Error );

                    return PartialView( "_AccessDenied" );
                }

                note.Status = ( note.Status == ( int ) Status.Active ) ? ( int ) Status.Inactive : ( int ) Status.Active;

                service.Update( note );

                Notify( "The selected Delivery Note was successfully Updated.", NotificationType.Success );
            }

            return DeliveryNotes( pm, csm );
        }

        //
        // POST: /Pallet/DeleteDeliveryNoteLine/5
        [HttpPost]
        [Requires( PermissionTo.Delete )]
        public ActionResult DeleteDeliveryNoteLine( int id )
        {
            using ( DeliveryNoteLineService dnlservice = new DeliveryNoteLineService() )
            {
                DeliveryNoteLine line = dnlservice.GetById( id );

                if ( line == null )
                {
                    Notify( "Sorry, the requested resource could not be found. Please try again", NotificationType.Error );

                    return PartialView( "_AccessDenied" );
                }

                int lineId = line.Id;

                DeliveryNote note = line.DeliveryNote;

                dnlservice.Delete( line );

                Notify( "The selected Delivery Note Line was successfully Deleted.", NotificationType.Success );

                DeliveryNoteViewModel model = new DeliveryNoteViewModel()
                {
                    DeliveryNoteLines = new List<DeliveryNoteLineViewModel>()
                };

                foreach ( DeliveryNoteLine l in note.DeliveryNoteLines )
                {
                    if ( lineId == l.Id ) continue;

                    model.DeliveryNoteLines.Add( new DeliveryNoteLineViewModel()
                    {
                        Id = l.Id,
                        Product = l.Product,
                        Status = ( Status ) l.Status,
                        Returned = ( int ) l.Returned,
                        Delivered = ( int ) l.Delivered,
                        Ordered = ( int ) l.OrderQuantity,
                        DeliveryNoteId = l.DeliveryNoteId,
                        ProductDescription = l.ProductDescription,
                    } );
                }

                return PartialView( "_DeliveryNoteLines", model );
            }
        }

        //
        // POST: /Pallet/EmailDeliveryNote
        [HttpPost]
        public ActionResult EmailDeliveryNote( int id, string email, string subject )
        {
            if ( string.IsNullOrWhiteSpace( email ) || string.IsNullOrWhiteSpace( subject ) )
            {
                return PartialView( "_Fail" );
            }

            using ( DeliveryNoteService service = new DeliveryNoteService() )
            {
                DeliveryNote note = service.GetById( id );

                if ( note == null )
                    return PartialView( "_AccessDenied" );

                string body = RenderViewToString( ControllerContext, "~/Views/Shared/_DeliveryNotePdf.cshtml", note, true );

                MemoryStream stream = new MemoryStream();

                iTextSharp.text.Document document = new iTextSharp.text.Document( iTextSharp.text.PageSize.A4, 10, 10, 10, 10 );

                PdfWriter pWriter = PdfWriter.GetInstance( document, stream );

                document.Open();

                XMLWorkerHelper.GetInstance().ParseXHtml( pWriter, document, new StringReader( body ) );

                pWriter.CloseStream = false;
                document.Close();

                stream.Position = 0;

                List<string> receivers = new List<string>() { email };

                EmailModel e = new EmailModel
                {
                    Body = body,
                    Subject = subject,
                    Recipients = receivers,
                    From = CurrentUser.Email,
                };

                e.Attachments = new List<System.Net.Mail.Attachment>
                {
                    new System.Net.Mail.Attachment( stream, $"Delivery Note {note.InvoiceNumber} ({DateTime.Now.ToString( "yyyy_MM_dd_HH_mm_ss" )}).pdf" )
                };

                Mail.Send( e );
            }

            return PartialView( "_Success" );
        }

        //
        // POST: /Pallet/PrintDeliveryNote
        public ActionResult PrintDeliveryNote( int id )
        {
            using ( DeliveryNoteService service = new DeliveryNoteService() )
            {
                DeliveryNote note = service.GetById( id );

                if ( note == null )
                    return PartialView( "_AccessDenied" );

                string body = RenderViewToString( ControllerContext, "~/Views/Shared/_DeliveryNotePdf.cshtml", note, true );

                MemoryStream stream = new MemoryStream();

                using ( iTextSharp.text.Document document = new iTextSharp.text.Document( iTextSharp.text.PageSize.A4, 10, 10, 10, 10 ) )
                {
                    PdfWriter pWriter = PdfWriter.GetInstance( document, stream );

                    PdfAction action = new PdfAction( PdfAction.PRINTDIALOG );
                    pWriter.SetOpenAction( action );

                    document.Open();

                    XMLWorkerHelper.GetInstance().ParseXHtml( pWriter, document, new StringReader( body ) );
                }

                return File( stream.ToArray(), "application/pdf" );
            }
        }

        #endregion

        //-------------------------------------------------------------------------------------

        #region Disputes

        //
        // GET: /Pallet/DisputeDetails/5
        public ActionResult DisputeDetails( int id, bool layout = true )
        {
            using ( DisputeService service = new DisputeService() )
            {
                Dispute dispute = service.GetById( id );

                if ( dispute == null )
                {
                    Notify( "Sorry, the requested resource could not be found. Please try again", NotificationType.Error );

                    return RedirectToAction( "Index" );
                }

                if ( layout )
                {
                    ViewBag.IncludeLayout = true;
                }

                dispute.DaysLeft = ( ( DisputeStatus ) dispute.Status == DisputeStatus.Active ) ? ( dispute.DaysLeft - ( DateTime.Now - dispute.CreatedOn ).Days ) : ( dispute.DaysLeft - ( DateTime.Now - dispute.ResolvedOn )?.Days );

                return View( dispute );
            }
        }

        //
        // GET: /Pallet/AddDispute/5 
        [Requires( PermissionTo.Create )]
        public ActionResult AddDispute()
        {
            DisputeViewModel model = new DisputeViewModel() { EditMode = true };

            return View( model );
        }

        //
        // POST: /Pallet/AddDispute/5
        [HttpPost]
        [Requires( PermissionTo.Create )]
        public ActionResult AddDispute( DisputeViewModel model )
        {
            if ( !ModelState.IsValid )
            {
                Notify( "Sorry, the Dispute was not created. Please correct all errors and try again.", NotificationType.Error );

                return View( model );
            }

            using ( DisputeService dservice = new DisputeService() )
            using ( ProductService pservice = new ProductService() )
            using ( ChepLoadService clservice = new ChepLoadService() )
            {
                //if ( dservice.Exist( model.DocketNumber ) )
                //{
                //    // Dispute already exist!
                //    Notify( $"Sorry, an ACTIVE Dispute for the Docket Number \"{model.DocketNumber}\" already exists!", NotificationType.Error );

                //    return View( model );
                //}

                ChepLoad ch = null;
                Product p = pservice.GetByName( model.Equipment?.Trim() );

                if ( !model.ChepLoadId.HasValue )
                {
                    // Attempt to locate ChepLoad using the specified DocketNumber
                    ch = clservice.GetByDocketNumber( model.DocketNumber );

                    if ( ch == null )
                    {
                        ch = clservice.GetByDocketNumber( model.OriginalDocketNumber );
                    }

                    model.ChepLoadId = ( ch != null ) ? ch.Id : model.ChepLoadId;
                }

                model.DaysLeft = 0;

                model.ActionById = ( !model.ActionById.HasValue ) ? CurrentUser.Id : model.ActionById;
                model.ResolvedOn = ( model.Status != DisputeStatus.Active && !model.ResolvedOn.HasValue ) ? DateTime.Now : model.ResolvedOn;
                model.ResolvedById = ( model.Status != DisputeStatus.Active && !model.ResolvedById.HasValue ) ? CurrentUser.Id : model.ResolvedById;

                Dispute dispute = new Dispute()
                {
                    Imported = false,
                    ProductId = p?.Id,
                    Action = model.Action,
                    //Sender = model.Sender,
                    Product = model.Equipment,
                    ActionBy = model.ActionBy,
                    Declarer = model.ActionBy,
                    Quantity = model.Quantity,
                    //Receiver = model.Receiver,
                    DaysLeft = model.DaysLeft,
                    Location = model.Location,
                    ShipDate = model.ShipDate,
                    Equipment = model.Equipment,
                    TDNNumber = model.TDNNumber,
                    OtherParty = model.OtherParty,
                    Status = ( int ) model.Status,
                    ResolvedOn = model.ResolvedOn,
                    ChepLoadId = model.ChepLoadId,
                    DataSource = model.DataSource,
                    LocationId = model.LocationId,
                    Reference = model.DocketNumber,
                    ActionedById = model.ActionById,
                    DocketNumber = model.DocketNumber,
                    DisputeEmail = model.DisputeEmail,
                    ResolvedById = model.ResolvedById,
                    HasDocket = ( ch != null ) ? 1 : 0,
                    DelilveryDate = model.DeliveryDate,
                    EffectiveDate = model.EffectiveDate,
                    EquipmentCode = model.EquipmentCode,
                    DisputeComment = model.DisputeComment,
                    OtherReference = model.OtherReference,
                    TransactionType = model.TransactionType,
                    DisputeReasonId = model.DisputeReasonId,
                    OriginalDocketNumber = model.OriginalDocketNumber,
                    CorrectionRequestDate = model.CorrectionRequestDate,
                    CorrectionRequestNumber = model.CorrectionRequestNumber,
                };

                dservice.Create( dispute );

                Notify( "The Dispute was successfully created.", NotificationType.Success );

                return Disputes( new PagingModel(), new CustomSearchModel() );
            }
        }

        //
        // GET: /Pallet/EditDispute/5
        [Requires( PermissionTo.Edit )]
        public ActionResult EditDispute( int id )
        {
            using ( DisputeService service = new DisputeService() )
            {
                Dispute dispute = service.GetById( id );

                if ( dispute == null )
                {
                    Notify( "Sorry, the requested resource could not be found. Please try again", NotificationType.Error );

                    return PartialView( "_AccessDenied" );
                }

                DisputeViewModel model = new DisputeViewModel()
                {
                    Id = dispute.Id,
                    EditMode = true,
                    Action = dispute.Action,
                    //Sender = dispute.Sender,
                    ShipDate = dispute.ShipDate,
                    Location = dispute.Location,
                    ActionBy = dispute.ActionBy,
                    Declarer = dispute.ActionBy,
                    //Receiver = dispute.Receiver,
                    TDNNumber = dispute.TDNNumber,
                    Equipment = dispute.Equipment,
                    Reference = dispute.Reference,
                    OtherParty = dispute.OtherParty,
                    ResolvedOn = dispute.ResolvedOn,
                    ChepLoadId = dispute.ChepLoadId,
                    DataSource = dispute.DataSource,
                    LocationId = dispute.LocationId,
                    ActionById = dispute.ActionedById,
                    DocketNumber = dispute.DocketNumber,
                    Quantity = ( int ) dispute.Quantity,
                    ResolvedById = dispute.ResolvedById,
                    DisputeEmail = dispute.DisputeEmail,
                    DaysLeft = ( int ) dispute.DaysLeft,
                    DeliveryDate = dispute.DelilveryDate,
                    EffectiveDate = dispute.EffectiveDate,
                    EquipmentCode = dispute.EquipmentCode,
                    DisputeComment = dispute.DisputeComment,
                    OtherReference = dispute.OtherReference,
                    TransactionType = dispute.TransactionType,
                    DisputeReasonId = dispute.DisputeReasonId,
                    Status = ( DisputeStatus ) dispute.Status,
                    OriginalDocketNumber = dispute.OriginalDocketNumber,
                    CorrectionRequestDate = dispute.CorrectionRequestDate,
                    CorrectionRequestNumber = dispute.CorrectionRequestNumber,
                };

                model.DaysLeft = model.DaysLeft ?? 0;

                model.DaysLeft = ( model.Status == DisputeStatus.Active ) ? ( model.DaysLeft - ( DateTime.Now - dispute.CreatedOn ).Days ) : ( model.DaysLeft - ( DateTime.Now - dispute.ResolvedOn )?.Days );

                return View( model );
            }
        }

        //
        // POST: /Pallet/EditDispute/5
        [HttpPost]
        [Requires( PermissionTo.Edit )]
        public ActionResult EditDispute( DisputeViewModel model )
        {
            using ( DisputeService service = new DisputeService() )
            using ( ProductService pservice = new ProductService() )
            using ( ChepLoadService clservice = new ChepLoadService() )
            {
                if ( !ModelState.IsValid )
                {
                    Notify( "Sorry, the selected Dispute was not updated. Please correct all errors and try again.", NotificationType.Error );

                    return View( model );
                }

                Dispute dispute = service.GetById( model.Id );

                if ( dispute == null )
                {
                    Notify( "Sorry, that Dispute does not exist! Please specify a valid Dispute Id and try again.", NotificationType.Error );

                    return View( model );
                }

                //if ( !dispute.DocketNumber.Equals( model.DocketNumber ) && service.Exist( model.DocketNumber ) )
                //{
                //    // Dispute already exist!
                //    Notify( $"Sorry, an ACTIVE Dispute with the Docket Number \"{model.DocketNumber}\" already exists!", NotificationType.Error );

                //    return View( model );
                //}

                ChepLoad ch = null;
                Product p = pservice.GetByName( model.Equipment?.Trim() );

                if ( !model.ChepLoadId.HasValue )
                {
                    // Attempt to locate ChepLoad using the specified DocketNumber
                    ch = clservice.GetByDocketNumber( model.DocketNumber );

                    if ( ch == null )
                    {
                        ch = clservice.GetByDocketNumber( model.OriginalDocketNumber );
                    }

                    model.ChepLoadId = ( ch != null ) ? ch.Id : model.ChepLoadId;
                }

                model.ActionById = ( !model.ActionById.HasValue ) ? CurrentUser.Id : model.ActionById;
                model.ResolvedOn = ( model.Status != DisputeStatus.Active && !model.ResolvedOn.HasValue ) ? DateTime.Now : model.ResolvedOn;
                model.ResolvedById = ( model.Status != DisputeStatus.Active && !model.ResolvedById.HasValue ) ? CurrentUser.Id : model.ResolvedById;

                dispute.ProductId = p?.Id;
                dispute.Action = model.Action;
                //dispute.Sender = model.Sender;
                dispute.Product = model.Equipment;
                dispute.ActionBy = model.ActionBy;
                dispute.Declarer = model.ActionBy;
                dispute.Quantity = model.Quantity;
                //dispute.Receiver = model.Receiver;
                dispute.DaysLeft = model.DaysLeft;
                dispute.Location = model.Location;
                dispute.ShipDate = model.ShipDate;
                dispute.Equipment = model.Equipment;
                dispute.TDNNumber = model.TDNNumber;
                dispute.OtherParty = model.OtherParty;
                dispute.Status = ( int ) model.Status;
                dispute.ResolvedOn = model.ResolvedOn;
                dispute.ChepLoadId = model.ChepLoadId;
                dispute.DataSource = model.DataSource;
                dispute.LocationId = model.LocationId;
                dispute.Reference = model.DocketNumber;
                dispute.ActionedById = model.ActionById;
                dispute.DocketNumber = model.DocketNumber;
                dispute.DisputeEmail = model.DisputeEmail;
                dispute.ResolvedById = model.ResolvedById;
                dispute.DelilveryDate = model.DeliveryDate;
                dispute.HasDocket = ( ch != null ) ? 1 : 0;
                dispute.EquipmentCode = model.EquipmentCode;
                dispute.EffectiveDate = model.EffectiveDate;
                dispute.DisputeComment = model.DisputeComment;
                dispute.OtherReference = model.OtherReference;
                dispute.TransactionType = model.TransactionType;
                dispute.DisputeReasonId = model.DisputeReasonId;
                dispute.OriginalDocketNumber = model.OriginalDocketNumber;
                dispute.CorrectionRequestDate = model.CorrectionRequestDate;
                dispute.CorrectionRequestNumber = model.CorrectionRequestNumber;

                dispute.DaysLeft = ( model.Status == DisputeStatus.Active ) ? ( dispute.DaysLeft - ( DateTime.Now - dispute.CreatedOn ).Days ) : ( dispute.DaysLeft - ( DateTime.Now - dispute.ResolvedOn )?.Days );

                service.Update( dispute );

                Notify( "The selected Dispute's details were successfully updated.", NotificationType.Success );

                return Disputes( new PagingModel(), new CustomSearchModel() );
            }
        }

        //
        // POST: /Pallet/DeleteDispute/5
        [HttpPost]
        [Requires( PermissionTo.Delete )]
        public ActionResult DeleteDispute( DisputeViewModel model )
        {
            using ( DisputeService service = new DisputeService() )
            {
                Dispute dispute = service.GetById( model.Id );

                if ( dispute == null )
                {
                    Notify( "Sorry, the requested resource could not be found. Please try again", NotificationType.Error );

                    return PartialView( "_AccessDenied" );
                }

                dispute.Status = ( ( ( DisputeStatus ) dispute.Status ) == DisputeStatus.Active ) ? ( int ) DisputeStatus.Cancelled : ( int ) DisputeStatus.Active;

                if ( ( DisputeStatus ) dispute.Status == DisputeStatus.Cancelled )
                {
                    dispute.ResolvedOn = DateTime.Now;
                    dispute.ResolvedById = CurrentUser.Id;
                }
                else
                {
                    dispute.ResolvedOn = null;
                    dispute.ResolvedById = null;
                }

                service.Update( dispute );

                Notify( "The selected Dispute was successfully updated.", NotificationType.Success );

                return Disputes( new PagingModel(), new CustomSearchModel() );
            }
        }

        // GET: Client/ImportDispute
        [Requires( PermissionTo.Create )]
        public ActionResult ImportDisputes()
        {
            DisputeViewModel model = new DisputeViewModel() { EditMode = true };

            return View( model );
        }

        // POST: Client/ImportDispute
        [HttpPost]
        [Requires( PermissionTo.Create )]
        public ActionResult ImportDisputes( DisputeViewModel model )
        {
            if ( model.File == null )
            {
                Notify( "Please select a file to upload and try again.", NotificationType.Error );

                return View( model );
            }

            int line = 0,
                count = 0,
                errors = 0,
                skipped = 0,
                created = 0,
                updated = 0;

            string cQuery, uQuery;

            using ( DisputeService dservice = new DisputeService() )
            using ( ProductService pservice = new ProductService() )
            using ( ChepLoadService chservice = new ChepLoadService() )
            using ( TextFieldParser parser = new TextFieldParser( model.File.InputStream ) )
            {
                parser.Delimiters = new string[] { "," };

                while ( true )
                {
                    string[] load = parser.ReadFields();

                    if ( load == null )
                    {
                        break;
                    }

                    line++;

                    if ( line == 1 ) continue;

                    cQuery = uQuery = string.Empty;

                    count++;

                    if ( load.NullableCount() < 2 )
                    {
                        skipped++;

                        continue;
                    }

                    load = load.ToSQLSafe();

                    Dispute l = dservice.GetByDocketNumber( load[ 6 ] );

                    string shipmentDate1 = string.Empty,
                           deliveryDate1 = string.Empty,
                           effectiveDate1 = string.Empty,
                           correctionRequestDate1 = string.Empty;

                    #region Dates

                    if ( !DateTime.TryParse( load[ 10 ], out DateTime shipmentDate ) )
                    {
                        if ( !DateTime.TryParseExact( load[ 10 ], "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out shipmentDate ) )
                        {
                            shipmentDate1 = "NULL";
                        }
                        else
                        {
                            shipmentDate1 = $"'{shipmentDate}'";
                        }
                    }
                    else
                    {
                        shipmentDate1 = $"'{shipmentDate}'";
                    }

                    if ( !DateTime.TryParse( load[ 11 ], out DateTime deliveryDate ) )
                    {
                        if ( !DateTime.TryParseExact( load[ 11 ], "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out deliveryDate ) )
                        {
                            deliveryDate1 = "NULL";
                        }
                        else
                        {
                            deliveryDate1 = $"'{deliveryDate}'";
                        }
                    }
                    else
                    {
                        deliveryDate1 = $"'{deliveryDate}'";
                    }

                    if ( !DateTime.TryParse( load[ 9 ], out DateTime effectiveDate ) )
                    {
                        if ( !DateTime.TryParseExact( load[ 9 ], "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out effectiveDate ) )
                        {
                            effectiveDate1 = "NULL";
                        }
                        else
                        {
                            effectiveDate1 = $"'{effectiveDate}'";
                        }
                    }
                    else
                    {
                        effectiveDate1 = $"'{effectiveDate}'";
                    }

                    if ( !DateTime.TryParse( load[ 17 ], out DateTime correctionRequestDate ) )
                    {
                        if ( !DateTime.TryParseExact( load[ 17 ], "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out correctionRequestDate ) )
                        {
                            correctionRequestDate1 = "NULL";
                        }
                        else
                        {
                            correctionRequestDate1 = $"'{correctionRequestDate}'";
                        }
                    }
                    else
                    {
                        correctionRequestDate1 = $"'{correctionRequestDate}'";
                    }

                    #endregion

                    ChepLoad ch = null;
                    Product p = pservice.GetByName( load[ 13 ].Trim() );

                    if ( !model.ChepLoadId.HasValue )
                    {
                        // Attempt to locate ChepLoad using the specified DocketNumber
                        ch = chservice.GetByDocketNumber( load[ 6 ] );

                        if ( ch == null )
                        {
                            ch = chservice.GetByDocketNumber( load[ 5 ] );
                        }
                    }

                    int.TryParse( load[ 14 ].Trim(), out int qty );

                    int hasDocket = ch != null ? 1 : 0;

                    string chepLoadId = ch != null ? ch.Id + "" : "NULL";
                    string productId = p != null ? p.Id + "" : "NULL";

                    if ( l == null )
                    {
                        #region Create Dispute

                        cQuery = $" {cQuery} INSERT INTO [dbo].[Dispute] ([ChepLoadId],[ActionedById],[ProductId],[CreatedOn],[ModifiedOn],[ModifiedBy],[DocketNumber],[TDNNumber],[Reference],[EquipmentCode],[Equipment],[OtherParty],[Product],[Quantity],[ActionBy],[Imported],[Status],[LocationId],[Location],[Action],[OriginalDocketNumber],[OtherReference],[EffectiveDate],[ShipDate],[DelilveryDate],[DaysLeft],[CorrectionRequestNumber],[CorrectionRequestDate],[TransactionType],[DataSource],[HasDocket]) ";
                        cQuery = $" {cQuery} VALUES ({chepLoadId},{CurrentUser.Id},{productId},{DateTime.Now},'{DateTime.Now}','{CurrentUser.Email}','{load[ 6 ]}','{load[ 24 ]}','{load[ 7 ]}','{load[ 12 ]}','{load[ 13 ]}','{load[ 4 ]}','{load[ 13 ]}',{qty},'{load[ 16 ]}',1,{( int ) Status.Active},'{load[ 18 ]}','{load[ 19 ]}','{load[ 2 ]}','{load[ 5 ]}','{load[ 8 ]}',{effectiveDate1},{shipmentDate1},{deliveryDate1},{load[ 0 ]},'{load[ 15 ]}',{correctionRequestDate1},'{load[ 20 ]}','{load[ 22 ]}',{hasDocket}) ";

                        #endregion

                        try
                        {
                            dservice.Query( cQuery );

                            created++;
                        }
                        catch ( Exception ex )
                        {
                            errors++;
                        }
                    }
                    else
                    {
                        #region Update Dispute

                        uQuery = $@"{uQuery} UPDATE [dbo].[Dispute] SET
                                                    [ModifiedOn]='{DateTime.Now}',
                                                    [ModifiedBy]='{CurrentUser.Email}',
                                                    [ChepLoadId]={chepLoadId},
                                                    [ActionedById]={CurrentUser.Id},
                                                    [ProductId]={productId},
                                                    [DocketNumber]='{load[ 6 ]}',
                                                    [TDNNumber]='{load[ 24 ]}',
                                                    [Reference]='{load[ 7 ]}',
                                                    [EquipmentCode]='{load[ 12 ]}',
                                                    [Equipment]='{load[ 13 ]}',
                                                    [OtherParty]='{load[ 4 ]}',
                                                    [Product]='{load[ 14 ]}',
                                                    [Quantity]={qty},
                                                    [ActionBy]='{load[ 16 ]}',
                                                    [Imported]=1,
                                                    [LocationId]='{load[ 18 ]}',
                                                    [Location]='{load[ 19 ]}',
                                                    [Action]='{load[ 2 ]}',
                                                    [OriginalDocketNumber]='{load[ 5 ]}',
                                                    [OtherReference]='{load[ 8 ]}',
                                                    [ShipmentDate]={shipmentDate1},
                                                    [DeliveryDate]={deliveryDate1},
                                                    [EffectiveDate]={effectiveDate1},
                                                    [DaysLeft]='{load[ 0 ]}',
                                                    [CorrectionRequestNumber]='{load[ 15 ]}',
                                                    [CorrectionRequestDate]={correctionRequestDate1},
                                                    [TransactionType]='{load[ 20 ]}',
                                                    [DataSource]='{load[ 22 ]}',
                                                    [HasDocket]={hasDocket}
                                                WHERE
                                                    [Id]={l.Id}";

                        #endregion

                        try
                        {
                            dservice.Query( uQuery );

                            updated++;
                        }
                        catch ( Exception ex )
                        {
                            errors++;
                        }
                    }
                }

                cQuery = string.Empty;
                uQuery = string.Empty;
            }

            Notify( $"{created} Disputes were successfully created, {updated} were updated, {skipped} were skipped and there were {errors} errors.", NotificationType.Success );

            return Disputes( new PagingModel(), new CustomSearchModel() );
        }

        #endregion

        //-------------------------------------------------------------------------------------

        #region Authorisation Code

        //
        // GET: /Pallet/AuthorisationCodeDetails/5
        public ActionResult AuthorisationCodeDetails( int id, bool layout = true )
        {
            using ( ClientAuthorisationService caservice = new ClientAuthorisationService() )
            {
                ClientAuthorisation model = caservice.GetById( id );

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

        //
        // GET: /Pallet/AddAuthorisationCode/5 
        [Requires( PermissionTo.Create )]
        public ActionResult AddAuthorisationCode()
        {
            AuthorisationCodeViewModel model = new AuthorisationCodeViewModel() { EditMode = true };

            return View( model );
        }

        //
        // POST: /Pallet/AddAuthorisationCode/5
        [HttpPost]
        [Requires( PermissionTo.Create )]
        public ActionResult AddAuthorisationCode( AuthorisationCodeViewModel model )
        {
            if ( !ModelState.IsValid )
            {
                Notify( "Sorry, the Authorisation Code was not created. Please correct all errors and try again.", NotificationType.Error );

                return View( model );
            }

            using ( ClientAuthorisationService caservice = new ClientAuthorisationService() )
            using ( ChepLoadService clservice = new ChepLoadService() )
            {
                if ( caservice.ExistByDocketNumber( model.DocketNumber ) )
                {
                    // AuthorisationCode already exist!
                    Notify( $"Sorry, an Authorisation Code with the Docket Number \"{model.DocketNumber}\" already exists!", NotificationType.Error );

                    return View( model );
                }

                if ( caservice.ExistByLoadNumber( model.LoadNumber ) )
                {
                    // AuthorisationCode already exist!
                    Notify( $"Sorry, an Authorisation Code with the Load Number \"{model.LoadNumber}\" already exists!", NotificationType.Error );

                    return View( model );
                }

                string number = ( caservice.Max( "Code" ) as string ) ?? "0";

                int.TryParse( number.Trim().Replace( "AC", "" ), out int n );

                model.Code = caservice.FormatNumber( "AC", ( n + 1 ) );

                ClientAuthorisation code = new ClientAuthorisation()
                {
                    Code = model.Code,
                    LoadNumber = model.LoadNumber,
                    Status = ( int ) model.Status,
                    DocketNumber = model.DocketNumber,
                    ClientSiteId = model.ClientSiteId,
                    TransporterId = model.TransporterId,
                    AuthorisationDate = model.AuthorisationDate.Value,
                };

                caservice.Create( code );

                Notify( "The Authorisation Code was successfully created.", NotificationType.Success );

                return AuthorisationCodes( new PagingModel(), new CustomSearchModel() );
            }
        }

        //
        // GET: /Pallet/EditAuthorisationCode/5
        [Requires( PermissionTo.Edit )]
        public ActionResult EditAuthorisationCode( int id )
        {
            using ( ClientAuthorisationService caservice = new ClientAuthorisationService() )
            {
                ClientAuthorisation code = caservice.GetById( id );

                if ( code == null )
                {
                    Notify( "Sorry, the requested resource could not be found. Please try again", NotificationType.Error );

                    return PartialView( "_AccessDenied" );
                }

                AuthorisationCodeViewModel model = new AuthorisationCodeViewModel()
                {
                    Id = code.Id,
                    EditMode = true,
                    Code = code.Code,
                    LoadNumber = code.LoadNumber,
                    Status = ( Status ) code.Status,
                    ClientSiteId = code.ClientSiteId,
                    DocketNumber = code.DocketNumber,
                    TransporterId = code.TransporterId,
                    AuthorisationDate = code.AuthorisationDate,
                };

                return View( model );
            }
        }

        //
        // POST: /Pallet/EditAuthorisationCode/5
        [HttpPost]
        [Requires( PermissionTo.Edit )]
        public ActionResult EditAuthorisationCode( AuthorisationCodeViewModel model )
        {
            using ( ClientAuthorisationService service = new ClientAuthorisationService() )
            using ( ChepLoadService clservice = new ChepLoadService() )
            {
                if ( !ModelState.IsValid )
                {
                    Notify( "Sorry, the selected Authorisation Code was not updated. Please correct all errors and try again.", NotificationType.Error );

                    return View( model );
                }

                ClientAuthorisation code = service.GetById( model.Id );

                if ( code == null )
                {
                    Notify( "Sorry, that Authorisation Code does not exist! Please specify a valid AuthorisationCode Id and try again.", NotificationType.Error );

                    return View( model );
                }

                if ( !code.DocketNumber.Equals( model.DocketNumber ) && service.ExistByDocketNumber( model.DocketNumber ) )
                {
                    // Authorisation Code already exist!
                    Notify( $"Sorry, an Authorisation Code with the Docket Number \"{model.DocketNumber}\" already exists!", NotificationType.Error );

                    return View( model );
                }

                if ( !code.LoadNumber.Equals( model.LoadNumber ) && service.ExistByLoadNumber( model.LoadNumber ) )
                {
                    // Authorisation Code already exist!
                    Notify( $"Sorry, an Authorisation Code with the Load Number \"{model.LoadNumber}\" already exists!", NotificationType.Error );

                    return View( model );
                }

                code.LoadNumber = model.LoadNumber;
                code.Status = ( int ) model.Status;
                code.DocketNumber = model.DocketNumber;
                code.ClientSiteId = model.ClientSiteId;
                code.TransporterId = model.TransporterId;
                code.AuthorisationDate = model.AuthorisationDate.Value;

                service.Update( code );

                Notify( "The selected Authorisation Code's details were successfully updated.", NotificationType.Success );

                return AuthorisationCodes( new PagingModel(), new CustomSearchModel() );
            }
        }

        //
        // POST: /Pallet/DeleteAuthorisationCode/5
        [HttpPost]
        [Requires( PermissionTo.Delete )]
        public ActionResult DeleteAuthorisationCode( AuthorisationCodeViewModel model )
        {
            using ( ClientAuthorisationService service = new ClientAuthorisationService() )
            {
                ClientAuthorisation code = service.GetById( model.Id );

                if ( code == null )
                {
                    Notify( "Sorry, the requested resource could not be found. Please try again", NotificationType.Error );

                    return PartialView( "_AccessDenied" );
                }

                code.Status = ( ( ( Status ) code.Status ) == Status.Active ) ? ( int ) Status.Inactive : ( int ) Status.Active;

                service.Update( code );

                Notify( "The selected Authorisation Code was successfully updated.", NotificationType.Success );

                return AuthorisationCodes( new PagingModel(), new CustomSearchModel() );
            }
        }

        public ActionResult GenerateAuthorisationCode( int id )
        {
            using ( ChepLoadService chservice = new ChepLoadService() )
            using ( ClientLoadService clservice = new ClientLoadService() )
            using ( ClientAuthorisationService caservice = new ClientAuthorisationService() )
            {
                ChepLoad ch = chservice.GetById( id );

                if ( ch == null )
                {
                    Notify( "Sorry, the requested resource could not be found. Please try again", NotificationType.Error );

                    return PartialView( "_AccessDenied" );
                }

                List<ClientLoad> cl = clservice.ListByChepRefOtherRef( ch.Ref?.Trim(), ch.OtherRef?.Trim() );

                string number = ( caservice.Max( "Code" ) as string ) ?? "0";

                int.TryParse( number.Trim().Replace( "AC", "" ), out int n );

                string code = caservice.FormatNumber( "AC", ( n + 1 ) );

                ClientAuthorisation ca = new ClientAuthorisation()
                {
                    Code = code,
                    ChepLoadId = ch.Id,
                    Status = ( int ) Status.Active,
                    DocketNumber = ch.DocketNumber,
                    AuthorisationDate = DateTime.Now,
                    LoadNumber = cl?.FirstOrDefault()?.LoadNumber,
                    ClientSiteId = cl?.FirstOrDefault()?.ClientSiteId,
                    TransporterId = cl?.FirstOrDefault()?.TransporterId,
                };

                caservice.Create( ca );

                Notify( "The Authorisation Code was successfully created for the selected Chep Load.", NotificationType.Success );

                return AuthorisationCodes( new PagingModel(), new CustomSearchModel() );
            }
        }

        #endregion

        //-------------------------------------------------------------------------------------


        #region Partial Views

        //
        // GET: /Pallet/PoolingAgentData
        public ActionResult PoolingAgentData( PagingModel pm, CustomSearchModel csm, bool givecsm = false )
        {
            using ( ChepLoadService service = new ChepLoadService() )
            {
                if ( givecsm )
                {
                    ViewBag.ViewName = "PoolingAgentData";

                    return PartialView( "_PoolingAgentDataCustomSearch", new CustomSearchModel( "PoolingAgentData" ) );
                }

                List<ChepLoadCustomModel> model = service.List1( pm, csm );
                int total = ( model.Count < pm.Take && pm.Skip == 0 ) ? model.Count : service.Total1( pm, csm );

                PagingExtension paging = PagingExtension.Create( model, total, pm.Skip, pm.Take, pm.Page );

                return PartialView( "_PoolingAgentData", paging );
            }
        }

        //
        // GET: /Pallet/ClientData
        public ActionResult ClientData( PagingModel pm, CustomSearchModel csm, bool givecsm = false )
        {
            if ( givecsm )
            {
                ViewBag.ViewName = "ClientData";

                return PartialView( "_ClientDataCustomSearch", new CustomSearchModel( "ClientData" ) );
            }

            using ( ClientLoadService service = new ClientLoadService() )
            {
                List<ClientLoadCustomModel> model = service.List1( pm, csm );
                int total = ( model.Count < pm.Take && pm.Skip == 0 ) ? model.Count : service.Total1( pm, csm );

                PagingExtension paging = PagingExtension.Create( model, total, pm.Skip, pm.Take, pm.Page );

                return PartialView( "_ClientData", paging );
            }
        }

        //
        // GET: /Pallet/ReconcileLoads
        public ActionResult ReconcileLoads( PagingModel pm, CustomSearchModel csm )
        {
            using ( ChepLoadService chservice = new ChepLoadService() )
            using ( ClientLoadService clservice = new ClientLoadService() )
            {
                csm.ReconciliationStatus = ReconciliationStatus.Unreconciled;

                pm.Sort = "ASC";

                pm.SortBy = "cl.[ShipmentDate]";

                List<ChepLoadCustomModel> chModel = chservice.List1( pm, csm );
                int chTotal = ( chModel.Count < pm.Take && pm.Skip == 0 ) ? chModel.Count : chservice.Total1( pm, csm );

                ViewBag.ChepLoads = PagingExtension.Create( chModel, chTotal, pm.Skip, pm.Take, pm.Page );

                pm.SortBy = "cl.[LoadDate]";

                List<ClientLoadCustomModel> clModel = clservice.List1( pm, csm );
                int clTotal = ( clModel.Count < pm.Take && pm.Skip == 0 ) ? clModel.Count : clservice.Total1( pm, csm );

                ViewBag.ClientLoads = PagingExtension.Create( clModel, clTotal, pm.Skip, pm.Take, pm.Page );
            }

            return PartialView( "_ReconcileLoads", new CustomSearchModel( "ReconcileLoads" ) );
        }

        //
        // GET: /Pallet/ReconcileInvoice
        public ActionResult ReconcileInvoices( PagingModel pm, CustomSearchModel csm )
        {
            using ( InvoiceService iservice = new InvoiceService() )
            using ( ClientLoadService clservice = new ClientLoadService() )
            {
                csm.InvoiceStatus = InvoiceStatus.NA;

                pm.Sort = "ASC";

                pm.SortBy = "i.[Date]";

                List<InvoiceCustomModel> iModel = iservice.List1( pm, csm );
                int iTotal = ( iModel.Count < pm.Take && pm.Skip == 0 ) ? iModel.Count : iservice.Total1( pm, csm );

                ViewBag.Invoices = PagingExtension.Create( iModel, iTotal, pm.Skip, pm.Take, pm.Page );

                pm.SortBy = "cl.[LoadDate]";

                List<ClientLoadCustomModel> clModel = clservice.List1( pm, csm );
                int clTotal = ( clModel.Count < pm.Take && pm.Skip == 0 ) ? clModel.Count : clservice.Total1( pm, csm );

                ViewBag.InvoiceClientLoads = PagingExtension.Create( clModel, clTotal, pm.Skip, pm.Take, pm.Page );
            }

            return PartialView( "_ReconcileInvoices", new CustomSearchModel( "ReconcileInvoice" ) );
        }

        //
        // GET: /Pallet/OutstandingPallets
        public ActionResult OutstandingPallets( PagingModel pm, CustomSearchModel csm, bool givecsm = false )
        {
            using ( ChepLoadService service = new ChepLoadService() )
            {
                if ( givecsm )
                {
                    ViewBag.ViewName = "OutstandingPallets";

                    return PartialView( "_OutstandingPallets1CustomSearch", new CustomSearchModel( "OutstandingPallets" ) );
                }

                csm.IsOP = true;
                csm.BalanceStatus = BalanceStatus.NotBalanced;
                csm.ReconciliationStatus = ReconciliationStatus.Reconciled;

                List<ChepLoadCustomModel> model = service.List1( pm, csm );
                int total = ( model.Count < pm.Take && pm.Skip == 0 ) ? model.Count : service.Total1( pm, csm );

                PagingExtension paging = PagingExtension.Create( model, total, pm.Skip, pm.Take, pm.Page );

                return PartialView( "_OutstandingPallets", paging );
            }
        }

        //
        // GET: /Pallet/Exceptions
        public ActionResult Exceptions( PagingModel pm, CustomSearchModel csm, bool givecsm = false )
        {
            if ( givecsm )
            {
                ViewBag.ViewName = "Exceptions";

                return PartialView( "_ExceptionsCustomSearch", new CustomSearchModel( "Exceptions" ) );
            }

            int total = 0;

            List<Site> model = new List<Site>();
            PagingExtension paging = PagingExtension.Create( model, total, pm.Skip, pm.Take, pm.Page );

            return PartialView( "_Exceptions", paging );
        }

        //
        // POST || GET: /Pallet/DeliveryNotes
        public ActionResult DeliveryNotes( PagingModel pm, CustomSearchModel csm, bool givecsm = false )
        {
            using ( DeliveryNoteService service = new DeliveryNoteService() )
            {
                if ( givecsm )
                {
                    ViewBag.ViewName = "DeliveryNotes";

                    return PartialView( "_DeliveryNotesCustomSearch", new CustomSearchModel( "DeliveryNotes" ) );
                }

                List<DeliveryNoteCustomModel> model = service.List1( pm, csm );

                int total = ( model.Count < pm.Take && pm.Skip == 0 ) ? model.Count : service.Total1( pm, csm );

                PagingExtension paging = PagingExtension.Create( model, total, pm.Skip, pm.Take, pm.Page );

                return PartialView( "_DeliveryNotes", paging );
            }
        }

        //
        // GET: /Pallet/Disputes
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
        // GET: /Pallet/AuthorisationCodes
        public ActionResult AuthorisationCodes( PagingModel pm, CustomSearchModel csm, bool givecsm = false )
        {
            using ( ClientAuthorisationService service = new ClientAuthorisationService() )
            {
                if ( givecsm )
                {
                    ViewBag.ViewName = "AuthorisationCodes";

                    return PartialView( "_AuthorisationCodesCustomSearch", new CustomSearchModel( "AuthorisationCodes" ) );
                }

                List<ChepLoadCustomModel> model = service.List1( pm, csm );

                int total = ( model.Count < pm.Take && pm.Skip == 0 ) ? model.Count : service.Total1( pm, csm );

                PagingExtension paging = PagingExtension.Create( model, total, pm.Skip, pm.Take, pm.Page );

                return PartialView( "_AuthorisationCodes", paging );
            }
        }

        #endregion
    }
}
