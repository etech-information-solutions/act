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
                                                    ( address != null ? ( (ProvinceEnum) address.Province ).GetDisplayText() : string.Empty ),
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

                    #region Link Products

                    csv = string.Format( "Id, ClientId, Company Name, ProductId, Product Name, Product Description, Active Date, HireRate, LostRate, IssueRate, PassonRate, PassonDays, Status {0}", Environment.NewLine );

                    List<ClientProductCustomModel> product = new List<ClientProductCustomModel>();

                    using ( ClientProductService service = new ClientProductService() )
                    {
                        product = service.List1( pm, csm );
                    }

                    if ( product != null && product.Any() )
                    {
                        foreach ( ClientProductCustomModel item in product )
                        {
                            csv = string.Format( "{0} {1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13} {14}",
                                                csv,
                                                item.Id,
                                                item.ClientId,
                                                item.ClientName,
                                                item.ProductId,
                                                item.ProductName,
                                                item.ProductDescription,
                                                item.ActiveDate,
                                                item.Rate,
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
                        Province = ( address != null ) ? (ProvinceEnum) address.Province : ProvinceEnum.All,
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
                        Province = ( address != null ) ? (ProvinceEnum) address.Province : ProvinceEnum.All,
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
                        Province = ( address != null ) ? (ProvinceEnum) address.Province : ProvinceEnum.All,
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
                            ObjectType = "Site",
                            ObjectId = model.Id,
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
            SiteViewModel model = new SiteViewModel() { EditMode = true };

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

            int rowNo = 0,
                skipped = 0,
                processed = 0;

            using ( SiteService sservice = new SiteService() )
            using ( RegionService rservice = new RegionService() )
            using ( AddressService aservice = new AddressService() )
            using ( ClientSiteService csservice = new ClientSiteService() )
            using ( ClientCustomerService ccservice = new ClientCustomerService() )
            using ( IExcelDataReader reader = ExcelReaderFactory.CreateReader( model.SiteImportFile.InputStream ) )
            {
                // DataSet - The result of each spreadsheet will be created in the result.Tables
                DataSet result = reader.AsDataSet();

                while ( rowNo < result.Tables[ 1 ].Rows.Count )
                {
                    if ( rowNo < 2 )
                    {
                        rowNo++;

                        continue;
                    }

                    try
                    {
                        using ( TransactionScope scope = new TransactionScope() )
                        {
                            string siteNumber = result.Tables[ 1 ].Rows[ rowNo ][ 2 ]?.ToString()?.Trim(),
                                   customerNumber = result.Tables[ 1 ].Rows[ rowNo ][ 3 ]?.ToString()?.Trim(),
                                   customerName = result.Tables[ 1 ].Rows[ rowNo ][ 4 ]?.ToString()?.Trim(),
                                   siteName = result.Tables[ 1 ].Rows[ rowNo ][ 5 ]?.ToString()?.Trim(),
                                   addressLine1 = result.Tables[ 1 ].Rows[ rowNo ][ 6 ]?.ToString()?.Trim(),
                                   addressLine2 = result.Tables[ 1 ].Rows[ rowNo ][ 7 ]?.ToString()?.Trim(),
                                   town = result.Tables[ 1 ].Rows[ rowNo ][ 8 ]?.ToString()?.Trim(),
                                   xCord = result.Tables[ 1 ].Rows[ rowNo ][ 9 ]?.ToString()?.Trim(),
                                   yCord = result.Tables[ 1 ].Rows[ rowNo ][ 10 ]?.ToString()?.Trim(),
                                   kamContact = result.Tables[ 1 ].Rows[ rowNo ][ 13 ]?.ToString()?.Trim(),
                                   managerContact = result.Tables[ 1 ].Rows[ rowNo ][ 14 ]?.ToString()?.Trim(),
                                   liquorLicenseNumber = result.Tables[ 1 ].Rows[ rowNo ][ 17 ]?.ToString()?.Trim(),
                                   kamName = result.Tables[ 1 ].Rows[ rowNo ][ 19 ]?.ToString()?.Trim(),
                                   region = result.Tables[ 1 ].Rows[ rowNo ][ 20 ]?.ToString()?.Trim(),
                                   province = result.Tables[ 1 ].Rows[ rowNo ][ 21 ]?.ToString()?.Trim(),
                                   salesManager = result.Tables[ 1 ].Rows[ rowNo ][ 23 ]?.ToString()?.Trim(),
                                   salesRepCluster = result.Tables[ 1 ].Rows[ rowNo ][ 24 ]?.ToString()?.Trim(),
                                   salesRep = result.Tables[ 1 ].Rows[ rowNo ][ 25 ]?.ToString()?.Trim();

                            if ( string.IsNullOrEmpty( siteName ) || string.IsNullOrEmpty( customerName ) || string.IsNullOrEmpty( customerNumber ) )
                            {
                                rowNo++;
                                skipped++;

                                continue;
                            }

                            Region r = rservice.Search( region, province );

                            #region Site

                            Site s = sservice.GetByXYCoordinates( xCord, yCord );

                            if ( s == null )
                            {
                                // Create new site

                                s = new Site()
                                {
                                    XCord = xCord,
                                    YCord = yCord,
                                    Name = siteName,
                                    RegionId = r?.Id,
                                    Depot = siteNumber,
                                    Description = siteName,
                                    AccountCode = siteNumber,
                                    ContactName = salesManager,
                                    ContactNo = managerContact,
                                    DepotManager = salesManager,
                                    FinanceContact = salesManager,
                                    Status = ( int ) Status.Active,
                                    ReceivingContact = salesManager,
                                    FinanceContactNo = managerContact,
                                    ReceivingContactNo = managerContact,
                                    DepotManagerContact = managerContact,
                                    Address = $"{addressLine1}\n {addressLine2}\n {town}",
                                };

                                s = sservice.Create( s );
                            }
                            else if ( s != null && s?.Name?.Trim().ToLower() == siteName?.Trim().ToLower() )
                            {
                                // Same site, update

                                s.XCord = xCord;
                                s.YCord = yCord;
                                s.Name = siteName;
                                s.Depot = siteNumber;
                                s.Description = siteName;
                                s.AccountCode = siteNumber;
                                s.ContactName = salesManager;
                                s.ContactNo = managerContact;
                                s.DepotManager = salesManager;
                                s.FinanceContact = salesManager;
                                s.Status = ( int ) Status.Active;
                                s.ReceivingContact = salesManager;
                                s.FinanceContactNo = managerContact;
                                s.ReceivingContactNo = managerContact;
                                s.DepotManagerContact = managerContact;
                                s.Address = $"{addressLine1}\n {addressLine2}\n {town}";
                                s.RegionId = ( r?.Id ?? s.RegionId );

                                s = sservice.Update( s );
                            }
                            else if ( s != null && s?.Name?.Trim().ToLower() != siteName?.Trim().ToLower() )
                            {
                                // Create new site, as a Sub-site

                                s = new Site()
                                {
                                    SiteId = s.Id,
                                    XCord = xCord,
                                    YCord = yCord,
                                    Name = siteName,
                                    RegionId = r?.Id,
                                    Depot = siteNumber,
                                    Description = siteName,
                                    AccountCode = siteNumber,
                                    ContactName = salesManager,
                                    ContactNo = managerContact,
                                    DepotManager = salesManager,
                                    FinanceContact = salesManager,
                                    Status = ( int ) Status.Active,
                                    ReceivingContact = salesManager,
                                    FinanceContactNo = managerContact,
                                    ReceivingContactNo = managerContact,
                                    DepotManagerContact = managerContact,
                                    Address = $"{addressLine1}\n {addressLine2}\n {town}",
                                };

                                s = sservice.Create( s );
                            }

                            if ( s == null )
                            {
                                rowNo++;
                                skipped++;

                                continue;
                            }

                            #endregion

                            #region Client Customer

                            ClientCustomer cc = ccservice.GetByNumber( model.ClientId, customerNumber );

                            if ( cc == null )
                            {
                                cc = new ClientCustomer()
                                {
                                    CustomerTown = town,
                                    ClientId = model.ClientId,
                                    CustomerName = customerName,
                                    Status = ( int ) Status.Active,
                                    CustomerNumber = customerNumber,
                                    CustomerAddress1 = addressLine1,
                                    CustomerAddress2 = addressLine2,
                                    CustomerContact = managerContact,
                                };

                                cc = ccservice.Create( cc );
                            }
                            else
                            {
                                cc.CustomerTown = town;
                                cc.CustomerName = customerName;
                                cc.Status = ( int ) Status.Active;
                                cc.CustomerNumber = customerNumber;
                                cc.CustomerAddress1 = addressLine1;
                                cc.CustomerAddress2 = addressLine2;
                                cc.CustomerContact = managerContact;

                                cc = ccservice.Update( cc );
                            }

                            #endregion

                            #region Client Site

                            ClientSite cs = csservice.GetBySiteId( cc.Id, s.Id );

                            if ( cs == null )
                            {
                                cs = new ClientSite()
                                {
                                    SiteId = s.Id,
                                    KAMName = kamName,
                                    KAMContact = kamContact,
                                    ClientCustomerId = cc.Id,
                                    ClientSalesRep = salesRep,
                                    AccountingCode = siteNumber,
                                    Status = ( int ) Status.Active,
                                    ClientSalesManager = salesManager,
                                    ClientManagerContact = managerContact,
                                    ClientCustomerNumber = customerNumber,
                                    ClientSalesRepContact = managerContact,
                                    LiquorLicenceNumber = liquorLicenseNumber,
                                };

                                csservice.Create( cs );
                            }
                            else
                            {
                                cs.SiteId = s.Id;
                                cs.KAMName = kamName;
                                cs.KAMContact = kamContact;
                                cs.ClientCustomerId = cc.Id;
                                cs.ClientSalesRep = salesRep;
                                cs.AccountingCode = siteNumber;
                                cs.Status = ( int ) Status.Active;
                                cs.ClientSalesManager = salesManager;
                                cs.ClientManagerContact = managerContact;
                                cs.ClientCustomerNumber = customerNumber;
                                cs.ClientSalesRepContact = managerContact;
                                cs.LiquorLicenceNumber = liquorLicenseNumber;

                                csservice.Update( cs );
                            }

                            #endregion

                            #region Site Address

                            if ( !Enum.TryParse( province?.Trim().Replace( " ", "" ), out ProvinceEnum prov ) )
                            {
                                prov = ProvinceEnum.All;
                            }

                            Address a = aservice.Get( s.Id, "Site" );

                            if ( a == null )
                            {
                                a = new Address()
                                {
                                    Town = town,
                                    ObjectId = s.Id,
                                    Latitude = yCord,
                                    Longitude = xCord,
                                    PostalCode = town,
                                    ObjectType = "Site",
                                    Province = ( int ) prov,
                                    Addressline1 = addressLine1,
                                    Addressline2 = addressLine2,
                                    Status = ( int ) Status.Active,
                                    Type = ( int ) AddressType.Postal,
                                };

                                aservice.Create( a );
                            }
                            else
                            {
                                a.Town = town;
                                a.Latitude = yCord;
                                a.Longitude = xCord;
                                a.PostalCode = town;
                                a.Province = ( int ) prov;
                                a.Addressline1 = addressLine1;
                                a.Addressline2 = addressLine2;
                                a.Type = ( int ) AddressType.Postal;

                                aservice.Update( a );
                            }

                            #endregion

                            rowNo++;
                            processed++;

                            scope.Complete();
                        }
                    }
                    catch ( Exception ex )
                    {
                        rowNo++;

                    }
                }
            }

            Notify( $"{processed} sites were successfully uploaded, {skipped} were skipped.", NotificationType.Success );

            return ManageSites( new PagingModel(), new CustomSearchModel() );
        }

        #endregion



        #region Client Products

        //
        // GET: /Client/ProductDetails/5
        public ActionResult ProductDetails( int id, bool layout = true )
        {
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

                List<Document> documents = dservice.List( model.ProductId, "Product" );

                if ( documents != null )
                {
                    ViewBag.Documents = documents;
                }

                return View( model );
            }
        }

        //
        // GET: /Client/LinkProduct
        [Requires( PermissionTo.Create )]
        public ActionResult AddProduct()
        {
            ProductViewModel model = new ProductViewModel() { LinkMode = true };

            return View( model );
        }

        //
        // POST: /Client/LinkProduct
        [HttpPost]
        [Requires( PermissionTo.Create )]
        public ActionResult AddProduct( ProductViewModel model )
        {
            using ( ClientService cservice = new ClientService() )
            using ( ClientProductService cpservice = new ClientProductService() )
            {
                Client c = cservice.GetById( model.ClientId );

                if ( c == null )
                {
                    Notify( "Sorry, the selected Client was not found. Please select a valid client and try again.", NotificationType.Error );

                    return View( model );
                }

                if ( !c.PSPClients.Any() )
                {
                    Notify( "Sorry, the selected Client is not linked to any PSP. Please select a valid client and try again or contact us for further assistance.", NotificationType.Error );

                    return View( model );
                }

                ClientProduct cp = new ClientProduct()
                {
                    Rate = model.HireRate,
                    LostRate = model.LostRate,
                    ClientId = model.ClientId,
                    ProductId = model.ProductId,
                    Equipment = model.Equipment,
                    IssueRate = model.IssueRate,
                    PassonDays = model.PassonDays,
                    PassonRate = model.PassonRate,
                    Status = ( int ) model.Status,
                    ActiveDate = model.ActiveDate,
                    RateType = ( int ) model.RateType,
                    AccountingCode = model.AccountingCode,
                    ProductDescription = model.Description,
                    PSPId = c.PSPClients.FirstOrDefault().PSPId,
                };

                cpservice.Create( cp );

                Notify( "The selected Product was successfully linked to the selected client.", NotificationType.Success );

                return LinkProducts( new PagingModel(), new CustomSearchModel() );
            }
        }

        //
        // GET: /Client/EditProduct/5
        [Requires( PermissionTo.Edit )]
        public ActionResult EditProduct( int id )
        {
            using ( ClientProductService pservice = new ClientProductService() )
            {
                ClientProduct cp = pservice.GetById( id );

                if ( cp == null )
                {
                    Notify( "Sorry, the requested resource could not be found. Please try again", NotificationType.Error );

                    return PartialView( "_AccessDenied" );
                }

                ProductViewModel model = new ProductViewModel()
                {
                    Id = cp.Id,
                    LinkMode = true,
                    HireRate = cp.Rate,
                    Name = cp.Product.Name,
                    ClientId = cp.ClientId,
                    LostRate = cp.LostRate,
                    Equipment = cp.Equipment,
                    ProductId = cp.ProductId,
                    IssueRate = cp.IssueRate,
                    ActiveDate = cp.ActiveDate,
                    PassonRate = cp.PassonRate,
                    PassonDays = cp.PassonDays,
                    Status = ( Status ) cp.Status,
                    AccountingCode = cp.AccountingCode,
                    RateType = ( RateType ) cp.RateType,
                    Description = cp.ProductDescription,
                };

                return View( model );
            }
        }

        //
        // POST: /Client/EditProduct/5
        [HttpPost]
        [Requires( PermissionTo.Edit )]
        public ActionResult EditProduct( ProductViewModel model )
        {
            using ( ClientProductService cpservice = new ClientProductService() )
            {
                ClientProduct cp = cpservice.GetById( model.Id );

                #region Validations

                if ( cp == null )
                {
                    Notify( "Sorry, that Client Product does not exist! Please try again.", NotificationType.Error );

                    return View( model );
                }

                #endregion

                #region Product

                cp.Rate = model.HireRate;
                cp.LostRate = model.LostRate;
                cp.Equipment = model.Equipment;
                cp.IssueRate = model.IssueRate;
                cp.PassonDays = model.PassonDays;
                cp.PassonRate = model.PassonRate;
                cp.Status = ( int ) model.Status;
                cp.ActiveDate = model.ActiveDate;
                cp.RateType = ( int ) model.RateType;
                cp.AccountingCode = model.AccountingCode;
                cp.ProductDescription = model.Description;

                #endregion

                cpservice.Update( cp );

                Notify( "The selected Client Product's details were successfully updated.", NotificationType.Success );
            }

            return LinkProducts( new PagingModel(), new CustomSearchModel() );
        }

        //
        // POST: /Client/DeleteProduct/5
        [HttpPost]
        [Requires( PermissionTo.Delete )]
        public ActionResult DeleteProduct( ProductViewModel model )
        {
            using ( ClientProductService service = new ClientProductService() )
            {
                ClientProduct cp = service.GetById( model.Id );

                if ( cp == null )
                {
                    Notify( "Sorry, the requested resource could not be found. Please try again", NotificationType.Error );

                    return PartialView( "_AccessDenied" );
                }

                cp.Status = ( ( ( Status ) cp.Status ) == Status.Active ) ? ( int ) Status.Inactive : ( int ) Status.Active;

                service.Update( cp );

                Notify( "The selected Client Product was successfully updated.", NotificationType.Success );
            }

            return LinkProducts( new PagingModel(), new CustomSearchModel() );
        }

        #endregion



        #region General

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
                                        provinceName = ( (ProvinceEnum) provinceId ).GetDisplayText();
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



        #region Manage Transporters

        //
        // GET: /Client/TransporterDetails/5
        public ActionResult TransporterDetails( int id, bool layout = true )
        {
            using ( ContactService cservice = new ContactService() )
            using ( VehicleService vservice = new VehicleService() )
            using ( TransporterService tservice = new TransporterService() )
            {
                Transporter model = tservice.GetById( id );

                if ( model == null )
                {
                    Notify( "Sorry, the requested resource could not be found. Please try again", NotificationType.Error );

                    return RedirectToAction( "Index" );
                }

                if ( layout )
                {
                    ViewBag.IncludeLayout = true;
                }

                ViewBag.Contacts = cservice.List( model.Id, "Transporter" );

                ViewBag.Vehicles = vservice.List( model.Id, "Transporter" );

                return View( model );
            }
        }

        // GET: Client/AddTransporter
        [Requires( PermissionTo.Create )]
        public ActionResult AddTransporter()
        {
            TransporterViewModel model = new TransporterViewModel()
            {
                EditMode = true,
                Contacts = new List<Contact>(),
                Vehicles = new List<Vehicle>()
            };

            return View( model );
        }

        // POST: Client/Transporter
        [HttpPost]
        [Requires( PermissionTo.Create )]
        public ActionResult AddTransporter( TransporterViewModel model )
        {
            if ( !ModelState.IsValid )
            {
                Notify( "Sorry, the Site was not created. Please correct all errors and try again.", NotificationType.Error );

                return View( model );
            }

            using ( ContactService cservice = new ContactService() )
            using ( VehicleService vservice = new VehicleService() )
            using ( TransactionScope scope = new TransactionScope() )
            using ( TransporterService tservice = new TransporterService() )
            {
                #region Validation

                if ( !string.IsNullOrEmpty( model.RegistrationNumber ) && tservice.ExistByRegistrationNumber( model.RegistrationNumber.Trim() ) )
                {
                    // Transporter already exist!
                    Notify( $"Sorry, a Transporter with the Registration Number \"{model.RegistrationNumber}\" already exists!", NotificationType.Error );

                    return View( model );
                }

                #endregion

                #region Transporter

                Transporter t = new Transporter()
                {
                    Name = model.Name,
                    Email = model.Email,
                    Status = ( int ) Status.Active,
                    TradingName = model.TradingName,
                    ContactNumber = model.ContactNumber,
                    RegistrationNumber = model.RegistrationNumber,
                };

                t = tservice.Create( t );

                #endregion

                #region Contacts

                if ( model.Contacts.NullableAny( c => !string.IsNullOrWhiteSpace( c.ContactName ) ) )
                {
                    foreach ( Contact mc in model.Contacts.Where( c => !string.IsNullOrWhiteSpace( c.ContactName ) ) )
                    {
                        Contact c = cservice.Get( mc.ContactIdNo, "Transporter" );

                        if ( c != null )
                        {
                            continue;
                        }

                        c = new Contact()
                        {
                            ObjectId = t.Id,
                            ObjectType = "Transporter",
                            ContactCell = mc.ContactCell,
                            ContactIdNo = mc.ContactIdNo,
                            ContactName = mc.ContactName,
                            Status = ( int ) model.Status,
                            ContactEmail = mc.ContactEmail,
                            ContactTitle = mc.ContactTitle,
                        };

                        cservice.Create( c );
                    }
                }

                #endregion

                #region Vehicles

                if ( model.Vehicles.NullableAny( c => !string.IsNullOrWhiteSpace( c.VINNumber ) ) )
                {
                    foreach ( Vehicle mv in model.Vehicles.Where( c => !string.IsNullOrWhiteSpace( c.VINNumber ) ) )
                    {
                        Vehicle v = vservice.Get( mv.VINNumber, "Transporter" );

                        if ( v != null )
                        {
                            continue;
                        }

                        v = new Vehicle()
                        {
                            Make = mv.Make,
                            Year = mv.Year,
                            ObjectId = t.Id,
                            Model = mv.Model,
                            Type = ( int ) mv.Type,
                            VINNumber = mv.VINNumber,
                            ObjectType = "Transporter",
                            Descriptoin = mv.Descriptoin,
                            Status = ( int ) model.Status,
                            Registration = mv.Registration,
                            EngineNumber = mv.EngineNumber,
                        };

                        vservice.Create( v );
                    }
                }

                #endregion

                scope.Complete();
            }

            Notify( "The Transporter was successfully created.", NotificationType.Success );

            return ManageTransporters( new PagingModel(), new CustomSearchModel() );
        }

        // GET: Client/EditTransporter/5
        [Requires( PermissionTo.Edit )]
        public ActionResult EditTransporter( int id )
        {
            using ( ContactService cservice = new ContactService() )
            using ( VehicleService vservice = new VehicleService() )
            using ( TransporterService tservice = new TransporterService() )
            {
                Transporter t = tservice.GetById( id );

                if ( t == null )
                {
                    Notify( "Sorry, the requested resource could not be found. Please try again", NotificationType.Error );

                    return PartialView( "_AccessDenied" );
                }

                List<Contact> contacts = cservice.List( t.Id, "Transporter" );
                List<Vehicle> vehicles = vservice.List( t.Id, "Transporter" );

                TransporterViewModel model = new TransporterViewModel()
                {
                    Id = t.Id,
                    Name = t.Name,
                    Email = t.Email,
                    EditMode = true,
                    Contacts = contacts,
                    Vehicles = vehicles,
                    TradingName = t.TradingName,
                    Status = ( Status ) t.Status,
                    ContactNumber = t.ContactNumber,
                    RegistrationNumber = t.RegistrationNumber,
                };

                return View( model );
            }
        }

        // POST: Client/EditTransporter/5
        [HttpPost]
        [Requires( PermissionTo.Edit )]
        public ActionResult EditTransporter( TransporterViewModel model )
        {
            if ( !ModelState.IsValid )
            {
                Notify( "Sorry, the selected Transporter was not updated. Please correct all errors and try again.", NotificationType.Error );

                return View( model );
            }

            using ( ContactService cservice = new ContactService() )
            using ( VehicleService vservice = new VehicleService() )
            using ( TransactionScope scope = new TransactionScope() )
            using ( TransporterService tservice = new TransporterService() )
            {
                Transporter t = tservice.GetById( model.Id );

                #region Validations

                if ( !string.IsNullOrEmpty( model.RegistrationNumber ) && model.RegistrationNumber.Trim().ToLower() != t.RegistrationNumber.Trim().ToLower() && tservice.ExistByRegistrationNumber( model.RegistrationNumber.Trim() ) )
                {
                    // Role already exist!
                    Notify( $"Sorry, a Transporter with the Company Registration Number \"{model.RegistrationNumber} ({model.RegistrationNumber})\" already exists!", NotificationType.Error );

                    return View( model );
                }

                #endregion

                #region Transporter

                // Update Transporter
                t.Id = model.Id;
                t.Name = model.Name;
                t.Email = model.Email;
                t.Status = ( int ) model.Status;
                t.TradingName = model.TradingName;
                t.ContactNumber = model.ContactNumber;
                t.RegistrationNumber = model.RegistrationNumber;

                tservice.Update( t );

                #endregion

                #region Contacts

                if ( model.Contacts.NullableAny( c => !string.IsNullOrWhiteSpace( c.ContactName ) ) )
                {
                    foreach ( Contact mc in model.Contacts.Where( c => !string.IsNullOrWhiteSpace( c.ContactName ) ) )
                    {
                        Contact c = cservice.GetById( mc.Id );

                        if ( c == null )
                        {
                            c = new Contact()
                            {
                                ObjectId = t.Id,
                                ObjectType = "Transporter",
                                ContactCell = mc.ContactCell,
                                ContactIdNo = mc.ContactIdNo,
                                ContactName = mc.ContactName,
                                Status = ( int ) model.Status,
                                ContactEmail = mc.ContactEmail,
                                ContactTitle = mc.ContactTitle,
                            };

                            cservice.Create( c );
                        }
                        else
                        {
                            c.ContactCell = mc.ContactCell;
                            c.ContactIdNo = mc.ContactIdNo;
                            c.ContactName = mc.ContactName;
                            c.Status = ( int ) model.Status;
                            c.ContactEmail = mc.ContactEmail;
                            c.ContactTitle = mc.ContactTitle;

                            cservice.Update( c );
                        }
                    }
                }

                #endregion

                #region Vehicles

                if ( model.Vehicles.NullableAny( c => !string.IsNullOrWhiteSpace( c.VINNumber ) ) )
                {
                    foreach ( Vehicle mv in model.Vehicles.Where( c => !string.IsNullOrWhiteSpace( c.VINNumber ) ) )
                    {
                        Vehicle v = vservice.GetById( mv.Id );

                        if ( v == null )
                        {
                            v = new Vehicle()
                            {
                                Make = mv.Make,
                                Year = mv.Year,
                                ObjectId = t.Id,
                                Model = mv.Model,
                                Type = ( int ) mv.Type,
                                VINNumber = mv.VINNumber,
                                ObjectType = "Transporter",
                                Status = ( int ) model.Status,
                                Registration = mv.Registration,
                                EngineNumber = mv.EngineNumber,
                                Descriptoin = $"{mv.Make} {mv.Model}",
                            };

                            vservice.Create( v );
                        }
                        else
                        {
                            v.Make = mv.Make;
                            v.Year = mv.Year;
                            v.Model = mv.Model;
                            v.Type = ( int ) mv.Type;
                            v.VINNumber = mv.VINNumber;
                            v.Status = ( int ) model.Status;
                            v.Registration = mv.Registration;
                            v.EngineNumber = mv.EngineNumber;
                            v.Descriptoin = $"{mv.Make} {mv.Model}";

                            vservice.Update( v );
                        }
                    }
                }

                #endregion

                scope.Complete();
            }

            Notify( "The selected Transporter details were successfully updated.", NotificationType.Success );

            return ManageTransporters( new PagingModel(), new CustomSearchModel() );
        }

        // POST: Client/DeleteTransporter/5
        [HttpPost]
        [Requires( PermissionTo.Delete )]
        public ActionResult DeleteTransporter( TransporterViewModel model )
        {
            using ( TransporterService service = new TransporterService() )
            {
                Transporter t = service.GetById( model.Id );

                if ( t == null )
                {
                    Notify( "Sorry, the requested resource could not be found. Please try again", NotificationType.Error );

                    return PartialView( "_AccessDenied" );
                }

                t.Status = ( ( ( Status ) t.Status ) == Status.Active ) ? ( int ) Status.Inactive : ( int ) Status.Active;

                service.Update( t );

                Notify( "The selected Transporter was successfully updated.", NotificationType.Success );

                return ManageTransporters( new PagingModel(), new CustomSearchModel() );
            }
        }

        // GET: Client/TransporterContacts/5
        public ActionResult TransporterContacts( int id )
        {
            using ( ContactService cservice = new ContactService() )
            {
                List<Contact> contacts = cservice.List( id, "Transporter" );

                return PartialView( "_ContactsView", contacts );
            }
        }

        // GET: Client/TransporterVehicles/5
        public ActionResult TransporterVehicles( int id )
        {
            using ( VehicleService vservice = new VehicleService() )
            {
                List<Vehicle> vehicles = vservice.List( id, "Transporter" );

                return PartialView( "_VehiclesView", vehicles );
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

        //
        // POST || GET: /Client/LinkProducts
        public ActionResult LinkProducts( PagingModel pm, CustomSearchModel csm, bool givecsm = false )
        {
            if ( givecsm )
            {
                ViewBag.ViewName = "LinkProducts";

                return PartialView( "_LinkProductsCustomSearch", new CustomSearchModel( "LinkProducts" ) );
            }

            using ( ClientProductService service = new ClientProductService() )
            {
                pm.Sort = pm.Sort ?? "DESC";
                pm.SortBy = pm.SortBy ?? "CreatedOn";

                List<ClientProductCustomModel> model = service.List1( pm, csm );

                int total = ( model.Count < pm.Take && pm.Skip == 0 ) ? model.Count : service.Total1( pm, csm );

                PagingExtension paging = PagingExtension.Create( model, total, pm.Skip, pm.Take, pm.Page );

                return PartialView( "_LinkProducts", paging );
            }
        }

        // GET: /Client/ManageTransporters
        public ActionResult ManageTransporters( PagingModel pm, CustomSearchModel csm, bool givecsm = false )
        {
            ViewBag.ViewName = "ManageTransporters";

            if ( givecsm )
            {
                ViewBag.ViewName = "ManageTransporters";

                return PartialView( "_ManageTransportersCustomSearch", new CustomSearchModel() );
            }

            using ( TransporterService service = new TransporterService() )
            {
                pm.Sort = pm.Sort ?? "DESC";
                pm.SortBy = pm.SortBy ?? "CreatedOn";

                List<TransporterCustomModel> model = service.List1( pm, csm );

                int total = ( model.Count < pm.Take && pm.Skip == 0 ) ? model.Count : service.Total1( pm, csm );

                PagingExtension paging = PagingExtension.Create( model, total, pm.Skip, pm.Take, pm.Page );

                return PartialView( "_ManageTransporters", paging );
            }
        }

        #endregion
    }
}
