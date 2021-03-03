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

        /// <summary>
        /// Gets a dispute using the specified Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public override Dispute GetById( int id )
        {
            context.Configuration.LazyLoadingEnabled = true;
            context.Configuration.ProxyCreationEnabled = true;

            return base.GetById( id );
        }

        /// <summary>
        /// Gets a Dispute using the specified docket number
        /// </summary>
        /// <param name="docketNumber"></param>
        /// <returns></returns>
        public Dispute GetByDocketNumber( string docketNumber )
        {
            return context.Disputes.FirstOrDefault( d => d.DocketNumber == docketNumber );
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
                { new SqlParameter( "csmDisputeReasonId", csm.DisputeReasonId ) },
                { new SqlParameter( "csmDisputeStatus", ( int ) csm.DisputeStatus ) },
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
                                LEFT OUTER JOIN [dbo].[DisputeReason] dr ON dr.[Id]=d.[DisputeReasonId]
                                LEFT OUTER JOIN [dbo].[ChepLoad] cl ON cl.[Id]=d.[ChepLoadId]
                                LEFT OUTER JOIN [dbo].[User] u1 ON u1.[Id]=d.[ActionedById]
                                LEFT OUTER JOIN [dbo].[User] u2 ON u2.[Id]=d.[ResolvedById]";

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

            if ( csm.DisputeStatus != DisputeStatus.All )
            {
                query = $"{query} AND (d.Status=@csmDisputeStatus) ";
            }
            if ( csm.DisputeReasonId != 0 )
            {
                query = $"{query} AND (dr.Id=@csmDisputeReasonId) ";
            }
            if ( csm.ClientId != 0 )
            {
                query = $"{query} AND (cl.[ClientId]=@csmClientId) ";
            }
            else if ( csm.ClientIds.NullableAny() )
            {
                query = $"{query} AND (cl.[ClientId] IN({string.Join( ",", csm.ClientIds )})) ";
            }
            if ( csm.SiteId != 0 )
            {
                query = $"{query} AND EXISTS(SELECT 1 FROM [dbo].[ClientSite] cs WHERE cs.[Id]=cl.[ClientSiteId] AND cs.SiteId=@csmSiteId) ";
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
                                                  u2.[Cell] LIKE '%{1}%' OR
                                                  dr.[Reason] LIKE '%{1}%'
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
                { new SqlParameter( "csmDisputeReasonId", csm.DisputeReasonId ) },
                { new SqlParameter( "csmDisputeStatus", ( int ) csm.DisputeStatus ) },
                { new SqlParameter( "query", csm.Query ?? ( object ) DBNull.Value ) },
                { new SqlParameter( "csmToDate", csm.ToDate ?? ( object ) DBNull.Value ) },
                { new SqlParameter( "userid", ( CurrentUser != null ) ? CurrentUser.Id : 0 ) },
                { new SqlParameter( "csmFromDate", csm.FromDate ?? ( object ) DBNull.Value ) },
            };

            #endregion

            string query = @"SELECT
	                            d.*,
	                            dr.Reason AS [DisputeReasonDetails],
	                            u1.Name + ' ' + u1.Surname AS [ActionUser],
	                            u2.Name + ' ' + u2.Surname AS [ResolvedUser],
	                            (SELECT TOP 1 k.[Disputes] FROM [dbo].[ClientKPI] k WHERE k.[ClientId]=cl.[ClientId]) AS [Disputes]
                             FROM
	                            [dbo].[Dispute] d
                                LEFT OUTER JOIN [dbo].[DisputeReason] dr ON dr.[Id]=d.[DisputeReasonId]
                                LEFT OUTER JOIN [dbo].[ChepLoad] cl ON cl.[Id]=d.[ChepLoadId]
                                LEFT OUTER JOIN [dbo].[User] u1 ON u1.[Id]=d.[ActionedById]
                                LEFT OUTER JOIN [dbo].[User] u2 ON u2.[Id]=d.[ResolvedById]";

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

            if ( csm.DisputeStatus != DisputeStatus.All )
            {
                query = $"{query} AND (d.Status=@csmDisputeStatus) ";
            }
            if ( csm.DisputeReasonId != 0 )
            {
                query = $"{query} AND (dr.Id=@csmDisputeReasonId) ";
            }
            if ( csm.ClientId != 0 )
            {
                query = $"{query} AND (cl.[ClientId]=@csmClientId) ";
            }
            else if ( csm.ClientIds.NullableAny() )
            {
                query = $"{query} AND (cl.[ClientId] IN({string.Join( ",", csm.ClientIds )})) ";
            }
            if ( csm.SiteId != 0 )
            {
                query = $"{query} AND EXISTS(SELECT 1 FROM [dbo].[ClientSite] cs WHERE cs.[Id]=cl.[ClientSiteId] AND cs.SiteId=@csmSiteId) ";
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
                                                  u2.[Cell] LIKE '%{1}%' OR
                                                  dr.[Reason] LIKE '%{1}%'
                                             ) ", query, csm.Query.Trim() );
            }

            #endregion

            // ORDER

            query = $"{query} ORDER BY {pm.SortBy} {pm.Sort}";

            // SKIP, TAKE

            query = string.Format( "{0} OFFSET (@skip) ROWS FETCH NEXT (@take) ROWS ONLY ", query );

            return context.Database.SqlQuery<DisputeCustomModel>( query, parameters.ToArray() ).ToList();
        }

        /// <summary>
        /// Gets the age of outstanding pallets for the specified From-To date as well as other applicable search params
        /// </summary>
        /// <param name="csm"></param>
        /// <param name="isYear"></param>
        /// <returns></returns>
        public NumberOfDisputes NumberOfDisputes( CustomSearchModel csm )
        {
            // Parameters

            #region Parameters

            List<object> parameters = new List<object>()
            {
                { new SqlParameter( "csmSiteId", csm.SiteId ) },
                { new SqlParameter( "csmClientId", csm.ClientId ) },
                { new SqlParameter( "csmGroupId", ( int ) csm.GroupId ) },
                { new SqlParameter( "csmRegionId", ( int ) csm.RegionId ) },
                { new SqlParameter( "csmToDate", csm.ToDate ?? ( object ) DBNull.Value ) },
                { new SqlParameter( "userid", ( CurrentUser != null ) ? CurrentUser.Id : 0 ) },
                { new SqlParameter( "csmFromDate", csm.FromDate ?? ( object ) DBNull.Value ) },
            };

            #endregion


            #region Query

            string query = @"SELECT
	                            COUNT(d.[Id]) AS [Total]
                             FROM
	                            [dbo].[Dispute] d
                                LEFT OUTER JOIN [dbo].[ChepLoad] cl ON cl.[Id]=d.[ChepLoadId]
                                LEFT OUTER JOIN [dbo].[ChepClient] cc ON cc.[ChepLoadsId]=cl.[Id]
	                            LEFT OUTER JOIN [dbo].[ClientLoad] cl1 ON cc.[ClientLoadsId]=cl1.[Id]
                             WHERE
                                (1=1)";

            #endregion

            // WHERE

            #region WHERE

            if ( CurrentUser.RoleType == RoleType.PSP )
            {
                query = $"{query} AND EXISTS(SELECT 1 FROM [dbo].[PSPUser] pu, [dbo].[PSPClient] pc WHERE pu.[PSPId]=pc.[PSPId] AND pc.[ClientId]=cl1.[ClientId] AND pu.[UserId]=@userid) ";
            }
            else if ( CurrentUser.RoleType == RoleType.Client )
            {
                query = $"{query} AND EXISTS(SELECT 1 FROM [dbo].[ClientUser] cu WHERE cu.[ClientId]=cl1.[ClientId] AND cu.[UserId]=@userid) ";
            }

            #endregion

            // CUSTOM SEARCH

            #region Custom Search

            if ( csm.ClientId > 0 )
            {
                query = $"{query} AND (cl.ClientId=@csmClientId) ";
            }

            if ( csm.SiteId > 0 )
            {
                query = $"{query} AND EXISTS(SELECT 1 FROM [dbo].[ClientSite] cs WHERE cs.[ClientId]=cl.[ClientId] AND cs.[SiteId]=@csmSiteId) ";
            }

            if ( csm.GroupId > 0 )
            {
                query = $"{query} AND EXISTS(SELECT 1 FROM [dbo].[ClientGroup] cg WHERE cg.[ClientId]=cl.[ClientId] AND cg.[GroupId]=@csmGroupId) ";
            }

            if ( csm.RegionId > 0 )
            {
                query = $"{query} AND EXISTS(SELECT 1 FROM [dbo].[Site] s, [dbo].[ClientSite] cs WHERE s.[Id]=cs.[SiteId] AND cs.[ClientId]=cl.[ClientId] AND s.[RegionId]=@csmRegionId) ";
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

            return context.Database.SqlQuery<NumberOfDisputes>( query, parameters.ToArray() ).FirstOrDefault();
        }

        /// <summary>
        /// Checks if a dispute with the specified docket number already exists
        /// </summary>
        /// <param name="docketNumber"></param>
        /// <returns></returns>
        public bool Exist( string docketNumber )
        {
            return context.Disputes.Any( d => d.DocketNumber == docketNumber );
        }
    }
}
