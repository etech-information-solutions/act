using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

using ACT.Core.Enums;
using ACT.Core.Models;
using ACT.Core.Models.Custom;
using ACT.Data.Models;

namespace ACT.Core.Services
{
    public class GroupService : BaseService<Group>, IDisposable
    {
        public GroupService()
        {

        }

        /// <summary>
        /// Gets a count of KPIs matching the specified search params
        /// </summary>
        /// <param name="pm"></param>
        /// <param name="csm"></param>
        /// <returns></returns>
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
                                [dbo].[Group] g";

            // WHERE

            #region WHERE

            query = $"{query} WHERE (1=1)";

            // Limit to only show KPIs for logged in user
            //if ( !CurrentUser.IsAdmin )
            //{
            //    query = $@"{query} AND EXISTS(SELECT 1 FROM [dbo].[PSPUser] pu
            //                                           INNER JOIN [dbo].[PSPClient] pc ON pc.PSPId=pu.PSPId
            //                                  WHERE
            //                                    pc.ClientId=c.Id AND
            //                                    pu.UserId=@userid
            //                                 ) ";
            //}

            #endregion

            // Custom Search

            #region Custom Search

            if ( csm.Status != Status.All )
            {
                query = $"{query} AND g.Status=@csmStatus ";
            }

            if ( csm.FromDate.HasValue && csm.ToDate.HasValue )
            {
                query = $"{query} AND (g.CreatedOn >= @csmFromDate AND g.CreatedOn <= @csmToDate) ";
            }
            else if ( csm.FromDate.HasValue || csm.ToDate.HasValue )
            {
                if ( csm.FromDate.HasValue )
                {
                    query = $"{query} AND (g.CreatedOn>=@csmFromDate) ";
                }
                if ( csm.ToDate.HasValue )
                {
                    query = $"{query} AND (g.CreatedOn<=@csmToDate) ";
                }
            }

            #endregion

            // Normal Search

            #region Normal Search

            if ( !string.IsNullOrEmpty( csm.Query ) )
            {
                query = string.Format( @"{0} AND (
                                                  g.[Name] LIKE '%{1}%' OR
                                                  g.[Description] LIKE '%{1}%'
                                                 ) ", query, csm.Query.Trim() );
            }

            #endregion

            CountModel model = context.Database.SqlQuery<CountModel>( query, parameters.ToArray() ).FirstOrDefault();

            return model.Total;
        }

        /// <summary>
        /// Gets a list of Client KPIs matching the specified search params
        /// </summary>
        /// <param name="pm"></param>
        /// <param name="csm"></param>
        /// <returns></returns>
        public List<GroupCustomModel> List1( PagingModel pm, CustomSearchModel csm )
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
                                g.*,
                                (SELECT COUNT(1) FROM [dbo].[ClientGroup] cg WHERE cg.[GroupId]=g.[Id]) AS [ClientCount]
                            FROM
                                [dbo].[Group] g";

            // WHERE

            #region WHERE

            query = $"{query} WHERE (1=1)";

            // Limit to only show Groups for logged in user
            //if ( !CurrentUser.IsAdmin )
            //{
            //    query = $@"{query} AND EXISTS(SELECT 1 FROM dbo].[PSPClient] pc WHERE EXISTS
            //                                    (
            //                                        SELECT 1 FROM [dbo].[ClientGroup] cg WHERE cg.[ClientId]=pc.[ClientId]
            //                                    )
            //                                 ) ";
            //}

            #endregion

            // Custom Search

            #region Custom Search

            if ( csm.Status != Status.All )
            {
                query = $"{query} AND g.Status=@csmStatus ";
            }

            if ( csm.FromDate.HasValue && csm.ToDate.HasValue )
            {
                query = $"{query} AND (g.CreatedOn >= @csmFromDate AND g.CreatedOn <= @csmToDate) ";
            }
            else if ( csm.FromDate.HasValue || csm.ToDate.HasValue )
            {
                if ( csm.FromDate.HasValue )
                {
                    query = $"{query} AND (g.CreatedOn>=@csmFromDate) ";
                }
                if ( csm.ToDate.HasValue )
                {
                    query = $"{query} AND (g.CreatedOn<=@csmToDate) ";
                }
            }

            #endregion

            // Normal Search

            #region Normal Search

            if ( !string.IsNullOrEmpty( csm.Query ) )
            {
                query = string.Format( @"{0} AND (
                                                  g.[Name] LIKE '%{1}%' OR
                                                  g.[Description] LIKE '%{1}%'
                                                 ) ", query, csm.Query.Trim() );
            }

            #endregion

            // ORDER

            query = $"{query} ORDER BY {pm.SortBy} {pm.Sort}";

            // SKIP, TAKE

            query = string.Format( "{0} OFFSET (@skip) ROWS FETCH NEXT (@take) ROWS ONLY ", query );

            List<GroupCustomModel> groups = context.Database.SqlQuery<GroupCustomModel>( query, parameters.ToArray() ).ToList();

            if ( groups.NullableAny() )
            {
                foreach ( GroupCustomModel g in groups )
                {
                    g.Clients = context.ClientGroups.Where( cg => cg.GroupId == g.Id ).Select( s => s.Client.CompanyName ).ToList();
                }
            }

            return groups;
        }

        /// <summary>
        /// Gets a list of Groups available to the logged in user
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public Dictionary<int, string> List( bool v )
        {
            Dictionary<int, string> pspOptions = new Dictionary<int, string>();
            List<IntStringKeyValueModel> model = new List<IntStringKeyValueModel>();

            List<object> parameters = new List<object>()
            {
                { new SqlParameter( "userid", ( CurrentUser != null ) ? CurrentUser.Id : 0 ) },
            };

            string query = $"SELECT g.Id AS [TKey], g.Description AS [TValue] FROM [dbo].[Group] g WHERE (1=1)";

            //if ( CurrentUser.RoleType == RoleType.PSP )
            //{
            //    query = $"{query} AND EXISTS(SELECT 1 FROM [dbo].[PSPUser] pu WHERE pu.UserId=@userid AND pu.PSPId=r.PSPId)";
            //}

            model = context.Database.SqlQuery<IntStringKeyValueModel>( query.Trim(), parameters.ToArray() ).ToList();

            if ( model != null && model.Any() )
            {
                foreach ( var k in model )
                {
                    if ( pspOptions.Keys.Any( x => x == k.TKey ) )
                        continue;

                    pspOptions.Add( k.TKey, ( k.TValue ?? "" ).Trim() );
                }
            }

            return pspOptions;
        }
    }
}
