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
using System.Net.Http.Headers;
using ExcelDataReader;
using System.Data;

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
                case "clients":

                    #region Clients

                    csv = string.Format( "Date Created, Company Name, Trading As, Reg #, Description, Chep reference, Status, Service Required, Type of Pallet Use, Other Type of Pallet Use, Company Type, PSP, VAT Number, BBBEE Level, Contact Person, Contact Number, Contact Email, Administrator, Administrator Email, Financial Person, Financial Person Email {0}", Environment.NewLine );

                    List<ClientCustomModel> clients = new List<ClientCustomModel>();

                    using ( ClientService service = new ClientService() )
                    {
                        clients = service.List1( pm, csm );
                    }

                    if ( clients != null && clients.Any() )
                    {
                        foreach ( ClientCustomModel item in clients )
                        {
                            csv = string.Format( "{0} {1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17},{18},{19},{20},{21} {22}",
                                                csv,
                                                item.CreatedOn,
                                                item.CompanyName,
                                                item.TradingAs,
                                                item.CompanyRegistrationNumber,
                                                item.Description,
                                                item.ChepReference,
                                                ( ( Status ) item.Status ).GetDisplayText(),
                                                ( ( ServiceType ) item.ServiceRequired ).GetDisplayText(),
                                                ( ( TypeOfPalletUse ) item.PalletType ).GetDisplayText(),
                                                item.PalletTypeOther,
                                                ( ( CompanyType ) item.CompanyType ).GetDisplayText(),
                                                ( item.PSPName ?? item.PSPCompanyName ),
                                                item.VATNumber,
                                                item.BBBEELevel,
                                                item.ContactPerson,
                                                item.ContactNumber,
                                                item.Email,
                                                item.AdminPerson,
                                                item.AdminEmail,
                                                item.FinancialPerson,
                                                item.FinPersonEmail,
                                                Environment.NewLine );
                        }
                    }


                    #endregion

                    break;

                case "managesites":

                    #region Manage Sites

                    csv = string.Format( "Date Created, Name, Description, X Coord, Y Coord, Address Line 1, Address Line 2, Town, Province, Postal Code, Contact Name, Contact No, Finance Contact, Finance No., Receiver Contact, Receiver No., Planning Point, Depot, Chep Site Code, Site Type, Status {0}", Environment.NewLine );

                    using ( SiteService service = new SiteService() )
                    using ( AddressService aservice = new AddressService() )
                    {
                        List<SiteCustomModel> sites = service.List1( pm, csm );

                        if ( sites != null && sites.Any() )
                        {
                            foreach ( SiteCustomModel item in sites )
                            {
                                Address address = aservice.Get( item.Id, "Site" );

                                csv = string.Format( "{0} {1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17},{18},{19},{20},{21} {22}",
                                                    csv,
                                                    item.CreatedOn,
                                                    item.Name,
                                                    item.Description,
                                                    item.XCord,
                                                    item.YCord,
                                                    address?.Addressline1,
                                                    address?.Addressline2,
                                                    address?.Town,
                                                    ( address != null ? ( ( Province ) address.Province ).GetDisplayText() : string.Empty ),
                                                    item.PostalCode,
                                                    item.ContactName,
                                                    item.ContactNo,
                                                    item.FinanceContact,
                                                    item.FinanceContactNo,
                                                    item.ReceivingContact,
                                                    item.ReceivingContactNo,
                                                    item.PlanningPoint,
                                                    item.Depot,
                                                    item.SiteCodeChep,
                                                    ( ( item.SiteType.HasValue ) ? ( ( SiteType ) item.SiteType ).GetDisplayText() : string.Empty ),
                                                    ( ( Status ) item.Status ).GetDisplayText(),
                                                    Environment.NewLine );
                            }
                        }
                    }

                    #endregion

                    break;

                case "linkproducts":

                    #region linkproducts
                    csv = string.Format( "Id, ClientId, Company Name, ProductId, Product Name, Product Description, Active Date, HireRate, LostRate, IssueRate, PassonRate, PassonDays, Status {0}", Environment.NewLine );

                    List<ClientProductCustomModel> product = new List<ClientProductCustomModel>();

                    using ( ClientProductService service = new ClientProductService() )
                    {
                        product = service.ListCSM( pm, csm );
                    }

                    if ( product != null && product.Any() )
                    {
                        foreach ( ClientProductCustomModel item in product )
                        {
                            csv = string.Format( "{0} {1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13} {14}",
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

                    #region Client Groups

                    csv = string.Format( "Date Created, Name, Description, Clients, Status {0}", Environment.NewLine );

                    using ( GroupService service = new GroupService() )
                    {
                        List<GroupCustomModel> clientGroups = service.List1( pm, csm );

                        if ( clientGroups != null && clientGroups.Any() )
                        {
                            foreach ( GroupCustomModel item in clientGroups )
                            {
                                csv = string.Format( "{0} {1},{2},{3},{4},{5} {6}",
                                                    csv,
                                                    item.CreatedOn,
                                                    item.Name,
                                                    item.Description,
                                                    item.ClientCount,
                                                    ( ( Status ) item.Status ).GetDisplayText(),
                                                    Environment.NewLine );
                            }
                        }
                    }

                    #endregion

                    break;

                case "clientkpi":

                    #region Client KPI

                    using ( ClientKPIService service = new ClientKPIService() )
                    {
                        csv = string.Format( "Date Created, Client, KPI Description, Outstanding Pallets, Disputes, Passons, Resolve Days, Outstanding Days, Monthly Cost, Weight %, Target Amount, Target Period, Status {0}", Environment.NewLine );

                        List<ClientKPICustomModel> kpis = new List<ClientKPICustomModel>();

                        kpis = service.List1( pm, csm );

                        if ( kpis != null && kpis.Any() )
                        {
                            foreach ( ClientKPICustomModel item in kpis )
                            {
                                csv = string.Format( "{0} {1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13} {14}",
                                                    csv,
                                                    item.CreatedOn,
                                                    "\"" + item.ClientName + "\"",
                                                    "\"" + item.KPIDescription + "\"",
                                                    "\"" + item.OutstandingPallets + "\"",
                                                    "\"" + item.Disputes + "\"",
                                                    "\"" + item.Passons + "\"",
                                                    "\"" + item.ResolveDays + "\"",
                                                    "\"" + item.OutstandingDays + "\"",
                                                    "\"" + item.MonthlyCost + "\"",
                                                    "\"" + item.Weight + "\"",
                                                    "\"" + item.TargetAmount + "\"",
                                                    ( ( TargetPeriod ) item.TargetPeriod ).GetDisplayText(),
                                                    ( ( Status ) item.Status ).GetDisplayText(),
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



        #region Clients

        //
        // GET: /Client/ClientDetails/5
        public ActionResult ClientDetails( int id, bool layout = true )
        {
            using ( ImageService iservice = new ImageService() )
            using ( ClientService cservice = new ClientService() )
            using ( AddressService aservice = new AddressService() )
            using ( DocumentService dservice = new DocumentService() )
            {
                Client model = cservice.GetById( id );

                if ( model == null )
                {
                    Notify( "Sorry, the requested resource could not be found. Please try again", NotificationType.Error );

                    return RedirectToAction( "Index" );
                }

                Address address = aservice.Get( model.Id, "Client" );

                List<Document> documents = dservice.List( model.Id, "Client" );

                if ( address != null )
                {
                    ViewBag.Address = address;
                }
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

        // GET: Client/AddClient
        [Requires( PermissionTo.Create )]
        public ActionResult AddClient()
        {
            ClientViewModel model = new ClientViewModel()
            {
                EditMode = true,
                Address = new AddressViewModel(),
                Files = new List<FileViewModel>(),
                ClientBudgets = new List<ClientBudget>()
            };

            return View( model );
        }

        // POST: Client/Create
        [HttpPost]
        [Requires( PermissionTo.Create )]
        public ActionResult AddClient( ClientViewModel model )
        {
            //if ( !ModelState.IsValid )
            //{
            //    Notify( "Sorry, the Client was not created. Please correct all errors and try again.", NotificationType.Error );

            //    return View( model );
            //}

            using ( ClientService service = new ClientService() )
            using ( AddressService aservice = new AddressService() )
            using ( TransactionScope scope = new TransactionScope() )
            using ( DocumentService dservice = new DocumentService() )
            using ( PSPClientService pcservice = new PSPClientService() )
            using ( ClientBudgetService bservice = new ClientBudgetService() )
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
                    AdminEmail = model.AdminEmail,
                    Status = ( int ) model.Status,
                    AdminPerson = model.AdminPerson,
                    CompanyName = model.CompanyName,
                    Description = model.CompanyName,
                    ChepReference = model.ChepReference,
                    ContactPerson = model.ContactPerson,
                    ContactNumber = model.ContactNumber,
                    FinPersonEmail = model.FinPersonEmail,
                    FinancialPerson = model.FinancialPerson,
                    ServiceRequired = ( int ) model.ServiceType,

                    PSPName = model.PSPName,
                    BBBEELevel = model.BBBEELevel,
                    CompanyType = ( int ) model.CompanyType,
                    PalletType = ( int ) model.TypeOfPalletUse,
                    PalletTypeOther = model.OtherTypeOfPalletUse,
                    NumberOfLostPallets = model.NumberOfLostPallets,
                    CompanyRegistrationNumber = model.CompanyRegistrationNumber,
                };

                client = service.Create( client );

                #endregion

                #region Create Client PSP link

                if ( model.ServiceType == ServiceType.HaveCompany && ( ( model.PSPId > 0 ) ? model.PSPId : CurrentUser.PSPs?.FirstOrDefault()?.Id ).HasValue )
                {
                    int? pspId = ( model.PSPId > 0 ) ? model.PSPId : CurrentUser.PSPs?.FirstOrDefault()?.Id;

                    PSPClient pClient = new PSPClient()
                    {
                        PSPId = pspId.Value,
                        ClientId = client.Id,
                        Status = ( int ) Status.Active
                    };

                    pcservice.Create( pClient );
                }

                #endregion

                #region Create Client Budget

                if ( model.ClientBudgets.NullableAny() )
                {
                    foreach ( ClientBudget l in model.ClientBudgets )
                    {
                        ClientBudget b = new ClientBudget()
                        {
                            ClientId = client.Id,
                            BudgetYear = l.BudgetYear,
                            Total = l.Total,
                            January = l.January,
                            February = l.February,
                            March = l.March,
                            April = l.April,
                            May = l.May,
                            June = l.June,
                            July = l.July,
                            August = l.August,
                            September = l.September,
                            October = l.October,
                            November = l.November,
                            December = l.December,
                            Status = ( int ) Status.Active,
                        };

                        bservice.Create( b );
                    }
                }

                #endregion

                #region Create Address (s)

                if ( model.Address != null )
                {
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
                    };

                    aservice.Create( address );
                }

                #endregion

                #region Any File Uploads

                if ( model.Files.NullableAny( f => f.File != null ) )
                {
                    foreach ( FileViewModel f in model.Files.Where( f => f.File != null ) )
                    {
                        // Create folder
                        string path = Server.MapPath( $"~/{VariableExtension.SystemRules.DocumentsLocation}/Clients/{client.Id}/" );

                        if ( !Directory.Exists( path ) )
                        {
                            Directory.CreateDirectory( path );
                        }

                        f.Name = f.Name ?? "File";

                        string now = DateTime.Now.ToString( "yyyyMMddHHmmss" );

                        Document doc = new Document()
                        {
                            Name = f.Name,
                            Category = f.Name,
                            ObjectId = client.Id,
                            ObjectType = "Client",
                            Title = f.File.FileName,
                            Size = f.File.ContentLength,
                            Description = f.Description,
                            Status = ( int ) Status.Active,
                            Type = Path.GetExtension( f.File.FileName ),
                            Location = $"Clients/{client.Id}/{now}-{f.File.FileName}"
                        };

                        dservice.Create( doc );

                        string fullpath = Path.Combine( path, $"{now}-{f.File.FileName}" );

                        f.File.SaveAs( fullpath );
                    }
                }

                #endregion

                scope.Complete();
            }

            Notify( "The Client was successfully created.", NotificationType.Success );

            return Clients( new PagingModel(), new CustomSearchModel() );
        }

        // GET: Client/EditClient/5
        [Requires( PermissionTo.Edit )]
        public ActionResult EditClient( int id )
        {
            using ( ClientService service = new ClientService() )
            using ( AddressService aservice = new AddressService() )
            using ( DocumentService dservice = new DocumentService() )
            using ( EstimatedLoadService eservice = new EstimatedLoadService() )
            using ( ClientKPIService kpiservice = new ClientKPIService() )
            {
                Client client = service.GetById( id );

                if ( client == null )
                {
                    Notify( "Sorry, the requested resource could not be found. Please try again", NotificationType.Error );

                    return PartialView( "_AccessDenied" );
                }

                Address address = aservice.Get( client.Id, "Client" );

                List<Document> documents = dservice.List( client.Id, "Client" );

                List<EstimatedLoad> loads = new List<EstimatedLoad>();

                bool unverified = ( client.Status == ( int ) PSPClientStatus.Unverified );

                if ( unverified )
                {
                    loads = eservice.List( client.Id, "Client" );
                }

                int? pspId = ( client.PSPClients.NullableAny() ) ? client.PSPClients?.FirstOrDefault()?.PSPId : CurrentUser?.PSPs?.FirstOrDefault()?.Id;

                #region Client

                ClientViewModel model = new ClientViewModel()
                {
                    PSPId = pspId,
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
                    Status = ( PSPClientStatus ) client.Status,
                    EditMode = true,

                    BBBEELevel = client.BBBEELevel,
                    CompanyType = ( CompanyType ) client.CompanyType,
                    NumberOfLostPallets = client.NumberOfLostPallets,
                    OtherTypeOfPalletUse = client.PalletTypeOther,
                    PSPName = client.PSPName,
                    ServiceType = ( ServiceType ) client.ServiceRequired,
                    TypeOfPalletUse = ( TypeOfPalletUse ) client.PalletType,

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

                    ClientBudgets = new List<ClientBudget>(),

                    Files = new List<FileViewModel>(),
                };

                #endregion

                #region Client Budgets

                if ( unverified && loads.NullableAny() )
                {
                    foreach ( EstimatedLoad l in loads )
                    {
                        model.ClientBudgets.Add( new ClientBudget()
                        {
                            Id = l.Id,
                            BudgetYear = l.BudgetYear,
                            Total = l.Total,
                            January = l.January,
                            February = l.February,
                            March = l.March,
                            April = l.April,
                            May = l.May,
                            June = l.June,
                            July = l.July,
                            August = l.August,
                            September = l.September,
                            October = l.October,
                            November = l.November,
                            December = l.December,
                        } );
                    }
                }
                else if ( client.ClientBudgets.NullableAny() )
                {
                    foreach ( ClientBudget l in client.ClientBudgets )
                    {
                        model.ClientBudgets.Add( new ClientBudget()
                        {
                            Id = l.Id,
                            BudgetYear = l.BudgetYear,
                            Total = l.Total,
                            January = l.January,
                            February = l.February,
                            March = l.March,
                            April = l.April,
                            May = l.May,
                            June = l.June,
                            July = l.July,
                            August = l.August,
                            September = l.September,
                            October = l.October,
                            November = l.November,
                            December = l.December,
                        } );
                    }
                }

                #endregion

                #region Client Files

                if ( documents.NullableAny() )
                {
                    foreach ( Document d in documents )
                    {
                        model.Files.Add( new FileViewModel()
                        {
                            Id = d.Id,
                            Name = d.Name,
                            Extension = d.Type,
                            Size = ( decimal ) d.Size,
                            Description = d.Description,
                        } );
                    }
                }

                #endregion

                return View( model );
            }
        }

        // POST: Client/EditClient/5
        [HttpPost]
        [Requires( PermissionTo.Edit )]
        public ActionResult EditClient( ClientViewModel model )
        {
            using ( ClientService cservice = new ClientService() )
            using ( AddressService aservice = new AddressService() )
            using ( TransactionScope scope = new TransactionScope() )
            using ( DocumentService dservice = new DocumentService() )
            using ( PSPClientService pcservice = new PSPClientService() )
            using ( ClientBudgetService bservice = new ClientBudgetService() )
            {
                //if ( !ModelState.IsValid )
                //{
                //    Notify( "Sorry, the selected Client was not updated. Please correct all errors and try again.", NotificationType.Error );

                //    return View( model );
                //}

                Client client = cservice.GetById( model.Id );

                #region Validations

                if ( client == null )
                {
                    Notify( "Sorry, that Client does not exist! Please specify a valid Role Id and try again.", NotificationType.Error );

                    return View( model );
                }

                if ( !string.IsNullOrEmpty( model.CompanyRegistrationNumber ) && model.CompanyRegistrationNumber.Trim().ToLower() != client.CompanyRegistrationNumber.Trim().ToLower() && cservice.ExistByCompanyRegistrationNumber( model.CompanyRegistrationNumber.Trim() ) )
                {
                    // Role already exist!
                    Notify( $"Sorry, a Client with the Registration Number \"{model.CompanyRegistrationNumber} ({model.CompanyRegistrationNumber})\" already exists!", NotificationType.Error );

                    return View( model );
                }

                #endregion

                #region Client

                client.Email = model.Email;
                client.TradingAs = model.TradingAs;
                client.VATNumber = model.VATNumber;
                client.AdminEmail = model.AdminEmail;
                client.Status = ( int ) model.Status;
                client.AdminPerson = model.AdminPerson;
                client.Description = model.Description;
                client.CompanyName = model.CompanyName;
                client.ChepReference = model.ChepReference;
                client.ContactNumber = model.ContactNumber;
                client.ContactPerson = model.ContactPerson;
                client.FinPersonEmail = model.FinPersonEmail;
                client.FinancialPerson = model.FinancialPerson;
                client.CompanyRegistrationNumber = model.CompanyRegistrationNumber;

                client.BBBEELevel = model.BBBEELevel;
                client.CompanyType = ( int ) model.CompanyType;
                client.NumberOfLostPallets = model.NumberOfLostPallets;
                client.PalletTypeOther = model.OtherTypeOfPalletUse;
                client.PSPName = model.PSPName;
                client.ServiceRequired = ( int ) model.ServiceType;
                client.PalletType = ( int ) model.TypeOfPalletUse;

                cservice.Update( client );

                #endregion

                #region Create Client PSP link

                if ( model.ServiceType == ServiceType.HaveCompany && ( ( model.PSPId > 0 ) ? model.PSPId : CurrentUser.PSPs?.FirstOrDefault()?.Id ).HasValue )
                {
                    int? pspId = ( model.PSPId > 0 ) ? model.PSPId : CurrentUser.PSPs?.FirstOrDefault()?.Id;

                    if ( client.PSPClients.NullableAny() )
                    {
                        pcservice.Query( $"DELETE FROM [PSPClient] WHERE ClientId={client.Id}" );
                    }

                    PSPClient pClient = new PSPClient()
                    {
                        PSPId = pspId.Value,
                        ClientId = client.Id,
                        Status = ( int ) Status.Active
                    };

                    pcservice.Create( pClient );
                }

                #endregion

                #region Client Budgets

                if ( model.ClientBudgets.NullableAny() )
                {
                    foreach ( ClientBudget l in model.ClientBudgets )
                    {
                        ClientBudget b = bservice.GetById( l.Id );

                        if ( b == null )
                        {
                            b = new ClientBudget()
                            {
                                ClientId = client.Id,
                                BudgetYear = l.BudgetYear,
                                Total = l.Total,
                                January = l.January,
                                February = l.February,
                                March = l.March,
                                April = l.April,
                                May = l.May,
                                June = l.June,
                                July = l.July,
                                August = l.August,
                                September = l.September,
                                October = l.October,
                                November = l.November,
                                December = l.December,
                                Status = ( int ) Status.Active,
                            };

                            bservice.Create( b );
                        }
                        else
                        {
                            b.BudgetYear = l.BudgetYear;
                            b.Total = l.Total;
                            b.January = l.January;
                            b.February = l.February;
                            b.March = l.March;
                            b.April = l.April;
                            b.May = l.May;
                            b.June = l.June;
                            b.July = l.July;
                            b.August = l.August;
                            b.September = l.September;
                            b.October = l.October;
                            b.November = l.November;
                            b.December = l.December;
                            b.Status = ( int ) Status.Active;

                            bservice.Update( b );
                        }
                    }
                }

                #endregion

                #region Address (s)

                if ( model.Address != null )
                {
                    Address address = aservice.GetById( model.Address.Id );

                    if ( address == null )
                    {
                        address = new Address()
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
                        };

                        aservice.Create( address );
                    }
                    else
                    {
                        address.Town = model.Address.Town;
                        address.PostalCode = model.Address.PostCode;
                        address.Type = ( int ) model.Address.AddressType;
                        address.Addressline1 = model.Address.AddressLine1;
                        address.Addressline2 = model.Address.AddressLine2;
                        address.Province = ( int ) model.Address.Province;

                        aservice.Update( address );
                    }
                }

                #endregion

                #region Any Uploads

                if ( model.Files.NullableAny( f => f.File != null ) )
                {
                    foreach ( FileViewModel f in model.Files.Where( f => f.File != null ) )
                    {
                        string path = Server.MapPath( $"~/{VariableExtension.SystemRules.DocumentsLocation}/Clients/{client.Id}/" );

                        if ( !Directory.Exists( path ) )
                        {
                            Directory.CreateDirectory( path );
                        }

                        Document doc;

                        string now = DateTime.Now.ToString( "yyyyMMddHHmmss" );

                        if ( f.Name?.ToLower() == "logo" )
                        {
                            doc = dservice.Get( client.Id, "Client", f.Name );

                            if ( doc != null )
                            {
                                try
                                {
                                    string p = Server.MapPath( $"~/{VariableExtension.SystemRules.DocumentsLocation}/{doc.Location}" );

                                    if ( System.IO.File.Exists( p ) )
                                    {
                                        System.IO.File.Delete( p );
                                    }
                                }
                                catch ( Exception ex )
                                {

                                }

                                dservice.Delete( doc );
                            }
                        }

                        f.Name = f.Name ?? f.Description?.Replace( " ", "" ) ?? "File";

                        doc = new Document()
                        {
                            Name = f.Name,
                            Category = f.Name,
                            ObjectId = client.Id,
                            ObjectType = "Client",
                            Title = f.File.FileName,
                            Size = f.File.ContentLength,
                            Description = f.Description,
                            Status = ( int ) Status.Active,
                            Type = Path.GetExtension( f.File.FileName ),
                            Location = $"Clients/{client.Id}/{now}-{f.File.FileName}"
                        };

                        dservice.Create( doc );

                        string fullpath = Path.Combine( path, $"{now}-{f.File.FileName}" );

                        f.File.SaveAs( fullpath );
                    }
                }

                #endregion

                scope.Complete();
            }

            Notify( "The selected Client details were successfully updated.", NotificationType.Success );

            return Clients( new PagingModel(), new CustomSearchModel() );
        }

        // POST: /Client/DeleteClient/5
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

        //
        // GET: /Client/ApproveDeclineClient/5
        public ActionResult ApproveDeclineClient( int id )
        {
            using ( ClientService service = new ClientService() )
            using ( AddressService aservice = new AddressService() )
            using ( DocumentService dservice = new DocumentService() )
            using ( ClientKPIService kpiservice = new ClientKPIService() )
            using ( EstimatedLoadService eservice = new EstimatedLoadService() )
            {
                Client client = service.GetById( id );

                if ( client == null )
                {
                    Notify( "Sorry, the requested resource could not be found. Please try again", NotificationType.Error );

                    return PartialView( "_AccessDenied" );
                }

                Address address = aservice.Get( client.Id, "Client" );

                List<Document> documents = dservice.List( client.Id, "Client" );

                List<EstimatedLoad> loads = new List<EstimatedLoad>();

                bool unverified = ( client.Status == ( int ) PSPClientStatus.Unverified );

                if ( unverified )
                {
                    loads = eservice.List( client.Id, "Client" );
                }

                PSP psp = ( client.PSPClients.NullableAny() ) ? client.PSPClients?.FirstOrDefault()?.PSP : null;
                int? pspId = ( client.PSPClients.NullableAny() ) ? client.PSPClients?.FirstOrDefault()?.PSPId : CurrentUser?.PSPs?.FirstOrDefault()?.Id;

                #region Client

                ClientViewModel model = new ClientViewModel()
                {
                    PSP = psp,
                    PSPId = pspId,
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
                    Status = ( PSPClientStatus ) client.Status,
                    EditMode = true,

                    BBBEELevel = client.BBBEELevel,
                    CompanyType = ( CompanyType ) client.CompanyType,
                    NumberOfLostPallets = client.NumberOfLostPallets,
                    OtherTypeOfPalletUse = client.PalletTypeOther,
                    PSPName = client.PSPName,
                    ServiceType = ( ServiceType ) client.ServiceRequired,
                    TypeOfPalletUse = ( TypeOfPalletUse ) client.PalletType,

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

                    ClientBudgets = new List<ClientBudget>(),

                    Files = new List<FileViewModel>(),
                };

                #endregion

                #region Client Budgets

                if ( unverified && loads.NullableAny() )
                {
                    foreach ( EstimatedLoad l in loads )
                    {
                        model.ClientBudgets.Add( new ClientBudget()
                        {
                            Id = l.Id,
                            BudgetYear = l.BudgetYear,
                            Total = l.Total,
                            January = l.January,
                            February = l.February,
                            March = l.March,
                            April = l.April,
                            May = l.May,
                            June = l.June,
                            July = l.July,
                            August = l.August,
                            September = l.September,
                            October = l.October,
                            November = l.November,
                            December = l.December,
                        } );
                    }
                }
                else if ( client.ClientBudgets.NullableAny() )
                {
                    foreach ( ClientBudget l in client.ClientBudgets )
                    {
                        model.ClientBudgets.Add( new ClientBudget()
                        {
                            Id = l.Id,
                            BudgetYear = l.BudgetYear,
                            Total = l.Total,
                            January = l.January,
                            February = l.February,
                            March = l.March,
                            April = l.April,
                            May = l.May,
                            June = l.June,
                            July = l.July,
                            August = l.August,
                            September = l.September,
                            October = l.October,
                            November = l.November,
                            December = l.December,
                        } );
                    }
                }

                #endregion

                #region Client Files

                if ( documents.NullableAny() )
                {
                    foreach ( Document d in documents )
                    {
                        model.Files.Add( new FileViewModel()
                        {
                            Id = d.Id,
                            Name = d.Name,
                            Extension = d.Type,
                            Size = ( decimal ) d.Size,
                            Description = d.Description,
                        } );
                    }
                }

                #endregion

                return PartialView( "_ApproveDeclineClient", model );
            }
        }

        //
        // POST: /Client/ApproveDeclineClient/5
        [HttpPost]
        [Requires( PermissionTo.Edit )]
        public ActionResult ApproveDeclineClient( ClientViewModel model )
        {
            using ( ClientService cservice = new ClientService() )
            using ( TransactionScope scope = new TransactionScope() )
            using ( PSPClientService pcservice = new PSPClientService() )
            using ( ClientBudgetService bservice = new ClientBudgetService() )
            {
                Client client = cservice.GetById( model.Id );

                #region Validations

                if ( client == null )
                {
                    Notify( "Sorry, that Client does not exist! Please specify a valid Role Id and try again.", NotificationType.Error );

                    return View( model );
                }

                #endregion

                #region Client

                client.Status = ( int ) model.Status;
                client.ChepReference = model.ChepReference;

                if ( !string.IsNullOrEmpty( model.DeclineReason ) )
                {
                    client.DeclinedReason = model.DeclineReason;
                }

                cservice.Update( client );

                #endregion

                #region Client Budgets

                if ( model.ClientBudgets.NullableAny() )
                {
                    foreach ( ClientBudget l in model.ClientBudgets )
                    {
                        ClientBudget b = new ClientBudget()
                        {
                            ClientId = client.Id,
                            BudgetYear = l.BudgetYear,
                            Total = l.Total,
                            January = l.January,
                            February = l.February,
                            March = l.March,
                            April = l.April,
                            May = l.May,
                            June = l.June,
                            July = l.July,
                            August = l.August,
                            September = l.September,
                            October = l.October,
                            November = l.November,
                            December = l.December,
                            Status = ( int ) Status.Active,
                        };

                        bservice.Create( b );
                    }
                }

                #endregion

                Notify( $"The selected Client ({client.CompanyName}) was successfully approved.", NotificationType.Success );

                scope.Complete();
            }

            return Clients( new PagingModel(), new CustomSearchModel() );
        }

        //
        // GET: /Client/ClientUsers/5
        public ActionResult ClientUsers( int id )
        {
            using ( ClientService cservice = new ClientService() )
            {
                Client model = cservice.GetById( id );

                if ( model == null )
                {
                    Notify( "Sorry, the requested resource could not be found. Please try again", NotificationType.Error );

                    return PartialView( "_Notification" );
                }

                return PartialView( "_ClientUsers", model );
            }
        }

        //
        // GET: /Client/ClientProducts/5
        public ActionResult ClientProducts( int id )
        {
            using ( ClientService cservice = new ClientService() )
            {
                Client model = cservice.GetById( id );

                if ( model == null )
                {
                    Notify( "Sorry, the requested resource could not be found. Please try again", NotificationType.Error );

                    return PartialView( "_Notification" );
                }

                return PartialView( "_ClientProducts", model );
            }
        }

        //
        // GET: /Client/ClientInvoices/5
        public ActionResult ClientInvoices( int id )
        {
            using ( ClientService cservice = new ClientService() )
            {
                Client model = cservice.GetById( id );

                if ( model == null )
                {
                    Notify( "Sorry, the requested resource could not be found. Please try again", NotificationType.Error );

                    return PartialView( "_Notification" );
                }

                return PartialView( "_ClientInvoices", model );
            }
        }

        //
        // GET: /Client/ClientBudgets/5
        public ActionResult ClientBudgets( int id )
        {
            using ( ClientService cservice = new ClientService() )
            using ( EstimatedLoadService eservice = new EstimatedLoadService() )
            {
                Client model = cservice.GetById( id );

                if ( model == null )
                {
                    Notify( "Sorry, the requested resource could not be found. Please try again", NotificationType.Error );

                    return PartialView( "_Notification" );
                }

                if ( !model.ClientBudgets.NullableAny() )
                {
                    List<EstimatedLoad> loads = eservice.List( id, "Client" );

                    if ( loads.NullableAny() )
                    {
                        foreach ( EstimatedLoad l in loads )
                        {
                            model.ClientBudgets.Add( new ClientBudget()
                            {
                                Id = l.Id,
                                BudgetYear = l.BudgetYear,
                                Total = l.Total,
                                January = l.January,
                                February = l.February,
                                March = l.March,
                                April = l.April,
                                May = l.May,
                                June = l.June,
                                July = l.July,
                                August = l.August,
                                September = l.September,
                                October = l.October,
                                November = l.November,
                                December = l.December,
                            } );
                        }
                    }
                }

                return PartialView( "_ClientBudgetsView", model );
            }
        }

        //
        // POST: /Client/DeleteClientBudget/5
        [HttpPost]
        [Requires( PermissionTo.Delete )]
        public ActionResult DeleteClientBudget( int id )
        {
            using ( ClientBudgetService bservice = new ClientBudgetService() )
            {
                ClientBudget b = bservice.GetById( id );

                if ( b == null )
                {
                    Notify( "Sorry, the requested resource could not be found. Please try again", NotificationType.Error );

                    return PartialView( "_AccessDenied" );
                }

                int clientId = b.ClientId;

                bservice.Delete( b );

                Notify( "The selected Budget was successfully Deleted.", NotificationType.Success );

                List<ClientBudget> model = bservice.ListByColumnWhere( "ClientId", clientId );

                return PartialView( "_ClientBudgets", model );
            }
        }

        //
        // POST: /Client/DeleteClientFile/5
        [HttpPost]
        [Requires( PermissionTo.Delete )]
        public ActionResult DeleteClientDocument( int id, bool selfDestruct = false )
        {
            using ( DocumentService dservice = new DocumentService() )
            {
                Document doc = dservice.GetById( id );

                if ( doc == null )
                {
                    Notify( "Sorry, the requested resource could not be found. Please try again", NotificationType.Error );

                    return PartialView( "_AccessDenied" );
                }

                int clientId = doc.ObjectId ?? 0;

                try
                {
                    string p = Server.MapPath( $"~/{VariableExtension.SystemRules.DocumentsLocation}/{doc.Location}" );

                    if ( System.IO.File.Exists( p ) )
                    {
                        System.IO.File.Delete( p );
                    }
                }
                catch ( Exception ex )
                {

                }

                dservice.Delete( doc );

                if ( selfDestruct )
                {
                    return PartialView( "_Empty" );
                }

                List<FileViewModel> model = new List<FileViewModel>();

                List<Document> documents = dservice.List( clientId, "Client" );

                if ( documents.NullableAny() )
                {
                    foreach ( Document d in documents )
                    {
                        model.Add( new FileViewModel()
                        {
                            Id = d.Id,
                            Name = d.Name,
                            Extension = d.Type,
                            Size = ( decimal ) d.Size,
                            Description = d.Description,
                        } );
                    }
                }

                return PartialView( "_ClientDocuments", model );
            }
        }

        #endregion



        #region KPIS

        //
        // GET: /Client/ClientDetails/5
        public ActionResult KPIDetails( int id, bool layout = true )
        {
            using ( ClientKPIService kservice = new ClientKPIService() )
            {
                ClientKPI model = kservice.GetById( id );

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

        // GET: Client/AddKPI
        [Requires( PermissionTo.Create )]
        public ActionResult AddKPI()
        {
            ClientKPIViewModel model = new ClientKPIViewModel() { EditMode = true };

            return View( model );
        }

        // POST: Client/AddKPI
        [HttpPost]
        [Requires( PermissionTo.Create )]
        public ActionResult AddKPI( ClientKPIViewModel model )
        {
            if ( !ModelState.IsValid )
            {
                Notify( "Sorry, the Client KPI was not created. Please correct all errors and try again.", NotificationType.Error );

                return View( model );
            }

            using ( ClientKPIService kservice = new ClientKPIService() )
            {
                #region Validation

                if ( kservice.ExistByKPIDescription( model.KPIDescription.Trim(), model.ClientId ) )
                {
                    // Bank already exist!
                    Notify( $"Sorry, a KPI with the Description \"{model.KPIDescription}\" already exists for the selected client!", NotificationType.Error );

                    return View( model );
                }

                #endregion

                #region Create Client KPI

                ClientKPI kpi = new ClientKPI()
                {
                    Weight = model.Weight,
                    Passons = model.Passons,
                    ClientId = model.ClientId,
                    Disputes = model.Disputes,
                    Status = ( int ) model.Status,
                    MonthlyCost = model.MonthlyCost,
                    ResolveDays = model.ResolveDays,
                    TargetAmount = model.TargetAmount,
                    KPIDescription = model.KPIDescription,
                    OutstandingDays = model.OutstandingDays,
                    TargetPeriod = ( int ) model.TargetPeriod,
                    OutstandingPallets = model.OutstandingPallets,
                };

                kservice.Create( kpi );

                #endregion

                Notify( "The Client KPI was successfully created.", NotificationType.Success );

                return ClientKPI( new PagingModel(), new CustomSearchModel() );
            }
        }

        // GET: Client/EditKPI/5
        [Requires( PermissionTo.Edit )]
        public ActionResult EditKPI( int id )
        {
            using ( ClientKPIService kservice = new ClientKPIService() )
            {
                ClientKPI kpi = kservice.GetById( id );

                if ( kpi == null )
                {
                    Notify( "Sorry, the requested resource could not be found. Please try again", NotificationType.Error );

                    return PartialView( "_AccessDenied" );
                }

                ClientKPIViewModel model = new ClientKPIViewModel()
                {
                    Id = kpi.Id,
                    EditMode = true,
                    Weight = kpi.Weight,
                    Passons = kpi.Passons,
                    Disputes = kpi.Disputes,
                    ClientId = kpi.ClientId,
                    MonthlyCost = kpi.MonthlyCost,
                    ResolveDays = kpi.ResolveDays,
                    Status = ( Status ) kpi.Status,
                    TargetAmount = kpi.TargetAmount,
                    KPIDescription = kpi.KPIDescription,
                    OutstandingDays = kpi.OutstandingDays,
                    OutstandingPallets = kpi.OutstandingPallets,
                    TargetPeriod = ( TargetPeriod ) kpi.TargetPeriod,
                };

                return View( model );
            }
        }

        // POST: Client/EditKPI/5
        [HttpPost]
        [Requires( PermissionTo.Edit )]
        public ActionResult EditKPI( ClientKPIViewModel model )
        {
            if ( !ModelState.IsValid )
            {
                Notify( "Sorry, the selected Client was not updated. Please correct all errors and try again.", NotificationType.Error );

                return View( model );
            }

            using ( ClientKPIService kservice = new ClientKPIService() )
            {
                ClientKPI kpi = kservice.GetById( model.Id );

                #region Validation

                if ( kpi == null )
                {
                    Notify( "Sorry, that Client KPI does not exist! Please specify a valid Role Id and try again.", NotificationType.Error );

                    return View( model );
                }

                if ( kpi.KPIDescription.Trim().ToLower() != model.KPIDescription.Trim().ToLower() && kservice.ExistByKPIDescription( model.KPIDescription.Trim(), model.ClientId ) )
                {
                    // KPI already exist!
                    Notify( $"Sorry, a KPI with the Description \"{model.KPIDescription}\" already exists for the selected client!", NotificationType.Error );

                    return View( model );
                }

                #endregion

                #region Update Client KPI

                // Update Client KPI
                kpi.Id = model.Id;
                kpi.Weight = model.Weight;
                kpi.Passons = model.Passons;
                kpi.ClientId = model.ClientId;
                kpi.Disputes = model.Disputes;
                kpi.Status = ( int ) model.Status;
                kpi.MonthlyCost = model.MonthlyCost;
                kpi.ResolveDays = model.ResolveDays;
                kpi.TargetAmount = model.TargetAmount;
                kpi.KPIDescription = model.KPIDescription;
                kpi.OutstandingDays = model.OutstandingDays;
                kpi.TargetPeriod = ( int ) model.TargetPeriod;
                kpi.OutstandingPallets = model.OutstandingPallets;

                kservice.Update( kpi );

                #endregion

                Notify( "The selected Client KPI details were successfully updated.", NotificationType.Success );

                return ClientKPI( new PagingModel(), new CustomSearchModel() );
            }
        }

        // POST: Client/DeleteKPI/5
        [HttpPost]
        [Requires( PermissionTo.Delete )]
        public ActionResult DeleteKPI( ClientViewModel model )
        {
            using ( ClientKPIService service = new ClientKPIService() )
            {
                ClientKPI kpi = service.GetById( model.Id );

                if ( kpi == null )
                {
                    Notify( "Sorry, the requested resource could not be found. Please try again", NotificationType.Error );

                    return PartialView( "_AccessDenied" );
                }

                kpi.Status = ( ( ( Status ) kpi.Status ) == Status.Active ) ? ( int ) Status.Inactive : ( int ) Status.Active;

                service.Update( kpi );

                Notify( "The selected Client KPI was successfully updated.", NotificationType.Success );

                return ClientKPI( new PagingModel(), new CustomSearchModel() );
            }
        }

        #endregion



        #region Client Group

        //
        // GET: /Client/GroupDetails/5
        public ActionResult GroupDetails( int id, bool layout = true )
        {
            using ( GroupService gservice = new GroupService() )
            {
                Group model = gservice.GetById( id );

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

        // GET: Client/AddGroup
        [Requires( PermissionTo.Create )]
        public ActionResult AddGroup()
        {
            GroupViewModel model = new GroupViewModel() { EditMode = true, Clients = new List<ClientGroupViewModel>() };

            return View( model );
        }

        // POST: Client/AddGroup
        [HttpPost]
        [Requires( PermissionTo.Create )]
        public ActionResult AddGroup( GroupViewModel model )
        {
            if ( !ModelState.IsValid )
            {
                Notify( "Sorry, the Group was not created. Please correct all errors and try again.", NotificationType.Error );

                return View( model );
            }

            using ( GroupService gservice = new GroupService() )
            using ( TransactionScope scope = new TransactionScope() )
            using ( ClientGroupService cgservice = new ClientGroupService() )
            {
                #region Create Group

                Group group = new Group()
                {
                    Name = model.Name,
                    Description = model.Description,
                    Status = ( int ) Status.Active
                };

                group = gservice.Create( group );

                #endregion

                #region Create Group Client

                if ( model.Clients.NullableAny( c => c.ClientId > 0 ) )
                {
                    foreach ( ClientGroupViewModel cg in model.Clients.Where( c => c.ClientId > 0 ) )
                    {
                        ClientGroup client = new ClientGroup()
                        {
                            GroupId = group.Id,
                            ClientId = cg.ClientId,
                            Status = ( int ) Status.Active
                        };

                        cgservice.Create( client );
                    }
                }

                #endregion

                scope.Complete();

                Notify( "The Group was successfully created.", NotificationType.Success );
            }

            return ClientGroups( new PagingModel(), new CustomSearchModel() );
        }

        // GET: Client/EditGroupGet/5
        [Requires( PermissionTo.Edit )]
        public ActionResult EditGroup( int id )
        {
            using ( GroupService service = new GroupService() )
            {
                Group group = service.GetById( id );

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
                    Status = ( Status ) group.Status,
                    EditMode = true,
                    Clients = new List<ClientGroupViewModel>()
                };

                if ( group.ClientGroups.NullableAny() )
                {
                    foreach ( ClientGroup cg in group.ClientGroups )
                    {
                        model.Clients.Add( new ClientGroupViewModel()
                        {
                            GroupId = cg.GroupId,
                            ClientId = cg.ClientId,
                        } );
                    }
                }

                return View( "EditGroup", model );
            }
        }

        // POST: Client/EditGroup/5
        [HttpPost]
        [Requires( PermissionTo.Edit )]
        public ActionResult EditGroup( GroupViewModel model )
        {
            if ( !ModelState.IsValid )
            {
                Notify( "Sorry, the selected Group was not updated. Please correct all errors and try again.", NotificationType.Error );

                return View( model );
            }

            using ( GroupService service = new GroupService() )
            using ( TransactionScope scope = new TransactionScope() )
            using ( ClientGroupService cgservice = new ClientGroupService() )
            {
                Group group = service.GetById( model.Id );

                #region Update Group

                group.Name = model.Name;
                group.Status = ( int ) model.Status;
                group.Description = model.Description;

                service.Update( group );

                #endregion

                #region Create Group Client

                cgservice.Query( $"DELETE FROM [dbo].[ClientGroup] WHERE [GroupId]={group.Id};" );

                if ( model.Clients.NullableAny( c => c.ClientId > 0 ) )
                {
                    foreach ( ClientGroupViewModel cg in model.Clients.Where( c => c.ClientId > 0 ) )
                    {
                        ClientGroup client = new ClientGroup()
                        {
                            GroupId = group.Id,
                            ClientId = cg.ClientId,
                            Status = ( int ) Status.Active
                        };

                        cgservice.Create( client );
                    }
                }

                #endregion

                scope.Complete();

                Notify( "The selected Group details were successfully updated.", NotificationType.Success );
            }

            return ClientGroups( new PagingModel(), new CustomSearchModel() );
        }

        // POST: Client/DeleteGroup/5
        [HttpPost]
        [Requires( PermissionTo.Delete )]
        public ActionResult DeleteGroup( GroupViewModel model )
        {
            using ( GroupService service = new GroupService() )
            {
                Group group = service.GetById( model.Id );

                if ( group == null )
                {
                    Notify( "Sorry, the requested resource could not be found. Please try again", NotificationType.Error );

                    return PartialView( "_AccessDenied" );
                }

                group.Status = ( ( ( Status ) group.Status ) == Status.Active ) ? ( int ) Status.Inactive : ( int ) Status.Active;

                service.Update( group );

                Notify( "The selected Group was successfully updated.", NotificationType.Success );
            }

            return ClientGroups( new PagingModel(), new CustomSearchModel() );
        }

        #endregion



        #region Manage Sites

        //
        // GET: /Client/SiteDetails/5
        public ActionResult SiteDetails( int id, bool layout = true )
        {
            using ( SiteService sservice = new SiteService() )
            using ( AddressService aservice = new AddressService() )
            {
                Site model = sservice.GetById( id );

                if ( model == null )
                {
                    Notify( "Sorry, the requested resource could not be found. Please try again", NotificationType.Error );

                    return PartialView( "_AccessDenied" );
                }

                Address address = aservice.Get( model.Id, "Site" );

                if ( layout )
                {
                    ViewBag.IncludeLayout = true;
                }

                ViewBag.Address = address;

                return View( model );
            }
        }

        // GET: Client/AddSite
        [Requires( PermissionTo.Create )]
        public ActionResult AddSite()
        {
            SiteViewModel model = new SiteViewModel()
            {
                EditMode = true,
                Clients = new List<ClientCustomer>(),
                SiteBudgets = new List<SiteBudget>(),
                Address = new AddressViewModel()
            };

            return View( model );
        }

        // POST: Client/Site
        [HttpPost]
        [Requires( PermissionTo.Create )]
        public ActionResult AddSite( SiteViewModel model )
        {
            if ( !ModelState.IsValid )
            {
                Notify( "Sorry, the Site was not created. Please correct all errors and try again.", NotificationType.Error );

                return View( model );
            }

            using ( SiteService sservice = new SiteService() )
            using ( AddressService aservice = new AddressService() )
            using ( TransactionScope scope = new TransactionScope() )
            using ( ClientSiteService csservice = new ClientSiteService() )
            using ( SiteBudgetService sbservice = new SiteBudgetService() )
            {
                #region Validation

                if ( sservice.ExistByNameRegion( model.Name?.Trim()?.ToLower(), model.RegionId ) )
                {
                    Notify( $"Sorry, a Site with the name {model.Name} in the specified region already exists.", NotificationType.Error );

                    return View( model );
                }

                Site existingSite = sservice.ExistByXYCoords( model.Longitude, model.Latitude );

                if ( existingSite != null )
                {
                    // Instead of pass back to view, we will create the new site as a subsite of the existing site.
                    // Get the existing site first
                    model.SiteId = existingSite.Id;
                }

                #endregion

                #region Create Site

                Site site = new Site()
                {
                    Name = model.Name,
                    Depot = model.Depot,
                    SiteId = model.SiteId,
                    RegionId = model.RegionId,
                    ContactNo = model.ContactNo,
                    Status = ( int ) model.Status,
                    ContactName = model.ContactName,
                    Description = model.Description,
                    AccountCode = model.AccountCode,
                    SiteType = ( int ) model.SiteType,
                    SiteCodeChep = model.SiteCodeChep,
                    YCord = model.Latitude,
                    XCord = model.Longitude,
                    PlanningPoint = model.PlanningPoint,
                    FinanceContact = model.FinanceContact,
                    ReceivingContact = model.ReceivingContact,
                    FinanceContactNo = model.FinanceContactNo,
                    ReceivingContactNo = model.ReceivingContactNo,
                };

                site = sservice.Create( site );

                #endregion

                #region Create Address (s)

                if ( model.Address != null )
                {
                    Address address = new Address()
                    {
                        ObjectId = site.Id,
                        ObjectType = "Site",
                        Town = model.Address.Town,
                        Latitude = model.Latitude,
                        Longitude = model.Longitude,
                        Status = ( int ) Status.Active,
                        Type = ( int ) AddressType.Postal,
                        PostalCode = model.Address.PostCode,
                        Addressline1 = model.Address.AddressLine1,
                        Addressline2 = model.Address.AddressLine2,
                        Province = ( int ) model.Address.Province,
                    };

                    aservice.Create( address );
                }

                #endregion

                #region Add Client Site

                if ( model.Clients.NullableAny( c => c.Id > 0 ) )
                {
                    foreach ( ClientCustomer c in model.Clients.Where( c => c.Id > 0 ) )
                    {
                        if ( c.Id <= 0 ) continue;

                        ClientSite csSite = new ClientSite()
                        {
                            SiteId = site.Id,
                            ClientCustomerId = c.Id,
                            Status = ( int ) model.Status,
                            AccountingCode = site.AccountCode,
                        };

                        csservice.Create( csSite );
                    }
                }

                #endregion

                #region Create Client Budget

                if ( model.SiteBudgets.NullableAny() )
                {
                    foreach ( SiteBudget l in model.SiteBudgets )
                    {
                        SiteBudget sb = new SiteBudget()
                        {
                            SiteId = site.Id,
                            BudgetYear = l.BudgetYear,
                            Total = l.Total,
                            January = l.January,
                            February = l.February,
                            March = l.March,
                            April = l.April,
                            May = l.May,
                            June = l.June,
                            July = l.July,
                            August = l.August,
                            September = l.September,
                            October = l.October,
                            November = l.November,
                            December = l.December,
                            Status = ( int ) model.Status,
                        };

                        sbservice.Create( sb );
                    }
                }

                #endregion

                scope.Complete();
            }

            Notify( "The Site was successfully created.", NotificationType.Success );

            return ManageSites( new PagingModel(), new CustomSearchModel() );
        }

        // GET: Client/EditSite/5
        [Requires( PermissionTo.Edit )]
        public ActionResult EditSite( int id )
        {
            using ( SiteService service = new SiteService() )
            using ( RegionService rservice = new RegionService() )
            using ( AddressService aservice = new AddressService() )
            {
                Site site = service.GetById( id );

                if ( site == null )
                {
                    Notify( "Sorry, the requested resource could not be found. Please try again", NotificationType.Error );

                    return PartialView( "_AccessDenied" );
                }

                Address address = aservice.Get( site.Id, "Site" );

                #region Site

                SiteViewModel model = new SiteViewModel()
                {
                    Id = site.Id,
                    EditMode = true,
                    Name = site.Name,
                    Depot = site.Depot,
                    Latitude = site.YCord,
                    Longitude = site.XCord,
                    RegionId = site.RegionId,
                    ContactNo = site.ContactNo,
                    ContactName = site.ContactName,
                    Description = site.Description,
                    AccountCode = site.AccountCode,
                    Status = ( Status ) site.Status,
                    SiteCodeChep = site.SiteCodeChep,
                    PlanningPoint = site.PlanningPoint,
                    FinanceContact = site.FinanceContact,
                    FinanceContactNo = site.FinanceContactNo,
                    ReceivingContact = site.ReceivingContact,
                    ReceivingContactNo = site.ReceivingContactNo,
                    SiteType = ( site.SiteType.HasValue ? ( SiteType ) site.SiteType : SiteType.All ),


                    Clients = new List<ClientCustomer>(),
                    SiteBudgets = new List<SiteBudget>(),
                };

                #endregion

                #region Clients

                if ( site.ClientSites.NullableAny() )
                {
                    foreach ( int cid in site.ClientSites.Select( s => s.ClientCustomerId ) )
                    {
                        model.Clients.Add( new ClientCustomer()
                        {
                            Id = cid
                        } );
                    }
                }

                #endregion

                #region Site Budgets

                if ( site.SiteBudgets.NullableAny() )
                {
                    foreach ( SiteBudget l in site.SiteBudgets )
                    {
                        model.SiteBudgets.Add( new SiteBudget()
                        {
                            Id = l.Id,
                            BudgetYear = l.BudgetYear,
                            Total = l.Total,
                            January = l.January,
                            February = l.February,
                            March = l.March,
                            April = l.April,
                            May = l.May,
                            June = l.June,
                            July = l.July,
                            August = l.August,
                            September = l.September,
                            October = l.October,
                            November = l.November,
                            December = l.December,
                        } );
                    }
                }

                #endregion

                #region Address

                if ( address != null )
                {
                    model.Address = new AddressViewModel()
                    {
                        EditMode = true,
                        Town = address?.Town,
                        Id = address?.Id ?? 0,
                        PostCode = address?.PostalCode,
                        AddressLine1 = address?.Addressline1,
                        AddressLine2 = address?.Addressline2,
                        Province = ( address != null ) ? ( Province ) address.Province : Province.All,
                        Longitude = address.Longitude,
                        Latitude = address.Latitude,
                    };
                }

                #endregion

                return View( model );
            }
        }

        // POST: Client/EditSite/5
        [HttpPost]
        [Requires( PermissionTo.Edit )]
        public ActionResult EditSite( SiteViewModel model )
        {
            if ( !ModelState.IsValid )
            {
                Notify( "Sorry, the selected Site was not updated. Please correct all errors and try again.", NotificationType.Error );

                return View( model );
            }

            using ( SiteService sservice = new SiteService() )
            using ( AddressService aservice = new AddressService() )
            using ( TransactionScope scope = new TransactionScope() )
            using ( ClientSiteService csservice = new ClientSiteService() )
            using ( SiteBudgetService sbservice = new SiteBudgetService() )
            {
                Site site = sservice.GetById( model.Id );

                #region Validations

                if ( site == null )
                {
                    Notify( "Sorry, the requested resource could not be found. Please try again", NotificationType.Error );

                    return PartialView( "_AccessDenied" );
                }

                if ( site.Name?.Trim()?.ToLower() != model.Name?.Trim()?.ToLower() && site.RegionId != model.RegionId && sservice.ExistByNameRegion( model.Name?.Trim()?.ToLower(), model.RegionId ) )
                {
                    Notify( $"Sorry, a Site with the name {model.Name} in the specified region already exists.", NotificationType.Error );

                    return View( model );
                }

                Site existingSite = sservice.ExistByXYCoords( model.Longitude?.Trim(), model.Latitude?.Trim() );

                if ( existingSite != null && site.Id != existingSite.Id && site.SiteId != existingSite.Id )
                {
                    // Instead of pass back to view, we will create the new site as a subsite of the existing site.
                    // Get the existing site first
                    model.SiteId = existingSite.Id;
                }

                #endregion

                #region Update Site

                site.Name = model.Name;
                site.Depot = model.Depot;
                site.YCord = model.Latitude;
                site.XCord = model.Longitude;
                site.RegionId = model.RegionId;
                site.ContactNo = model.ContactNo;
                site.Status = ( int ) model.Status;
                site.Description = model.Description;
                site.ContactName = model.ContactName;
                site.AccountCode = model.AccountCode;
                site.SiteType = ( int ) model.SiteType;
                site.SiteCodeChep = model.SiteCodeChep;
                site.PlanningPoint = model.PlanningPoint;
                site.FinanceContact = model.FinanceContact;
                site.ReceivingContact = model.ReceivingContact;
                site.FinanceContactNo = model.FinanceContactNo;
                site.ReceivingContactNo = model.ReceivingContactNo;

                sservice.Update( site );

                #endregion

                #region Add Client Site

                csservice.Query( $"UPDATE [dbo].[ClientSite] SET [Status]={( int ) Status.Inactive} WHERE SiteId={site.Id}" );

                if ( model.Clients.NullableAny( c => c.Id > 0 ) )
                {
                    foreach ( ClientCustomer c in model.Clients.Where( c => c.Id > 0 ) )
                    {
                        if ( c.Id <= 0 ) continue;

                        ClientSite cs = csservice.GetBySiteId( c.Id, site.Id );

                        if ( cs != null )
                        {
                            cs.Status = ( int ) model.Status;
                            cs.AccountingCode = site.AccountCode;

                            csservice.Update( cs );
                        }
                        else
                        {
                            cs = new ClientSite()
                            {
                                SiteId = site.Id,
                                ClientCustomerId = c.Id,
                                Status = ( int ) model.Status,
                                AccountingCode = site.AccountCode,
                            };

                            csservice.Create( cs );
                        }
                    }
                }

                #endregion

                #region Client Budgets

                if ( model.SiteBudgets.NullableAny() )
                {
                    foreach ( SiteBudget l in model.SiteBudgets )
                    {
                        SiteBudget b = sbservice.GetById( l.Id );

                        if ( b == null )
                        {
                            b = new SiteBudget()
                            {
                                SiteId = site.Id,
                                BudgetYear = l.BudgetYear,
                                Total = l.Total,
                                January = l.January,
                                February = l.February,
                                March = l.March,
                                April = l.April,
                                May = l.May,
                                June = l.June,
                                July = l.July,
                                August = l.August,
                                September = l.September,
                                October = l.October,
                                November = l.November,
                                December = l.December,
                                Status = ( int ) Status.Active,
                            };

                            sbservice.Create( b );
                        }
                        else
                        {
                            b.BudgetYear = l.BudgetYear;
                            b.Total = l.Total;
                            b.January = l.January;
                            b.February = l.February;
                            b.March = l.March;
                            b.April = l.April;
                            b.May = l.May;
                            b.June = l.June;
                            b.July = l.July;
                            b.August = l.August;
                            b.September = l.September;
                            b.October = l.October;
                            b.November = l.November;
                            b.December = l.December;
                            b.Status = ( int ) Status.Active;

                            sbservice.Update( b );
                        }
                    }
                }

                #endregion

                #region Address (s)

                if ( model.Address != null )
                {
                    Address address = aservice.GetById( model.Address.Id );

                    if ( address == null )
                    {
                        address = new Address()
                        {
                            ObjectId = model.Id,
                            ObjectType = "Client",
                            Town = model.Address.Town,
                            Latitude = model.Latitude,
                            Longitude = model.Longitude,
                            Status = ( int ) Status.Active,
                            PostalCode = model.Address.PostCode,
                            Type = ( int ) model.Address.AddressType,
                            Addressline1 = model.Address.AddressLine1,
                            Addressline2 = model.Address.AddressLine2,
                            Province = ( int ) model.Address.Province,
                        };

                        aservice.Create( address );
                    }
                    else
                    {
                        address.Town = model.Address.Town;
                        address.Latitude = model.Latitude;
                        address.Longitude = model.Longitude;
                        address.PostalCode = model.Address.PostCode;
                        address.Type = ( int ) model.Address.AddressType;
                        address.Addressline1 = model.Address.AddressLine1;
                        address.Addressline2 = model.Address.AddressLine2;
                        address.Province = ( int ) model.Address.Province;

                        aservice.Update( address );
                    }
                }

                #endregion

                scope.Complete();
            }

            Notify( "The selected Site details were successfully updated.", NotificationType.Success );

            return ManageSites( new PagingModel(), new CustomSearchModel() );
        }

        // POST: Client/DeleteSite/5
        [HttpPost]
        [Requires( PermissionTo.Delete )]
        public ActionResult DeleteSite( SiteViewModel model )
        {
            using ( SiteService service = new SiteService() )
            {
                Site site = service.GetById( model.Id );

                if ( site == null )
                {
                    Notify( "Sorry, the requested resource could not be found. Please try again", NotificationType.Error );

                    return PartialView( "_AccessDenied" );
                }

                site.Status = ( ( ( Status ) site.Status ) == Status.Active ) ? ( int ) Status.Inactive : ( int ) Status.Active;

                service.Update( site );

                Notify( "The selected Site was successfully updated.", NotificationType.Success );

                return ManageSites( new PagingModel(), new CustomSearchModel() );
            }
        }

        //
        // POST: /Site/DeleteSiteBudget/5
        [HttpPost]
        [Requires( PermissionTo.Delete )]
        public ActionResult DeleteSiteBudget( int id )
        {
            using ( SiteBudgetService bservice = new SiteBudgetService() )
            {
                SiteBudget b = bservice.GetById( id );

                if ( b == null )
                {
                    Notify( "Sorry, the requested resource could not be found. Please try again", NotificationType.Error );

                    return PartialView( "_AccessDenied" );
                }

                int SiteId = b.SiteId;

                bservice.Delete( b );

                Notify( "The selected Budget was successfully Deleted.", NotificationType.Success );

                List<SiteBudget> model = bservice.ListByColumnWhere( "SiteId", SiteId );

                return PartialView( "_SiteBudgets", model );
            }
        }

        // GET: Client/AddSite
        [Requires( PermissionTo.Create )]
        public ActionResult ImportSite()
        {
            SiteViewModel model = new SiteViewModel();

            return View( model );
        }

        // POST: Client/AddSite
        [HttpPost]
        [Requires( PermissionTo.Create )]
        public ActionResult ImportSite( SiteViewModel model )
        {
            if ( model.SiteImportFile == null )
            {
                Notify( "Please select a file to upload and try again.", NotificationType.Error );

                return View( model );
            }

            using ( SiteService sservice = new SiteService() )
            using ( AddressService aservice = new AddressService() )
            using ( ClientSiteService csservice = new ClientSiteService() )
            using ( IExcelDataReader reader = ExcelReaderFactory.CreateReader( model.SiteImportFile.InputStream ) )
            {
                // DataSet - The result of each spreadsheet will be created in the result.Tables
                DataSet result = reader.AsDataSet();

                int rowNo = 0,
                    skipped = 0,
                    processed = 0;

                while ( rowNo < result.Tables[ 1 ].Rows.Count )
                {
                    using ( TransactionScope scope = new TransactionScope() )
                    {
                        string xCord = result.Tables[ 1 ].Rows[ rowNo ][ 10 ]?.ToString(),
                           yCord = result.Tables[ 1 ].Rows[ rowNo ][ 11 ]?.ToString();

                        /*Site s = sservice.GetByXYCoordinates( model.ClientId, xCord, yCord );

                        if ( s == null )
                        {
                            s = new Site()
                            {

                            };

                            s = sservice.Create( s );
                        }
                        else
                        {
                            s = sservice.Update( s );
                        }*/



                        rowNo++;
                        processed++;

                        scope.Complete();

                    }
                }
            }

            return View( model );
        }

        #endregion



        #region Sub Sites

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
                                    Rate = hrate,
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
            List<ClientCustomModel> clientList = new List<ClientCustomModel>();
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
                    clientList = clientService.List1( new PagingModel(), new CustomSearchModel() { Status = Status.Active, ClientId = clientId } );
                }
            }
            else
            {
                using ( ProductService service = new ProductService() )
                using ( ClientService clientService = new ClientService() )
                {
                    model = service.ListCSM( pm, csm );
                    total = ( model.Count < pm.Take && pm.Skip == 0 ) ? model.Count : service.Total1( pm, csm );
                    clientList = clientService.List1( new PagingModel(), new CustomSearchModel() { Status = Status.Active, ClientId = clientId } );
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
                    product.Rate = hrate;
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



        #region General

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

                    using ( SiteService sservice = new SiteService() )
                    using ( AddressService aservice = new AddressService() )
                    using ( TransactionScope scope = new TransactionScope() )
                    using ( ClientSiteService csservice = new ClientSiteService() )
                    using ( StreamReader sreader = new StreamReader( postedFile.InputStream ) )
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

                            Site existingSite = sservice.ExistByXYCoords( strLngX, strLatY );

                            #region Validation

                            if ( !string.IsNullOrEmpty( strLngX ) && existingSite != null )
                            {
                                //Notify($"Sorry, a Site with the same X Y Coordinates already exists \"{model.XCord}\" already exists!", NotificationType.Error);
                                //return View(model);

                                //rather than pass back to view, we will create the new site as a subsite of the existing site. 
                                //Get the existing site first
                                existingSite = sservice.GetByColumnsWhere( "XCord", strLngX, "YCord", strLatY );
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

                            site = sservice.Create( site );

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

                            #region Add Client Site

                            ClientSite csSite = new ClientSite()
                            {
                                ClientCustomerId = clientID,
                                SiteId = site.Id,
                                AccountingCode = site.AccountCode,
                                Status = ( int ) Status.Active
                            };

                            csservice.Create( csSite );

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

                            Site existingSite = siteService.ExistByXYCoords( strLngX, strLatY );

                            #region Validation

                            if ( !string.IsNullOrEmpty( strLngX ) && existingSite != null )
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
                                ClientCustomerId = clientID,
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



        #region Manage Transporters

        // GET: Client/AddTransporter
        [Requires(PermissionTo.Create)]
        public ActionResult AddTransporter()
        {
            TransporterViewModel model = new TransporterViewModel() { EditMode = true };
            return View(model);
        }


        // POST: Client/Transporter
        [HttpPost]
        [Requires(PermissionTo.Create)]
        public ActionResult AddTransporter(TransporterViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    Notify("Sorry, the Site was not created. Please correct all errors and try again.", NotificationType.Error);

                    return View(model);
                }

                using (TransporterService siteService = new TransporterService())
                using (TransactionScope scope = new TransactionScope())
                {
                    //#region Validation
                    //if (!string.IsNullOrEmpty(model.RegistrationNumber) && siteService.ExistByName(model.RegistrationNumber.Trim()))
                    //{
                    //    // Bank already exist!
                    //    Notify($"Sorry, a Site with the Account number \"{model.AccountCode}\" already exists!", NotificationType.Error);

                    //    return View(model);
                    //}
                    //#endregion
                    #region Create Transporter
                    Transporter site = new Transporter()
                    {
                        Name = model.Name,
                        TradingName = model.TradingName,
                        RegistrationNumber = model.RegistrationNumber,
                        Email = model.Email,
                        ContactNumber = model.ContactNumber,
                        Status = (int)Status.Active
                    };
                    site = siteService.Create(site);
                    #endregion

                    scope.Complete();
                }

                Notify("The Transporter was successfully created.", NotificationType.Success);
                return RedirectToAction("ManageTransporters");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return View();
            }
        }



        // GET: Client/EditTransporter/5
        [Requires(PermissionTo.Edit)]
        public ActionResult EditTransporter(int id)
        {
            Transporter site;
            //int pspId = Session[ "UserPSP" ];
            //int pspId = ( CurrentUser != null ? CurrentUser.PSPs.FirstOrDefault().Id : 0 );



            using (TransporterService service = new TransporterService())
            using (AddressService aservice = new AddressService())
            {
                site = service.GetById(id);

                if (site == null)
                {
                    Notify("Sorry, the requested resource could not be found. Please try again", NotificationType.Error);

                    return PartialView("_AccessDenied");
                }

                Address address = aservice.Get(site.Id, "Site");


                bool unverified = (site.Status == (int)PSPClientStatus.Unverified);

                TransporterViewModel model = new TransporterViewModel()
                {
                    Id = site.Id,
                    Name = site.Name,
                    TradingName = site.TradingName,
                    RegistrationNumber = site.RegistrationNumber,
                    Email = site.Email,
                    ContactNumber = site.ContactNumber,
                    // Status = (int)Status.Active
                    Status = (int)site.Status,
                    EditMode = true
                };
                return View(model);
            }
        }

        // POST: Client/EditTransporter/5
        [HttpPost]
        [Requires(PermissionTo.Edit)]
        public ActionResult EditTransporter(TransporterViewModel model, PagingModel pm, bool isstructure = false)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    Notify("Sorry, the selected Transporter was not updated. Please correct all errors and try again.", NotificationType.Error);

                    return View(model);
                }

                Transporter site;

                using (TransporterService service = new TransporterService())
                using (TransactionScope scope = new TransactionScope())
                {
                    site = service.GetById(model.Id);


                    #region Validations

                    //if (!string.IsNullOrEmpty(model.AccountCode) && service.ExistByAccountCode(model.AccountCode.Trim()))
                    //{
                    //    // Role already exist!
                    //    Notify($"Sorry, a Site with the Account Code \"{model.AccountCode} ({model.AccountCode})\" already exists!", NotificationType.Error);

                    //    return View(model);
                    //}

                    #endregion
                    #region Update Transporter

                    // Update Transporter
                    site.Id = model.Id;
                    site.Name = model.Name;
                    site.RegistrationNumber = model.RegistrationNumber;
                    site.Email = model.Email;
                    site.ContactNumber = model.ContactNumber;
                    site.TradingName = model.TradingName;
                    site.Status = (int)model.Status;

                    service.Update(site);

                    #endregion




                    scope.Complete();
                }

                Notify("The selected Transporter details were successfully updated.", NotificationType.Success);

                return RedirectToAction("ManageTransporters");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return View();
            }
        }

        // POST: Client/DeleteTransporter/5
        [HttpPost]
        [Requires(PermissionTo.Delete)]
        public ActionResult DeleteTransporter(SiteViewModel model)
        {
            Transporter site;
            try
            {

                using (TransporterService service = new TransporterService())
                using (TransactionScope scope = new TransactionScope())
                {
                    site = service.GetById(model.Id);

                    if (site == null)
                    {
                        Notify("Sorry, the requested resource could not be found. Please try again", NotificationType.Error);

                        return PartialView("_AccessDenied");
                    }

                    site.Status = (((Status)site.Status) == Status.Active) ? (int)Status.Inactive : (int)Status.Active;

                    service.Update(site);
                    scope.Complete();

                }
                Notify("The selected Transporter was successfully updated.", NotificationType.Success);
                return RedirectToAction("ManageSites");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return View();
            }
        }

        #endregion



        #region Partial Views

        //
        // POST || GET: /Client/Clients
        public ActionResult Clients( PagingModel pm, CustomSearchModel csm, bool givecsm = false )
        {
            if ( givecsm )
            {
                ViewBag.ViewName = "Clients";

                return PartialView( "_ClientsCustomSearch", new CustomSearchModel( "Clients" ) );
            }

            using ( ClientService service = new ClientService() )
            {
                pm.Sort = pm.Sort ?? "ASC";
                pm.SortBy = pm.SortBy ?? "c.CompanyName";

                List<ClientCustomModel> model = service.List1( pm, csm );

                int total = ( model.Count < pm.Take && pm.Skip == 0 ) ? model.Count : service.Total1( pm, csm );

                PagingExtension paging = PagingExtension.Create( model, total, pm.Skip, pm.Take, pm.Page );


                return PartialView( "_Clients", paging );
            }
        }

        //
        // POST || GET: /Client/ClientKPIS
        public ActionResult ClientKPI( PagingModel pm, CustomSearchModel csm, bool givecsm = false )
        {
            if ( givecsm )
            {
                ViewBag.ViewName = "ClientKPI";

                return PartialView( "_ClientKPICustomSearch", new CustomSearchModel( "ClientKPI" ) );
            }

            using ( ClientKPIService service = new ClientKPIService() )
            {
                pm.Sort = pm.Sort ?? "ASC";
                pm.SortBy = pm.SortBy ?? "c.CompanyName";

                List<ClientKPICustomModel> model = service.List1( pm, csm );

                int total = ( model.Count < pm.Take && pm.Skip == 0 ) ? model.Count : service.Total1( pm, csm );

                PagingExtension paging = PagingExtension.Create( model, total, pm.Skip, pm.Take, pm.Page );

                return PartialView( "_ClientKPI", paging );
            }
        }

        //
        // GET: /Client/ClientGroups
        public ActionResult ClientGroups( PagingModel pm, CustomSearchModel csm, bool givecsm = false )
        {
            if ( givecsm )
            {
                ViewBag.ViewName = "ClientGroups";

                return PartialView( "_ClientGroupsCustomSearch", new CustomSearchModel( "ClientGroups" ) );
            }

            using ( GroupService gservice = new GroupService() )
            {
                pm.Sort = pm.Sort ?? "DESC";
                pm.SortBy = pm.SortBy ?? "Name";

                List<GroupCustomModel> model = gservice.List1( pm, csm );

                int total = ( model.Count < pm.Take && pm.Skip == 0 ) ? model.Count : gservice.Total1( pm, csm );

                PagingExtension paging = PagingExtension.Create( model, total, pm.Skip, pm.Take, pm.Page );

                return PartialView( "_ClientGroups", paging );
            }
        }

        //
        // GET: /Client/ManageSites
        public ActionResult ManageSites( PagingModel pm, CustomSearchModel csm, bool givecsm = false )
        {
            if ( givecsm )
            {
                ViewBag.ViewName = "ManageSites";

                return PartialView( "_ManageSitesCustomSearch", new CustomSearchModel( "ManageSites" ) );
            }

            using ( SiteService service = new SiteService() )
            {
                pm.Sort = pm.Sort ?? "ASC";
                pm.SortBy = pm.SortBy ?? "s.Name";

                List<SiteCustomModel> model = service.List1( pm, csm );
                int total = ( model.Count < pm.Take && pm.Skip == 0 ) ? model.Count : service.Total1( pm, csm );

                PagingExtension paging = PagingExtension.Create( model, total, pm.Skip, pm.Take, pm.Page );

                return PartialView( "_ManageSites", paging );
            }
        }

        //
        // POST || GET: /Client/SubSites
        public ActionResult SubSites( int siteId = 0 )
        {

            ViewBag.ViewName = "_SubSites";

            int total = 0;

            List<Site> model = new List<Site>();
            List<ClientCustomModel> clientList = new List<ClientCustomModel>();
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
                clientList = clientService.List1( new PagingModel(), new CustomSearchModel() { ClientId = clientId, Status = Status.Active } );
            }
            IEnumerable<SelectListItem> clientDDL = clientList.Select( c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.CompanyName

            } );
            ViewBag.ClientList = clientDDL;

            ViewBag.ContextualModeSite = ( siteId > 0 ? true : false ); //Whether a client is specific or not and the View can know about it

            //using ( SiteService service = new SiteService() )
            //{
            //    mainSiteList = service.List1( new PagingModel() { Sort = "ASC", SortBy = "Name" }, new CustomSearchModel() { SiteId = siteId, ClientId = clientId } );

            //}
            //IEnumerable<SelectListItem> siteDDL = mainSiteList.Select( c => new SelectListItem
            //{
            //    Value = c.Id.ToString(),
            //    Text = c.Name

            //} );
            //ViewBag.ClientSiteList = siteDDL;
            ViewBag.SelectedSiteId = siteId;


            return PartialView( "_SubSites" );
        }

        // GET: /Client/ManageTransporters
        public ActionResult ManageTransporters(PagingModel pm, CustomSearchModel csm, bool givecsm = false)
        {

            ViewBag.ViewName = "ManageTransporters";
            if (givecsm)
            {
                ViewBag.ViewName = "ManageTransporters";

                return PartialView("_ManageTransportersCustomSearch", new CustomSearchModel("ManageTransporters"));
            }
            int total = 0;

            List<Transporter> model = new List<Transporter>();
            //int pspId = Session[ "UserPSP" ];
            //int pspId = ( CurrentUser != null ? CurrentUser.PSPs.FirstOrDefault().Id : 0 );
            using (TransporterService service = new TransporterService())
            {
                pm.Sort = pm.Sort ?? "DESC";
                pm.SortBy = pm.SortBy ?? "CreatedOn";

                model = service.List(pm, csm);
                total = (model.Count < pm.Take && pm.Skip == 0) ? model.Count : service.Total(pm, csm);
            }

            PagingExtension paging = PagingExtension.Create(model, total, pm.Skip, pm.Take, pm.Page);

            return PartialView("_ManageTransporters", paging);
        }

        #endregion
    }
}
