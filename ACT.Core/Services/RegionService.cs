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
    public class RegionService : BaseService<Region>, IDisposable
    {
        public RegionService()
        {

        }

        public override Region GetById( int id )
        {
            context.Configuration.LazyLoadingEnabled = true;
            context.Configuration.ProxyCreationEnabled = true;

            return base.GetById( id );
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
                { new SqlParameter( "csmPSPId", csm.PSPId ) },
                { new SqlParameter( "query", csm.Query ?? ( object ) DBNull.Value ) },
                { new SqlParameter( "csmToDate", csm.ToDate ?? ( object ) DBNull.Value ) },
                { new SqlParameter( "userid", ( CurrentUser != null ) ? CurrentUser.Id : 0 ) },
                { new SqlParameter( "csmFromDate", csm.FromDate ?? ( object ) DBNull.Value ) },
            };

            #endregion

            string query = @"SELECT
                                COUNT(1) AS [Total]
                             FROM
                                [dbo].[Region] r
                                LEFT OUTER JOIN [dbo].[User] u ON u.Id=r.RegionManagerId";

            // WHERE

            #region WHERE

            query = $"{query} WHERE (1=1)";

            // Limit to only show PSP for logged in user
            if ( CurrentUser.RoleType == RoleType.PSP )
            {
                query = $"{query} AND EXISTS(SELECT 1 FROM [dbo].[PSPUser] pu WHERE r.PSPId=pu.PSPId AND pu.UserId=@userid) ";
            }

            #endregion

            // Custom Search

            #region Custom Search

            if ( csm.PSPId != 0 )
            {
                query = $"{query} AND r.PSPId=@csmPSPId ";
            }

            if ( csm.FromDate.HasValue && csm.ToDate.HasValue )
            {
                query = $"{query} AND (r.CreatedOn >= @csmFromDate AND r.CreatedOn <= @csmToDate) ";
            }
            else if ( csm.FromDate.HasValue || csm.ToDate.HasValue )
            {
                if ( csm.FromDate.HasValue )
                {
                    query = $"{query} AND (r.CreatedOn>=@csmFromDate) ";
                }
                if ( csm.ToDate.HasValue )
                {
                    query = $"{query} AND (r.CreatedOn<=@csmToDate) ";
                }
            }

            #endregion

            // Normal Search

            #region Normal Search

            if ( !string.IsNullOrEmpty( csm.Query ) )
            {
                query = string.Format( @"{0} AND (r.[Name] LIKE '%{1}%' OR
                                                  r.[Description] LIKE '%{1}%' OR
                                                  u.[Name] LIKE '%{1}%' OR
                                                  u.[Surname] LIKE '%{1}%' OR
                                                  u.[Email] LIKE '%{1}%'
                                                 ) ", query, csm.Query.Trim() );
            }

            #endregion

            CountModel model = context.Database.SqlQuery<CountModel>( query, parameters.ToArray() ).FirstOrDefault();

            return model.Total;
        }

        /// <summary>
        /// Gets a list of PSPs matching the specified search params
        /// </summary>
        /// <param name="pm"></param>
        /// <param name="csm"></param>
        /// <returns></returns>
        public List<RegionCustomModel> ListCSM( PagingModel pm, CustomSearchModel csm )
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
                { new SqlParameter( "csmPSPId", csm.PSPId ) },
                { new SqlParameter( "query", csm.Query ?? ( object ) DBNull.Value ) },
                { new SqlParameter( "csmToDate", csm.ToDate ?? ( object ) DBNull.Value ) },
                { new SqlParameter( "userid", ( CurrentUser != null ) ? CurrentUser.Id : 0 ) },
                { new SqlParameter( "csmFromDate", csm.FromDate ?? ( object ) DBNull.Value ) },
            };

            #endregion

            string query = @"SELECT
                                r.*,
                                p.CompanyName AS [PSPName],
                                u.Email AS [RegionManagerEmail],
                                u.Name + ' ' + u.Surname AS [RegionManagerName]
                             FROM
                                [dbo].[Region] r
                                INNER JOIN [dbo].[PSP] p ON p.Id=r.PSPId
                                LEFT OUTER JOIN [dbo].[User] u ON u.Id=r.RegionManagerId";

            // WHERE

            #region WHERE

            query = $"{query} WHERE (1=1)";

            // Limit to only show PSP for logged in user
            if ( CurrentUser.RoleType == RoleType.PSP )
            {
                query = $"{query} AND EXISTS(SELECT 1 FROM [dbo].[PSPUser] pu WHERE r.PSPId=pu.PSPId AND pu.UserId=@userid) ";
            }

            #endregion

            // Custom Search

            #region Custom Search

            if ( csm.PSPId != 0 )
            {
                query = $"{query} AND r.PSPId=@csmPSPId ";
            }

            if ( csm.FromDate.HasValue && csm.ToDate.HasValue )
            {
                query = $"{query} AND (r.CreatedOn >= @csmFromDate AND r.CreatedOn <= @csmToDate) ";
            }
            else if ( csm.FromDate.HasValue || csm.ToDate.HasValue )
            {
                if ( csm.FromDate.HasValue )
                {
                    query = $"{query} AND (r.CreatedOn>=@csmFromDate) ";
                }
                if ( csm.ToDate.HasValue )
                {
                    query = $"{query} AND (r.CreatedOn<=@csmToDate) ";
                }
            }

            #endregion

            // Normal Search

            #region Normal Search

            if ( !string.IsNullOrEmpty( csm.Query ) )
            {
                query = string.Format( @"{0} AND (r.[Name] LIKE '%{1}%' OR
                                                  r.[Description] LIKE '%{1}%' OR
                                                  u.[Name] LIKE '%{1}%' OR
                                                  u.[Surname] LIKE '%{1}%' OR
                                                  u.[Email] LIKE '%{1}%'
                                                 ) ", query, csm.Query.Trim() );
            }

            #endregion

            // ORDER

            query = $"{query} ORDER BY {pm.SortBy} {pm.Sort}";

            // SKIP, TAKE

            query = string.Format( "{0} OFFSET (@skip) ROWS FETCH NEXT (@take) ROWS ONLY ", query );

            return context.Database.SqlQuery<RegionCustomModel>( query, parameters.ToArray() ).ToList();
        }

        /// <summary>
        /// Gets a list of Regions available to the logged in user
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

            string query = $"SELECT r.Id AS [TKey], r.Description AS [TValue] FROM [dbo].[Region] r WHERE (1=1)";

            if ( CurrentUser.RoleType == RoleType.PSP )
            {
                query = $"{query} AND EXISTS(SELECT 1 FROM [dbo].[PSPUser] pu WHERE pu.UserId=@userid AND pu.PSPId=r.PSPId)";
            }

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

        /// <summary>
        /// Checks if a region with the specified name exists for the selected PSP.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="pspId"></param>
        /// <returns></returns>
        public bool Exist( string name, int pspId )
        {
            return context.Regions.Any( r => r.Name.Trim() == name.Trim() && r.PSPId == pspId );
        }
    }
}
