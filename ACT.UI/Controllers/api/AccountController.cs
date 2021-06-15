
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Hosting;
using System.Web.Http;
using System.Web.Routing;

using ACT.Core.Enums;
using ACT.Core.Models;
using ACT.Core.Models.Custom;
using ACT.Core.Services;
using ACT.Data.Models;
using ACT.UI.Models;

namespace ACT.UI.Controllers.API
{
    public class AccountController : ApiController
    {
        BaseController baseC = new BaseController();

        // GET api/<Account>/5
        [HttpGet]
        public UserModel LoginByPin( string pin, string email, string apikey )
        {
            using ( UserService service = new UserService() )
            {
                UserModel user = service.GetByPin( pin, email );

                // User valid?

                if ( user == null )
                {
                    user = new UserModel()
                    {
                        Code = -1,
                        Message = "Incorrect pin."
                    };
                }
                else
                {
                    user.Code = 1;
                    user.Message = "Login Success!";
                }

                return user;
            }
        }

        // GET api/<Account>/5
        [HttpGet]
        public UserModel Login( string apikey, string email, string password )
        {
            using ( UserService service = new UserService() )
            {
                UserModel user = service.Login( email, password, true );

                // User valid?
                if ( user == null )
                {
                    user = new UserModel()
                    {
                        Code = -1,
                        Message = "Incorrect email and password combination."
                    };
                }
                else
                {
                    user.Code = 1;
                    user.Message = "Login Success!";
                }

                return user;
            }
        }


        //
        // POST: api/Account/Update/5
        [HttpGet]
        [HttpPost]
        public UserModel Update( UserViewModel model )
        {
            User user;

            try
            {
                UserModel response = new UserModel();

                using ( UserService uservice = new UserService() )
                {
                    user = uservice.GetById( model.Id );

                    if ( user == null )
                    {
                        return new UserModel()
                        {
                            Code = -1,
                            Message = "Sorry, that User does not exist! Please specify a valid User Id and try again."
                        };
                    }


                    #region Validations

                    if ( !( user.Email ?? "" ).Trim().Equals( model.Email ) && uservice.ExistByEmail( model.Email?.Trim() ) )
                    {
                        return new UserModel()
                        {
                            Code = -1,
                            Message = $"Sorry, a User with the Email Address \"{model.Email}\" already exists!"
                        };
                    }

                    #endregion



                    #region Update User

                    // Update User
                    user.Cell = model.Cell;
                    user.Name = model.Name;
                    user.Email = model.Email;
                    user.Surname = model.Surname;

                    user = uservice.Update( user );

                    #endregion
                }

                response = new UserService().GetUser( user.Email );

                response.Code = 1;
                response.Message = "Your Account was successfully updated.";

                return response;
            }
            catch ( Exception ex )
            {
                return new UserModel()
                {
                    Code = -1,
                    Message = $"{ex.Message} - {ex.InnerException?.Message} - {ex.InnerException?.InnerException?.Message}"
                };
            }
        }

        [HttpGet]
        public List<BroadcastCustomModel> Notifications( int id, string apikey )
        {
            using ( BroadcastService service = new BroadcastService() )
            {
                List<BroadcastCustomModel> resp = new List<BroadcastCustomModel>();

                List<Broadcast> broadcasts = service.List( new PagingModel(), new CustomSearchModel() );

                if ( broadcasts.NullableAny() )
                {
                    foreach ( Broadcast b in broadcasts.OrderByDescending( o => o.CreatedOn ) )
                    {
                        if ( b.Status == Status.Inactive.GetIntValue() ) continue;

                        resp.Add( new BroadcastCustomModel()
                        {
                            Id = b.Id,
                            Message = b.Message,
                            CreatedOn = b.CreatedOn,
                            Status = b.UserBroadcasts.Any( ub => ub.UserId == id ) ? 1 : 0,
                        } );
                    }
                }

                return resp;
            }
        }

        // GET api/<Account>/ForgotPassword
        [HttpGet]
        public UserModel ForgotPassword( string apiKey, string email )
        {
            try
            {
                using ( UserService uservice = new UserService() )
                using ( TokenService tservice = new TokenService() )
                {
                    baseC.CurrentUser = uservice.GetUser( email );

                    User user = uservice.GetByEmail( email );

                    if ( user == null )
                    {
                        baseC.CurrentUser = new UserModel()
                        {
                            Code = -1,
                            Message = "Email Address could not be found"
                        };

                        return baseC.CurrentUser;
                    }

                    baseC.CurrentUser.Code = 1;

                    Token t = new Token()
                    {
                        UserId = user.Id,
                        UID = Guid.NewGuid(),
                        Status = ( int ) Status.Active
                    };

                    t = tservice.Create( t );

                    //user.Reset = true;

                    user = uservice.Update( user );

                    // Add key value
                    RouteData route = new RouteData();
                    route.Values.Add( "controller", "Base" ); // Controller Name

                    System.Web.Mvc.ControllerContext newContext = new System.Web.Mvc.ControllerContext( new HttpContextWrapper( System.Web.HttpContext.Current ), route, baseC );
                    baseC.ControllerContext = newContext;

                    Boolean sent = baseC.SendResetPassword( t.UID, user );

                    if ( sent )
                    {
                        baseC.CurrentUser.Message = "A Password Reset Request has been sent to your Email Address";
                    }
                    else
                    {
                        baseC.CurrentUser.Message = "Sorry, a Password Reset Request could not be sent. Please try again later";
                    }

                    return baseC.CurrentUser;
                }
            }
            catch ( Exception ex )
            {
                return new UserModel()
                {
                    Code = -1,
                    Message = $"{ex.Message} - {ex.InnerException?.Message}"
                };
            }
        }


        //
        // POST: api/Account/Update/5
        [HttpGet]
        [HttpPost]
        public UserModel UpdatePin( UserViewModel model )
        {
            try
            {
                using ( UserService uservice = new UserService() )
                {
                    User user = uservice.GetById( model.Id );

                    if ( user == null )
                    {
                        return new UserModel()
                        {
                            Code = -1,
                            Message = "Sorry, that User does not exist! Please specify a valid User Id and try again."
                        };
                    }


                    #region Validations

                    if ( model.Pin != model.ConfirmPin )
                    {
                        return new UserModel()
                        {
                            Code = -1,
                            Message = $"Pin and Confirm Pin not the same!"
                        };
                    }

                    #endregion



                    #region Update User

                    // Update User
                    user.Pin = uservice.GetSha1Md5String( model.Pin );

                    user = uservice.Update( user );

                    #endregion

                    UserModel response = new UserService().GetUser( user.Email );

                    response.Code = 1;
                    response.Message = "Your pin was successfully updated.";

                    return response;
                }
            }
            catch ( Exception ex )
            {
                return new UserModel()
                {
                    Code = -1,
                    Message = $"{ex.Message} - {ex.InnerException?.Message} - {ex.InnerException?.InnerException?.Message}"
                };
            }
        }


        [HttpPost]
        public BroadcastCustomModel MarkNotificationAsRead( int id, int userid, string apikey )
        {
            using ( UserBroadcastService service = new UserBroadcastService() )
            {
                UserBroadcast ub = new UserBroadcast()
                {
                    BroadcastId = id,
                    UserId = userid
                };

                service.Create( ub );
            }

            return new BroadcastCustomModel()
            {
                ResponseCode = 1,
                Description = "Updated successfully"
            };
        }

        [HttpPost]
        public UserModel UploadDocument( int agentId, string apikey )
        {
            UserModel response = new UserModel();

            try
            {
                using ( UserService uservice = new UserService() )
                using ( ImageService iservice = new ImageService() )
                {
                    User user = uservice.GetById( agentId );

                    if ( user == null )
                    {
                        response.Code = -1;
                        response.Message = "We could not find an Agent using the specified details.";

                        return response;
                    }

                    response.Id = user.Id;

                    HttpPostedFile file = HttpContext.Current.Request.Files.Count > 0 ? HttpContext.Current.Request.Files[ 0 ] : null;

                    if ( file == null )
                    {
                        response.Code = -1;
                        response.Message = "No file uploaded!";

                        return response;
                    }

                    // Create folder
                    string path = HostingEnvironment.MapPath( $"~/{VariableExtension.SystemRules.ImagesLocation}/Users/{user.Cell?.Trim()}/" );

                    string now = DateTime.Now.ToString( "yyyyMMddHHmmss" );

                    string ext = Path.GetExtension( file.FileName ),
                           name = file.FileName.Replace( ext, "" );

                    // Check if a logo already exists?
                    Image img = iservice.Get( user.Id, "User", ( name?.ToLower() == "selfie" ) );

                    if ( img != null )
                    {
                        DeleteImage( img.Id );
                    }

                    if ( !Directory.Exists( path ) )
                    {
                        Directory.CreateDirectory( path );
                    }

                    Image image = new Image()
                    {
                        Name = name,
                        ObjectId = user.Id,
                        ObjectType = "User",
                        Size = file.ContentLength,
                        Description = name,
                        IsMain = ( name?.ToLower() == "selfie" ),
                        Extension = ext,
                        Location = $"Users/{user.Cell?.Trim()}/{now}-{file.FileName}"
                    };

                    iservice.Create( image );

                    string fullpath = Path.Combine( path, $"{now}-{file.FileName}" );

                    file.SaveAs( fullpath );

                    response.Code = 1;
                    response.Message = "Document upload successful..!";
                }
            }
            catch ( Exception ex )
            {
                return new UserModel()
                {
                    Code = -1,
                    Message = $"{ex.Message} - {ex.InnerException?.Message} - {ex.InnerException?.InnerException?.Message} - TRACE - {ex.StackTrace} Line - {( new StackTrace( ex, true ) ).GetFrame( 0 ).GetFileLineNumber()}"
                };
            }

            return response;
        }

        /// <summary>
        /// Deletes an image
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool DeleteImage( int id )
        {
            using ( ImageService service = new ImageService() )
            {
                Image i = service.GetById( id );

                if ( i == null )
                    return false;

                string path = HostingEnvironment.MapPath( string.Format( "{0}/{1}", VariableExtension.SystemRules.ImagesLocation, i.Location ) );

                service.Delete( i );

                if ( File.Exists( path ) )
                {
                    File.Delete( path );
                }
            }

            return true;
        }

        [HttpGet]
        public SystemConfig GetRules( string apiKey )
        {
            using ( UserService uservice = new UserService() )
            {
                //uservice.CurrentUser = uservice.GetUser( email );

                return uservice.SystemConfig;
            }
        }

        [HttpGet]
        public object GetRoles( string apiKey )
        {
            List<object> obj = new List<object>();

            foreach ( int e in Enum.GetValues( typeof( RoleType ) ) )
            {
                obj.Add( new { Key = e, Value = ( ( RoleType ) e ).GetDisplayText() } );
            }

            return obj;
        }

        [HttpGet]
        public List<PSPCustomModel> GetPSPs( string email, string apiKey )
        {
            using ( PSPService service = new PSPService() )
            {
                service.CurrentUser = service.GetUser( email );

                return service.List1( new PagingModel() { Take = int.MaxValue, Skip = 0, Query = string.Empty }, new CustomSearchModel() { Status = Status.Active } );
            }
        }

        [HttpGet]
        public List<CommonModel> GetSites( string email, string apiKey )
        {
            using ( SiteService service = new SiteService() )
            {
                service.CurrentUser = service.GetUser( email );

                return service.ListTiny( new PagingModel() { Take = int.MaxValue, Skip = 0, Sort ="ASC", SortBy = "s.Name", Query = string.Empty }, new CustomSearchModel() { Status = Status.Active } );
            }
        }

        [HttpGet]
        public List<ClientCustomModel> GetClients( string email, string apiKey )
        {
            using ( ClientService service = new ClientService() )
            {
                service.CurrentUser = service.GetUser( email );

                return service.List1( new PagingModel() { Take = int.MaxValue, Skip = 0, Query = string.Empty }, new CustomSearchModel() { Status = Status.Active } );
            }
        }
    }
}
