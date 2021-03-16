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
    public class ClientProductService : BaseService<ClientProduct>, IDisposable
    {
        public ClientProductService()
        {

        }

        /// <summary>
        /// Gets a total count of Client Products matching the specified search params
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
                { new SqlParameter( "csmClientId", csm.ClientId ) },
                { new SqlParameter( "csmProductId", csm.ProductId ) },
                { new SqlParameter( "query", csm.Query ?? ( object ) DBNull.Value ) },
                { new SqlParameter( "csmToDate", csm.ToDate ?? ( object ) DBNull.Value ) },
                { new SqlParameter( "userid", ( CurrentUser != null ) ? CurrentUser.Id : 0 ) },
                { new SqlParameter( "csmFromDate", csm.FromDate ?? ( object ) DBNull.Value ) },
            };

            #endregion

            string query = @"SELECT
                                COUNT(cp.Id)
                             FROM
                                [dbo].[ClientProduct] cp
                                INNER JOIN [dbo].[Product] p ON p.Id = cp.ProductId
                                INNER JOIN [dbo].[Client] c ON c.Id = cp.ClientId";

            // WHERE

            #region WHERE

            query = $"{query} WHERE (1=1)";

            #endregion

            // Custom Search

            #region Custom Search

            if ( csm.ClientId > 0 )
            {
                query = $"{query} AND cp.ClientId=@csmClientId ";
            }
            if ( csm.ProductId > 0 )
            {
                query = $"{query} AND cp.ProductId=@csmProductId ";
            }

            if ( csm.FromDate.HasValue && csm.ToDate.HasValue )
            {
                query = $"{query} AND (cp.CreatedOn >= @csmFromDate AND cp.CreatedOn <= @csmToDate) ";
            }
            else if ( csm.FromDate.HasValue || csm.ToDate.HasValue )
            {
                if ( csm.FromDate.HasValue )
                {
                    query = $"{query} AND (cp.CreatedOn>=@csmFromDate) ";
                }
                if ( csm.ToDate.HasValue )
                {
                    query = $"{query} AND (cp.CreatedOn<=@csmToDate) ";
                }
            }

            #endregion

            // Normal Search

            #region Normal Search

            if ( !string.IsNullOrEmpty( csm.Query ) )
            {
                query = string.Format( @"{0} AND (cp.[AccountingCode] LIKE '%{1}%' OR
                                                  cp.[ProductDescription] LIKE '%{1}%' OR
                                                  p.[Name] LIKE '%{1}%' OR
                                                  p.[Description] LIKE '%{1}%' OR
                                                  c.[CompanyName] LIKE '%{1}%'
                                                 ) ", query, csm.Query.Trim() );
            }

            #endregion

            CountModel model = context.Database.SqlQuery<CountModel>( query, parameters.ToArray() ).FirstOrDefault();

            return model.Total;
        }

        /// <summary>
        /// Gets a list of Client Products matching the specified search params
        /// </summary>
        /// <param name="pm"></param>
        /// <param name="csm"></param>
        /// <returns></returns>
        public List<ClientProductCustomModel> List1( PagingModel pm, CustomSearchModel csm )
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
                { new SqlParameter( "csmClientId", csm.ClientId ) },
                { new SqlParameter( "csmProductId", csm.ProductId ) },
                { new SqlParameter( "query", csm.Query ?? ( object ) DBNull.Value ) },
                { new SqlParameter( "csmToDate", csm.ToDate ?? ( object ) DBNull.Value ) },
                { new SqlParameter( "userid", ( CurrentUser != null ) ? CurrentUser.Id : 0 ) },
                { new SqlParameter( "csmFromDate", csm.FromDate ?? ( object ) DBNull.Value ) },
            };

            #endregion

            string query = @"SELECT
                                cp.*,
                                p.[Name] AS [ProductName],
                                c.[CompanyName] AS [ClientName]
                             FROM
                                [dbo].[ClientProduct] cp
                                INNER JOIN [dbo].[Product] p ON p.Id = cp.ProductId
                                INNER JOIN [dbo].[Client] c ON c.Id = cp.ClientId";

            // WHERE

            #region WHERE

            query = $"{query} WHERE (1=1)";

            #endregion

            // Custom Search

            #region Custom Search

            if ( csm.ClientId > 0 )
            {
                query = $"{query} AND cp.ClientId = @csmClientId ";
            }
            if ( csm.ProductId > 0 )
            {
                query = $"{query} AND cp.ProductId=@csmProductId ";
            }

            if ( csm.FromDate.HasValue && csm.ToDate.HasValue )
            {
                query = $"{query} AND (cp.CreatedOn >= @csmFromDate AND cp.CreatedOn <= @csmToDate) ";
            }
            else if ( csm.FromDate.HasValue || csm.ToDate.HasValue )
            {
                if ( csm.FromDate.HasValue )
                {
                    query = $"{query} AND (cp.CreatedOn>=@csmFromDate) ";
                }
                if ( csm.ToDate.HasValue )
                {
                    query = $"{query} AND (cp.CreatedOn<=@csmToDate) ";
                }
            }

            #endregion

            // Normal Search

            #region Normal Search

            if ( !string.IsNullOrEmpty( csm.Query ) )
            {
                query = string.Format( @"{0} AND (cp.[AccountingCode] LIKE '%{1}%' OR
                                                  cp.[ProductDescription] LIKE '%{1}%' OR
                                                  p.[Name] LIKE '%{1}%' OR
                                                  p.[Description] LIKE '%{1}%' OR
                                                  c.[CompanyName] LIKE '%{1}%'
                                                 ) ", query, csm.Query.Trim() );
            }

            #endregion

            // ORDER

            query = $"{query} ORDER BY {pm.SortBy} {pm.Sort}";

            // SKIP, TAKE

            query = string.Format( "{0} OFFSET (@skip) ROWS FETCH NEXT (@take) ROWS ONLY ", query );

            return context.Database.SqlQuery<ClientProductCustomModel>( query, parameters.ToArray() ).ToList();
        }

        /// <summary>
        /// Gets a list of products using the specified client id
        /// </summary>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public List<ClientProduct> ListByClient( int clientId )
        {
            return context.ClientProducts.Include( "Product" ).Where( cp => cp.ClientId == clientId ).ToList();
        }
    }
}
