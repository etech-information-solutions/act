using ACT.Core.Enums;
using ACT.Core.Models;
using ACT.Core.Models.Custom;
using ACT.Core.Services;
using ACT.Data.Models;
using ACT.UI.Models;
using ACT.UI.Mvc;

using Microsoft.VisualBasic.FileIO;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Transactions;
using System.Web.Mvc;

namespace ACT.UI.Controllers
{
    [Requires( PermissionTo.View, PermissionContext.Administration )]
    public class AdministrationController : BaseController
    {
        // GET: Administration
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
                case "users":

                    #region Manage Users

                    csv = string.Format( "Date Created, Name, Status, Role, Email, Cell, Last Login, Can use Chats, Can Respond to Chats, Online Status {0}", Environment.NewLine );

                    using ( UserService uservice = new UserService() )
                    {
                        List<UserCustomModel> users = uservice.List1( pm, csm );

                        if ( users.NullableAny() )
                        {
                            foreach ( UserCustomModel item in users )
                            {
                                Status status = ( Status ) item.Status;

                                csv = string.Format( "{0} {1},{2},{3},{4},{5},{6},{7},{8},{9} {10}",
                                                    csv,
                                                    "\"" + item.CreatedOn + "\"",
                                                    "\"" + item.Name + " " + item.Surname + "\"",
                                                    "\"" + status.GetDisplayText() + "\"",
                                                    "\"" + item.RoleName + "\"",
                                                    "\"" + item.Email + "\"",
                                                    "\"" + item.Cell + "\"",
                                                    "\"" + item.SendChat + "\"",
                                                    "\"" + item.ReceiveChat + "\"",
                                                    "\"" + ( ( OnlineStatus ) item.ChatStatus ).GetDisplayText() + "\"",
                                                    Environment.NewLine );
                            }
                        }
                    }

                    #endregion

                    break;

                case "psps":

                    #region PSPs

                    csv = string.Format( "Date Created, Company Name, VAT Number, Contact Person, Email, Cell,Financial Person,Financial Person Email,Admin Person, Admin Email, Service Required, Status, Declined Reason,Type of Pallet Use, BBBEE Level,Company Type,Number Of Lost Pallets {0}", Environment.NewLine );

                    using ( PSPService pservice = new PSPService() )
                    {
                        List<PSPCustomModel> psps = pservice.List1( pm, csm );

                        if ( psps.NullableAny() )
                        {
                            foreach ( PSPCustomModel item in psps )
                            {
                                PSPClientStatus status = ( PSPClientStatus ) item.Status;

                                csv = string.Format( "{0} {1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17} {18}",
                                                    csv,
                                                    "\"" + item.CreatedOn + "\"",
                                                    "\"" + item.CompanyName + "\"",
                                                    "\"" + item.VATNumber + "\"",
                                                    "\"" + item.ContactPerson + "\"",
                                                    "\"" + item.Email + "\"",
                                                    "\"" + item.ContactNumber + "\"",
                                                    "\"" + item.FinancialPerson + "\"",
                                                    "\"" + item.FinPersonEmail + "\"",
                                                    "\"" + item.AdminPerson + "\"",
                                                    "\"" + item.AdminEmail + "\"",
                                                    "\"" + ( ( ServiceType ) item.ServiceRequired ).GetDisplayText() + "\"",
                                                    "\"" + status.GetDisplayText() + "\"",
                                                    "\"" + item.DeclinedReason + "\"",
                                                    "\"" + ( item.PalletType.HasValue ? ( ( TypeOfPalletUse ) item.PalletType ).GetDisplayText() : item.PalletTypeOther ) + "\"",
                                                    "\"" + item.BBBEELevel + "\"",
                                                    "\"" + ( item.CompanyType.HasValue ? ( ( CompanyType ) item.CompanyType ).GetDisplayText() : string.Empty ) + "\"",
                                                    "\"" + item.NumberOfLostPallets + "\"",
                                                    Environment.NewLine );
                            }
                        }
                    }

                    #endregion

                    break;

                case "regions":

                    #region Regions

                    csv = string.Format( "PSP, Name, Description, Country, Province, Regional Manager, Regional Manager Email, Status {0}", Environment.NewLine );

                    using ( RegionService service = new RegionService() )
                    {
                        List<RegionCustomModel> regions = service.List1( pm, csm );

                        if ( regions.NullableAny() )
                        {
                            foreach ( RegionCustomModel item in regions )
                            {
                                csv = string.Format( "{0} {1},{2},{3},{4},{5},{6},{7},{8} {9}",
                                                    csv,
                                                    "\"" + item.PSPName + "\"",
                                                    "\"" + item.Name + "\"",
                                                    "\"" + item.Description + "\"",
                                                    "\"" + item.CountryName + "\"",
                                                    "\"" + item.ProvinceName + "\"",
                                                    "\"" + item.RegionManagerName + "\"",
                                                    "\"" + item.RegionManagerEmail + "\"",
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

                case "products":

                    #region Products

                    using ( ProductService service = new ProductService() )
                    {
                        csv = string.Format( "Date Created, Name, Description, Status {0}", Environment.NewLine );

                        List<ProductCustomModel> product = service.List1( pm, csm );

                        if ( product.NullableAny() )
                        {
                            foreach ( ProductCustomModel item in product )
                            {
                                csv = string.Format( "{0} {1},{2},{3},{4} {5}",
                                                    csv,
                                                    "\"" + item.CreatedOn + "\"",
                                                    "\"" + item.Name + "\"",
                                                    "\"" + item.Description + "\"",
                                                    "\"" + ( ( Status ) item.Status ).GetDisplayText() + "\"",
                                                    Environment.NewLine );
                            }
                        }
                    }

                    #endregion

                    break;

                case "auditlog":

                    #region Audit Log

                    csv = string.Format( "Date, Activity, User, Action Table, Action, Controller, Comments, Image Before, Image After, Browser {0}", Environment.NewLine );

                    using ( AuditLogService service = new AuditLogService() )
                    {
                        List<AuditLogCustomModel> audits = service.List( pm, csm );

                        if ( audits.NullableAny() )
                        {
                            foreach ( AuditLogCustomModel item in audits )
                            {
                                ActivityTypes activity = ( ActivityTypes ) item.Type;

                                csv = string.Format( "{0} {1},{2},{3},{4},{5},{6},{7},{8},{9},{10} {11}",
                                                    csv,
                                                    "\"" + item.CreatedOn + "\"",
                                                    "\"" + activity.GetDisplayText() + "\"",
                                                    "\"" + item.User.Name + " " + item.User.Surname + "\"",
                                                    "\"" + item.ActionTable + "\"",
                                                    "\"" + item.Action + "\"",
                                                    "\"" + item.Controller + "\"",
                                                    "\"" + item.Comments + "\"",
                                                    "\"" + ( item.BeforeImage ?? "" ).Replace( '"', ' ' ) + "\"",
                                                    "\"" + ( item.AfterImage ?? "" ).Replace( '"', ' ' ) + "\"",
                                                    "\"" + ( item.Browser ?? "" ) + "\"",
                                                    Environment.NewLine );
                            }
                        }
                    }

                    #endregion

                    break;

                case "broadcasts":

                    #region BroadCasts

                    csv = string.Format( "Date Created, Start Date, End Date, Status, xRead, Message {0}", Environment.NewLine );

                    using ( BroadcastService service = new BroadcastService() )
                    {
                        List<Broadcast> broadcasts = service.List( pm, csm );

                        if ( broadcasts.NullableAny() )
                        {
                            foreach ( Broadcast item in broadcasts )
                            {
                                Status status = ( Status ) item.Status;

                                csv = string.Format( "{0} {1},{2},{3},{4},{5},{6} {7}",
                                                    csv,
                                                    "\"" + item.CreatedOn + "\"",
                                                    "\"" + item.StartDate + "\"",
                                                    "\"" + item.EndDate + "\"",
                                                    "\"" + status.GetDisplayText() + "\"",
                                                    "\"" + item.UserBroadcasts.Count + "\"",
                                                    "\"" + item.Message + "\"",
                                                    Environment.NewLine );
                            }
                        }
                    }

                    #endregion

                    break;

                case "roles":

                    #region Roles

                    csv = string.Format( "Name, Type, Dashboard, Administration, Finance, Clients, Customers, Products, Pallets {0}", Environment.NewLine );

                    using ( RoleService service = new RoleService() )
                    {
                        List<Role> roles = service.List( pm, csm );

                        if ( roles.NullableAny() )
                        {
                            foreach ( Role item in roles )
                            {
                                csv = string.Format( "{0} {1},{2},{3},{4},{5},{6},{7},{8},{9} {10}",
                                                    csv,
                                                    "\"" + item.Name + "\"",
                                                    "\"" + ( ( RoleType ) item.Type ).GetDisplayText() + "\"",
                                                    "\"" + item.DashBoard + "\"",
                                                    "\"" + item.Administration + "\"",
                                                    "\"" + item.Finance + "\"",
                                                    "\"" + item.Client + "\"",
                                                    "\"" + item.Customer + "\"",
                                                    "\"" + item.Product + "\"",
                                                    "\"" + item.Pallet + "\"",
                                                    Environment.NewLine );
                            }
                        }
                    }

                    #endregion

                    break;

                case "systemconfig":

                    #region System Config

                    csv = string.Format( "System Contact Email,Finance Contact Email,Activation Email,Correspondence Email,System Contact Number,System Contact Address,Invoice Run Day,Auto Logoff,Auto Logoff Seconds,Password Change, Images Location, Documents Location,App Download Url,Website Url {0}", Environment.NewLine );

                    using ( SystemConfigService service = new SystemConfigService() )
                    {
                        List<SystemConfig> items = service.List( pm, csm );

                        if ( items != null && items.Any() )
                        {
                            foreach ( SystemConfig item in items )
                            {
                                csv = string.Format( "{0} {1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14} {15}",
                                                    csv,
                                                    "\"" + item.SystemContactEmail + "\"",
                                                    "\"" + item.FinancialContactEmail + "\"",
                                                    "\"" + item.ActivationEmail + "\"",
                                                    "\"" + item.CorrespondenceEmail + "\"",
                                                    "\"" + item.ContactNumber + "\"",
                                                    "\"" + item.Address + "\"",
                                                    "\"" + item.InvoiceRunDay + "\"",
                                                    "\"" + item.AutoLogoff + "\"",
                                                    "\"" + item.LogoffSeconds + "\"",
                                                    "\"" + item.PasswordChange + "\"",
                                                    "\"" + item.ImagesLocation + "\"",
                                                    "\"" + item.DocumentsLocation + "\"",
                                                    "\"" + item.AppDownloadUrl + "\"",
                                                    "\"" + item.WebsiteUrl + "\"",
                                                    Environment.NewLine );
                            }
                        }
                    }

                    #endregion

                    break;

                case "declinereasons":

                    #region Decline Reasons

                    csv = string.Format( "Date Created, Description, Status {0}", Environment.NewLine );

                    using ( DeclineReasonService service = new DeclineReasonService() )
                    {
                        List<DeclineReason> declineReasons = service.List( pm, csm );

                        if ( declineReasons != null && declineReasons.Any() )
                        {
                            foreach ( DeclineReason item in declineReasons )
                            {
                                Status status = ( Status ) ( item.Status );

                                csv = string.Format( "{0} {1},{2},{3} {4}",
                                                    csv,
                                                    "\"" + item.CreatedOn + "\"",
                                                    "\"" + item.Description + "\"",
                                                    "\"" + status.GetDisplayText() + "\"",
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



        #region Roles

        //
        // GET: /Administration/RoleDetails/5
        public ActionResult RoleDetails( int id, bool layout = true )
        {
            Role model = new Role();

            using ( RoleService service = new RoleService() )
            {
                model = service.GetById( id );
            }

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

        //
        // GET: /Administration/AddRole/5 
        [Requires( PermissionTo.Create )]
        public ActionResult AddRole()
        {
            RoleViewModel model = new RoleViewModel() { EditMode = true };

            return View( model );
        }

        //
        // POST: /Administration/AddRole/5
        [HttpPost]
        [Requires( PermissionTo.Create )]
        public ActionResult AddRole( RoleViewModel model )
        {
            if ( !ModelState.IsValid )
            {
                Notify( "Sorry, the Role was not created. Please correct all errors and try again.", NotificationType.Error );

                return View( model );
            }

            using ( RoleService rservice = new RoleService() )
            using ( TransactionScope scope = new TransactionScope() )
            {
                #region Validations

                if ( rservice.ExistByNameType( model.Name?.Trim(), model.Type ) )
                {
                    // Role already exist!
                    Notify( $"Sorry, a Role with the name \"{model.Name} ({model.Type.GetDisplayText()})\" already exists!", NotificationType.Error );

                    return View( model );
                }

                #endregion

                #region Create Role

                // Create Role
                Role role = new Role()
                {
                    Name = model.Name,
                    Client = model.Client,
                    Pallet = model.Pallet,
                    Product = model.Product,
                    Finance = model.Finance,
                    Type = ( int ) model.Type,
                    Customer = model.Customer,
                    DashBoard = model.DashBoard,
                    Administration = model.Administration
                };

                rservice.Create( role );

                #endregion

                // We're done here..

                scope.Complete();
            }

            Notify( "The Role was successfully created.", NotificationType.Success );

            return RedirectToAction( "Roles" );
        }

        //
        // GET: /Administration/EditRole/5
        [Requires( PermissionTo.Edit )]
        public ActionResult EditRole( int id )
        {
            Role role;

            using ( RoleService service = new RoleService() )
            {
                role = service.GetById( id );
            }

            if ( role == null )
            {
                Notify( "Sorry, the requested resource could not be found. Please try again", NotificationType.Error );

                return PartialView( "_AccessDenied" );
            }

            RoleViewModel model = new RoleViewModel()
            {
                Id = role.Id,
                Name = role.Name,
                Pallet = role.Pallet,
                Client = role.Client,
                Product = role.Product,
                Finance = role.Finance,
                Customer = role.Customer,
                DashBoard = role.DashBoard,
                Type = ( RoleType ) role.Type,
                Administration = role.Administration,
                EditMode = true
            };

            return View( model );
        }

        //
        // POST: /Administration/EditRole/5
        [HttpPost]
        [Requires( PermissionTo.Edit )]
        public ActionResult EditRole( RoleViewModel model, PagingModel pm, bool isstructure = false )
        {
            if ( !ModelState.IsValid )
            {
                Notify( "Sorry, the selected Role was not updated. Please correct all errors and try again.", NotificationType.Error );

                return View( model );
            }

            Role role;

            using ( RoleService rservice = new RoleService() )
            using ( TransactionScope scope = new TransactionScope() )
            {
                role = rservice.GetById( model.Id );

                if ( role == null )
                {
                    Notify( "Sorry, that Role does not exist! Please specify a valid Role Id and try again.", NotificationType.Error );

                    return View( model );
                }

                #region Validations

                if ( ( !role.Name.Trim().Equals( model.Name ) || role.Type != ( int ) model.Type ) && rservice.ExistByNameType( model.Name?.Trim(), model.Type ) )
                {
                    // Role already exist!
                    Notify( $"Sorry, a Role with the Name \"{model.Name} ({model.Type.GetDisplayText()})\" already exists!", NotificationType.Error );

                    return View( model );
                }

                #endregion

                #region Update Role

                // Update Role
                role.Name = model.Name;
                role.Client = model.Client;
                role.Pallet = model.Pallet;
                role.Product = model.Product;
                role.Finance = model.Finance;
                role.Type = ( int ) model.Type;
                role.Customer = model.Customer;
                role.DashBoard = model.DashBoard;
                role.Administration = model.Administration;

                rservice.Update( role );

                #endregion

                scope.Complete();
            }

            Notify( "The selected Role details were successfully updated.", NotificationType.Success );

            return RedirectToAction( "Roles" );
        }

        //
        // POST: /Administration/DeleteRole/5
        [HttpPost]
        [Requires( PermissionTo.Delete )]
        public ActionResult DeleteRole( RoleViewModel model, PagingModel pm )
        {
            Role role;

            using ( RoleService service = new RoleService() )
            {
                role = service.GetById( model.Id );

                if ( role == null )
                {
                    Notify( "Sorry, the requested resource could not be found. Please try again", NotificationType.Error );

                    return PartialView( "_AccessDenied" );
                }

                service.Delete( role );

                Notify( "The selected Role was successfully Deleted.", NotificationType.Success );
            }

            return RedirectToAction( "Roles" );
        }

        #endregion



        #region System Config

        //
        // GET: /Administration/EditSystemConfig/5
        [Requires( PermissionTo.Edit )]
        public ActionResult EditSystemConfig( int id )
        {
            SystemConfig config = new SystemConfig();

            using ( SystemConfigService service = new SystemConfigService() )
            {
                config = service.GetById( id );
            }

            if ( config == null )
            {
                Notify( "Sorry, the requested resource could not be found. Please try again", NotificationType.Error );

                return PartialView( "_AccessDenied" );
            }

            SystemConfigViewModel model = new SystemConfigViewModel()
            {
                Id = config.Id,
                Address = config.Address,
                AutoLogoff = config.AutoLogoff,
                WebsiteUrl = config.WebsiteUrl,
                ContactNumber = config.ContactNumber,
                LogoffSeconds = config.LogoffSeconds,
                InvoiceRunDay = config.InvoiceRunDay,
                ImagesLocation = config.ImagesLocation,
                AppDownloadUrl = config.AppDownloadUrl,
                PasswordChange = config.PasswordChange,
                ActivationEmail = config.ActivationEmail,
                DocumentsLocation = config.DocumentsLocation,
                FinancialEmail = config.FinancialContactEmail,
                SystemContactEmail = config.SystemContactEmail,
                CorrespondenceEmail = config.CorrespondenceEmail,

                DisputeMonitorPath = config.DisputeMonitorPath,
                DisputeMonitorTime = config.DisputeMonitorTime,
                DisputeMonitorInterval = config.DisputeMonitorInterval,
                DisputeMonitorEnabled = config.DisputeMonitorEnabled ? YesNo.Yes : YesNo.No,

                ClientMonitorPath = config.ClientMonitorPath,
                ClientMonitorTime = config.ClientMonitorTime,
                ClientMonitorInterval = config.ClientMonitorInterval,
                ClientMonitorEnabled = config.ClientMonitorEnabled ? YesNo.Yes : YesNo.No,

                ClientContractRenewalReminderMonths = config.ClientContractRenewalReminderMonths,
                DisputeDaysToResolve = config.DisputeDaysToResolve
            };

            return View( model );
        }

        //
        // POST: /Administration/EditSystemConfig/5
        [HttpPost]
        [Requires( PermissionTo.Edit )]
        public ActionResult EditSystemConfig( SystemConfigViewModel model, PagingModel pm )
        {
            if ( !ModelState.IsValid )
            {
                Notify( "Sorry, the selected Config was not updated. Please correct all errors and try again.", NotificationType.Error );

                return View( model );
            }

            SystemConfig config;

            using ( SystemConfigService service = new SystemConfigService() )
            {
                config = service.GetById( model.Id );

                if ( config == null )
                {
                    Notify( "Sorry, that Config does not exist! Please specify a valid Config Id and try again.", NotificationType.Error );

                    return View( model );
                }

                config.AutoLogoff = model.AutoLogoff;
                config.LogoffSeconds = model.LogoffSeconds;

                config.InvoiceRunDay = model.InvoiceRunDay;

                config.ImagesLocation = model.ImagesLocation;
                config.DocumentsLocation = model.DocumentsLocation;

                config.PasswordChange = model.PasswordChange;

                config.ActivationEmail = model.ActivationEmail;
                config.SystemContactEmail = model.SystemContactEmail;
                config.FinancialContactEmail = model.FinancialEmail;
                config.CorrespondenceEmail = model.CorrespondenceEmail;

                config.Address = model.Address;
                config.ContactNumber = model.ContactNumber;

                config.WebsiteUrl = model.WebsiteUrl;
                config.AppDownloadUrl = model.AppDownloadUrl;

                config.DisputeMonitorPath = model.DisputeMonitorPath;
                config.DisputeMonitorTime = model.DisputeMonitorTime;
                config.DisputeMonitorInterval = model.DisputeMonitorInterval;
                config.DisputeMonitorEnabled = model.DisputeMonitorEnabled.GetBoolValue();

                config.ClientMonitorPath = model.ClientMonitorPath;
                config.ClientMonitorTime = model.ClientMonitorTime;
                config.ClientMonitorInterval = model.ClientMonitorInterval;
                config.ClientMonitorEnabled = model.ClientMonitorEnabled.GetBoolValue();

                config.ClientContractRenewalReminderMonths = model.ClientContractRenewalReminderMonths;
                config.DisputeDaysToResolve = model.DisputeDaysToResolve;

                service.Update( config );
            }

            Notify( "The System Configuration details were successfully updated.", NotificationType.Success );

            VariableExtension.SystemRules = null;
            ContextExtensions.RemoveCachedData( "SR_ca" );

            return RedirectToAction( "SystemConfig" );
        }

        #endregion



        #region Audit Log Managements

        //
        // GET: /Administration/AuditLogDetails/5
        public ActionResult AuditLogDetails( int id, bool layout = true )
        {
            AuditLogCustomModel model = new AuditLogCustomModel();

            using ( AuditLogService service = new AuditLogService() )
            {
                model = service.GetById( id );
            }

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

        #endregion



        #region User Management

        //
        // GET: /Administration/UserDetails/5
        public ActionResult UserDetails( int id, bool layout = true )
        {
            User model;

            using ( UserService service = new UserService() )
            {
                model = service.GetById( id );
            }

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

        //
        // GET: /Administration/AddUser/5 
        [Requires( PermissionTo.Create )]
        public ActionResult AddUser()
        {
            UserViewModel model = new UserViewModel() { EditMode = true };

            return View( model );
        }

        //
        // POST: /Administration/AddUser/5
        [HttpPost]
        [Requires( PermissionTo.Create )]
        public ActionResult AddUser( UserViewModel model )
        {
            if ( !ModelState.IsValid )
            {
                Notify( "Sorry, the User was not created. Please correct all errors and try again.", NotificationType.Error );

                return View( model );
            }

            using ( UserService uservice = new UserService() )
            using ( RoleService rservice = new RoleService() )
            {
                #region Validations

                if ( uservice.ExistByEmail( model.Email?.Trim() ) )
                {
                    // User already exist!
                    Notify( string.Format( "Sorry, a User with the Email Address \"{0}\" already exists!", model.Email ), NotificationType.Error );

                    return View( model );
                }
                if ( !string.Equals( model.Password?.Trim(), model.ConfirmPassword?.Trim() ) )
                {
                    // Password mismatch
                    Notify( "Password combination does not match. Please try again.", NotificationType.Error );

                    return View( model );
                }

                #endregion


                #region Create User

                Role role = rservice.GetById( model.RoleId );

                // Create User
                User user = new User()
                {
                    Type = role.Type,
                    Cell = model.Cell,
                    Name = model.Name,
                    Email = model.Email,
                    Surname = model.Surname,
                    PasswordDate = DateTime.Now,
                    Status = ( int ) model.Status,
                    Password = uservice.GetSha1Md5String( model.Password ),
                };

                if ( CurrentUser.IsAdmin )
                {
                    user.ChatStatus = ( int ) model.OnlineStatus;
                    user.SendChat = model.CanSendChat.GetBoolValue();
                    user.ReceiveChat = model.CanReceiveChat.GetBoolValue();
                }

                #region Link PSP / Client

                if ( role.Type == ( int ) RoleType.PSP && model.PSPId > 0 )
                {
                    user.PSPUsers.Add( new PSPUser()
                    {
                        UserId = user.Id,
                        CreatedOn = DateTime.Now,
                        ModifiedOn = DateTime.Now,
                        PSPId = model.PSPId.Value,
                        ModifiedBy = CurrentUser.Email,
                        Status = ( int ) Status.Active,
                    } );
                }
                else if ( role.Type == ( int ) RoleType.Client && model.ClientId > 0 )
                {
                    user.ClientUsers.Add( new ClientUser()
                    {
                        UserId = user.Id,
                        CreatedOn = DateTime.Now,
                        ModifiedOn = DateTime.Now,
                        Status = ( int ) Status.Active,
                        ModifiedBy = CurrentUser.Email,
                        ClientId = model.ClientId.Value,

                    } );
                }

                #endregion

                user = uservice.Create( user, model.RoleId );

                #endregion

                // We're done here..

                Notify( "The User was successfully created.", NotificationType.Success );

                return RedirectToAction( "Users" );
            }
        }

        //
        // GET: /Administration/EditUser/5
        [Requires( PermissionTo.Edit )]
        public ActionResult EditUser( int id )
        {
            using ( UserService service = new UserService() )
            {
                User user = service.GetById( id );

                if ( user == null )
                {
                    Notify( "Sorry, the requested resource could not be found. Please try again", NotificationType.Error );

                    return PartialView( "_AccessDenied" );
                }

                Role role = null;

                if ( user.UserRoles.Any() )
                {
                    role = user.UserRoles.FirstOrDefault().Role;
                }

                UserViewModel model = new UserViewModel()
                {
                    Role = role,
                    Id = user.Id,
                    EditMode = true,
                    RoleId = role.Id,
                    Name = user.Name?.Trim(),
                    Cell = user.Cell?.Trim(),
                    Email = user.Email?.Trim(),
                    Surname = user.Surname?.Trim(),
                    Status = ( Status ) user.Status,
                    RoleType = ( RoleType ) user.Type,
                    OnlineStatus = ( OnlineStatus ) user.ChatStatus,
                    PSPId = user.PSPUsers.FirstOrDefault()?.PSPId ?? 0,
                    CanSendChat = user.SendChat ? YesNo.Yes : YesNo.No,
                    CanReceiveChat = user.ReceiveChat ? YesNo.Yes : YesNo.No,
                    ClientId = user.ClientUsers.FirstOrDefault()?.ClientId ?? 0
                };

                return View( model );
            }
        }

        //
        // POST: /Administration/EditUser/5
        [HttpPost]
        [Requires( PermissionTo.Edit )]
        public ActionResult EditUser( UserViewModel model, PagingModel pm )
        {
            if ( !ModelState.IsValid )
            {
                Notify( "Sorry, the selected User was not updated. Please correct all errors and try again.", NotificationType.Error );

                return View( model );
            }

            using ( UserService uservice = new UserService() )
            using ( RoleService rservice = new RoleService() )
            {
                User user = uservice.GetById( model.Id );

                if ( user == null )
                {
                    Notify( "Sorry, that User does not exist! Please specify a valid User Id and try again.", NotificationType.Error );

                    return View( model );
                }

                #region Validations

                if ( !( user.Email ?? "" ).Trim().Equals( model.Email ) && uservice.ExistByEmail( model.Email?.Trim() ) )
                {
                    // User already exist!
                    Notify( string.Format( "Sorry, a User with the Email Address \"{0}\" already exists!", model.Email ), NotificationType.Error );

                    return View( model );
                }

                if ( !string.IsNullOrEmpty( model.Password ) && !string.Equals( model.Password?.Trim(), model.ConfirmPassword?.Trim() ) )
                {
                    // Password mismatch
                    Notify( "Password combination does not match. Please try again.", NotificationType.Error );

                    return View( model );
                }

                #endregion


                #region Update User

                Role role = rservice.GetById( model.RoleId );

                // Update User

                user.Cell = model.Cell;
                user.Name = model.Name;
                user.Email = model.Email;
                user.Surname = model.Surname;

                if ( !string.IsNullOrEmpty( model.Password ) )
                {
                    user.PasswordDate = DateTime.Now;
                    user.Password = uservice.GetSha1Md5String( model.Password );
                }

                if ( CurrentUser.IsAdmin )
                {
                    user.Type = role.Type;
                    user.Status = ( int ) model.Status;
                    user.ChatStatus = ( int ) model.OnlineStatus;
                    user.SendChat = model.CanSendChat.GetBoolValue();
                    user.ReceiveChat = model.CanReceiveChat.GetBoolValue();

                    if ( role.Type == ( int ) RoleType.PSP && model.PSPId > 0 && !user.PSPUsers.Any( p => p.PSPId == model.PSPId ) )
                    {
                        user.PSPUsers.Add( new PSPUser()
                        {
                            UserId = user.Id,
                            Status = user.Status,
                            CreatedOn = DateTime.Now,
                            ModifiedOn = DateTime.Now,
                            PSPId = model.PSPId.Value,
                            ModifiedBy = CurrentUser.Email,
                        } );
                    }
                    else if ( role.Type == ( int ) RoleType.Client && model.ClientId > 0 && !user.ClientUsers.Any( p => p.ClientId == model.ClientId ) )
                    {
                        user.ClientUsers.Add( new ClientUser()
                        {
                            UserId = user.Id,
                            Status = user.Status,
                            CreatedOn = DateTime.Now,
                            ModifiedOn = DateTime.Now,
                            ModifiedBy = CurrentUser.Email,
                            ClientId = model.ClientId.Value,

                        } );
                    }
                }

                user = uservice.Update( user, model.RoleId );

                #endregion

                Notify( "The selected User's details were successfully updated.", NotificationType.Success );

                return RedirectToAction( "Users" );
            }
        }

        //
        // POST: /Administration/DeleteUser/5
        [HttpPost]
        [Requires( PermissionTo.Delete )]
        public ActionResult DeleteUser( UserViewModel model, PagingModel pm )
        {
            using ( UserService service = new UserService() )
            {
                User user = service.GetById( model.Id );

                if ( user == null )
                {
                    Notify( "Sorry, the requested resource could not be found. Please try again", NotificationType.Error );

                    return PartialView( "_AccessDenied" );
                }

                user.Status = ( ( ( Status ) user.Status ) == Status.Active ) ? ( int ) Status.Inactive : ( int ) Status.Active;

                service.Update( user );

                Notify( "The selected User was successfully updated.", NotificationType.Success );

                return RedirectToAction( "Users" );
            }
        }

        #endregion



        #region PSP Management

        //
        // GET: /Administration/PSPDetails/5
        public ActionResult PSPDetails( int id, bool layout = true )
        {
            using ( PSPService pservice = new PSPService() )
            using ( AddressService aservice = new AddressService() )
            using ( DocumentService dservice = new DocumentService() )
            {
                PSP model = pservice.GetById( id );

                if ( model == null )
                {
                    Notify( "Sorry, the requested resource could not be found. Please try again", NotificationType.Error );

                    return RedirectToAction( "Index" );
                }

                Address address = aservice.Get( model.Id, "PSP" );

                List<Document> documents = dservice.List( model.Id, "PSP" );

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

        //
        // GET: /Administration/AddPSP/5 
        [Requires( PermissionTo.Create )]
        public ActionResult AddPSP()
        {
            PSPViewModel model = new PSPViewModel()
            {
                Address = new AddressViewModel(),
                Files = new List<FileViewModel>(),
                PSPBudgets = new List<PSPBudget>()
            };

            return View( model );
        }

        //
        // POST: /Administration/AddPSP/5
        [HttpPost]
        [Requires( PermissionTo.Create )]
        public ActionResult AddPSP( PSPViewModel model )
        {
            if ( !ModelState.IsValid )
            {
                Notify( "Sorry, the PSP was not created. Please correct all errors and try again.", NotificationType.Error );

                return View( model );
            }

            using ( PSPService pservice = new PSPService() )
            using ( AddressService aservice = new AddressService() )
            using ( TransactionScope scope = new TransactionScope() )
            using ( DocumentService dservice = new DocumentService() )
            using ( PSPBudgetService bservice = new PSPBudgetService() )
            {
                #region Validations

                if ( !string.IsNullOrEmpty( model.CompanyRegistrationNumber ) && pservice.ExistByCompanyRegistrationNumber( model.CompanyRegistrationNumber.Trim() ) )
                {
                    Notify( $"A Pooling Service Provider (PSP) with that Company Registration Number already exists. Contact us if you require further assistance.", NotificationType.Error );

                    return View( model );
                }

                #endregion

                #region Create PSP

                PSP psp = new PSP()
                {
                    Email = model.EmailAddress,
                    TradingAs = model.TradingAs,
                    VATNumber = model.VATNumber,
                    AdminEmail = model.AdminEmail,
                    AdminPerson = model.AdminPerson,
                    CompanyName = model.CompanyName,
                    Description = model.Description,
                    ContactPerson = model.ContactPerson,
                    ContactNumber = model.ContactNumber,
                    FinPersonEmail = model.FinPersonEmail,
                    FinancialPerson = model.FinancialPerson,
                    Status = ( int ) PSPClientStatus.Verified,
                    ServiceRequired = ( int ) model.ServiceType,
                    CompanyRegistrationNumber = model.CompanyRegistrationNumber,

                    BBBEELevel = model.BBBEELevel,
                    CompanyType = ( int ) model.CompanyType,
                    PalletType = ( int ) model.TypeOfPalletUse,
                    PalletTypeOther = model.OtherTypeOfPalletUse,
                    NumberOfLostPallets = model.NumberOfLostPallets,
                };

                psp = pservice.Create( psp );

                #endregion

                #region Create PSP Budget

                if ( model.PSPBudgets.NullableAny() )
                {
                    foreach ( PSPBudget l in model.PSPBudgets )
                    {
                        PSPBudget b = new PSPBudget()
                        {
                            PSPId = psp.Id,
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
                        ObjectId = psp.Id,
                        ObjectType = "PSP",
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
                        string path = Server.MapPath( $"~/{VariableExtension.SystemRules.DocumentsLocation}/PSP/{psp.Id}/" );

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
                            ObjectId = psp.Id,
                            ObjectType = "PSP",
                            Title = f.File.FileName,
                            Size = f.File.ContentLength,
                            Description = f.Description,
                            Status = ( int ) Status.Active,
                            Type = Path.GetExtension( f.File.FileName ),
                            Location = $"PSP/{psp.Id}/{now}-{f.File.FileName}"
                        };

                        dservice.Create( doc );

                        string fullpath = Path.Combine( path, $"{now}-{f.File.FileName}" );

                        f.File.SaveAs( fullpath );
                    }
                }

                #endregion

                // Complete the scope
                scope.Complete();

                // We're done here..
            }

            Notify( "The PSP was successfully created.", NotificationType.Success );

            return RedirectToAction( "PSPs" );
        }

        //
        // GET: /Administration/EditPSP/5
        [Requires( PermissionTo.Edit )]
        public ActionResult EditPSP( int id )
        {
            using ( PSPService pservice = new PSPService() )
            using ( AddressService aservice = new AddressService() )
            using ( DocumentService dservice = new DocumentService() )
            {
                PSP psp = pservice.GetById( id );

                if ( psp == null )
                {
                    Notify( "Sorry, the requested resource could not be found. Please try again", NotificationType.Error );

                    return PartialView( "_AccessDenied" );
                }

                PSPViewModel model = ConstructPSPViewModel( psp );

                return View( model );
            }
        }

        //
        // POST: /Administration/EditPSP/5
        [HttpPost]
        [Requires( PermissionTo.Edit )]
        public ActionResult EditPSP( PSPViewModel model, PagingModel pm )
        {
            //if ( !ModelState.IsValid )
            //{
            //    Notify( "Sorry, the selected PSP was not updated. Please correct all errors and try again.", NotificationType.Error );

            //    return View( model );
            //}

            using ( PSPService pservice = new PSPService() )
            using ( AddressService aservice = new AddressService() )
            using ( TransactionScope scope = new TransactionScope() )
            using ( DocumentService dservice = new DocumentService() )
            using ( PSPBudgetService bservice = new PSPBudgetService() )
            using ( PSPClientService cservice = new PSPClientService() )
            {
                PSP psp = pservice.GetById( model.Id );

                if ( psp == null )
                {
                    Notify( "Sorry, that PSP does not exist! Please specify a valid PSP Id and try again.", NotificationType.Error );

                    return View( model );
                }

                #region Validations

                if ( !string.IsNullOrEmpty( model.CompanyRegistrationNumber ) && model.CompanyRegistrationNumber.Trim().ToLower() != psp.CompanyRegistrationNumber.Trim().ToLower() && pservice.ExistByCompanyRegistrationNumber( model.CompanyRegistrationNumber.Trim() ) )
                {
                    Notify( $"A Pooling Service Provider (PSP) with that Company Registration Number already exists. Contact us if you require further assistance.", NotificationType.Error );

                    return View( model );
                }

                #endregion

                #region Update PSP

                // Update PSP

                psp.Email = model.EmailAddress;
                psp.TradingAs = model.TradingAs;
                psp.VATNumber = model.VATNumber;
                psp.Status = ( int ) model.Status;
                psp.AdminEmail = model.AdminEmail;
                psp.AdminPerson = model.AdminPerson;
                psp.CompanyName = model.CompanyName;
                psp.Description = model.Description;
                psp.ContactPerson = model.ContactPerson;
                psp.ContactNumber = model.ContactNumber;
                psp.FinPersonEmail = model.FinPersonEmail;
                psp.FinancialPerson = model.FinancialPerson;
                psp.ServiceRequired = ( int ) model.ServiceType;
                psp.CompanyRegistrationNumber = model.CompanyRegistrationNumber;

                psp.BBBEELevel = model.BBBEELevel;
                psp.CompanyType = ( int ) model.CompanyType;
                psp.NumberOfLostPallets = model.NumberOfLostPallets;
                psp.PalletTypeOther = model.OtherTypeOfPalletUse;
                psp.ServiceRequired = ( int ) model.ServiceType;
                psp.PalletType = ( int ) model.TypeOfPalletUse;

                psp = pservice.Update( psp );

                #endregion

                #region PSP Budgets

                if ( model.PSPBudgets.NullableAny() )
                {
                    foreach ( PSPBudget l in model.PSPBudgets )
                    {
                        PSPBudget b = bservice.GetById( l.Id );

                        if ( b == null )
                        {
                            b = new PSPBudget()
                            {
                                PSPId = psp.Id,
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

                #region Clients

                if ( model.PSPClients.NullableAny() )
                {
                    foreach ( PSPClient l in model.PSPClients )
                    {
                        PSPClient pc = cservice.GetById( l.Id );

                        if ( pc == null )
                        {
                            continue;
                        }
                        else
                        {
                            pc.FinAccountingCode = l.FinAccountingCode;
                            pc.ContractRenewalDate = l.ContractRenewalDate;

                            pc.Status = ( int ) Status.Active;

                            cservice.Update( pc );
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
                            ObjectType = "PSP",
                            ObjectId = model.Id,
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
                        string path = Server.MapPath( $"~/{VariableExtension.SystemRules.DocumentsLocation}/PSP/{psp.Id}/" );

                        if ( !Directory.Exists( path ) )
                        {
                            Directory.CreateDirectory( path );
                        }

                        Document doc;

                        string now = DateTime.Now.ToString( "yyyyMMddHHmmss" );

                        if ( f.Name?.ToLower() == "logo" )
                        {
                            doc = dservice.Get( psp.Id, "PSP", f.Name );

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
                            ObjectId = psp.Id,
                            ObjectType = "PSP",
                            Title = f.File.FileName,
                            Size = f.File.ContentLength,
                            Description = f.Description,
                            Status = ( int ) Status.Active,
                            Type = Path.GetExtension( f.File.FileName ),
                            Location = $"PSP/{psp.Id}/{now}-{f.File.FileName}"
                        };

                        dservice.Create( doc );

                        string fullpath = Path.Combine( path, $"{now}-{f.File.FileName}" );

                        f.File.SaveAs( fullpath );
                    }
                }

                #endregion

                // Complete the scope
                scope.Complete();
            }

            Notify( "The selected PSP's details were successfully updated.", NotificationType.Success );

            return PSPs( new PagingModel(), new CustomSearchModel() );
        }

        //
        // POST: /Administration/DeletePSP/5
        [HttpPost]
        [Requires( PermissionTo.Delete )]
        public ActionResult DeletePSP( PSPViewModel model )
        {
            PSP psp;

            using ( PSPService service = new PSPService() )
            {
                psp = service.GetById( model.Id );

                if ( psp == null )
                {
                    Notify( "Sorry, the requested resource could not be found. Please try again", NotificationType.Error );

                    return PartialView( "_AccessDenied" );
                }

                psp.Status = ( ( ( PSPClientStatus ) psp.Status ) == PSPClientStatus.Verified ) ? ( int ) PSPClientStatus.Inactive : ( int ) PSPClientStatus.Verified;

                service.Update( psp );

                Notify( "The selected PSP was successfully updated.", NotificationType.Success );
            }

            return PSPs( new PagingModel(), new CustomSearchModel() );
        }

        /// <summary>
        /// Constructs a PSP View Model
        /// </summary>
        /// <param name="psp"></param>
        /// <returns></returns>
        public PSPViewModel ConstructPSPViewModel( PSP psp )
        {
            using ( AddressService aservice = new AddressService() )
            using ( DocumentService dservice = new DocumentService() )
            using ( EstimatedLoadService eservice = new EstimatedLoadService() )
            {
                Address address = aservice.Get( psp.Id, "PSP" );

                List<Document> documents = dservice.List( psp.Id, "PSP" );

                List<EstimatedLoad> loads = new List<EstimatedLoad>();

                bool unverified = ( psp.Status == ( int ) PSPClientStatus.Unverified );

                if ( unverified )
                {
                    loads = eservice.List( psp.Id, "PSP" );
                }

                #region PSP

                PSPViewModel model = new PSPViewModel()
                {
                    Id = psp.Id,
                    EmailAddress = psp.Email,
                    TradingAs = psp.TradingAs,
                    VATNumber = psp.VATNumber,
                    AdminEmail = psp.AdminEmail,
                    CompanyName = psp.CompanyName,
                    Description = psp.Description,
                    ContactNumber = psp.ContactNumber,
                    ContactPerson = psp.ContactPerson,
                    AdminPerson = psp.AdminPerson,
                    FinancialPerson = psp.FinancialPerson,
                    FinPersonEmail = psp.FinPersonEmail,
                    Status = ( PSPClientStatus ) psp.Status,
                    ServiceType = ( ServiceType ) psp.ServiceRequired,
                    CompanyRegistrationNumber = psp.CompanyRegistrationNumber,

                    BBBEELevel = psp.BBBEELevel,
                    CompanyType = ( CompanyType ) psp.CompanyType,
                    NumberOfLostPallets = psp.NumberOfLostPallets,
                    OtherTypeOfPalletUse = psp.PalletTypeOther,
                    TypeOfPalletUse = ( TypeOfPalletUse ) psp.PalletType,

                    Address = new AddressViewModel()
                    {
                        EditMode = true,
                        Town = address?.Town,
                        Id = address?.Id ?? 0,
                        PostCode = address?.PostalCode,
                        AddressLine1 = address?.Addressline1,
                        AddressLine2 = address?.Addressline2,
                        ProvinceId = address?.ProvinceId ?? 0,
                        Province = address?.Province,
                        AddressType = ( address != null ) ? ( AddressType ) address.Type : AddressType.Postal,
                    },
                    User = new UserViewModel()
                    {
                        EditMode = true,
                        Password = " ",
                        ConfirmPassword = " ",
                        Status = Status.Inactive,
                        Cell = psp.PSPUsers?.FirstOrDefault()?.User?.Cell,
                        Name = psp.PSPUsers?.FirstOrDefault()?.User?.Name,
                        Id = psp.PSPUsers?.FirstOrDefault()?.User?.Id ?? 0,
                        Email = psp.PSPUsers?.FirstOrDefault()?.User?.Email,
                        Surname = psp.PSPUsers?.FirstOrDefault()?.User?.Surname,
                        RoleId = psp.PSPUsers?.FirstOrDefault()?.User?.UserRoles?.FirstOrDefault()?.RoleId ?? 0,
                    },

                    PSPBudgets = new List<PSPBudget>(),

                    Files = new List<FileViewModel>(),

                    PSPClients = psp.PSPClients.ToList(),
                };

                #endregion

                #region PSP Budgets

                if ( unverified && loads.NullableAny() )
                {
                    foreach ( EstimatedLoad l in loads )
                    {
                        model.PSPBudgets.Add( new PSPBudget()
                        {
                            Id = l.Id,
                            BudgetYear = l.BudgetYear ?? 0,
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
                else if ( psp.PSPBudgets.NullableAny() )
                {
                    foreach ( PSPBudget l in psp.PSPBudgets )
                    {
                        model.PSPBudgets.Add( new PSPBudget()
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

                #region PSP Files

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

                return model;
            }
        }

        //
        // GET: /Administration/ApprovePSP/5
        public ActionResult ApproveDeclinePSP( int id )
        {
            using ( PSPService pservice = new PSPService() )
            using ( AddressService aservice = new AddressService() )
            using ( DocumentService dservice = new DocumentService() )
            {
                PSP psp = pservice.GetById( id );

                if ( psp == null )
                {
                    Notify( "Sorry, the requested resource could not be found. Please try again", NotificationType.Error );

                    return PartialView( "_Notification" );
                }

                PSPViewModel model = ConstructPSPViewModel( psp );

                return PartialView( "_ApproveDeclinePSP", model );
            }
        }

        //
        // POST: /Administration/ApprovePSP/5
        [HttpPost]
        [Requires( PermissionTo.Edit )]
        public ActionResult ApproveDeclinePSP( PSPViewModel model )
        {
            using ( PSPService pservice = new PSPService() )
            using ( RoleService rservice = new RoleService() )
            using ( UserService uservice = new UserService() )
            using ( AddressService aservice = new AddressService() )
            using ( TransactionScope scope = new TransactionScope() )
            using ( DocumentService dservice = new DocumentService() )
            using ( PSPBudgetService bservice = new PSPBudgetService() )
            {
                PSP psp = pservice.GetById( model.Id );

                if ( psp == null )
                {
                    Notify( "Sorry, the requested resource could not be found. Please try again", NotificationType.Error );

                    return PartialView( "_Notification" );
                }

                //if ( !ModelState.IsValid )
                //{
                //    Notify( "Sorry, the selected PSP was not updated. Please correct all errors and try again.", NotificationType.Error );

                //    model = ConstructPSPViewModel( psp );

                //    return PartialView( "_ApproveDeclinePSP", model );
                //}

                #region Update PSP

                psp.Status = ( int ) model.Status;

                if ( !string.IsNullOrEmpty( model.DeclineReason ) )
                {
                    psp.DeclinedReason = model.DeclineReason;
                }

                psp = pservice.Update( psp );

                #endregion

                #region Create/Update User

                User user = uservice.GetById( model.User.Id );

                if ( model.User != null && model.Status == PSPClientStatus.Verified )
                {
                    if ( user == null )
                    {
                        Role role = rservice.GetByName( RoleType.PSP.GetStringValue() );

                        user = new User()
                        {
                            Cell = model.User.Cell,
                            Name = model.User.Name,
                            Email = model.User.Email,
                            Type = ( int ) RoleType.PSP,
                            PasswordDate = DateTime.Now,
                            Surname = model.User.Surname,
                            Status = ( int ) Status.Active,
                            Password = uservice.GetSha1Md5String( model.User.Password )
                        };

                        uservice.Create( user, role.Id );
                    }
                    else
                    {
                        user.Cell = model.User.Cell;
                        user.Name = model.User.Name;
                        user.Email = model.User.Email;
                        user.Surname = model.User.Surname;
                        user.PasswordDate = DateTime.Now;
                        user.Status = ( int ) Status.Active;
                        user.Password = uservice.GetSha1Md5String( model.User.Password );

                        uservice.Update( user );
                    }
                }
                else if ( user != null )
                {
                    user.Status = ( int ) Status.Rejected;

                    uservice.Update( user );
                }

                #endregion

                #region PSP Budgets

                if ( model.PSPBudgets.NullableAny() )
                {
                    foreach ( PSPBudget l in model.PSPBudgets )
                    {
                        PSPBudget b = new PSPBudget()
                        {
                            PSPId = psp.Id,
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

                scope.Complete();

                #region Emails

                bool emailed;

                if ( model.Status == PSPClientStatus.Verified )
                {
                    emailed = SendUserApproved( model.User );

                    if ( emailed )
                    {
                        Notify( $"The selected PSP ({model.CompanyName}) was successfully approved and an email has been sent to {model.User.Email}.", NotificationType.Success );
                    }
                    else
                    {
                        Notify( $"The selected PSP ({model.CompanyName}) was successfully approved and an email could not be sent to {model.User.Email}. NOTE: Send login details manually.", NotificationType.Warn );
                    }
                }
                else if ( model.Status == PSPClientStatus.Rejected )
                {
                    emailed = SendUserDecline( model.User );

                    if ( emailed )
                    {
                        Notify( $"The selected PSP ({model.CompanyName}) was successfully declined and an email has been sent to {model.User.Email}.", NotificationType.Success );
                    }
                    else
                    {
                        Notify( $"The selected PSP ({model.CompanyName}) was successfully declined BUT an email could not be sent to {model.User.Email}. NOTE: Notify manually.", NotificationType.Warn );
                    }
                }

                #endregion
            }

            return PSPs( new PagingModel(), new CustomSearchModel() );
        }

        //
        // GET: /Administration/PSPConfig/5
        public ActionResult PSPConfig( int id )
        {
            using ( PSPService pservice = new PSPService() )
            {
                PSP psp = pservice.GetById( id );

                if ( psp == null )
                {
                    Notify( "Sorry, the requested resource could not be found. Please try again", NotificationType.Error );

                    return PartialView( "_Notification" );
                }

                PSPConfigViewModel model = new PSPConfigViewModel() { PSPId = psp.Id };

                if ( psp.PSPConfigs.Any() )
                {
                    model = new PSPConfigViewModel()
                    {
                        PSPId = psp.Id,
                        Id = psp.PSPConfigs.FirstOrDefault().Id,
                        ImportEmail = psp.PSPConfigs.FirstOrDefault().ImportEmail,
                        InvoiceRunDay = psp.PSPConfigs.FirstOrDefault().InvoiceRunDay,
                        FinancialEmail = psp.PSPConfigs.FirstOrDefault().FinancialEmail,
                        OpsManagerEmail = psp.PSPConfigs.FirstOrDefault().OpsManagerEmail,
                        DeclineEmailName = psp.PSPConfigs.FirstOrDefault().DeclineEmailName,
                        DocumentLocation = psp.PSPConfigs.FirstOrDefault().DocumentLocation,
                        WelcomeEmailName = psp.PSPConfigs.FirstOrDefault().WelcomeEmailName,
                        AdminManagerEmail = psp.PSPConfigs.FirstOrDefault().AdminManagerEmail,
                        BillingFileLocation = psp.PSPConfigs.FirstOrDefault().BillingFileLocation,
                        SystemContactEmail = psp.PSPConfigs.FirstOrDefault().ReceivingManagerEmail,
                        ReceivingManagerEmail = psp.PSPConfigs.FirstOrDefault().ReceivingManagerEmail,
                        ClientCorrespondenceName = psp.PSPConfigs.FirstOrDefault().ClientCorrespondenceName,
                        ImportEmailHost = psp.PSPConfigs.FirstOrDefault().ImportEmailHost,
                        ImportEmailPassword = psp.PSPConfigs.FirstOrDefault().ImportEmailPassword,
                        ImportEmailPort = psp.PSPConfigs.FirstOrDefault().ImportEmailPort,
                        ImportEmailUsername = psp.PSPConfigs.FirstOrDefault().ImportEmailUsername,
                        ImportUseSSL = psp.PSPConfigs.FirstOrDefault().ImportUseSSL == true ? YesNo.Yes : YesNo.No,
                    };
                }

                return PartialView( "_PSPConfig", model );
            }
        }

        //
        // POST: /Administration/PSPConfig/5
        [HttpPost]
        [Requires( PermissionTo.Edit )]
        public ActionResult PSPConfig( PSPConfigViewModel model )
        {
            if ( !ModelState.IsValid )
            {
                Notify( "Sorry, the selected PSP Config was not updated. Please correct all errors and try again.", NotificationType.Error );

                return PartialView( "_PSPConfig", model );
            }

            using ( PSPConfigService pservice = new PSPConfigService() )
            {
                PSPConfig config = pservice.GetById( model.Id );

                if ( config == null )
                {
                    config = new PSPConfig()
                    {
                        PSPId = model.PSPId,
                        ImportEmail = model.ImportEmail,
                        InvoiceRunDay = model.InvoiceRunDay,
                        FinancialEmail = model.FinancialEmail,
                        SystemEmail = model.SystemContactEmail,
                        OpsManagerEmail = model.OpsManagerEmail,
                        WelcomeEmailName = model.WelcomeEmailName,
                        DeclineEmailName = model.DeclineEmailName,
                        DocumentLocation = model.DocumentLocation,
                        AdminManagerEmail = model.AdminManagerEmail,
                        BillingFileLocation = model.BillingFileLocation,
                        ReceivingManagerEmail = model.ReceivingManagerEmail,
                        ClientCorrespondenceName = model.ClientCorrespondenceName,
                        ImportEmailHost = model.ImportEmailHost,
                        ImportEmailUsername = model.ImportEmailUsername,
                        ImportEmailPassword = model.ImportEmailPassword,
                        ImportEmailPort = model.ImportEmailPort,
                        ImportUseSSL = model.ImportUseSSL == YesNo.Yes ? true : false,
                    };

                    pservice.Create( config );
                }
                else
                {
                    config.ImportEmail = model.ImportEmail;
                    config.InvoiceRunDay = model.InvoiceRunDay;
                    config.FinancialEmail = model.FinancialEmail;
                    config.SystemEmail = model.SystemContactEmail;
                    config.OpsManagerEmail = model.OpsManagerEmail;
                    config.WelcomeEmailName = model.WelcomeEmailName;
                    config.DeclineEmailName = model.DeclineEmailName;
                    config.DocumentLocation = model.DocumentLocation;
                    config.AdminManagerEmail = model.AdminManagerEmail;
                    config.BillingFileLocation = model.BillingFileLocation;
                    config.ReceivingManagerEmail = model.ReceivingManagerEmail;
                    config.ClientCorrespondenceName = model.ClientCorrespondenceName;

                    config.ImportEmailHost = model.ImportEmailHost;
                    config.ImportEmailUsername = model.ImportEmailUsername;
                    config.ImportEmailPassword = model.ImportEmailPassword;
                    config.ImportEmailPort = model.ImportEmailPort;
                    config.ImportUseSSL = model.ImportUseSSL == YesNo.Yes ? true : false;

                    pservice.Update( config );
                }
            }

            Notify( "The PSP Configuration details were successfully updated.", NotificationType.Success );

            return PSPs( new PagingModel(), new CustomSearchModel() );
        }

        //
        // POST: /PSP/DeletePSPBudget/5
        [HttpPost]
        [Requires( PermissionTo.Delete )]
        public ActionResult DeletePSPBudget( int id )
        {
            using ( PSPBudgetService bservice = new PSPBudgetService() )
            {
                PSPBudget b = bservice.GetById( id );

                if ( b == null )
                {
                    Notify( "Sorry, the requested resource could not be found. Please try again", NotificationType.Error );

                    return PartialView( "_AccessDenied" );
                }

                int PSPId = b.PSPId;

                bservice.Delete( b );

                Notify( "The selected Budget was successfully Deleted.", NotificationType.Success );

                List<PSPBudget> model = bservice.ListByColumnWhere( "PSPId", PSPId );

                return PartialView( "_PSPBudgets", model );
            }
        }

        //
        // POST: /PSP/DeletePSPClient/5
        [HttpPost]
        [Requires( PermissionTo.Delete )]
        public ActionResult DeletePSPClient( int id )
        {
            using ( PSPClientService bservice = new PSPClientService() )
            {
                PSPClient pc = bservice.GetById( id );

                if ( pc == null )
                {
                    Notify( "Sorry, the requested resource could not be found. Please try again", NotificationType.Error );

                    return PartialView( "_AccessDenied" );
                }

                int PSPId = pc.PSPId;

                bservice.Delete( pc );

                Notify( "The selected PSP Client was successfully removed from the selected PSP.", NotificationType.Success );

                List<PSPClient> model = bservice.ListByColumnWhere( "PSPId", PSPId );

                return PartialView( "_PSPClients", model );
            }
        }

        //
        // POST: /PSP/DeletePSPFile/5
        [HttpPost]
        [Requires( PermissionTo.Delete )]
        public ActionResult DeletePSPDocument( int id, bool selfDestruct = false )
        {
            using ( DocumentService dservice = new DocumentService() )
            {
                Document doc = dservice.GetById( id );

                if ( doc == null )
                {
                    Notify( "Sorry, the requested resource could not be found. Please try again", NotificationType.Error );

                    return PartialView( "_AccessDenied" );
                }

                int PSPId = doc.ObjectId ?? 0;

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

                List<Document> documents = dservice.List( PSPId, "PSP" );

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

                return PartialView( "_PSPDocuments", model );
            }
        }

        //
        // GET: /Administration/PSPUsers/5
        public ActionResult PSPUsers( int id )
        {
            using ( PSPService pservice = new PSPService() )
            {
                PSP model = pservice.GetById( id );

                if ( model == null )
                {
                    Notify( "Sorry, the requested resource could not be found. Please try again", NotificationType.Error );

                    return PartialView( "_Notification" );
                }

                return PartialView( "_PSPUsers", model );
            }
        }

        //
        // GET: /Administration/PSPClients/5
        public ActionResult PSPClients( int id )
        {
            using ( PSPService pservice = new PSPService() )
            {
                PSP model = pservice.GetById( id );

                if ( model == null )
                {
                    Notify( "Sorry, the requested resource could not be found. Please try again", NotificationType.Error );

                    return PartialView( "_Notification" );
                }

                return PartialView( "_PSPClientsView", model );
            }
        }

        //
        // GET: /Administration/PSPProducts/5
        public ActionResult PSPProducts( int id )
        {
            using ( PSPService pservice = new PSPService() )
            {
                PSP model = pservice.GetById( id );

                if ( model == null )
                {
                    Notify( "Sorry, the requested resource could not be found. Please try again", NotificationType.Error );

                    return PartialView( "_Notification" );
                }

                return PartialView( "_PSPProducts", model );
            }
        }

        //
        // GET: /Administration/PSPInvoices/5
        public ActionResult PSPInvoices( int id )
        {
            using ( PSPService pservice = new PSPService() )
            {
                PSP model = pservice.GetById( id );

                if ( model == null )
                {
                    Notify( "Sorry, the requested resource could not be found. Please try again", NotificationType.Error );

                    return PartialView( "_Notification" );
                }

                return PartialView( "_PSPInvoices", model );
            }
        }

        //
        // GET: /Administration/PSPBudgets/5
        public ActionResult PSPBudgets( int id )
        {
            using ( PSPService pservice = new PSPService() )
            {
                PSP model = pservice.GetById( id );

                if ( model == null )
                {
                    Notify( "Sorry, the requested resource could not be found. Please try again", NotificationType.Error );

                    return PartialView( "_Notification" );
                }

                return PartialView( "_PSPBudgetsView", model );
            }
        }

        #endregion



        #region Broadcasts

        //
        // GET: /Administration/BroadcastDetails/5
        public ActionResult BroadcastDetails( int id, bool layout = true )
        {
            Broadcast model;

            using ( BroadcastService service = new BroadcastService() )
            {
                model = service.GetById( id );
            }

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

        //
        // GET: /Administration/AddBroadcast
        [Requires( PermissionTo.Create )]
        public ActionResult AddBroadcast()
        {
            BroadcastViewModel model = new BroadcastViewModel() { EditMode = true };

            return View( model );
        }

        //
        // POST: /Administration/AddBroadcast/5
        [HttpPost]
        [Requires( PermissionTo.Create )]
        public ActionResult AddBroadcast( BroadcastViewModel model )
        {
            if ( !ModelState.IsValid )
            {
                Notify( "Sorry, the Broadcast was not created. Please correct all errors and try again.", NotificationType.Error );

                return View( model );
            }

            using ( BroadcastService service = new BroadcastService() )
            {
                if ( service.Exist( model.StartDate.Value ) )
                {
                    // Broadcast already exist!
                    Notify( string.Format( "Sorry, a Broadcast for/within the specified period \"{0}\" already exists!", model.StartDate ), NotificationType.Error );

                    return View( model );
                }

                Broadcast broadcast = new Broadcast()
                {
                    Message = model.Message,
                    EndDate = model.EndDate,
                    StartDate = model.StartDate.Value,
                    Status = ( int ) model.Status
                };

                if ( CurrentUser.RoleType == RoleType.PSP )
                {
                    broadcast.ObjectType = "PSP";
                    broadcast.ObjectId = CurrentUser.PSPs.FirstOrDefault().Id;
                }
                else if ( CurrentUser.RoleType == RoleType.Client )
                {
                    broadcast.ObjectType = "Client";
                    broadcast.ObjectId = CurrentUser.Clients.FirstOrDefault().Id;
                }

                broadcast = service.Create( broadcast );

                Notify( "The selected Broadcast were successfully created.", NotificationType.Success );
            }

            return RedirectToAction( "Broadcasts" );
        }

        //
        // GET: /Administration/EditBroadcast/5
        [Requires( PermissionTo.Edit )]
        public ActionResult EditBroadcast( int id )
        {
            Broadcast broadcast;

            using ( BroadcastService service = new BroadcastService() )
            {
                broadcast = service.GetById( id );
            }

            if ( broadcast == null )
            {
                Notify( "Sorry, the requested resource could not be found. Please try again", NotificationType.Error );

                return PartialView( "_AccessDenied" );
            }

            BroadcastViewModel model = new BroadcastViewModel()
            {
                Id = broadcast.Id,
                StartDate = broadcast.StartDate,
                EndDate = broadcast.EndDate,
                Message = broadcast.Message,
                Status = ( Status ) broadcast.Status,
                EditMode = true
            };

            return View( model );
        }

        //
        // POST: /Administration/EditBroadcast/5
        [HttpPost]
        [Requires( PermissionTo.Edit )]
        public ActionResult EditBroadcast( BroadcastViewModel model, PagingModel pm )
        {
            if ( !ModelState.IsValid )
            {
                Notify( "Sorry, the selected Broadcast was not updated. Please correct all errors and try again.", NotificationType.Error );

                return View( model );
            }

            Broadcast broadcast;

            using ( BroadcastService service = new BroadcastService() )
            {
                broadcast = service.GetById( model.Id );

                if ( broadcast == null )
                {
                    Notify( "Sorry, that Broadcast does not exist! Please specify a valid Broadcast Id and try again.", NotificationType.Error );

                    return View( model );
                }

                if ( !broadcast.Message.ToLower().Equals( model.Message.ToLower() ) && broadcast.StartDate.Date != model.StartDate.Value.Date && service.Exist( model.StartDate.Value ) )
                {
                    // Broadcast already exist!
                    Notify( string.Format( "Sorry, a Broadcast for/within the specified period \"{0}\" already exists!", model.StartDate ), NotificationType.Error );

                    return View( model );
                }

                broadcast.StartDate = model.StartDate.Value;
                broadcast.EndDate = model.EndDate;
                broadcast.Message = model.Message;
                broadcast.Status = ( int ) model.Status;

                broadcast = service.Update( broadcast );

                Notify( "The selected Broadcast's details were successfully updated.", NotificationType.Success );
            }

            return RedirectToAction( "Broadcasts" );
        }

        //
        // POST: /Administration/DeleteBroadcast/5
        [HttpPost]
        [Requires( PermissionTo.Delete )]
        public ActionResult DeleteBroadcast( BroadcastViewModel model )
        {
            Broadcast broadcast;

            using ( BroadcastService service = new BroadcastService() )
            {
                broadcast = service.GetById( model.Id );

                if ( broadcast == null )
                {
                    Notify( "Sorry, the requested resource could not be found. Please try again", NotificationType.Error );

                    return PartialView( "_Notification" );
                }

                broadcast.Status = ( broadcast.Status == ( int ) Status.Active ) ? ( int ) Status.Inactive : ( int ) Status.Active;

                service.Update( broadcast );

                Notify( "The selected Broadcast was successfully updated.", NotificationType.Success );
            }

            return RedirectToAction( "Broadcasts" );
        }

        #endregion



        #region Banks

        //
        // GET: /Administration/BankDetails/5
        public ActionResult BankDetails( int id, bool layout = true )
        {
            Bank model;

            using ( BankService service = new BankService() )
            {
                model = service.GetById( id );
            }

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

        //
        // GET: /Administration/AddBank/5 
        [Requires( PermissionTo.Create )]
        public ActionResult AddBank()
        {
            BankViewModel model = new BankViewModel() { EditMode = true };

            return View( model );
        }

        //
        // POST: /Administration/AddBank/5
        [HttpPost]
        [Requires( PermissionTo.Create )]
        public ActionResult AddBank( BankViewModel model )
        {
            if ( !ModelState.IsValid )
            {
                Notify( "Sorry, the Bank was not created. Please correct all errors and try again.", NotificationType.Error );

                return View( model );
            }

            Bank bank = new Bank();

            using ( BankService service = new BankService() )
            {
                if ( service.Exist( model.Name ) )
                {
                    // Bank already exist!
                    Notify( $"Sorry, a Bank with the Name \"{model.Name}\" already exists!", NotificationType.Error );

                    return View( model );
                }

                bank.Name = model.Name;
                bank.Code = model.Code;
                bank.Description = model.Description;
                bank.Status = ( int ) model.Status;

                bank = service.Create( bank );

                Notify( "The Bank was successfully created.", NotificationType.Success );
            }

            return RedirectToAction( "Banks" );
        }

        //
        // GET: /Administration/EditBank/5
        [Requires( PermissionTo.Edit )]
        public ActionResult EditBank( int id )
        {
            Bank bank;

            using ( BankService service = new BankService() )
            {
                bank = service.GetById( id );
            }

            if ( bank == null )
            {
                Notify( "Sorry, the requested resource could not be found. Please try again", NotificationType.Error );

                return PartialView( "_AccessDenied" );
            }

            BankViewModel model = new BankViewModel()
            {
                Id = bank.Id,
                Name = bank.Name,
                Description = bank.Description,
                Code = bank.Code,
                Status = ( Status ) bank.Status,
                EditMode = true
            };

            return View( model );
        }

        //
        // POST: /Administration/EditBank/5
        [HttpPost]
        [Requires( PermissionTo.Edit )]
        public ActionResult EditBank( BankViewModel model )
        {
            if ( !ModelState.IsValid )
            {
                Notify( "Sorry, the selected Bank was not updated. Please correct all errors and try again.", NotificationType.Error );

                return View( model );
            }

            Bank bank;

            using ( BankService service = new BankService() )
            {
                bank = service.GetById( model.Id );

                if ( bank == null )
                {
                    Notify( "Sorry, that Bank does not exist! Please specify a valid Bank Id and try again.", NotificationType.Error );

                    return View( model );
                }

                if ( !bank.Name.Equals( model.Name ) && service.Exist( model.Name ) )
                {
                    // Bank already exist!
                    Notify( $"Sorry, a Bank with the Name \"{model.Name}\" already exists!", NotificationType.Error );

                    return View( model );
                }

                bank.Name = model.Name;
                bank.Description = model.Description;
                bank.Code = model.Code;
                bank.Status = ( int ) model.Status;

                bank = service.Update( bank );

                Notify( "The selected Bank's details were successfully updated.", NotificationType.Success );
            }

            return RedirectToAction( "Banks" );
        }

        //
        // POST: /Administration/DeleteBank/5
        [HttpPost]
        [Requires( PermissionTo.Delete )]
        public ActionResult DeleteBank( BankViewModel model )
        {
            Bank bank;

            using ( BankService service = new BankService() )
            {
                bank = service.GetById( model.Id );

                if ( bank == null )
                {
                    Notify( "Sorry, the requested resource could not be found. Please try again", NotificationType.Error );

                    return PartialView( "_AccessDenied" );
                }

                bank.Status = ( ( ( Status ) bank.Status ) == Status.Active ) ? ( int ) Status.Inactive : ( int ) Status.Active;

                service.Update( bank );

                Notify( "The selected Bank was successfully updated.", NotificationType.Success );
            }

            return RedirectToAction( "Banks" );
        }

        #endregion



        #region Decline Reasons

        //
        // GET: /Administration/DeclineReasonDetails/5
        public ActionResult DeclineReasonDetails( int id, bool layout = true )
        {
            DeclineReason model;

            using ( DeclineReasonService service = new DeclineReasonService() )
            {
                model = service.GetById( id );
            }

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

        //
        // GET: /Administration/AddDeclineReason/5 
        [Requires( PermissionTo.Create )]
        public ActionResult AddDeclineReason()
        {
            DeclineReasonViewModel model = new DeclineReasonViewModel() { EditMode = true };

            return View( model );
        }

        //
        // POST: /Administration/AddDeclineReason/5
        [HttpPost]
        [Requires( PermissionTo.Create )]
        public ActionResult AddDeclineReason( DeclineReasonViewModel model )
        {
            if ( !ModelState.IsValid )
            {
                Notify( "Sorry, the DeclineReason was not created. Please correct all errors and try again.", NotificationType.Error );

                return View( model );
            }

            DeclineReason DeclineReason = new DeclineReason();

            using ( DeclineReasonService service = new DeclineReasonService() )
            {
                DeclineReason.Description = model.Description;
                DeclineReason.Status = ( int ) model.Status;

                service.Create( DeclineReason );

                Notify( "The Decline Reason was successfully created.", NotificationType.Success );
            }

            return RedirectToAction( "DeclineReasons" );
        }

        //
        // GET: /Administration/EditDeclineReason/5
        [Requires( PermissionTo.Edit )]
        public ActionResult EditDeclineReason( int id )
        {
            DeclineReason declineReason;

            using ( DeclineReasonService service = new DeclineReasonService() )
            {
                declineReason = service.GetById( id );
            }

            if ( declineReason == null )
            {
                Notify( "Sorry, the requested resource could not be found. Please try again", NotificationType.Error );

                return PartialView( "_AccessDenied" );
            }

            DeclineReasonViewModel model = new DeclineReasonViewModel()
            {
                EditMode = true,
                Id = declineReason.Id,
                Description = declineReason.Description,
                Status = ( Status ) declineReason.Status,
            };

            return View( model );
        }

        //
        // POST: /Administration/EditDeclineReason/5
        [HttpPost]
        [Requires( PermissionTo.Edit )]
        public ActionResult EditDeclineReason( DeclineReasonViewModel model )
        {
            if ( !ModelState.IsValid )
            {
                Notify( "Sorry, the selected Decline Reason was not updated. Please correct all errors and try again.", NotificationType.Error );

                return View( model );
            }

            DeclineReason DeclineReason;

            using ( DeclineReasonService service = new DeclineReasonService() )
            {
                DeclineReason = service.GetById( model.Id );

                if ( DeclineReason == null )
                {
                    Notify( "Sorry, that Decline Reason does not exist! Please specify a valid Decline Reason Id and try again.", NotificationType.Error );

                    return View( model );
                }

                DeclineReason.Status = ( int ) model.Status;
                DeclineReason.Description = model.Description;

                DeclineReason = service.Update( DeclineReason );

                Notify( "The selected Decline Reason's details were successfully updated.", NotificationType.Success );
            }

            return RedirectToAction( "DeclineReasons" );
        }

        //
        // POST: /Administration/DeleteDeclineReason/5
        [HttpPost]
        [Requires( PermissionTo.Delete )]
        public ActionResult DeleteDeclineReason( DeclineReasonViewModel model )
        {
            DeclineReason DeclineReason;

            using ( DeclineReasonService service = new DeclineReasonService() )
            {
                DeclineReason = service.GetById( model.Id );

                if ( DeclineReason == null )
                {
                    Notify( "Sorry, the requested resource could not be found. Please try again", NotificationType.Error );

                    return PartialView( "_AccessDenied" );
                }

                DeclineReason.Status = ( ( ( Status ) DeclineReason.Status ) == Status.Active ) ? ( int ) Status.Inactive : ( int ) Status.Active;

                service.Update( DeclineReason );

                Notify( "The selected Decline Reason was successfully updated.", NotificationType.Success );
            }

            return RedirectToAction( "DeclineReasons" );
        }

        #endregion



        #region Regions

        //
        // GET: /Administration/RegionDetails/5
        public ActionResult RegionDetails( int id, bool layout = true )
        {
            Region model;

            using ( RegionService service = new RegionService() )
            {
                model = service.GetById( id );
            }

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

        //
        // GET: /Administration/AddRegion/5
        [Requires( PermissionTo.Create )]
        public ActionResult AddRegion()
        {
            RegionViewModel model = new RegionViewModel() { EditMode = true };

            return View( model );
        }

        //
        // POST: /Administration/AddRegion/5
        [HttpPost]
        [Requires( PermissionTo.Create )]
        public ActionResult AddRegion( RegionViewModel model )
        {
            if ( !ModelState.IsValid )
            {
                Notify( "Sorry, the Region was not created. Please correct all errors and try again.", NotificationType.Error );

                return View( model );
            }

            using ( RegionService rservice = new RegionService() )
            {
                if ( rservice.Exist( model.Name, model.PSPId ) )
                {
                    // Region already exist!
                    Notify( $"Sorry, a Region with the Name \"{model.Name}\" already exists for the selected PSP!", NotificationType.Error );

                    return View( model );
                }

                Region region = new Region
                {
                    Name = model.Name,
                    PSPId = model.PSPId,
                    CountryId = model.CountryId,
                    Status = ( int ) model.Status,
                    ProvinceId = model.ProvinceId,
                    Description = model.Description,
                    RegionManagerId = model.RegionManagerId
                };

                rservice.Create( region );

                Notify( "The Region was successfully created.", NotificationType.Success );
            }

            return Regions( new PagingModel(), new CustomSearchModel() );
        }

        //
        // GET: /Administration/EditRegion/5
        [Requires( PermissionTo.Edit )]
        public ActionResult EditRegion( int id )
        {
            using ( RegionService service = new RegionService() )
            {
                Region region = service.GetById( id );

                if ( region == null )
                {
                    Notify( "Sorry, the requested resource could not be found. Please try again", NotificationType.Error );

                    return PartialView( "_AccessDenied" );
                }

                RegionViewModel model = new RegionViewModel()
                {
                    Id = region.Id,
                    EditMode = true,
                    Name = region.Name,
                    PSPId = region.PSPId,
                    CountryId = region.CountryId,
                    ProvinceId = region.ProvinceId,
                    Description = region.Description,
                    Status = ( Status ) region.Status,
                    RegionManagerId = region.RegionManagerId,
                };

                return View( model );
            }
        }

        //
        // POST: /Administration/EditRegion/5
        [HttpPost]
        [Requires( PermissionTo.Edit )]
        public ActionResult EditRegion( RegionViewModel model )
        {
            if ( !ModelState.IsValid )
            {
                Notify( "Sorry, the selected Region was not updated. Please correct all errors and try again.", NotificationType.Error );

                return View( model );
            }

            using ( RegionService rservice = new RegionService() )
            {
                Region region = rservice.GetById( model.Id );

                if ( region == null )
                {
                    Notify( "Sorry, that Region does not exist! Please specify a valid Region Id and try again.", NotificationType.Error );

                    return View( model );
                }

                if ( region.Name.Trim().ToLower() != model.Name.Trim().ToLower() && region.PSPId != model.PSPId && rservice.Exist( model.Name, model.PSPId ) )
                {
                    // Region already exist!
                    Notify( $"Sorry, a Region with the Name \"{model.Name}\" already exists for the selected PSP!", NotificationType.Error );

                    return View( model );
                }

                region.Name = model.Name;
                region.PSPId = model.PSPId;
                region.CountryId = model.CountryId;
                region.ProvinceId = model.ProvinceId;
                region.Status = ( int ) model.Status;
                region.Description = model.Description;
                region.RegionManagerId = model.RegionManagerId;

                rservice.Update( region );

                Notify( "The selected Region's details were successfully updated.", NotificationType.Success );
            }

            return Regions( new PagingModel(), new CustomSearchModel() );
        }

        //
        // POST: /Administration/DeleteRegion/5
        [HttpPost]
        [Requires( PermissionTo.Delete )]
        public ActionResult DeleteRegion( RegionViewModel model )
        {
            using ( RegionService service = new RegionService() )
            {
                Region region = service.GetById( model.Id );

                if ( region == null )
                {
                    Notify( "Sorry, the requested resource could not be found. Please try again", NotificationType.Error );

                    return PartialView( "_AccessDenied" );
                }

                region.Status = ( ( ( Status ) region.Status ) == Status.Active ) ? ( int ) Status.Inactive : ( int ) Status.Active;

                service.Update( region );

                Notify( "The selected Region was successfully updated.", NotificationType.Success );
            }

            return Regions( new PagingModel(), new CustomSearchModel() );
        }

        #endregion



        #region Products

        //
        // GET: /Administration/ProductDetails/5
        public ActionResult ProductDetails( int id, bool layout = true )
        {
            using ( ProductService pservice = new ProductService() )
            using ( DocumentService dservice = new DocumentService() )
            {
                Product model = pservice.GetById( id );

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
        // GET: /Administration/AddProduct/5
        [Requires( PermissionTo.Create )]
        public ActionResult AddProduct()
        {
            ProductViewModel model = new ProductViewModel() { EditMode = true, ProductPrices = new List<ProductPriceViewModel>() };

            foreach ( int item in Enum.GetValues( typeof( ProductPriceType ) ) )
            {
                ProductPriceType type = ( ProductPriceType ) item;

                model.ProductPrices.Add( new ProductPriceViewModel()
                {
                    Type = type,
                    Status = Status.Active
                } );
            }

            return View( model );
        }

        //
        // POST: /Administration/AddProduct/5
        [HttpPost]
        [Requires( PermissionTo.Create )]
        public ActionResult AddProduct( ProductViewModel model )
        {
            if ( !ModelState.IsValid )
            {
                Notify( "Sorry, the Product was not created. Please correct all errors and try again.", NotificationType.Error );

                return View( model );
            }

            using ( ProductService pservice = new ProductService() )
            using ( TransactionScope scope = new TransactionScope() )
            using ( DocumentService dservice = new DocumentService() )
            using ( ProductPriceService ppservice = new ProductPriceService() )
            {
                #region Validations

                if ( pservice.Exist( model.Name ) )
                {
                    // Product already exist!
                    Notify( $"Sorry, a Product with the Name \"{model.Name}\" already exists!", NotificationType.Error );

                    return View( model );
                }

                #endregion

                #region Product

                Product product = new Product
                {
                    Name = model.Name,
                    Status = ( int ) model.Status,
                    Description = model.Description,
                };

                product = pservice.Create( product );

                #endregion

                #region Product Prices

                if ( model.ProductPrices.NullableAny() )
                {
                    foreach ( ProductPriceViewModel price in model.ProductPrices )
                    {
                        ProductPrice pp = new ProductPrice()
                        {
                            ProductId = product.Id,
                            Rate = price.Rate ?? 0,
                            Type = ( int ) price.Type,
                            RateUnit = price.RateUnit,
                            FromDate = price.StartDate,
                            Status = ( int ) price.Status,
                        };

                        ppservice.Create( pp );
                    }
                }

                #endregion

                #region Any Files

                if ( model.File != null )
                {
                    // Create folder
                    string path = Server.MapPath( $"~/{VariableExtension.SystemRules.DocumentsLocation}/Product/{model.Name.Trim().Replace( "/", "_" ).Replace( "\\", "_" )}/" );

                    if ( !Directory.Exists( path ) )
                    {
                        Directory.CreateDirectory( path );
                    }

                    string now = DateTime.Now.ToString( "yyyyMMddHHmmss" );

                    Document doc = new Document()
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

                Notify( "The Product was successfully created.", NotificationType.Success );
            }

            return RedirectToAction( "Products" );
        }

        //
        // GET: /Administration/EditProduct/5
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

                foreach ( int item in Enum.GetValues( typeof( ProductPriceType ) ) )
                {
                    ProductPriceType type = ( ProductPriceType ) item;

                    ProductPrice existingPrice = product.ProductPrices.FirstOrDefault( p => p.Type == item );


                    if ( existingPrice != null )
                    {
                        model.ProductPrices.Add( new ProductPriceViewModel()
                        {
                            Id = existingPrice.Id,
                            Rate = existingPrice.Rate,
                            RateUnit = existingPrice.RateUnit,
                            StartDate = existingPrice.FromDate,
                            ProductId = existingPrice.ProductId,
                            Status = ( Status ) existingPrice.Status,
                            Type = ( ProductPriceType ) existingPrice.Type
                        } );
                    }
                    else
                    {
                        model.ProductPrices.Add( new ProductPriceViewModel()
                        {
                            Type = type,
                            Status = Status.Inactive
                        } );
                    }
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
        // POST: /Administration/EditProduct/5
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
            using ( ProductPriceService ppservice = new ProductPriceService() )
            {
                Product product = pservice.GetById( model.Id );

                #region Validations

                if ( product == null )
                {
                    Notify( "Sorry, that Product does not exist! Please specify a valid Product Id and try again.", NotificationType.Error );

                    return View( model );
                }

                if ( product.Name != model.Name && pservice.Exist( model.Name ) )
                {
                    // Product already exist!
                    Notify( $"Sorry, a Product with the Name \"{model.Name}\" already exists!", NotificationType.Error );

                    return View( model );
                }

                #endregion

                #region Product

                product.Name = model.Name;
                product.Status = ( int ) model.Status;
                product.Description = model.Description;

                product = pservice.Update( product );

                #endregion

                #region Product Prices

                if ( model.ProductPrices.NullableAny() )
                {
                    foreach ( ProductPriceViewModel price in model.ProductPrices )
                    {
                        ProductPrice pp = ppservice.GetById( price.Id );

                        if ( pp == null )
                        {
                            pp = new ProductPrice()
                            {
                                ProductId = product.Id,
                                Rate = price.Rate ?? 0,
                                Type = ( int ) price.Type,
                                RateUnit = price.RateUnit,
                                FromDate = price.StartDate,
                                Status = ( int ) price.Status,
                            };

                            ppservice.Create( pp );
                        }
                        else
                        {
                            pp.Rate = price.Rate ?? 0;
                            pp.RateUnit = price.RateUnit;
                            pp.FromDate = price.StartDate;
                            pp.Status = ( int ) price.Status;

                            ppservice.Update( pp );
                        }
                    }
                }

                #endregion

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

            return RedirectToAction( "Products" );
        }

        //
        // POST: /Administration/DeleteProduct/5
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

            return RedirectToAction( "Products" );
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
                                ObjectType = "Transporter",
                                FleetNumber = mv.FleetNumber,
                                Status = ( int ) model.Status,
                                Registration = mv.Registration,
                                //EngineNumber = mv.EngineNumber,
                                Descriptoin = $"{mv.Make} {mv.Model}",
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

                    if ( load.NullableCount() < 2 )
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



        #region Old Client Load Import

        // POST: Administration/OldDataImport
        [HttpPost]
        [Requires( PermissionTo.Create )]
        public ActionResult ProcessOldDataImport( ClientLoadViewModel model )
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
            using ( UserService uservice = new UserService() )
            using ( RegionService rservice = new RegionService() )
            using ( VehicleService vservice = new VehicleService() )
            using ( ProvinceService pservice = new ProvinceService() )
            using ( ClientSiteService csservice = new ClientSiteService() )
            using ( ClientLoadService clservice = new ClientLoadService() )
            using ( PODCommentService podservice = new PODCommentService() )
            using ( TransporterService tservice = new TransporterService() )
            using ( ClientCustomerService ccservice = new ClientCustomerService() )
            using ( ClientAuthorisationService caservice = new ClientAuthorisationService() )
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

                    string date = load[ 0 ]?.Trim(),
                           palletReturnDate = load[ 1 ]?.Trim(),
                           equipmentCode = load[ 2 ]?.Trim(),
                           glid = load[ 3 ]?.Trim(),
                           supplierFrom = load[ 4 ]?.Trim(), // FromSite
                           loadNumber = load[ 5 ]?.Trim(),
                           customerType = load[ 6 ]?.Trim(),
                           accountingCode = load[ 7 ]?.Trim(),
                           customerTo = load[ 8 ]?.Trim(), // ToSite
                           regionFrom = load[ 9 ]?.Trim(),
                           provinceFrom = load[ 10 ]?.Trim(),
                           pcn = load[ 11 ]?.Trim(),
                           orderNumber = load[ 12 ]?.Trim(),
                           pod = load[ 13 ]?.Trim(),
                           doc = load[ 14 ]?.Trim(),
                           deliveryNoteNumber = load[ 15 ]?.Trim(),
                           deliveredQty = load[ 16 ]?.Trim(),
                           returnedQty = load[ 17 ]?.Trim(),
                           outstandingQty = load[ 18 ]?.Trim(),
                           podComments = load[ 19 ]?.Trim(),
                           transporterName = load[ 20 ]?.Trim(),
                           reg = load[ 21 ]?.Trim(),
                           palletReturnSlipNo = load[ 22 ]?.Trim(),
                           customerThan = load[ 23 ]?.Trim(),
                           depotThan = load[ 24 ]?.Trim(),
                           clientLoadNotes = load[ 25 ]?.Trim(),
                           authorizerName = load[ 26 ]?.Trim(),
                           authorizationCode = load[ 27 ]?.Trim();

                    int.TryParse( deliveredQty.Split( '.' )[ 0 ], out int qty );

                    int.TryParse( returnedQty.Split( '.' )[ 0 ], out int returnQty );

                    int.TryParse( outstandingQty.Split( '.' )[ 0 ], out int outstandQty );

                    string uid = clservice.GetSha1Md5String( $"{model.ClientId}{deliveryNoteNumber}{date}{loadNumber}{transporterName}{reg}" );

                    if ( uid.Length > 500 )
                    {
                        uid = uid.Substring( 0, 500 );
                    }

                    DateTime.TryParseExact( date, model.DateFormats.GetDisplayText(), CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime loadDate );

                    DateTime.TryParseExact( date, model.DateFormats.GetDisplayText(), CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime deliveryDate );

                    string returnDate = "NULL";

                    if ( DateTime.TryParseExact( palletReturnDate, model.DateFormats.GetDisplayText(), CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime prd ) )
                    {
                        returnDate = $"'{prd}'";
                    }

                    int? clId = clservice.GetIdByUID( uid );

                    #region Region

                    Region region = rservice.GetByCode( regionFrom );

                    #endregion

                    #region POD Comment

                    PODComment podC = podservice.GetByComment( podComments );

                    string podid = podC != null ? podC.Id + "" : "NULL";

                    #endregion

                    #region Transporter

                    int? t = tservice.GetIdByClientAndName( model.ClientId, transporterName );

                    if ( ( t == null || t <= 0 ) && !string.IsNullOrWhiteSpace( transporterName ) )
                    {
                        #region Create Transporter

                        cQuery = $" {cQuery} INSERT INTO [dbo].[Transporter]([ClientId],[CreatedOn],[ModifiedOn],[ModifiedBy],[Name],[TradingName],[Status]) ";
                        cQuery = $" {cQuery} VALUES ({model.ClientId},'{DateTime.Now}','{DateTime.Now}','{CurrentUser.Email}','{transporterName}','{transporterName}',{( int ) Status.Active}) ";

                        #endregion

                        try
                        {
                            tservice.Query( cQuery );

                            t = tservice.GetIdByClientAndName( model.ClientId, transporterName );
                        }
                        catch ( Exception ex )
                        {
                            errs.Add( ex.ToString() );
                        }
                    }

                    #endregion

                    string tid = t != null ? t + "" : "NULL";

                    #region Vehicle

                    int? v = vservice.GetIdByRegistrationNumber( reg, "Transporter" );

                    if ( ( v == null || v <= 0 ) && !string.IsNullOrWhiteSpace( reg ) )
                    {
                        #region Create Vehicle

                        cQuery = $" {cQuery} INSERT INTO [dbo].[Vehicle]([ObjectId],[CreatedOn],[ModifiedOn],[ModifiedBy],[Descriptoin],[Registration],[ObjectType],[Type],[Status]) ";
                        cQuery = $" {cQuery} VALUES ({tid},'{DateTime.Now}','{DateTime.Now}','{CurrentUser.Email}','{reg}','{reg}','Transporter',{( int ) VehicleType.Pickup},{( int ) Status.Active}) ";

                        #endregion

                        try
                        {
                            vservice.Query( cQuery );

                            v = vservice.GetIdByRegistrationNumber( reg, "Transporter" );
                        }
                        catch ( Exception ex )
                        {
                            errs.Add( ex.ToString() );
                        }
                    }

                    #endregion

                    string vid = v != null ? v + "" : "NULL";

                    #region Client Site 1

                    ClientSite cs1 = null;
                    Site site = sservice.GetByClientAndName( model.ClientId, supplierFrom );

                    if ( site == null && !string.IsNullOrWhiteSpace( supplierFrom ) )
                    {
                        site = new Site()
                        {
                            Name = supplierFrom,
                            RegionId = region?.Id,
                            Description = supplierFrom,
                            Status = ( int ) Status.Active,
                        };

                        site = sservice.Create( site );
                    }

                    if ( site != null && !site.ClientSites.NullableAny() && !string.IsNullOrWhiteSpace( supplierFrom ) )
                    {
                        ClientCustomer cc = ccservice.GetByNumber( model.ClientId, accountingCode );

                        if ( cc == null )
                        {
                            cc = new ClientCustomer()
                            {
                                ClientId = model.ClientId,
                                CustomerName = customerTo,
                                Status = ( int ) Status.Active,
                                CustomerNumber = accountingCode,
                                CustomerTown = provinceFrom,
                            };

                            cc = ccservice.Create( cc );
                        }

                        cs1 = new ClientSite()
                        {
                            GLIDNo = glid,
                            SiteId = site.Id,
                            ClientCustomerId = cc.Id,
                            Status = ( int ) Status.Active,
                            AccountingCode = accountingCode,
                            ClientSiteCode = accountingCode,
                            ClientCustomerNumber = accountingCode,
                        };

                        cs1 = csservice.Create( cs1 );
                    }
                    else if ( site != null && !string.IsNullOrWhiteSpace( supplierFrom ) )
                    {
                        cs1 = site.ClientSites.FirstOrDefault();
                    }

                    #endregion

                    #region Client Site 2

                    ClientSite cs2 = null;
                    Site site2 = sservice.GetByClientAndName( model.ClientId, customerTo );

                    if ( site2 == null && !string.IsNullOrWhiteSpace( customerTo ) )
                    {
                        site2 = new Site()
                        {
                            Name = customerTo,
                            Description = customerTo,
                            Status = ( int ) Status.Active,
                        };

                        site2 = sservice.Create( site2 );
                    }

                    if ( site2 != null && !site2.ClientSites.NullableAny() && !string.IsNullOrWhiteSpace( customerTo ) )
                    {
                        ClientCustomer cc = ccservice.GetByNumber( model.ClientId, accountingCode );

                        if ( cc == null )
                        {
                            cc = new ClientCustomer()
                            {
                                ClientId = model.ClientId,
                                Status = ( int ) Status.Active,
                                CustomerName = customerTo,
                                CustomerNumber = accountingCode,
                                CustomerTown = accountingCode,
                            };

                            cc = ccservice.Create( cc );
                        }

                        cs2 = new ClientSite()
                        {
                            GLIDNo = glid,
                            SiteId = site2.Id,
                            ClientCustomerId = cc.Id,
                            Status = ( int ) Status.Active,
                            AccountingCode = accountingCode,
                            ClientSiteCode = accountingCode,
                            ClientCustomerNumber = accountingCode,
                        };

                        cs2 = csservice.Create( cs2 );
                    }
                    else if ( site2 != null && !string.IsNullOrWhiteSpace( customerTo ) )
                    {
                        cs2 = site2.ClientSites.FirstOrDefault();
                    }

                    #endregion

                    string cs1id = cs1 != null ? cs1.Id + "" : "NULL";
                    string cs2id = cs2 != null ? cs2.Id + "" : "NULL";

                    if ( clId == null || clId <= 0 )
                    {
                        #region Create Client Load

                        cQuery = $" {cQuery} INSERT INTO [dbo].[ClientLoad]([ClientId],[ClientSiteId],[ToClientSiteId],[VehicleId],[TransporterId],[OutstandingReasonId],[PODCommentId],[CreatedOn],[ModifiedOn],[ModifiedBy],[LoadNumber],[LoadDate],[EffectiveDate],[NotifyDate],[AccountNumber],[ClientDescription],[DeliveryNote],[OriginalQuantity],[NewQuantity],[ReturnQty],[OutstandingQty],[PODNumber],[PCNNumber],[Status],[PostingType],[THAN],[PODStatus],[InvoiceStatus],[UID],[ChepCustomerThanDocNo],[WarehouseTransferDocNo],[EquipmentCode],[GLID],[CustomerType],[OrderNumber],[ReferenceNumber],[DocNumber],[PalletReturnSlipNo],[ClientLoadNotes]) ";
                        cQuery = $" {cQuery} VALUES ({model.ClientId},{cs1id},{cs2id},{vid},{tid},9,{podid},'{DateTime.Now}','{DateTime.Now}','{CurrentUser.Email}','{loadNumber}','{loadDate}','{loadDate}','{loadDate}','{accountingCode}','{customerTo}','{deliveryNoteNumber}',{qty},{qty},{returnQty},{outstandQty},'{pod}','{pcn}',{( int ) ReconciliationStatus.Unreconciled},{( int ) PostingType.Import},'{customerThan}',0,0,'{uid}','{customerThan}','{depotThan}','{equipmentCode}','{glid}','{customerType}','{orderNumber}','{orderNumber}','{doc}','{palletReturnSlipNo}','{clientLoadNotes}') ";

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
                                                    [VehicleId]={vid},
                                                    [ClientSiteId]={cs1id},
                                                    [ToClientSiteId]={cs2id},
                                                    [TransporterId]={tid},
                                                    [PODCommentId]={podid},
                                                    [LoadNumber]='{loadNumber}',
                                                    [LoadDate]='{loadDate}',
                                                    [EffectiveDate]='{deliveryDate}',
                                                    [NotifyDate]='{deliveryDate}',
                                                    [PalletReturnDate]={returnDate},
                                                    [AccountNumber]='{accountingCode}',
                                                    [ClientDescription]='{customerTo}',
                                                    [DeliveryNote]='{deliveryNoteNumber}',
                                                    [OriginalQuantity]={qty},
                                                    [NewQuantity]={qty},
                                                    [ReturnQty]={returnQty},
                                                    [OutstandingQty]={outstandQty},
                                                    [PODNumber]='{pod}',
                                                    [PCNNumber]='{pcn}',
                                                    [THAN]='{customerThan}',
                                                    [ChepCustomerThanDocNo]='{customerThan}',
                                                    [WarehouseTransferDocNo]='{depotThan}',
                                                    [EquipmentCode]='{equipmentCode}',
                                                    [GLID]='{glid}',
                                                    [CustomerType]='{customerType}',
                                                    [OrderNumber]='{orderNumber}',
                                                    [ReferenceNumber]='{orderNumber}',
                                                    [DocNumber]='{doc}',
                                                    [PalletReturnSlipNo]='{palletReturnSlipNo}',
                                                    [ClientLoadNotes]='{clientLoadNotes}'
                                                WHERE
                                                    [Id]={clId}";

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

                    #region Client Authorisation

                    if ( !string.IsNullOrWhiteSpace( authorizationCode ) && !string.IsNullOrWhiteSpace( authorizerName ) )
                    {
                        ClientAuthorisation ca = caservice.GetByCode( authorizationCode );

                        if ( ca == null )
                        {
                            clId = clId.HasValue ? clId : clservice.GetIdByUID( uid );

                            // User
                            User user = uservice.GetByMaybeNameOrSurname( authorizerName );

                            if ( user == null )
                            {
                                user = new User()
                                {
                                    Name = authorizerName,
                                    Surname = authorizerName,
                                    Email = authorizerName,
                                    Cell = string.Empty,
                                    Password = string.Empty,
                                    PasswordDate = DateTime.Now,
                                    Status = ( int ) Status.Active,
                                    Type = ( int ) RoleType.PSP,

                                };

                                user = uservice.Create( user );
                            }

                            ca = new ClientAuthorisation()
                            {
                                UserId = user.Id,
                                Code = authorizationCode,
                                LoadNumber = loadNumber,
                                ClientLoadId = clId.Value,
                                Status = ( int ) Status.Active,
                                AuthorisationDate = DateTime.Now,
                            };

                            ca = caservice.Create( ca );
                        }
                    }

                    #endregion
                }

                cQuery = string.Empty;
                uQuery = string.Empty;

                if ( errs.NullableAny() )
                {
                    errorDocId = LogImportErrors( errs, model.ClientId );
                }
            }

            AutoReconcileLoads( model.ClientId );

            string resp = $"{created} loads were successfully created, {updated} were updated, {skipped} were skipped and there were {errors} errors.";

            if ( errs.NullableAny() && errorDocId > 0 )
            {
                resp = $"{resp} <a href='/Pallet/ViewDocument/{errorDocId}' target='_blank'>Click here</a> to view the erros.";
            }

            Notify( resp, NotificationType.Success );

            return OldDataImport();
        }

        #endregion



        #region Partial Views

        //
        // POST || GET: /Administration/Users
        public ActionResult Users( PagingModel pm, CustomSearchModel csm, bool givecsm = false )
        {
            if ( givecsm )
            {
                ViewBag.ViewName = "Users";
                return PartialView( "_UsersCustomSearch", new CustomSearchModel( "Users" ) );
            }

            using ( UserService service = new UserService() )
            {
                List<UserCustomModel> model = service.List1( pm, csm );
                int total = ( model.Count < pm.Take && pm.Skip == 0 ) ? model.Count : service.Total1( pm, csm );

                PagingExtension paging = PagingExtension.Create( model, total, pm.Skip, pm.Take, pm.Page );

                return PartialView( "_Users", paging );
            }
        }


        //
        // POST || GET: /Administration/PSPs
        public ActionResult PSPs( PagingModel pm, CustomSearchModel csm, bool givecsm = false )
        {
            if ( givecsm )
            {
                ViewBag.ViewName = "PSPs";

                return PartialView( "_PSPCustomSearch", new CustomSearchModel( "PSPs" ) );
            }

            using ( PSPService service = new PSPService() )
            {
                List<PSPCustomModel> model = service.List1( pm, csm );
                int total = ( model.Count < pm.Take && pm.Skip == 0 ) ? model.Count : service.Total1( pm, csm );

                PagingExtension paging = PagingExtension.Create( model, total, pm.Skip, pm.Take, pm.Page );

                return PartialView( "_PSPs", paging );
            }
        }


        //
        // POST || GET: /Administration/Regions
        public ActionResult Regions( PagingModel pm, CustomSearchModel csm, bool givecsm = false )
        {
            if ( givecsm )
            {
                ViewBag.ViewName = "Regions";

                return PartialView( "_RegionCustomSearch", new CustomSearchModel( "Regions" ) );
            }

            using ( RegionService service = new RegionService() )
            {
                List<RegionCustomModel> model = service.List1( pm, csm );

                int total = ( model.Count < pm.Take && pm.Skip == 0 ) ? model.Count : service.Total1( pm, csm );

                PagingExtension paging = PagingExtension.Create( model, total, pm.Skip, pm.Take, pm.Page );

                return PartialView( "_Regions", paging );
            }
        }


        //
        // POST || GET: /Administration/Products
        public ActionResult Products( PagingModel pm, CustomSearchModel csm, bool givecsm = false )
        {
            if ( givecsm )
            {
                ViewBag.ViewName = "Products";

                return PartialView( "_ProductCustomSearch", new CustomSearchModel( "Products" ) );
            }

            int total = 0;

            List<ProductCustomModel> model = new List<ProductCustomModel>();

            using ( ProductService service = new ProductService() )
            {
                model = service.List1( pm, csm );
                total = ( model.Count < pm.Take && pm.Skip == 0 ) ? model.Count : service.Total1( pm, csm );
            }

            PagingExtension paging = PagingExtension.Create( model, total, pm.Skip, pm.Take, pm.Page );

            return PartialView( "_Products", paging );
        }


        //
        // POST || GET: /Administration/AuditLog
        public ActionResult AuditLog( PagingModel pm, CustomSearchModel csm, bool givecsm = false )
        {
            if ( givecsm )
            {
                ViewBag.ViewName = "_AuditLog";
                return PartialView( "_AuditLogCustomSearch", new CustomSearchModel( "AuditLog" ) );
            }

            int total = 0;

            List<AuditLogCustomModel> model = new List<AuditLogCustomModel>();

            using ( AuditLogService service = new AuditLogService() )
            {
                pm.Sort = pm.Sort ?? "DESC";
                pm.SortBy = pm.SortBy ?? "CreatedOn";

                model = service.List( pm, csm );
                total = ( model.Count < pm.Take && pm.Skip == 0 ) ? model.Count : service.Total( pm, csm );
            }

            PagingExtension paging = PagingExtension.Create( model, total, pm.Skip, pm.Take, pm.Page );

            return PartialView( "_AuditLog", paging );
        }


        //
        // POST || GET: /Administration/Broadcasts
        public ActionResult Broadcasts( PagingModel pm, CustomSearchModel csm, bool givecsm = false )
        {
            if ( givecsm )
            {
                ViewBag.ViewName = "_Broadcasts";

                return PartialView( "_BroadcastCustomSearch", new CustomSearchModel( "Broadcast" ) );
            }

            pm.Take = int.MaxValue;

            pm.Sort = "DESC";
            pm.SortBy = "CreatedOn";

            int total = 0;

            List<BroadcastCustomModel> model;

            using ( BroadcastService service = new BroadcastService() )
            {
                model = service.List1( pm, csm );
                total = ( model.Count < pm.Take && pm.Skip == 0 ) ? model.Count : service.Total1( pm, csm );
            }

            PagingExtension paging = PagingExtension.Create( model, total, pm.Skip, pm.Take, pm.Page );

            return PartialView( "_Broadcasts", paging );
        }


        //
        // POST || GET: /Administration/Banks
        public ActionResult Banks( PagingModel pm, CustomSearchModel csm )
        {
            int total = 0;

            List<Bank> model = new List<Bank>();

            using ( BankService service = new BankService() )
            {
                model = service.List( pm, csm );
                total = ( model.Count < pm.Take && pm.Skip == 0 ) ? model.Count : service.Total( pm, csm );
            }

            PagingExtension paging = PagingExtension.Create( ( object ) model, total, pm.Skip, pm.Take, pm.Page );

            return PartialView( "_Banks", paging );
        }

        //
        // POST || GET: /Administration/Roles
        public ActionResult Roles( PagingModel pm, CustomSearchModel csm )
        {
            int total = 0;

            List<Role> model = new List<Role>();

            using ( RoleService service = new RoleService() )
            {
                pm.Sort = pm.Sort ?? "DESC";
                pm.SortBy = pm.SortBy ?? "CreatedOn";

                model = service.List( pm, csm );
                total = ( model.Count < pm.Take && pm.Skip == 0 ) ? model.Count : service.Total( pm, csm );
            }

            PagingExtension paging = PagingExtension.Create( model, total, pm.Skip, pm.Take, pm.Page );

            return PartialView( "_Roles", paging );
        }


        //
        // POST || GET: /Administration/SystemConfig
        public ActionResult SystemConfig( PagingModel pm, CustomSearchModel csm )
        {
            SystemConfig model;

            using ( SystemConfigService service = new SystemConfigService() )
            {
                model = service.List( pm, csm ).FirstOrDefault() ?? new SystemConfig() { };
            }

            return PartialView( "_SystemConfig", model );
        }


        //
        // POST || GET: /Administration/DeclineReasons
        public ActionResult DeclineReasons( PagingModel pm, CustomSearchModel csm )
        {
            using ( DeclineReasonService service = new DeclineReasonService() )
            {
                List<DeclineReason> model = service.List( pm, csm );
                int total = ( model.Count < pm.Take && pm.Skip == 0 ) ? model.Count : service.Total( pm, csm );

                PagingExtension paging = PagingExtension.Create( ( object ) model, total, pm.Skip, pm.Take, pm.Page );

                return PartialView( "_DeclineReasons", paging );
            }
        }

        //
        // GET: /Administration/ManageTransporters
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

        // GET: Administration/OldDataImport
        [Requires( PermissionTo.Create )]
        public ActionResult OldDataImport()
        {
            ClientLoadViewModel model = new ClientLoadViewModel() { EditMode = true };

            return View( "OldDataImport", model );
        }

        #endregion
    }
}
