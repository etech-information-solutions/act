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

namespace ACT.UI.Controllers
{
    [Requires( PermissionTo.View, PermissionContext.Client )]
    public class ClientController : BaseController
    {
        // GET: Client
        public ActionResult Index()
        {
            return View();
        }



        #region Clients

        //
        // POST || GET: /Client/ClientList
        public ActionResult ClientList( PagingModel pm, CustomSearchModel csm, bool givecsm = false )
        {
            if ( givecsm )
            {
                ViewBag.ViewName = "ClientList";

                return PartialView( "_ClientListCustomSearch", new CustomSearchModel( "ClientList" ) );
            }
            int total = 0;

            List<Client> model = new List<Client>();
            List<ClientViewModel> viewModel = new List<ClientViewModel>();

            using ( ClientService service = new ClientService() )
            {
                pm.Sort = pm.Sort ?? "ASC";
                pm.SortBy = pm.SortBy ?? "Name";
                csm.Status = Status.Active;
                csm.Status = Status.Active;
                if ( CurrentUser == null )
                {
                    Notify( "Sorry, it seems the session had expired. Please log in again.", NotificationType.Error );

                    return RedirectToAction( "Index", "Administration" ); //Return to login as session is invalid
                }
                //if (CurrentUser.PSPs.Count > 0)
                //{
                string sessClientId = ( Session[ "ClientId" ] != null ? Session[ "ClientId" ].ToString() : null );
                int clientId = ( !string.IsNullOrEmpty( sessClientId ) ? int.Parse( sessClientId ) : 0 );
                if ( clientId > 0 )
                {
                    csm.ClientId = clientId;
                }

                model = service.ListCSM( pm, csm );//service.GetClientsByPSP(CurrentUser.PSPs.FirstOrDefault().Id);
                foreach ( Client cl in model )
                {
                    ClientViewModel vm = new ClientViewModel()
                    {
                        Id = cl.Id,
                        CompanyName = cl.CompanyName,
                        CompanyRegistrationNumber = cl.CompanyRegistrationNumber,
                        TradingAs = cl.TradingAs,
                        Description = cl.Description,
                        VATNumber = cl.VATNumber,
                        ChepReference = cl.ChepReference,
                        ContactNumber = cl.ContactNumber,
                        ContactPerson = cl.ContactPerson,
                        AdminPerson = cl.AdminPerson,
                        AdminEmail = cl.AdminEmail,
                        FinancialPerson = cl.FinancialPerson,
                        FinPersonEmail = cl.FinPersonEmail,
                        Email = cl.Email,
                        Status = cl.Status,
                    };
                    viewModel.Add( vm );
                }
                //}
                //else
                //{
                //    model = null;
                //}

                // var testModel = service.ListByColumn(null, "CompanyRegistrationNumber", "123456");
                total = ( viewModel.Count < pm.Take && pm.Skip == 0 ) ? viewModel.Count : service.Total();
            }

            PagingExtension paging = PagingExtension.Create( viewModel, total, pm.Skip, pm.Take, pm.Page );


            return PartialView( "_ClientList", paging );
        }

        //
        // GET: /Client/ClientDetails/5
        public ActionResult ClientDetails( int id, bool layout = true )
        {
            Client model = new Client();

            using ( ClientService service = new ClientService() )
            using ( AddressService aservice = new AddressService() )
            using ( DocumentService dservice = new DocumentService() )
            using ( ClientKPIService kservice = new ClientKPIService() )
            {
                model = service.GetById( id );
                if ( model == null )
                {
                    Notify( "Sorry, the requested resource could not be found. Please try again", NotificationType.Error );

                    return RedirectToAction( "Index" );
                }
                // Session["ClientId"] = id;
                Address address = aservice.Get( model.Id, "Client" );

                List<Document> documents = dservice.List( model.Id, "Client" );
                List<Document> logo = dservice.List( model.Id, "ClientLogo" );
                ClientKPI kpi = kservice.GetByColumnWhere( "ClientId", id );

                if ( address != null )
                {
                    ViewBag.Address = address;
                }
                if ( documents != null )
                {
                    ViewBag.Documents = documents;
                }
                if ( logo != null )
                {
                    ViewBag.Logo = logo;
                }
                if ( kpi != null )
                {
                    ViewBag.KPI = kpi;
                }
            }

            if ( layout )
            {
                ViewBag.IncludeLayout = true;
            }

            return View( model );
        }

        // GET: Client/AddClient
        [Requires( PermissionTo.Create )]
        public ActionResult AddClient()
        {
            ClientViewModel model = new ClientViewModel() { EditMode = true };
            return View( model );
        }

        // POST: Client/Create
        [HttpPost]
        [Requires( PermissionTo.Create )]
        public ActionResult AddClient( ClientViewModel model )
        {
            try
            {
                if ( !ModelState.IsValid )
                {
                    Notify( "Sorry, the Client was not created. Please correct all errors and try again.", NotificationType.Error );

                    return View( model );
                }

                //On adding Client, reset the client session variable so that the client list can load fully
                Session[ "ClientId" ] = null;

                using ( ClientService service = new ClientService() )
                using ( PSPClientService pspclientservice = new PSPClientService() )
                using ( AddressService aservice = new AddressService() )
                using ( TransactionScope scope = new TransactionScope() )
                using ( DocumentService dservice = new DocumentService() )
                using ( ClientKPIService kpiservice = new ClientKPIService() )
                // using (ClientBudgetService bservice = new ClientBudgetService())
                {
                    #region Validation
                    if ( !string.IsNullOrEmpty( model.CompanyRegistrationNumber ) && service.ExistByCompanyRegistrationNumber( model.CompanyRegistrationNumber.Trim() ) )
                    {
                        // Bank already exist!
                        Notify( $"Sorry, a Client with the Registration number \"{model.CompanyRegistrationNumber}\" already exists!", NotificationType.Error );

                        return View( model );
                    }
                    #endregion
                    #region Create Client
                    Client client = new Client()
                    {
                        Email = model.Email,
                        TradingAs = model.TradingAs,
                        VATNumber = model.VATNumber,
                        ChepReference = model.ChepReference,
                        CompanyName = model.CompanyName,
                        Description = model.CompanyName,
                        ContactPerson = model.ContactPerson,
                        ContactNumber = model.ContactNumber,
                        FinancialPerson = model.FinancialPerson, //regional manager
                        FinPersonEmail = model.FinPersonEmail,
                        AdminPerson = model.AdminPerson,
                        AdminEmail = model.AdminEmail,
                        Status = ( int ) Status.Active,//model.Status,
                        ServiceRequired = ( int ) ServiceType.ManageOwnPallets,
                        //ServiceRequired = (int)model.ServiceRequired,
                        CompanyRegistrationNumber = model.CompanyRegistrationNumber
                    };
                    client = service.Create( client );
                    #endregion

                    #region Create Client PSP link
                    //int pspId = Session[ "UserPSP" ];
                    int pspId = ( CurrentUser != null ? CurrentUser.PSPs.FirstOrDefault().Id : 0 );
                    PSPClient pClient = new PSPClient()
                    {
                        PSPId = pspId,
                        ClientId = client.Id,
                        Status = ( int ) Status.Active
                    };
                    pClient = pspclientservice.Create( pClient );
                    #endregion

                    #region Create Client Budget
                    //Moved to  seperate API calls to get and set from view
                    #endregion

                    #region Create Address (s)

                    if ( model.Address != null )
                    {
                        //Enum.TryParse(model.Address.Province.ToString().Replace(" ", "").Replace("-", ""), out Province mappedProvinceVal);
                        Address address = new Address()
                        {
                            ObjectId = client.Id,
                            ObjectType = "Client",
                            Town = model.Address.Town,
                            Status = ( int ) Status.Active,
                            PostalCode = model.Address.PostCode,
                            Type = ( int ) model.Address.AddressType,
                            Addressline1 = model.Address.AddressLine1,
                            Addressline2 = model.Address.AddressLine2,
                            Province = ( int ) model.Address.Province,
                            // Province = (int)mappedProvinceVal,
                        };

                        aservice.Create( address );
                    }

                    #endregion

                    #region Any File Uploads
                    //moved to seperate ajax function
                    if ( !string.IsNullOrEmpty( model.DocsList ) )
                    {
                        String[] dList = model.DocsList.Split( ',' );
                        string lastId = "0";
                        foreach ( string itm in dList )
                        {
                            if ( !string.IsNullOrEmpty( itm ) )
                            {
                                int itmId = 0;
                                if ( int.TryParse( itm, out itmId ) )
                                {
                                    Document doc = dservice.GetById( itmId );
                                    if ( doc != null )
                                    {
                                        doc.ObjectId = itmId;//should be null for client adds, so we can update it later
                                        doc.Status = ( int ) Status.Active;

                                        dservice.Update( doc );
                                    }
                                }
                            }
                            lastId = itm;
                        }
                    }
                    #endregion

                    #region Any Logo Uploads
                    if ( model.Logo != null && model.Logo.File != null )
                    {
                        // Create folder
                        string path = Server.MapPath( $"~/{VariableExtension.SystemRules.DocumentsLocation}/Client/{client.Id}/Logo/" );

                        if ( !Directory.Exists( path ) )
                        {
                            Directory.CreateDirectory( path );
                        }

                        string now = DateTime.Now.ToString( "yyyyMMddHHmmss" );

                        Document doc = new Document()
                        {
                            ObjectId = client.Id,
                            ObjectType = "Client",
                            Status = ( int ) Status.Active,
                            Name = model.Logo.Name,
                            Category = model.Logo.Name,
                            Title = model.Logo.File.FileName,
                            Size = model.Logo.File.ContentLength,
                            Description = model.Logo.Description,
                            Type = Path.GetExtension( model.Logo.File.FileName ),
                            Location = $"Client/{client.Id}/Logo/{now}-{model.Logo.File.FileName}"
                        };

                        dservice.Create( doc );

                        string fullpath = Path.Combine( path, $"{now}-{model.Logo.File.FileName}" );
                        model.Logo.File.SaveAs( fullpath );
                    }

                    #endregion

                    #region KPI
                    //if (model.KPIDisputes > 0)
                    //{

                    //    ClientKPI newKPI = new ClientKPI()
                    //    {
                    //        ClientId = client.Id,
                    //        Disputes = model.KPIDisputes,
                    //        OutstandingPallets = model.KPIOutstanding,
                    //        Passons = 0, //TODO: Fix
                    //        MonthlyCost = 0, //TODO: Fix
                    //        ResolveDays = model.KPIDaysToResolve,
                    //        OutstandingDays = model.KPIDaysOutstanding,
                    //        Status = (int)Status.Active
                    //    };
                    //    kpiservice.Create(newKPI);

                    //}
                    #endregion


                    scope.Complete();
                }
                Notify( "The Client was successfully created.", NotificationType.Success );
                return RedirectToAction( "ClientList" );
            }
            catch ( Exception ex )
            {
                ViewBag.Message = ex.Message;
                return View();
            }
        }

        // GET: Client/EditClient/5
        [Requires( PermissionTo.Edit )]
        public ActionResult EditClient( int id )
        {
            Client client;

            using ( ClientService service = new ClientService() )
            using ( AddressService aservice = new AddressService() )
            using ( DocumentService dservice = new DocumentService() )
            using ( EstimatedLoadService eservice = new EstimatedLoadService() )
            using ( ClientKPIService kpiservice = new ClientKPIService() )
            {
                client = service.GetById( id );


                if ( client == null )
                {
                    Notify( "Sorry, the requested resource could not be found. Please try again", NotificationType.Error );

                    return PartialView( "_AccessDenied" );
                }
                //Session["ClientId"] = id;
                Address address = aservice.Get( client.Id, "Client" );

                List<Document> logo = dservice.List( client.Id, "ClientLogo" );
                List<Document> documents = dservice.List( client.Id, "Client" );

                ClientKPI kpi = kpiservice.GetByColumnWhere( "ClientId", client.Id );

                EstimatedLoad load = new EstimatedLoad();

                bool unverified = ( client.Status == ( int ) PSPClientStatus.Unverified );

                if ( unverified )
                {
                    load = eservice.Get( client.Id, "Client" );
                }

                ClientViewModel model = new ClientViewModel()
                {
                    Id = client.Id,
                    CompanyName = client.CompanyName,
                    CompanyRegistrationNumber = client.CompanyRegistrationNumber,
                    TradingAs = client.TradingAs,
                    Description = client.Description,
                    VATNumber = client.VATNumber,
                    ChepReference = client.ChepReference,
                    ContactNumber = client.ContactNumber,
                    ContactPerson = client.ContactPerson,
                    FinancialPerson = client.FinancialPerson,
                    FinPersonEmail = client.FinPersonEmail,
                    Email = client.Email,
                    AdminPerson = client.AdminPerson,
                    AdminEmail = client.AdminEmail,
                    DeclinedReason = client.DeclinedReason,
                    //ServiceRequired = client.ServiceRequired,
                    Status = client.Status,
                    EditMode = true,
                    Address = new AddressViewModel()
                    {
                        EditMode = true,
                        Town = address?.Town,
                        Id = address?.Id ?? 0,
                        PostCode = address?.PostalCode,
                        AddressLine1 = address?.Addressline1,
                        AddressLine2 = address?.Addressline2,
                        Province = ( address != null ) ? ( Province ) address.Province : Province.All,
                        AddressType = ( address != null ) ? ( AddressType ) address.Type : AddressType.Postal,
                    },
                    KPIDisputes = ( kpi != null ? kpi.Disputes : 0 ),
                    KPIOutstanding = ( kpi != null ? kpi.OutstandingPallets : 0 ),
                    KPIDaysToResolve = ( kpi != null ? kpi.ResolveDays : 0 ),
                    KPIDaysOutstanding = ( kpi != null ? kpi.OutstandingDays : 0 ),
                };
                if ( logo != null && logo.Count > 0 )
                {
                    List<FileViewModel> logofvm = new List<FileViewModel>();
                    foreach ( Document doc in logo )
                    {
                        FileViewModel tfvm = new FileViewModel()
                        {
                            Id = doc.Id,
                            Location = doc.Location,
                            Name = doc.Name,
                            Description = doc.Description

                        };
                        logofvm.Add( tfvm );
                        //model.CompanyFile.Add(fvm);

                    }
                    model.Logo = logofvm.LastOrDefault();
                    ViewBag.Logo = logo;
                }
                //Removed for validation purposes
                //Documents moved to seperate AJAX calls

                return View( model );
            }
        }

        // POST: Client/EditClient/5
        [HttpPost]
        [Requires( PermissionTo.Edit )]
        public ActionResult EditClient( ClientViewModel model, PagingModel pm, bool isstructure = false )
        {
            try
            {
                if ( !ModelState.IsValid )
                {
                    Notify( "Sorry, the selected Client was not updated. Please correct all errors and try again.", NotificationType.Error );

                    return View( model );
                }

                Client client;

                using ( ClientService service = new ClientService() )
                using ( AddressService aservice = new AddressService() )
                using ( TransactionScope scope = new TransactionScope() )
                using ( DocumentService dservice = new DocumentService() )
                using ( EstimatedLoadService eservice = new EstimatedLoadService() )
                using ( ClientKPIService kpiservice = new ClientKPIService() )
                // using (ClientBudgetService bservice = new ClientBudgetService())
                {
                    client = service.GetById( model.Id );
                    Address address = aservice.Get( client.Id, "Client" );

                    if ( client == null )
                    {
                        Notify( "Sorry, that Client does not exist! Please specify a valid Role Id and try again.", NotificationType.Error );

                        return View( model );
                    }

                    // Address address = aservice.Get(client.Id, "Client");

                    List<Document> documents = dservice.List( client.Id, "Client" );
                    List<Document> logos = dservice.List( client.Id, "ClientLogo" );

                    #region Validations

                    if ( !string.IsNullOrEmpty( model.CompanyRegistrationNumber ) && model.CompanyRegistrationNumber.Trim().ToLower() != client.CompanyRegistrationNumber.Trim().ToLower() && service.ExistByCompanyRegistrationNumber( model.CompanyRegistrationNumber.Trim() ) )
                    {
                        // Role already exist!
                        Notify( $"Sorry, a Client with the Registration Number \"{model.CompanyRegistrationNumber} ({model.CompanyRegistrationNumber})\" already exists!", NotificationType.Error );

                        return View( model );
                    }

                    #endregion

                    #region Update Client

                    // Update Client
                    client.Id = model.Id;
                    client.CompanyName = model.CompanyName;
                    client.CompanyRegistrationNumber = model.CompanyRegistrationNumber;
                    client.TradingAs = model.TradingAs;
                    client.Description = model.Description;
                    client.VATNumber = model.VATNumber;
                    client.ChepReference = model.ChepReference;
                    client.ContactNumber = model.ContactNumber;
                    client.ContactPerson = model.ContactPerson;
                    client.FinPersonEmail = model.FinPersonEmail;
                    client.FinancialPerson = model.FinancialPerson;
                    client.Email = model.Email;
                    client.AdminPerson = model.AdminPerson;
                    client.AdminEmail = model.AdminEmail;
                    //client.DeclinedReason = model.DeclinedReason;
                    //client.ServiceRequired = model.ServiceRequired;
                    client.Status = ( int ) model.Status;

                    service.Update( client );

                    #endregion


                    #region Create Address (s)

                    if ( model.Address != null )
                    {
                        Address clientAddress = aservice.GetById( model.Address.Id );
                        //Enum.TryParse(model.Address.Province.ToString().Replace(" ", "").Replace("-", ""), out Province mappedProvinceVal);

                        if ( clientAddress == null )
                        {

                            clientAddress = new Address()
                            {
                                ObjectId = model.Id,
                                ObjectType = "Client",
                                Town = model.Address.Town,
                                Status = ( int ) Status.Active,
                                PostalCode = model.Address.PostCode,
                                Type = ( int ) model.Address.AddressType,
                                Addressline1 = model.Address.AddressLine1,
                                Addressline2 = model.Address.AddressLine2,
                                Province = ( int ) model.Address.Province,
                                //Province = (int)mappedProvinceVal,
                            };

                            aservice.Create( clientAddress );
                        }
                        else
                        {
                            clientAddress.Town = model.Address.Town;
                            clientAddress.PostalCode = model.Address.PostCode;
                            clientAddress.Type = ( int ) model.Address.AddressType;
                            clientAddress.Addressline1 = model.Address.AddressLine1;
                            clientAddress.Addressline2 = model.Address.AddressLine2;
                            clientAddress.Province = ( int ) model.Address.Province;
                            //clientAddress.Province = (int)mappedProvinceVal;

                            aservice.Update( clientAddress );
                        }
                    }

                    #endregion

                    #region Any Uploads
                    //File Uploads moved to AJAX

                    #endregion

                    #region Any Logos
                    if ( model.Logo.File != null )
                    {
                        //foreach (FileViewModel logo in model.Logo)
                        //{
                        if ( model.Logo.Name != null )
                        {
                            // Create folder
                            string path = Server.MapPath( $"~/{VariableExtension.SystemRules.DocumentsLocation}/Client/{model.Id}/Logo/" );

                            if ( !Directory.Exists( path ) )
                            {
                                Directory.CreateDirectory( path );
                            }

                            string now = DateTime.Now.ToString( "yyyyMMddHHmmss" );

                            Document doc = dservice.GetById( model.Logo.Id );

                            if ( doc != null )
                            {
                                // Disable this file...
                                doc.Status = ( int ) Status.Inactive;

                                dservice.Update( doc );
                            }

                            doc = new Document()
                            {
                                ObjectId = model.Id,
                                ObjectType = "ClientLogo",
                                Status = ( int ) Status.Active,
                                Name = model.Logo.Name,
                                Category = model.Logo.Name,
                                Title = model.Logo.File.FileName,
                                Size = model.Logo.File.ContentLength,
                                Description = "Logo",
                                Type = Path.GetExtension( model.Logo.File.FileName ),
                                Location = $"Client/{model.Id}/Logo/{now}-{model.Logo.File.FileName}"

                            };

                            dservice.Create( doc );

                            string fullpath = Path.Combine( path, $"{now}-{model.Logo.File.FileName}" );
                            model.Logo.File.SaveAs( fullpath );
                        }
                        //}
                    }

                    #endregion

                    #region ClientKPIS
                    //if (model.KPIDisputes > 0)
                    //{
                    //    ClientKPI kpi = kpiservice.GetByColumnWhere("ClientId", model.Id);
                    //    if (kpi != null)
                    //    {
                    //        //foreach (ClientKPI k in kpi) {                            
                    //            kpi.OutstandingPallets = model.KPIOutstanding;
                    //            kpi.Disputes = model.KPIDisputes;
                    //            kpi.OutstandingDays = model.KPIDaysOutstanding;
                    //            kpi.ResolveDays = model.KPIDaysToResolve;

                    //            kpiservice.Update(kpi);
                    //        //}
                    //    }
                    //    else
                    //    {
                    //        ClientKPI newKPI = new ClientKPI()
                    //        {
                    //            ClientId = client.Id,
                    //            Disputes = model.KPIDisputes,
                    //            OutstandingPallets = model.KPIOutstanding,
                    //            Passons = 0, //TODO: Fix
                    //            MonthlyCost = 0, //TODO: Fix
                    //            Status = (int)Status.Active
                    //        };
                    //        kpiservice.Create(newKPI);
                    //    }
                    //}

                    #endregion

                    scope.Complete();
                }

                Notify( "The selected Client details were successfully updated.", NotificationType.Success );

                return RedirectToAction( "ClientList" );
            }
            catch ( Exception ex )
            {
                ViewBag.Message = ex.Message;
                return View();
            }
        }

        // POST: Client/DeleteClient/5
        [HttpPost]
        [Requires( PermissionTo.Delete )]
        public ActionResult DeleteClient( ClientViewModel model )
        {
            Client client;

            //On deleting Client, reset the client session variable so that the client list can load fully
            Session[ "ClientId" ] = null;
            try
            {

                using ( ClientService service = new ClientService() )
                using ( TransactionScope scope = new TransactionScope() )
                {
                    client = service.GetById( model.Id );

                    if ( client == null )
                    {
                        Notify( "Sorry, the requested resource could not be found. Please try again", NotificationType.Error );

                        return PartialView( "_AccessDenied" );
                    }

                    client.Status = ( ( ( Status ) client.Status ) == Status.Active ) ? ( int ) Status.Inactive : ( int ) Status.Active;

                    service.Update( client );
                    scope.Complete();

                }
                Notify( "The selected Client was successfully updated.", NotificationType.Success );
                return RedirectToAction( "ClientList" );
            }
            catch ( Exception ex )
            {
                ViewBag.Message = ex.Message;
                return View();
            }
        }


        #endregion

        #region Manage Sites
        //
        // GET: /Client/ManageSites
        public ActionResult ManageSites( PagingModel pm, CustomSearchModel csm, bool givecsm = false )
        {
            if ( givecsm )
            {
                ViewBag.ViewName = "ManageSites";

                return PartialView( "_ManageSitesCustomSearch", new CustomSearchModel( "ManageSites" ) );
            }
            ViewBag.ViewName = "ManageSites";

            int total = 0;

            List<Site> model = new List<Site>();
            //int pspId = Session[ "UserPSP" ];
            int pspId = ( CurrentUser != null ? CurrentUser.PSPs.FirstOrDefault().Id : 0 );
            string sessClientId = ( Session[ "ClientId" ] != null ? Session[ "ClientId" ].ToString() : null );
            int clientId = ( !string.IsNullOrEmpty( sessClientId ) ? int.Parse( sessClientId ) : 0 );
            ViewBag.ContextualMode = ( clientId > 0 ? true : false ); //Whether a client is specific or not and the View can know about it
            //model.ContextualMode = (clientId > 0 ? true : false); //Whether a client is specific or not and the View can know about it
            //string sessSiteId = (Session["SiteId"] != null ? Session["SiteId"].ToString() : null);
            //int SiteID = (!string.IsNullOrEmpty(sessSiteId) ? int.Parse(sessSiteId) : 0);

            List<Client> clientList = new List<Client>();
            using ( ClientService clientService = new ClientService() )
            {
                clientList = clientService.ListCSM( new PagingModel(), new CustomSearchModel() { ClientId = clientId, Status = Status.Active } );

            }

            IEnumerable<SelectListItem> clientDDL = clientList.Select( c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.CompanyName

            } );
            ViewBag.ClientList = clientDDL;
            //Now get sites for the contextual list
            using ( SiteService service = new SiteService() )
            {
                pm.Sort = pm.Sort ?? "ASC";
                pm.SortBy = pm.SortBy ?? "Name";
                if ( clientId > 0 )
                {
                    csm.ClientId = clientId;
                }
                model = service.ListCSM( pm, csm );
                total = ( model.Count < pm.Take && pm.Skip == 0 ) ? model.Count : service.Total( pm, csm );

            }

            PagingExtension paging = PagingExtension.Create( model, total, pm.Skip, pm.Take, pm.Page );
            List<Region> regionOptions = new List<Region>();
            using ( RegionService rservice = new RegionService() )
            {
                regionOptions = rservice.ListByColumnWhere( "PSPId", pspId );
            }
            ViewBag.RegionOptions = regionOptions;

            return PartialView( "_ManageSites", paging );
        }

        //
        // GET: /Client/SiteDetails/5
        public ActionResult SiteDetails( int id, string sourceView = "ManageSites", bool layout = true )
        {
            Site site;
            //int pspId = Session[ "UserPSP" ];
            int pspId = ( CurrentUser != null ? CurrentUser.PSPs.FirstOrDefault().Id : 0 );
            List<Region> regionOptions = new List<Region>();

            using ( SiteService service = new SiteService() )
            using ( RegionService rservice = new RegionService() )
            using ( AddressService aservice = new AddressService() )
            {
                site = service.GetById( id );
                // regionOptions = rservice.ListByColumnWhere("PSPId", pspId);

                if ( site == null )
                {
                    Notify( "Sorry, the requested resource could not be found. Please try again", NotificationType.Error );

                    return PartialView( "_AccessDenied" );
                }

                Address address = aservice.Get( site.Id, "Site" );


                bool unverified = ( site.Status == ( int ) PSPClientStatus.Unverified );

                Site model = new Site()
                {
                    Id = site.Id,
                    Name = site.Name,
                    Description = site.Description,
                    XCord = site.XCord,
                    YCord = site.YCord,
                    Address = site.Address,
                    PostalCode = site.PostalCode,
                    ContactName = site.ContactName,
                    ContactNo = site.ContactNo,
                    PlanningPoint = site.PlanningPoint,
                    SiteType = ( int ) site.SiteType,
                    AccountCode = site.AccountCode,
                    Depot = site.Depot,
                    SiteCodeChep = site.SiteCodeChep,
                    Status = ( int ) site.Status,
                    // EditMode = true,
                    RegionId = ( site.RegionId != null ? site.RegionId : -1 ),
                    // SourceView = sourceView,
                    //RegionOptions = regionOptions,
                    //FullAddress = new AddressViewModel()
                    //{
                    //    EditMode = true,
                    //    Town = address?.Town,
                    //    Id = address?.Id ?? 0,
                    //    PostCode = address?.PostalCode,
                    //    AddressLine1 = address?.Addressline1,
                    //    AddressLine2 = address?.Addressline2,
                    //    Province = (address != null) ? (Province)address.Province : Province.All,
                    //    AddressType = (address != null) ? (AddressType)address.Type : AddressType.Postal,
                    //}

                };
                if ( layout )
                {
                    ViewBag.IncludeLayout = true;
                }
                ViewBag.Address = address;
                // ViewBag.RegionOptions = regionOptions;
                return View( model );
            }
        }

        // GET: Client/AddSite
        [Requires( PermissionTo.Create )]
        public ActionResult AddSite( string sourceView )
        {

            //int pspId = Session[ "UserPSP" ];
            int pspId = ( CurrentUser != null ? CurrentUser.PSPs.FirstOrDefault().Id : 0 );
            //regionOptions = Model.RegionOptions.Where(r => r.PSPId == pspId).ToList();
            SiteViewModel model = new SiteViewModel() { EditMode = true, SourceView = sourceView };//, RegionOptions = regionOptions
            List<Region> regionOptions = new List<Region>();
            using ( RegionService rservice = new RegionService() )
            {
                regionOptions = rservice.ListByColumnWhere( "PSPId", pspId );

                IEnumerable<SelectListItem> regionDDL = regionOptions.Select( c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Description

                } );
                ViewBag.RegionOptions = regionDDL;

                //if (sourceView == "ManageSites")
                //{
                //    return RedirectToAction("ManageSites");
                //}
                //else
                //{
                //    return RedirectToAction("SubSites");
                //}
                return View( model );
            }
        }


        // POST: Client/Site
        [HttpPost]
        [Requires( PermissionTo.Create )]
        public ActionResult AddSite( SiteViewModel model )
        {
            int pspId = ( CurrentUser != null ? CurrentUser.PSPs.FirstOrDefault().Id : 0 );
            List<Region> regionOptions = new List<Region>();
            using ( RegionService rservice = new RegionService() )
            {
                regionOptions = rservice.ListByColumnWhere( "PSPId", pspId );
            }
            IEnumerable<SelectListItem> regionDDL = regionOptions.Select( c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Description

            } );
            ViewBag.RegionOptions = regionDDL;

            try
            {
                if ( !ModelState.IsValid )
                {
                    Notify( "Sorry, the Site was not created. Please correct all errors and try again.", NotificationType.Error );

                    return View( model );
                }
                //get clientId from session as there has to be one for the add button to have appeared in teh first place, grab it and assign sites to this until it changes

                string sessClientId = ( Session[ "ClientId" ] != null ? Session[ "ClientId" ].ToString() : null );
                int clientID = ( !string.IsNullOrEmpty( sessClientId ) ? int.Parse( sessClientId ) : 0 );
                //for subsites to add subsites under a mainsite
                string sessSiteId = ( Session[ "SiteId" ] != null ? Session[ "SiteId" ].ToString() : null );
                int SiteID = ( !string.IsNullOrEmpty( sessSiteId ) ? int.Parse( sessSiteId ) : 0 );
                int siteType = 1;
                // decimal latitudeY = decimal.Parse(model.YCord);
                //latitudeY = decimal.Round(latitudeY, 4);                
                //decimal longitudeX = decimal.Parse(model.XCord);
                //longitudeX = decimal.Round(longitudeX, 4);
                string strLatY = model.YCord;
                decimal latitudeY = 0;
                try
                {
                    decimal.TryParse( model.YCord, out latitudeY );
                    latitudeY = decimal.Round( latitudeY, 4 );
                    strLatY = ( latitudeY != 0 ? latitudeY.ToString() : strLatY );
                }
                catch ( Exception ex )
                {
                    //ViewBag.Message = string.Concat(rows[3].ToString(), " ", ex.Message);
                }
                string strLngX = model.XCord;
                decimal longitudeX = 0;
                try
                {
                    decimal.TryParse( model.XCord, out longitudeX );
                    longitudeX = decimal.Round( longitudeX, 4 );
                    strLngX = ( longitudeX != 0 ? longitudeX.ToString() : strLngX );
                }
                catch ( Exception ex )
                {
                    // ViewBag.Message = string.Concat(model.XCord, " ", ex.Message);
                }

                using ( SiteService siteService = new SiteService() )
                using ( ClientSiteService csService = new ClientSiteService() )
                using ( TransactionScope scope = new TransactionScope() )
                using ( AddressService aservice = new AddressService() )
                {
                    #region Validation
                    Site existingSite = null;
                    //if (!string.IsNullOrEmpty(model.AccountCode) && siteService.ExistByAccountCode(model.AccountCode.Trim()))
                    if ( !string.IsNullOrEmpty( strLngX ) && siteService.ExistByXYCoords( strLngX, strLatY ) )
                    {
                        //Notify($"Sorry, a Site with the same X Y Coordinates already exists \"{model.XCord}\" already exists!", NotificationType.Error);
                        //return View(model);

                        //rather than pass back to view, we will create the new site as a subsite of the existing site. 
                        //Get the existing site first
                        existingSite = siteService.GetByColumnsWhere( "XCord", strLngX, "YCord", strLatY );
                        SiteID = existingSite.Id;//This is the existing site retrieved by mapping same X and Y coord, read that into the model.SiteId which makes the new site a child site
                        siteType = 2;//Mark teh site as a subsite by default
                    }
                    #endregion

                    #region Create Site
                    Site site = new Site()
                    {
                        Name = model.Name,
                        Description = model.Description,
                        XCord = strLngX,
                        YCord = strLatY,
                        Address = model.FullAddress.AddressLine1 + " " + model.FullAddress.Town + " " + model.FullAddress.PostCode, //model.Address,
                        PostalCode = model.FullAddress.PostCode,
                        ContactName = model.ContactName,
                        ContactNo = model.ContactNo,
                        PlanningPoint = model.PlanningPoint,
                        SiteType = ( model.SiteType > 0 ? ( int ) model.SiteType : siteType ),
                        AccountCode = model.AccountCode,
                        Depot = model.Depot,
                        SiteCodeChep = model.SiteCodeChep,
                        Status = ( int ) Status.Active,
                        RegionId = model.RegionId,
                        FinanceContact = model.FinanceContact,
                        ReceivingContact = model.ReceivingContact,
                        FinanceContactNo = model.FinanceContactNo,
                        ReceivingContactNo = model.ReceivingContactNo,
                    };
                    if ( SiteID > 0 )
                    {
                        site.SiteId = SiteID;
                    }
                    site = siteService.Create( site );
                    #endregion

                    #region Create Address (s)

                    if ( model.FullAddress != null )
                    {
                        Address address = new Address()
                        {
                            ObjectId = site.Id,
                            ObjectType = "Site",
                            Town = model.FullAddress.Town,
                            Status = ( int ) Status.Active,
                            PostalCode = model.FullAddress.PostCode,
                            Type = ( int ) AddressType.Postal,
                            Addressline1 = model.FullAddress.AddressLine1,
                            Addressline2 = model.FullAddress.AddressLine2,
                            Province = ( int ) model.FullAddress.Province,
                            Longitude = strLngX,
                            Latitude = strLatY
                        };
                        aservice.Create( address );
                    }

                    #endregion

                    //tie Client in Session to New Site
                    #region Add ClientSite
                    ClientSite csSite = new ClientSite()
                    {
                        ClientId = clientID,
                        SiteId = site.Id,
                        AccountingCode = site.AccountCode,
                        Status = ( int ) Status.Active
                    };
                    csService.Create( csSite );
                    #endregion


                    scope.Complete();
                }

                Notify( "The Site was successfully created.", NotificationType.Success );
                if ( model.SourceView == "ManageSites" )
                {
                    return RedirectToAction( "ManageSites" );
                }
                else
                {
                    return RedirectToAction( "SubSites" );
                }
            }
            catch ( Exception ex )
            {
                ViewBag.Message = ex.Message;
                return View();
            }
        }



        // GET: Client/EditSite/5
        [Requires( PermissionTo.Edit )]
        public ActionResult EditSite( int id ) //, string sourceView
        {
            Site site;
            //int pspId = Session[ "UserPSP" ];
            //bool layout = true; //temporarily while i figure out why the table edit doesnt work
            using ( SiteService service = new SiteService() )
            using ( RegionService rservice = new RegionService() )
            using ( AddressService aservice = new AddressService() )
            {
                site = service.GetById( id );
                //regionOptions = rservice.ListByColumnWhere("PSPId", pspId);

                if ( site == null )
                {
                    Notify( "Sorry, the requested resource could not be found. Please try again", NotificationType.Error );

                    return PartialView( "_AccessDenied" );
                }

                Address address = aservice.Get( site.Id, "Site" );

                bool unverified = ( site.Status == ( int ) PSPClientStatus.Unverified );

                SiteViewModel model = new SiteViewModel()
                {
                    Id = site.Id,
                    Name = site.Name,
                    Description = site.Description,
                    XCord = site.XCord,
                    YCord = site.YCord,
                    Address = site.Address,
                    PostalCode = site.PostalCode,
                    ContactName = site.ContactName,
                    ContactNo = site.ContactNo,
                    PlanningPoint = site.PlanningPoint,
                    SiteType = ( site.SiteType != null ? ( int ) site.SiteType : ( int ) SiteType.Depot ),
                    AccountCode = site.AccountCode,
                    Depot = site.Depot,
                    SiteCodeChep = site.SiteCodeChep,
                    Status = ( int ) site.Status,
                    FinanceContact = site.FinanceContact,
                    FinanceContactNo = site.FinanceContactNo,
                    ReceivingContact = site.ReceivingContact,
                    ReceivingContactNo = site.ReceivingContactNo,
                    EditMode = true,
                    RegionId = ( site.RegionId.HasValue ? site.RegionId : null ),
                    //RegionOptions = regionOptions,
                    SourceView = "ManageSites",
                };
                if ( address != null )
                {
                    model.FullAddress = new AddressViewModel()
                    {
                        EditMode = true,
                        Town = address?.Town,
                        Id = address?.Id ?? 0,
                        PostCode = address?.PostalCode,
                        AddressLine1 = address?.Addressline1,
                        AddressLine2 = address?.Addressline2,
                        Province = ( address != null ) ? ( Province ) address.Province : Province.All,
                        AddressType = ( address != null ) ? ( AddressType ) address.Type : AddressType.Postal,
                        Longitude = address.Longitude,
                        Latitude = address.Latitude
                    };
                }

                //SiteViewModel model = new SiteViewModel() { EditMode = true, SourceView = sourceView };//, RegionOptions = regionOptions
                //int pspId = Session[ "UserPSP" ];
                int pspId = ( CurrentUser != null ? CurrentUser.PSPs.FirstOrDefault().Id : 0 );
                List<Region> regionOptions = new List<Region>();

                regionOptions = rservice.ListByColumnWhere( "PSPId", pspId );

                IEnumerable<SelectListItem> regionDDL = regionOptions.Select( c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Description

                } );
                ViewBag.RegionOptions = regionDDL;
                //if (sourceView == "ManageSites")
                //{
                //    return RedirectToAction("ManageSites");
                //}
                //else
                //{
                //    return RedirectToAction("SubSites");
                //}
                //if (layout)
                //{
                //    ViewBag.IncludeLayout = true;
                //}
                return View( model );
            }
        }

        // POST: Client/EditSite/5
        [HttpPost]
        [Requires( PermissionTo.Edit )]
        public ActionResult EditSite( SiteViewModel model, PagingModel pm, bool isstructure = false )
        {

            int pspId = ( CurrentUser != null ? CurrentUser.PSPs.FirstOrDefault().Id : 0 );
            List<Region> regionOptions = new List<Region>();
            using ( RegionService rservice = new RegionService() )
            {
                regionOptions = rservice.ListByColumnWhere( "PSPId", pspId );
            }
            IEnumerable<SelectListItem> regionDDL = regionOptions.Select( c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Description

            } );
            ViewBag.RegionOptions = regionDDL;

            try
            {
                if ( !ModelState.IsValid )
                {
                    Notify( "Sorry, the selected Site was not updated. Please correct all errors and try again.", NotificationType.Error );

                    return View( model );
                }

                Site site;

                using ( SiteService service = new SiteService() )
                using ( AddressService aservice = new AddressService() )
                using ( TransactionScope scope = new TransactionScope() )
                {
                    site = service.GetById( model.Id );
                    Address address = aservice.Get( model.Id, "Client" );

                    #region Validations

                    //if (!string.IsNullOrEmpty(model.AccountCode) && service.ExistByAccountCode(model.AccountCode.Trim()))
                    //{
                    //    // Role already exist!
                    //    Notify($"Sorry, a Site with the Account Code \"{model.AccountCode} ({model.AccountCode})\" already exists!", NotificationType.Error);

                    //    return View(model);
                    //}
                    #endregion

                    //stuff changed, lets redo this
                    string strLatY = model.YCord;
                    string strLngX = model.XCord;

                    if ( site.XCord != strLngX )
                    {
                        decimal latitudeY = 0;
                        try
                        {
                            decimal.TryParse( model.YCord, out latitudeY );
                            latitudeY = decimal.Round( latitudeY, 4 );
                            strLatY = ( latitudeY != 0 ? latitudeY.ToString() : strLatY );
                        }
                        catch ( Exception ex )
                        {
                            //ViewBag.Message = string.Concat(rows[3].ToString(), " ", ex.Message);
                        }

                        decimal longitudeX = 0;
                        try
                        {
                            decimal.TryParse( model.XCord, out longitudeX );
                            longitudeX = decimal.Round( longitudeX, 4 );
                            strLngX = ( longitudeX != 0 ? longitudeX.ToString() : strLngX );
                        }
                        catch ( Exception ex )
                        {
                            // ViewBag.Message = string.Concat(model.XCord, " ", ex.Message);
                        }

                    }

                    #region Create Address (s)
                    string stringAddress = "";
                    if ( model.FullAddress != null )
                    {
                        Address siteAddress = aservice.GetById( model.FullAddress.Id );

                        if ( siteAddress == null )
                        {
                            siteAddress = new Address()
                            {
                                ObjectId = model.Id,
                                ObjectType = "Site",
                                Town = model.FullAddress.Town,
                                Status = ( int ) Status.Active,
                                PostalCode = model.FullAddress.PostCode,
                                Type = ( int ) model.FullAddress.AddressType,
                                Addressline1 = model.FullAddress.AddressLine1,
                                Addressline2 = model.FullAddress.AddressLine2,
                                Province = ( int ) model.FullAddress.Province,
                                Longitude = strLngX,
                                Latitude = strLatY
                            };

                            aservice.Create( siteAddress );
                        }
                        else
                        {
                            siteAddress.Town = model.FullAddress.Town;
                            siteAddress.PostalCode = model.FullAddress.PostCode;
                            siteAddress.Type = ( int ) model.FullAddress.AddressType;
                            siteAddress.Addressline1 = model.FullAddress.AddressLine1;
                            siteAddress.Addressline2 = model.FullAddress.AddressLine2;
                            siteAddress.Province = ( int ) model.FullAddress.Province;
                            siteAddress.Longitude = strLngX;
                            siteAddress.Latitude = strLatY;

                            aservice.Update( siteAddress );
                        }
                        stringAddress = model.FullAddress.AddressLine1 + " " + model.FullAddress.Town + " " + model.FullAddress.PostCode;
                    }
                    #endregion

                    #region Update Site
                    // Update Site
                    site.Id = model.Id;
                    site.Name = model.Name;
                    site.Description = model.Description;
                    site.XCord = strLngX;
                    site.YCord = strLatY;
                    site.Address = stringAddress;
                    site.PostalCode = model.FullAddress.PostCode;
                    site.ContactName = model.ContactName;
                    site.ContactNo = model.ContactNo;
                    site.PlanningPoint = model.PlanningPoint;
                    site.SiteType = ( int ) model.SiteType;
                    site.AccountCode = model.AccountCode;
                    site.Depot = model.Depot;
                    site.SiteCodeChep = model.SiteCodeChep;
                    site.Status = ( int ) model.Status;
                    site.RegionId = model.RegionId;
                    site.FinanceContact = model.FinanceContact;
                    site.ReceivingContact = model.ReceivingContact;
                    site.FinanceContactNo = model.FinanceContactNo;
                    site.ReceivingContactNo = model.ReceivingContactNo;

                    service.Update( site );

                    #endregion                  
                    scope.Complete();
                }

                Notify( "The selected Site details were successfully updated.", NotificationType.Success );

                return RedirectToAction( "ManageSites" );
            }
            catch ( Exception ex )
            {
                ViewBag.Message = ex.Message;
                return View();
            }
        }

        // POST: Client/DeleteSite/5
        [HttpPost]
        [Requires( PermissionTo.Delete )]
        public ActionResult DeleteSite( SiteViewModel model )
        {
            Site site;
            try
            {

                using ( SiteService service = new SiteService() )
                using ( TransactionScope scope = new TransactionScope() )
                {
                    site = service.GetById( model.Id );

                    if ( site == null )
                    {
                        Notify( "Sorry, the requested resource could not be found. Please try again", NotificationType.Error );

                        return PartialView( "_AccessDenied" );
                    }

                    site.Status = ( ( ( Status ) site.Status ) == Status.Active ) ? ( int ) Status.Inactive : ( int ) Status.Active;

                    service.Update( site );
                    scope.Complete();

                }
                Notify( "The selected Client was successfully updated.", NotificationType.Success );
                return RedirectToAction( "ManageSites" );
            }
            catch ( Exception ex )
            {
                ViewBag.Message = ex.Message;
                return View();
            }
        }


        [AcceptVerbs( HttpVerbs.Get | HttpVerbs.Post )]
        public JsonResult GetSitesForClient( string clientId )
        {
            if ( !string.IsNullOrEmpty( clientId ) )
            {
                Session[ "ClientId" ] = clientId;//wroite to session to use for adds and updates
                List<Site> sites = new List<Site>();
                //int pspId = Session[ "UserPSP" ];
                //int pspId = (CurrentUser != null ? CurrentUser.PSPs.FirstOrDefault().Id : 0);

                using ( SiteService service = new SiteService() )
                using ( RegionService regionservice = new RegionService() )
                {
                    List<Region> regions = regionservice.List( new PagingModel(), new CustomSearchModel() );
                    sites = service.GetSitesByClientIncluded( int.Parse( clientId ) ); //GetClientsByPSPIncludedGroup(pspId, int.Parse(groupId));
                    foreach ( Site site in sites )
                    {
                        if ( site.RegionId != null )
                        {
                            site.Region = regions.FirstOrDefault( r => r.Id == site.RegionId );
                        }
                        else
                        {
                            site.Region = null;
                        }
                    }
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

        // POST || GET: /Client/GetSiteList
        public ActionResult GetSiteList( Nullable<int> siteId )
        {



            return PartialView( "partials/_ManageSitesList" );
        }

        #endregion

        #region Sub Sites
        //
        // POST || GET: /Client/SubSites
        public ActionResult SubSites( int siteId = 0 )
        {

            ViewBag.ViewName = "_SubSites";

            int total = 0;

            List<Site> model = new List<Site>();
            List<Client> clientList = new List<Client>();
            List<Site> mainSiteList;

            //int pspId = Session[ "UserPSP" ];
            int pspId = ( CurrentUser != null ? CurrentUser.PSPs.FirstOrDefault().Id : 0 );
            string sessClientId = ( Session[ "ClientId" ] != null ? Session[ "ClientId" ].ToString() : null );
            int clientId = ( !string.IsNullOrEmpty( sessClientId ) ? int.Parse( sessClientId ) : 0 );

            string sessSiteId = ( Session[ "SiteId" ] != null ? Session[ "SiteId" ].ToString() : null );
            siteId = ( !string.IsNullOrEmpty( sessSiteId ) ? int.Parse( sessSiteId ) : 0 );

            //check if site is in session but client is not, then retrieve clientid to pass through as context
            if ( clientId == 0 && siteId > 0 )
            {
                if ( siteId > 0 )
                {
                    using ( SiteService service = new SiteService() )
                    {
                        clientId = service.GetClientBySite( siteId );
                    }
                    //Session["ClientId"] = clientId;
                }
            }

            ViewBag.ContextualMode = ( clientId > 0 ? true : false ); //Whether a client is specific or not and the View can know about it
            //model.ContextualMode = (clientId > 0 ? true : false); //Whether a client is specific or not and the View can know about it
            using ( ClientService clientService = new ClientService() )
            {
                clientList = clientService.ListCSM( new PagingModel(), new CustomSearchModel() { ClientId = clientId, Status = Status.Active } );
            }
            IEnumerable<SelectListItem> clientDDL = clientList.Select( c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.CompanyName

            } );
            ViewBag.ClientList = clientDDL;

            ViewBag.ContextualModeSite = ( siteId > 0 ? true : false ); //Whether a client is specific or not and the View can know about it

            using ( SiteService service = new SiteService() )
            {
                mainSiteList = service.ListCSM( new PagingModel() { Sort = "ASC", SortBy = "Name" }, new CustomSearchModel() { SiteId = siteId, ClientId = clientId } );

            }
            IEnumerable<SelectListItem> siteDDL = mainSiteList.Select( c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Name

            } );
            ViewBag.ClientSiteList = siteDDL;
            ViewBag.SelectedSiteId = siteId;


            return PartialView( "_SubSites" );
        }

        [AcceptVerbs( HttpVerbs.Get | HttpVerbs.Post )]
        public JsonResult GetSiteListForClient( string clientId )
        {
            if ( clientId != null && clientId != "" )
            {
                Session[ "ClientId" ] = clientId;
                List<Site> sites = null;

                //int pspId = Session[ "UserPSP" ];
                //int pspId = (CurrentUser != null ? CurrentUser.PSPs.FirstOrDefault().Id : 0);
                string sessSiteId = ( Session[ "SiteId" ] != null ? Session[ "SiteId" ].ToString() : null );
                int siteId = ( !string.IsNullOrEmpty( sessSiteId ) ? int.Parse( sessSiteId ) : 0 );

                using ( SiteService service = new SiteService() )
                using ( ClientSiteService cservice = new ClientSiteService() )
                {
                    if ( siteId > 0 )
                        sites = service.GetSitesByClients( int.Parse( clientId ), siteId );
                    else
                        sites = service.GetSitesByClients( int.Parse( clientId ) );

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
        public JsonResult GetSitesForClientSiteIncluded( string clientId, string siteId )
        {
            if ( clientId != null && clientId != "" )
            {
                string sessClientId = ( Session[ "ClientId" ] != null ? Session[ "ClientId" ].ToString() : null );
                int currentClientId = ( !string.IsNullOrEmpty( clientId ) ? int.Parse( clientId ) : ( !string.IsNullOrEmpty( sessClientId ) ? int.Parse( sessClientId ) : 0 ) );

                Session[ "ClientId" ] = clientId;
                string sessSiteId = ( Session[ "SiteId" ] != null ? Session[ "SiteId" ].ToString() : null );
                int currentSiteId = ( !string.IsNullOrEmpty( siteId ) ? int.Parse( siteId ) : ( !string.IsNullOrEmpty( sessSiteId ) ? int.Parse( sessSiteId ) : 0 ) );
                Session[ "SiteId" ] = currentSiteId;
                List<Site> sites = null;
                //int pspId = Session[ "UserPSP" ];
                // int pspId = (CurrentUser != null ? CurrentUser.PSPs.FirstOrDefault().Id : 0);

                using ( SiteService service = new SiteService() )
                {

                    sites = service.GetSitesByClientsIncluded( currentClientId, currentSiteId ); //GetClientsByPSPIncludedGroup(pspId, int.Parse(groupId));

                }
                //var jsonList = JsonConvert.SerializeObject(sites);
                var data = JsonConvert.SerializeObject( sites, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore } );
                return Json( new { data = data }, JsonRequestBehavior.AllowGet );
            }
            else
            {
                return Json( data: "Error", behavior: JsonRequestBehavior.AllowGet );
            }
        }

        [AcceptVerbs( HttpVerbs.Get | HttpVerbs.Post )]
        public JsonResult GetSitesForClientSiteExcluded( string clientId, string siteId )
        {
            if ( !string.IsNullOrEmpty( clientId ) && !string.IsNullOrEmpty( siteId ) )
            {
                string sessClientId = ( Session[ "ClientId" ] != null ? Session[ "ClientId" ].ToString() : null );
                int currentClientId = ( !string.IsNullOrEmpty( clientId ) ? int.Parse( clientId ) : ( !string.IsNullOrEmpty( sessClientId ) ? int.Parse( sessClientId ) : 0 ) );

                Session[ "ClientId" ] = clientId;
                string sessSiteId = ( Session[ "SiteId" ] != null ? Session[ "SiteId" ].ToString() : null );
                int currentSiteId = ( !string.IsNullOrEmpty( siteId ) ? int.Parse( siteId ) : ( !string.IsNullOrEmpty( sessSiteId ) ? int.Parse( sessSiteId ) : 0 ) );
                Session[ "SiteId" ] = currentSiteId;
                List<Site> sites = null;
                //int pspId = Session[ "UserPSP" ];
                //int pspId = (CurrentUser != null ? CurrentUser.PSPs.FirstOrDefault().Id : 0);

                using ( SiteService service = new SiteService() )
                {

                    sites = service.GetSitesByClientsExcluded( currentClientId, currentSiteId ); //GetClientsByPSPIncludedGroup(pspId, int.Parse(groupId));

                }
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
        public JsonResult GetSiteDetailsForMain( string siteId )
        {
            if ( siteId != null && siteId != "" )
            {
                //Session["ClientId"] = clientId;
                // Session["SiteId"] = siteId;
                Site site = null;
                using ( SiteService service = new SiteService() )
                {
                    site = service.GetById( int.Parse( siteId ) );
                }
                //return Json(site, JsonRequestBehavior.AllowGet);
                var data = JsonConvert.SerializeObject( site, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore } );
                return Json( new { data = data }, JsonRequestBehavior.AllowGet );
            }
            else
            {
                return Json( data: "Error", behavior: JsonRequestBehavior.AllowGet );
            }
        }


        [AcceptVerbs( HttpVerbs.Get | HttpVerbs.Post )]
        public JsonResult SetSiteForClientSiteExcluded( string mainSiteId, string movedSiteId, string clientId )
        {
            if ( !string.IsNullOrEmpty( mainSiteId ) && !string.IsNullOrEmpty( movedSiteId ) && !string.IsNullOrEmpty( clientId ) )
            {
                //Session["ClientId"] = clientId;
                Session[ "SiteId" ] = mainSiteId;
                //using (GroupService service = new GroupService())
                //using (ClientSiteService clientservice = new ClientSiteService())
                using ( SiteService sservice = new SiteService() )
                using ( TransactionScope scope = new TransactionScope() )
                {
                    Site currentSite = sservice.GetById( int.Parse( movedSiteId ) );
                    currentSite.SiteId = null;
                    sservice.Update( currentSite );

                    //ClientSite site = new ClientSite()
                    //{
                    //    Site
                    //    ClientId = int.Parse(clientId),
                    //    Status = (int)Status.Active
                    //};
                    //clientgroupservice.Create(group);

                    scope.Complete();
                }


                return Json( data: "True", behavior: JsonRequestBehavior.AllowGet );
            }
            else
            {
                return Json( data: "False", behavior: JsonRequestBehavior.AllowGet );
            }
        }

        [AcceptVerbs( HttpVerbs.Get | HttpVerbs.Post )]
        public JsonResult SetSiteForClientSiteIncluded( string mainSiteId, string movedSiteId, string clientId )
        {
            if ( !string.IsNullOrEmpty( mainSiteId ) && !string.IsNullOrEmpty( movedSiteId ) && !string.IsNullOrEmpty( clientId ) )
            {
                //Session["ClientId"] = clientId;
                Session[ "SiteId" ] = mainSiteId;
                //using (GroupService service = new GroupService())
                //using (ClientSiteService clientservice = new ClientSiteService())
                using ( SiteService sservice = new SiteService() )
                using ( TransactionScope scope = new TransactionScope() )
                {
                    Site currentSite = sservice.GetById( int.Parse( movedSiteId ) );
                    currentSite.SiteId = int.Parse( mainSiteId );
                    sservice.Update( currentSite );

                    //ClientSite site = new ClientSite()
                    //{
                    //    Site
                    //    ClientId = int.Parse(clientId),
                    //    Status = (int)Status.Active
                    //};
                    //clientgroupservice.Create(group);

                    scope.Complete();
                }


                return Json( data: "True", behavior: JsonRequestBehavior.AllowGet );
            }
            else
            {
                return Json( data: "False", behavior: JsonRequestBehavior.AllowGet );
            }
        }



        #endregion

        #region Client Group
        //
        // GET: /Client/ClientGroups`
        public ActionResult ClientGroups( PagingModel pm, CustomSearchModel csm, bool givecsm = false )
        {
            ViewBag.ViewName = "ClientGroups";
            if ( givecsm )
            {
                ViewBag.ViewName = "ClientGroups";

                return PartialView( "_ClientGroupsCustomSearch", new CustomSearchModel( "ClientGroups" ) );
            }
            int total = 0;

            List<Group> model = new List<Group>();
            //int pspId = Session[ "UserPSP" ];
            int pspId = ( CurrentUser != null ? CurrentUser.PSPs.FirstOrDefault().Id : 0 );
            string sessClientId = ( Session[ "ClientId" ] != null ? Session[ "ClientId" ].ToString() : null );
            int clientId = ( !string.IsNullOrEmpty( sessClientId ) ? int.Parse( sessClientId ) : 0 );
            //get group list, and their associated clients. TRhis woill be extended with an api call to get clients included and excluded as the button is clicked, and as the groups are changed
            using ( ClientService clientService = new ClientService() )
            //using (ClientGroupService clientGroupService = new ClientGroupService())
            using ( GroupService groupService = new GroupService() )
            {
                pm.Sort = pm.Sort ?? "DESC";
                pm.SortBy = pm.SortBy ?? "Name";

                model = groupService.GetGroupsByPSP( pspId, new CustomSearchModel() { ClientId = clientId } );
                total = ( model.Count < pm.Take && pm.Skip == 0 ) ? model.Count : groupService.Total();

                //get the specific list of clients that exists for the first group, to render the tables, will use an api call to change it accordingly after reselection
                //Group clientGroup = groupService.GetGroupsByPSP(pspId).FirstOrDefault();
                //if (clientGroup != null)
                //{
                //    ViewBag.ClientListIncluded = clientService.GetClientsByPSPIncludedGroup(pspId, clientGroup.Id);
                //    ViewBag.ClientListExcluded = clientService.GetClientsByPSPExcludedGroup(pspId, clientGroup.Id);
                //    //ViewBag.GroupData = groupService.GetGroupsByPSP(pspId);
                //}
            }
            PagingExtension paging = PagingExtension.Create( model, total, pm.Skip, pm.Take, pm.Page );

            return PartialView( "_ClientGroups", paging );
        }


        // GET: Client/AddGroup
        [Requires( PermissionTo.Create )]
        public ActionResult AddGroup()
        {
            GroupViewModel model = new GroupViewModel() { EditMode = true };
            int pspId = ( CurrentUser != null ? CurrentUser.PSPs.FirstOrDefault().Id : 0 );
            string sessClientId = ( Session[ "ClientId" ] != null ? Session[ "ClientId" ].ToString() : null );
            int clientId = ( !string.IsNullOrEmpty( sessClientId ) ? int.Parse( sessClientId ) : 0 );
            ViewBag.ContextualMode = ( clientId > 0 ? true : false ); //Whether a client is specific or not and the View can know about it
            model.ContextualMode = ( clientId > 0 ? true : false ); //Whether a client is specific or not and the View can know about it
            List<Client> clientList = new List<Client>();
            //TODO
            using ( ClientService clientService = new ClientService() )
            {
                clientList = clientService.ListCSM( new PagingModel(), new CustomSearchModel() { ClientId = clientId, Status = Status.Active } );
            }

            IEnumerable<SelectListItem> clientDDL = clientList.Select( c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.CompanyName

            } );
            ViewBag.ClientList = clientDDL;


            return View( model );
        }


        // POST: Client/AddGroup
        [HttpPost]
        [Requires( PermissionTo.Create )]
        public ActionResult AddGroup( GroupViewModel model )
        {
            try
            {

                if ( !ModelState.IsValid )
                {
                    Notify( "Sorry, the Group was not created. Please correct all errors and try again.", NotificationType.Error );

                    return View( model );
                }

                using ( GroupService gService = new GroupService() )
                using ( ClientGroupService cgservice = new ClientGroupService() )
                using ( TransactionScope scope = new TransactionScope() )
                {
                    #region Create Group
                    Group group = new Group()
                    {
                        Name = model.Name,
                        Description = model.Description,
                        Status = ( int ) Status.Active
                    };
                    group = gService.Create( group );
                    #endregion

                    #region Create Group Client Links
                    if ( !string.IsNullOrEmpty( model.GroupClientList ) )
                    {
                        String[] clientList = model.GroupClientList.Split( ',' );
                        string lastId = "0";
                        foreach ( string itm in clientList )
                        {
                            //test to see if its not been added before
                            ClientGroup checkCG = cgservice.GetByColumnsWhere( "ClientId", int.Parse( itm ), "GroupId", group.Id );

                            if ( !string.IsNullOrEmpty( itm ) && itm != lastId && checkCG == null )
                            {
                                ClientGroup client = new ClientGroup()
                                {
                                    ClientId = int.Parse( itm ),
                                    GroupId = group.Id,
                                    Status = ( int ) Status.Active
                                };
                                cgservice.Create( client );
                            }
                            lastId = itm;
                        }
                    }
                    #endregion

                    scope.Complete();
                }

                Notify( "The Group was successfully created.", NotificationType.Success );
                return RedirectToAction( "ClientGroups" );
            }
            catch ( Exception ex )
            {
                ViewBag.Message = ex.Message;
                return View();
            }
        }


        // GET: Client/EditGroupGet/5
        [Requires( PermissionTo.Edit )]
        public ActionResult EditGroupGet( int id )
        {
            Group group;

            using ( GroupService service = new GroupService() )
            {
                group = service.GetById( id );


                if ( group == null )
                {
                    Notify( "Sorry, the requested resource could not be found. Please try again", NotificationType.Error );

                    return PartialView( "_AccessDenied" );
                }

                GroupViewModel model = new GroupViewModel()
                {
                    Id = group.Id,
                    Name = group.Name,
                    Description = group.Description,
                    Status = ( int ) group.Status,
                    EditMode = true
                };
                return View( "EditGroup", model );
            }
        }

        // POST: Client/EditGroup/5
        [Requires( PermissionTo.Edit )]
        public ActionResult EditGroup( GroupViewModel model, PagingModel pm, bool isstructure = false )
        {
            try
            {
                if ( !ModelState.IsValid )
                {
                    Notify( "Sorry, the selected Group was not updated. Please correct all errors and try again.", NotificationType.Error );

                    return View( model );
                }

                Group group;

                using ( GroupService service = new GroupService() )
                using ( TransactionScope scope = new TransactionScope() )
                {
                    group = service.GetById( model.Id );

                    #region Update Group

                    // Update Group
                    //group.Id = model.Id;
                    group.Name = model.Name;
                    group.Description = model.Description;
                    group.Status = ( int ) model.Status;

                    service.Update( group );

                    #endregion


                    scope.Complete();
                }

                Notify( "The selected Group details were successfully updated.", NotificationType.Success );

                return RedirectToAction( "ClientGroups" );
            }
            catch ( Exception ex )
            {
                ViewBag.Message = ex.Message;
                return View();
            }
        }

        // POST: Client/DeleteGroup/5
        [HttpPost]
        [Requires( PermissionTo.Delete )]
        public ActionResult DeleteGroup( GroupViewModel model )
        {
            Group group;
            ClientGroup clientGroup;
            try
            {

                using ( GroupService service = new GroupService() )
                // using (ClientGroupService clientgroupservice = new ClientGroupService())
                using ( TransactionScope scope = new TransactionScope() )
                {
                    group = service.GetById( model.Id );

                    if ( group == null )
                    {
                        Notify( "Sorry, the requested resource could not be found. Please try again", NotificationType.Error );

                        return PartialView( "_AccessDenied" );
                    }

                    group.Status = ( ( ( Status ) group.Status ) == Status.Active ) ? ( int ) Status.Inactive : ( int ) Status.Active;

                    //clientGroup = clientgroupservice.GetById(model.Id);
                    //clientGroup.Status = (((Status)group.Status) == Status.Active) ? (int)Status.Inactive : (int)Status.Active;                    

                    service.Update( group );
                    // clientgroupservice.Update(clientGroup);
                    scope.Complete();

                }
                Notify( "The selected Group was successfully updated.", NotificationType.Success );
                return RedirectToAction( "ClientGroups" );
            }
            catch ( Exception ex )
            {
                ViewBag.Message = ex.Message;
                return View();
            }
        }



        [AcceptVerbs( HttpVerbs.Get | HttpVerbs.Post )]
        public JsonResult GetClientsForGroupIncluded( string groupId )
        {
            if ( groupId != null && groupId != "" )
            {
                List<Client> clients;
                //int pspId = Session[ "UserPSP" ];
                int pspId = ( CurrentUser != null ? CurrentUser.PSPs.FirstOrDefault().Id : 0 );
                string sessClientId = ( Session[ "ClientId" ] != null ? Session[ "ClientId" ].ToString() : null );
                int clientId = ( !string.IsNullOrEmpty( sessClientId ) ? int.Parse( sessClientId ) : 0 );
                using ( ClientService clientService = new ClientService() )
                {

                    clients = clientService.GetClientsByPSPIncludedGroup( pspId, int.Parse( groupId ), new CustomSearchModel() { ClientId = clientId, Status = Status.Active } );

                }
                return Json( clients, JsonRequestBehavior.AllowGet );
            }
            else
            {
                return Json( data: "Error", behavior: JsonRequestBehavior.AllowGet );
            }
        }

        [AcceptVerbs( HttpVerbs.Get | HttpVerbs.Post )]
        public JsonResult GetClientsForGroupExcluded( string groupId )
        {
            if ( groupId != null && groupId != "" )
            {
                List<Client> clients;
                //int pspId = Session[ "UserPSP" ];
                int pspId = ( CurrentUser != null ? CurrentUser.PSPs.FirstOrDefault().Id : 0 );
                string sessClientId = ( Session[ "ClientId" ] != null ? Session[ "ClientId" ].ToString() : null );
                int clientId = ( !string.IsNullOrEmpty( sessClientId ) ? int.Parse( sessClientId ) : 0 );

                using ( ClientService clientService = new ClientService() )
                {

                    clients = clientService.GetClientsByPSPExcludedGroup( pspId, int.Parse( groupId ), new CustomSearchModel() { ClientId = clientId, Status = Status.Active } );

                }
                return Json( clients, JsonRequestBehavior.AllowGet );
            }
            else
            {
                return Json( data: "Error", behavior: JsonRequestBehavior.AllowGet );
            }
        }

        [AcceptVerbs( HttpVerbs.Get | HttpVerbs.Post )]
        public JsonResult SetClientForGroupExcluded( string groupId, string clientId )
        {
            if ( !string.IsNullOrEmpty( groupId ) && !string.IsNullOrEmpty( clientId ) )
            {
                //using (GroupService service = new GroupService())
                using ( ClientGroupService clientgroupservice = new ClientGroupService() )
                using ( TransactionScope scope = new TransactionScope() )
                {
                    List<ClientGroup> group = new List<ClientGroup>();
                    group = clientgroupservice.GetClientGroupsByClientGroup( int.Parse( groupId ), int.Parse( clientId ) );

                    if ( group == null )
                    {
                        return Json( data: "False", behavior: JsonRequestBehavior.AllowGet );
                    }
                    foreach ( ClientGroup g in group )
                    {
                        clientgroupservice.Delete( g );
                    }
                    scope.Complete();
                }


                return Json( data: "True", behavior: JsonRequestBehavior.AllowGet );
            }
            else
            {
                return Json( data: "False", behavior: JsonRequestBehavior.AllowGet );
            }
        }

        [AcceptVerbs( HttpVerbs.Get | HttpVerbs.Post )]
        public JsonResult SetClientForGroupIncluded( string groupId, string clientId )
        {
            if ( !string.IsNullOrEmpty( groupId ) && !string.IsNullOrEmpty( clientId ) )
            {
                //using (GroupService service = new GroupService())
                using ( ClientGroupService clientgroupservice = new ClientGroupService() )
                using ( GroupService groupservice = new GroupService() )
                using ( ClientService clientService = new ClientService() )
                using ( TransactionScope scope = new TransactionScope() )
                {
                    ClientGroup checkCG = clientgroupservice.GetByColumnsWhere( groupId, clientId );//check this link doesnt already exist, ignore if it does
                    //Group groupObj = groupservice.GetById(int.Parse(groupId));
                    if ( checkCG == null )
                    {
                        ClientGroup cgroup = new ClientGroup()
                        {
                            GroupId = int.Parse( groupId ),
                            ClientId = int.Parse( clientId ),
                            Status = ( int ) Status.Active,
                        };
                        clientgroupservice.Create( cgroup );

                        scope.Complete();
                    } //nothing to do here if the group is already linekd  to the same client
                    else
                    {
                        //just update status to make sure its visible and active, maybe it was disabled
                        checkCG.Status = ( int ) Status.Active;
                        clientgroupservice.Update( checkCG );
                    }
                }


            }
            return Json( data: "True", behavior: JsonRequestBehavior.AllowGet );
        }

        #endregion


        #region Products

        [AcceptVerbs( HttpVerbs.Get | HttpVerbs.Post )]
        public JsonResult LinkClientToProduct( string client, string product )
        {
            if ( !string.IsNullOrEmpty( client ) && !string.IsNullOrEmpty( product ) )
            {
                int clientId, productId = 0;
                int.TryParse( client, out clientId );
                int.TryParse( product, out productId );

                if ( clientId > 0 )
                {
                    Session[ "ClientId" ] = clientId; //set client to session for the client context after reload of screen
                    using ( ProductService service = new ProductService() )
                    using ( ClientProductService cpservice = new ClientProductService() )
                    using ( TransactionScope scope = new TransactionScope() )
                    {

                        ProductCustomModel pcm = service.ListCSM( new PagingModel(), new CustomSearchModel() ).FirstOrDefault( p => p.Id == productId );//get the list and then pick one out based on the productId passed in                        
                        if ( pcm != null )
                        {
                            List<ProductPrice> prices = new List<ProductPrice>();
                            decimal hrate = 0;
                            int? hrateunit = 0;
                            decimal irate = 0;
                            int? irateunit = 0;
                            decimal lrate = 0;
                            int? lrateunit = 0;
                            decimal pocost = 0;
                            int pocostunit = 0;
                            //get the individual prices to copy over
                            using ( ProductPriceService aservice = new ProductPriceService() )
                            {
                                prices = aservice.ListByColumnWhere( "ProductId", productId );
                                if ( prices != null )
                                {
                                    foreach ( ProductPrice price in prices )
                                    {
                                        switch ( price.Type )
                                        {
                                            case ( int ) ProductPriceType.Hire:
                                                hrate = price.Rate;
                                                hrateunit = price.RateUnit;
                                                break;
                                            case ( int ) ProductPriceType.Issue:
                                                irate = price.Rate;
                                                irateunit = price.RateUnit;
                                                break;
                                            case ( int ) ProductPriceType.Lost:
                                                lrate = price.Rate;
                                                lrateunit = price.RateUnit;
                                                break;
                                            default:
                                                break;
                                        }
                                    }
                                }
                            }

                            ClientProduct testcp = cpservice.GetByColumnsWhere( "ProductId", productId, "ClientId", clientId );
                            if ( testcp == null )
                            {
                                //set the client product up
                                ClientProduct cg = new ClientProduct()
                                {
                                    ClientId = clientId,
                                    ProductId = productId,
                                    ActiveDate = DateTime.Now,
                                    ProductDescription = pcm.Description,
                                    HireRate = hrate,
                                    IssueRate = irate,
                                    LostRate = lrate,
                                    PassonDays = 0, //to be edited by client
                                    PassonRate = pocost,
                                    Status = ( int ) Status.Active
                                };
                                cpservice.Create( cg ); //save the clientproduct
                            }
                        }
                        scope.Complete();
                    }
                }
                return Json( data: "True", behavior: JsonRequestBehavior.AllowGet );
            }
            else
            {
                return Json( data: "Error", behavior: JsonRequestBehavior.AllowGet );
            }
        }

        //
        // POST || GET: /Client/LinkProducts
        public ActionResult LinkProducts( PagingModel pm, CustomSearchModel csm, bool givecsm = false )
        {
            if ( givecsm )
            {
                ViewBag.ViewName = "Products";

                return PartialView( "_LinkProductsCustomSearch", new CustomSearchModel( "LinkProducts" ) );
            }

            int total = 0;

            List<ProductCustomModel> model = new List<ProductCustomModel>();
            List<Client> clientList = new List<Client>();
            //int pspId = Session[ "UserPSP" ];
            int pspId = ( CurrentUser != null ? CurrentUser.PSPs.FirstOrDefault().Id : 0 );
            string sessClientId = ( Session[ "ClientId" ] != null ? Session[ "ClientId" ].ToString() : null );
            int clientId = ( !string.IsNullOrEmpty( sessClientId ) ? int.Parse( sessClientId ) : 0 );
            ViewBag.ContextualMode = ( clientId > 0 ? true : false ); //Whether a client is specific or not and the View can know about it
            //model.ContextualMode = (clientId > 0 ? true : false); //Whether a client is specific or not and the View can know about it


            //rebuild the whole object from clientproduct to product custom to fit in the same grid
            if ( clientId > 0 )
            { //if its client specific pull it from the ClientProduct table instead and show whats relevant
                using ( ClientProductService cpservice = new ClientProductService() )
                using ( ClientService clientService = new ClientService() )
                {
                    csm.ClientId = clientId;
                    csm.Status = Status.Active;

                    //List<ProductCustomModel> cpmodel
                    List<ClientProductCustomModel> clientProducts = cpservice.ListCSM( pm, csm );

                    if ( clientProducts != null )
                    {
                        foreach ( ClientProductCustomModel item in clientProducts )
                        {
                            ProductCustomModel prod = new ProductCustomModel()
                            {
                                Name = item.Name,
                                Description = item.ProductDescription,
                                Id = item.Id,
                                Status = item.Status,
                                ProductPriceCount = item.ProductPriceCount,
                                DocumentCount = item.DocumentCount,
                                Documents = item.Documents
                            };
                            model.Add( prod );
                        }
                    }
                    clientList = clientService.ListCSM( new PagingModel(), new CustomSearchModel() { Status = Status.Active, ClientId = clientId } );
                }
            }
            else
            {
                using ( ProductService service = new ProductService() )
                using ( ClientService clientService = new ClientService() )
                {
                    model = service.ListCSM( pm, csm );
                    total = ( model.Count < pm.Take && pm.Skip == 0 ) ? model.Count : service.Total1( pm, csm );
                    clientList = clientService.ListCSM( new PagingModel(), new CustomSearchModel() { Status = Status.Active, ClientId = clientId } );
                }
            }

            PagingExtension paging = PagingExtension.Create( model, total, pm.Skip, pm.Take, pm.Page );

            IEnumerable<SelectListItem> clientDDL = clientList.Select( c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.CompanyName

            } );
            ViewBag.ClientList = clientDDL;

            return PartialView( "_LinkProducts", paging );
        }


        //
        // GET: /Client/ProductDetails/5
        public ActionResult ProductDetails( int id, bool layout = true )
        {
            using ( ProductService pservice = new ProductService() )
            using ( ClientProductService cpservice = new ClientProductService() )
            using ( DocumentService dservice = new DocumentService() )
            {
                ClientProduct model = cpservice.GetById( id );

                if ( model == null )
                {
                    Notify( "Sorry, the requested resource could not be found. Please try again", NotificationType.Error );

                    return RedirectToAction( "Index" );
                }

                if ( layout )
                {
                    ViewBag.IncludeLayout = true;
                }

                List<Document> documents = dservice.List( model.Id, "Product" );

                if ( documents != null )
                {
                    ViewBag.Documents = documents;
                }

                return View( model );
            }
        }

        //
        //// GET: /Client/AddProduct/5
        //[Requires(PermissionTo.Create)]
        //public ActionResult AddProduct()
        //{
        //    ProductViewModel model = new ProductViewModel() { EditMode = true, ProductPrices = new List<ProductPriceViewModel>() };

        //    foreach (int item in Enum.GetValues(typeof(ProductPriceType)))
        //    {
        //        ProductPriceType type = (ProductPriceType)item;

        //        model.ProductPrices.Add(new ProductPriceViewModel()
        //        {
        //            Type = type,
        //            Status = Status.Active
        //        });
        //    }

        //    return View(model);
        //}

        //
        // POST: /Client/AddProduct/5
        //[HttpPost]
        //[Requires(PermissionTo.Create)]
        //public ActionResult AddProduct(ProductViewModel model)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        Notify("Sorry, the Product was not created. Please correct all errors and try again.", NotificationType.Error);

        //        return View(model);
        //    }

        //    ClientProduct product = new ClientProduct();

        //    using (ProductService pservice = new ProductService())
        //    using (TransactionScope scope = new TransactionScope())
        //    using (DocumentService dservice = new DocumentService())
        //    using (ClientProductService pcservice = new ClientProductService())
        //    using (ProductPriceService ppservice = new ProductPriceService())
        //    {
        //        #region Validations

        //        if (pservice.Exist(model.Name))
        //        {
        //            // Product already exist!
        //            Notify($"Sorry, a Product with the Name \"{model.Name}\" already exists!", NotificationType.Error);

        //            return View(model);
        //        }

        //        #endregion

        //        #region Product

        //        //product.Name = model.Name;
        //        product.Status = (int)model.Status;
        //        product.ProductDescription = model.Description;

        //        decimal hrate = 0;
        //        int? hrateunit = 0;
        //        decimal irate = 0;
        //        int? irateunit = 0;
        //        decimal lrate = 0;
        //        int? lrateunit = 0;
        //        decimal pocost = 0;
        //        int pocostunit = 0;

        //        //if (model.ProductPrices.NullableAny())
        //        //{
        //        //    foreach (ProductPriceViewModel price in model.ProductPrices)
        //        //    {
        //        //        ProductPrice pp = new ProductPrice()
        //        //        {
        //        //            ProductId = product.Id,
        //        //            Rate = price.Rate ?? 0,
        //        //            Type = (int)price.Type,
        //        //            RateUnit = price.RateUnit,
        //        //            FromDate = price.StartDate,
        //        //            Status = (int)price.Status,
        //        //        };

        //        //       // ppservice.Create(pp);
        //        //    }
        //        //}
        //        #endregion

        //        #region Product Prices

        //        if (model.ProductPrices.NullableAny())
        //        {
        //            foreach (ProductPriceViewModel price in model.ProductPrices)
        //            {
        //                    switch (price.Type)
        //                    {
        //                        case (int)ProductPriceType.Hire:
        //                            hrate = price.Rate;
        //                            hrateunit = price.RateUnit;
        //                            break;
        //                        case (int)ProductPriceType.Issue:
        //                            irate = price.Rate;
        //                            irateunit = price.RateUnit;
        //                            break;
        //                        case (int)ProductPriceType.Lost:
        //                            lrate = price.Rate;
        //                            lrateunit = price.RateUnit;
        //                            break;
        //                        default:
        //                            break;

        //                }
        //                                product = pcservice.Create(product);
        //                //ProductPrice pp = new ProductPrice()
        //                //{
        //                //    ProductId = product.Id,
        //                //    Rate = price.Rate ?? 0,
        //                //    Type = (int)price.Type,
        //                //    RateUnit = price.RateUnit,
        //                //    FromDate = price.StartDate,
        //                //    Status = (int)price.Status,
        //                //};

        //                //ppservice.Create(pp);
        //            }
        //        }

        //        #endregion

        //        //gathered all the info as needed, now create
        //        product = pcservice.Create(product);

        //        #region Product to Client
        //        //string sessClientId = (Session["ClientId"] != null ? Session["ClientId"].ToString() : null);
        //        //int clientID = (!string.IsNullOrEmpty(sessClientId) ? int.Parse(sessClientId) : 0);
        //        //ClientProduct clientproduct = new ClientProduct()
        //        //{
        //        //    ClientId = clientID,
        //        //    ProductId = product.Id,
        //        //    ProductDescription = product.Description,
        //        //    ActiveDate = product.CreatedOn,
        //        //    HireRate = 0,
        //        //    LostRate = 0,
        //        //    IssueRate = 0,
        //        //    PassonRate = 0,
        //        //    PassonDays = 0,
        //        //    Status = product.Status,

        //        //};
        //        //pcservice.Create(clientproduct);
        //        #endregion

        //        #region Any Files

        //        if (model.File != null)
        //        {
        //            // Create folder
        //            string path = Server.MapPath($"~/{VariableExtension.SystemRules.DocumentsLocation}/Product/{model.Name.Trim().Replace("/", "_").Replace("\\", "_")}/");

        //            if (!Directory.Exists(path))
        //            {
        //                Directory.CreateDirectory(path);
        //            }

        //            string now = DateTime.Now.ToString("yyyyMMddHHmmss");

        //            Document doc = new Document()
        //            {
        //                ObjectId = product.Id,
        //                ObjectType = "Product",
        //                Name = model.File.Name,
        //                Category = model.File.Name,
        //                Status = (int)Status.Active,
        //                Title = model.File.File.FileName,
        //                Size = model.File.File.ContentLength,
        //                Description = model.File.Description,
        //                Type = Path.GetExtension(model.File.File.FileName),
        //                Location = $"Product/{model.Name.Trim().Replace("/", "_").Replace("\\", "_")}/{now}-{model.File.File.FileName}"
        //            };

        //            dservice.Create(doc);

        //            string fullpath = Path.Combine(path, $"{now}-{model.File.File.FileName}");
        //            model.File.File.SaveAs(fullpath);
        //        }

        //        #endregion

        //        scope.Complete();

        //        Notify("The Product was successfully created.", NotificationType.Success);
        //    }

        //    return RedirectToAction("LinkProducts");
        //}

        //
        // GET: /Client/EditProduct/5
        [Requires( PermissionTo.Edit )]
        public ActionResult EditProduct( int id )
        {
            using ( DocumentService dservice = new DocumentService() )
            using ( ProductService pservice = new ProductService() )
            {
                Product product = pservice.GetById( id );

                if ( product == null )
                {
                    Notify( "Sorry, the requested resource could not be found. Please try again", NotificationType.Error );

                    return PartialView( "_AccessDenied" );
                }

                List<Document> documents = dservice.List( product.Id, "Product" );

                ProductViewModel model = new ProductViewModel()
                {
                    Id = product.Id,
                    EditMode = true,
                    Name = product.Name,
                    Description = product.Description,
                    Status = ( Status ) product.Status,
                    File = new FileViewModel()
                    {
                        Name = documents?.FirstOrDefault()?.Name,
                        Id = documents?.FirstOrDefault()?.Id ?? 0,
                        Extension = documents?.FirstOrDefault()?.Type,
                        Description = documents?.FirstOrDefault()?.Description,
                    },
                    ProductPrices = new List<ProductPriceViewModel>(),
                };

                foreach ( ProductPrice p in product.ProductPrices )
                {
                    model.ProductPrices.Add( new ProductPriceViewModel()
                    {
                        Id = p.Id,
                        Rate = p.Rate,
                        RateUnit = p.RateUnit,
                        StartDate = p.FromDate,
                        ProductId = p.ProductId,
                        Status = ( Status ) p.Status,
                        Type = ( ProductPriceType ) p.Type
                    } );
                }

                if ( model.ProductPrices.Count < 3 )
                {
                    foreach ( int item in Enum.GetValues( typeof( ProductPriceType ) ) )
                    {
                        ProductPriceType type = ( ProductPriceType ) item;

                        if ( model.ProductPrices.Any( p => p.Type == type ) ) continue;

                        model.ProductPrices.Add( new ProductPriceViewModel()
                        {
                            Type = type,
                            Status = Status.Active
                        } );
                    }
                }

                return View( model );
            }
        }

        //
        // POST: /Client/EditProduct/5
        [HttpPost]
        [Requires( PermissionTo.Edit )]
        public ActionResult EditProduct( ProductViewModel model )
        {
            if ( !ModelState.IsValid )
            {
                Notify( "Sorry, the selected Product was not updated. Please correct all errors and try again.", NotificationType.Error );

                return View( model );
            }

            using ( ProductService pservice = new ProductService() )
            using ( TransactionScope scope = new TransactionScope() )
            using ( DocumentService dservice = new DocumentService() )
            using ( ClientProductService cpservice = new ClientProductService() )
            using ( ProductPriceService ppservice = new ProductPriceService() )
            {
                ClientProduct product = cpservice.GetById( model.Id );

                #region Validations

                if ( product == null )
                {
                    Notify( "Sorry, that Product does not exist! Please try again.", NotificationType.Error );

                    return View( model );
                }

                //if (product.Name != model.Name && pservice.Exist(model.Name))
                //{
                //    // Product already exist!
                //    Notify($"Sorry, a Product with the Name \"{model.Name}\" already exists!", NotificationType.Error);

                //    return View(model);
                //}

                #endregion

                #region Product

                //product.Name = model.Name;
                product.Status = ( int ) model.Status;
                product.ProductDescription = model.Description;
                #endregion

                #region Product Prices

                decimal? hrate = 0;
                decimal? irate = 0;
                decimal? lrate = 0;
                decimal? pocost = 0;


                if ( model.ProductPrices.NullableAny() )
                {
                    foreach ( ProductPriceViewModel price in model.ProductPrices )
                    {
                        switch ( price.Type )
                        {
                            case ProductPriceType.Hire:
                                hrate = price.Rate;
                                //hrateunit = price.RateUnit;
                                break;
                            case ProductPriceType.Issue:
                                irate = price.Rate;
                                //irateunit = price.RateUnit;
                                break;
                            case ProductPriceType.Lost:
                                lrate = price.Rate;
                                // lrateunit = price.RateUnit;
                                break;
                            default:
                                break;

                        }
                    }
                    //product.PassonRate = model.PassonRate;
                    //product.PassonDays = model.PassonDays;
                    product.HireRate = hrate;
                    product.IssueRate = irate;
                    product.LostRate = lrate;
                }

                #endregion

                //gathered all the info now update the entry
                product = cpservice.Update( product );

                #region Any Files

                if ( model.File.File != null )
                {
                    // Create folder
                    string path = Server.MapPath( $"~/{VariableExtension.SystemRules.DocumentsLocation}/Product/{model.Name.Trim().Replace( "/", "_" ).Replace( "\\", "_" )}/" );

                    if ( !Directory.Exists( path ) )
                    {
                        Directory.CreateDirectory( path );
                    }

                    string now = DateTime.Now.ToString( "yyyyMMddHHmmss" );

                    Document doc = dservice.GetById( model.File.Id );

                    if ( doc != null )
                    {
                        // Disable this file...
                        doc.Status = ( int ) Status.Inactive;

                        dservice.Update( doc );
                    }

                    doc = new Document()
                    {
                        ObjectId = product.Id,
                        ObjectType = "Product",
                        Name = model.File.Name,
                        Category = model.File.Name,
                        Status = ( int ) Status.Active,
                        Title = model.File.File.FileName,
                        Size = model.File.File.ContentLength,
                        Description = model.File.Description,
                        Type = Path.GetExtension( model.File.File.FileName ),
                        Location = $"Product/{model.Name.Trim().Replace( "/", "_" ).Replace( "\\", "_" )}/{now}-{model.File.File.FileName}"
                    };

                    dservice.Create( doc );

                    string fullpath = Path.Combine( path, $"{now}-{model.File.File.FileName}" );
                    model.File.File.SaveAs( fullpath );
                }

                #endregion

                scope.Complete();

                Notify( "The selected Product's details were successfully updated.", NotificationType.Success );
            }

            return RedirectToAction( "LinkProducts" );
        }

        //
        // POST: /Client/DeleteProduct/5
        [HttpPost]
        [Requires( PermissionTo.Delete )]
        public ActionResult DeleteProduct( ProductViewModel model )
        {
            Product product;

            using ( ProductService service = new ProductService() )
            {
                product = service.GetById( model.Id );

                if ( product == null )
                {
                    Notify( "Sorry, the requested resource could not be found. Please try again", NotificationType.Error );

                    return PartialView( "_AccessDenied" );
                }

                product.Status = ( ( ( Status ) product.Status ) == Status.Active ) ? ( int ) Status.Inactive : ( int ) Status.Active;

                service.Update( product );

                Notify( "The selected Product was successfully updated.", NotificationType.Success );
            }

            return RedirectToAction( "LinkProducts" );
        }

        [AcceptVerbs( HttpVerbs.Get | HttpVerbs.Post )]
        public JsonResult GetProductsForClient( string clientId )
        {
            if ( !string.IsNullOrEmpty( clientId ) )
            {
                Session[ "ClientId" ] = clientId;//wroite to session to use for adds and updates
                List<ProductCustomModel> products = null;
                //int pspId = Session[ "UserPSP" ];
                //int pspId = (CurrentUser != null ? CurrentUser.PSPs.FirstOrDefault().Id : 0);

                using ( ProductService service = new ProductService() )
                {
                    products = service.ListCSM( new PagingModel(), new CustomSearchModel() { ClientId = int.Parse( clientId ), Status = Status.Active } );
                }
                //ProductPrice price = new ProductPrice();

                //using (ProductPriceService aservice = new ProductPriceService())
                //{
                //    foreach (ProductCustomModel prod in products)
                //    {
                //        string hrate = "";
                //        string irate = "";
                //        string lrate = "";
                //        string pocost = "";
                //        price = aservice.Get(prod.Id);
                //        if (price != null)
                //        {
                //            hrate = price.RateUnit.ToString() + ' ' + price.Rate.ToString();
                //            irate = price.RateUnit.ToString() + ' ' + price.Rate.ToString();
                //            lrate = price.RateUnit.ToString() + ' ' + price.Rate.ToString();
                //            pocost = price.RateUnit.ToString() + ' ' + price.Rate.ToString();

                //        }

                //        Status status = (Status)prod.Status;
                //        String active = status.Equals(Status.Active) ? "active" : "inactive";
                //        String enable = status.Equals(Status.Active) ? "Disable" : "Enable";


                //    }
                //}


                //var jsonList = JsonConvert.SerializeObject(sites);
                return Json( products, JsonRequestBehavior.AllowGet );
            }
            else
            {
                return Json( data: "Error", behavior: JsonRequestBehavior.AllowGet );
            }
        }

        #endregion    

        #region Awaiting Activation 

        //
        // POST || GET: /Client/AwaitingActivation
        public ActionResult AwaitingActivation( PagingModel pm, CustomSearchModel csm, bool givecsm = false )
        {
            if ( givecsm )
            {
                ViewBag.ViewName = "AwaitingActivation";

                return PartialView( "_AwaitingActivationCustomSearch", new CustomSearchModel( "AwaitingActivation" ) );
            }
            int total = 0;

            List<Client> model = new List<Client>();
            //int pspId = Session[ "UserPSP" ];
            //int pspId = (CurrentUser != null ? CurrentUser.PSPs.FirstOrDefault().Id : 0);

            string sessClientId = ( Session[ "ClientId" ] != null ? Session[ "ClientId" ].ToString() : null );
            int clientId = ( !string.IsNullOrEmpty( sessClientId ) ? int.Parse( sessClientId ) : 0 );
            if ( clientId > 0 )
            {
                // csm.ClientId = clientId;
            }

            using ( ClientService service = new ClientService() )
            {
                pm.Sort = pm.Sort ?? "ASC";
                pm.SortBy = pm.SortBy ?? "Companyname";
                // csm.PSPClientStatus = PSPClientStatus.Rejected;//Status 2
                // csm.Status = Status.Pending;
                model = service.ListAwaitingActivation( pm, csm );//service.GetClientsByPSP(CurrentUser.PSPs.FirstOrDefault().Id);

                // var testModel = service.ListByColumn(null, "CompanyRegistrationNumber", "123456");
                total = ( model.Count < pm.Take && pm.Skip == 0 ) ? model.Count : service.Total();
            }

            PagingExtension paging = PagingExtension.Create( model, total, pm.Skip, pm.Take, pm.Page );


            return PartialView( "_AwaitingActivation", paging );
        }
        #endregion

        #region Exports

        //
        // GET: /Administration/Export
        public FileContentResult Export( PagingModel pm, CustomSearchModel csm, string type = "configurations" )
        {
            string csv = "";
            string filename = string.Format( "{0}-{1}.csv", type.ToUpperInvariant(), DateTime.Now.ToString( "yyyy_MM_dd_HH_mm" ) );

            pm.Skip = 0;
            pm.Take = int.MaxValue;

            switch ( type )
            {
                case "clientlist":
                    #region ClientList
                    csv = String.Format( "Id, Company Name, Reg #, Trading As, Vat Number, Chep reference, Contact Person, Contact Person Number,  Contact Person Email, Administrator Name,Administrator Email,Financial Person,Financial Person Email, Status {0}", Environment.NewLine );

                    List<Client> clients = new List<Client>();

                    using ( ClientService service = new ClientService() )
                    {
                        clients = service.ListCSM( pm, csm );
                    }

                    if ( clients != null && clients.Any() )
                    {
                        foreach ( Client item in clients )
                        {
                            csv = String.Format( "{0} {1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14} {15}",
                                                csv,
                                                item.Id,
                                                item.CompanyName,
                                                item.CompanyRegistrationNumber,
                                                item.TradingAs,
                                                item.VATNumber,
                                                item.ChepReference,
                                                item.ContactPerson,
                                                item.ContactNumber,
                                                item.Email,
                                                item.AdminPerson,
                                                item.AdminEmail,
                                                item.FinancialPerson,
                                                item.FinPersonEmail,
                                                ( Status ) ( int ) item.Status,
                                                Environment.NewLine );
                        }
                    }


                    #endregion

                    break;
                case "awaitingactivation":
                    #region AwaitingActivation
                    csv = String.Format( "Id, Company Name, Reg #, Trading As, Vat Number, Chep reference, Contact Person, Contact Person Number,  Contact Person Email, Administrator Name,Administrator Email,Financial Person,Financial Person Email, Status {0}", Environment.NewLine );

                    List<Client> inactiveclients = new List<Client>();
                    csm.Status = Status.Pending;
                    using ( ClientService service = new ClientService() )
                    {
                        inactiveclients = service.ListCSM( pm, csm );
                    }

                    if ( inactiveclients != null && inactiveclients.Any() )
                    {
                        foreach ( Client item in inactiveclients )
                        {
                            csv = String.Format( "{0} {1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14} {15}",
                                                csv,
                                                item.Id,
                                                item.CompanyName,
                                                item.CompanyRegistrationNumber,
                                                item.TradingAs,
                                                item.VATNumber,
                                                item.ChepReference,
                                                item.ContactPerson,
                                                item.ContactNumber,
                                                item.Email,
                                                item.AdminPerson,
                                                item.AdminEmail,
                                                item.FinancialPerson,
                                                item.FinPersonEmail,
                                                ( Status ) ( int ) item.Status,
                                                Environment.NewLine );
                        }
                    }


                    #endregion

                    break;
                case "managesites":
                    #region AwaitingActivation
                    csv = String.Format( "Id, Name, Description, X Coord, Y Coord, Address, Postal Code, Contact Name,Contact No,Planning Point, Depot, Chep Sitecode, Site Type, Status {0}", Environment.NewLine );

                    List<Site> siteList = new List<Site>();

                    using ( SiteService service = new SiteService() )
                    {
                        siteList = service.ListCSM( pm, csm );
                    }

                    if ( siteList != null && siteList.Any() )
                    {
                        foreach ( Site item in siteList )
                        {
                            csv = String.Format( "{0} {1},{2},{3},{4},{5},{6},{7},{8},{9},{10}, {11}, {12}, {13}, {14} {15}",
                                                csv,
                                                item.Id,
                                                item.Name,
                                                item.Description,
                                                item.XCord,
                                                item.YCord,
                                                item.Address,
                                                item.PostalCode,
                                                item.ContactName,
                                                item.ContactNo,
                                                item.PlanningPoint,
                                                item.Depot,
                                                item.SiteCodeChep,
                                                ( SiteType ) ( int ) item.SiteType,
                                                ( Status ) ( int ) item.Status,
                                                Environment.NewLine );
                        }
                    }
                    #endregion
                    break;

                case "linkproducts":
                    #region linkproducts
                    csv = String.Format( "Id, ClientId, Company Name, ProductId, Product Name, Product Description, Active Date, HireRate, LostRate, IssueRate, PassonRate, PassonDays, Status {0}", Environment.NewLine );

                    List<ClientProductCustomModel> product = new List<ClientProductCustomModel>();

                    using ( ClientProductService service = new ClientProductService() )
                    {
                        product = service.ListCSM( pm, csm );
                    }

                    if ( product != null && product.Any() )
                    {
                        foreach ( ClientProductCustomModel item in product )
                        {
                            csv = String.Format( "{0} {1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13} {14}",
                                                csv,
                                                item.Id,
                                                item.ClientId,
                                                item.CompanyName,
                                                item.ProductId,
                                                item.Name,
                                                item.ProductDescription,
                                                item.ActiveDate,
                                                item.HireRate,
                                                item.LostRate,
                                                item.IssueRate,
                                                item.PassonRate,
                                                item.PassonDays,
                                                ( Status ) ( int ) item.Status,
                                                Environment.NewLine );
                        }
                    }


                    #endregion

                    break;

                case "clientgroups":
                    #region ClientGroup
                    csv = String.Format( "Id, Group Name, Description, Status {0}", Environment.NewLine );

                    List<ClientGroupCustomModel> clientGroups = new List<ClientGroupCustomModel>();

                    using ( ClientGroupService service = new ClientGroupService() )
                    {
                        clientGroups = service.ListCSM( pm, csm );
                    }

                    if ( clientGroups != null && clientGroups.Any() )
                    {
                        foreach ( ClientGroupCustomModel item in clientGroups )
                        {
                            csv = String.Format( "{0} {1},{2},{3},{4} {5}",
                                                csv,
                                                item.Id,
                                                item.Name,
                                                item.Description,
                                                 ( Status ) ( int ) item.Status,
                                                Environment.NewLine );
                        }
                    }


                    #endregion

                    break;
                case "clientkpis":
                    #region ClientKPI
                    csv = String.Format( "Id, Client Id,Description, Weight %, TargetAmount, Target Period, Status {0}", Environment.NewLine );

                    List<ClientKPI> kpis = new List<ClientKPI>();

                    using ( ClientKPIService service = new ClientKPIService() )
                    {
                        kpis = service.ListCSM( pm, csm );
                    }

                    if ( kpis != null && kpis.Any() )
                    {
                        foreach ( ClientKPI item in kpis )
                        {
                            csv = String.Format( "{0} {1},{2},{3},{4},{5},{6},{7},{8},{9},{10} {11}",
                                                csv,
                                                item.Id,
                                                item.ClientId,
                                                item.KPIDescription,
                                                //item.Disputes,
                                                //item.OutstandingPallets,
                                                //item.OutstandingDays,
                                                //item.Passons,
                                                item.Weight,
                                                item.TargetAmount,
                                                item.TargetPeriod,
                                                ( Status ) ( int ) item.Status,
                                                Environment.NewLine );
                        }
                    }


                    #endregion

                    break;
            }

            return File( new System.Text.UTF8Encoding().GetBytes( csv ), "text/csv", filename );
        }

        #endregion

        #region Clients

        //
        // POST || GET: /Client/ClientKPIS
        public ActionResult ClientKPIS( PagingModel pm, CustomSearchModel csm, bool givecsm = false )
        {
            if ( givecsm )
            {
                ViewBag.ViewName = "ClientKPIS";

                return PartialView( "_ClientKPISCustomSearch", new CustomSearchModel( "ClientKPIS" ) );
            }
            int total = 0;

            List<ClientKPI> model = new List<ClientKPI>();
            List<ClientKPIViewModel> viewModel = new List<ClientKPIViewModel>();

            using ( ClientKPIService service = new ClientKPIService() )
            using ( ClientService clientservice = new ClientService() )
            {
                pm.Sort = pm.Sort ?? "ASC";
                pm.SortBy = pm.SortBy ?? "Name";
                csm.Status = Status.Active;
                csm.Status = Status.Active;
                if ( CurrentUser == null )
                {
                    Notify( "Sorry, it seems the session had expired. Please log in again.", NotificationType.Error );

                    return RedirectToAction( "Index", "Administration" ); //Return to login as session is invalid
                }
                if ( CurrentUser.PSPs.Count > 0 )
                {
                    string sessClientId = ( Session[ "ClientId" ] != null ? Session[ "ClientId" ].ToString() : null );
                    int clientId = ( !string.IsNullOrEmpty( sessClientId ) ? int.Parse( sessClientId ) : 0 );
                    if ( clientId > 0 )
                    {
                        csm.ClientId = clientId;
                    }

                    model = service.ListCSM( pm, csm );//service.GetClientsByPSP(CurrentUser.PSPs.FirstOrDefault().Id);

                    foreach ( ClientKPI cl in model )
                    {
                        Client client = clientservice.GetById( cl.ClientId );
                        ClientKPIViewModel vm = new ClientKPIViewModel()
                        {
                            Id = cl.Id,
                            ClientName = client.CompanyName,
                            TradingAs = client.TradingAs,
                            KPIDescription = cl.KPIDescription,
                            Weight = cl.Weight,
                            TargetAmount = cl.TargetAmount,
                            TargetPeriod = cl.TargetPeriod,
                            Status = cl.Status,
                            //Disputes = cl.Disputes,
                            //OutstandingDays = cl.OutstandingDays,
                            //OutstandingPallets = cl.OutstandingPallets,
                            //Passons = cl.Passons,
                            //MonthlyCost = cl.MonthlyCost,
                            //ResolveDays = cl.ResolveDays,
                        };
                        viewModel.Add( vm );
                    }
                }
                else
                {
                    model = null;
                }

                // var testModel = service.ListByColumn(null, "CompanyRegistrationNumber", "123456");
                total = ( viewModel.Count < pm.Take && pm.Skip == 0 ) ? viewModel.Count : service.Total();
            }

            PagingExtension paging = PagingExtension.Create( viewModel, total, pm.Skip, pm.Take, pm.Page );


            return PartialView( "_ClientKPIS", paging );
        }

        //
        // GET: /Client/ClientDetails/5
        public ActionResult KPIDetails( int id, bool layout = true )
        {
            ClientKPI model = new ClientKPI();
            Client client = new Client();
            ClientKPIViewModel kpiview = new ClientKPIViewModel();
            using ( ClientService service = new ClientService() )
            using ( ClientKPIService kservice = new ClientKPIService() )
            {

                model = kservice.GetById( id );
                if ( model == null )
                {
                    Notify( "Sorry, the requested resource could not be found. Please try again", NotificationType.Error );

                    return RedirectToAction( "Index" );
                }
                client = service.GetById( model.ClientId );

                //repopulate the view model
                kpiview.KPIDescription = model.KPIDescription;
                kpiview.Id = model.Id;
                kpiview.ClientId = model.ClientId;
                kpiview.ClientName = client.CompanyName;
                kpiview.TradingAs = client.TradingAs;
                kpiview.Disputes = model.Disputes;
                kpiview.OutstandingDays = model.OutstandingDays;
                kpiview.OutstandingPallets = model.OutstandingPallets;
                kpiview.Passons = model.Passons;
                kpiview.MonthlyCost = model.MonthlyCost;
                kpiview.ResolveDays = model.ResolveDays;
                kpiview.Status = model.Status;
                kpiview.Weight = model.Weight;
                kpiview.TargetAmount = model.TargetAmount;
                kpiview.TargetPeriod = model.TargetPeriod;
            }

            if ( layout )
            {
                ViewBag.IncludeLayout = true;
            }

            return View( kpiview );
        }

        // GET: Client/AddKPI
        [Requires( PermissionTo.Create )]
        public ActionResult AddKPI()
        {
            ClientKPIViewModel model = new ClientKPIViewModel() { EditMode = true };
            List<Client> clientList = new List<Client>();
            string sessClientId = ( Session[ "ClientId" ] != null ? Session[ "ClientId" ].ToString() : null );
            int clientId = ( !string.IsNullOrEmpty( sessClientId ) ? int.Parse( sessClientId ) : 0 );
            ViewBag.ContextualMode = ( clientId > 0 ? true : false ); //Whether a client is specific or not and the View can know about it
            model.ContextualMode = ( clientId > 0 ? true : false ); //Whether a client is specific or not and the View can know about it
            using ( ClientService clientService = new ClientService() )
            {
                clientList = clientService.ListCSM( new PagingModel(), new CustomSearchModel() { ClientId = clientId, Status = Status.Active } );
            }

            IEnumerable<SelectListItem> clientDDL = clientList.Select( c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.CompanyName

            } );
            ViewBag.ClientList = clientDDL;
            return View( model );
        }

        // POST: Client/AddKPI
        [HttpPost]
        [Requires( PermissionTo.Create )]
        public ActionResult AddKPI( ClientKPIViewModel model )
        {
            try
            {
                if ( !ModelState.IsValid )
                {
                    Notify( "Sorry, the Client KPI was not created. Please correct all errors and try again.", NotificationType.Error );

                    return View( model );
                }

                using ( TransactionScope scope = new TransactionScope() )
                using ( ClientKPIService kpiservice = new ClientKPIService() )
                // using (ClientBudgetService bservice = new ClientBudgetService())
                {
                    #region Validation
                    //if (!string.IsNullOrEmpty(model) && service.ExistByCompanyRegistrationNumber(model.CompanyRegistrationNumber.Trim()))
                    //{
                    //    // Bank already exist!
                    //    Notify($"Sorry, a Client with the Registration number \"{model.CompanyRegistrationNumber}\" already exists!", NotificationType.Error);

                    //    return View(model);
                    //}
                    #endregion
                    #region Create ClientKPI
                    ClientKPI clientkpi = new ClientKPI()
                    {
                        KPIDescription = model.KPIDescription,
                        ClientId = model.ClientId,
                        //Disputes = model.Disputes,
                        //OutstandingPallets = model.OutstandingPallets,
                        //Passons = model.Passons,
                        //MonthlyCost = model.MonthlyCost,
                        //OutstandingDays = model.OutstandingDays,
                        //ResolveDays = model.ResolveDays,
                        Status = ( int ) Status.Active,//model.Status,
                        Weight = model.Weight,
                        TargetAmount = model.TargetAmount,
                        TargetPeriod = model.TargetPeriod
                    };
                    ClientKPI kpimodel = kpiservice.Create( clientkpi );
                    #endregion

                    scope.Complete();
                }
                Notify( "The Client KPI was successfully created.", NotificationType.Success );
                return RedirectToAction( "ClientKPIS" );
            }
            catch ( Exception ex )
            {
                ViewBag.Message = ex.Message;
                return View();
            }
        }

        // GET: Client/EditKPI/5
        [Requires( PermissionTo.Edit )]
        public ActionResult EditKPI( int id )
        {
            ClientKPI clientkpi;

            using ( ClientService clientservice = new ClientService() )
            using ( ClientKPIService kpiservice = new ClientKPIService() )
            {
                clientkpi = kpiservice.GetById( id );

                if ( clientkpi == null )
                {
                    Notify( "Sorry, the requested resource could not be found. Please try again", NotificationType.Error );

                    return PartialView( "_AccessDenied" );
                }
                Client client = clientservice.Get( clientkpi.ClientId );
                ClientKPIViewModel model = new ClientKPIViewModel()
                {
                    KPIDescription = clientkpi.KPIDescription,
                    Id = clientkpi.Id,
                    ClientId = clientkpi.ClientId,
                    ClientName = client.CompanyName,
                    TradingAs = client.TradingAs,
                    Disputes = clientkpi.Disputes,
                    OutstandingDays = clientkpi.OutstandingDays,
                    OutstandingPallets = clientkpi.OutstandingPallets,
                    Passons = clientkpi.Passons,
                    MonthlyCost = clientkpi.MonthlyCost,
                    ResolveDays = clientkpi.ResolveDays,
                    Status = clientkpi.Status,
                    Weight = clientkpi.Weight,
                    TargetAmount = clientkpi.TargetAmount,
                    TargetPeriod = clientkpi.TargetPeriod,
                    EditMode = true
                };


                return View( model );
            }
        }

        // POST: Client/EditKPI/5
        [HttpPost]
        [Requires( PermissionTo.Edit )]
        public ActionResult EditKPI( ClientKPIViewModel model, PagingModel pm, bool isstructure = false )
        {
            try
            {
                if ( !ModelState.IsValid )
                {
                    Notify( "Sorry, the selected Client was not updated. Please correct all errors and try again.", NotificationType.Error );

                    return View( model );
                }

                ClientKPI kpiview;

                using ( TransactionScope scope = new TransactionScope() )
                using ( ClientKPIService kpiservice = new ClientKPIService() )
                // using (ClientBudgetService bservice = new ClientBudgetService())
                {
                    kpiview = kpiservice.GetById( model.Id );

                    if ( kpiview == null )
                    {
                        Notify( "Sorry, that Client KPI does not exist! Please specify a valid Role Id and try again.", NotificationType.Error );

                        return View( model );
                    }

                    #region Update Client KPI

                    // Update Client KPI
                    kpiview.KPIDescription = model.KPIDescription;
                    kpiview.Id = model.Id;
                    kpiview.ClientId = model.ClientId;
                    kpiview.Disputes = model.Disputes;
                    kpiview.OutstandingDays = model.OutstandingDays;
                    kpiview.OutstandingPallets = model.OutstandingPallets;
                    kpiview.Passons = model.Passons;
                    kpiview.MonthlyCost = model.MonthlyCost;
                    kpiview.ResolveDays = model.ResolveDays;
                    kpiview.Status = model.Status;
                    kpiview.Weight = model.Weight;
                    kpiview.TargetAmount = model.TargetAmount;
                    kpiview.TargetPeriod = model.TargetPeriod;

                    kpiservice.Update( kpiview );

                    #endregion

                    scope.Complete();
                }

                Notify( "The selected Client KPI details were successfully updated.", NotificationType.Success );

                return RedirectToAction( "ClientKPIS" );
            }
            catch ( Exception ex )
            {
                ViewBag.Message = ex.Message;
                return View();
            }
        }

        // POST: Client/DeleteKPI/5
        [HttpPost]
        [Requires( PermissionTo.Delete )]
        public ActionResult DeleteKPI( ClientViewModel model )
        {
            ClientKPI clientkpi;
            try
            {

                using ( ClientKPIService service = new ClientKPIService() )
                using ( TransactionScope scope = new TransactionScope() )
                {
                    clientkpi = service.GetById( model.Id );

                    if ( clientkpi == null )
                    {
                        Notify( "Sorry, the requested resource could not be found. Please try again", NotificationType.Error );

                        return PartialView( "_AccessDenied" );
                    }

                    clientkpi.Status = ( ( ( Status ) clientkpi.Status ) == Status.Active ) ? ( int ) Status.Inactive : ( int ) Status.Active;

                    service.Update( clientkpi );
                    scope.Complete();

                }
                Notify( "The selected Client KPI was successfully updated.", NotificationType.Success );
                return RedirectToAction( "ClientKPIS" );
            }
            catch ( Exception ex )
            {
                ViewBag.Message = ex.Message;
                return View();
            }
        }


        #endregion

        #region General

        //[HttpPost]
        //// GET: /Client/ImportSites
        //public ActionResult ImportSubSites(HttpPostedFileBase postedFile)
        //{

        //    return RedirectToAction("ImportSites", "Client");
        //}


        [HttpPost]
        // GET: /Client/ImportSites
        public ActionResult ImportSites( HttpPostedFileBase postedFile )
        {
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
                    //for subsites to load tehse sites under a main site
                    string sessSiteId = ( Session[ "SiteId" ] != null ? Session[ "SiteId" ].ToString() : null );
                    int SiteID = ( !string.IsNullOrEmpty( sessSiteId ) ? int.Parse( sessSiteId ) : 0 );

                    using ( var sreader = new StreamReader( postedFile.InputStream ) )
                    using ( SiteService siteService = new SiteService() )
                    using ( ClientSiteService csService = new ClientSiteService() )
                    using ( TransactionScope scope = new TransactionScope() )
                    using ( AddressService aservice = new AddressService() )
                    {
                        //First line is header. If header is not passed in csv then we can neglect the below line.
                        string[] headers = sreader.ReadLine().Split( ',' );
                        //Loop through the records
                        while ( !sreader.EndOfStream )
                        {
                            string[] rows = sreader.ReadLine().Split( ',' );

                            int siteType = 1;

                            string strLatY = rows[ 3 ].ToString();
                            decimal latitudeY = 0;
                            try
                            {
                                decimal.TryParse( rows[ 3 ].ToString(), out latitudeY );
                                latitudeY = decimal.Round( latitudeY, 4 );
                                strLatY = ( latitudeY != 0 ? latitudeY.ToString() : strLatY );
                            }
                            catch ( Exception ex )
                            {
                                ViewBag.Message = string.Concat( rows[ 3 ].ToString(), " ", ex.Message );
                            }
                            string strLngX = rows[ 2 ].ToString();
                            decimal longitudeX = 0;
                            try
                            {
                                decimal.TryParse( rows[ 2 ].ToString(), out longitudeX );
                                longitudeX = decimal.Round( longitudeX, 4 );
                                strLngX = ( longitudeX != 0 ? longitudeX.ToString() : strLngX );
                            }
                            catch ( Exception ex )
                            {
                                ViewBag.Message = string.Concat( rows[ 3 ].ToString(), " ", ex.Message );
                            }

                            Site existingSite = null;
                            #region Validation
                            if ( !string.IsNullOrEmpty( strLngX ) && siteService.ExistByXYCoords( strLngX, strLatY ) )
                            {
                                //Notify($"Sorry, a Site with the same X Y Coordinates already exists \"{model.XCord}\" already exists!", NotificationType.Error);
                                //return View(model);

                                //rather than pass back to view, we will create the new site as a subsite of the existing site. 
                                //Get the existing site first
                                existingSite = siteService.GetByColumnsWhere( "XCord", strLngX, "YCord", strLatY );
                                SiteID = existingSite.Id;//This is the existing site retrieved by mapping same X and Y coord, read that into the model.SiteId which makes the new site a child site
                                siteType = 2;//Mark teh site as a subsite by default
                            }

                            int regionId = 0;
                            if ( !string.IsNullOrEmpty( rows[ 16 ].ToString() ) )
                            {
                                regionId = int.Parse( rows[ 16 ].ToString() );
                            }
                            int provinceId = 0;
                            string provinceName = "";
                            if ( !string.IsNullOrEmpty( rows[ 8 ].ToString() ) )
                            {
                                int.TryParse( rows[ 8 ].ToString(), out provinceId );
                                try
                                {
                                    if ( provinceId > 0 )
                                    {
                                        provinceName = ( ( Province ) provinceId ).GetDisplayText();
                                    }
                                }
                                catch ( Exception ex )
                                {
                                    ViewBag.Message = string.Concat( rows[ 8 ].ToString(), " ", ex.Message );
                                }
                            }
                            #region Create Site
                            Site site = new Site()
                            {
                                Name = rows[ 0 ].ToString(),
                                Description = rows[ 1 ].ToString(),
                                XCord = strLngX,
                                YCord = strLatY,
                                Address = rows[ 4 ].ToString() + " " + rows[ 5 ].ToString() + " " + rows[ 6 ].ToString() + " " + rows[ 7 ].ToString() + " " + provinceName,
                                PostalCode = rows[ 7 ].ToString(),
                                ContactName = rows[ 10 ].ToString(),
                                ContactNo = rows[ 9 ].ToString(),
                                PlanningPoint = rows[ 11 ].ToString(),
                                SiteType = siteType,
                                AccountCode = rows[ 13 ].ToString(),
                                Depot = rows[ 14 ].ToString(),
                                SiteCodeChep = rows[ 15 ].ToString(),
                                Status = ( int ) Status.Pending,
                                RegionId = regionId,
                                FinanceContact = rows[ 17 ].ToString(),
                                FinanceContactNo = rows[ 18 ].ToString(),
                                ReceivingContact = rows[ 19 ].ToString(),
                                ReceivingContactNo = rows[ 20 ].ToString(),
                            };
                            //For Subsites
                            if ( SiteID > 0 )
                            {
                                site.SiteId = SiteID;
                            }
                            site = siteService.Create( site );
                            #endregion

                            #region Create Address (s)

                            if ( !string.IsNullOrEmpty( rows[ 3 ].ToString() ) )
                            {
                                Address address = new Address()
                                {
                                    ObjectId = site.Id,
                                    ObjectType = "Site",
                                    Town = rows[ 6 ].ToString(),
                                    Status = ( int ) Status.Active,
                                    PostalCode = rows[ 7 ].ToString(),
                                    Type = ( int ) AddressType.Postal,
                                    Addressline1 = rows[ 4 ].ToString(),
                                    Addressline2 = rows[ 5 ].ToString(),
                                    Province = provinceId,
                                };
                                aservice.Create( address );
                            }

                            #endregion

                            //tie Client in Session to New Site
                            #region Add ClientSite
                            ClientSite csSite = new ClientSite()
                            {
                                ClientId = clientID,
                                SiteId = site.Id,
                                AccountingCode = site.AccountCode,
                                Status = ( int ) Status.Active
                            };
                            csService.Create( csSite );
                            #endregion

                        }
                        #endregion
                        scope.Complete();
                    }

                }
                catch ( Exception ex )
                {
                    ViewBag.Message = ex.Message;
                }
                finally
                {

                }
            }
            else
            {

            }
            return RedirectToAction( "ManageSites", "Client" );
        }

        [HttpPost]
        // GET: /Client/ImportSites
        public ActionResult ImportSubSites( HttpPostedFileBase postedFile )
        {
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
                    //for subsites to load tehse sites under a main site
                    string sessSiteId = ( Session[ "SiteId" ] != null ? Session[ "SiteId" ].ToString() : null );
                    int SiteID = ( !string.IsNullOrEmpty( sessSiteId ) ? int.Parse( sessSiteId ) : 0 );

                    using ( var sreader = new StreamReader( postedFile.InputStream ) )
                    using ( SiteService siteService = new SiteService() )
                    using ( ClientSiteService csService = new ClientSiteService() )
                    using ( TransactionScope scope = new TransactionScope() )
                    using ( AddressService aservice = new AddressService() )
                    {
                        //First line is header. If header is not passed in csv then we can neglect the below line.
                        string[] headers = sreader.ReadLine().Split( ',' );
                        //Loop through the records
                        while ( !sreader.EndOfStream )
                        {
                            string[] rows = sreader.ReadLine().Split( ',' );

                            int siteType = 1;

                            string strLatY = rows[ 3 ].ToString();
                            decimal latitudeY = 0;
                            try
                            {
                                decimal.TryParse( rows[ 3 ].ToString(), out latitudeY );
                                latitudeY = decimal.Round( latitudeY, 4 );
                                strLatY = ( latitudeY != 0 ? latitudeY.ToString() : strLatY );
                            }
                            catch ( Exception ex )
                            {
                                ViewBag.Message = string.Concat( rows[ 3 ].ToString(), " ", ex.Message );
                            }
                            string strLngX = rows[ 2 ].ToString();
                            decimal longitudeX = 0;
                            try
                            {
                                decimal.TryParse( rows[ 2 ].ToString(), out longitudeX );
                                longitudeX = decimal.Round( longitudeX, 4 );
                                strLngX = ( longitudeX != 0 ? longitudeX.ToString() : strLngX );
                            }
                            catch ( Exception ex )
                            {
                                ViewBag.Message = string.Concat( rows[ 3 ].ToString(), " ", ex.Message );
                            }

                            Site existingSite = null;
                            #region Validation
                            if ( !string.IsNullOrEmpty( strLngX ) && siteService.ExistByXYCoords( strLngX, strLatY ) )
                            {
                                //Notify($"Sorry, a Site with the same X Y Coordinates already exists \"{model.XCord}\" already exists!", NotificationType.Error);
                                //return View(model);

                                //rather than pass back to view, we will create the new site as a subsite of the existing site. 
                                //Get the existing site first
                                existingSite = siteService.GetByColumnsWhere( "XCord", strLngX, "YCord", strLatY );
                                SiteID = existingSite.Id;//This is the existing site retrieved by mapping same X and Y coord, read that into the model.SiteId which makes the new site a child site
                                siteType = 2;//Mark teh site as a subsite by default
                            }

                            int regionId = 0;
                            if ( !string.IsNullOrEmpty( rows[ 16 ].ToString() ) )
                            {
                                regionId = int.Parse( rows[ 16 ].ToString() );
                            }
                            int provinceId = 0;
                            string provinceName = "";
                            if ( !string.IsNullOrEmpty( rows[ 8 ].ToString() ) )
                            {
                                int.TryParse( rows[ 8 ].ToString(), out provinceId );
                                try
                                {
                                    if ( provinceId > 0 )
                                    {
                                        provinceName = ( ( Province ) provinceId ).GetDisplayText();
                                    }
                                }
                                catch ( Exception ex )
                                {
                                    ViewBag.Message = string.Concat( rows[ 8 ].ToString(), " ", ex.Message );
                                }
                            }
                            #region Create Site
                            Site site = new Site()
                            {
                                Name = rows[ 0 ].ToString(),
                                Description = rows[ 1 ].ToString(),
                                XCord = strLngX,
                                YCord = strLatY,
                                Address = rows[ 4 ].ToString() + " " + rows[ 5 ].ToString() + " " + rows[ 6 ].ToString() + " " + rows[ 7 ].ToString() + " " + provinceName,
                                PostalCode = rows[ 7 ].ToString(),
                                ContactName = rows[ 10 ].ToString(),
                                ContactNo = rows[ 9 ].ToString(),
                                PlanningPoint = rows[ 11 ].ToString(),
                                SiteType = siteType,
                                AccountCode = rows[ 13 ].ToString(),
                                Depot = rows[ 14 ].ToString(),
                                SiteCodeChep = rows[ 15 ].ToString(),
                                Status = ( int ) Status.Pending,
                                RegionId = regionId,
                                FinanceContact = rows[ 17 ].ToString(),
                                FinanceContactNo = rows[ 18 ].ToString(),
                                ReceivingContact = rows[ 19 ].ToString(),
                                ReceivingContactNo = rows[ 20 ].ToString(),
                            };
                            //For Subsites
                            if ( SiteID > 0 )
                            {
                                site.SiteId = SiteID;
                            }
                            site = siteService.Create( site );
                            #endregion

                            #region Create Address (s)

                            if ( !string.IsNullOrEmpty( rows[ 3 ].ToString() ) )
                            {
                                Address address = new Address()
                                {
                                    ObjectId = site.Id,
                                    ObjectType = "Site",
                                    Town = rows[ 6 ].ToString(),
                                    Status = ( int ) Status.Active,
                                    PostalCode = rows[ 7 ].ToString(),
                                    Type = ( int ) AddressType.Postal,
                                    Addressline1 = rows[ 4 ].ToString(),
                                    Addressline2 = rows[ 5 ].ToString(),
                                    Province = provinceId,
                                };
                                aservice.Create( address );
                            }

                            #endregion

                            //tie Client in Session to New Site
                            #region Add ClientSite
                            ClientSite csSite = new ClientSite()
                            {
                                ClientId = clientID,
                                SiteId = site.Id,
                                AccountingCode = site.AccountCode,
                                Status = ( int ) Status.Active
                            };
                            csService.Create( csSite );
                            #endregion

                        }
                        #endregion
                        scope.Complete();
                    }

                }
                catch ( Exception ex )
                {
                    ViewBag.Message = ex.Message;
                }
                finally
                {

                }
            }
            else
            {

            }
            return RedirectToAction( "ManageSites", "Client" );
        }


       

        #endregion

        #region Budgets
        [AcceptVerbs( HttpVerbs.Get | HttpVerbs.Post )]
        public JsonResult GetClientBudgets( string clientId )
        {
            if ( clientId != null && clientId != "" )
            {
                List<ClientBudget> load = null;

                using ( ClientBudgetService bservice = new ClientBudgetService() )
                {
                    load = bservice.ListByColumnWhere( "ClientId", int.Parse( clientId ) );
                    return Json( load, JsonRequestBehavior.AllowGet );
                }
            }
            else
            {
                return Json( data: "Error", behavior: JsonRequestBehavior.AllowGet );
            }
        }

        [AcceptVerbs( HttpVerbs.Get | HttpVerbs.Post )]
        public JsonResult SetClientBudget( string Id, string ClientId, string BudgetYear, 
            string January, string February, string March, string April, string May, 
            string June, string July, string August, string September, string October, 
            string November, string December )
        {
            if ( Id != null )
            {
                using ( ClientBudgetService bservice = new ClientBudgetService() )
                using ( TransactionScope scope = new TransactionScope() )
                {
                    //Collection of budgets or singular?
                    if ( int.Parse( Id ) > 0 )
                    {
                        ClientBudget budget = bservice.GetById( int.Parse( Id ) );

                        budget.ClientId = int.Parse( ClientId );
                        budget.BudgetYear = int.Parse( BudgetYear );
                        budget.January = decimal.Parse( January );
                        budget.February = decimal.Parse( February );
                        budget.March = decimal.Parse( March );
                        budget.April = decimal.Parse( April );
                        budget.May = decimal.Parse( May );
                        budget.June = decimal.Parse( June );
                        budget.July = decimal.Parse( July );
                        budget.August = decimal.Parse( August );
                        budget.September = decimal.Parse( September );
                        budget.October = decimal.Parse( October );
                        budget.November = decimal.Parse( November );
                        budget.December = decimal.Parse( December );
                        budget.Status = ( int ) Status.Active;

                        bservice.Update( budget );
                    }
                    else
                    {
                        ClientBudget budget = new ClientBudget();
                        budget.ClientId = int.Parse( ClientId );
                        budget.BudgetYear = int.Parse( BudgetYear );
                        budget.January = decimal.Parse( January );
                        budget.February = decimal.Parse( February );
                        budget.March = decimal.Parse( March );
                        budget.April = decimal.Parse( April );
                        budget.May = decimal.Parse( May );
                        budget.June = decimal.Parse( June );
                        budget.July = decimal.Parse( July );
                        budget.August = decimal.Parse( August );
                        budget.September = decimal.Parse( September );
                        budget.October = decimal.Parse( October );
                        budget.November = decimal.Parse( November );
                        budget.December = decimal.Parse( December );
                        budget.Status = ( int ) Status.Active;

                        bservice.Create( budget );
                    }
                    scope.Complete();
                }

                return Json( data: "True", behavior: JsonRequestBehavior.AllowGet );
            }
            else
            {
                return Json( data: "Error", behavior: JsonRequestBehavior.AllowGet );
            }
        }

        [AcceptVerbs( HttpVerbs.Get | HttpVerbs.Post )]
        public JsonResult GetSiteBudgetList( string siteId )
        {
            if ( siteId != null && siteId != "" )
            {
                List<SiteBudget> load = null;

                using ( SiteBudgetService bservice = new SiteBudgetService() )
                {
                    load = bservice.ListByColumnWhere( "SiteId", int.Parse( siteId ) );
                    return Json( load, JsonRequestBehavior.AllowGet );
                }
            }
            else
            {
                return Json( data: "Error", behavior: JsonRequestBehavior.AllowGet );
            }
        }

        [AcceptVerbs( HttpVerbs.Get | HttpVerbs.Post )]
        public JsonResult SetSiteBudget( string Id, string SiteId, string BudgetYear, 
            string January, string February, string March, string April, string May, 
            string June, string July, string August, string September, string October, 
            string November, string December )
        {
            if ( Id != null )
            {
                using ( SiteBudgetService bservice = new SiteBudgetService() )
                using ( TransactionScope scope = new TransactionScope() )
                {
                    //Collection of budgets or singular?
                    if ( int.Parse( Id ) > 0 )
                    {
                        SiteBudget budget = bservice.GetById( int.Parse( Id ) );

                        budget.SiteId = int.Parse( SiteId );
                        budget.BudgetYear = int.Parse( BudgetYear );
                        budget.January = decimal.Parse( January );
                        budget.February = decimal.Parse( February );
                        budget.March = decimal.Parse( March );
                        budget.April = decimal.Parse( April );
                        budget.May = decimal.Parse( May );
                        budget.June = decimal.Parse( June );
                        budget.July = decimal.Parse( July );
                        budget.August = decimal.Parse( August );
                        budget.September = decimal.Parse( September );
                        budget.October = decimal.Parse( October );
                        budget.November = decimal.Parse( November );
                        budget.December = decimal.Parse( December );
                        budget.Status = ( int ) Status.Active;

                        bservice.Update( budget );
                    }
                    else
                    {
                        SiteBudget budget = new SiteBudget();
                        budget.SiteId = int.Parse( SiteId );
                        budget.BudgetYear = int.Parse( BudgetYear );
                        budget.January = decimal.Parse( January );
                        budget.February = decimal.Parse( February );
                        budget.March = decimal.Parse( March );
                        budget.April = decimal.Parse( April );
                        budget.May = decimal.Parse( May );
                        budget.June = decimal.Parse( June );
                        budget.July = decimal.Parse( July );
                        budget.August = decimal.Parse( August );
                        budget.September = decimal.Parse( September );
                        budget.October = decimal.Parse( October );
                        budget.November = decimal.Parse( November );
                        budget.December = decimal.Parse( December );
                        budget.Status = ( int ) Status.Active;

                        bservice.Create( budget );
                    }
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

    }
}
