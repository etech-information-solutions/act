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
    public class DisputeService : BaseService<Dispute>, IDisposable
    {
        public DisputeService()
        {

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
                { new SqlParameter( "csmSiteId", csm.SiteId ) },
                { new SqlParameter( "csmClientId", csm.ClientId ) },
                { new SqlParameter( "query", csm.Query ?? ( object ) DBNull.Value ) },
                { new SqlParameter( "csmToDate", csm.ToDate ?? ( object ) DBNull.Value ) },
                { new SqlParameter( "userid", ( CurrentUser != null ) ? CurrentUser.Id : 0 ) },
                { new SqlParameter( "csmFromDate", csm.FromDate ?? ( object ) DBNull.Value ) },
            };

            #endregion

            string query = @"SELECT
	                            COUNT(d.[Id]) AS [Total]
                            FROM
	                            [dbo].[Dispute] d
                                INNER JOIN [dbo].[ChepLoad] cl ON cl.[Id]=d.[ChepLoadId]
                                LEFT OUTER JOIN [dbo].[User] u1 ON u1.[Id]=d.[ActionedBy]";

            // WHERE

            #region WHERE

            query = $"{query} WHERE (1=1)";

            // Limit to only show Disputes for logged in user
            if ( CurrentUser.RoleType == RoleType.PSP )
            {
                query = $@"{query} AND EXISTS(SELECT
                                                1
                                              FROM
                                                [dbo].[PSPUser] pu
                                              WHERE
                                                (p.Id=pu.PSPId) AND
                                                (pu.UserId=@userid) AND
                                                (EXISTS(SELECT 1 FROM [dbo].[PSPClient] pc
                                                        INNER JOIN [dbo].[ClientLoad] cl1 ON cl1.[ClientId]=pc.[ClientId]
                                                        INNER JOIN [dbo].[ChepClient] cc ON (cc.[ClientLoadsId]=cl1.[Id] AND cc.[ChepLoadsId]=cl.[Id])
                                                        WHERE
                                                            (pc.[PSPId]=pu.[PSPId])
                                                       )
                                                )
                                             ) ";
            }
            else if ( CurrentUser.RoleType == RoleType.Client )
            {
                query = $@"{query} AND EXISTS(SELECT
                                                1
                                              FROM
                                                [dbo].[ClientUser] cu 
                                              WHERE
                                                (u.Id=cu.UserId) AND
                                                (cu.UserId=@userid) AND
                                                (EXISTS(SELECT 1 FROM [dbo].[ClientLoad] cl1
                                                        INNER JOIN [dbo].[ChepClient] cc ON (cc.[ClientLoadsId]=cl1.[Id] AND cc.[ChepLoadsId]=cl.[Id])
                                                        WHERE
                                                            (cl1.[ClientId]=cu.[ClientId])
                                                       )
                                                )
                                             ) ";
            }

            #endregion

            // Custom Search

            #region Custom Search

            if ( csm.ClientId != 0 )
            {
                query = $"{query} AND EXISTS(SELECT 1 FROM [dbo].[ClientLoad] cl1, [dbo].[ChepClient] cc WHERE cl1.[Id]=cc.[ClientLoadsId] AND cc.[ChepLoadsId]=cl.[Id] AND cl1.[ClientId]=@csmClientId) ";
            }
            else if ( csm.ClientIds.NullableAny() )
            {
                query = $"{query} AND EXISTS(SELECT 1 FROM [dbo].[ClientLoad] cl1, [dbo].[ChepClient] cc WHERE cl1.[Id]=cc.[ClientLoadsId] AND cc.[ChepLoadsId]=cl.[Id] AND cl1.[ClientId] IN({string.Join( ",", csm.ClientIds )})) ";
            }
            if ( csm.SiteId != 0 )
            {
                query = $"{query} AND EXISTS(SELECT 1 FROM [dbo].[ClientSite] cs, [dbo].[ClientLoad] cl1, [dbo].[ChepClient] cc WHERE cl1.[Id]=cc.[ClientLoadsId] AND cs.[ClientId]=cl1.[ClientId] AND cc.[ChepLoadsId]=cl.[Id] AND cs.Id=@csmSiteId) ";
            }

            if ( csm.FromDate.HasValue && csm.ToDate.HasValue )
            {
                query = $"{query} AND (d.CreatedOn >= @csmFromDate AND d.CreatedOn <= @csmToDate) ";
            }
            else if ( csm.FromDate.HasValue || csm.ToDate.HasValue )
            {
                if ( csm.FromDate.HasValue )
                {
                    query = $"{query} AND (d.CreatedOn>=@csmFromDate) ";
                }
                if ( csm.ToDate.HasValue )
                {
                    query = $"{query} AND (d.CreatedOn<=@csmToDate) ";
                }
            }

            #endregion

            // Normal Search

            #region Normal Search

            if ( !string.IsNullOrEmpty( csm.Query ) )
            {
                query = string.Format( @"{0} AND (d.[DocketNumber] LIKE '%{1}%' OR
                                                  d.[DisputeReason] LIKE '%{1}%' OR
                                                  d.[DisputeEmail] LIKE '%{1}%' OR
                                                  d.[TDNNumber] LIKE '%{1}%' OR
                                                  d.[Reference] LIKE '%{1}%' OR
                                                  d.[Equipment] LIKE '%{1}%' OR
                                                  d.[OtherParty] LIKE '%{1}%' OR
                                                  d.[Sender] LIKE '%{1}%' OR
                                                  d.[Receiver] LIKE '%{1}%' OR
                                                  d.[Declarer] LIKE '%{1}%' OR
                                                  d.[Product] LIKE '%{1}%' OR
                                                  u1.[Name] LIKE '%{1}%' OR
                                                  u1.[Surname] LIKE '%{1}%' OR
                                                  u1.[Email] LIKE '%{1}%' OR
                                                  u1.[Cell] LIKE '%{1}%' OR
                                                  u2.[Name] LIKE '%{1}%' OR
                                                  u2.[Surname] LIKE '%{1}%' OR
                                                  u2.[Email] LIKE '%{1}%' OR
                                                  u2.[Cell] LIKE '%{1}%'
                                             ) ", query, csm.Query.Trim() );
            }

            #endregion

            CountModel model = context.Database.SqlQuery<CountModel>( query, parameters.ToArray() ).FirstOrDefault();

            return model.Total;
        }

        /// <summary>
        /// Gets a list of Client Disputes matching the specified search params
        /// </summary>
        /// <param name="pm"></param>
        /// <param name="csm"></param>
        /// <returns></returns>
        public List<DisputeCustomModel> List1( PagingModel pm, CustomSearchModel csm )
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
                { new SqlParameter( "csmSiteId", csm.SiteId ) },
                { new SqlParameter( "csmClientId", csm.ClientId ) },
                { new SqlParameter( "query", csm.Query ?? ( object ) DBNull.Value ) },
                { new SqlParameter( "csmToDate", csm.ToDate ?? ( object ) DBNull.Value ) },
                { new SqlParameter( "userid", ( CurrentUser != null ) ? CurrentUser.Id : 0 ) },
                { new SqlParameter( "csmFromDate", csm.FromDate ?? ( object ) DBNull.Value ) },
            };

            #endregion

            string query = @"SELECT
	                            d.*,
	                            cl.AccountNumber AS [ChepLoadAccountNumber],
	                            u1.Name + ' ' + u1.Surname AS [ActionUser],
	                            u2.Name + ' ' + u2.Surname AS [ResolvedUser]
                             FROM
	                            [dbo].[Dispute] d
                                INNER JOIN [dbo].[ChepLoad] cl ON cl.[Id]=d.[ChepLoadId]
                                LEFT OUTER JOIN [dbo].[User] u1 ON u1.[Id]=d.[ActionedBy]
                                LEFT OUTER JOIN [dbo].[User] u2 ON u2.[Id]=d.[ResolvedBy]";

            // WHERE

            #region WHERE

            query = $"{query} WHERE (1=1)";

            // Limit to only show Disputes for logged in user
            if ( CurrentUser.RoleType == RoleType.PSP )
            {
                query = $@"{query} AND EXISTS(SELECT
                                                1
                                              FROM
                                                [dbo].[PSPUser] pu
                                              WHERE
                                                (p.Id=pu.PSPId) AND
                                                (pu.UserId=@userid) AND
                                                (EXISTS(SELECT 1 FROM [dbo].[PSPClient] pc
                                                        INNER JOIN [dbo].[ClientLoad] cl1 ON cl1.[ClientId]=pc.[ClientId]
                                                        INNER JOIN [dbo].[ChepClient] cc ON (cc.[ClientLoadsId]=cl1.[Id] AND cc.[ChepLoadsId]=cl.[Id])
                                                        WHERE
                                                            (pc.[PSPId]=pu.[PSPId])
                                                       )
                                                )
                                             ) ";
            }
            else if ( CurrentUser.RoleType == RoleType.Client )
            {
                query = $@"{query} AND EXISTS(SELECT
                                                1
                                              FROM
                                                [dbo].[ClientUser] cu 
                                              WHERE
                                                (u.Id=cu.UserId) AND
                                                (cu.UserId=@userid) AND
                                                (EXISTS(SELECT 1 FROM [dbo].[ClientLoad] cl1
                                                        INNER JOIN [dbo].[ChepClient] cc ON (cc.[ClientLoadsId]=cl1.[Id] AND cc.[ChepLoadsId]=cl.[Id])
                                                        WHERE
                                                            (cl1.[ClientId]=cu.[ClientId])
                                                       )
                                                )
                                             ) ";
            }

            #endregion

            // Custom Search

            #region Custom Search

            if ( csm.ClientId != 0 )
            {
                query = $"{query} AND EXISTS(SELECT 1 FROM [dbo].[ClientLoad] cl1, [dbo].[ChepClient] cc WHERE cl1.[Id]=cc.[ClientLoadsId] AND cc.[ChepLoadsId]=cl.[Id] AND cl1.[ClientId]=@csmClientId) ";
            }
            else if ( csm.ClientIds.NullableAny() )
            {
                query = $"{query} AND EXISTS(SELECT 1 FROM [dbo].[ClientLoad] cl1, [dbo].[ChepClient] cc WHERE cl1.[Id]=cc.[ClientLoadsId] AND cc.[ChepLoadsId]=cl.[Id] AND cl1.[ClientId] IN({string.Join( ",", csm.ClientIds )})) ";
            }
            if ( csm.SiteId != 0 )
            {
                query = $"{query} AND EXISTS(SELECT 1 FROM [dbo].[ClientSite] cs, [dbo].[ClientLoad] cl1, [dbo].[ChepClient] cc WHERE cl1.[Id]=cc.[ClientLoadsId] AND cs.[ClientId]=cl1.[ClientId] AND cc.[ChepLoadsId]=cl.[Id] AND cs.Id=@csmSiteId) ";
            }

            if ( csm.FromDate.HasValue && csm.ToDate.HasValue )
            {
                query = $"{query} AND (d.CreatedOn >= @csmFromDate AND d.CreatedOn <= @csmToDate) ";
            }
            else if ( csm.FromDate.HasValue || csm.ToDate.HasValue )
            {
                if ( csm.FromDate.HasValue )
                {
                    query = $"{query} AND (d.CreatedOn>=@csmFromDate) ";
                }
                if ( csm.ToDate.HasValue )
                {
                    query = $"{query} AND (d.CreatedOn<=@csmToDate) ";
                }
            }

            #endregion

            // Normal Search

            #region Normal Search

            if ( !string.IsNullOrEmpty( csm.Query ) )
            {
                query = string.Format( @"{0} AND (d.[DocketNumber] LIKE '%{1}%' OR
                                                  d.[DisputeReason] LIKE '%{1}%' OR
                                                  d.[DisputeEmail] LIKE '%{1}%' OR
                                                  d.[TDNNumber] LIKE '%{1}%' OR
                                                  d.[Reference] LIKE '%{1}%' OR
                                                  d.[Equipment] LIKE '%{1}%' OR
                                                  d.[OtherParty] LIKE '%{1}%' OR
                                                  d.[Sender] LIKE '%{1}%' OR
                                                  d.[Receiver] LIKE '%{1}%' OR
                                                  d.[Declarer] LIKE '%{1}%' OR
                                                  d.[Product] LIKE '%{1}%' OR
                                                  u1.[Name] LIKE '%{1}%' OR
                                                  u1.[Surname] LIKE '%{1}%' OR
                                                  u1.[Email] LIKE '%{1}%' OR
                                                  u1.[Cell] LIKE '%{1}%' OR
                                                  u2.[Name] LIKE '%{1}%' OR
                                                  u2.[Surname] LIKE '%{1}%' OR
                                                  u2.[Email] LIKE '%{1}%' OR
                                                  u2.[Cell] LIKE '%{1}%'
                                             ) ", query, csm.Query.Trim() );
            }

            #endregion

            // ORDER

            query = $"{query} ORDER BY {pm.SortBy} {pm.Sort}";

            // SKIP, TAKE

            query = string.Format( "{0} OFFSET (@skip) ROWS FETCH NEXT (@take) ROWS ONLY ", query );

            return context.Database.SqlQuery<DisputeCustomModel>( query, parameters.ToArray() ).ToList();
        }
    }
}
