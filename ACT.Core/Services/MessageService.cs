using System;
using System.Collections.Generic;
using System.Linq;

using ACT.Core.Models;
using ACT.Data.Models;

namespace ACT.Core.Services
{
    public class MessageService : BaseService<Message>, IDisposable
    {
        public MessageService()
        {

        }

        public List<Message> Receive( int ticketId )
        {
            return context.Messages.Where( m => m.TicketId == ticketId ).ToList();
        }
    }
}
