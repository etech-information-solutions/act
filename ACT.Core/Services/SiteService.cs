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
    public class SiteService : BaseService<Site>, IDisposable
    {
        public SiteService()
        {

        }

        /// <summary>
        /// Gets a site using the specified Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public override Site GetById( int id )
        {
            context.Configuration.LazyLoadingEnabled = true;
            context.Configuration.ProxyCreationEnabled = true;

            return base.GetById( id );
        }

        /// <summary>
        /// Gets a list of clients
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public Dictionary<int, string> List( bool v, int regionId = 0 )
        {
            Dictionary<int, string> siteOptions = new Dictionary<int, string>();
            List<IntStringKeyValueModel> model = new List<IntStringKeyValueModel>();

            List<object> parameters = new List<object>()
            {
                { new SqlParameter( "regionId", regionId ) },
                { new SqlParameter( "sAct", Status.Active ) },
                { new SqlParameter( "userid", ( CurrentUser != null ) ? CurrentUser.Id : 0 ) },
            };

            string query = string.Empty;

            query = $"SELECT s.Id AS [TKey], s.Name AS [TValue] FROM [dbo].[Site] s WHERE (s.[Status]=@sAct)";

            if ( regionId > 0 )
            {
                query = $"{query} AND (s.[RegionId]=@regionId)";
            }

            if ( CurrentUser.RoleType == RoleType.PSP )
            {
                query = $"{query} AND EXISTS(SELECT 1 FROM [dbo].[PSPUser] pu, [dbo].[Region] r WHERE r.Id=s.RegionId AND r.PSPId=pu.PSPId AND pu.UserId=@userid)";
            }
            else if ( CurrentUser.RoleType == RoleType.Client )
            {
                query = $"{query} AND EXISTS(SELECT 1 FROM [dbo].[ClientUser] cu, [dbo].[PSPClient] pc, [dbo].[Region] r WHERE r.Id=s.RegionId AND r.PSPId=pc.PSPId AND cu.ClientId=pc.ClientId AND cu.UserId=@userid)";
            }

            model = context.Database.SqlQuery<IntStringKeyValueModel>( query.Trim(), parameters.ToArray() ).ToList();

            if ( model != null && model.Any() )
            {
                foreach ( var k in model )
                {
                    if ( siteOptions.Keys.Any( x => x == k.TKey ) )
                        continue;

                    siteOptions.Add( k.TKey, ( k.TValue ?? "" ).Trim() );
                }
            }

            return siteOptions;
        }

        /// <summary>
        /// Gets a total count of Sites matching the specified search params
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
                { new SqlParameter( "csmSiteId", csm.SiteId ) },
                { new SqlParameter( "csmClientId", csm.ClientId ) },
                { new SqlParameter( "csmRegionId", csm.RegionId ) },
                { new SqlParameter( "query", csm.Query ?? ( object ) DBNull.Value ) },
                { new SqlParameter( "csmToDate", csm.ToDate ?? ( object ) DBNull.Value ) },
                { new SqlParameter( "userid", ( CurrentUser != null ) ? CurrentUser.Id : 0 ) },
                { new SqlParameter( "csmFromDate", csm.FromDate ?? ( object ) DBNull.Value ) },

            };

            #endregion

            string query = @"SELECT
                                COUNT(1) AS [Total]
                             FROM
                                [dbo].[Site] s
                                LEFT OUTER JOIN [dbo].[Region] r ON r.[Id]=s.[RegionId]";

            // WHERE

            #region WHERE

            query = $"{query} WHERE (1=1)";

            // Limit to only show Sites for logged in user
            if ( !CurrentUser.IsAdmin )
            {
                query = $@"{query} AND EXISTS(SELECT 1 FROM [dbo].[PSPUser] pu
                                                       INNER JOIN [dbo].[PSPClient] pc ON pc.PSPId=pu.PSPId
                                                       LEFT OUTER JOIN [dbo].[ClientSite] cs ON pc.ClientId=cs.ClientId
                                              WHERE
                                                cs.SiteId=s.Id AND
                                                pu.UserId=@userid
                                             ) ";
            }

            #endregion

            // Custom Search

            #region Custom Search

            if ( csm.ClientId != 0 )
            {
                query = $"{query} AND EXISTS(SELECT 1 FROM [dbo].[ClientSite] cs, [dbo].[ClientCustomer] cc WHERE s.Id=cs.SiteId AND cc.Id=cs.ClientCustomerId AND cc.ClientId=@csmClientId) ";
            }
            if ( csm.SiteId != 0 )
            {
                query = $"{query} AND s.SiteId=@csmSiteId ";
            }
            if ( csm.RegionId != 0 )
            {
                query = $"{query} AND s.RegionId=@csmRegionId ";
            }

            if ( csm.FromDate.HasValue && csm.ToDate.HasValue )
            {
                query = $"{query} AND (s.CreatedOn >= @csmFromDate AND s.CreatedOn <= @csmToDate) ";
            }
            else if ( csm.FromDate.HasValue || csm.ToDate.HasValue )
            {
                if ( csm.FromDate.HasValue )
                {
                    query = $"{query} AND (s.CreatedOn>=@csmFromDate) ";
                }
                if ( csm.ToDate.HasValue )
                {
                    query = $"{query} AND (s.CreatedOn<=@csmToDate) ";
                }
            }

            #endregion

            // Normal Search

            #region Normal Search

            if ( !string.IsNullOrEmpty( csm.Query ) )
            {
                query = string.Format( @"{0} AND (s.[Name] LIKE '%{1}%' OR
                                                  s.[Description] LIKE '%{1}%' OR
                                                  s.[XCord] LIKE '%{1}%' OR
                                                  s.[YCord] LIKE '%{1}%' OR
                                                  s.[Address] LIKE '%{1}%' OR
                                                  s.[AccountCode] LIKE '%{1}%' OR
                                                  s.[ContactNo] LIKE '%{1}%' OR
                                                  s.[ContactName] LIKE '%{1}%' OR
                                                  s.[Depot] LIKE '%{1}%' OR
                                                  s.[SiteCodeChep] LIKE '%{1}%' OR
                                                  s.[PlanningPoint] LIKE '%{1}%' OR
                                                  s.[FinanceContact] LIKE '%{1}%' OR
                                                  s.[FinanceContactNo] LIKE '%{1}%' OR
                                                  s.[ReceivingContact] LIKE '%{1}%' OR
                                                  s.[ReceivingContactNo] LIKE '%{1}%' OR
                                                  r.[Name] LIKE '%{1}%' OR
                                                  r.[Description] LIKE '%{1}%'
                                             ) ", query, csm.Query.Trim() );
            }

            #endregion

            CountModel model = context.Database.SqlQuery<CountModel>( query, parameters.ToArray() ).FirstOrDefault();

            return model.Total;
        }

        /// <summary>
        /// Gets a list of Sites matching the specified search params
        /// </summary>
        /// <param name="pm"></param>
        /// <param name="csm"></param>
        /// <returns></returns>
        public List<SiteCustomModel> List1( PagingModel pm, CustomSearchModel csm )
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
                { new SqlParameter( "csmRegionId", csm.RegionId ) },
                { new SqlParameter( "query", csm.Query ?? ( object ) DBNull.Value ) },
                { new SqlParameter( "csmToDate", csm.ToDate ?? ( object ) DBNull.Value ) },
                { new SqlParameter( "userid", ( CurrentUser != null ) ? CurrentUser.Id : 0 ) },
                { new SqlParameter( "csmFromDate", csm.FromDate ?? ( object ) DBNull.Value ) },

            };

            #endregion

            string query = @"SELECT
                                s.*,
                                r.[Description] AS [RegionName],
                                (SELECT COUNT (1) FROM [dbo].[Site] s1 WHERE s1.[SiteId]=s.[Id]) AS [SubSiteCount],
                                (SELECT COUNT (1) FROM [dbo].[ClientSite] cs WHERE cs.[SiteId]=s.[Id]) AS [ClientCount],
                                (SELECT COUNT (1) FROM [dbo].[SiteBudget] sb WHERE sb.[SiteId]=s.[Id]) AS [BudgetCount]
                             FROM
                                [dbo].[Site] s
                                LEFT OUTER JOIN [dbo].[Region] r ON r.[Id]=s.[RegionId]";

            // WHERE

            #region WHERE

            query = $"{query} WHERE (1=1)";

            // Limit to only show Sites for logged in user 
            if ( !CurrentUser.IsAdmin )
            {
                query = $@"{query} AND EXISTS(SELECT 1 FROM [dbo].[PSPUser] pu
                                                       INNER JOIN [dbo].[PSPClient] pc ON pc.PSPId=pu.PSPId
                                                       LEFT OUTER JOIN [dbo].[ClientSite] cs ON pc.ClientId=cs.ClientId
                                              WHERE
                                                cs.SiteId=s.Id AND
                                                pu.UserId=@userid
                                             ) ";
            }

            #endregion

            // Custom Search

            #region Custom Search

            if ( csm.ClientId != 0 )
            {
                query = $"{query} AND EXISTS(SELECT 1 FROM [dbo].[ClientSite] cs, [dbo].[ClientCustomer] cc WHERE s.Id=cs.SiteId AND cc.Id=cs.ClientCustomerId AND cc.ClientId=@csmClientId) ";
            }
            if ( csm.SiteId != 0 )
            {
                query = $"{query} AND s.SiteId=@csmSiteId ";
            }
            if ( csm.RegionId != 0 )
            {
                query = $"{query} AND s.RegionId=@csmRegionId ";
            }

            if ( csm.FromDate.HasValue && csm.ToDate.HasValue )
            {
                query = $"{query} AND (s.CreatedOn >= @csmFromDate AND s.CreatedOn <= @csmToDate) ";
            }
            else if ( csm.FromDate.HasValue || csm.ToDate.HasValue )
            {
                if ( csm.FromDate.HasValue )
                {
                    query = $"{query} AND (s.CreatedOn>=@csmFromDate) ";
                }
                if ( csm.ToDate.HasValue )
                {
                    query = $"{query} AND (s.CreatedOn<=@csmToDate) ";
                }
            }

            #endregion

            // Normal Search

            #region Normal Search

            if ( !string.IsNullOrEmpty( csm.Query ) )
            {
                query = string.Format( @"{0} AND (s.[Name] LIKE '%{1}%' OR
                                                  s.[Description] LIKE '%{1}%' OR
                                                  s.[XCord] LIKE '%{1}%' OR
                                                  s.[YCord] LIKE '%{1}%' OR
                                                  s.[Address] LIKE '%{1}%' OR
                                                  s.[AccountCode] LIKE '%{1}%' OR
                                                  s.[ContactNo] LIKE '%{1}%' OR
                                                  s.[ContactName] LIKE '%{1}%' OR
                                                  s.[Depot] LIKE '%{1}%' OR
                                                  s.[SiteCodeChep] LIKE '%{1}%' OR
                                                  s.[PlanningPoint] LIKE '%{1}%' OR
                                                  s.[FinanceContact] LIKE '%{1}%' OR
                                                  s.[FinanceContactNo] LIKE '%{1}%' OR
                                                  s.[ReceivingContact] LIKE '%{1}%' OR
                                                  s.[ReceivingContactNo] LIKE '%{1}%' OR
                                                  r.[Name] LIKE '%{1}%' OR
                                                  r.[Description] LIKE '%{1}%'
                                             ) ", query, csm.Query.Trim() );
            }

            #endregion

            // ORDER

            query = $"{query} ORDER BY {pm.SortBy} {pm.Sort}";

            // SKIP, TAKE

            query = string.Format( "{0} OFFSET (@skip) ROWS FETCH NEXT (@take) ROWS ONLY ", query );

            List<SiteCustomModel> model = context.Database.SqlQuery<SiteCustomModel>( query, parameters.ToArray() ).ToList();

            if ( model.NullableAny( p => p.ClientCount > 0 ) )
            {
                foreach ( SiteCustomModel item in model.Where( p => p.ClientCount > 0 ) )
                {
                    item.Clients = context.ClientSites.Where( cs => cs.SiteId == item.Id ).Select( s => s.ClientCustomer.CustomerName ).ToList();
                }
            }

            if ( model.NullableAny( p => p.SubSiteCount > 0 ) )
            {
                foreach ( SiteCustomModel item in model.Where( p => p.SubSiteCount > 0 ) )
                {
                    item.SubSites = context.Sites.Where( s => s.SiteId == item.Id ).Select( s => s.Name ).ToList();
                }
            }

            return model;
        }

        public int GetClientBySite( int siteId )
        {
            int clientId;
            clientId = ( from c in context.ClientSites
                         where c.SiteId == siteId
                         where c.Status == ( int ) Status.Active
                         select c.ClientCustomerId ).FirstOrDefault();


            return clientId;
        }

        /// <summary>
        /// Checks if a site already exists using the specified XY Coordinates
        /// </summary>
        /// <param name="xcoord"></param>
        /// <param name="ycoord"></param>
        /// <returns></returns>
        public Site ExistByXYCoords( string xcoord, string ycoord )
        {
            return context.Sites.FirstOrDefault( c => c.XCord == xcoord && c.YCord == ycoord );
        }

        /// <summary>
        /// Checks if a site with the same name already exists
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool ExistByNameRegion( string name, int? regionId )
        {
            return context.Sites.Any( s => s.Name.Trim().ToLower() == name.Trim().ToLower() && s.RegionId == regionId );
        }

        public Site GetByXYCoordinates( string xCord, string yCord )
        {
            return context.Sites.FirstOrDefault( s => s.XCord == xCord && s.YCord == yCord );
        }

        /// <summary>
        /// Gets a site using the specified site code
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public Site GetBySiteCode( string code )
        {
            return context.Sites.Include( "ClientSites" ).FirstOrDefault( s => s.SiteCodeChep.Trim() == code.Trim() );
        }
    }
}
