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
    public class ClientCustomerService : BaseService<ClientCustomer>, IDisposable
    {
        public ClientCustomerService()
        {

        }

        /// <summary>
        /// Gets a client using the specified Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public override ClientCustomer GetById( int id )
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
        public Dictionary<int, string> List( bool v, int clientId = 0 )
        {
            Dictionary<int, string> clientOptions = new Dictionary<int, string>();
            List<IntStringKeyValueModel> model = new List<IntStringKeyValueModel>();

            List<object> parameters = new List<object>()
            {
                { new SqlParameter( "clientid", clientId ) },
                { new SqlParameter( "userid", ( CurrentUser != null ) ? CurrentUser.Id : 0 ) },
            };
            
            string query = string.Empty;

            query = $"SELECT cc.Id AS [TKey], cc.[CustomerName] + ' (' + cs.[AccountingCode] + ')' AS [TValue] FROM [dbo].[ClientCustomer] cc, [dbo].[ClientSite] cs WHERE (cs.[ClientCustomerId]=cc.[Id])";

            if ( CurrentUser.RoleType == RoleType.PSP )
            {
                query = $@"{query} AND EXISTS(SELECT 1 FROM [dbo].[PSPUser] pu INNER JOIN [dbo].[PSPClient] pc ON pc.PSPId=pu.PSPId WHERE pc.ClientId=cc.ClientId AND pu.UserId=@userid ) ";
            }
            else if ( CurrentUser.RoleType == RoleType.Client )
            {
                query = $"{query} AND EXISTS(SELECT 1 FROM [dbo].[ClientUser] cu WHERE cu.UserId=@userid AND cu.ClientId=cc.ClientId)";
            }

            if ( clientId > 0 )
            {
                query = $"{query} AND cc.ClientId=@clientid";
            }

            model = context.Database.SqlQuery<IntStringKeyValueModel>( query.Trim(), parameters.ToArray() ).ToList();

            if ( model != null && model.Any() )
            {
                foreach ( var k in model )
                {
                    if ( clientOptions.Keys.Any( x => x == k.TKey ) )
                        continue;

                    clientOptions.Add( k.TKey, ( k.TValue ?? "" ).Trim() );
                }
            }

            return clientOptions;
        }

        /// <summary>
        /// Gets a Total count of Clients matching the specified search params
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
                { new SqlParameter( "csmPSPId", csm.PSPId ) },
                { new SqlParameter( "csmStatus", ( int ) csm.PSPClientStatus ) },
                { new SqlParameter( "query", csm.Query ?? ( object ) DBNull.Value ) },
                { new SqlParameter( "csmToDate", csm.ToDate ?? ( object ) DBNull.Value ) },
                { new SqlParameter( "userid", ( CurrentUser != null ) ? CurrentUser.Id : 0 ) },
                { new SqlParameter( "csmFromDate", csm.FromDate ?? ( object ) DBNull.Value ) },
            };

            #endregion

            string query = @"SELECT
                                COUNT(c.Id) AS [Total]
                             FROM
                                [dbo].[Client] c
                                LEFT OUTER JOIN [dbo].[PSPClient] pc ON pc.Id=(SELECT TOP 1 pc1.Id FROM [dbo].[PSPClient] pc1 WHERE pc1.ClientId=pc.ClientId AND pc1.ClientId=c.Id)
                                LEFT OUTER JOIN [dbo].[PSP] p ON p.Id=pc.PSPId";

            // WHERE

            #region WHERE

            query = $"{query} WHERE (1=1)";

            // Limit to only show clients for logged in PSP
            if ( !CurrentUser.IsAdmin )
            {
                query = $@"{query} AND EXISTS(SELECT 1 FROM [dbo].[PSPUser] pu
                                                       INNER JOIN [dbo].[PSPClient] pc ON pc.PSPId=pu.PSPId
                                              WHERE
                                                pc.ClientId=c.Id AND
                                                pu.UserId=@userid
                                             ) ";
            }

            #endregion

            // Custom Search

            #region Custom Search

            if ( csm.PSPId > 0 )
            {
                query = $"{query} AND (p.Id=@csmPSPId) ";
            }

            if ( csm.PSPClientStatus != PSPClientStatus.All )
            {
                query = $"{query} AND (c.Status=@csmStatus) ";
            }
            if ( csm.FromDate.HasValue && csm.ToDate.HasValue )
            {
                query = $"{query} AND (c.CreatedOn >= @csmFromDate AND c.CreatedOn <= @csmToDate) ";
            }
            else if ( csm.FromDate.HasValue || csm.ToDate.HasValue )
            {
                if ( csm.FromDate.HasValue )
                {
                    query = $"{query} AND (c.CreatedOn>=@csmFromDate) ";
                }
                if ( csm.ToDate.HasValue )
                {
                    query = $"{query} AND (c.CreatedOn<=@csmToDate) ";
                }
            }

            #endregion

            // Normal Search

            #region Normal Search

            if ( !string.IsNullOrEmpty( csm.Query ) )
            {
                query = string.Format( @"{0} AND (c.[CompanyName] LIKE '%{1}%' OR
                                                  c.[CompanyRegistrationNumber] LIKE '%{1}%' OR
                                                  c.[CompanyName] LIKE '%{1}%' OR
                                                  c.[TradingAs] LIKE '%{1}%' OR
                                                  c.[Description] LIKE '%{1}%' OR
                                                  c.[VATNumber] LIKE '%{1}%' OR
                                                  c.[ContactNumber] LIKE '%{1}%' OR
                                                  c.[ContactPerson] LIKE '%{1}%' OR
                                                  c.[FinancialPerson] LIKE '%{1}%' OR
                                                  c.[FinPersonEmail] LIKE '%{1}%' OR
                                                  c.[Email] LIKE '%{1}%' OR
                                                  c.[AdminEmail] LIKE '%{1}%' OR
                                                  c.[AdminPerson] LIKE '%{1}%' OR
                                                  c.[ChepReference] LIKE '%{1}%' OR
                                                  p.[CompanyName] LIKE '%{1}%'
                                             ) ", query, csm.Query.Trim() );
            }

            #endregion

            CountModel model = context.Database.SqlQuery<CountModel>( query, parameters.ToArray() ).FirstOrDefault();

            return model.Total;
        }

        /// <summary>
        /// Gets a list of Clients matching the specified search params
        /// </summary>
        /// <param name="pm"></param>
        /// <param name="csm"></param>
        /// <returns></returns>
        public List<ClientCustomModel> List1( PagingModel pm, CustomSearchModel csm )
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
                { new SqlParameter( "csmStatus", ( int ) csm.PSPClientStatus ) },
                { new SqlParameter( "query", csm.Query ?? ( object ) DBNull.Value ) },
                { new SqlParameter( "csmToDate", csm.ToDate ?? ( object ) DBNull.Value ) },
                { new SqlParameter( "userid", ( CurrentUser != null ) ? CurrentUser.Id : 0 ) },
                { new SqlParameter( "csmFromDate", csm.FromDate ?? ( object ) DBNull.Value ) },
            };

            #endregion

            string query = @"SELECT
                                c.*,
                                p.CompanyName as [PSPCompanyName],
                                (SELECT COUNT(1) FROM [dbo].[ClientUser] cu WHERE c.Id=cu.ClientId) AS [UserCount],
                                (SELECT COUNT(1) FROM [dbo].[ClientBudget] cb WHERE c.Id=cb.ClientId) AS [BudgetCount],
                                (SELECT COUNT(1) FROM [dbo].[ClientProduct] cp WHERE c.Id=cp.ClientId) AS [ProductCount],
                                (SELECT COUNT(1) FROM [dbo].[Document] d WHERE c.Id=d.ObjectId AND d.ObjectType='Client') AS [DocumentCount],
                                (SELECT COUNT(1) FROM [dbo].[EstimatedLoad] el WHERE c.Id=el.ObjectId AND el.ObjectType='Client') AS [EstimatedLoadCount],
                                (SELECT COUNT(1) FROM [dbo].[ClientInvoice] ci, [dbo].[ClientLoad] cl WHERE cl.Id=ci.ClientLoadId AND c.Id=cl.ClientId) AS [InvoiceCount]
                             FROM
                                [dbo].[Client] c
                                LEFT OUTER JOIN [dbo].[PSPClient] pc ON pc.Id=(SELECT TOP 1 pc1.Id FROM [dbo].[PSPClient] pc1 WHERE pc1.ClientId=pc.ClientId AND pc1.ClientId=c.Id)
                                LEFT OUTER JOIN [dbo].[PSP] p ON p.Id=pc.PSPId";

            // WHERE

            #region WHERE

            query = $"{query} WHERE (1=1)";

            // Limit to only show clients for logged in PSP
            if ( !CurrentUser.IsAdmin )
            {
                query = $@"{query} AND EXISTS(SELECT 1 FROM [dbo].[PSPUser] pu
                                                       INNER JOIN [dbo].[PSPClient] pc ON pc.PSPId=pu.PSPId
                                              WHERE
                                                pc.ClientId=c.Id AND
                                                pu.UserId=@userid
                                             ) ";
            }

            #endregion

            // Custom Search

            #region Custom Search

            if ( csm.PSPId > 0 )
            {
                query = $"{query} AND (p.Id=@csmPSPId) ";
            }

            if ( csm.PSPClientStatus != PSPClientStatus.All )
            {
                query = $"{query} AND (c.Status=@csmStatus) ";
            }
            if ( csm.FromDate.HasValue && csm.ToDate.HasValue )
            {
                query = $"{query} AND (c.CreatedOn >= @csmFromDate AND c.CreatedOn <= @csmToDate) ";
            }
            else if ( csm.FromDate.HasValue || csm.ToDate.HasValue )
            {
                if ( csm.FromDate.HasValue )
                {
                    query = $"{query} AND (c.CreatedOn>=@csmFromDate) ";
                }
                if ( csm.ToDate.HasValue )
                {
                    query = $"{query} AND (c.CreatedOn<=@csmToDate) ";
                }
            }

            #endregion

            // Normal Search

            #region Normal Search

            if ( !string.IsNullOrEmpty( csm.Query ) )
            {
                query = string.Format( @"{0} AND (c.[CompanyName] LIKE '%{1}%' OR
                                                  c.[CompanyRegistrationNumber] LIKE '%{1}%' OR
                                                  c.[CompanyName] LIKE '%{1}%' OR
                                                  c.[TradingAs] LIKE '%{1}%' OR
                                                  c.[Description] LIKE '%{1}%' OR
                                                  c.[VATNumber] LIKE '%{1}%' OR
                                                  c.[ContactNumber] LIKE '%{1}%' OR
                                                  c.[ContactPerson] LIKE '%{1}%' OR
                                                  c.[FinancialPerson] LIKE '%{1}%' OR
                                                  c.[FinPersonEmail] LIKE '%{1}%' OR
                                                  c.[Email] LIKE '%{1}%' OR
                                                  c.[AdminEmail] LIKE '%{1}%' OR
                                                  c.[AdminPerson] LIKE '%{1}%' OR
                                                  c.[ChepReference] LIKE '%{1}%' OR
                                                  c.[AdminPerson] LIKE '%{1}%' OR
                                                  p.[CompanyName] LIKE '%{1}%'
                                             ) ", query, csm.Query.Trim() );
            }

            #endregion

            // ORDER

            query = $"{query} ORDER BY {pm.SortBy} {pm.Sort}";

            // SKIP, TAKE

            query = string.Format( "{0} OFFSET (@skip) ROWS FETCH NEXT (@take) ROWS ONLY ", query );

            List<ClientCustomModel> model = context.Database.SqlQuery<ClientCustomModel>( query, parameters.ToArray() ).ToList();

            if ( model.NullableAny( c => c.DocumentCount > 0 ) )
            {
                using ( DocumentService dservice = new DocumentService() )
                {
                    foreach ( ClientCustomModel item in model.Where( c => c.DocumentCount > 0 ) )
                    {
                        item.Documents = dservice.List( item.Id, "Client" );
                    }
                }
            }

            return model;
        }

        /// <summary>
        /// Gets a Client Customer using the customer number for the specified client
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="customerNumber"></param>
        /// <returns></returns>
        public ClientCustomer GetByNumber( int clientId, string customerNumber )
        {
            return context.ClientCustomers.FirstOrDefault( cc => cc.ClientId == clientId && cc.CustomerNumber.Trim() == customerNumber.Trim() );
        }

        /// <summary>
        /// Gets a client customer record using the specified client id
        /// </summary>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public ClientCustomer GetByClient( int clientId )
        {
            return context.ClientCustomers.FirstOrDefault( cc => cc.ClientId == clientId );
        }
    }
}
