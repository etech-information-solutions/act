using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

using ACT.Core.Enums;
using ACT.Core.Models;
using ACT.Core.Models.Custom;
using ACT.Core.Services;
using ACT.Data.Models;
using System.Transactions;
using Newtonsoft.Json;
using System.Web;

namespace ACT.UI.Controllers.api
{
    public class ChatController : ApiController
    {
        BaseController baseC = new BaseController( HttpContext.Current.Server.MapPath( $"~/Logs/log.log" ) );

        // GET api/Chat/List
        [HttpGet]
        public List<TicketCustomModel> List( string email, string apikey )
        {
            using ( TicketService tservice = new TicketService() )
            {
                tservice.CurrentUser = tservice.GetUser( email );

                if ( tservice.CurrentUser == null )
                {

                }

                PagingModel pm = new PagingModel() { Take = int.MaxValue, Sort = "DESC", SortBy = "m.CreatedOn" };
                CustomSearchModel csm = new CustomSearchModel() { UserId = tservice.CurrentUser.Id };

                List<TicketCustomModel> response = tservice.List1( pm, csm );

                if ( !response.NullableAny() )
                {
                    return response;
                }

                return response;
            }
        }

        // POST api/Chat/Send
        [HttpPost]
        public TicketCustomModel Send( MessageCustomModel model )
        {
            int ticketId = 0,
                messageId = 0;

            using ( UserService uservice = new UserService() )
            using ( TicketService tservice = new TicketService() )
            using ( MessageService mservice = new MessageService() )
            using ( TransactionScope scope = new TransactionScope() )
            {
                uservice.CurrentUser = uservice.GetUser( model.Email );

                baseC.LogWriter.WriteLine();
                baseC.LogWriter.WriteLine( $"BEGIN: Creating chat @{DateTime.Now} by {uservice.CurrentUser.Email}" );

                #region Determine Receiver

                int? receiverId = model.ReceiverUserId;

                if ( !receiverId.HasValue )
                {
                    List<User> users = uservice.ListAvalaibleSupport(); // => Can Receive

                    if ( users.NullableAny( a => ( OnlineStatus ) a.ChatStatus == OnlineStatus.Online ) )
                    {
                        receiverId = users.FirstOrDefault( a => ( OnlineStatus ) a.ChatStatus == OnlineStatus.Online ).Id;
                    }
                    else if ( users.NullableAny() )
                    {
                        receiverId = users.FirstOrDefault().Id;
                    }
                }

                model.ReceiverUserId = receiverId;

                #endregion

                int status = model.IsClose ? 1 : 0;

                #region Ticket

                Ticket t = ( model.TicketId > 0 ) ? tservice.GetById( model.TicketId ) : null;

                string number = tservice.GetSha1Md5String( DateTime.Now.Ticks.ToString() );

                if ( t == null )
                {
                    t = new Ticket()
                    {
                        Status = status,
                        UID = Guid.NewGuid(),
                        OwnerUserId = model.SenderUserId,
                        Number = number.Substring( 0, 5 ),
                        DepartmentId = model.DepartmentId,
                        SupportUserId = model.ReceiverUserId,
                    };

                    tservice.Create( t );
                }
                else if ( t != null && model.IsClose )
                {
                    t.Rating = model.Rating;
                    t.DateClosed = DateTime.Now;
                    t.ClosedByUserId = model.SenderUserId;

                    tservice.Update( t );
                }

                ticketId = t.Id;

                #endregion

                #region Message

                Message m = new Message()
                {
                    TicketId = t.Id,
                    Status = status,
                    UID = Guid.NewGuid(),
                    Details = model.Details,
                    IsClose = model.IsClose,
                    IsSupport = model.IsSupport,
                    SenderUserId = model.SenderUserId,
                    ReceiverUserId = model.ReceiverUserId,
                };

                m = mservice.Create( m );

                messageId = m.Id;

                #endregion

                baseC.LogWriter.WriteLine();
                baseC.LogWriter.WriteLine( $"FINISH: Creating chat @{DateTime.Now} by {uservice.CurrentUser.Email}" );

                scope.Complete();
            }

            SendMobileNotification( model.ReceiverUserId ?? 0, model.Details, ticketId, messageId, model.Email );

            List<TicketCustomModel> tickets = List( model.Email, model.APIKey );

            return tickets?.FirstOrDefault();
        }

        /// <summary>
        /// Sends a notification to a user using the specified params
        /// </summary>
        /// <param name="receiverId"></param>
        /// <param name="message"></param>
        /// <param name="ticketId"></param>
        /// <param name="messageId"></param>
        /// <returns></returns>
        public bool SendMobileNotification( int receiverId, string message, int ticketId, int messageId, string email )
        {
            using ( UserService uservice = new UserService() )
            {
                uservice.CurrentUser = uservice.GetUser( email );

                User receiver = uservice.GetById( receiverId );

                if ( receiver == null )
                {
                    baseC.LogWriter.WriteLine();
                    baseC.LogWriter.WriteLine( $"x FAILED TO INITIATE sending of a Notification @{DateTime.Now} to receiver {receiverId}" );

                    return false;
                }

                if ( string.IsNullOrWhiteSpace( receiver.DeviceId ) )
                {
                    baseC.LogWriter.WriteLine();
                    baseC.LogWriter.WriteLine( $"x FAILED TO INITIATE sending of a Notification @{DateTime.Now} to receiver {receiver.Name} {receiver.Surname} because there's no DeviceId setup. Please advise user to launch ACT Mobile App at least once!" );

                    return false;
                }

                ConfigSettings.SetRules();

                /*var payload = new
                {
                    priority = "high",
                    to = receiver.DeviceId,
                    content_available = true,
                    notification = new
                    {
                        body = model.Details,
                        title = $"{uservice.CurrentUser.Name} {uservice.CurrentUser.Surname}",
                        badge = 1,
                        sound = "default",
                    },
                    data = new
                    {
                        TicketId = t.Id,
                        MessageId = m.Id
                    }
                };*/

                //string postbody = JsonConvert.SerializeObject( payload ).ToString();

                /*List<string> headers = new List<string>()
                {
                    { $"Authorization: key={ConfigSettings.SystemRules.FirebaseServerKey}" }, // ServerKey - Key from Firebase cloud messaging server 
                    { $"Sender: id={ConfigSettings.SystemRules.FirebaseSenderId}" }, // Sender Id - From firebase project setting
                };*/

                List<string> headers = new List<string>()
                {
                    { $"Authorization: Basic {ConfigSettings.SystemRules.OneSignaIAPIKey}" },
                };

                var payload = new
                {
                    app_id = ConfigSettings.SystemRules.OneSignaIAppId,
                    include_player_ids = new string[] { receiver.DeviceId },
                    headings = new
                    {
                        en = $"{uservice.CurrentUser.Name} {uservice.CurrentUser.Surname}"
                    },
                    contents = new
                    {
                        en = message
                    },
                    data = new
                    {
                        TicketId = ticketId,
                        MessageId = messageId
                    }
                };

                string postbody = JsonConvert.SerializeObject( payload ).ToString();

                baseC.LogWriter.WriteLine();
                baseC.LogWriter.WriteLine( $"   - SENT Notification @{DateTime.Now}" );

                baseC.LogWriter.WriteLine();
                baseC.LogWriter.WriteLine( $"   {postbody}" );

                string resp = uservice.Post( ConfigSettings.SystemRules.OneSignalAPIUrl, headers, postbody, "application/json", "text/json" );

                baseC.LogWriter.WriteLine();
                baseC.LogWriter.WriteLine( $"   - Notification RESPONSE @{DateTime.Now}" );
                baseC.LogWriter.WriteLine();
                baseC.LogWriter.WriteLine( $"   {resp}" );

                return true;
            }
        }

        // POST api/Chat/MarkAsRead
        [HttpPost]
        public ResponseModel MarkAsRead( MessageCustomModel model )
        {
            using ( MessageService mservice = new MessageService() )
            {
                mservice.Query( $"UPDATE [dbo].[Message] SET [Status]=1 WHERE [TicketId]={model.TicketId} AND [ReceiverUserId]={model.ReceiverUserId} AND [Status]=0;" );
            }

            return new ResponseModel() { Code = 1 };
        }

        // GET api/Chat/Receive
        [HttpGet]
        public TicketCustomModel Receive( string email, string apikey, int ticketId )
        {
            using ( TicketService tservice = new TicketService() )
            {
                tservice.CurrentUser = tservice.GetUser( email );

                PagingModel pm = new PagingModel() { Take = int.MaxValue, Sort = "DESC", SortBy = "m.CreatedOn" };
                CustomSearchModel csm = new CustomSearchModel() { UserId = tservice.CurrentUser.Id, TicketId = ticketId };

                return tservice.List1( pm, csm )?.FirstOrDefault();
            }
        }

        // GET api/Chat/GetDepartments
        [HttpGet]
        public List<Department> GetDepartments( string apikey )
        {
            using ( DepartmentService dservice = new DepartmentService() )
            {
                return dservice.List();
            }
        }

        // POST api/Chat/UpDateDeviceCode
        [HttpPost]
        public ResponseModel UpDateDeviceCode( UserModel model )
        {
            baseC.LogWriter.WriteLine();
            baseC.LogWriter.WriteLine( $"BEGIN: UPDATING Device Code for {model.Id} @{DateTime.Now} with Device Code: {model.DeviceId}, Device OS: {model.DeviceOS}" );

            using ( UserService uservice = new UserService() )
            {
                User u = uservice.GetById( model.Id );

                if ( u == null )
                {
                    baseC.LogWriter.WriteLine();
                    baseC.LogWriter.WriteLine( $"User {model.Id} NOT FOUND @{DateTime.Now}" );

                    return new ResponseModel() { Code = -1 };
                }

                u.DeviceId = model.DeviceId;
                u.DeviceOS = model.DeviceOS;

                uservice.Update( u );

                baseC.LogWriter.WriteLine();
                baseC.LogWriter.WriteLine( $"FINISH: UPDATING Device Code for {model.Id} @{DateTime.Now} with Device Code: {model.DeviceId}, Device OS: {model.DeviceOS}" );

                return new ResponseModel() { Code = 1 };
            }
        }
    }
}
