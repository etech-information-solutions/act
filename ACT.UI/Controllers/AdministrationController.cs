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
                case "roles":

                    #region Roles

                    csv = String.Format( "Name, Dashboard, Campaign, Dinner, Trading Partner, Member, Client, Reward, Administration, Reports {0}", Environment.NewLine );

                    List<Role> roles = new List<Role>();

                    using ( RoleService service = new RoleService() )
                    {
                        roles = service.List( pm, csm );
                    }

                    if ( roles != null && roles.Any() )
                    {
                        foreach ( Role item in roles )
                        {
                            //csv = String.Format( "{0} {1},{2},{3},{4},{5},{6},{7},{8},{9},{10} {11}",
                            //                    csv,
                            //                    item.Name,
                            //                    item.DashBoard,
                            //                    item.Campaign,
                            //                    item.Dinner,
                            //                    item.TradingPartner,
                            //                    item.Member,
                            //                    item.Client,
                            //                    item.Reward,
                            //                    item.Administration,
                            //                    item.Report,
                            //                    Environment.NewLine );
                        }
                    }

                    #endregion

                    break;

                case "systemconfig":

                    #region System Config

                    csv = String.Format( "System Contact Email, Finance Contact Email, Password Change, Images Location, Documents Location {0}", Environment.NewLine );

                    List<SystemConfig> items = new List<SystemConfig>();

                    using ( SystemConfigService service = new SystemConfigService() )
                    {
                        items = service.List( pm, csm );
                    }

                    if ( items != null && items.Any() )
                    {
                        foreach ( SystemConfig item in items )
                        {
                            //csv = String.Format( "{0} {1},{2},{3},{4},{5} {6}",
                            //                    csv,
                            //                    item.ContactEmail,
                            //                    item.FinancialEmail,
                            //                    item.PasswordChange,
                            //                    item.ImagesLocation,
                            //                    item.DocumentsLocation,
                            //                    Environment.NewLine );
                        }
                    }

                    #endregion

                    break;

                case "auditlog":

                    #region Audit Log

                    csv = string.Format( "Date, Activity, User, Action Table, Action, Controller, Comments, Image Before, Image After, Browser {0}", Environment.NewLine );

                    List<AuditLogCustomModel> auditModel = new List<AuditLogCustomModel>();

                    using ( AuditLogService service = new AuditLogService() )
                    {
                        auditModel = service.List( pm, csm );
                    }

                    if ( auditModel != null && auditModel.Any() )
                    {
                        foreach ( AuditLogCustomModel item in auditModel )
                        {
                            ActivityTypes activity = ( ActivityTypes ) item.Type;

                            csv = string.Format( "{0} {1},{2},{3},{4},{5},{6},{7},{8},{9},{10} {11}",
                                                csv,
                                                item.CreatedOn.ToString( "yyyy/MM/dd" ),
                                                activity.GetDisplayText(),
                                                item.User.Name + " " + item.User.Surname,
                                                item.ActionTable,
                                                item.Action,
                                                item.Controller,
                                                "\"" + item.Comments + "\"",
                                                "\"" + ( item.BeforeImage ?? "" ).Replace( '"', ' ' ) + "\"",
                                                "\"" + ( item.BeforeImage ?? "" ).Replace( '"', ' ' ) + "\"",
                                                "\"" + ( item.Browser ?? "" ) + "\"",
                                                Environment.NewLine );
                        }
                    }

                    #endregion

                    break;

                case "broadcasts":

                    #region BroadCasts

                    csv = String.Format( "Date Created, Start Date, End Date, Status, xRead, Message {0}", Environment.NewLine );

                    List<Broadcast> broadcasts = new List<Broadcast>();

                    using ( BroadcastService service = new BroadcastService() )
                    {
                        broadcasts = service.List( pm, csm );
                    }

                    if ( broadcasts != null && broadcasts.Any() )
                    {
                        foreach ( Broadcast item in broadcasts )
                        {
                            Status status = ( Status ) item.Status;

                            csv = String.Format( "{0} {1},{2},{3},{4},{5},{6} {7}",
                                                csv,
                                                item.CreatedOn,
                                                item.StartDate,
                                                item.EndDate,
                                                status.GetDisplayText(),
                                                item.UserBroadcasts.Count,
                                                item.Message,
                                                Environment.NewLine );
                        }
                    }

                    #endregion

                    break;

                case "users":

                    #region Manage Users

                    csv = string.Format( "Date Created, Name, Status, Role, Email, Cell {0}", Environment.NewLine );

                    List<User> userModel = new List<User>();

                    using ( UserService uservice = new UserService() )
                    {
                        userModel = uservice.List( pm, csm );

                        if ( userModel != null && userModel.Any() )
                        {
                            foreach ( User item in userModel )
                            {
                                Status status = ( Status ) item.Status;

                                Role role = new Role() { Name = "~/~", Type = -1 };

                                if ( item.UserRoles.Any() )
                                {
                                    role = item.UserRoles.FirstOrDefault().Role;
                                }

                                RoleType roleType = ( RoleType ) role.Type;

                                csv = string.Format( "{0} {1},{2},{3},{4},{5},{6} {7}", csv,
                                                    item.CreatedOn.ToString( "yyyy/MM/dd" ),
                                                    item.Name + " " + item.Surname,
                                                    status.GetDisplayText(),
                                                    roleType.GetDisplayText(),
                                                    item.Email,
                                                    item.Cell,
                                                    Environment.NewLine );
                            }
                        }
                    }

                    #endregion

                    break;

                case "banks":

                    #region Banks

                    csv = string.Format( "Date Created, Name, Description, Code, Status {0}", Environment.NewLine );

                    List<Bank> banks = new List<Bank>();

                    using ( BankService service = new BankService() )
                    {
                        banks = service.List( pm, csm );
                    }

                    if ( banks != null && banks.Any() )
                    {
                        foreach ( Bank item in banks )
                        {
                            Status status = ( Status ) ( item.Status );

                            csv = string.Format( "{0} {1},{2},{3},{4},{5} {6}",
                                                csv,
                                                item.CreatedOn.ToString( "yyyy/MM/dd" ),
                                                item.Name,
                                                item.Description,
                                                item.Code,
                                                status.GetDisplayText(),
                                                Environment.NewLine );
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

            User user;

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
                user = new User()
                {
                    Type = role.Type,
                    Cell = model.Cell,
                    Name = model.Name,
                    Email = model.Email,
                    Surname = model.Surname,
                    PasswordDate = DateTime.Now,
                    Status = ( int ) model.Status,
                    Password = uservice.GetSha1Md5String( model.Password )
                };

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
            }

            Notify( "The User was successfully created.", NotificationType.Success );

            return RedirectToAction( "Users" );
        }

        //
        // GET: /Administration/EditUser/5
        [Requires( PermissionTo.Edit )]
        public ActionResult EditUser( int id )
        {
            User user;

            using ( UserService service = new UserService() )
            {
                user = service.GetById( id );
            }

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
                PSPId = user.PSPUsers.FirstOrDefault()?.PSPId ?? 0,
                ClientId = user.ClientUsers.FirstOrDefault()?.ClientId ?? 0
            };

            return View( model );
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

            User user;

            using ( UserService uservice = new UserService() )
            using ( RoleService rservice = new RoleService() )
            {
                user = uservice.GetById( model.Id );

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
            }

            Notify( "The selected User's details were successfully updated.", NotificationType.Success );

            return RedirectToAction( "Users" );
        }

        //
        // POST: /Administration/DeleteUser/5
        [HttpPost]
        [Requires( PermissionTo.Delete )]
        public ActionResult DeleteUser( UserViewModel model, PagingModel pm )
        {
            User user;

            using ( UserService service = new UserService() )
            {
                user = service.GetById( model.Id );

                if ( user == null )
                {
                    Notify( "Sorry, the requested resource could not be found. Please try again", NotificationType.Error );

                    return PartialView( "_AccessDenied" );
                }

                user.Status = ( ( ( Status ) user.Status ) == Status.Active ) ? ( int ) Status.Inactive : ( int ) Status.Active;

                service.Update( user );

                Notify( "The selected User was successfully updated.", NotificationType.Success );
            }

            return RedirectToAction( "Users" );
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
            PSPViewModel model = new PSPViewModel();

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
                    CompanyName = model.CompanyName,
                    Description = model.Description,
                    ContactPerson = model.ContactPerson,
                    ContactNumber = model.ContactNumber,
                    FinancialPerson = model.ContactPerson,
                    Status = ( int ) PSPClientStatus.Verified,
                    ServiceRequired = ( int ) model.ServiceType,
                    CompanyRegistrationNumber = model.CompanyRegistrationNumber,

                };

                psp = pservice.Create( psp );

                #endregion

                #region Create PSP Budget

                if ( model.PSPBudget != null )
                {
                    PSPBudget budget = new PSPBudget()
                    {
                        PSPId = psp.Id,
                        Status = ( int ) Status.Active,
                        BudgetYear = DateTime.Now.Year,
                        January = model.PSPBudget.January,
                        February = model.PSPBudget.February,
                        March = model.PSPBudget.March,
                        April = model.PSPBudget.April,
                        May = model.PSPBudget.May,
                        June = model.PSPBudget.June,
                        July = model.PSPBudget.July,
                        August = model.PSPBudget.August,
                        September = model.PSPBudget.September,
                        October = model.PSPBudget.October,
                        November = model.PSPBudget.November,
                        December = model.PSPBudget.December,

                    };

                    budget = bservice.Create( budget );
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
                        Province = ( int ) model.Address.Province,
                    };

                    aservice.Create( address );
                }

                #endregion

                #region Any Uploads

                if ( model.RegistrationFile != null )
                {
                    // Create folder
                    string path = Server.MapPath( $"~/{VariableExtension.SystemRules.DocumentsLocation}/PSP/{model.CompanyName.Trim()}-{model.CompanyRegistrationNumber.Trim().Replace( "/", "_" ).Replace( "\\", "_" )}/" );

                    if ( !Directory.Exists( path ) )
                    {
                        Directory.CreateDirectory( path );
                    }

                    string now = DateTime.Now.ToString( "yyyyMMddHHmmss" );

                    Document doc = new Document()
                    {
                        ObjectId = psp.Id,
                        ObjectType = "PSP",
                        Status = ( int ) Status.Active,
                        Name = model.RegistrationFile.Name,
                        Category = model.RegistrationFile.Name,
                        Title = model.RegistrationFile.File.FileName,
                        Size = model.RegistrationFile.File.ContentLength,
                        Description = model.RegistrationFile.Description,
                        Type = Path.GetExtension( model.RegistrationFile.File.FileName ),
                        Location = $"PSP/{model.CompanyName.Trim()}-{model.CompanyRegistrationNumber.Trim().Replace( "/", "_" ).Replace( "\\", "_" )}/{now}-{model.RegistrationFile.File.FileName}"
                    };

                    dservice.Create( doc );

                    string fullpath = Path.Combine( path, $"{now}-{model.RegistrationFile.File.FileName}" );
                    model.RegistrationFile.File.SaveAs( fullpath );
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
            if ( !ModelState.IsValid )
            {
                Notify( "Sorry, the selected PSP was not updated. Please correct all errors and try again.", NotificationType.Error );

                return View( model );
            }

            PSP psp;

            using ( PSPService pservice = new PSPService() )
            using ( AddressService aservice = new AddressService() )
            using ( TransactionScope scope = new TransactionScope() )
            using ( DocumentService dservice = new DocumentService() )
            using ( PSPBudgetService bservice = new PSPBudgetService() )
            {
                psp = pservice.GetById( model.Id );

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
                //psp.Status = ( int ) model.Status;
                psp.AdminEmail = model.AdminEmail;
                psp.CompanyName = model.CompanyName;
                psp.Description = model.Description;
                psp.ContactPerson = model.ContactPerson;
                psp.ContactNumber = model.ContactNumber;
                psp.FinancialPerson = model.ContactPerson;
                psp.ServiceRequired = ( int ) model.ServiceType;
                psp.CompanyRegistrationNumber = model.CompanyRegistrationNumber;

                psp = pservice.Update( psp );

                #endregion

                #region Update PSP Budget

                if ( model.PSPBudget != null )
                {
                    PSPBudget budget = bservice.GetById( model.PSPBudget.Id );

                    if ( budget == null )
                    {
                        budget = new PSPBudget()
                        {
                            PSPId = psp.Id,
                            Status = ( int ) Status.Active,
                            BudgetYear = DateTime.Now.Year,
                            January = model.PSPBudget.January,
                            February = model.PSPBudget.February,
                            March = model.PSPBudget.March,
                            April = model.PSPBudget.April,
                            May = model.PSPBudget.May,
                            June = model.PSPBudget.June,
                            July = model.PSPBudget.July,
                            August = model.PSPBudget.August,
                            September = model.PSPBudget.September,
                            October = model.PSPBudget.October,
                            November = model.PSPBudget.November,
                            December = model.PSPBudget.December,

                        };

                        bservice.Create( budget );
                    }
                    else
                    {
                        budget.BudgetYear = DateTime.Now.Year;
                        budget.January = model.PSPBudget.January;
                        budget.February = model.PSPBudget.February;
                        budget.March = model.PSPBudget.March;
                        budget.April = model.PSPBudget.April;
                        budget.May = model.PSPBudget.May;
                        budget.June = model.PSPBudget.June;
                        budget.July = model.PSPBudget.July;
                        budget.August = model.PSPBudget.August;
                        budget.September = model.PSPBudget.September;
                        budget.October = model.PSPBudget.October;
                        budget.November = model.PSPBudget.November;
                        budget.December = model.PSPBudget.December;

                        bservice.Update( budget );
                    }
                }

                #endregion

                #region Create Address (s)

                if ( model.Address != null )
                {
                    Address address = aservice.GetById( model.Address.Id );

                    if ( address == null )
                    {
                        address = new Address()
                        {
                            ObjectId = psp.Id,
                            ObjectType = "PSP",
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

                if ( model.RegistrationFile.File != null )
                {
                    // Create folder
                    string path = Server.MapPath( $"~/{VariableExtension.SystemRules.DocumentsLocation}/PSP/{model.CompanyName.Trim()}-{model.CompanyRegistrationNumber.Trim().Replace( "/", "_" ).Replace( "\\", "_" )}/" );

                    if ( !Directory.Exists( path ) )
                    {
                        Directory.CreateDirectory( path );
                    }

                    string now = DateTime.Now.ToString( "yyyyMMddHHmmss" );

                    Document doc = dservice.GetById( model.RegistrationFile.Id );

                    if ( doc != null )
                    {
                        // Disable this file...
                        doc.Status = ( int ) Status.Inactive;

                        dservice.Update( doc );
                    }

                    doc = new Document()
                    {
                        ObjectId = psp.Id,
                        ObjectType = "PSP",
                        Status = ( int ) Status.Active,
                        Name = model.RegistrationFile.Name,
                        Category = model.RegistrationFile.Name,
                        Title = model.RegistrationFile.File.FileName,
                        Size = model.RegistrationFile.File.ContentLength,
                        Description = model.RegistrationFile.Description,
                        Type = Path.GetExtension( model.RegistrationFile.File.FileName ),
                        Location = $"PSP/{model.CompanyName.Trim()}-{model.CompanyRegistrationNumber.Trim().Replace( "/", "_" ).Replace( "\\", "_" )}/{now}-{model.RegistrationFile.File.FileName}"
                    };

                    dservice.Create( doc );

                    string fullpath = Path.Combine( path, $"{now}-{model.RegistrationFile.File.FileName}" );
                    model.RegistrationFile.File.SaveAs( fullpath );
                }

                #endregion

                // Complete the scope
                scope.Complete();
            }

            Notify( "The selected PSP's details were successfully updated.", NotificationType.Success );

            return RedirectToAction( "PSPs" );
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

            return RedirectToAction( "PSPs" );
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

                EstimatedLoad load = new EstimatedLoad();

                bool unverified = ( psp.Status == ( int ) PSPClientStatus.Unverified );

                if ( unverified )
                {
                    load = eservice.Get( psp.Id, "PSP" );
                }

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
                    Status = ( PSPClientStatus ) psp.Status,
                    ServiceType = ( ServiceType ) psp.ServiceRequired,
                    CompanyRegistrationNumber = psp.CompanyRegistrationNumber,

                    RegistrationFile = new FileViewModel()
                    {
                        Name = documents?.FirstOrDefault()?.Name,
                        Id = documents?.FirstOrDefault()?.Id ?? 0,
                        Extension = documents?.FirstOrDefault()?.Type,
                        Description = documents?.FirstOrDefault()?.Description,
                    },
                    PSPBudget = new EstimatedLoadViewModel()
                    {
                        Id = ( unverified ) ? 0 : psp.PSPBudgets?.FirstOrDefault()?.Id ?? 0,
                        January = ( unverified ) ? load.January : psp.PSPBudgets?.FirstOrDefault()?.January,
                        February = ( unverified ) ? load.February : psp.PSPBudgets?.FirstOrDefault()?.February,
                        March = ( unverified ) ? load.March : psp.PSPBudgets?.FirstOrDefault()?.March,
                        April = ( unverified ) ? load.April : psp.PSPBudgets?.FirstOrDefault()?.April,
                        May = ( unverified ) ? load.May : psp.PSPBudgets?.FirstOrDefault()?.May,
                        June = ( unverified ) ? load.June : psp.PSPBudgets?.FirstOrDefault()?.June,
                        July = ( unverified ) ? load.July : psp.PSPBudgets?.FirstOrDefault()?.July,
                        August = ( unverified ) ? load.August : psp.PSPBudgets?.FirstOrDefault()?.August,
                        September = ( unverified ) ? load.September : psp.PSPBudgets?.FirstOrDefault()?.September,
                        October = ( unverified ) ? load.October : psp.PSPBudgets?.FirstOrDefault()?.October,
                        November = ( unverified ) ? load.November : psp.PSPBudgets?.FirstOrDefault()?.November,
                        December = ( unverified ) ? load.December : psp.PSPBudgets?.FirstOrDefault()?.December,
                    },
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
                    }
                };

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

                if ( !ModelState.IsValid )
                {
                    Notify( "Sorry, the selected PSP was not updated. Please correct all errors and try again.", NotificationType.Error );

                    model = ConstructPSPViewModel( psp );

                    return PartialView( "_ApproveDeclinePSP", model );
                }

                #region Update PSP

                psp.Status = ( int ) model.Status;

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

                #region Create PSPBilling

                if ( model.PSPBudget != null )
                {
                    PSPBudget budget = new PSPBudget()
                    {
                        PSPId = psp.Id,
                        BudgetYear = DateTime.Now.Year,
                        Status = ( int ) Status.Active,
                        January = model.PSPBudget.January,
                        February = model.PSPBudget.February,
                        March = model.PSPBudget.March,
                        April = model.PSPBudget.April,
                        May = model.PSPBudget.May,
                        June = model.PSPBudget.June,
                        July = model.PSPBudget.July,
                        August = model.PSPBudget.August,
                        September = model.PSPBudget.September,
                        October = model.PSPBudget.October,
                        November = model.PSPBudget.November,
                        December = model.PSPBudget.December,
                    };

                    bservice.Create( budget );
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

                    pservice.Update( config );
                }
            }

            Notify( "The PSP Configuration details were successfully updated.", NotificationType.Success );

            return PSPs( new PagingModel(), new CustomSearchModel() );
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

                return PartialView( "_PSPClients", model );
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

                return PartialView( "_PSPBudgets", model );
            }
        }

        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public JsonResult GetPSPBudgets(string PSPId)
        {
            if (PSPId != null && PSPId != "")
            {
                List<ClientBudget> load = null;

                using (ClientBudgetService bservice = new ClientBudgetService())
                {
                    load = bservice.ListByColumnWhere("PSPId", int.Parse(PSPId));
                    // return Json(load.Any(l => l.Status == (int)Status.Active), JsonRequestBehavior.AllowGet);
                    return Json(load, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(data: "Error", behavior: JsonRequestBehavior.AllowGet);
            }
        }

        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public JsonResult SetPSPBudget(string Id, string PSPId, string BudgetYear, string January, string February, string March, string April, string May, string June, string July, string August, string September, string October, string November, string December)
        {
            if (Id != null)
            {
                using (PSPBudgetService bservice = new PSPBudgetService())
                using (TransactionScope scope = new TransactionScope())
                {
                    //Collection of budgets or singular?
                    if (int.Parse(Id) > 0)
                    {
                        PSPBudget budget = bservice.GetById(int.Parse(Id));

                        budget.PSPId = int.Parse(PSPId);
                        budget.Status = (int)Status.Active;
                        budget.BudgetYear = DateTime.Now.Year;
                        budget.January = int.Parse(January);
                        budget.February = int.Parse(February);
                        budget.March = int.Parse(March);
                        budget.April = int.Parse(April);
                        budget.May = int.Parse(May);
                        budget.June = int.Parse(June);
                        budget.July = int.Parse(July);
                        budget.August = int.Parse(August);
                        budget.September = int.Parse(September);
                        budget.October = int.Parse(October);
                        budget.November = int.Parse(November);
                        budget.December = int.Parse(December);
                        budget.Status = (int)Status.Active;


                        bservice.Update(budget);
                    }
                    else
                    {
                        PSPBudget budget = new PSPBudget();
                        budget.PSPId = int.Parse(PSPId);
                        budget.BudgetYear = DateTime.Now.Year;
                        budget.January = int.Parse(January);
                        budget.February = int.Parse(February);
                        budget.March = int.Parse(March);
                        budget.April = int.Parse(April);
                        budget.May = int.Parse(May);
                        budget.June = int.Parse(June);
                        budget.July = int.Parse(July);
                        budget.August = int.Parse(August);
                        budget.September = int.Parse(September);
                        budget.October = int.Parse(October);
                        budget.November = int.Parse(November);
                        budget.December = int.Parse(December);
                        budget.Status = (int)Status.Active;

                        bservice.Create(budget);
                    }
                    scope.Complete();
                }

                return Json(data: "True", behavior: JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(data: "Error", behavior: JsonRequestBehavior.AllowGet);
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

            Region region = new Region();

            using ( RegionService service = new RegionService() )
            {
                if ( service.Exist( model.Name, model.PSPId ) )
                {
                    // Region already exist!
                    Notify( $"Sorry, a Region with the Name \"{model.Name}\" already exists for the selected PSP!", NotificationType.Error );

                    return View( model );
                }

                region.Name = model.Name;
                region.PSPId = model.PSPId;
                region.Status = ( int ) model.Status;
                region.Description = model.Description;
                region.RegionManagerId = model.RegionManagerId;

                region = service.Create( region );

                Notify( "The Region was successfully created.", NotificationType.Success );
            }

            return RedirectToAction( "Regions" );
        }

        //
        // GET: /Administration/EditRegion/5
        [Requires( PermissionTo.Edit )]
        public ActionResult EditRegion( int id )
        {
            Region region;

            using ( RegionService service = new RegionService() )
            {
                region = service.GetById( id );
            }

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
                Description = region.Description,
                Status = ( Status ) region.Status,
                RegionManagerId = region.RegionManagerId,
            };

            return View( model );
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

            Region region;

            using ( RegionService service = new RegionService() )
            {
                region = service.GetById( model.Id );

                if ( region == null )
                {
                    Notify( "Sorry, that Region does not exist! Please specify a valid Region Id and try again.", NotificationType.Error );

                    return View( model );
                }

                if ( region.Name != model.Name && region.PSPId != model.PSPId && service.Exist( model.Name, model.PSPId ) )
                {
                    // Region already exist!
                    Notify( $"Sorry, a Region with the Name \"{model.Name}\" already exists for the selected PSP!", NotificationType.Error );

                    return View( model );
                }

                region.Name = model.Name;
                region.PSPId = model.PSPId;
                region.Status = ( int ) model.Status;
                region.Description = model.Description;
                region.RegionManagerId = model.RegionManagerId;

                region = service.Update( region );

                Notify( "The selected Region's details were successfully updated.", NotificationType.Success );
            }

            return RedirectToAction( "Regions" );
        }

        //
        // POST: /Administration/DeleteRegion/5
        [HttpPost]
        [Requires( PermissionTo.Delete )]
        public ActionResult DeleteRegion( RegionViewModel model )
        {
            Region region;

            using ( RegionService service = new RegionService() )
            {
                region = service.GetById( model.Id );

                if ( region == null )
                {
                    Notify( "Sorry, the requested resource could not be found. Please try again", NotificationType.Error );

                    return PartialView( "_AccessDenied" );
                }

                region.Status = ( ( ( Status ) region.Status ) == Status.Active ) ? ( int ) Status.Inactive : ( int ) Status.Active;

                service.Update( region );

                Notify( "The selected Region was successfully updated.", NotificationType.Success );
            }

            return RedirectToAction( "Regions" );
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

            Product product = new Product();

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

                product.Name = model.Name;
                product.Status = ( int ) model.Status;
                product.Description = model.Description;

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

            int total = 0;

            List<UserCustomModel> model;

            using ( UserService service = new UserService() )
            {
                model = service.ListCSM( pm, csm );
                total = ( model.Count < pm.Take && pm.Skip == 0 ) ? model.Count : service.Total1( pm, csm );
            }

            PagingExtension paging = PagingExtension.Create( model, total, pm.Skip, pm.Take, pm.Page );

            return PartialView( "_Users", paging );
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

            int total = 0;

            List<PSPCustomModel> model = new List<PSPCustomModel>();

            using ( PSPService service = new PSPService() )
            {
                model = service.ListCSM( pm, csm );
                total = ( model.Count < pm.Take && pm.Skip == 0 ) ? model.Count : service.Total1( pm, csm );
            }

            PagingExtension paging = PagingExtension.Create( model, total, pm.Skip, pm.Take, pm.Page );

            return PartialView( "_PSPs", paging );
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

            int total = 0;

            List<RegionCustomModel> model = new List<RegionCustomModel>();

            using ( RegionService service = new RegionService() )
            {
                model = service.ListCSM( pm, csm );
                total = ( model.Count < pm.Take && pm.Skip == 0 ) ? model.Count : service.Total1( pm, csm );
            }

            PagingExtension paging = PagingExtension.Create( model, total, pm.Skip, pm.Take, pm.Page );

            return PartialView( "_Regions", paging );
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
                model = service.ListCSM( pm, csm );
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
                model = service.ListCSM( pm, csm );
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
            int total = 0;

            List<DeclineReason> model = new List<DeclineReason>();

            using ( DeclineReasonService service = new DeclineReasonService() )
            {
                model = service.List( pm, csm );
                total = ( model.Count < pm.Take && pm.Skip == 0 ) ? model.Count : service.Total( pm, csm );
            }

            PagingExtension paging = PagingExtension.Create( ( object ) model, total, pm.Skip, pm.Take, pm.Page );

            return PartialView( "_DeclineReasons", paging );
        }

        #endregion

        #region Manage Transporters
        //
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
            int pspId = (CurrentUser != null ? CurrentUser.PSPs.FirstOrDefault().Id : 0);
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
            int pspId = (CurrentUser != null ? CurrentUser.PSPs.FirstOrDefault().Id : 0);



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
    }
}
