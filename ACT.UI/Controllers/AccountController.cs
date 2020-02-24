using System;
using System.Web;
using System.Linq;
using System.Web.Mvc;
using ACT.UI.Models;
using ACT.Core.Services;
using System.Web.Security;
using ACT.Core.Models;
using ACT.Core.Enums;
using ACT.Data.Models;
using System.Transactions;
using System.IO;

namespace ACT.UI.Controllers
{
    public class AccountController : BaseController
    {
        //
        // GET: /Account/Index
        public ActionResult Index()
        {
            if ( Request.IsAuthenticated )
            {
                return DoLogin( CurrentUser.RoleType );
            }

            return RedirectToAction( "Login" );
        }

        //
        // GET: /Account/Login
        public ActionResult Login( string returnUrl = "" )
        {
            if ( Request.IsAuthenticated )
            {
                Notify( "You're already logged in, what would you like to do next? Please contact us if you require any help.", NotificationType.Warn );

                return DoLogin( CurrentUser.RoleType );
            }

            LoginViewModel model = new LoginViewModel() { ReturnUrl = returnUrl };

            return View( model );
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login( LoginViewModel model, string returnUrl )
        {
            // Model valid?
            if ( !ModelState.IsValid )
            {
                // If we got this far, something failed, redisplay form
                Notify( "The supplied information is invalid. Please try again.", NotificationType.Error );

                return View( model );
            }

            UserModel user;

            using ( UserService uservice = new UserService() )
            using ( AuditLogService aservice = new AuditLogService() )
            {
                user = uservice.Login( model.UserName, model.Password );

                // User valid?
                if ( user == null )
                {
                    // If we got this far, something failed, redisplay form
                    Notify( "The user name or password provided is incorrect.", NotificationType.Error );

                    return View( model );
                }

                CustomExpiration( user, 3600 );

                User u = uservice.GetById( user.Id );

                u.LastLogin = DateTime.Now;

                uservice.Update( u );

                aservice.Create( ActivityTypes.Login, user, null, user.Id );
            }

            if ( !string.IsNullOrEmpty( returnUrl ) )
            {
                Redirect( returnUrl );
            }

            return DoLogin( user.RoleType );
        }

        public ActionResult DoLogin( RoleType roleType )
        {
            switch ( roleType )
            {
                case RoleType.Client:

                    return RedirectToAction( "Index", "Client" );

                case RoleType.PSP:
                case RoleType.SuperAdmin:

                    return RedirectToAction( "Index", "Administration" );
            }

            return LogOff();
        }

        //
        // GET: /Account/ForgotPassword
        public ActionResult ForgotPassword( string returnUrl )
        {
            if ( Request.IsAuthenticated )
            {
                Notify( "You're already logged in, what would you like to do next? Please contact us if you require any help.", NotificationType.Warn );

                return DoLogin( CurrentUser.RoleType );
            }

            ForgotPasswordViewModel model = new ForgotPasswordViewModel() { ReturnUrl = returnUrl };

            return View( model );
        }

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ForgotPassword( ForgotPasswordViewModel model, string returnUrl = "" )
        {
            if ( !ModelState.IsValid )
            {
                Notify( "The supplied information is invalid. Please try again.", NotificationType.Error );

                return View( model );
            }

            using ( UserService uservice = new UserService() )
            using ( TokenService tservice = new TokenService() )
            {
                User user = uservice.GetByEmail( model.Email );

                if ( user == null )
                {
                    Notify( "The e-mail address provided is incorrect.", NotificationType.Error );

                    return View( model );
                }

                Token t = new Token()
                {
                    UserId = user.Id,
                    UID = Guid.NewGuid(),
                    Status = ( int ) Status.Active
                };

                t = tservice.Create( t );

                user = uservice.Update( user );

                bool sent = SendResetPassword( t.UID, user, returnUrl );

                if ( sent )
                {
                    Notify( "A Password Reset Request has been sent to your Email Address. Follow the instructions in the email to continue.", NotificationType.Success );
                }
                else
                {
                    Notify( "Sorry, a Password Reset Request could not be sent. Please try again later.", NotificationType.Warn );
                }
            }

            return View( model );
        }

        //
        // GET: /Account/Register
        public ActionResult Register()
        {
            if ( Request.IsAuthenticated )
            {
                return RedirectToAction( "Index", "DashBoard" );
            }

            RegisterViewModel model = new RegisterViewModel();

            return View( model );
        }

        //
        // GET: /Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register( RegisterViewModel model )
        {
            PSP psp;
            User user;
            Client client;

            int objectId;
            string objectType;

            using ( PSPService pservice = new PSPService() )
            using ( UserService uservice = new UserService() )
            using ( RoleService rservice = new RoleService() )
            using ( ClientService cservice = new ClientService() )
            using ( AddressService aservice = new AddressService() )
            using ( TransactionScope scope = new TransactionScope() )
            using ( DocumentService dservice = new DocumentService() )
            using ( EstimatedLoadService eservice = new EstimatedLoadService() )
            {
                model.PSPOptions = pservice.List();

                if ( !ModelState.IsValid )
                {
                    Notify( "The supplied information is invalid. Please try again.", NotificationType.Error );

                    return View( model );
                }

                bool isClient = ( model.ServiceType == ServiceType.ManageOwnPallets || model.ServiceType == ServiceType.ProvidePalletManagement );

                #region Validations

                // Check company does not exist by Company Name & Registration Number
                if ( isClient && !string.IsNullOrEmpty( model.CompanyRegistrationNumber ) && cservice.ExistByCompanyRegistrationNumber( model.CompanyRegistrationNumber.Trim() ) )
                {
                    Notify( $"A Client with that Company Registration Number already exists. Contact us if you require further assistance.", NotificationType.Error );

                    return View( model );
                }
                else if ( !isClient && !string.IsNullOrEmpty( model.CompanyRegistrationNumber ) && pservice.ExistByCompanyRegistrationNumber( model.CompanyRegistrationNumber.Trim() ) )
                {
                    Notify( $"A Pooling Service Provider (PSP) with that Company Registration Number already exists. Contact us if you require further assistance.", NotificationType.Error );

                    return View( model );
                }

                #endregion

                RoleType roleType = ( isClient ) ? RoleType.Client : RoleType.PSP;

                user = new User()
                {
                    Type = ( int ) roleType,
                    Name = model.CompanyName,
                    Email = model.AdminEmail,
                    Cell = model.ContactNumber,
                    Surname = model.CompanyName,
                    Status = ( int ) Status.Pending,
                };

                // Get Role
                Role role = rservice.GetByName( roleType.GetStringValue() );

                if ( isClient )
                {
                    #region Create Client

                    client = new Client()
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
                        Status = ( int ) PSPClientStatus.Unverified,
                        ServiceRequired = ( int ) model.ServiceType,
                        CompanyRegistrationNumber = model.CompanyRegistrationNumber,

                    };

                    if ( model.PSPId > 0 )
                    {
                        client.PSPClients.Add( new PSPClient()
                        {
                            PSPId = model.PSPId,
                            ClientId = client.Id,
                            CreatedOn = DateTime.Now,
                            ModifiedOn = DateTime.Now,
                            ModifiedBy = model.AdminEmail,
                            Status = ( int ) Status.Active,
                        } );
                    }

                    client = cservice.Create( client );

                    user.ClientUsers.Add( new ClientUser()
                    {
                        UserId = user.Id,
                        ClientId = client.Id,
                        CreatedOn = DateTime.Now,
                        ModifiedOn = DateTime.Now,
                        ModifiedBy = model.AdminEmail,
                        Status = ( int ) Status.Active,

                    } );

                    objectId = client.Id;
                    objectType = "Client";

                    #endregion
                }
                else
                {
                    #region Create PSP

                    psp = new PSP()
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
                        Status = ( int ) PSPClientStatus.Unverified,
                        ServiceRequired = ( int ) model.ServiceType,
                        CompanyRegistrationNumber = model.CompanyRegistrationNumber,

                    };

                    psp = pservice.Create( psp );

                    user.PSPUsers.Add( new PSPUser()
                    {
                        PSPId = psp.Id,
                        UserId = user.Id,
                        CreatedOn = DateTime.Now,
                        ModifiedOn = DateTime.Now,
                        ModifiedBy = model.AdminEmail,
                        Status = ( int ) Status.Active,
                    } );

                    objectId = psp.Id;
                    objectType = "PSP";

                    #endregion
                }

                model.Id = objectId;

                #region Create User

                user = uservice.Create( user, role.Id );

                #endregion


                #region Create Estimated Load

                if ( model.EstimatedLoad != null )
                {
                    EstimatedLoad load = new EstimatedLoad()
                    {
                        ObjectId = objectId,
                        ObjectType = objectType,
                        January = model.EstimatedLoad.January,
                        February = model.EstimatedLoad.February,
                        March = model.EstimatedLoad.March,
                        April = model.EstimatedLoad.April,
                        May = model.EstimatedLoad.May,
                        June = model.EstimatedLoad.June,
                        July = model.EstimatedLoad.July,
                        August = model.EstimatedLoad.August,
                        September = model.EstimatedLoad.September,
                        October = model.EstimatedLoad.October,
                        November = model.EstimatedLoad.November,
                        December = model.EstimatedLoad.December,

                    };

                    load = eservice.Create( load );
                }

                #endregion


                #region Create Address (s)

                if ( model.Address != null )
                {
                    Address address = new Address()
                    {
                        ObjectId = objectId,
                        ObjectType = objectType,
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

                if ( model.RegistrationFile.File != null )
                {
                    // Create folder
                    string path = Server.MapPath( $"~/{VariableExtension.SystemRules.DocumentsLocation}/{objectType}/{model.CompanyName.Trim()}-{model.CompanyRegistrationNumber.Trim().Replace( "/", "_" ).Replace( "\\", "_" )}/" );

                    if ( !Directory.Exists( path ) )
                    {
                        Directory.CreateDirectory( path );
                    }

                    string now = DateTime.Now.ToString( "yyyyMMddHHmmss" );

                    Document doc = new Document()
                    {
                        ObjectId = objectId,
                        ObjectType = objectType,
                        Status = ( int ) Status.Active,
                        Name = model.RegistrationFile.Name,
                        Category = model.RegistrationFile.Name,
                        Title = model.RegistrationFile.File.FileName,
                        Size = model.RegistrationFile.File.ContentLength,
                        Description = model.RegistrationFile.Description,
                        Type = Path.GetExtension( model.RegistrationFile.File.FileName ),
                        Location = $"{objectType}/{model.CompanyName.Trim()}-{model.CompanyRegistrationNumber.Trim().Replace( "/", "_" ).Replace( "\\", "_" )}/{now}-{model.RegistrationFile.File.FileName}"
                    };

                    dservice.Create( doc );

                    string fullpath = Path.Combine( path, $"{now}-{model.RegistrationFile.File.FileName}" );
                    model.RegistrationFile.File.SaveAs( fullpath );
                }

                #endregion

                // Complete the scope
                scope.Complete();
            }

            // Send Welcome Email
            bool sent = SendUserWelcome( model );

            if ( sent )
            {
                SendUserWelcome1( model );
            }

            Notify( "Thank you for your application to register, you will receive an email with further instructions shortly. Please check your spam folder if you do not receive mail within 24 hours.", NotificationType.Success );

            return RedirectToAction( "Login" );
        }

        //
        // GET: /Account/ResetPassword
        public ActionResult ResetPassword( Guid uid, string returnUrl = "" )
        {
            if ( Request.IsAuthenticated )
            {
                Notify( "You're already logged in, what would you like to do next? Please contact us if you require any help.", NotificationType.Warn );

                return DoLogin( CurrentUser.RoleType );
            }

            // Token is available?
            if ( uid == null )
            {
                Notify( "The supplied information is invalid. Please try again.", NotificationType.Error );

                return RedirectToAction( "Login" );
            }

            using ( TokenService service = new TokenService() )
            {
                // Token is valid?
                if ( !service.Exist( uid, DateTime.Now ) )
                {
                    Notify( "The supplied token is no longer valid. Please try again.", NotificationType.Error );

                    return RedirectToAction( "Login" );
                }
            }

            ResetPasswordViewModel model = new ResetPasswordViewModel() { UID = uid, ReturnUrl = returnUrl };

            return View( model );
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ResetPassword( ResetPasswordViewModel model, string returnUrl = "" )
        {
            if ( !ModelState.IsValid )
            {
                Notify( "The supplied information is invalid. Please try again.", NotificationType.Error );

                return ResetPassword( model.UID, returnUrl );
            }

            // Token is available?
            if ( model?.UID == null )
            {
                Notify( "The supplied information is invalid. Please try again.", NotificationType.Error );

                return ResetPassword( model.UID, returnUrl );
            }

            User user;

            using ( UserService uservice = new UserService() )
            using ( TokenService tservice = new TokenService() )
            using ( TransactionScope scope = new TransactionScope() )
            {
                // Token is valid?
                if ( !tservice.Exist( model.UID, DateTime.Now ) )
                {
                    Notify( "The supplied token is no longer valid. Please try again.", NotificationType.Error );

                    return ResetPassword( model.UID, returnUrl );
                }

                // Password mismatch?
                if ( !string.Equals( model.Password, model.ConfirmPassword, StringComparison.CurrentCulture ) )
                {
                    Notify( "Password combination does not match. Please try again.", NotificationType.Error );

                    return View( model );
                }

                Token t = tservice.GetByUid( model.UID );
                user = uservice.GetById( t.UserId );

                t.Status = ( int ) Status.Inactive;

                //user.Reset = false;
                user.PasswordDate = DateTime.Now;
                user.Password = uservice.GetSha1Md5String( model.Password );

                tservice.Update( t );
                uservice.Update( user );

                scope.Complete();
            }

            Notify( "Your password was successfully updated. You can proceed and login below.", NotificationType.Success );

            return Login( new LoginViewModel() { Password = model.Password, ReturnUrl = returnUrl, UserName = user.Email }, returnUrl );
        }

        /// <summary>
        /// Simply creates a Custom Authentication
        /// </summary>
        /// <param name="model"></param>
        /// <param name="expires"></param>
        private void CustomExpiration( UserModel model, int expires )
        {
            FormsAuthenticationTicket ticket = new FormsAuthenticationTicket( model.Email, true, ( expires / 60 ) );

            // Encrypt the ticket.
            string encTicket = FormsAuthentication.Encrypt( ticket );

            // Create the cookie.
            Response.Cookies.Add( new HttpCookie( FormsAuthentication.FormsCookieName, encTicket ) );
        }

        //
        // GET: /Account/LogOff
        public ActionResult LogOff()
        {
            using ( AuditLogService aservice = new AuditLogService() )
            {
                aservice.Create( ActivityTypes.Logout, CurrentUser, null, CurrentUser?.Id ?? 0 );
            }

            FormsAuthentication.SignOut();

            this.HttpContext.Cache.Remove( User.Identity.Name );

            ContextExtensions.RemoveCachedUserData();

            return RedirectToAction( "Login" );
        }

        public ActionResult PartialLogOff()
        {
            FormsAuthentication.SignOut();

            this.HttpContext.Cache.Remove( User.Identity.Name );

            ContextExtensions.RemoveCachedUserData();

            return PartialView( "_Empty" );
        }

        #region Helpers

        private ActionResult RedirectToLocal( string returnUrl )
        {
            if ( Url.IsLocalUrl( returnUrl ) || IsSubDomain( returnUrl ) )
            {
                return Redirect( returnUrl );
            }
            else
            {
                return DoLogin( CurrentUser.RoleType );
            }
        }

        private bool IsSubDomain( string url )
        {
            if ( string.IsNullOrEmpty( url ) )
                return false;


            Uri.TryCreate( url, UriKind.Absolute, out Uri absoluteUri );

            return absoluteUri.Host.EndsWith( Request.Url.Host.Replace( "www.", "" ) );
        }

        private static string ErrorCodeToString( MembershipCreateStatus createStatus )
        {
            // See http://go.microsoft.com/fwlink/?LinkID=177550 for
            // a full list of status codes.
            switch ( createStatus )
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "User name already exists. Please enter a different user name.";

                case MembershipCreateStatus.DuplicateEmail:
                    return "A user name for that e-mail address already exists. Please enter a different e-mail address.";

                case MembershipCreateStatus.InvalidPassword:
                    return "The password provided is invalid. Please enter a valid password value.";

                case MembershipCreateStatus.InvalidEmail:
                    return "The e-mail address provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidAnswer:
                    return "The password retrieval answer provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidQuestion:
                    return "The password retrieval question provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidUserName:
                    return "The user name provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.ProviderError:
                    return "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                case MembershipCreateStatus.UserRejected:
                    return "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                default:
                    return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
            }
        }

        #endregion
    }
}
