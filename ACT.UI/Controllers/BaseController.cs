﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using ACT.Core.Enums;
using ACT.Core.Services;
using ACT.Data.Models;
using System.Web.Script.Serialization;
using ACT.Core.Models;
using ACT.Core.Helpers;
using System.Text.RegularExpressions;
using ACT.Core.Extension;
using System.Net;
using ACT.Mailer;
using ACT.UI.Models;
using ACT.Core.Models.Custom;

namespace ACT.UI.Controllers
{
    public class BaseController : Controller
    {
        private string _currentController = null;

        public List<NotificationModel> Notifications = new List<NotificationModel>();

        /// <summary>
        /// Gets the name of the current controller instance.
        /// </summary>
        public string CurrentController
        {
            get
            {
                if ( _currentController == null )
                {
                    _currentController = ControllerContext.RouteData.GetRequiredString( "controller" );
                }
                return _currentController;
            }
        }

        private string _currentAction = null;

        /// <summary>
        /// SA ID Regex
        /// </summary>
        public static Regex SAIDRegex
        {
            get
            {
                return new Regex( @"^(((\d{2}((0[13578]|1[02])(0[1-9]|[12]\d|3[01])|(0[13456789]|1[012])(0[1-9]|[12]\d|30)|02(0[1-9]|1\d|2[0-8])))|([02468][048]|[13579][26])0229))(( |-)(\d{4})( |-)(\d{3})|(\d{7}))" );
            }
        }

        /// <summary>
        /// Gets the name of the current Action being executed.
        /// </summary>
        public string CurrentAction
        {
            get
            {
                if ( _currentAction == null )
                {
                    _currentAction = ControllerContext.RouteData.GetRequiredString( "action" );
                }
                return _currentAction;
            }
        }

        public BaseController()
        {
            ConfigSettings.SetRules();
            VariableExtension.SetRules();

            ViewBag.SystemRules = ConfigSettings.SystemRules;
        }

        public string CurrentUrl
        {
            get
            {
                return Request.Url.Scheme + "://" + Request.Url.Host + ( Request.Url.IsDefaultPort ? "" : ":" + Request.Url.Port );
            }
        }

        /// <summary>
        /// Initialized the CurrentUser property.
        /// </summary>
        protected virtual void InitCurrentUser()
        {
            if ( Request.IsAuthenticated )
            {
                this.CurrentUser = this.GetUser( User.Identity.Name );
            }
        }

        /// <summary>
        /// Gets a User instance for the specified username.
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        protected virtual UserModel GetUser( string email )
        {
            using ( UserService service = new UserService() )
            {
                UserModel user = service.GetUser( email );

                return user;
            }
        }

        private UserModel _currentUser = null;

        /// <summary>
        /// The current User. Null if the Request is not authenticated.
        /// </summary>
        public virtual UserModel CurrentUser
        {
            get
            {
                if ( _currentUser == null && Request.IsAuthenticated )
                {
                    try
                    {
                        this.InitCurrentUser();
                    }
                    catch ( Exception ex )
                    {

                    }
                }

                ViewBag.CurrentUser = _currentUser;

                return _currentUser;
            }
            set
            {
                _currentUser = value;
            }
        }

        public Dictionary<string, List<string>> FileTypes
        {
            get
            {
                Dictionary<string, List<string>> fileTypes = new Dictionary<string, List<string>>
                {
                    { "PDF", new List<string>() { { ".pdf" } } },
                    { "CSV", new List<string>() { { ".csv" } } },
                    { "Word", new List<string>() { { ".doc" }, { ".docx" } } },
                    { "Excel", new List<string>() { { ".xls" }, { ".xlsx" } } },
                    { "Power Point", new List<string>() { { ".ppt" }, { ".pptx" } } },
                    { "Video", new List<string>() { { ".mpg" }, { ".mp4" }, { ".avi" }, { ".flv" }, { ".mkv" }, { ".wmv" } } }
                };

                return fileTypes;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="filterContext"></param>
        public void HandleUnauthorizedUserRequest( AuthorizationContext filterContext )
        {
            if ( Request.IsAjaxRequest() )
            {
                filterContext.Result = PartialView( "_AccessExpired" );
            }
            else
            {
                // Check which controller failed and try the next one
                // If all 3 fails, then log user out.
                if ( Request.IsAuthenticated )
                {
                    switch ( CurrentController )
                    {
                        case "DashBoard":

                            filterContext.Result = RedirectToAction( "Index", "Campaign" );

                            break;

                        case "Campaign":

                            filterContext.Result = RedirectToAction( "Index", "Dinner" );

                            break;

                        case "Dinner":

                            filterContext.Result = RedirectToAction( "Index", "TradingPartner" );

                            break;

                        case "TradingPartner":

                            filterContext.Result = RedirectToAction( "Index", "Member" );

                            break;

                        case "Member":

                            filterContext.Result = RedirectToAction( "Index", "Client" );

                            break;

                        case "Client":

                            filterContext.Result = RedirectToAction( "Index", "Reward" );

                            break;

                        case "Reward":

                            filterContext.Result = RedirectToAction( "Index", "Administration" );

                            break;

                        case "Report":
                        default:

                            Notifications.Add( new NotificationModel() { Message = "Please note, you were signed out from your previous session due to time-out", Type = NotificationType.Error } );

                            Session[ "Notifications" ] = Notifications;

                            filterContext.Result = RedirectToAction( "LogOff", "Account", new
                            {
                                @returnUrl = Request.RawUrl
                            } );

                            break;
                    }
                }
                else
                {
                    Notifications.Add( new NotificationModel() { Message = "Please note, you were signed out from your previous session due to time-out", Type = NotificationType.Error } );

                    Session[ "Notifications" ] = Notifications;

                    filterContext.Result = RedirectToAction( "LogOff", "Account", new
                    {
                        @returnUrl = Request.RawUrl
                    } );
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filterContext"></param>
        public void HandleUnauthorizedBrowserRequest( AuthorizationContext filterContext )
        {
            //NB: This error message must not indicate to the Client why the access was denied!
            filterContext.Result = PartialView( "_AccessExpired" );
        }

        /// <summary>
        /// Will use the Controller name to guess the appropriate PermissionContext.
        /// Override to customize.
        /// </summary>
        public virtual PermissionContext DefaultPermissionContext
        {
            get
            {
                return EnumHelper.Parse<PermissionContext>( this.GetName() );
            }
        }

        /// <summary>
        /// Renders a view or partial view and returns it as a string.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="viewPath"></param>
        /// <param name="model"></param>
        /// <param name="partial"></param>
        /// <returns></returns>
        public static string RenderViewToString( ControllerContext context, string viewPath, object model = null, bool partial = false )
        {
            // First find the ViewEngine for this view
            ViewEngineResult viewEngineResult;
            if ( partial )
                viewEngineResult = ViewEngines.Engines.FindPartialView( context, viewPath );
            else
                viewEngineResult = ViewEngines.Engines.FindView( context, viewPath, null );

            if ( viewEngineResult == null )
                throw new FileNotFoundException( "View cannot be found." );

            // get the view and attach the model to view data
            var view = viewEngineResult.View;
            context.Controller.ViewData.Model = model;

            string result = null;

            using ( var sw = new StringWriter() )
            {
                var ctx = new ViewContext( context, view, context.Controller.ViewData, context.Controller.TempData, sw );

                view.Render( ctx, sw );
                result = sw.ToString();
            }


            return result;
        }

        /// <summary>
        /// Adds a notification to the list
        /// </summary>
        /// <param name="error"></param>
        /// <param name="type"></param>
        public void Notify( string error, NotificationType type )
        {
            Notifications.Add( new NotificationModel() { Message = error, Type = type } );

            Session[ "Notifications" ] = Notifications;
        }


        public bool CanView()
        {


            return true;
        }

        /// <summary>
        /// Checks if the supplied bank details are valid using the external CDV API.
        /// </summary>
        /// <param name="accountNo"></param>
        /// <param name="branchCode"></param>
        /// <param name="accountType"></param>
        /// <returns></returns>
        public CDVResult ValidateBankDetails( string accountNo, string branchCode, int accountType )
        {
            CDVResult res = new CDVResult();

            try
            {
                int accType = 0;

                // Port Account Type => CDV AccountType
                switch ( accountType )
                {
                    case 1: // Savings
                        accType = 2;
                        break;

                    case 2: // Current/Cheque
                        accType = 1;
                        break;

                    case 3: // Transmission
                        accType = 3;
                        break;

                    case 4: // Credit Card
                    case 5: // Bill Payment
                        return res;

                    default:
                        return new CDVResult() { Code = -1, Message = "Unknown Account Type" };
                }


                string url = string.Format( "http://localhost:5000/api/v1/cdv/verifyaccount?branchcode={0}&accountno={1}&accounttype={2}",
                                             branchCode, accountNo, accType );

                string response = Get( url, "application/json" );

                res = new JavaScriptSerializer().Deserialize<CDVResult>( response );
            }
            catch ( Exception ex )
            {

            }

            return res;
        }



        #region Common Action Result

        public ActionResult StayAlive()
        {
            return PartialView( "_Empty" );
        }

        /// <summary>
        /// Performs a GET request to the specified URL
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public string Get( string url, string contentType = "" )
        {
            string responseString;
            try
            {
                WebRequest request = ( HttpWebRequest ) WebRequest.Create( url );

                if ( !string.IsNullOrEmpty( contentType ) )
                {
                    request.ContentType = "application/json";
                }

                WebResponse response = ( HttpWebResponse ) request.GetResponse();

                responseString = new StreamReader( response.GetResponseStream() ).ReadToEnd();
            }
            catch ( Exception ex )
            {
                responseString = "{ \"Code\": \"-1\", \"Message\": \"" + ex.Message + "\"}";
            }

            return responseString;
        }

        /// <summary>
        /// Validates the supplied bank details
        /// </summary>
        /// <param name="accountNo"></param>
        /// <param name="branchCode"></param>
        /// <param name="accountType"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult IsValidBankDetails( string accountNo, string branchCode, int accountType )
        {
            CDVResult res = ValidateBankDetails( accountNo.Trim(), branchCode.Trim(), accountType );

            return new JsonResult()
            {
                Data = new
                {
                    Code = ( res == null ) ? -1 : res.Code,
                    Message = ( res == null ) ? "Validation Engine Unavailable: Please consult your Systems Administrator for further assistance." : string.Format( "<p>{0}</p><p>{1}</p>", res.Message, "Provided bank details might not be valid. Please make corrections and try again..." )
                }
            };
        }

        /// <summary>
        /// Get's a total for the specified control and/or action/tab
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="tab"></param>
        /// <returns></returns>
        public JsonResult HotSpot( string controller, string tab = "" )
        {
            int total = 0;



            return new JsonResult() { Data = new { total = total } };
        }

        /// <summary>
        /// Gets a bank using the specified bankId and returns a JSON representation. More fields can be added when need be
        /// </summary>
        /// <param name="bankId"></param>
        /// <returns></returns>
        public JsonResult GetBank( int bankId = 0 )
        {
            Bank bank;

            using ( BankService service = new BankService() )
            {
                bank = service.GetById( bankId );
            }

            return new JsonResult()
            {
                Data = new
                {
                    Id = bank.Id,
                    Name = bank.Name,
                    Code = bank.Code,
                    Description = bank.Description
                }
            };
        }

        /// <summary>
        /// Gets a client using the specified clientId and returns a JSON representation. More fields can be added when need be
        /// </summary>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public JsonResult GetClient( int clientId = 0 )
        {
            Client client;

            string logoPath = "~/Images/no-preview.png";

            using ( ClientService service = new ClientService() )
            {
                client = service.GetById( clientId );
            }

            return new JsonResult()
            {
                Data = new
                {
                    Id = client.Id,
                    Logo = logoPath
                }
            };
        }

        //
        // GET: /Authorisation/GetBroadcast
        public JsonResult GetBroadcast()
        {
            if ( !Request.IsAuthenticated )
            {
                return new JsonResult() { Data = new { Found = 0, } };
            }

            Broadcast b;

            int found = 1;

            using ( BroadcastService service = new BroadcastService() )
            {
                b = service.GetUnreadByUser( CurrentUser?.Id ?? 0 );
            }

            if ( b == null )
            {
                found = 0;
                b = new Broadcast();
            }

            return new JsonResult() { Data = new { Found = found, Bid = b.Id, b.Message, b.StartDate, b.EndDate } };
        }

        //
        // POST: /Authorisation/AddUserBroadcast/5
        [HttpPost]
        public ActionResult AddUserBroadcast( int id )
        {
            using ( UserBroadcastService service = new UserBroadcastService() )
            {
                UserBroadcast ub = new UserBroadcast()
                {
                    BroadcastId = id,
                    UserId = CurrentUser.Id
                };

                service.Create( ub );
            }

            return PartialView( "_Empty" );
        }

        public ActionResult PopAuditLog( int id, string type )
        {
            List<AuditLogCustomModel> logs;

            using ( AuditLogService aservice = new AuditLogService() )
            {
                logs = aservice.List( new PagingModel() { Take = int.MaxValue, Skip = 0 }, new CustomSearchModel() { ObjectId = id, TableName = type } );
            }

            return PartialView( "_PopAuditLog", logs );
        }

        public ActionResult PopAuditLogDetails( int id, bool layout = true )
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

            return PartialView( "_PopAuditLogDetails", model );
        }

        //
        // GET: /Document/ViewDocument/5
        public ActionResult ViewDocument( int id )
        {
            using ( DocumentService service = new DocumentService() )
            using ( UserDocumentService uservice = new UserDocumentService() )
            {
                Document d = service.GetById( id );

                if ( d == null )
                    return PartialView( "_AccessDenied" );

                try
                {
                    uservice.Create( new UserDocument()
                    {
                        ActionType = ( int ) ActivityTypes.Download,
                        DocumentId = id,
                        UserId = CurrentUser.Id
                    } );
                }
                catch
                {

                }

                string path = Server.MapPath( string.Format( "{0}/{1}", VariableExtension.SystemRules.DocumentsLocation, d.Location ) );

                if ( !System.IO.File.Exists( path ) && !string.IsNullOrEmpty( d.Url ) )
                {
                    return Redirect( d.Url );
                }

                return File( path, System.Web.MimeMapping.GetMimeMapping( path ) );
            }
        }

        //
        // GET: /Document/DownloadDocument/5
        public ActionResult DownloadDocument( int id )
        {
            using ( DocumentService service = new DocumentService() )
            using ( UserDocumentService uservice = new UserDocumentService() )
            {
                Document d = service.GetById( id );

                if ( d == null )
                    return PartialView( "_AccessDenied" );

                try
                {
                    uservice.Create( new UserDocument()
                    {
                        ActionType = ( int ) ActivityTypes.Download,
                        DocumentId = id,
                        UserId = CurrentUser.Id
                    } );
                }
                catch
                {

                }

                string path = Server.MapPath( string.Format( "{0}/{1}", VariableExtension.SystemRules.DocumentsLocation, d.Location ) );

                if ( !System.IO.File.Exists( path ) && !string.IsNullOrEmpty( d.Url ) )
                {
                    return Redirect( d.Url );
                }

                return File( path, System.Web.MimeMapping.GetMimeMapping( path ), Path.GetFileName( path ) );
            }
        }

        #endregion



        #region Emails

        /// <summary>
        /// Sends a Reset Password email to the requesting user 
        /// </summary>
        /// <param name="token"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public bool SendResetPassword( Guid token, User user, string returnUrl = "" )
        {
            ViewBag.Token = token;

            ViewBag.ReturnUrl = returnUrl;

            string body = RenderViewToString( ControllerContext, "~/Views/Email/_ResetPassword.cshtml", user, true );

            EmailModel email = new EmailModel()
            {
                Body = body,
                Subject = "ACT - Reset Password",
                From = ConfigSettings.SystemRules.SystemContactEmail,
                Recipients = new List<string>() { user.Email }
            };

            return Mail.Send( email );
        }

        /// <summary>
        /// Sends a Welcome email to the requesting user
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public bool SendUserWelcome( RegisterViewModel user )
        {
            if ( user == null )
            {
                return false;
            }

            string body = RenderViewToString( ControllerContext, "~/Views/Email/_UserWelcome.cshtml", user, true );

            EmailModel email = new EmailModel()
            {
                Body = body,
                Subject = "ACT Pallet Solutions - Welcome",
                From = ConfigSettings.SystemRules.SystemContactEmail,
                Recipients = new List<string>() { user.AdminEmail, user.EmailAddress }
            };

            return Mail.Send( email );
        }

        /// <summary>
        /// Sends a Welcome email to the requesting user
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool SendUserWelcome1( RegisterViewModel model )
        {
            if ( model == null )
            {
                return false;
            }

            bool isClient = ( model.ServiceType == ServiceType.ManageOwnPallets || model.ServiceType == ServiceType.ProvidePalletManagement );

            string str = isClient ? "Client" : "PSP";

            string body = RenderViewToString( ControllerContext, "~/Views/Email/_UserWelcome1.cshtml", model, true );

            EmailModel email = new EmailModel()
            {
                Body = body,
                Subject = $"ACT Pallet Solutions - NEW {str} Register RREQUEST",
                From = ConfigSettings.SystemRules.SystemContactEmail,
                Recipients = new List<string>() { ConfigSettings.SystemRules.SystemContactEmail }
            };

            return Mail.Send( email );
        }

        /// <summary>
        /// Sends a Welcome email to the requesting user
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public bool SendUserApproved( UserViewModel user )
        {
            if ( user == null )
            {
                return false;
            }

            string body = RenderViewToString( ControllerContext, "~/Views/Email/_UserApproved.cshtml", user, true );

            EmailModel email = new EmailModel()
            {
                Body = body,
                Subject = "ACT Pallet Solutions - Signup Request Approved",
                From = ConfigSettings.SystemRules.SystemContactEmail,
                Recipients = new List<string>() { user.Email }
            };

            return Mail.Send( email );
        }

        /// <summary>
        /// Sends a Welcome email to the requesting user
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public bool SendUserDecline( UserViewModel user )
        {
            if ( user == null )
            {
                return false;
            }

            string body = RenderViewToString( ControllerContext, "~/Views/Email/_UserDecline.cshtml", user, true );

            EmailModel email = new EmailModel()
            {
                Body = body,
                Subject = "ACT Pallet Solutions - Signup Request Declined",
                From = ConfigSettings.SystemRules.SystemContactEmail,
                Recipients = new List<string>() { user.Email }
            };

            return Mail.Send( email );
        }

        #endregion

    }
}
