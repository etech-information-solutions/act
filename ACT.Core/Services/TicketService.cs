using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

using ACT.Core.Models;
using ACT.Data.Models;

namespace ACT.Core.Services
{
    public class TicketService : BaseService<Ticket>, IDisposable
    {
        public TicketService()
        {

        }

        /// <summary>
        /// Gets a list of Tickets matching the specified search params
        /// </summary>
        /// <param name="pm"></param>
        /// <param name="csm"></param>
        /// <returns></returns>
        public List<TicketCustomModel> List1( PagingModel pm, CustomSearchModel csm )
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
                { new SqlParameter( "csmUserId", csm.UserId ) },
                { new SqlParameter( "csmTicketId", csm.TicketId ) },
                { new SqlParameter( "csmStatus", ( int ) csm.Status ) },
                { new SqlParameter( "query", csm.Query ?? ( object ) DBNull.Value ) },
                { new SqlParameter( "csmToDate", csm.ToDate ?? ( object ) DBNull.Value ) },
                { new SqlParameter( "userid", ( CurrentUser != null ) ? CurrentUser.Id : 0 ) },
                { new SqlParameter( "csmFromDate", csm.FromDate ?? ( object ) DBNull.Value ) },
                { new SqlParameter( "useremail", ( CurrentUser != null ) ? CurrentUser.Email : "" ) },
            };

            #endregion

            string query = @"SELECT
                                t.*,
                                m.[Details] AS [Message],
                                m.[Status] AS [MessageStatus],
                                m.[CreatedOn] AS [MessageCreatedOn],
                                d.[Description] AS [DepartmentName],
                                m.[SenderUserId] AS [MessageSenderUserId],
                                m.[ReceiverUserId] AS [MessageReceiverUserId],
                                u1.[Name] + ' ' + u1.[Surname] AS [OwnerName],
                                u2.[Name] + ' ' + u2.[Surname] AS [SupportName]
                             FROM
                                [dbo].[Ticket] t
                                LEFT OUTER JOIN [dbo].[User] u1 ON u1.[Id]=t.[OwnerUserId]
                                LEFT OUTER JOIN [dbo].[User] u2 ON u2.[Id]=t.[SupportUserId]
                                LEFT OUTER JOIN [dbo].[Department] d ON d.[Id]=t.[DepartmentId]
                                LEFT OUTER JOIN [dbo].[Message] m ON m.[Id]=(SELECT TOP 1 m1.[Id] FROM [dbo].[Message] m1 WHERE m1.[TicketId]=t.[Id] ORDER BY m1.[CreatedOn] DESC)";

            // WHERE

            #region WHERE

            query = $"{query} WHERE (1=1)";

            #endregion

            // Custom Search

            #region Custom Search

            if ( csm.UserId > 0 )
            {
                query = $"{query} AND (t.[OwnerUserId]=@csmUserId OR t.[SupportUserId]=@csmUserId) ";
            }
            if ( csm.TicketId > 0 )
            {
                query = $"{query} AND (t.[Id]=@csmTicketId) ";
            }
            if ( csm.FromDate.HasValue && csm.ToDate.HasValue )
            {
                query = $"{query} AND (t.[CreatedOn] >= @csmFromDate AND t.[CreatedOn] <= @csmToDate) ";
            }
            else if ( csm.FromDate.HasValue || csm.ToDate.HasValue )
            {
                if ( csm.FromDate.HasValue )
                {
                    query = $"{query} AND (t.[CreatedOn] >= @csmFromDate) ";
                }
                if ( csm.ToDate.HasValue )
                {
                    query = $"{query} AND (t.[CreatedOn] <= @csmToDate) ";
                }
            }

            #endregion

            // Normal Search

            #region Normal Search

            if ( !string.IsNullOrEmpty( csm.Query ) )
            {
                query = string.Format( @"{0} AND (t.[Number] LIKE '%{1}%' OR
                                                  m.[Message] LIKE '%{1}%' OR
                                                  u1.[Name] LIKE '%{1}%' OR
                                                  u1.[Surname] LIKE '%{1}%' OR
                                                  u2.[Name] LIKE '%{1}%' OR
                                                  u2.[Surname] LIKE '%{1}%' OR
                                                  d.[Name] LIKE '%{1}%' OR
                                                  d.[Description] LIKE '%{1}%'
                                             ) ", query, csm.Query.Trim() );
            }

            #endregion

            // ORDER

            query = $"{query} ORDER BY {pm.SortBy} {pm.Sort}";

            // SKIP, TAKE

            query = string.Format( "{0} OFFSET (@skip) ROWS FETCH NEXT (@take) ROWS ONLY ", query );

            List<TicketCustomModel> model = context.Database.SqlQuery<TicketCustomModel>( query, parameters.ToArray() ).ToList();

            if ( model.NullableAny() )
            {
                foreach ( TicketCustomModel t in model )
                {
                    t.Messages = context.Messages.Where( m => m.TicketId == t.Id ).ToList();
                }
            }

            return model;
        }
    }
}
