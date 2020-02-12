using System;
using System.Linq;
using System.Collections.Generic;
using ACT.Core.Models;
using ACT.Data.Models;

namespace ACT.Core.Services
{
    public class BroadcastService : BaseService<Broadcast>, IDisposable
    {
        public BroadcastService()
        {

        }

        public override Broadcast GetById( int id )
        {
            base.context.Configuration.LazyLoadingEnabled = true;
            base.context.Configuration.ProxyCreationEnabled = true;

            return base.GetById( id );
        }

        public override List<Broadcast> List( PagingModel pm, CustomSearchModel csm )
        {
            base.context.Configuration.LazyLoadingEnabled = true;
            base.context.Configuration.ProxyCreationEnabled = true;

            return base.List( pm, csm );
        }

        /// <summary>
        /// Checks if a broadcast whose end date is less/equal specified end date already exists...
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool Exist( DateTime startDate )
        {
            return context.Broadcasts.Any( b => b.EndDate > startDate );
        }

        /// <summary>
        /// Gets a broadcast that has not yet been read by the user...
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        public Broadcast GetReadByUser( int userid )
        {
            return context.Broadcasts.FirstOrDefault( b => b.UserBroadcasts.Any( ub => ub.UserId == userid ) );
        }

        /// <summary>
        /// Gets a broadcast that has not yet been read by the user...
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        public Broadcast GetUnreadByUser( int userid )
        {
            return context.Broadcasts.FirstOrDefault( b => ( !b.EndDate.HasValue || b.EndDate <= DateTime.Now ) && DateTime.Now > b.StartDate && !b.UserBroadcasts.Any( ub => ub.UserId == userid ) );
        }
    }
}
