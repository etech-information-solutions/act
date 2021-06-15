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

namespace ACT.UI.Controllers.api
{
    public class ChatController : ApiController
    {
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
            using ( UserService uservice = new UserService() )
            using ( TicketService tservice = new TicketService() )
            using ( MessageService mservice = new MessageService() )
            using ( TransactionScope scope = new TransactionScope() )
            {
                uservice.CurrentUser = tservice.GetUser( model.Email );

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
                        SupportUserId = receiverId,
                        OwnerUserId = model.SenderUserId,
                        Number = number.Substring( 0, 5 ),
                        DepartmentId = model.DepartmentId,
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
                    ReceiverUserId = receiverId,
                    SenderUserId = model.SenderUserId,
                };

                mservice.Create( m );

                #endregion

                scope.Complete();


            }

            List<TicketCustomModel> tickets = List( model.Email, model.APIKey );

            return tickets?.FirstOrDefault();
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
            using ( UserService uservice = new UserService() )
            {
                User u = uservice.GetById( model.Id );

                if ( u == null )
                {
                    return new ResponseModel() { Code = -1 };
                }

                u.DeviceId = model.DeviceId;

                uservice.Update( u );

                return new ResponseModel() { Code = 1 };
            }
        }
    }
}
