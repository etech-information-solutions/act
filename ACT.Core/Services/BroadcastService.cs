using System;
using System.Linq;
using System.Collections.Generic;
using ACT.Core.Models;
using ACT.Data.Models;
using ACT.Core.Models.Custom;
using ACT.Core.Enums;
using System.Data.SqlClient;

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

            return base.List();
        }

        public int Total1( PagingModel pm, CustomSearchModel csm )
        {
            if ( csm.FromDate.HasValue && csm.ToDate.HasValue && csm.FromDate?.Date == csm.ToDate?.Date )
            {
                csm.ToDate = csm.ToDate?.AddDays( 1 );
            }

            // Parameters

            #region Parameters

            List<object> parameters = new List<object>()
            {
                { new SqlParameter( "skip", pm.Skip ) },
                { new SqlParameter( "take", pm.Take ) },
                { new SqlParameter( "csmStatus", ( int ) csm.Status ) },
                { new SqlParameter( "query", csm.Query ?? ( object ) DBNull.Value ) },
                { new SqlParameter( "csmToDate", csm.ToDate ?? ( object ) DBNull.Value ) },
                { new SqlParameter( "userid", ( CurrentUser != null ) ? CurrentUser.Id : 0 ) },
                { new SqlParameter( "csmFromDate", csm.FromDate ?? ( object ) DBNull.Value ) },
            };

            #endregion

            string query = @"SELECT
                                COUNT(1) AS [Total]
                             FROM
                                [dbo].[Broadcast] b";

            // WHERE

            #region WHERE

            query = $"{query} WHERE (1=1)";

            // Limit to only show PSP for logged in user
            if ( CurrentUser.RoleType == RoleType.PSP )
            {
                query = $"{query} AND (b.ObjectType='PSP' AND b.ObjectId IN({string.Join( ",", CurrentUser.PSPs.Select( s => s.Id ) )})) ";
            }
            else if ( CurrentUser.RoleType == RoleType.Client )
            {
                query = $"{query} AND (b.ObjectType='Client' AND b.ObjectId IN({string.Join( ",", CurrentUser.Clients.Select( s => s.Id ) )})) ";
            }

            #endregion

            // Custom Search

            #region Custom Search

            if ( csm.Status != Status.All )
            {
                query = $"{query} AND (b.Status=@csmStatus) ";
            }

            if ( csm.FromDate.HasValue && csm.ToDate.HasValue )
            {
                query = $"{query} AND (b.CreatedOn >= @csmFromDate AND b.CreatedOn <= @csmToDate) ";
            }
            else if ( csm.FromDate.HasValue || csm.ToDate.HasValue )
            {
                if ( csm.FromDate.HasValue )
                {
                    query = $"{query} AND (b.CreatedOn>=@csmFromDate) ";
                }
                if ( csm.ToDate.HasValue )
                {
                    query = $"{query} AND (b.CreatedOn<=@csmToDate) ";
                }
            }

            #endregion

            // Normal Search

            #region Normal Search

            if ( !string.IsNullOrEmpty( csm.Query ) )
            {
                query = string.Format( @"{0} AND (b.[Message] LIKE '%{1}%') ", query, csm.Query.Trim() );
            }

            #endregion

            CountModel model = context.Database.SqlQuery<CountModel>( query, parameters.ToArray() ).FirstOrDefault();

            return model.Total;
        }

        public List<BroadcastCustomModel> List1( PagingModel pm, CustomSearchModel csm )
        {
            if ( csm.FromDate.HasValue && csm.ToDate.HasValue && csm.FromDate?.Date == csm.ToDate?.Date )
            {
                csm.ToDate = csm.ToDate?.AddDays( 1 );
            }

            // Parameters

            #region Parameters

            List<object> parameters = new List<object>()
            {
                { new SqlParameter( "skip", pm.Skip ) },
                { new SqlParameter( "take", pm.Take ) },
                { new SqlParameter( "csmStatus", ( int ) csm.Status ) },
                { new SqlParameter( "query", csm.Query ?? ( object ) DBNull.Value ) },
                { new SqlParameter( "csmToDate", csm.ToDate ?? ( object ) DBNull.Value ) },
                { new SqlParameter( "userid", ( CurrentUser != null ) ? CurrentUser.Id : 0 ) },
                { new SqlParameter( "csmFromDate", csm.FromDate ?? ( object ) DBNull.Value ) },
            };

            #endregion

            string query = @"SELECT
                                b.*,
                                (SELECT COUNT(1) FROM [dbo].[UserBroadcast] ub WHERE b.Id=ub.[BroadcastId]) AS [XRead],
                                (SELECT p.CompanyName + ' (PSP)' FROM [dbo].[PSP] p WHERE p.Id=b.[ObjectId] AND b.ObjectType='PSP') AS [PSPName],
                                (SELECT c.CompanyName + ' (Client)' FROM [dbo].[Client] c WHERE c.Id=b.[ObjectId] AND b.ObjectType='Client') AS [ClientName]
                             FROM
                                [dbo].[Broadcast] b";

            // WHERE

            #region WHERE

            query = $"{query} WHERE (1=1)";

            // Limit to only show PSP for logged in user
            if ( CurrentUser.RoleType == RoleType.PSP )
            {
                query = $"{query} AND (b.ObjectType='PSP' AND b.ObjectId IN({string.Join( ",", CurrentUser.PSPs.Select( s => s.Id ) )})) ";
            }
            else if ( CurrentUser.RoleType == RoleType.Client )
            {
                query = $"{query} AND (b.ObjectType='Client' AND b.ObjectId IN({string.Join( ",", CurrentUser.Clients.Select( s => s.Id ) )})) ";
            }

            #endregion

            // Custom Search

            #region Custom Search

            if ( csm.Status != Status.All )
            {
                query = $"{query} AND (b.Status=@csmStatus) ";
            }

            if ( csm.FromDate.HasValue && csm.ToDate.HasValue )
            {
                query = $"{query} AND (b.CreatedOn >= @csmFromDate AND b.CreatedOn <= @csmToDate) ";
            }
            else if ( csm.FromDate.HasValue || csm.ToDate.HasValue )
            {
                if ( csm.FromDate.HasValue )
                {
                    query = $"{query} AND (b.CreatedOn>=@csmFromDate) ";
                }
                if ( csm.ToDate.HasValue )
                {
                    query = $"{query} AND (b.CreatedOn<=@csmToDate) ";
                }
            }

            #endregion

            // Normal Search

            #region Normal Search

            if ( !string.IsNullOrEmpty( csm.Query ) )
            {
                query = string.Format( @"{0} AND (b.[Message] LIKE '%{1}%') ", query, csm.Query.Trim() );
            }

            #endregion

            // ORDER

            query = $"{query} ORDER BY {pm.SortBy} {pm.Sort}";

            // SKIP, TAKE

            query = string.Format( "{0} OFFSET (@skip) ROWS FETCH NEXT (@take) ROWS ONLY ", query );

            return context.Database.SqlQuery<BroadcastCustomModel>( query, parameters.ToArray() ).ToList();
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
            List<int> pspIds = CurrentUser.PSPs.Select( s => s.Id ).ToList();
            List<int> clientIds = CurrentUser.Clients.Select( s => s.Id ).ToList();

            return context.Broadcasts.FirstOrDefault( b =>
                                                        (
                                                            ( CurrentUser.RoleType == RoleType.PSP ? ( b.ObjectId == null || ( b.ObjectType == "PSP" && pspIds.Contains( b.ObjectId.Value ) ) ) : true ) &&
                                                            ( CurrentUser.RoleType == RoleType.Client ? ( b.ObjectId == null || ( b.ObjectType == "Client" && clientIds.Contains( b.ObjectId.Value ) ) ) : true ) &&
                                                            ( CurrentUser.RoleType != RoleType.PSP && CurrentUser.RoleType != RoleType.Client ? b.ObjectId == null : true )
                                                        ) &&
                                                        ( DateTime.Now > b.StartDate ) &&
                                                        ( !b.EndDate.HasValue || b.EndDate <= DateTime.Now ) &&
                                                        ( !b.UserBroadcasts.Any( ub => ub.UserId == userid ) )
                                                    );
        }
    }
}
