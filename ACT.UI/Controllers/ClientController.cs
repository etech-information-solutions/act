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
using Microsoft.VisualBasic.FileIO;

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
        public FileContentResult Export( PagingModel pm, CustomSearchModel csm, string type = "clients" )
        {
            string csv = string.Empty;
            string filename = string.Format( "{0}-{1}.csv", type.ToUpperInvariant(), DateTime.Now.ToString( "yyyy_MM_dd_HH_mm" ) );

            pm.Skip = 0;
            pm.Take = int.MaxValue;

            switch ( type )
            {
                case "clients":

                    #region Clients

                    using ( ClientService service = new ClientService() )
                    {
                        csv = string.Format( "Date Created, Company Name, Trading As, Reg #, Description, Chep reference, Status, Service Required, Type of Pallet Use, Other Type of Pallet Use, Company Type, PSP, VAT Number, BBBEE Level, Contact Person, Contact Number, Contact Email, Administrator, Administrator Email, Financial Person, Financial Person Email {0}", Environment.NewLine );

                        List<ClientCustomModel> clients = service.List1( pm, csm );

                        if ( clients.NullableAny() )
                        {
                            foreach ( ClientCustomModel item in clients )
                            {
                                csv = string.Format( "{0} {1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17},{18},{19},{20},{21} {22}",
                                                    csv,
                                                    "\"" + item.CreatedOn + "\"",
                                                    "\"" + item.CompanyName + "\"",
                                                    "\"" + item.TradingAs + "\"",
                                                    "\"" + item.CompanyRegistrationNumber + "\"",
                                                    "\"" + item.Description + "\"",
                                                    "\"" + item.ChepReference + "\"",
                                                    "\"" + ( ( Status ) item.Status ).GetDisplayText() + "\"",
                                                    "\"" + ( ( ServiceType ) item.ServiceRequired ).GetDisplayText() + "\"",
                                                    "\"" + ( ( TypeOfPalletUse ) item.PalletType ).GetDisplayText() + "\"",
                                                    "\"" + item.PalletTypeOther + "\"",
                                                    "\"" + ( ( CompanyType ) item.CompanyType ).GetDisplayText() + "\"",
                                                    "\"" + ( item.PSPName ?? item.PSPCompanyName ) + "\"",
                                                    "\"" + item.VATNumber + "\"",
                                                    "\"" + item.BBBEELevel + "\"",
                                                    "\"" + item.ContactPerson + "\"",
                                                    "\"" + item.ContactNumber + "\"",
                                                    "\"" + item.Email + "\"",
                                                    "\"" + item.AdminPerson + "\"",
                                                    "\"" + item.AdminEmail + "\"",
                                                    "\"" + item.FinancialPerson + "\"",
                                                    "\"" + item.FinPersonEmail + "\"",
                                                    Environment.NewLine );
                            }
                        }
                    }

                    #endregion

                    break;

                case "managesites":

                    #region Manage Sites

                    using ( SiteService service = new SiteService() )
                    using ( AddressService aservice = new AddressService() )
                    {
                        csv = string.Format( "Date Created, Name, Description, X Coord, Y Coord, Address Line 1, Address Line 2, Town, Province, Postal Code, Contact Name, Contact No, Finance Contact, Finance No., Receiver Contact, Receiver No., Planning Point, Depot, Chep Site Code, Site Type, Status {0}", Environment.NewLine );

                        List<SiteCustomModel> sites = service.List1( pm, csm );

                        if ( sites.NullableAny() )
                        {
                            foreach ( SiteCustomModel item in sites )
                            {
                                Address address = aservice.Get( item.Id, "Site" );

                                csv = string.Format( "{0} {1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17},{18},{19},{20},{21} {22}",
                                                    csv,
                                                    "\"" + item.CreatedOn + "\"",
                                                    "\"" + item.Name + "\"",
                                                    "\"" + item.Description + "\"",
                                                    "\"" + item.XCord + "\"",
                                                    "\"" + item.YCord + "\"",
                                                    "\"" + address?.Addressline1 + "\"",
                                                    "\"" + address?.Addressline2 + "\"",
                                                    "\"" + address?.Town + "\"",
                                                    "\"" + ( address != null ? address.Province.Description : string.Empty ) + "\"",
                                                    "\"" + item.PostalCode + "\"",
                                                    "\"" + item.ContactName + "\"",
                                                    "\"" + item.ContactNo + "\"",
                                                    "\"" + item.FinanceContact + "\"",
                                                    "\"" + item.FinanceContactNo + "\"",
                                                    "\"" + item.ReceivingContact + "\"",
                                                    "\"" + item.ReceivingContactNo + "\"",
                                                    "\"" + item.PlanningPoint + "\"",
                                                    "\"" + item.Depot + "\"",
                                                    "\"" + item.SiteCodeChep + "\"",
                                                    "\"" + ( ( item.SiteType.HasValue ) ? ( ( SiteType ) item.SiteType ).GetDisplayText() : string.Empty ) + "\"",
                                                    "\"" + ( ( Status ) item.Status ).GetDisplayText() + "\"",
                                                    Environment.NewLine );
                            }
                        }
                    }

                    #endregion

                    break;

                case "linkproducts":

                    #region Link Products

                    using ( ClientProductService service = new ClientProductService() )
                    {
                        csv = string.Format( "Date Created, Client, Name, Description, Active Date, Hire Rate, Lost Rate, Issue Rate, Passon Rate, Passon Days, Rate Type, Status {0}", Environment.NewLine );

                        List<ClientProductCustomModel> product = service.List1( pm, csm );

                        if ( product.NullableAny() )
                        {
                            foreach ( ClientProductCustomModel item in product )
                            {
                                csv = string.Format( "{0} {1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12} {13}",
                                                    csv,
                                                    "\"" + item.CreatedOn + "\"",
                                                    "\"" + item.ClientName + "\"",
                                                    "\"" + item.ProductName + "\"",
                                                    "\"" + item.ProductDescription + "\"",
                                                    "\"" + item.ActiveDate + "\"",
                                                    "\"" + item.Rate + "\"",
                                                    "\"" + item.LostRate + "\"",
                                                    "\"" + item.IssueRate + "\"",
                                                    "\"" + item.PassonRate + "\"",
                                                    "\"" + item.PassonDays + "\"",
                                                    "\"" + ( ( RateType ) item.RateType ).GetDisplayText() + "\"",
                                                    "\"" + ( ( Status ) item.Status ).GetDisplayText() + "\"",
                                                    Environment.NewLine );
                            }
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
                                                    "\"" + item.CreatedOn + "\"",
                                                    "\"" + item.Name + "\"",
                                                    "\"" + item.Description + "\"",
                                                    "\"" + item.ClientCount + "\"",
                                                    "\"" + ( ( Status ) item.Status ).GetDisplayText() + "\"",
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
                                                    "\"" + item.CreatedOn + "\"",
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
                                                    "\"" + ( ( TargetPeriod ) item.TargetPeriod ).GetDisplayText() + "\"",
                                                    "\"" + ( ( Status ) item.Status ).GetDisplayText() + "\"",
                                                    Environment.NewLine );
                            }
                        }
                    }

                    #endregion

                    break;

                case "managetransporters":

                    #region Manage Transporters

                    using ( TransporterService service = new TransporterService() )
                    {
                        csv = string.Format( "Date Created, Client, Name, Trading Name, Registration Number, Contact Name, Contact Number, Email, Supplier Code, Client Transporter Code, Chep Client Transporter Code, Status {0}", Environment.NewLine );

                        List<TransporterCustomModel> transporters = service.List1( pm, csm );

                        if ( transporters.NullableAny() )
                        {
                            foreach ( TransporterCustomModel item in transporters )
                            {
                                csv = string.Format( "{0} {1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12} {13}",
                                                    csv,
                                                    "\"" + item.CreatedOn + "\"",
                                                    "\"" + item.ClientName + "\"",
                                                    "\"" + item.Name + "\"",
                                                    "\"" + item.TradingName + "\"",
                                                    "\"" + item.RegistrationNumber + "\"",
                                                    "\"" + item.ContactName + "\"",
                                                    "\"" + item.ContactNumber + "\"",
                                                    "\"" + item.Email + "\"",
                                                    "\"" + item.SupplierCode + "\"",
                                                    "\"" + item.ClientTransporterCode + "\"",
                                                    "\"" + item.ChepClientTransporterCode + "\"",
                                                    "\"" + ( ( Status ) item.Status ).GetDisplayText() + "\"",
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
                ClientBudgets = new List<ClientBudget>(),
                ClientChepAccounts = new List<ClientChepAccount>(),
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
            using ( ClientChepAccountService ccaservice = new ClientChepAccountService() )
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

                    IsChepClient = model.IsChepClient.GetBoolValue(),
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

                #region Client Chep Accounts

                if ( model.ClientChepAccounts.NullableAny() )
                {
                    foreach ( ClientChepAccount item in model.ClientChepAccounts )
                    {
                        ClientChepAccount cca = ccaservice.GetById( item.Id );

                        if ( cca == null )
                        {
                            cca = new ClientChepAccount()
                            {
                                ClientId = client.Id,
                                Status = item.Status,
                                ChepReference = item.ChepReference,
                            };

                            ccaservice.Create( cca );
                        }
                        else
                        {
                            cca.Status = item.Status;
                            cca.ChepReference = item.ChepReference;

                            ccaservice.Update( cca );
                        }
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
                        ProvinceId = model.Address.ProvinceId,
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
                    IsChepClient = client.IsChepClient ? YesNo.Yes : YesNo.No,

                    Address = new AddressViewModel()
                    {
                        EditMode = true,
                        Town = address?.Town,
                        Id = address?.Id ?? 0,
                        PostCode = address?.PostalCode,
                        AddressLine1 = address?.Addressline1,
                        AddressLine2 = address?.Addressline2,
                        ProvinceId = address?.ProvinceId ?? 0,
                        AddressType = ( address != null ) ? ( AddressType ) address.Type : AddressType.Postal,
                    },

                    ClientBudgets = new List<ClientBudget>(),

                    Files = new List<FileViewModel>(),

                    ClientChepAccounts = client.ClientChepAccounts.ToList()
                };

                if ( !client.ClientChepAccounts.NullableAny( a => client.ChepReference?.Trim()?.ToLower() == client.ChepReference?.Trim()?.ToLower() ) )
                {
                    model.ClientChepAccounts.Add( new ClientChepAccount()
                    {
                        Status = ( int ) CommonStatus.Active,
                        ChepReference = client.ChepReference
                    } );
                }

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
            using ( ClientChepAccountService ccaservice = new ClientChepAccountService() )
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
                client.IsChepClient = model.IsChepClient.GetBoolValue();

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

                #region Client Chep Accounts

                if ( model.ClientChepAccounts.NullableAny() )
                {
                    foreach ( ClientChepAccount item in model.ClientChepAccounts )
                    {
                        ClientChepAccount cca = ccaservice.GetById( item.Id );

                        if ( cca == null )
                        {
                            cca = new ClientChepAccount()
                            {
                                ClientId = model.Id,
                                Status = item.Status,
                                ChepReference = item.ChepReference,
                            };

                            ccaservice.Create( cca );
                        }
                        else
                        {
                            cca.Status = item.Status;
                            cca.ChepReference = item.ChepReference;

                            ccaservice.Update( cca );
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
                            ProvinceId = model.Address.ProvinceId,
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
                        address.ProvinceId = model.Address.ProvinceId;

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
                        ProvinceId = address?.ProvinceId ?? 0,
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
            using ( ClientCustomerService ccservice = new ClientCustomerService() )
            {
                Site site1 = sservice.GetById( model.SiteId ?? 0 );

                #region Validation

                if ( sservice.ExistByClientAndName( model.ClientId, model.Name?.Trim()?.ToLower() ) )
                {
                    Notify( $"Sorry, a Site with the name {model.Name} in the specified region already exists.", NotificationType.Error );

                    return View( model );
                }

                #endregion

                if ( !model.SiteId.HasValue && !string.IsNullOrWhiteSpace( model.Longitude ) && !string.IsNullOrWhiteSpace( model.Latitude ) )
                {
                    Site existingSite = sservice.ExistByXYCoords( model.Longitude?.Trim(), model.Latitude?.Trim() );

                    if ( existingSite != null )
                    {
                        // Instead of pass back to view, we will create the new site as a subsite of the existing site.
                        // Get the existing site first
                        model.SiteId = existingSite.Id;
                    }
                }

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
                    ReceivingEmail = model.ReceivingEmail,

                    DepotManager = model.DepotManager,
                    DepotManagerEmail = model.DepotManagerEmail,
                    DepotManagerContact = model.DepotManagerContact,
                    FinanceEmail = model.FinanceEmail,
                    LocationNumber = model.LocationNumber,

                    ARPMSalesManagerId = model.ARPMSalesManagerId,
                    CLCode = model.CLCode,
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
                        ProvinceId = model.Address.ProvinceId,
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
                            KAMName = site.ContactName,
                            KAMContact = site.ContactNo,
                            KAMEmail = site.ReceivingEmail,
                            AuthorisationEmail1 = model.AuthorisationEmail1,
                            AuthorisationEmail2 = model.AuthorisationEmail2,
                            AuthorisationEmail3 = model.AuthorisationEmail3,
                            ClientManagerContact = model.ClientManagerContact,
                            ClientManagerEmail = model.ClientManagerEmail,
                            ClientSalesManager = model.ClientSalesManager,
                            ClientSalesRegEmail = model.ClientSalesRegEmail,
                            ClientSalesRepContact = model.ClientSalesRepContact,

                        };

                        csservice.Create( csSite );
                    }
                }
                else
                {
                    ClientCustomer cc = new ClientCustomer()
                    {
                        ClientId = model.ClientId,
                        Status = ( int ) model.Status,
                        CustomerName = site1?.Name ?? model.Name,
                        CustomerNumber = model.CustomerNoDebtorCode,
                    };

                    cc = ccservice.Create( cc );

                    ClientSite csSite = new ClientSite()
                    {
                        SiteId = site.Id,
                        ClientCustomerId = cc.Id,
                        Status = ( int ) model.Status,
                        AccountingCode = site.AccountCode,
                        KAMName = site.ContactName,
                        KAMContact = site.ContactNo,
                        KAMEmail = site.ReceivingEmail,
                        AuthorisationEmail1 = model.AuthorisationEmail1,
                        AuthorisationEmail2 = model.AuthorisationEmail2,
                        AuthorisationEmail3 = model.AuthorisationEmail3,
                        ClientManagerContact = model.ClientManagerContact,
                        ClientManagerEmail = model.ClientManagerEmail,
                        ClientSalesManager = model.ClientSalesManager,
                        ClientSalesRegEmail = model.ClientSalesRegEmail,
                        ClientSalesRepContact = model.ClientSalesRepContact,
                    };

                    csservice.Create( csSite );
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
                    SiteId = site.SiteId,
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
                    FinanceEmail = site.FinanceEmail,
                    FinanceContact = site.FinanceContact,
                    FinanceContactNo = site.FinanceContactNo,
                    ReceivingContact = site.ReceivingContact,
                    ReceivingContactNo = site.ReceivingContactNo,
                    ReceivingEmail = site.ReceivingEmail,
                    DepotManager = site.DepotManager,
                    DepotManagerEmail = site.DepotManagerEmail,
                    DepotManagerContact = site.DepotManagerContact,
                    LocationNumber = site.LocationNumber,

                    ARPMSalesManagerId = site.ARPMSalesManagerId,
                    CLCode = site.CLCode,

                    AuthorisationEmail1 = site.ClientSites?.FirstOrDefault()?.AuthorisationEmail1,
                    AuthorisationEmail2 = site.ClientSites?.FirstOrDefault()?.AuthorisationEmail2,
                    AuthorisationEmail3 = site.ClientSites?.FirstOrDefault()?.AuthorisationEmail3,

                    ClientSalesManager = site.ClientSites?.FirstOrDefault()?.ClientSalesManager,
                    ClientManagerContact = site.ClientSites?.FirstOrDefault()?.ClientManagerContact,
                    ClientManagerEmail = site.ClientSites?.FirstOrDefault()?.ClientManagerEmail,
                    ClientSalesRepContact = site.ClientSites?.FirstOrDefault()?.ClientSalesRepContact,
                    ClientSalesRegEmail = site.ClientSites?.FirstOrDefault()?.ClientSalesRegEmail,

                    SiteType = ( site.SiteType.HasValue ? ( SiteType ) site.SiteType : SiteType.All ),


                    Clients = new List<ClientCustomer>(),
                    SiteBudgets = new List<SiteBudget>(),
                    ClientId = site.ClientSites.FirstOrDefault()?.ClientCustomer?.ClientId ?? 0,
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
                        ProvinceId = address?.ProvinceId ?? 0,
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
            using ( ClientCustomerService ccservice = new ClientCustomerService() )
            {
                Site site = sservice.GetById( model.Id );

                Site site1 = sservice.GetById( model.SiteId ?? 0 );

                #region Validations

                if ( site == null )
                {
                    Notify( "Sorry, the requested resource could not be found. Please try again", NotificationType.Error );

                    return PartialView( "_AccessDenied" );
                }

                if ( site.Name?.Trim()?.ToLower() != model.Name?.Trim()?.ToLower() && site.RegionId != model.RegionId && sservice.ExistByClientAndName( model.ClientId, model.Name?.Trim()?.ToLower() ) )
                {
                    Notify( $"Sorry, a Site with the name {model.Name} in the specified region already exists.", NotificationType.Error );

                    return View( model );
                }

                #endregion

                if ( !model.SiteId.HasValue && !string.IsNullOrWhiteSpace( model.Longitude ) && !string.IsNullOrWhiteSpace( model.Latitude ) )
                {
                    Site existingSite = sservice.ExistByXYCoords( model.Longitude?.Trim(), model.Latitude?.Trim() );

                    if ( existingSite != null && site.Id != existingSite.Id && site.SiteId != existingSite.Id )
                    {
                        // Instead of pass back to view, we will create the new site as a subsite of the existing site.
                        // Get the existing site first
                        model.SiteId = existingSite.Id;
                    }
                }

                #region Update Site

                site.Name = model.Name;
                site.Depot = model.Depot;
                site.SiteId = model.SiteId;
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
                site.FinanceEmail = model.FinanceEmail;
                site.FinanceContact = model.FinanceContact;
                site.ReceivingContact = model.ReceivingContact;
                site.FinanceContactNo = model.FinanceContactNo;
                site.ReceivingContactNo = model.ReceivingContactNo;
                site.ReceivingEmail = model.ReceivingEmail;
                site.DepotManager = model.DepotManager;
                site.DepotManagerEmail = model.DepotManagerEmail;
                site.DepotManagerContact = model.DepotManagerContact;
                site.LocationNumber = model.LocationNumber;

                site.ARPMSalesManagerId = model.ARPMSalesManagerId;
                site.CLCode = model.CLCode;

                sservice.Update( site );

                #endregion

                #region Add Client Site

                csservice.Query( $"UPDATE [dbo].[ClientSite] SET [Status]={( int ) Status.Inactive} WHERE [SiteId]={site.Id}" );

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
                            cs.KAMName = site.ContactName;
                            cs.KAMContact = site.ContactNo;
                            cs.KAMEmail = site.ReceivingEmail;
                            cs.AuthorisationEmail1 = model.AuthorisationEmail1;
                            cs.AuthorisationEmail2 = model.AuthorisationEmail2;
                            cs.AuthorisationEmail3 = model.AuthorisationEmail3;
                            cs.ClientManagerContact = model.ClientManagerContact;
                            cs.ClientManagerEmail = model.ClientManagerEmail;
                            cs.ClientSalesManager = model.ClientSalesManager;
                            cs.ClientSalesRegEmail = model.ClientSalesRegEmail;
                            cs.ClientSalesRepContact = model.ClientSalesRepContact;

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
                                KAMName = site.ContactName,
                                KAMContact = site.ContactNo,
                                KAMEmail = site.ReceivingEmail,
                                AuthorisationEmail1 = model.AuthorisationEmail1,
                                AuthorisationEmail2 = model.AuthorisationEmail2,
                                AuthorisationEmail3 = model.AuthorisationEmail3,
                                ClientManagerContact = model.ClientManagerContact,
                                ClientManagerEmail = model.ClientManagerEmail,
                                ClientSalesManager = model.ClientSalesManager,
                                ClientSalesRegEmail = model.ClientSalesRegEmail,
                                ClientSalesRepContact = model.ClientSalesRepContact,
                            };

                            csservice.Create( cs );
                        }
                    }
                }
                else
                {
                    ClientCustomer cc = new ClientCustomer()
                    {
                        ClientId = model.ClientId,
                        Status = ( int ) model.Status,
                        CustomerName = site1?.Name ?? model.Name,
                        CustomerNumber = model.CustomerNoDebtorCode,
                    };

                    cc = ccservice.Create( cc );

                    ClientSite csSite = new ClientSite()
                    {
                        SiteId = site.Id,
                        ClientCustomerId = cc.Id,
                        Status = ( int ) model.Status,
                        AccountingCode = site.AccountCode,
                        KAMName = site.ContactName,
                        KAMContact = site.ContactNo,
                        KAMEmail = site.ReceivingEmail,
                        AuthorisationEmail1 = model.AuthorisationEmail1,
                        AuthorisationEmail2 = model.AuthorisationEmail2,
                        AuthorisationEmail3 = model.AuthorisationEmail3,
                        ClientManagerContact = model.ClientManagerContact,
                        ClientManagerEmail = model.ClientManagerEmail,
                        ClientSalesManager = model.ClientSalesManager,
                        ClientSalesRegEmail = model.ClientSalesRegEmail,
                        ClientSalesRepContact = model.ClientSalesRepContact,
                    };

                    csservice.Create( csSite );
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
                            ProvinceId = model.Address.ProvinceId,
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
                        address.ProvinceId = model.Address.ProvinceId;

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

        // GET: Client/ImportSite
        [Requires( PermissionTo.Create )]
        public ActionResult ImportSite()
        {
            SiteViewModel model = new SiteViewModel() { EditMode = true };

            return View( model );
        }

        // POST: Client/ImportSite
        [HttpPost]
        [Requires( PermissionTo.Create )]
        public ActionResult ImportSite( SiteViewModel model )
        {
            if ( model.SiteImportFile == null )
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
            using ( RegionService rservice = new RegionService() )
            using ( AddressService aservice = new AddressService() )
            using ( ProvinceService pservice = new ProvinceService() )
            using ( ClientSiteService csservice = new ClientSiteService() )
            using ( ClientCustomerService ccservice = new ClientCustomerService() )
            using ( TextFieldParser parser = new TextFieldParser( model.SiteImportFile.InputStream ) )
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

                    try
                    {
                        using ( TransactionScope scope = new TransactionScope() )
                        {
                            string siteNumber = load[ 0 ],
                                   customerNumber = load[ 1 ],
                                   customerName = load[ 2 ],
                                   siteName = load[ 3 ],
                                   subSiteName = load[ 4 ],
                                   addressLine1 = load[ 5 ],
                                   addressLine2 = load[ 6 ],
                                   town = load[ 7 ],
                                   xCord = load[ 8 ],
                                   yCord = load[ 9 ],
                                   kamContact = load[ 10 ],
                                   managerContact = load[ 11 ],
                                   liquorLicenseNumber = load[ 12 ],
                                   kamName = load[ 13 ],
                                   region = load[ 14 ],
                                   province = load[ 15 ],
                                   salesManager = load[ 16 ],
                                   salesRepCluster = load[ 17 ],
                                   salesRep = load[ 18 ];

                            if ( string.IsNullOrEmpty( siteName ) || string.IsNullOrEmpty( customerName ) || string.IsNullOrEmpty( customerNumber ) )
                            {
                                skipped++;

                                throw new Exception( $"Site name, Customer Name or Customer Number missing @ line {line}!" );
                            }

                            Region r = rservice.Search( region, province );

                            #region Site

                            Site s;

                            int? siteId = null;

                            if ( !string.IsNullOrWhiteSpace( subSiteName ) )
                            {
                                s = sservice.GetByClientAndName( model.ClientId, subSiteName );
                                Site s1 = sservice.GetByClientAndName( model.ClientId, siteName );

                                siteName = subSiteName;

                                siteId = s1?.Id;
                            }
                            else
                            {
                                s = sservice.GetByClientAndName( model.ClientId, siteName );
                            }

                            if ( s == null )
                            {
                                // Create new site

                                s = new Site()
                                {
                                    XCord = xCord,
                                    YCord = yCord,
                                    Name = siteName,
                                    SiteId = siteId,
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

                                created++;
                            }
                            else if ( s != null )
                            {
                                // Same site, update

                                s.XCord = xCord;
                                s.YCord = yCord;
                                s.Name = siteName;
                                s.SiteId = siteId;
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

                                updated++;
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

                            int? provinceId = pservice.GetIdByName( province?.Trim()?.ToLower()?.Replace( " ", "" ) );

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
                                    ProvinceId = provinceId,
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
                                a.ProvinceId = provinceId;
                                a.Addressline1 = addressLine1;
                                a.Addressline2 = addressLine2;
                                a.Type = ( int ) AddressType.Postal;

                                aservice.Update( a );
                            }

                            #endregion

                            scope.Complete();
                        }
                    }
                    catch ( Exception ex )
                    {
                        errors++;

                        errs.Add( ex.ToString() );
                    }
                }

                cQuery = string.Empty;
                uQuery = string.Empty;

                if ( errs.NullableAny() )
                {
                    errorDocId = LogImportErrors( errs, model.ClientId );
                }
            }

            string resp = $"{created} sites were successfully created, {updated} were updated, {skipped} were skipped and there were {errors} errors.";

            if ( errs.NullableAny() && errorDocId > 0 )
            {
                resp = $"{resp} <a href='/Client/ViewDocument/{errorDocId}' target='_blank'>Click here</a> to view the erros.";
            }

            Notify( resp, NotificationType.Success );

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
            using ( ProductService pservice = new ProductService() )
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

                Product p = pservice.GetById( model.ProductId );

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
                    ProductDescription = p.Description,
                    AccountingCode = model.AccountingCode,
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
                //cp.ProductDescription = model.Description;

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

                if ( !string.IsNullOrEmpty( model.Name ) && tservice.ExistByClientAndName( model.ClientId, model.Name.Trim() ) )
                {
                    // Transporter already exist!
                    Notify( $"Sorry, a Transporter with the Company Name \"{model.Name}\" for the selected client already exists!", NotificationType.Error );

                    return View( model );
                }

                #endregion

                #region Transporter

                Transporter t = new Transporter()
                {
                    Name = model.Name,
                    Email = model.Email,
                    ClientId = model.ClientId,
                    Status = ( int ) Status.Active,
                    TradingName = model.TradingName,
                    ContactName = model.ContactName,
                    SupplierCode = model.SupplierCode,
                    ContactNumber = model.ContactNumber,
                    RegistrationNumber = model.RegistrationNumber,
                    ClientTransporterCode = model.ClientTransporterCode,
                    ChepClientTransporterCode = model.ChepClientTransporterCode,
                };

                t = tservice.Create( t );

                #endregion

                #region Contacts

                if ( model.Contacts.NullableAny( c => !string.IsNullOrWhiteSpace( c.ContactName ) ) )
                {
                    foreach ( Contact mc in model.Contacts.Where( c => !string.IsNullOrWhiteSpace( c.ContactName ) ) )
                    {
                        Contact c = cservice.Get( mc.ContactEmail, "Transporter" );

                        if ( c == null )
                        {
                            c = new Contact()
                            {
                                ObjectId = t.Id,
                                JobTitle = mc.JobTitle,
                                ObjectType = "Transporter",
                                ContactCell = mc.ContactCell,
                                ContactName = mc.ContactName,
                                Status = ( int ) model.Status,
                                ContactEmail = mc.ContactEmail,
                                ContactTitle = mc.ContactTitle,
                            };

                            cservice.Create( c );
                        }
                        else
                        {
                            c.JobTitle = mc.JobTitle;
                            c.ContactCell = mc.ContactCell;
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

                if ( model.Vehicles.NullableAny( c => !string.IsNullOrWhiteSpace( c.Registration ) ) )
                {
                    foreach ( Vehicle mv in model.Vehicles.Where( c => !string.IsNullOrWhiteSpace( c.Registration ) ) )
                    {
                        Vehicle v = vservice.Get( mv.Registration, "Transporter" );

                        if ( v == null )
                        {
                            v = new Vehicle()
                            {
                                Type = mv.Type,
                                Make = mv.Make,
                                //Year = mv.Year,
                                ObjectId = t.Id,
                                //VINNumber = mv.VINNumber,
                                ObjectType = "Transporter",
                                FleetNumber = mv.FleetNumber,
                                Descriptoin = mv.Descriptoin,
                                Status = ( int ) model.Status,
                                Registration = mv.Registration,
                                //EngineNumber = mv.EngineNumber,
                            };

                            vservice.Create( v );
                        }
                        else
                        {
                            v.Type = mv.Type;
                            v.Make = mv.Make;
                            //v.Year = mv.Year;
                            //v.VINNumber = mv.VINNumber;
                            v.Descriptoin = mv.Descriptoin;
                            v.FleetNumber = mv.FleetNumber;
                            v.Status = ( int ) model.Status;
                            v.Registration = mv.Registration;
                            //v.EngineNumber = mv.EngineNumber;

                            vservice.Update( v );
                        }
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
                    ClientId = t.ClientId,
                    ContactName = t.ContactName,
                    TradingName = t.TradingName,
                    Status = ( Status ) t.Status,
                    SupplierCode = t.SupplierCode,
                    ContactNumber = t.ContactNumber,
                    RegistrationNumber = t.RegistrationNumber,
                    ClientTransporterCode = t.ClientTransporterCode,
                    ChepClientTransporterCode = t.ChepClientTransporterCode,
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

                if ( !string.IsNullOrEmpty( model.Name ) && model.Name.Trim().ToLower() != t.Name.Trim().ToLower() && tservice.ExistByClientAndName( model.ClientId, model.Name.Trim() ) )
                {
                    // Role already exist!
                    Notify( $"Sorry, a Transporter with the Company Name \"{model.Name}\" for the selected client already exists!", NotificationType.Error );

                    return View( model );
                }

                #endregion

                #region Transporter

                t.Id = model.Id;
                t.Name = model.Name;
                t.Email = model.Email;
                t.Status = ( int ) model.Status;
                t.TradingName = model.TradingName;
                t.ContactName = model.ContactName;
                t.SupplierCode = model.SupplierCode;
                t.ContactNumber = model.ContactNumber;
                t.RegistrationNumber = model.RegistrationNumber;
                t.ClientTransporterCode = model.ClientTransporterCode ?? string.Empty;
                t.ChepClientTransporterCode = model.ChepClientTransporterCode ?? string.Empty;

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
                                JobTitle = mc.JobTitle,
                                ObjectType = "Transporter",
                                ContactCell = mc.ContactCell,
                                //ContactIdNo = mc.ContactIdNo,
                                ContactName = mc.ContactName,
                                Status = ( int ) model.Status,
                                ContactEmail = mc.ContactEmail,
                                ContactTitle = mc.ContactTitle,
                            };

                            cservice.Create( c );
                        }
                        else
                        {
                            c.JobTitle = mc.JobTitle;
                            c.ContactCell = mc.ContactCell;
                            //c.ContactIdNo = mc.ContactIdNo;
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

                if ( model.Vehicles.NullableAny( c => !string.IsNullOrWhiteSpace( c.Registration ) ) )
                {
                    foreach ( Vehicle mv in model.Vehicles.Where( c => !string.IsNullOrWhiteSpace( c.Registration ) ) )
                    {
                        Vehicle v = vservice.GetById( mv.Id );

                        if ( v == null )
                        {
                            v = new Vehicle()
                            {
                                Type = mv.Type,
                                Make = mv.Make,
                                //Year = mv.Year,
                                ObjectId = t.Id,
                                //VINNumber = mv.VINNumber,
                                ObjectType = "Transporter",
                                FleetNumber = mv.FleetNumber,
                                Status = ( int ) model.Status,
                                Registration = mv.Registration,
                                //EngineNumber = mv.EngineNumber,
                                Descriptoin = $"{mv.Make} {mv.FleetNumber}",
                            };

                            vservice.Create( v );
                        }
                        else
                        {
                            v.Type = mv.Type;
                            v.Make = mv.Make;
                            //v.Year = mv.Year;
                            //v.VINNumber = mv.VINNumber;
                            v.FleetNumber = mv.FleetNumber;
                            v.Status = ( int ) model.Status;
                            v.Registration = mv.Registration;
                            //v.EngineNumber = mv.EngineNumber;
                            v.Descriptoin = $"{mv.Make} {mv.FleetNumber}";

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

        // GET: Client/ImportTransporter
        [Requires( PermissionTo.Create )]
        public ActionResult ImportTransporter()
        {
            TransporterViewModel model = new TransporterViewModel() { EditMode = true };

            return View( model );
        }

        // POST: Client/ImportTransporter
        [HttpPost]
        [Requires( PermissionTo.Create )]
        public ActionResult ImportTransporter( TransporterViewModel model )
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

            using ( TransporterService tservice = new TransporterService() )
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

                    if ( load.NullableCount() < 2 || string.IsNullOrWhiteSpace( load[ 0 ].Trim() ) )
                    {
                        skipped++;

                        continue;
                    }

                    load = load.ToSQLSafe();

                    Transporter t = tservice.GetByClientAndName( model.ClientId, load[ 0 ] );

                    if ( t == null )
                    {
                        #region Create Transporter

                        cQuery = $" {cQuery} INSERT INTO [dbo].[Transporter] ([ClientId],[CreatedOn],[ModifiedOn],[ModifiedBy],[Name],[ContactNumber],[Email],[TradingName],[RegistrationNumber],[ContactName],[SupplierCode],[ClientTransporterCode],[ChepClientTransporterCode],[Status]) ";
                        cQuery = $" {cQuery} VALUES ({model.ClientId},'{DateTime.Now}','{DateTime.Now}','{CurrentUser.Email}','{load[ 0 ]}','{load[ 1 ]}','{load[ 2 ]}','{load[ 3 ]}','{load[ 4 ]}','{load[ 6 ]}','{load[ 7 ]}','{load[ 8 ]}','{load[ 9 ]}',{( int ) Status.Active}) ";

                        #endregion

                        try
                        {
                            tservice.Query( cQuery );

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
                        #region Update Transporter

                        uQuery = $@"{uQuery} UPDATE [dbo].[Transporter] SET
                                                    [ModifiedOn]='{DateTime.Now}',
                                                    [ModifiedBy]='{CurrentUser.Email}',
                                                    [Name]='{load[ 0 ]}',
                                                    [ContactNumber]='{load[ 1 ]}',
                                                    [Email]='{load[ 2 ]}',
                                                    [TradingName]='{load[ 3 ]}',
                                                    [RegistrationNumber]='{load[ 4 ]}',
                                                    [ContactName]='{load[ 6 ]}',
                                                    [SupplierCode]='{load[ 7 ]}',
                                                    [ClientTransporterCode]='{load[ 8 ]}',
                                                    [ChepClientTransporterCode]='{load[ 9 ]}',
                                                    [Status]={( int ) Status.Active}
                                                WHERE
                                                    [Id]={t.Id}";

                        #endregion

                        try
                        {
                            tservice.Query( uQuery );

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
                    errorDocId = LogImportErrors( errs, model.ClientId ?? 0 );
                }
            }

            string resp = $"{created} Transporters were successfully created, {updated} were updated, {skipped} were skipped and there were {errors} errors.";

            if ( errs.NullableAny() && errorDocId > 0 )
            {
                resp = $"{resp} <a href='/Client/ViewDocument/{errorDocId}' target='_blank'>Click here</a> to view the errors.";
            }

            Notify( resp, NotificationType.Success );

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

                return PartialView( "_ManageTransportersCustomSearch", new CustomSearchModel( "ManageTransporters" ) );
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
