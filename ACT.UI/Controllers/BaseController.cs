using System;
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
using System.Web;

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

                            filterContext.Result = RedirectToAction( "Index", "Client" );

                            break;

                        case "Client":

                            filterContext.Result = RedirectToAction( "Index", "ClientReporting" );

                            break;

                        case "ClientReporting":

                            filterContext.Result = RedirectToAction( "Index", "Pallet" );

                            break;

                        case "Pallet":

                            filterContext.Result = RedirectToAction( "Index", "Administration" );

                            break;

                        case "Administration":

                            filterContext.Result = RedirectToAction( "Index", "Finance" );

                            break;

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
        /// Gets a ChepLoad using the specified bankId and returns a JSON representation. More fields can be added when need be
        /// </summary>
        /// <param name="bankId"></param>
        /// <returns></returns>
        public JsonResult GetChepLoad( int chepLoadId = 0 )
        {
            using ( ChepLoadService service = new ChepLoadService() )
            {
                ChepLoad chepLoad = service.GetById( chepLoadId );

                return new JsonResult()
                {
                    Data = new
                    {
                        chepLoad.Id,
                        chepLoad.Equipment,
                        chepLoad.DocketNumber,
                        chepLoad.OriginalDocketNumber,
                        chepLoad.Ref,
                        chepLoad.OtherRef,
                        chepLoad.OtherParty,
                        chepLoad.EquipmentCode,
                        chepLoad.Location,
                        chepLoad.LocationId,
                        chepLoad.TransactionType,
                        chepLoad.DataSource,
                        EffectiveDate = chepLoad.EffectiveDate?.ToString( "yyyy/MM/dd" ),
                        ShipmentDate = chepLoad.ShipmentDate?.ToString( "yyyy/MM/dd" ),
                        DeliveryDate = chepLoad.DeliveryDate?.ToString( "yyyy/MM/dd" ),
                    }
                };
            }
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

        //
        // GET: /Image/ViewImage/5
        public ActionResult ViewImage( int id )
        {
            using ( ImageService service = new ImageService() )
            {
                Image i = service.GetById( id );

                if ( i == null )
                    return PartialView( "_AccessDenied" );

                string path = Server.MapPath( string.Format( "{0}/{1}", VariableExtension.SystemRules.ImagesLocation, i.Location ) );

                return File( path, System.Web.MimeMapping.GetMimeMapping( path ) );
            }
        }

        //
        // GET: /Image/DownloadImage/5
        public ActionResult DownloadImage( int id )
        {
            using ( ImageService service = new ImageService() )
            {
                Image d = service.GetById( id );

                if ( d == null )
                    return PartialView( "_AccessDenied" );

                string path = Server.MapPath( string.Format( "{0}/{1}", VariableExtension.SystemRules.ImagesLocation, d.Location ) );

                return File( path, System.Web.MimeMapping.GetMimeMapping( path ), Path.GetFileName( path ) );
            }
        }

        //
        // POST: /Document/DeleteImage/5
        public ActionResult DeleteImage( int id )
        {
            using ( ImageService service = new ImageService() )
            {
                Image i = service.GetById( id );

                if ( i == null )
                    return PartialView( "_AccessDenied" );

                string path = Server.MapPath( string.Format( "{0}/{1}", VariableExtension.SystemRules.ImagesLocation, i.Location ) );
                string folder = Path.GetDirectoryName( path );

                service.Delete( i );

                if ( System.IO.File.Exists( path ) )
                {
                    System.IO.File.Delete( path );
                }

                if ( Directory.Exists( folder ) && Directory.GetFiles( folder )?.Length <= 0 )
                {
                    Directory.Delete( folder );
                }
            }

            return PartialView( "_Empty" );
        }

        //
        // Returns a general list of all active clients allowed in the current context to be selected from
        [AcceptVerbs( HttpVerbs.Get | HttpVerbs.Post )]
        public JsonResult GetClientList()
        {

            List<ClientCustomModel> model = new List<ClientCustomModel>();
            PagingModel pm = new PagingModel();
            CustomSearchModel csm = new CustomSearchModel();

            using ( ClientService service = new ClientService() )
            {
                pm.Sort = pm.Sort ?? "ASC";
                pm.SortBy = pm.SortBy ?? "Name";
                csm.Status = Status.Active;
                csm.Status = Status.Active;

                model = service.List1( pm, csm );
            }
            if ( model.Any() )
            {
                IEnumerable<SelectListItem> clientDDL = model.Select( c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.CompanyName

                } );


                return Json( clientDDL, JsonRequestBehavior.AllowGet );
            }
            else
            {
                return Json( data: "Error", behavior: JsonRequestBehavior.AllowGet );
            }
        }

        [AcceptVerbs( HttpVerbs.Get | HttpVerbs.Post )]
        public JsonResult GetClientDetail( string clientId )
        {
            if ( clientId != null && clientId != "" )
            {
                Client client = null;

                using ( ClientService bservice = new ClientService() )
                {
                    client = bservice.GetById( int.Parse( clientId ) );
                    return Json( client, JsonRequestBehavior.AllowGet );
                }
            }
            else
            {
                return Json( data: "Error", behavior: JsonRequestBehavior.AllowGet );
            }
        }

        [AcceptVerbs( HttpVerbs.Get | HttpVerbs.Post )]
        public JsonResult GetObjectAddress( string objectId, string objectType )
        {
            if ( objectId != null && objectId != "" && !string.IsNullOrEmpty( objectType ) )
            {
                Address address = null;

                using ( AddressService bservice = new AddressService() )
                {
                    address = bservice.GetByColumnsWhere( "ObjectId", int.Parse( objectId ), "ObjectType", objectType );
                    return Json( address, JsonRequestBehavior.AllowGet );
                }
            }
            else
            {
                return Json( data: "Error", behavior: JsonRequestBehavior.AllowGet );
            }
        }


        [AcceptVerbs( HttpVerbs.Get | HttpVerbs.Post )]
        public JsonResult SetClient( string ClientId )
        {
            if ( ClientId != null )
            {
                int.TryParse( ClientId, out int intClientId ); //to ensure we have a parseable value to use, whether 0 or greater, and not a NaN
                Session[ "ClientId" ] = intClientId;

                return Json( data: "True", behavior: JsonRequestBehavior.AllowGet );
            }
            else
            {
                return Json( data: "Error", behavior: JsonRequestBehavior.AllowGet );
            }
        }

        [AcceptVerbs( HttpVerbs.Get | HttpVerbs.Post )]
        public JsonResult SetSite( string SiteId )
        {
            if ( SiteId != null )
            {
                Session[ "SiteId" ] = SiteId;
                return Json( data: "True", behavior: JsonRequestBehavior.AllowGet );
            }
            else
            {
                return Json( data: "Error", behavior: JsonRequestBehavior.AllowGet );
            }
        }

        [AcceptVerbs( HttpVerbs.Get | HttpVerbs.Post )]
        public JsonResult SetClientSite( string SiteId )
        {
            if ( SiteId != null )
            {
                int clientId = 0;
                if ( int.Parse( SiteId ) > 0 )
                {
                    using ( SiteService service = new SiteService() )
                    {
                        clientId = service.GetClientBySite( int.Parse( SiteId ) );
                    }
                    Session[ "ClientId" ] = clientId;
                }
                Session[ "SiteId" ] = SiteId;
                return Json( data: "True", behavior: JsonRequestBehavior.AllowGet );
            }
            else
            {
                return Json( data: "Error", behavior: JsonRequestBehavior.AllowGet );
            }
        }

        public JsonResult RemoveFile( int? id = null, string utype = null, string uname = null )
        {
            int docId = ( id != null ? ( int ) id : 0 );
            if ( docId > 0 )
            {
                using ( DocumentService docservice = new DocumentService() )
                {

                    Document rDoc = docservice.GetById( docId );
                    rDoc.Status = ( int ) Status.Inactive;
                    docservice.Update( rDoc );
                }
                return Json( data: "True", behavior: JsonRequestBehavior.AllowGet );
            }
            else
            {
                return Json( data: "Error", behavior: JsonRequestBehavior.AllowGet );
            }

        }

        public JsonResult Upload( int? id = null, string utype = null, string uname = null )
        {
            FileViewModel nfv = new FileViewModel();
            for ( int i = 0; i < Request.Files.Count; i++ )
            {
                HttpPostedFileBase file = Request.Files[ i ]; //Uploaded file
                                                              //Use the following properties to get file's name, size and MIMEType
                int fileSize = file.ContentLength;
                string fileName = file.FileName;
                string mimeType = file.ContentType;
                Stream fileContent = file.InputStream;
                //To save file, use SaveAs method
                //file.SaveAs(Server.MapPath("~/") + fileName); //File will be saved in application root

                // int clientid = 0;//clientId

                if ( fileName != null )
                {
                    // Create folder
                    string path = Server.MapPath( $"~/{VariableExtension.SystemRules.DocumentsLocation}/{utype}/{id}/" );

                    if ( !Directory.Exists( path ) )
                    {
                        Directory.CreateDirectory( path );
                    }

                    string now = DateTime.Now.ToString( "yyyyMMddHHmmss" );
                    using ( DocumentService dservice = new DocumentService() )
                    {
                        Document doc = new Document()
                        {
                            ObjectId = id,//should be null for client adds, so we can update it later
                            ObjectType = utype,
                            Status = ( int ) Status.Active,
                            Name = fileName,
                            Category = utype + " Document",
                            Title = utype + " Document",
                            Size = fileSize,
                            Description = utype + " Document",
                            Type = mimeType,
                            Location = $"{utype}/{id}/{now}-{id}-{fileName}"
                        };

                        dservice.Create( doc );

                        string fullpath = Path.Combine( path, $"{now}-{id}-{fileName}" );
                        file.SaveAs( fullpath );

                        nfv.Description = doc.Description;
                        nfv.Extension = mimeType;
                        nfv.Location = doc.Location;
                        nfv.Name = fileName;
                        nfv.Id = doc.Id;

                    }
                }
            }

            //return Json("Uploaded " + Request.Files.Count + " files");
            return Json( nfv, JsonRequestBehavior.AllowGet );
        }

        public ActionResult ListFiles( string objId, string objType )
        {
            ObjectDocumentsViewModel model = new ObjectDocumentsViewModel();
            if ( !string.IsNullOrEmpty( objType ) && !string.IsNullOrEmpty( objId ) )
            {
                string controllerObjectType = objType;
                switch ( objType )
                {
                    case "ChepLoad":
                        controllerObjectType = "Pallet";
                        break;
                    case "ClientLoad":
                        controllerObjectType = "Pallet";
                        break;
                    default:
                        break;
                }
                using ( DocumentService docservice = new DocumentService() )
                {
                    List<Document> docList = new List<Document>();
                    int oId = int.Parse( objId );
                    docList = docservice.List( oId, objType );

                    model.objDocuments = docList;
                    model.objId = oId;
                    model.objType = controllerObjectType;
                }
            }
            return PartialView( "_ListDocuments", model );
        }

        public ActionResult ListFilesTable( string objId, string objType )
        {
            ObjectDocumentsViewModel model = new ObjectDocumentsViewModel();
            if ( !string.IsNullOrEmpty( objType ) && !string.IsNullOrEmpty( objId ) )
            {
                string controllerObjectType = objType;
                switch ( objType )
                {
                    case "ChepLoad":
                        controllerObjectType = "Pallet";
                        break;
                    case "ClientLoad":
                        controllerObjectType = "Pallet";
                        break;
                    default:
                        break;
                }
                using ( DocumentService docservice = new DocumentService() )
                {
                    List<Document> docList = new List<Document>();
                    int oId = int.Parse( objId );
                    docList = docservice.List( oId, objType );

                    model.objDocuments = docList;
                    model.objId = oId;
                    model.objType = controllerObjectType;
                }
            }
            return PartialView( "_ListDocumentsTable", model );
        }

        public ActionResult GetHtmlSiteList( int regionId = 0 )
        {
            using ( SiteService sservice = new SiteService() )
            {
                Dictionary<int, string> siteOptions = sservice.List( true, regionId );

                return PartialView( "_Sites", new CustomSearchModel() { SiteOptions = siteOptions } ); // Views/Dashboard/_Sites
            }
        }

        public ActionResult GetHtmlClientList( List<int> siteIds = null, int siteId = 0 )
        {
            using ( ClientService sservice = new ClientService() )
            {
                Dictionary<int, string> clientOptions = sservice.List( true, siteId, siteIds );

                return PartialView( "_Clients", new CustomSearchModel() { ClientOptions = clientOptions } ); // Views/Dashboard/_Sites
            }
        }

        public ActionResult GetClientSites( int clientId )
        {
            using ( ClientSiteService sservice = new ClientSiteService() )
            {
                Dictionary<int, string> siteOptions = sservice.List( true, clientId );

                return PartialView( "_Sites", new DeliveryNoteViewModel() { SiteOptions = siteOptions } ); // Views/Pallet/_Sites
            }
        }

        public ActionResult GetClientVehicles( int clientId )
        {
            using ( VehicleService vservice = new VehicleService() )
            {
                Dictionary<int, string> vehicleOptions = vservice.List( true, clientId, "Client" );

                return PartialView( "_Vehicles", new DeliveryNoteViewModel() { VehicleOptions = vehicleOptions } ); // Views/Pallet/_Vehicles
            }
        }

        /// <summary>
        /// Gets a product using the specified bankId and returns a JSON representation. More fields can be added when need be
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public JsonResult GetProduct( int id = 0 )
        {
            using ( ProductService service = new ProductService() )
            {
                Product p = service.GetById( id );

                if ( p == null )
                {
                    return new JsonResult()
                    {
                        Data = new { Id = 0 }
                    };
                }

                List<ProductPrice> prices = p.ProductPrices.ToList();

                return new JsonResult()
                {
                    Data = new
                    {
                        Id = p.Id,
                        Name = p.Name,
                        Description = p.Description,
                        CreatedOn = p.CreatedOn.ToString( "yyyy/MM/dd" ),
                        LostRate = prices?.FirstOrDefault( pp => pp.Type == ( int ) ProductPriceType.Lost && pp.Status == ( int ) Status.Active ).Rate,
                        IssueRate = prices?.FirstOrDefault( pp => pp.Type == ( int ) ProductPriceType.Issue && pp.Status == ( int ) Status.Active ).Rate,
                        HireRate = prices?.FirstOrDefault( pp => pp.Type == ( int ) ProductPriceType.Hire && pp.Status == ( int ) Status.Active ).Rate,
                    }
                };
            }
        }

        public int AnalyseAutoReconcile( string type )
        {
            int total = 0;

            if ( type == "loads" )
            {
                total = GetAutoReconcilliableLoadTotal();
            }
            else if ( type == "invoices" )
            {
                total = GetAutoReconcilliableInvoiceTotal();
            }

            return total;
        }

        /// <summary>
        /// Gets the total number of loads that can automatically be reconcilled
        /// </summary>
        /// <returns></returns>
        public int GetAutoReconcilliableLoadTotal()
        {
            using ( ClientLoadService clservice = new ClientLoadService() )
            {
                return clservice.GetAutoReconcilliableLoadTotal();
            }
        }

        /// <summary>
        /// Auto reconciles Client/Chep loads with matching references
        /// </summary>
        /// <returns></returns>
        public ActionResult AutoReconcileLoads()
        {
            using ( ChepLoadService chservice = new ChepLoadService() )
            using ( ClientLoadService clservice = new ClientLoadService() )
            {
                int count = 0;
                string q = string.Empty;

                #region Client & Chep Load

                List<ClientLoad> clLoads = clservice.GetAutoReconcilliableLoads();

                if ( clLoads.NullableAny() )
                {
                    foreach ( ClientLoad l in clLoads )
                    {
                        q = $"UPDATE [dbo].[ClientLoad] SET [Status]={( int ) ReconciliationStatus.Reconciled},[ModifiedOn]='{DateTime.Now}',[ModifiedBy]='{CurrentUser?.Email}' WHERE Id={l.Id};";

                        clservice.Query( q );

                        List<ChepLoadCustomModel> cheps = chservice.ListClientLoadMatch( l.ReceiverNumber?.Trim() );

                        if ( !cheps.NullableAny() ) continue;

                        int sum = cheps.Sum( s => s.Quantity ?? 0 );

                        if ( sum == 0 )
                        {
                            // Complete recon
                            q = $"UPDATE [dbo].[ChepLoad] SET [Status]={( int ) ReconciliationStatus.Reconciled},[BalanceStatus]={( int ) BalanceStatus.Balanced},[ModifiedOn]='{DateTime.Now}',[ModifiedBy]='{CurrentUser?.Email}' WHERE Id IN({string.Join( ",", cheps.Select( s => s.Id ) )});";

                            chservice.Query( q );
                        }
                        else if ( sum > 0 )
                        {
                            q = $"UPDATE [dbo].[ChepLoad] SET [Status]={( int ) ReconciliationStatus.Reconciled},[BalanceStatus]={( int ) BalanceStatus.NotBalanced},[ModifiedOn]='{DateTime.Now}',[ModifiedBy]='{CurrentUser?.Email}' WHERE Id IN({string.Join( ",", cheps.Select( s => s.Id ) )});";

                            chservice.Query( q );
                        }
                        else if ( sum < 0 )
                        {
                            q = $"UPDATE [dbo].[ChepLoad] SET [Status]={( int ) ReconciliationStatus.Unreconciled},[BalanceStatus]={( int ) BalanceStatus.NotBalanced},[ModifiedOn]='{DateTime.Now}',[ModifiedBy]='{CurrentUser?.Email}' WHERE Id IN({string.Join( ",", cheps.Select( s => s.Id ) )});";

                            chservice.Query( q );
                        }

                        count++;
                    }
                }

                #endregion

                #region Chep Load Only

                List<ChepLoadCustomModel> chs1 = new List<ChepLoadCustomModel>(),
                                          removeCH = new List<ChepLoadCustomModel>();

                List<ChepLoadCustomModel> chLoads = chservice.List1( new PagingModel() { Take = int.MaxValue }, new CustomSearchModel() { BalanceStatus = BalanceStatus.NotBalanced } );

                if ( chLoads.NullableAny() )
                {
                    foreach ( ChepLoadCustomModel item in chLoads )
                    {
                        if ( removeCH.NullableAny( r => r.Id == item.Id ) ) continue;

                        q = $"SELECT ch.[Id], ch.[Quantity] FROM [dbo].[ChepLoad] ch WHERE ch.[Id] != {item.Id} AND (LTRIM(RTRIM(ch.[Ref]))='{item.DocketNumber?.Trim()}' OR LTRIM(RTRIM(ch.[Ref]))='{item.Ref?.Trim()}' OR LTRIM(RTRIM(ch.[Ref]))='{item.OtherRef?.Trim()}' OR LTRIM(RTRIM(ch.[OtherRef]))='{item.OtherRef?.Trim()}');";

                        chs1 = chservice.SqlQueryList<ChepLoadCustomModel>( q );

                        if ( !chs1.NullableAny() ) continue;

                        if ( ( chs1.Sum( s => s.Quantity ) + item.Quantity ) != 0 ) continue;

                        chs1.Add( item );

                        q = $"UPDATE [dbo].[ChepLoad] SET [Status]={( int ) ReconciliationStatus.Reconciled},[BalanceStatus]={( int ) BalanceStatus.Balanced},[ModifiedOn]='{DateTime.Now}',[ModifiedBy]='{CurrentUser?.Email}' WHERE Id IN({string.Join( ",", chs1.Select( s => s.Id ) )});";

                        chservice.Query( q );

                        removeCH.AddRange( chLoads.Where( r => chs1.Select( s => s.Id ).Contains( r.Id ) ) );

                        count++;
                    }
                }

                #endregion

                Notify( $"{count} loads were successfully reconcilled.", NotificationType.Success );
            }

            return PartialView( "_Empty" );
        }

        /// <summary>
        /// Gets the total number of invoice that can automatically be reconcilled
        /// </summary>
        /// <returns></returns>
        public int GetAutoReconcilliableInvoiceTotal()
        {
            using ( InvoiceService iservice = new InvoiceService() )
            {
                return iservice.GetAutoReconcilliableInvoiceTotal();
            }
        }

        /// <summary>
        /// Gets a list of outstanding pallets
        /// </summary>
        /// <returns></returns>
        public List<OutstandingPalletsModel> GetOutstandingPallets( PagingModel pm, CustomSearchModel csm, string email = "" )
        {
            using ( ChepLoadService chservice = new ChepLoadService() )
            {
                if ( !string.IsNullOrWhiteSpace( email ) || chservice.CurrentUser == null )
                {
                    chservice.CurrentUser = chservice.GetUser( email );
                }

                csm.BalanceStatus = BalanceStatus.NotBalanced;

                List<OutstandingPalletsModel> resp = new List<OutstandingPalletsModel>();

                pm.Skip = 0;
                pm.Sort = "ASC";
                pm.Take = int.MaxValue;
                pm.SortBy = "c.CompanyName";

                List<ChepLoadCustomModel> model = chservice.List1( pm, csm );

                if ( !model.NullableAny() )
                {
                    return resp;
                }

                List<int?> clientIds = new List<int?>();

                foreach ( ChepLoadCustomModel item in model )
                {
                    if ( clientIds.Any( c => c == item.ClientId ) ) continue;

                    clientIds.Add( item.ClientId );

                    OutstandingPalletsModel m = new OutstandingPalletsModel()
                    {
                        ClientLoad = item,
                        OutstandingReasons = GetOutstandingReasons( item.ClientId, model ),
                        GrandTotal = new OutstandingReasonModel()
                        {
                            Description = "Grand Total",
                            To30Days = model.Count( c => c.ClientId == item.ClientId && ( DateTime.Now - ( item.EffectiveDate ?? item.CreatedOn ) ).Days <= 30 ),
                            To60Days = model.Count( c => c.ClientId == item.ClientId && ( ( DateTime.Now - ( item.EffectiveDate ?? item.CreatedOn ) ).Days >= 31 && ( DateTime.Now - ( item.EffectiveDate ?? item.CreatedOn ) ).Days <= 60 ) ),
                            To90Days = model.Count( c => c.ClientId == item.ClientId && ( ( DateTime.Now - ( item.EffectiveDate ?? item.CreatedOn ) ).Days >= 61 && ( DateTime.Now - ( item.EffectiveDate ?? item.CreatedOn ) ).Days <= 90 ) ),
                            To120Days = model.Count( c => c.ClientId == item.ClientId && ( ( DateTime.Now - ( item.EffectiveDate ?? item.CreatedOn ) ).Days >= 91 && ( DateTime.Now - ( item.EffectiveDate ?? item.CreatedOn ) ).Days <= 120 ) ),
                            To183Days = model.Count( c => c.ClientId == item.ClientId && ( ( DateTime.Now - ( item.EffectiveDate ?? item.CreatedOn ) ).Days >= 121 && ( DateTime.Now - ( item.EffectiveDate ?? item.CreatedOn ) ).Days <= 183 ) ),
                            To270Days = model.Count( c => c.ClientId == item.ClientId && ( ( DateTime.Now - ( item.EffectiveDate ?? item.CreatedOn ) ).Days >= 184 && ( DateTime.Now - ( item.EffectiveDate ?? item.CreatedOn ) ).Days <= 270 ) ),
                            To365Days = model.Count( c => c.ClientId == item.ClientId && ( ( DateTime.Now - ( item.EffectiveDate ?? item.CreatedOn ) ).Days >= 271 && ( DateTime.Now - ( item.EffectiveDate ?? item.CreatedOn ) ).Days <= 365 ) ),
                            GrandTotal = model.Count( c => c.ClientId == item.ClientId ),
                        }
                    };

                    resp.Add( m );
                }

                return resp;
            }
        }

        /// <summary>
        /// Gets a list of outstanding reasons
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="items"></param>
        /// <returns></returns>
        public List<OutstandingReasonModel> GetOutstandingReasons( int clientId, List<ChepLoadCustomModel> items )
        {
            List<string> outstandingIds = new List<string>();

            List<OutstandingReasonModel> outstandingReasons = new List<OutstandingReasonModel>();

            foreach ( ChepLoadCustomModel item in items )
            {
                if ( item.ClientId != clientId ) continue;

                if ( outstandingIds.Any( c => c == item.OutstandingReasonId + "-" + clientId ) ) continue;

                outstandingIds.Add( item.OutstandingReasonId + "-" + clientId );

                OutstandingReasonModel r = new OutstandingReasonModel()
                {
                    Description = item.OutstandingReason ?? "-N/A-",
                    To30Days = items.Count( c => c.OutstandingReasonId == item.OutstandingReasonId && c.ClientId == clientId && ( DateTime.Now - ( item.EffectiveDate ?? item.CreatedOn ) ).Days <= 30 ),
                    To60Days = items.Count( c => c.OutstandingReasonId == item.OutstandingReasonId && c.ClientId == clientId && ( ( DateTime.Now - ( item.EffectiveDate ?? item.CreatedOn ) ).Days >= 31 && ( DateTime.Now - ( item.EffectiveDate ?? item.CreatedOn ) ).Days <= 60 ) ),
                    To90Days = items.Count( c => c.OutstandingReasonId == item.OutstandingReasonId && c.ClientId == clientId && ( ( DateTime.Now - ( item.EffectiveDate ?? item.CreatedOn ) ).Days >= 61 && ( DateTime.Now - ( item.EffectiveDate ?? item.CreatedOn ) ).Days <= 90 ) ),
                    To120Days = items.Count( c => c.OutstandingReasonId == item.OutstandingReasonId && c.ClientId == clientId && ( ( DateTime.Now - ( item.EffectiveDate ?? item.CreatedOn ) ).Days >= 91 && ( DateTime.Now - ( item.EffectiveDate ?? item.CreatedOn ) ).Days <= 120 ) ),
                    To183Days = items.Count( c => c.OutstandingReasonId == item.OutstandingReasonId && c.ClientId == clientId && ( ( DateTime.Now - ( item.EffectiveDate ?? item.CreatedOn ) ).Days >= 121 && ( DateTime.Now - ( item.EffectiveDate ?? item.CreatedOn ) ).Days <= 183 ) ),
                    To270Days = items.Count( c => c.OutstandingReasonId == item.OutstandingReasonId && c.ClientId == clientId && ( ( DateTime.Now - ( item.EffectiveDate ?? item.CreatedOn ) ).Days >= 184 && ( DateTime.Now - ( item.EffectiveDate ?? item.CreatedOn ) ).Days <= 270 ) ),
                    To365Days = items.Count( c => c.OutstandingReasonId == item.OutstandingReasonId && c.ClientId == clientId && ( ( DateTime.Now - ( item.EffectiveDate ?? item.CreatedOn ) ).Days >= 271 && ( DateTime.Now - ( item.EffectiveDate ?? item.CreatedOn ) ).Days <= 365 ) ),
                    GrandTotal = items.Count( c => c.OutstandingReasonId == item.OutstandingReasonId && c.ClientId == clientId ),
                };

                outstandingReasons.Add( r );
            }

            return outstandingReasons;
        }

        /// <summary>
        /// Automatically reconciles all unreconcilled invoices
        /// </summary>
        /// <returns></returns>
        public ActionResult AutoReconcileInvoices()
        {
            using ( InvoiceService iservice = new InvoiceService() )
            using ( ClientLoadService clservice = new ClientLoadService() )
            using ( ClientInvoiceService ciservice = new ClientInvoiceService() )
            {
                List<Invoice> invoices = iservice.ListUnReconciledInvoices();

                if ( !invoices.NullableAny() ) return PartialView( "_Empty" );

                foreach ( Invoice i in invoices )
                {
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
                                InvoiceId = i.Id,
                                ClientLoadId = item.Id,
                                Status = ( int ) Status.Active,
                            };

                            ciservice.Create( ci );
                        }
                    }
                }

                Notify( $"{invoices.Count} invoices were successfully reconcilled.", NotificationType.Success );
            }

            return PartialView( "_Empty" );
        }

        /// <summary>
        /// Logs any errors in the specified params
        /// </summary>
        /// <param name="errors"></param>
        /// <param name="r"></param>
        /// <returns></returns>
        public int LogImportErrors( List<string> errors, int r )
        {
            Document doc;

            // Create folder
            string path = Server.MapPath( $"~/{VariableExtension.SystemRules.DocumentsLocation}/Errors/{r}/" );

            if ( !Directory.Exists( path ) )
            {
                Directory.CreateDirectory( path );
            }

            string now = DateTime.Now.ToString( "yyyyMMddHHmmss" );

            string fileName = $"{path}{now}-ImportPoolingAgentData-Errors.txt";

            using ( StreamWriter sw = new StreamWriter( fileName, true ) )
            using ( DocumentService dservice = new DocumentService() )
            {
                foreach ( string e in errors )
                {
                    sw.WriteLine( e );
                }

                doc = new Document()
                {
                    Size = 0,
                    Type = "txt",
                    ObjectId = r,
                    ObjectType = "Error",
                    Status = ( int ) Status.Active,
                    Name = "Import Pooling Agent Data Errors",
                    Title = "Import Pooling Agent Data Errors",
                    Category = "Import Pooling Agent Data Errors",
                    Description = "Import Pooling Agent Data Errors",
                    Location = $"Errors/{now}-ImportPoolingAgentData-Errors.txt",
                };

                doc = dservice.Create( doc );
            }

            return doc.Id;
        }

        /// <summary>
        /// Gets a list of customers with outstanding pallets
        /// </summary>
        /// <param name="pm"></param>
        /// <param name="csm"></param>
        /// <returns></returns>
        public List<OutstandingPalletsModel> GetOustandingCustomers( PagingModel pm, CustomSearchModel csm )
        {
            using ( ChepLoadService clservice = new ChepLoadService() )
            {
                List<ChepLoadCustomModel> loads = clservice.ListTopOustandingCustomers( pm, csm );

                List<OutstandingPalletsModel> model = new List<OutstandingPalletsModel>();

                if ( loads.NullableAny() )
                {
                    foreach ( ChepLoadCustomModel item in loads )
                    {
                        if ( model.NullableAny( m => m.ClientLoad.ClientId == item.ClientId ) ) continue;

                        OutstandingPalletsModel o = new OutstandingPalletsModel()
                        {
                            ClientLoad = item,
                            Total = loads.Count( l => l.ClientId == item.ClientId ),
                            Regions = GetOutstandingRegions( loads.Where( l => l.ClientId == item.ClientId ).ToList() )
                        };

                        model.Add( o );
                    }
                }


                return model;
            }
        }

        /// <summary>
        /// Gets a list of regions with outstanding pallets
        /// </summary>
        /// <param name="loads"></param>
        /// <returns></returns>
        public List<OutstandingRegionModel> GetOutstandingRegions( List<ChepLoadCustomModel> loads )
        {
            List<OutstandingRegionModel> regions = new List<OutstandingRegionModel>();

            if ( !loads.NullableAny() ) return regions;

            List<string> regionIds = new List<string>();

            foreach ( ChepLoadCustomModel item in loads )
            {
                if ( regionIds.NullableAny( r => r == item.RegionName ) ) continue;

                regionIds.Add( item.RegionName );

                OutstandingRegionModel reg = new OutstandingRegionModel()
                {
                    Name = item.RegionName,
                    Total = loads.Count( l => l.RegionName == item.RegionName ),
                    Sites = GetOutstandingSites( loads.Where( l => l.RegionName == item.RegionName ).ToList() ),
                };

                regions.Add( reg );
            }

            return regions;
        }

        /// <summary>
        /// Gets a list of client sites with outstanding pallets
        /// </summary>
        /// <param name="loads"></param>
        /// <returns></returns>
        public List<OutstandingSiteModel> GetOutstandingSites( List<ChepLoadCustomModel> loads )
        {
            List<OutstandingSiteModel> sites = new List<OutstandingSiteModel>();

            if ( !loads.NullableAny() ) return sites;

            List<int?> siteIds = new List<int?>();

            foreach ( ChepLoadCustomModel item in loads )
            {
                if ( siteIds.NullableAny( r => r == item.ClientSiteId ) ) continue;

                siteIds.Add( item.ClientSiteId );

                OutstandingSiteModel s = new OutstandingSiteModel()
                {
                    Name = item.SiteName,
                    Total = loads.Count( l => l.ClientSiteId == item.ClientSiteId ),
                    OutstandingReasons = GetOutstandingReasons1( loads.Where( l => l.ClientSiteId == item.ClientSiteId ).ToList() ),
                };

                sites.Add( s );
            }

            return sites;
        }

        /// <summary>
        /// Gets a list of outstanding reasons with outstanding pallets
        /// </summary>
        /// <param name="loads"></param>
        /// <returns></returns>
        public List<OutstandingReasonModel> GetOutstandingReasons1( List<ChepLoadCustomModel> loads )
        {
            List<OutstandingReasonModel> outstandingReasons = new List<OutstandingReasonModel>();

            if ( !loads.NullableAny() ) return outstandingReasons;

            List<int?> outstandingReasonIds = new List<int?>();

            foreach ( ChepLoadCustomModel item in loads )
            {
                if ( outstandingReasonIds.NullableAny( r => r == item.OutstandingReasonId ) ) continue;

                outstandingReasonIds.Add( item.OutstandingReasonId );

                OutstandingReasonModel s = new OutstandingReasonModel()
                {
                    Description = item.OutstandingReason,
                    Total = loads.Count( l => l.OutstandingReasonId == item.OutstandingReasonId ),
                };

                outstandingReasons.Add( s );
            }

            return outstandingReasons;
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
