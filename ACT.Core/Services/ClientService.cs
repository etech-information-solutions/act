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
    public class ClientService : BaseService<Client>, IDisposable
    {
        public ClientService()
        {

        }

        /// <summary>
        /// Gets a client using the specified Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public override Client GetById( int id )
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
        public Dictionary<int, string> List( bool v, int siteId = 0, List<int> siteIds = null )
        {
            Dictionary<int, string> clientOptions = new Dictionary<int, string>();
            List<IntStringKeyValueModel> model = new List<IntStringKeyValueModel>();

            List<object> parameters = new List<object>()
            {
                { new SqlParameter( "siteid", siteId ) },
                { new SqlParameter( "userid", ( CurrentUser != null ) ? CurrentUser.Id : 0 ) },
            };

            string query = string.Empty;

            query = $"SELECT c.Id AS [TKey], c.CompanyName AS [TValue] FROM [dbo].[Client] c WHERE (1=1)";

            if ( !CurrentUser.IsAdmin )
            {
                query = $"{query} AND EXISTS(SELECT 1 FROM [dbo].[ClientUser] cu WHERE cu.UserId=@userid AND cu.ClientId=c.Id)";
            }

            if ( siteId > 0 )
            {
                query = $"{query} AND EXISTS(SELECT 1 FROM [dbo].[ClientSite] cs WHERE cs.SiteId=@siteid AND cs.ClientId=c.Id)";
            }
            else if ( siteIds.NullableAny() )
            {
                query = $"{query} AND EXISTS(SELECT 1 FROM [dbo].[ClientSite] cs WHERE cs.SiteId IN({string.Join( ",", siteIds )}) AND cs.ClientId=c.Id)";
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
        /// Gets a list of PSPs matching the specified search params
        /// </summary>
        /// <param name="pm"></param>
        /// <param name="csm"></param>
        /// <returns></returns>
        public List<Client> ListAwaitingActivation( PagingModel pm, CustomSearchModel csm )
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
                { new SqlParameter( "csmClientStatus", ( int ) csm.Status ) },
                { new SqlParameter( "csmToDate", csm.ToDate ?? ( object ) DBNull.Value ) },
                { new SqlParameter( "userid", ( CurrentUser != null ) ? CurrentUser.Id : 0 ) },
                { new SqlParameter( "csmFromDate", csm.FromDate ?? ( object ) DBNull.Value ) },
            };

            #endregion

            string query = @"SELECT
                                p.*
                                FROM
                                [dbo].[Client] p";

            // WHERE

            #region WHERE

            query = $"{query} WHERE (1=1)";

            // Limit to only show PSP for logged in user
            //if (!CurrentUser.IsAdmin)
            //{
            //    query = $"{query} AND EXISTS(SELECT 1 FROM[dbo].[PSPUser] pu LEFT JOIN[dbo].[PSPClient] pc ON pc.PSPId = pu.PSPId WHERE pc.ClientId = p.Id AND pu.UserId = @userid) ";
            //    //query = $"{query} AND EXISTS(SELECT 1 FROM [dbo].[PSPUser] pu WHERE p.Id=pu.PSPId AND pu.UserId=@userid) ";
            //}

            #endregion

            // Custom Search

            #region Custom Search
            query = $"{query} AND p.Status in (2) ";
            //if (csm.ClientId > 0)
            //{
            //    query = $"{query} AND EXISTS(SELECT 1 FROM [dbo].[PSPClient] pc WHERE p.Id=pc.ClientId AND pc.ClientId=@csmClientId) ";
            //}
            //if (csm.ProductId != 0)
            //{
            //    query = $"{query} AND EXISTS(SELECT 1 FROM [dbo].[PSPProduct] pp WHERE p.Id=pp.PSPId AND pp.ProductId=@csmProductId) ";
            //}
            //if (csm.ClientStatus != Status.All)
            //{
            //    query = $"{query} AND (p.Status=@csmClientStatus) ";
            //}
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
                                                  p.[AdminEmail] LIKE '%{1}%' OR
                                                 p.[AdminPerson] LIKE '%{1}%' OR
                                                 p.[FinPersonEmail] LIKE '%{1}%' OR
                                                 p.[ChepReference] LIKE '%{1}%'
                                             ) ", query, csm.Query.Trim() );
            }

            #endregion

            // ORDER

            query = $"{query} ORDER BY {pm.SortBy} {pm.Sort}";

            // SKIP, TAKE

            query = string.Format( "{0} OFFSET (@skip) ROWS FETCH NEXT (@take) ROWS ONLY ", query );

            List<Client> model = context.Database.SqlQuery<Client>( query, parameters.ToArray() ).ToList();

            return model;
        }

        public List<Client> GetClientsByPSP( int pspId )
        {
            List<Client> clientList;
            //context.Roles.FirstOrDefault(c => c.Name.ToLower().Trim() == name.ToLower().Trim());
            clientList = ( from p in context.PSPClients
                           join e in context.Clients
                           on p.ClientId equals e.Id
                           where p.PSPId == pspId
                           select e ).Distinct().ToList();

            return clientList;
        }


        public List<Client> GetClientsByPSPAwaitingActivation( int pspId )
        {
            List<Client> clientList;
            List<int> statusList = new List<int>();
            statusList.Add( ( int ) Status.Pending );
            statusList.Add( ( int ) Status.Inactive );
            //context.Roles.FirstOrDefault(c => c.Name.ToLower().Trim() == name.ToLower().Trim());
            clientList = ( from p in context.PSPClients
                           join e in context.Clients
                           on p.ClientId equals e.Id
                           where p.PSPId == pspId
                           where statusList.Contains( e.Status )
                           select e ).ToList();

            return clientList;
        }

        public List<Client> GetClientsByPSPIncludedGroup( int pspId, int groupId, CustomSearchModel csm )
        {
            #region Parameters

            List<object> parameters = new List<object>()
            {
                { new SqlParameter( "csmClientId", csm.ClientId ) },
                { new SqlParameter( "csmClientStatus", ( int ) csm.Status ) },
            };

            #endregion

            //return clientList;
            string query = string.Format( "SELECT DISTINCT(c.Id) as DClientId, c.* FROM [dbo].[Client] c LEFT JOIN [dbo].[PSPClient] psp ON c.Id = psp.ClientID LEFT JOIN [dbo].[ClientGroup] cg ON cg.ClientId = psp.ClientId WHERE cg.GroupId = {0} AND psp.PSPId = {1}", groupId, pspId );
            #region Custom Search

            if ( csm.ClientId > 0 )
            {
                query = $"{query} AND c.Id=@csmClientId ";
            }

            if ( csm.Status != Status.All )
            {
                query = $"{query} AND (c.Status=@csmClientStatus) ";
            }
            #endregion
            return context.Database.SqlQuery<Client>( query, parameters.ToArray() ).ToList();
        }

        public List<Client> GetClientsByPSPExcludedGroup( int pspId, int groupId, CustomSearchModel csm )
        {
            #region Parameters

            List<object> parameters = new List<object>()
            {
                { new SqlParameter( "csmClientId", csm.ClientId ) },
                { new SqlParameter( "csmClientStatus", ( int ) csm.Status ) },
            };

            #endregion

            string query = string.Format( "SELECT DISTINCT(c.Id) as DClientId,c.* FROM [dbo].[Client] c LEFT JOIN [dbo].[PSPClient] psp ON c.Id = psp.ClientID WHERE NOT EXISTS (SELECT cg.ClientID FROM [dbo].[ClientGroup] cg WHERE cg.GroupId = {0} AND cg.ClientId = psp.ClientId) AND psp.PSPId = {1}", groupId, pspId );
            #region Custom Search

            if ( csm.ClientId > 0 )
            {
                query = $"{query} AND c.Id=@csmClientId ";
            }

            if ( csm.Status != Status.All )
            {
                query = $"{query} AND (c.Status=@csmClientStatus) ";
            }
            #endregion

            return context.Database.SqlQuery<Client>( query, parameters.ToArray() ).ToList();
        }

        public bool ExistByCompanyRegistrationNumber( string registrationNumber )
        {
            return context.Clients.Any( c => c.CompanyRegistrationNumber == registrationNumber );
        }
    }
}
