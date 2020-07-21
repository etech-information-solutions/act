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
                                                    item.DisputeReason,
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



        #region Pallet PoolingAgentData
        //
        // GET: /Pallet/PoolingAgentData
        public ActionResult PoolingAgentData( PagingModel pm, CustomSearchModel csm, bool givecsm = false )
        {
            if ( givecsm )
            {
                ViewBag.ViewName = "PoolingAgentData";

                return PartialView( "_PoolingAgentDataCustomSearch", new CustomSearchModel( "PoolingAgentData" ) );
            }

            ViewBag.ViewName = "PoolingAgentData";
            //check if there are any viewbag messages from imports
            string msg = ( Session[ "ImportMessage" ] != null ? Session[ "ImportMessage" ].ToString() : null );
            if ( !string.IsNullOrEmpty( msg ) )
            {
                ViewBag.Message = msg;
                Notify( msg, NotificationType.Success );

                //clear the message for next time
                Session[ "ImportMessage" ] = null;
            }

            string sessClientId = ( Session[ "ClientId" ] != null ? Session[ "ClientId" ].ToString() : null );
            int clientId = ( !string.IsNullOrEmpty( sessClientId ) ? int.Parse( sessClientId ) : 0 );
            ViewBag.ContextualMode = ( clientId > 0 ? true : false ); //Whether a client is specific or not and the View can know about it
            //model.ContextualMode = (clientId > 0 ? true : false); //Whether a client is specific or not and the View can know about it
            List<ClientCustomModel> clientList = new List<ClientCustomModel>();
            //TODO
            using ( ClientService clientService = new ClientService() )
            {
                clientList = clientService.List1( new PagingModel(), new CustomSearchModel() { ClientId = clientId, Status = Status.Active } );
            }

            IEnumerable<SelectListItem> clientDDL = clientList.Select( c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.CompanyName

            } );
            ViewBag.ClientList = clientDDL;


            int total = 0;
            List<ChepLoadCustomModel> model = new List<ChepLoadCustomModel>();
            using ( ChepLoadService service = new ChepLoadService() )
            {
                if ( clientId > 0 )
                {
                    csm.ClientId = clientId;
                }

                model = service.ListCSM( pm, csm );
                total = ( model.Count < pm.Take && pm.Skip == 0 ) ? model.Count : service.Total();
            }
            PagingExtension paging = PagingExtension.Create( model, total, pm.Skip, pm.Take, pm.Page );

            return PartialView( "_PoolingAgentData", paging );
        }

        // GET: Pallet/AddPoolingAgentData
        [Requires( PermissionTo.Create )]
        public ActionResult AddPoolingAgentData()
        {
            int pspId = ( CurrentUser != null ? CurrentUser.PSPs.FirstOrDefault().Id : 0 );
            ChepLoadViewModel model = new ChepLoadViewModel();
            model.PostingType = 3;
            return View( model );
        }

        // POST: Pallet/AddPoolingAgentData
        [HttpPost]
        [Requires( PermissionTo.Create )]
        public ActionResult AddPoolingAgentData( ChepLoadViewModel model )
        {
            try
            {
                if ( !ModelState.IsValid )
                {
                    Notify( "Sorry, the item was not created. Please correct all errors and try again.", NotificationType.Error );

                    return View( model );
                }

                using ( ChepLoadService gService = new ChepLoadService() )
                using ( TransactionScope scope = new TransactionScope() )
                {
                    #region Create ClientLoadCustomModel
                    ChepLoad clientload = new ChepLoad()
                    {
                        LoadDate = model.LoadDate,
                        EffectiveDate = model.EffectiveDate,
                        NotifyDate = model.NotifyDate,
                        AccountNumber = model.AccountNumber,
                        ClientDescription = model.ClientDescription,
                        DeliveryNote = model.DeliveryNote,
                        ReferenceNumber = model.ReferenceNumber,
                        ReceiverNumber = model.ReceiverNumber,
                        Equipment = model.Equipment,
                        OriginalQuantity = model.OriginalQuantity,
                        NewQuantity = model.NewQuantity,
                        Status = ( int ) Status.Active,
                        CreatedOn = DateTime.Now,
                        ModfiedOn = DateTime.Now,
                        ModifiedBy = CurrentUser.Email
                    };
                    gService.Create( clientload );
                    #endregion

                    scope.Complete();
                }

                Notify( "The item was successfully created.", NotificationType.Success );
                return RedirectToAction( "AgentPoolingData" );
            }
            catch ( Exception ex )
            {
                ViewBag.Message = ex.Message;
                return View();
            }
        }


        // GET: Pallet/EditPoolingAgentData/5
        [Requires( PermissionTo.Edit )]
        public ActionResult EditPoolingAgentData( int id )
        {
            ChepLoad load;

            using ( ChepLoadService service = new ChepLoadService() )
            {
                load = service.GetById( id );


                if ( load == null )
                {
                    Notify( "Sorry, the requested resource could not be found. Please try again", NotificationType.Error );

                    return PartialView( "_AccessDenied" );
                }

                ChepLoadViewModel model = new ChepLoadViewModel()
                {
                    LoadDate = load.LoadDate,
                    EffectiveDate = load.EffectiveDate,
                    NotifyDate = load.NotifyDate,
                    AccountNumber = load.AccountNumber,
                    ClientDescription = load.ClientDescription,
                    DeliveryNote = load.DeliveryNote,
                    ReferenceNumber = load.ReferenceNumber,
                    ReceiverNumber = load.ReceiverNumber,
                    Equipment = load.Equipment,
                    OriginalQuantity = load.OriginalQuantity,
                    NewQuantity = load.NewQuantity,
                    Status = ( int ) load.Status,
                    //DocumentCount = (int)load.DocumentCount,
                    // DocsList = ""
                };
                return View( model );
            }
        }

        // POST: Pallet/EditPoolingAgentData/5
        [HttpPost]
        [Requires( PermissionTo.Edit )]
        public ActionResult EditPoolingAgentData( ChepLoadViewModel model, PagingModel pm, bool isstructure = false )
        {
            try
            {
                if ( !ModelState.IsValid )
                {
                    Notify( "Sorry, the selected Group was not updated. Please correct all errors and try again.", NotificationType.Error );

                    return View( model );
                }

                ChepLoad load;

                using ( ChepLoadService service = new ChepLoadService() )
                using ( TransactionScope scope = new TransactionScope() )
                {
                    load = service.GetById( model.Id );

                    #region Update Chep Load
                    load.LoadDate = model.LoadDate;
                    load.EffectiveDate = model.EffectiveDate;
                    load.NotifyDate = model.NotifyDate;
                    load.AccountNumber = model.AccountNumber;
                    load.ClientDescription = model.ClientDescription;
                    load.DeliveryNote = model.DeliveryNote;
                    load.ReferenceNumber = model.ReferenceNumber;
                    load.ReceiverNumber = model.ReceiverNumber;
                    load.Equipment = model.Equipment;
                    load.OriginalQuantity = model.OriginalQuantity;
                    load.NewQuantity = model.NewQuantity;
                    load.Status = ( int ) model.Status;
                    load.ModfiedOn = DateTime.Now;
                    load.ModifiedBy = CurrentUser.Email;
                    service.Update( load );

                    #endregion


                    scope.Complete();
                }

                Notify( "The selected Item details were successfully updated.", NotificationType.Success );

                return RedirectToAction( "ClientData" );
            }
            catch ( Exception ex )
            {
                ViewBag.Message = ex.Message;
                return View();
            }
        }

        // POST: Pallet/DeletePoolingAgentData/5
        [HttpPost]
        [Requires( PermissionTo.Delete )]
        public ActionResult DeletePoolingAgentData( ChepLoadViewModel model )
        {
            ChepLoad activeLoad;
            try
            {

                using ( ChepLoadService service = new ChepLoadService() )
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
                return RedirectToAction( "PoolingAgentData" );
            }
            catch ( Exception ex )
            {
                ViewBag.Message = ex.Message;
                return View();
            }
        }
        #endregion

        //-------------------------------------------------------------------------------------

        #region Pallet ClientData
        //
        // GET: /Pallet/ClientData
        public ActionResult ClientData( PagingModel pm, CustomSearchModel csm, bool givecsm = false )
        {
            if ( givecsm )
            {
                ViewBag.ViewName = "ClientData";

                return PartialView( "_ClientDataCustomSearch", new CustomSearchModel( "ClientData" ) );
            }
            ViewBag.ViewName = "ClientData";
            //check if there are any viewbag messages from imports
            string msg = ( Session[ "ImportMessage" ] != null ? Session[ "ImportMessage" ].ToString() : null );
            if ( !string.IsNullOrEmpty( msg ) )
            {
                ViewBag.Message = msg;
                Notify( msg, NotificationType.Success );

                //clear the message for next time
                Session[ "ImportMessage" ] = null;
            }
            string sessClientId = ( Session[ "ClientId" ] != null ? Session[ "ClientId" ].ToString() : null );
            int clientId = ( !string.IsNullOrEmpty( sessClientId ) ? int.Parse( sessClientId ) : 0 );
            ViewBag.ContextualMode = ( clientId > 0 ? true : false ); //Whether a client is specific or not and the View can know about it
            //model.ContextualMode = (clientId > 0 ? true : false); //Whether a client is specific or not and the View can know about it
            List<ClientCustomModel> clientList = new List<ClientCustomModel>();
            //TODO
            using ( ClientService clientService = new ClientService() )
            {
                clientList = clientService.List1( new PagingModel(), new CustomSearchModel() { ClientId = clientId, Status = Status.Active } );
            }

            IEnumerable<SelectListItem> clientDDL = clientList.Select( c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.CompanyName

            } );
            ViewBag.ClientList = clientDDL;

            int total = 0;
            List<ClientLoadCustomModel> model = new List<ClientLoadCustomModel>();
            using ( ClientLoadService service = new ClientLoadService() )
            {
                if ( clientId > 0 )
                {
                    csm.ClientId = clientId;
                }

                model = service.ListCSM( pm, csm );
                total = ( model.Count < pm.Take && pm.Skip == 0 ) ? model.Count : service.Total();
            }
            PagingExtension paging = PagingExtension.Create( model, total, pm.Skip, pm.Take, pm.Page );

            return PartialView( "_ClientData", paging );
        }

        // GET: Pallet/AddClientLoad
        [Requires( PermissionTo.Create )]
        public ActionResult AddClientData()
        {
            ClientLoadViewModel model = new ClientLoadViewModel();
            string sessClientId = ( Session[ "ClientId" ] != null ? Session[ "ClientId" ].ToString() : null );
            int clientId = ( !string.IsNullOrEmpty( sessClientId ) ? int.Parse( sessClientId ) : 0 );
            ViewBag.ContextualMode = ( clientId > 0 ? true : false ); //Whether a client is specific or not and the View can know about it

            int pspId = ( CurrentUser != null ? CurrentUser.PSPs.FirstOrDefault().Id : 0 );
            List<Transporter> transporterOptions = new List<Transporter>();
            using ( TransporterService rservice = new TransporterService() )
            {
                transporterOptions = rservice.List();
            }
            IEnumerable<SelectListItem> transporterOptionsDDL = transporterOptions.Select( c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.TradingName

            } );
            ViewBag.TransporterOptions = transporterOptionsDDL;
            return View( model );
        }

        // POST: Pallet/AddClientData
        [HttpPost]
        [Requires( PermissionTo.Create )]
        public ActionResult AddClientData( ClientLoadViewModel model )
        {
            try
            {
                if ( !ModelState.IsValid )
                {
                    Notify( "Sorry, the item was not created. Please correct all errors and try again.", NotificationType.Error );

                    return View( model );
                }
                string sessClientId = ( Session[ "ClientId" ] != null ? Session[ "ClientId" ].ToString() : null );
                int clientId = ( !string.IsNullOrEmpty( sessClientId ) ? int.Parse( sessClientId ) : 0 );
                ViewBag.ContextualMode = ( clientId > 0 ? true : false ); //Whether a client is specific or not and the View can know about it
                List<Transporter> transporterOptions = new List<Transporter>();
                using ( TransporterService rservice = new TransporterService() )
                {
                    transporterOptions = rservice.List();
                }
                IEnumerable<SelectListItem> transporterOptionsDDL = transporterOptions.Select( c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.TradingName

                } );
                ViewBag.TransporterOptions = transporterOptionsDDL;

                if ( clientId > 0 || model.ClientId > 0 )
                {
                    using ( ClientLoadService gService = new ClientLoadService() )
                    using ( DeliveryNoteService service = new DeliveryNoteService() )
                    using ( DeliveryNoteLineService lineservice = new DeliveryNoteLineService() )
                    using ( AddressService addservice = new AddressService() )
                    using ( ClientService clientservice = new ClientService() )
                    using ( TransactionScope scope = new TransactionScope() )
                    {
                        #region Create ClientLoadCustomModel
                        ClientLoad clientload = new ClientLoad()
                        {
                            ClientId = ( model.ClientId > 0 ? model.ClientId : clientId ),
                            VehicleId = model.VehicleId,
                            // TransporterId = model.TransporterId,
                            LoadDate = model.LoadDate,//DateTimeExtensions.formatImportDate(model.LoadDate),
                            LoadNumber = model.LoadNumber,
                            EffectiveDate = model.EffectiveDate,//DateTimeExtensions.formatImportDate(model.EffectiveDate),
                            NotifyDate = model.NotifyDate,//DateTimeExtensions.formatImportDate(model.NotifyeDate),
                            AccountNumber = model.AccountNumber,
                            ClientDescription = model.ClientDescription,
                            DeliveryNote = model.DeliveryNote,
                            ReferenceNumber = model.ReferenceNumber,
                            ReceiverNumber = model.ReceiverNumber,
                            Equipment = model.Equipment,
                            OriginalQuantity = model.OriginalQuantity,
                            NewQuantity = model.NewQuantity,
                            ReturnQty = model.ReturnQty,
                            OutstandingQty = ( model.OriginalQuantity - model.ReturnQty ), //double check this
                            //ReconcileDate = model.ReconcileDate,
                            //ReconcileInvoice = model.ReconcileInvoice,
                            PODNumber = model.PODNumber,
                            PCNNumber = model.PCNNumber,
                            PRNNumber = model.PRNNumber,
                            //ARPMComments = model.ARPMComments,
                            //ProvCode = model.ProvCode,
                            Status = ( int ) Status.Active,
                            CreatedOn = DateTime.Now,
                            ModifiedOn = DateTime.Now,
                            ModifiedBy = CurrentUser.Email
                        };
                        gService.Create( clientload );
                        #endregion

                        #region Create DeliveryNote for new ClientLoad
                        if ( clientload.Id > 0 )
                            try
                            {
                                DeliveryNote delnote = new DeliveryNote();

                                Client noteClient = clientservice.GetById( model.ClientId );
                                Address clientAddress = addservice.GetByColumnsWhere( "ObjectId", model.ClientId, "ObjectType", "Client" );
                                string customerAddress = clientAddress.Addressline1 + ' ' + clientAddress.Addressline2 + ' ' + clientAddress.Town + ' ' + clientAddress.PostalCode + ' ' + ( ( Province ) clientAddress.Province ).GetDisplayText();
                                //string billingAddress = model.BillingAddress + ' ' + model.BillingAddress2 + ' ' + model.BillingAddressTown + ' ' + model.BillingPostalCode + ' ' + ((Province)model.BillingProvince).GetDisplayText();
                                //string deliveryAddress = model.DeliveryAddress + ' ' + model.DeliveryAddress2 + ' ' + model.DeliveryAddressTown + ' ' + model.DeliveryPostalCode + ' ' + ((Province)model.DeliveryProvince).GetDisplayText();

                                if ( model.Id > 0 )
                                {
                                    try
                                    {
                                        delnote.ClientId = model.ClientId;
                                        //Create
                                        delnote.InvoiceNumber = model.DeliveryNote;
                                        delnote.CustomerName = noteClient.CompanyName;
                                        delnote.EmailAddress = noteClient.Email;
                                        delnote.OrderNumber = model.ReferenceNumber;
                                        delnote.OrderDate = model.LoadDate;
                                        //delnote.ContactNumber = model.ContactNumber;
                                        delnote.Reference306 = model.LoadNumber;
                                        delnote.Status = ( int ) Status.Active;

                                        delnote.CustomerAddress = customerAddress;
                                        delnote.CustomerPostalCode = clientAddress.PostalCode;
                                        delnote.CustomerProvince = clientAddress.Province;

                                        //dont have the below to create
                                        //delnote.DeliveryAddress = deliveryAddress;
                                        //delnote.DeliveryPostalCode = model.DeliveryPostalCode;
                                        //delnote.DeliveryProvince = model.DeliveryProvince;

                                        //delnote.BillingAddress = billingAddress;
                                        //delnote.BililngPostalCode = model.BillingPostalCode;
                                        //delnote.BillingProvince = model.BillingProvince;

                                        //Create Client Invoice Address
                                        service.Create( delnote );

                                        //Create Invoice Customer Address
                                        Address invoiceCustomerAddress = new Address()
                                        {
                                            ObjectId = delnote.Id,
                                            ObjectType = "InvoiceCustomer",
                                            Town = clientAddress.Town,
                                            Status = ( int ) Status.Active,
                                            PostalCode = clientAddress.PostalCode,
                                            Type = 1,
                                            Addressline1 = clientAddress.Addressline1,
                                            Addressline2 = clientAddress.Addressline2,
                                            Province = ( int ) clientAddress.Province,
                                            //Province = (int)mappedProvinceVal,
                                        };
                                        addservice.Create( invoiceCustomerAddress );


                                    }
                                    catch ( Exception ex )
                                    {
                                        ViewBag.Message = ex.Message;
                                        //  return View();
                                        return PartialView( "_GenerateDeliveryNote", model );
                                    }

                                }
                                #endregion
                                #region Save DeliveryNoteLine
                                try
                                {
                                    DeliveryNoteLine noteline = new DeliveryNoteLine();

                                    noteline.DeliveryNoteId = delnote.Id;//deliveryNoteId;
                                    noteline.Delivered = model.NewQuantity;
                                    noteline.Product = "Delivery Note";
                                    noteline.ProductDescription = "Delivery Note";
                                    noteline.OrderQuantity = model.OriginalQuantity;
                                    noteline.ModifiedOn = DateTime.Now;
                                    noteline.CreatedOn = DateTime.Now;
                                    noteline.Status = ( int ) Status.Active;

                                    lineservice.Create( noteline );
                                }
                                catch ( Exception ex )
                                {
                                    ViewBag.Message = ex.Message;
                                }

                                #endregion
                            }
                            catch ( Exception ex )
                            {
                                ViewBag.Message = ex.Message;
                            }

                        scope.Complete();
                    }

                    Notify( "The item was successfully created.", NotificationType.Success );
                    return RedirectToAction( "ClientData" );
                }
                else
                {
                    ViewBag.Message = "No Client Accounted for";
                    return View();
                }
            }
            catch ( Exception ex )
            {
                ViewBag.Message = ex.Message;
                return View();
            }
        }


        // GET: Pallet/EditClientData/5
        [Requires( PermissionTo.Edit )]
        public ActionResult EditClientData( int id )
        {
            ClientLoad load;

            using ( ClientLoadService service = new ClientLoadService() )
            {
                load = service.GetById( id );


                if ( load == null )
                {
                    Notify( "Sorry, the requested resource could not be found. Please try again", NotificationType.Error );

                    return PartialView( "_AccessDenied" );
                }

                ClientLoadViewModel model = new ClientLoadViewModel()
                {
                    ClientId = load.ClientId,
                    VehicleId = load.VehicleId,
                    //TransporterId = (int)load.TransporterId,
                    LoadDate = load.LoadDate,
                    LoadNumber = load.LoadNumber,
                    EffectiveDate = load.EffectiveDate,
                    NotifyDate = load.NotifyDate,
                    AccountNumber = load.AccountNumber,
                    ClientDescription = load.ClientDescription,
                    DeliveryNote = load.DeliveryNote,
                    ReferenceNumber = load.ReferenceNumber,
                    ReceiverNumber = load.ReceiverNumber,
                    Equipment = load.Equipment,
                    OriginalQuantity = load.OriginalQuantity,
                    NewQuantity = load.NewQuantity,
                    ReturnQty = load.ReturnQty,
                    ReconcileDate = load.ReconcileDate,
                    ReconcileInvoice = load.ReconcileInvoice,
                    PODNumber = load.PODNumber,
                    PCNNumber = load.PCNNumber,
                    PRNNumber = load.PRNNumber,
                    Status = ( int ) load.Status
                    // DocumentCount = (int)load.DocumentCount,
                    // DocsList = ""
                };
                List<Transporter> transporterOptions = new List<Transporter>();
                using ( TransporterService rservice = new TransporterService() )
                {
                    transporterOptions = rservice.List();
                }
                IEnumerable<SelectListItem> transporterOptionsDDL = transporterOptions.Select( c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.TradingName

                } );
                ViewBag.TransporterOptions = transporterOptionsDDL;
                List<Vehicle> vehicleOptions = new List<Vehicle>();
                using ( VehicleService vservice = new VehicleService() )
                {
                    vehicleOptions = vservice.ListByColumnsWhere( "ObjectId", model.ClientId, "Object", "Client" );
                }
                IEnumerable<SelectListItem> vehicleOptionsDDL = vehicleOptions.Select( c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Registration

                } );
                ViewBag.VehicleOptions = vehicleOptionsDDL;
                return View( "EditClientData", model );
            }
        }

        // POST: Pallet/EditClientData/5
        [HttpPost]
        [Requires( PermissionTo.Edit )]
        public ActionResult EditClientData( ClientLoadViewModel model, PagingModel pm, bool isstructure = false )
        {
            try
            {
                if ( !ModelState.IsValid )
                {
                    Notify( "Sorry, the selected Group was not updated. Please correct all errors and try again.", NotificationType.Error );

                    return View( model );
                }

                ClientLoad load;

                using ( ClientLoadService service = new ClientLoadService() )
                using ( TransactionScope scope = new TransactionScope() )
                {
                    load = service.GetById( model.Id );

                    #region Update Client Load

                    // Update Group
                    //group.Id = model.Id;
                    load.ClientId = model.ClientId;
                    load.VehicleId = model.VehicleId;
                    //   load.TransporterId = model.TransporterId;
                    load.LoadDate = model.LoadDate;// DateTimeExtensions.formatImportDate(model.LoadDate);
                    load.LoadNumber = model.LoadNumber;
                    load.EffectiveDate = model.EffectiveDate;// DateTimeExtensions.formatImportDate(model.EffectiveDate);
                    load.NotifyDate = model.NotifyDate;// DateTimeExtensions.formatImportDate(model.NotifyeDate);
                    load.AccountNumber = model.AccountNumber;
                    load.ClientDescription = model.ClientDescription;
                    load.DeliveryNote = model.DeliveryNote;
                    load.ReferenceNumber = model.ReferenceNumber;
                    load.ReceiverNumber = model.ReceiverNumber;
                    load.Equipment = model.Equipment;
                    load.OriginalQuantity = model.OriginalQuantity;
                    load.NewQuantity = model.NewQuantity;
                    load.ReturnQty = model.ReturnQty;
                    load.ReconcileDate = model.ReconcileDate;
                    load.ReconcileInvoice = model.ReconcileInvoice;
                    load.PODNumber = model.PODNumber;
                    load.PCNNumber = model.PCNNumber;
                    load.PRNNumber = model.PRNNumber;
                    //load.ARPMComments = model.ARPMComments;
                    //load.ProvCode = model.ProvCode;
                    load.Status = ( int ) model.Status;
                    load.ModifiedBy = CurrentUser.Email;
                    load.ModifiedOn = DateTime.Now;

                    service.Update( load );

                    #endregion


                    scope.Complete();
                }

                Notify( "The selected Item details were successfully updated.", NotificationType.Success );

                return RedirectToAction( "ClientData" );
            }
            catch ( Exception ex )
            {
                ViewBag.Message = ex.Message;
                return View();
            }
        }

        // POST: Pallet/DeleteClientData/5
        [HttpPost]
        [Requires( PermissionTo.Delete )]
        public ActionResult DeleteClientData( ClientLoadCustomModel model )
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

        #region Pallet ReconcileLoads
        //
        // GET: /Pallet/ReconcileLoads
        public ActionResult ReconcileLoads( PagingModel pm, CustomSearchModel csm, bool givecsm = false )
        {
            if ( givecsm )
            {
                ViewBag.ViewName = "ReconcileLoads";

                return PartialView( "_ReconcileLoadsCustomSearch", new CustomSearchModel( "ReconcileLoads" ) );
            }
            ViewBag.ViewName = "ReconcileLoads";
            string sessClientId = ( Session[ "ClientId" ] != null ? Session[ "ClientId" ].ToString() : null );
            int clientId = ( !string.IsNullOrEmpty( sessClientId ) ? int.Parse( sessClientId ) : 0 );
            ViewBag.ContextualMode = ( clientId > 0 ? true : false ); //Whether a client is specific or not and the View can know about it
            ViewBag.ClientId = clientId;
            List<ClientCustomModel> clientList = new List<ClientCustomModel>();
            using ( ClientService clientService = new ClientService() )
            {
                clientList = clientService.List1( new PagingModel(), new CustomSearchModel() { ClientId = clientId, Status = Status.Active } );
            }

            IEnumerable<SelectListItem> clientDDL = clientList.Select( c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.CompanyName

            } );
            ViewBag.ClientList = clientDDL;

            return PartialView( "_ReconcileLoads" );
        }

        [AcceptVerbs( HttpVerbs.Get | HttpVerbs.Post )]
        public JsonResult GetUnreconciledClientLoads( int? clientId )
        {
            string sessClientId = ( Session[ "ClientId" ] != null ? Session[ "ClientId" ].ToString() : null );
            clientId = ( int ) ( clientId != null ? ( int ) clientId : !string.IsNullOrEmpty( sessClientId ) ? int.Parse( sessClientId ) : 0 );

            List<ClientLoadCustomModel> load;
            //int pspId = Session[ "UserPSP" ];
            int pspId = ( CurrentUser != null ? CurrentUser.PSPs.FirstOrDefault().Id : 0 );


            using ( ClientLoadService clientService = new ClientLoadService() )
            {

                load = clientService.ListCSM( new PagingModel(), new CustomSearchModel() { ClientId = ( int ) clientId, Status = ( int ) ReconciliationStatus.Unreconciled, ReconciliationStatus = ( int ) ReconciliationStatus.Unreconciled } ); //(pspId, int.Parse(groupId), new CustomSearchModel() { ClientId = clientId, Status = Status.Active });

            }
            if ( load != null )
            {
                return Json( load, JsonRequestBehavior.AllowGet );
            }
            else
            {
                return Json( data: "Error", behavior: JsonRequestBehavior.AllowGet );
            }
        }

        [AcceptVerbs( HttpVerbs.Get | HttpVerbs.Post )]
        public JsonResult GetUnreconciledAgentLoads( int? clientId )
        {

            List<ChepLoadCustomModel> load;
            //int pspId = Session[ "UserPSP" ];
            int pspId = ( CurrentUser != null ? CurrentUser.PSPs.FirstOrDefault().Id : 0 );
            string sessClientId = ( Session[ "ClientId" ] != null ? Session[ "ClientId" ].ToString() : null );
            clientId = ( int ) ( clientId != null ? ( int ) clientId : !string.IsNullOrEmpty( sessClientId ) ? int.Parse( sessClientId ) : 0 );

            using ( ChepLoadService chepService = new ChepLoadService() )
            {

                load = chepService.ListCSM( new PagingModel(), new CustomSearchModel() { ClientId = ( int ) clientId, Status = ( int ) ReconciliationStatus.Unreconciled, ReconciliationStatus = ( int ) ReconciliationStatus.Unreconciled } ); //(pspId, int.Parse(groupId), new CustomSearchModel() { ClientId = clientId, Status = Status.Active });

            }

            if ( load != null )
            {
                return Json( load, JsonRequestBehavior.AllowGet );
            }
            else
            {
                return Json( data: "Error", behavior: JsonRequestBehavior.AllowGet );
            }
        }
        #endregion

        //-------------------------------------------------------------------------------------

        #region Pallet ReconcileInvoice
        //
        // GET: /Pallet/ReconcileInvoice
        public ActionResult ReconcileInvoice( PagingModel pm, CustomSearchModel csm, bool givecsm = false )
        {
            if ( givecsm )
            {
                ViewBag.ViewName = "ReconcileInvoice";

                return PartialView( "_ReconcileInvoiceCustomSearch", new CustomSearchModel( "ReconcileInvoice" ) );
            }
            ViewBag.ViewName = "ReconcileInvoice";
            string sessClientId = ( Session[ "ClientId" ] != null ? Session[ "ClientId" ].ToString() : null );
            int clientId = ( !string.IsNullOrEmpty( sessClientId ) ? int.Parse( sessClientId ) : 0 );
            ViewBag.ContextualMode = ( clientId > 0 ? true : false ); //Whether a client is specific or not and the View can know about it
                                                                      // model.ContextualMode = (clientId > 0 ? true : false); //Whether a client is specific or not and the View can know about it
            List<ClientCustomModel> clientList = new List<ClientCustomModel>();
            //TODO
            using ( ClientService clientService = new ClientService() )
            {
                clientList = clientService.List1( new PagingModel(), new CustomSearchModel() { ClientId = clientId, Status = Status.Active } );
            }

            IEnumerable<SelectListItem> clientDDL = clientList.Select( c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.CompanyName

            } );
            ViewBag.ClientList = clientDDL;
            int total = 0;

            List<Site> model = new List<Site>();
            PagingExtension paging = PagingExtension.Create( model, total, pm.Skip, pm.Take, pm.Page );

            return PartialView( "_ReconcileInvoice", paging );
        }
        #endregion

        //-------------------------------------------------------------------------------------

        #region Pallet Exceptions
        //
        // GET: /Pallet/Exceptions
        public ActionResult Exceptions( PagingModel pm, CustomSearchModel csm, bool givecsm = false )
        {
            if ( givecsm )
            {
                ViewBag.ViewName = "Exceptions";

                return PartialView( "_ExceptionsCustomSearch", new CustomSearchModel( "Exceptions" ) );
            }
            ViewBag.ViewName = "Exceptions";
            string sessClientId = ( Session[ "ClientId" ] != null ? Session[ "ClientId" ].ToString() : null );
            int clientId = ( !string.IsNullOrEmpty( sessClientId ) ? int.Parse( sessClientId ) : 0 );
            ViewBag.ContextualMode = ( clientId > 0 ? true : false ); //Whether a client is specific or not and the View can know about it
            //model.ContextualMode = (clientId > 0 ? true : false); //Whether a client is specific or not and the View can know about it
            List<ClientCustomModel> clientList = new List<ClientCustomModel>();
            //TODO
            using ( ClientService clientService = new ClientService() )
            {
                clientList = clientService.List1( new PagingModel(), new CustomSearchModel() { ClientId = clientId, Status = Status.Active } );
            }

            IEnumerable<SelectListItem> clientDDL = clientList.Select( c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.CompanyName

            } );
            ViewBag.ClientList = clientDDL;
            int total = 0;

            List<Site> model = new List<Site>();
            PagingExtension paging = PagingExtension.Create( model, total, pm.Skip, pm.Take, pm.Page );

            return PartialView( "_Exceptions", paging );
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
                Dispute model = service.GetById( id );

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
            using ( ChepLoadService clservice = new ChepLoadService() )
            {
                //if ( dservice.Exist( model.DocketNumber ) )
                //{
                //    // Dispute already exist!
                //    Notify( $"Sorry, an ACTIVE Dispute for the Docket Number \"{model.DocketNumber}\" already exists!", NotificationType.Error );

                //    return View( model );
                //}

                if ( !model.ChepLoadId.HasValue )
                {
                    // Attempt to locate ChepLoad using the specified DocketNumber
                    ChepLoad cl = clservice.GetByDocketNumber( model.DocketNumber?.Trim() );

                    model.ChepLoadId = ( cl != null ) ? cl.Id : model.ChepLoadId;
                }

                model.ActionById = ( !model.ActionById.HasValue ) ? CurrentUser.Id : model.ActionById;
                model.ResolvedOn = ( model.Status != DisputeStatus.Active && !model.ResolvedOn.HasValue ) ? DateTime.Now : model.ResolvedOn;
                model.ResolvedById = ( model.Status != DisputeStatus.Active && !model.ResolvedById.HasValue ) ? CurrentUser.Id : model.ResolvedById;

                Dispute dispute = new Dispute()
                {
                    Sender = model.Sender,
                    Product = model.Product,
                    Declarer = model.Declarer,
                    Quantity = model.Quantity,
                    Receiver = model.Receiver,
                    Equipment = model.Equipment,
                    TDNNumber = model.TDNNumber,
                    OtherParty = model.OtherParty,
                    Status = ( int ) model.Status,
                    ResolvedOn = model.ResolvedOn,
                    ChepLoadId = model.ChepLoadId,
                    Reference = model.DocketNumber,
                    ActionedById = model.ActionById,
                    DocketNumber = model.DocketNumber,
                    DisputeEmail = model.DisputeEmail,
                    ResolvedById = model.ResolvedById,
                    DisputeReason = model.DisputeReason,
                };

                dservice.Create( dispute );

                Notify( "The Dispute was successfully created.", NotificationType.Success );

                return RedirectToAction( "Disputes" );
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
                    Sender = dispute.Sender,
                    Product = dispute.Product,
                    Declarer = dispute.Declarer,
                    Receiver = dispute.Receiver,
                    TDNNumber = dispute.TDNNumber,
                    Equipment = dispute.Equipment,
                    OtherParty = dispute.OtherParty,
                    ResolvedOn = dispute.ResolvedOn,
                    ChepLoadId = dispute.ChepLoadId,
                    ActionById = dispute.ActionedById,
                    DocketNumber = dispute.DocketNumber,
                    Quantity = ( int ) dispute.Quantity,
                    ResolvedById = dispute.ResolvedById,
                    DisputeEmail = dispute.DisputeEmail,
                    DisputeReason = dispute.DisputeReason,
                    Status = ( DisputeStatus ) dispute.Status,
                };

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

                if ( !model.ChepLoadId.HasValue )
                {
                    // Attempt to locate ChepLoad using the specified DocketNumber
                    ChepLoad cl = clservice.GetByDocketNumber( model.DocketNumber?.Trim() );

                    model.ChepLoadId = ( cl != null ) ? cl.Id : model.ChepLoadId;
                }

                model.ActionById = ( !model.ActionById.HasValue ) ? CurrentUser.Id : model.ActionById;
                model.ResolvedOn = ( model.Status != DisputeStatus.Active && !model.ResolvedOn.HasValue ) ? DateTime.Now : model.ResolvedOn;
                model.ResolvedById = ( model.Status != DisputeStatus.Active && !model.ResolvedById.HasValue ) ? CurrentUser.Id : model.ResolvedById;

                dispute.Sender = model.Sender;
                dispute.Product = model.Product;
                dispute.Declarer = model.Declarer;
                dispute.Quantity = model.Quantity;
                dispute.Receiver = model.Receiver;
                dispute.Equipment = model.Equipment;
                dispute.TDNNumber = model.TDNNumber;
                dispute.OtherParty = model.OtherParty;
                dispute.Status = ( int ) model.Status;
                dispute.ResolvedOn = model.ResolvedOn;
                dispute.ChepLoadId = model.ChepLoadId;
                dispute.Reference = model.DocketNumber;
                dispute.ActionedById = model.ActionById;
                dispute.DocketNumber = model.DocketNumber;
                dispute.DisputeEmail = model.DisputeEmail;
                dispute.ResolvedById = model.ResolvedById;
                dispute.DisputeReason = model.DisputeReason;

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

        #endregion


        //-------------------------------------------------------------------------------------


        #region General


        #endregion


        //-------------------------------------------------------------------------------------

        #region APIs

        [AcceptVerbs( HttpVerbs.Get | HttpVerbs.Post )]
        public JsonResult GetVehiclesForTransporter( string transId )
        {
            if ( transId != null && transId != "" )
            {
                List<Vehicle> sites = null;
                using ( VehicleService service = new VehicleService() )
                {

                    sites = service.ListByColumnsWhere( "ObjectId", int.Parse( transId ), "ObjectType", "Transporter" );

                }
                //var jsonList = JsonConvert.SerializeObject(sites);
                //return Json(sites, JsonRequestBehavior.AllowGet);
                var data = JsonConvert.SerializeObject( sites, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore } );
                return Json( new { data = data }, JsonRequestBehavior.AllowGet );
            }
            else
            {
                return Json( data: "Error", behavior: JsonRequestBehavior.AllowGet );
            }
        }

        [AcceptVerbs( HttpVerbs.Get | HttpVerbs.Post )]
        public JsonResult ReconcileLoadsByIds( string agentLoadId, string clientLoadId )
        {
            if ( !string.IsNullOrEmpty( agentLoadId ) && !string.IsNullOrEmpty( clientLoadId ) )
            {
                //using (GroupService service = new GroupService())
                using ( ChepClientService cgservivce = new ChepClientService() )
                using ( ClientLoadService clientservice = new ClientLoadService() )
                using ( ChepLoadService chepService = new ChepLoadService() )
                using ( TransactionScope scope = new TransactionScope() )
                {
                    //get objects to reconcile
                    ClientLoad client = clientservice.GetById( int.Parse( clientLoadId ) );
                    ChepLoad agent = chepService.GetById( int.Parse( agentLoadId ) );
                    // create new chep client object
                    ChepClient agentclient = new ChepClient();

                    //Run Validation first to ensure everything checks out to allow reconcilliation

                    //set up all agent items and update
                    agent.Status = ( int ) ReconciliationStatus.Reconciled;
                    client.Status = ( int ) ReconciliationStatus.Reconciled;
                    agentclient.Status = ( int ) ReconciliationStatus.Reconciled;
                    agentclient.ChepLoadsId = agent.Id;
                    agentclient.ClientLoadsId = client.Id;

                    chepService.Update( agent );
                    clientservice.Update( client );
                    cgservivce.Create( agentclient );

                    scope.Complete();
                }


                return Json( data: "True", behavior: JsonRequestBehavior.AllowGet );
            }
            else
            {
                return Json( data: "Error", behavior: JsonRequestBehavior.AllowGet );
            }
        }


        #endregion

        #region Journals Tasks and Comments

        #region Comments
        public JsonResult RemoveComment( int? id = null, string ctype = null )
        {
            int commId = ( id != null ? ( int ) id : 0 );
            if ( commId > 0 )
            {
                using ( TransactionScope scope = new TransactionScope() )
                using ( CommentService service = new CommentService() )
                {

                    Comment comm = service.GetById( commId );
                    comm.Status = ( int ) Status.Inactive;
                    service.Update( comm );

                    scope.Complete();
                }
                return Json( data: "True", behavior: JsonRequestBehavior.AllowGet );
            }
            else
            {
                return Json( data: "Error", behavior: JsonRequestBehavior.AllowGet );
            }

        }
        public JsonResult AddComment( int? id = null, string utype = null, string details = null )
        {

            if ( id != null && !string.IsNullOrEmpty( utype ) )
            {
                Comment comm = new Comment()
                {
                    Details = details,
                    ObjectId = ( int ) id,
                    ObjectType = utype,
                    Status = ( int ) Status.Active
                };

                using ( TransactionScope scope = new TransactionScope() )
                using ( CommentService service = new CommentService() )
                {
                    service.Create( comm );
                }
            }

            //return Json("Uploaded " + Request.Files.Count + " files");
            return Json( data: "True", behavior: JsonRequestBehavior.AllowGet );
        }

        public ActionResult ListComments( string objId, string objType )
        {
            List<Comment> commlist = new List<Comment>();
            if ( !string.IsNullOrEmpty( objType ) && !string.IsNullOrEmpty( objId ) )
            {
                string controllerObjectType = objType;
                using ( CommentService service = new CommentService() )
                {
                    commlist = service.ListByColumnsWhere( "ObjectId", objId, "ObjectType", objType );
                }
            }
            if ( commlist != null )
            {
                return Json( commlist, JsonRequestBehavior.AllowGet );
            }
            else
            {
                return Json( data: "Error", behavior: JsonRequestBehavior.AllowGet );
            }
        }
        #endregion

        #region Tasks
        public JsonResult RemoveTask( int? id = null, string ctype = null )
        {
            int tskId = ( id != null ? ( int ) id : 0 );
            if ( tskId > 0 )
            {
                using ( TransactionScope scope = new TransactionScope() )
                using ( TaskService service = new TaskService() )
                {

                    Task tsk = service.GetById( tskId );
                    tsk.Status = ( int ) Status.Inactive;
                    service.Update( tsk );

                    scope.Complete();
                }
                return Json( data: "True", behavior: JsonRequestBehavior.AllowGet );
            }
            else
            {
                return Json( data: "Error", behavior: JsonRequestBehavior.AllowGet );
            }

        }
        public JsonResult AddTask( int clientId, int agentLoadId, int clientLoadId, string name, string description )
        {

            if ( clientId > 0 && agentLoadId > 0 && clientLoadId > 0 )
            {
                Task tsk = new Task()
                {
                    ClientId = clientId,
                    ChepLoadId = agentLoadId,
                    ClientLoadId = clientLoadId,
                    Name = name,
                    Description = description,
                    Status = ( int ) Status.Active,
                    Action = ( int ) ActionType.None
                };

                using ( TransactionScope scope = new TransactionScope() )
                using ( TaskService service = new TaskService() )
                {
                    service.Create( tsk );
                    scope.Complete();
                }
            }

            //return Json("Uploaded " + Request.Files.Count + " files");
            return Json( data: "True", behavior: JsonRequestBehavior.AllowGet );
        }

        public JsonResult MarkTask( int taskId, int agentLoadId, int clientLoadId )
        {

            if ( taskId > 0 && agentLoadId > 0 && clientLoadId > 0 )
            {

                using ( TransactionScope scope = new TransactionScope() )
                using ( TaskService service = new TaskService() )
                {
                    Task tsk = service.GetById( taskId );
                    tsk.Status = ( int ) Status.Inactive;
                    service.Update( tsk );

                    scope.Complete();

                }
            }

            //return Json("Uploaded " + Request.Files.Count + " files");
            return Json( data: "True", behavior: JsonRequestBehavior.AllowGet );
        }

        public JsonResult EmailTask( int taskId, int agentLoadId, int clientLoadId, string instruction, string to )
        {

            if ( taskId > 0 && agentLoadId > 0 && clientLoadId > 0 )
            {
                //UPDATE EMAILS HERE TO EMAIL

                using ( TransactionScope scope = new TransactionScope() )
                using ( TaskService service = new TaskService() )
                {
                    Task tsk = service.GetById( taskId );
                    tsk.Status = ( int ) Status.Active;
                    tsk.Action = ( int ) ActionType.Email;
                    service.Update( tsk );

                    scope.Complete();
                }
            }

            //return Json("Uploaded " + Request.Files.Count + " files");
            return Json( data: "True", behavior: JsonRequestBehavior.AllowGet );
        }

        public ActionResult ListTasks( string objId, string objType )
        {
            List<Comment> commlist = new List<Comment>();
            if ( !string.IsNullOrEmpty( objType ) && !string.IsNullOrEmpty( objId ) )
            {
                string controllerObjectType = objType;
                using ( CommentService service = new CommentService() )
                {
                    commlist = service.ListByColumnsWhere( "ObjectId", objId, "ObjectType", objType );
                }
            }
            if ( commlist != null )
            {
                return Json( commlist, JsonRequestBehavior.AllowGet );
            }
            else
            {
                return Json( data: "Error", behavior: JsonRequestBehavior.AllowGet );
            }
        }
        #endregion

        #region Tasks
        public JsonResult RemoveJournal( int? id = null, string ctype = null )
        {
            int tskId = ( id != null ? ( int ) id : 0 );
            if ( tskId > 0 )
            {
                using ( TransactionScope scope = new TransactionScope() )
                using ( JournalService service = new JournalService() )
                {

                    Journal tsk = service.GetById( tskId );
                    tsk.Status = ( int ) Status.Inactive;
                    service.Update( tsk );

                    scope.Complete();
                }
                return Json( data: "True", behavior: JsonRequestBehavior.AllowGet );
            }
            else
            {
                return Json( data: "Error", behavior: JsonRequestBehavior.AllowGet );
            }

        }
        public JsonResult AddJournal( int clientLoadId, int siteAuditId, int documentId, string description, string qty, int isIn, string refnumber )
        {

            if ( clientLoadId > 0 )
            {
                Journal journal = new Journal() { };
                journal.ClientLoadId = ( int ) clientLoadId;
                if ( siteAuditId > 0 )
                    journal.SiteAuditId = ( int ) siteAuditId;
                if ( documentId > 0 )
                    journal.DocumetId = ( int ) documentId;

                journal.PostingDescription = description;
                journal.InOutInd = ( ( int ) isIn == 0 ? false : true );

                if ( qty != null )
                {
                    decimal inQty = 0;
                    decimal.TryParse( qty, out inQty );
                    journal.PostingQuantity = inQty;

                }
                journal.THAN = refnumber;
                journal.Status = ( int ) Status.Active;
                journal.JournalType = "ClientLoad";

                using ( TransactionScope scope = new TransactionScope() )
                using ( JournalService service = new JournalService() )
                {
                    service.Create( journal );
                }
            }

            //return Json("Uploaded " + Request.Files.Count + " files");
            return Json( data: "True", behavior: JsonRequestBehavior.AllowGet );
        }

        public ActionResult ListJournals( string clientLoadId )
        {
            List<Journal> commlist = new List<Journal>();
            if ( !string.IsNullOrEmpty( clientLoadId ) )
            {
                using ( JournalService service = new JournalService() )
                {
                    commlist = service.ListByColumnWhere( "ClientLoadId", int.Parse( clientLoadId ) );
                }
            }
            if ( commlist != null )
            {
                return Json( commlist, JsonRequestBehavior.AllowGet );
            }
            else
            {
                return Json( data: "Error", behavior: JsonRequestBehavior.AllowGet );
            }
        }
        #endregion


        #endregion

        #region Uploads and Imports



        [HttpPost]
        // POSt: /Client/ImportChepLoad
        public ActionResult ImportChepLoad( HttpPostedFileBase postedFile )
        {
            string importMessage = "Pooling Agent Import Started\r\n";
            if ( postedFile != null )
            {
                string fileExtension = Path.GetExtension( postedFile.FileName );

                //Validate uploaded file and return error.
                if ( fileExtension != ".csv" )
                {
                    ViewBag.Message = "Please select the csv file with .csv extension";
                    return View();
                }

                try
                {
                    string sessClientId = ( Session[ "ClientId" ] != null ? Session[ "ClientId" ].ToString() : null );
                    int clientID = ( !string.IsNullOrEmpty( sessClientId ) ? int.Parse( sessClientId ) : 0 );
                    int cnt = 0;
                    int cntCreated = 0;

                    using ( var sreader = new StreamReader( postedFile.InputStream ) )
                    using ( ChepLoadService service = new ChepLoadService() )
                    using ( TransactionScope scope = new TransactionScope() )
                    {
                        //First line is header. If header is not passed in csv then we can neglect the below line.
                        string[] headers = sreader.ReadLine().Split( ',' );
                        //Loop through the records
                        while ( !sreader.EndOfStream )
                        {
                            try
                            {
                                string[] rows = sreader.ReadLine().Split( ',' );

                                ChepLoad model = new ChepLoad();

                                if ( !string.IsNullOrEmpty( rows[ 0 ] ) )
                                {
                                    model.LoadDate = ( !string.IsNullOrEmpty( rows[ 0 ] ) ? DateTimeHelper.formatImportDate( rows[ 0 ] ) : DateTime.Now );
                                    model.EffectiveDate = ( !string.IsNullOrEmpty( rows[ 0 ] ) ? DateTimeHelper.formatImportDate( rows[ 1 ] ) : DateTime.Now );
                                    model.NotifyDate = ( !string.IsNullOrEmpty( rows[ 0 ] ) ? DateTimeHelper.formatImportDate( rows[ 2 ] ) : DateTime.Now );
                                    model.DocketNumber = rows[ 3 ];
                                    model.PostingType = 2;//import - Transfer Customer to Customer - Out
                                    model.ClientDescription = rows[ 5 ];
                                    model.ReferenceNumber = rows[ 6 ];
                                    model.ReceiverNumber = rows[ 7 ];
                                    model.Equipment = rows[ 10 ];
                                    model.CreatedOn = DateTime.Now;
                                    model.ModfiedOn = DateTime.Now;
                                    model.OriginalQuantity = ( decimal.TryParse( rows[ 11 ], out decimal oQty ) ? oQty : 0 );
                                    model.NewQuantity = model.OriginalQuantity;

                                    service.Create( model );
                                    importMessage += " Trading Partner: " + model.ClientDescription + " created at Id " + model.Id + "<br>";
                                    Notify( "The PSP Configuration details were successfully updated.", NotificationType.Success );
                                    cntCreated++;
                                }
                            }
                            catch ( Exception ex )
                            {
                                importMessage += ex.Message + "<br>";
                                ViewBag.Message = ex.Message;
                            }
                            cnt++;
                        }
                        importMessage += " Records To Process: " + cnt + "<br>";
                        importMessage += " Records Processed: " + cntCreated + "<br>";
                        scope.Complete();
                        Session[ "ImportMessage" ] = importMessage;
                    }

                }
                catch ( Exception ex )
                {
                    importMessage += ex.Message + "<br>";
                    ViewBag.Message = ex.Message;
                }
                finally
                {

                }
            }
            else
            {

            }
            Session[ "ImportMessage" ] = importMessage;
            return RedirectToAction( "PoolingAgentData", "Pallet" );
        }


        [HttpPost]
        // GET: /Client/ImportClientLoad
        public ActionResult ImportClientLoad( HttpPostedFileBase postedFile )
        {
            string importMessage = "Client Load Import Started\r\n";
            if ( postedFile != null )
            {
                string fileExtension = Path.GetExtension( postedFile.FileName );

                //Validate uploaded file and return error.
                if ( fileExtension != ".csv" )
                {
                    ViewBag.Message = "Please select the csv file with .csv extension";
                    return View();
                }

                try
                {
                    string sessClientId = ( Session[ "ClientId" ] != null ? Session[ "ClientId" ].ToString() : null );
                    int clientID = ( !string.IsNullOrEmpty( sessClientId ) ? int.Parse( sessClientId ) : 0 );
                    int cnt = 0;
                    int cntCreated = 0;

                    using ( var sreader = new StreamReader( postedFile.InputStream ) )
                    using ( ClientLoadService service = new ClientLoadService() )
                    using ( DeliveryNoteService noteservice = new DeliveryNoteService() )
                    using ( DeliveryNoteLineService lineservice = new DeliveryNoteLineService() )
                    using ( AddressService addservice = new AddressService() )
                    using ( ClientService clientservice = new ClientService() )
                    using ( TransactionScope scope = new TransactionScope() )
                    {
                        //First line is header. If header is not passed in csv then we can neglect the below line.
                        string[] headers = sreader.ReadLine().Split( ',' );
                        //Loop through the records
                        while ( !sreader.EndOfStream )
                        {
                            try
                            {
                                string[] rows = sreader.ReadLine().Split( ',' );

                                ClientLoad model = new ClientLoad();
                                int vehicleId = 0;
                                //no transporter data
                                //int transporterId = 0;
                                //if (!string.IsNullOrEmpty(rows[11]))
                                //{
                                //    using (TransporterService transservice = new TransporterService())
                                //    {
                                //        TransporterCustomModel trans = transservice.ListCSM(new PagingModel(), new CustomSearchModel() { Query = rows[11], Status = Status.Active }).FirstOrDefault();
                                //        if (trans != null)
                                //        {
                                //            transporterId = trans.Id; //else remains 0
                                //        }
                                //        else
                                //        {
                                //            try
                                //            {
                                //                Transporter tra = new Transporter()
                                //                {
                                //                    Name = rows[11],
                                //                    ContactNumber = "TBC",
                                //                    Email = "TBC",
                                //                    TradingName = rows[11],
                                //                    RegistrationNumber = rows[12],
                                //                    Status = (int)Status.Active

                                //                };
                                //                transservice.Create(tra);
                                //                transporterId = tra.Id;
                                //                importMessage += " Transporter: " + tra.Name + " created at Id " + tra.Id + "<br>";
                                //            }
                                //            catch (Exception ex)
                                //            {
                                //                importMessage += "Reg : " + rows[12] + "Error " + ex.Message + "<br>";
                                //                ViewBag.Message = ex.Message;
                                //            }
                                //        }
                                //    }
                                //} //no else, if  there are no column data we cant add or facilitate a row addition
                                if ( !string.IsNullOrEmpty( rows[ 12 ] ) )// && transporterId > 0
                                {
                                    using ( VehicleService vehservice = new VehicleService() )
                                    {
                                        VehicleCustomModel vehicle = vehservice.ListCSM( new PagingModel(), new CustomSearchModel() { Query = rows[ 12 ], Status = Status.Active } ).FirstOrDefault();
                                        if ( vehicle != null )
                                        {
                                            vehicleId = vehicle.Id; //else remains 0
                                        }
                                        else
                                        {
                                            try
                                            {
                                                Vehicle veh = new Vehicle()
                                                {
                                                    ObjectId = clientID,
                                                    ObjectType = "Client",
                                                    Make = "TBC",
                                                    Model = "TBC",
                                                    Year = 1,
                                                    EngineNumber = rows[ 12 ],
                                                    VINNumber = rows[ 12 ],
                                                    Registration = rows[ 12 ],
                                                    Descriptoin = rows[ 12 ],
                                                    Type = ( int ) VehicleType.Other,
                                                    Status = ( int ) Status.Active
                                                };
                                                vehservice.Create( veh );
                                                vehicleId = veh.Id;
                                                importMessage += " Vehicle: " + veh.Registration + " created at Id " + veh.Id + " For Transporter " + rows[ 11 ] + "<br>";
                                            }
                                            catch ( Exception ex )
                                            {
                                                importMessage += "Reg : " + rows[ 12 ] + "Error " + ex.Message + "<br>";
                                                ViewBag.Message = ex.Message;
                                            }
                                        }
                                    }

                                } //no else, if  there are no column data we cant add or facilitate a row addition

                                if ( !string.IsNullOrEmpty( rows[ 0 ] ) )
                                {
                                    if ( vehicleId > 0 )//&& transporterId > 0
                                    {
                                        model.ClientId = clientID;
                                        model.LoadDate = ( !string.IsNullOrEmpty( rows[ 0 ] ) ? DateTimeHelper.formatImportDate( rows[ 0 ] ) : DateTime.Now );
                                        model.LoadNumber = rows[ 1 ];
                                        model.AccountNumber = rows[ 2 ];
                                        model.ClientDescription = rows[ 3 ];
                                        //model.ProvCode = rows[4];
                                        model.PCNNumber = rows[ 5 ];
                                        model.DeliveryNote = rows[ 6 ];
                                        //model.PRNNumber = rows[7];
                                        model.OriginalQuantity = ( decimal.TryParse( rows[ 7 ], out decimal oQty ) ? oQty : 0 );
                                        model.NewQuantity = model.OriginalQuantity;
                                        model.ReturnQty = ( decimal.TryParse( rows[ 8 ], out decimal rQty ) ? rQty : 0 );
                                        //model.RetQuantity = decimal.Parse(rows[9]);
                                        //       model.ARPMComments = rows[10];
                                        //some of the columns are malaligned but leaving it as is, I added 3 new columns
                                        if ( vehicleId > 0 )
                                            model.VehicleId = vehicleId;
                                        //if (transporterId > 0)
                                        //    model.TransporterId = transporterId;
                                        model.CreatedOn = DateTime.Now;
                                        model.ModifiedOn = DateTime.Now;
                                        service.Create( model );
                                        importMessage += " Customer: " + model.ClientDescription + " created at Id " + model.Id + "<br>";
                                        cntCreated++;

                                        //after import create delivery note
                                        try
                                        {
                                            #region Save DeliveryNote
                                            DeliveryNote delnote = new DeliveryNote();

                                            Client noteClient = clientservice.GetById( clientID );
                                            Address clientAddress = addservice.GetByColumnsWhere( "ObjectId", model.ClientId, "ObjectType", "Client" );
                                            string customerAddress = clientAddress.Addressline1 + ' ' + clientAddress.Addressline2 + ' ' + clientAddress.Town + ' ' + clientAddress.PostalCode + ' ' + ( ( Province ) clientAddress.Province ).GetDisplayText();
                                            //string billingAddress = model.BillingAddress + ' ' + model.BillingAddress2 + ' ' + model.BillingAddressTown + ' ' + model.BillingPostalCode + ' ' + ((Province)model.BillingProvince).GetDisplayText();
                                            //string deliveryAddress = model.DeliveryAddress + ' ' + model.DeliveryAddress2 + ' ' + model.DeliveryAddressTown + ' ' + model.DeliveryPostalCode + ' ' + ((Province)model.DeliveryProvince).GetDisplayText();

                                            if ( model.Id > 0 )
                                            {
                                                try
                                                {
                                                    delnote.ClientId = model.ClientId;
                                                    //Create
                                                    delnote.InvoiceNumber = model.DeliveryNote;
                                                    delnote.CustomerName = noteClient.CompanyName;
                                                    delnote.EmailAddress = noteClient.Email;
                                                    delnote.OrderNumber = model.ReferenceNumber;
                                                    delnote.OrderDate = model.LoadDate;
                                                    //delnote.ContactNumber = model.ContactNumber;
                                                    delnote.Reference306 = model.LoadNumber;
                                                    delnote.Status = ( int ) Status.Active;

                                                    delnote.CustomerAddress = customerAddress;
                                                    delnote.CustomerPostalCode = clientAddress.PostalCode;
                                                    delnote.CustomerProvince = clientAddress.Province;

                                                    //dont have the below to create
                                                    //delnote.DeliveryAddress = deliveryAddress;
                                                    //delnote.DeliveryPostalCode = model.DeliveryPostalCode;
                                                    //delnote.DeliveryProvince = model.DeliveryProvince;

                                                    //delnote.BillingAddress = billingAddress;
                                                    //delnote.BililngPostalCode = model.BillingPostalCode;
                                                    //delnote.BillingProvince = model.BillingProvince;

                                                    //Create Client Invoice Address
                                                    noteservice.Create( delnote );

                                                    //Create Invoice Customer Address
                                                    Address invoiceCustomerAddress = new Address()
                                                    {
                                                        ObjectId = delnote.Id,
                                                        ObjectType = "InvoiceCustomer",
                                                        Town = clientAddress.Town,
                                                        Status = ( int ) Status.Active,
                                                        PostalCode = clientAddress.PostalCode,
                                                        Type = 1,
                                                        Addressline1 = clientAddress.Addressline1,
                                                        Addressline2 = clientAddress.Addressline2,
                                                        Province = ( int ) clientAddress.Province,
                                                        //Province = (int)mappedProvinceVal,
                                                    };
                                                    addservice.Create( invoiceCustomerAddress );


                                                }
                                                catch ( Exception ex )
                                                {
                                                    ViewBag.Message = ex.Message;
                                                    //  return View();
                                                    return PartialView( "_GenerateDeliveryNote", model );
                                                }

                                            }
                                            #endregion
                                            #region Save DeliveryNoteLine
                                            try
                                            {
                                                DeliveryNoteLine noteline = new DeliveryNoteLine();

                                                noteline.DeliveryNoteId = delnote.Id;//deliveryNoteId;
                                                noteline.Delivered = model.NewQuantity;
                                                noteline.Product = "Delivery Note";
                                                noteline.ProductDescription = "Delivery Note";
                                                noteline.OrderQuantity = model.OriginalQuantity;
                                                noteline.ModifiedOn = DateTime.Now;
                                                noteline.CreatedOn = DateTime.Now;
                                                noteline.Status = ( int ) Status.Active;

                                                lineservice.Create( noteline );
                                            }
                                            catch ( Exception ex )
                                            {
                                                ViewBag.Message = ex.Message;
                                            }

                                            #endregion
                                        }
                                        catch ( Exception ex )
                                        {
                                            ViewBag.Message = ex.Message;
                                        }
                                    }
                                    else
                                    {
                                        importMessage += " File Row : " + cnt + " could not be inserted, no Vehicle or Transporter that matches. Skipped<br>";
                                    }
                                }
                                else
                                {
                                    importMessage += " File Row : " + cnt + " could not be inserted. Check Data<br>";
                                }
                            }
                            catch ( Exception ex )
                            {
                                importMessage += ex.Message + "<br>";
                                ViewBag.Message = ex.Message;
                            }
                            cnt++;
                        }
                        importMessage += " Records To Process: " + cnt + "<br>";
                        importMessage += " Records Processed: " + cntCreated + "<br>";
                        scope.Complete();
                        Session[ "ImportMessage" ] = importMessage;
                    }

                }
                catch ( Exception ex )
                {
                    importMessage += ex.Message + "<br>";
                    ViewBag.Message = ex.Message;
                }
                finally
                {

                }
            }
            else
            {

            }
            Session[ "ImportMessage" ] = importMessage;
            return RedirectToAction( "ClientData", "Pallet" );
        }


        //
        // Returns a general list of all active clients allowed in the current context to be selected from
        [AcceptVerbs( HttpVerbs.Get | HttpVerbs.Post )]
        public JsonResult ImportEmails( string pspId = null )
        {
            if ( pspId != null )
            {
                using ( PSPConfigService confservice = new PSPConfigService() )
                {
                    PSPConfig conf = confservice.GetById( int.Parse( pspId ) );

                    if ( conf.ImportEmailHost != null && conf.ImportEmailPort != null )
                    {
                        OpenPop.Pop3.Pop3Client pop3Client = new OpenPop.Pop3.Pop3Client();
                        pop3Client.Connect( conf.ImportEmailHost, conf.ImportEmailPort ?? 110, true );
                        pop3Client.Authenticate( conf.ImportEmailUsername, conf.ImportEmailPassword, OpenPop.Pop3.AuthenticationMethod.UsernameAndPassword );

                        int count = pop3Client.GetMessageCount();
                        List<EmailCustomModel> Emails = new List<EmailCustomModel>();
                        int counter = 0;
                        for ( int i = count; i >= 1; i-- )
                        {
                            OpenPop.Mime.Message message = pop3Client.GetMessage( i );
                            EmailCustomModel email = new EmailCustomModel()
                            {
                                MessageNumber = i,
                                Subject = message.Headers.Subject,
                                DateSent = message.Headers.DateSent,
                                From = string.Format( "<a href = 'mailto:{1}'>{0}</a>", message.Headers.From.DisplayName, message.Headers.From.Address ),
                            };
                            OpenPop.Mime.MessagePart body = message.FindFirstHtmlVersion();
                            if ( body != null )
                            {
                                email.Body = body.GetBodyAsText();
                            }
                            else
                            {
                                body = message.FindFirstPlainTextVersion();
                                if ( body != null )
                                {
                                    email.Body = body.GetBodyAsText();
                                }
                            }
                            List<OpenPop.Mime.MessagePart> attachments = message.FindAllAttachments();

                            foreach ( OpenPop.Mime.MessagePart attachment in attachments )
                            {
                                email.Attachments.Add( new AttachmentCustomModel
                                {
                                    FileName = attachment.FileName,
                                    ContentType = attachment.ContentType.MediaType,
                                    Content = attachment.Body
                                } );
                            }
                            Emails.Add( email );
                            counter++;
                            if ( counter > 2 )
                            {
                                break;
                            }
                        }
                        // Session["Pop3Client"] = pop3Client;
                        //List<Client> model = new List<Client>();
                        //PagingModel pm = new PagingModel();
                        //CustomSearchModel csm = new CustomSearchModel();

                        //using (ClientService service = new ClientService())
                        //{
                        //    pm.Sort = pm.Sort ?? "ASC";
                        //    pm.SortBy = pm.SortBy ?? "Name";
                        //    csm.Status = Status.Active;
                        //    csm.Status = Status.Active;

                        //    model = service.ListCSM(pm, csm);
                        //}
                        //if (model.Any())
                        //{
                        //    IEnumerable<SelectListItem> clientDDL = model.Select(c => new SelectListItem
                        //    {
                        //        Value = c.Id.ToString(),
                        //        Text = c.CompanyName

                        //    });
                    }
                }

                return Json( "OK", JsonRequestBehavior.AllowGet );
            }
            else
            {
                return Json( data: "Error", behavior: JsonRequestBehavior.AllowGet );
            }
        }


        #endregion



        #region Partial Views

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

                List<ClientAuthorisationCustomModel> model = service.List1( pm, csm );

                int total = ( model.Count < pm.Take && pm.Skip == 0 ) ? model.Count : service.Total1( pm, csm );

                PagingExtension paging = PagingExtension.Create( model, total, pm.Skip, pm.Take, pm.Page );

                return PartialView( "_AuthorisationCodes", paging );
            }
        }

        #endregion
    }
}
