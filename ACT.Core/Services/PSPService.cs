using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using ACT.Core.Models;
using ACT.Core.Models.Custom;
using ACT.Data.Models;

namespace ACT.Core.Services
{
    public class PSPService : BaseService<PSP>, IDisposable
    {
        public PSPService()
        {

        }

        public override PSP GetById( int id )
        {
            context.Configuration.LazyLoadingEnabled = true;
            context.Configuration.ProxyCreationEnabled = true;

            return base.GetById( id );
        }

        public bool ExistByCompanyRegistrationNumber( string registrationNumber )
        {
            return context.PSPs.Any( p => p.CompanyRegistrationNumber == registrationNumber );
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
                { new SqlParameter( "csmClientId", csm.ClientId ) },
                { new SqlParameter( "csmProductId", csm.ProductId ) },
                { new SqlParameter( "query", csm.Query ?? ( object ) DBNull.Value ) },
                { new SqlParameter( "csmPSPClientStatus", ( int ) csm.PSPClientStatus ) },
                { new SqlParameter( "csmToDate", csm.ToDate ?? ( object ) DBNull.Value ) },
                { new SqlParameter( "userid", ( CurrentUser != null ) ? CurrentUser.Id : 0 ) },
                { new SqlParameter( "csmFromDate", csm.FromDate ?? ( object ) DBNull.Value ) },
            };

            #endregion

            string query = @"SELECT
                                COUNT(1) AS [Total]
                             FROM
                                [dbo].[PSP] p";

            // WHERE

            #region WHERE

            query = $"{query} WHERE (1=1)";

            // Limit to only show PSP for logged in user
            if ( !CurrentUser.IsAdmin )
            {
                query = $"{query} AND EXISTS(SELECT 1 FROM [dbo].[PSPUser] pu WHERE p.Id=pu.PSPId AND pu.UserId=@userid) ";
            }

            #endregion

            // Custom Search

            #region Custom Search

            if ( csm.ClientId != 0 )
            {
                query = $"{query} AND EXISTS(SELECT 1 FROM [dbo].[PSPClient] pc WHERE p.Id=pc.PSPId AND pc.ClientId=@csmClientId) ";
            }
            if ( csm.ProductId != 0 )
            {
                query = $"{query} AND EXISTS(SELECT 1 FROM [dbo].[PSPProduct] pp WHERE p.Id=pp.PSPId AND pp.ProductId=@csmProductId) ";
            }
            if ( csm.PSPClientStatus != Enums.PSPClientStatus.All )
            {
                query = $"{query} AND (p.Status=@csmPSPClientStatus) ";
            }

            if ( csm.FromDate.HasValue && csm.ToDate.HasValue )
            {
                query = $"{query} AND (p.CreatedOn >= @csmFromDate AND p.CreatedOn <= @csmToDate) ";
            }
            else if ( csm.FromDate.HasValue || csm.ToDate.HasValue )
            {
                if ( csm.FromDate.HasValue )
                {
                    query = $"{query} AND (p.CreatedOn>=@csmFromDate) ";
                }
                if ( csm.ToDate.HasValue )
                {
                    query = $"{query} AND (p.CreatedOn<=@csmToDate) ";
                }
            }

            #endregion

            // Normal Search

            #region Normal Search

            if ( !string.IsNullOrEmpty( csm.Query ) )
            {
                query = string.Format( @"{0} AND (p.[CompanyName] LIKE '%{1}%' OR
                                                  p.[CompanyRegistrationNumber] LIKE '%{1}%' OR
                                                  p.[CompanyName] LIKE '%{1}%' OR
                                                  p.[TradingAs] LIKE '%{1}%' OR
                                                  p.[Description] LIKE '%{1}%' OR
                                                  p.[VATNumber] LIKE '%{1}%' OR
                                                  p.[ContactNumber] LIKE '%{1}%' OR
                                                  p.[ContactPerson] LIKE '%{1}%' OR
                                                  p.[Email] LIKE '%{1}%' OR
                                                  p.[AdminEmail] LIKE '%{1}%'
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
        public List<PSPCustomModel> List1( PagingModel pm, CustomSearchModel csm )
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
                { new SqlParameter( "csmPSPClientStatus", ( int ) csm.PSPClientStatus ) },
                { new SqlParameter( "csmToDate", csm.ToDate ?? ( object ) DBNull.Value ) },
                { new SqlParameter( "userid", ( CurrentUser != null ) ? CurrentUser.Id : 0 ) },
                { new SqlParameter( "csmFromDate", csm.FromDate ?? ( object ) DBNull.Value ) },
            };

            #endregion

            string query = @"SELECT
                                p.*,
                                (SELECT COUNT(1) FROM [dbo].[PSPUser] pu WHERE p.Id=pu.PSPId) AS [UserCount],
                                (SELECT COUNT(1) FROM [dbo].[PSPBudget] pb WHERE p.Id=pb.PSPId) AS [BudgetCount],
                                (SELECT COUNT(1) FROM [dbo].[PSPClient] pc WHERE p.Id=pc.PSPId) AS [ClientCount],
                                (SELECT COUNT(1) FROM [dbo].[PSPProduct] pp WHERE p.Id=pp.PSPId) AS [ProductCount],
                                (SELECT COUNT(1) FROM [dbo].[PSPBilling] pi WHERE p.Id=pi.PSPId) AS [InvoiceCount],
                                (SELECT COUNT(1) FROM [dbo].[Document] d WHERE p.Id=d.ObjectId AND d.ObjectType='PSP') AS [DocumentCount]
                             FROM
                                [dbo].[PSP] p";

            // WHERE

            #region WHERE

            query = $"{query} WHERE (1=1)";

            // Limit to only show PSP for logged in user
            if ( !CurrentUser.IsAdmin )
            {
                query = $"{query} AND EXISTS(SELECT 1 FROM [dbo].[PSPUser] pu WHERE p.Id=pu.PSPId AND pu.UserId=@userid) ";
            }

            #endregion

            // Custom Search

            #region Custom Search

            if ( csm.ClientId != 0 )
            {
                query = $"{query} AND EXISTS(SELECT 1 FROM [dbo].[PSPClient] pc WHERE p.Id=pc.PSPId AND pc.ClientId=@csmClientId) ";
            }
            if ( csm.ProductId != 0 )
            {
                query = $"{query} AND EXISTS(SELECT 1 FROM [dbo].[PSPProduct] pp WHERE p.Id=pp.PSPId AND pp.ProductId=@csmProductId) ";
            }
            if ( csm.PSPClientStatus != Enums.PSPClientStatus.All )
            {
                query = $"{query} AND (p.Status=@csmPSPClientStatus) ";
            }

            if ( csm.FromDate.HasValue && csm.ToDate.HasValue )
            {
                query = $"{query} AND (p.CreatedOn >= @csmFromDate AND p.CreatedOn <= @csmToDate) ";
            }
            else if ( csm.FromDate.HasValue || csm.ToDate.HasValue )
            {
                if ( csm.FromDate.HasValue )
                {
                    query = $"{query} AND (p.CreatedOn>=@csmFromDate) ";
                }
                if ( csm.ToDate.HasValue )
                {
                    query = $"{query} AND (p.CreatedOn<=@csmToDate) ";
                }
            }

            #endregion

            // Normal Search

            #region Normal Search

            if ( !string.IsNullOrEmpty( csm.Query ) )
            {
                query = string.Format( @"{0} AND (p.[CompanyName] LIKE '%{1}%' OR
                                                  p.[CompanyRegistrationNumber] LIKE '%{1}%' OR
                                                  p.[CompanyName] LIKE '%{1}%' OR
                                                  p.[TradingAs] LIKE '%{1}%' OR
                                                  p.[Description] LIKE '%{1}%' OR
                                                  p.[VATNumber] LIKE '%{1}%' OR
                                                  p.[ContactNumber] LIKE '%{1}%' OR
                                                  p.[ContactPerson] LIKE '%{1}%' OR
                                                  p.[Email] LIKE '%{1}%' OR
                                                  p.[AdminEmail] LIKE '%{1}%'
                                             ) ", query, csm.Query.Trim() );
            }

            #endregion

            // ORDER

            query = $"{query} ORDER BY {pm.SortBy} {pm.Sort}";

            // SKIP, TAKE

            query = string.Format( "{0} OFFSET (@skip) ROWS FETCH NEXT (@take) ROWS ONLY ", query );

            List<PSPCustomModel> model = context.Database.SqlQuery<PSPCustomModel>( query, parameters.ToArray() ).ToList();

            if ( model.NullableAny( p => p.DocumentCount > 0 ) )
            {
                using ( DocumentService dservice = new DocumentService() )
                {
                    foreach ( PSPCustomModel item in model.Where( p => p.DocumentCount > 0 ) )
                    {
                        item.Documents = dservice.List( item.Id, "PSP" );
                    }
                }
            }

            return model;
        }

        /// <summary>
        /// Gets a list of clients
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

            string query = $"SELECT p.Id AS [TKey], p.CompanyName AS [TValue] FROM [dbo].[PSP] p";

            if ( !CurrentUser.IsAdmin )
            {
                query = $"{query} AND EXISTS(SELECT 1 FROM [dbo].[PSPUser] pu WHERE pu.UserId=@userid AND pu.PSPId=p.Id)";
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
    }
}
