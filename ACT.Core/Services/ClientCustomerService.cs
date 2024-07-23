using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;

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
        /// Gets a Total count of Customers matching the specified search params
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
            List<object> parameters = new List<object>()
            {
                new SqlParameter("csmPSPId", csm.PSPId),
                new SqlParameter("csmStatus", (int)csm.PSPClientStatus),
                new SqlParameter("query", csm.Query ?? (object)DBNull.Value),
                new SqlParameter("csmToDate", csm.ToDate ?? (object)DBNull.Value),
                new SqlParameter("userid", (CurrentUser != null) ? CurrentUser.Id : 0),
                new SqlParameter("csmFromDate", csm.FromDate ?? (object)DBNull.Value),
            };

            string query = @"
                            SELECT COUNT(DISTINCT cc.Id) AS [Total]
                            FROM [dbo].[ClientCustomer] cc
                            INNER JOIN [dbo].[Client] c ON c.Id = cc.ClientId";

            // WHERE
            query += " WHERE (1=1)";

            // Limit to only show customers for logged in Client
            if ( !CurrentUser.IsAdmin )
            {
                query += @" AND EXISTS(SELECT 1 FROM [dbo].[ClientUser] cu
                    WHERE cc.ClientId = cu.ClientId AND cu.UserId = @userid)";
            }

            // Custom Search
            if ( csm.PSPClientStatus != PSPClientStatus.All )
            {
                query += " AND (cc.Status = @csmStatus)";
            }

            if ( csm.FromDate.HasValue && csm.ToDate.HasValue )
            {
                query += " AND (cc.CreatedOn >= @csmFromDate AND cc.CreatedOn <= @csmToDate)";
            }
            else if ( csm.FromDate.HasValue )
            {
                query += " AND (cc.CreatedOn >= @csmFromDate)";
            }
            else if ( csm.ToDate.HasValue )
            {
                query += " AND (cc.CreatedOn <= @csmToDate)";
            }

            if ( csm.ClientId > 0 )
            {
                query += " AND (cc.ClientId = @clientId)";
                parameters.Add( new SqlParameter( "@clientId", csm.ClientId ) );
            }

            if ( csm.CustomerId > 0 )
            {
                query += " AND (cc.Id = @customerId)";
                parameters.Add( new SqlParameter( "@customerId", csm.CustomerId ) );
            }

            // Normal Search
            if ( !string.IsNullOrEmpty( csm.Query ) )
            {
                query += string.Format( @" AND (cc.CustomerName LIKE '%{0}%' OR
                               cc.CustomerNumber LIKE '%{0}%' OR
                               cc.CustomerContact LIKE '%{0}%' OR
                               cc.CustomerAddress1 LIKE '%{0}%' OR
                               cc.CustomerTown LIKE '%{0}%' OR
                               cc.CustomerPostalCode LIKE '%{0}%' OR
                               c.CompanyName LIKE '%{0}%')",
                                        csm.Query.Trim() );
            }

            CountModel model = context.Database.SqlQuery<CountModel>( query, parameters.ToArray() ).FirstOrDefault();
            return model.Total;
        }

        /// <summary>
        /// Gets a list of Customers matching the specified search params
        /// </summary>
        /// <param name="pm"></param>
        /// <param name="csm"></param>
        /// <returns></returns>
        public List<ClientCustomerCustomModel> List1( PagingModel pm, CustomSearchModel csm )
        {
            if ( csm.FromDate.HasValue && csm.ToDate.HasValue && csm.FromDate?.Date == csm.ToDate?.Date )
            {
                csm.ToDate = csm.ToDate?.AddDays( 1 );
            }

            List<object> parameters = new List<object>()
            {
                new SqlParameter("skip", pm.Skip),
                new SqlParameter("take", pm.Take),
                new SqlParameter("csmPSPId", csm.PSPId),
                new SqlParameter("csmStatus", (int)csm.PSPClientStatus),
                new SqlParameter("query", csm.Query ?? (object)DBNull.Value),
                new SqlParameter("csmToDate", csm.ToDate ?? (object)DBNull.Value),
                new SqlParameter("userid", (CurrentUser != null) ? CurrentUser.Id : 0),
                new SqlParameter("csmFromDate", csm.FromDate ?? (object)DBNull.Value),
                new SqlParameter("clientId", csm.ClientId),
                new SqlParameter("customerId", csm.CustomerId)
            };

            string query = @"
                    SELECT
                        cc.Id,
                        cc.CreatedOn,
                        c.CompanyName AS ClientName,
                        cc.CustomerName,
                        cc.CustomerNumber,
                        MAX(CASE WHEN cont.JobTitle = 2 THEN cont.ContactCell ELSE NULL END) AS CustomerContact,
                        MAX(CASE WHEN cont.JobTitle = 2 THEN cont.ContactName ELSE NULL END) AS KeyAccountManager,
                        CONCAT(
                            ISNULL(a.Addressline1, ''), 
                            CASE WHEN a.Addressline1 IS NOT NULL AND a.Addressline2 IS NOT NULL THEN ', ' ELSE '' END, 
                            ISNULL(a.Addressline2, '')
                        ) AS CustomerAddress1,
                        a.Town AS CustomerTown,
                        a.PostalCode AS CustomerPostalCode,
                        cc.Status,
                        c.Id AS ClientId
                    FROM
                        [dbo].[ClientCustomer] cc
                        INNER JOIN [dbo].[Client] c ON c.Id = cc.ClientId
                        LEFT JOIN [dbo].[Address] a ON a.ObjectId = cc.Id AND a.ObjectType = 'Customer'
                        LEFT JOIN [dbo].[Contact] cont ON cont.ObjectId = cc.Id AND cont.ObjectType = 'Customer'
                    WHERE (1=1)";

            if ( !CurrentUser.IsAdmin )
            {
                query += @" AND EXISTS(SELECT 1 FROM [dbo].[ClientUser] cu
                WHERE cc.ClientId = cu.ClientId AND cu.UserId = @userid)";
            }

            if ( csm.PSPClientStatus != PSPClientStatus.All )
            {
                query += " AND (cc.Status = @csmStatus)";
            }

            if ( csm.FromDate.HasValue && csm.ToDate.HasValue )
            {
                query += " AND (cc.CreatedOn >= @csmFromDate AND cc.CreatedOn <= @csmToDate)";
            }
            else if ( csm.FromDate.HasValue )
            {
                query += " AND (cc.CreatedOn >= @csmFromDate)";
            }
            else if ( csm.ToDate.HasValue )
            {
                query += " AND (cc.CreatedOn <= @csmToDate)";
            }

            if ( csm.ClientId > 0 )
            {
                query += " AND (cc.ClientId = @clientId)";
            }

            if ( csm.CustomerId > 0 )
            {
                query += " AND (cc.Id = @customerId)";
            }

            if ( !string.IsNullOrEmpty( csm.Query ) )
            {
                query += string.Format( @" AND (cc.CustomerName LIKE '%{0}%' OR
                                       cc.CustomerNumber LIKE '%{0}%' OR
                                       cont.ContactCell LIKE '%{0}%' OR
                                       a.Addressline1 LIKE '%{0}%' OR
                                       a.Addressline2 LIKE '%{0}%' OR
                                       a.Town LIKE '%{0}%' OR
                                       a.PostalCode LIKE '%{0}%' OR
                                       c.CompanyName LIKE '%{0}%' OR
                                       kam.ContactName LIKE '%{0}%')",
                                       csm.Query.Trim() );
            }

            query += @"
            GROUP BY
            cc.Id, cc.CreatedOn, c.CompanyName, cc.CustomerName, cc.CustomerNumber,
            a.Addressline1, a.Addressline2, a.Town, a.PostalCode, cc.Status, c.Id";

            query += $" ORDER BY {pm.SortBy} {pm.Sort}";
            query += " OFFSET (@skip) ROWS FETCH NEXT (@take) ROWS ONLY";

            List<ClientCustomerCustomModel> results = context.Database.SqlQuery<ClientCustomerCustomModel>( query, parameters.ToArray() ).ToList();

            foreach ( var item in results )
            {
                string[] addressParts = item.CustomerAddress1.Split( '|' );
                item.CustomerAddress1 = string.Join( "<br>", addressParts.Where( p => !string.IsNullOrWhiteSpace( p ) ) );

                // Set KeyAccountManager to "No Key Account Manager" if it's null or empty
                item.KeyAccountManager = !string.IsNullOrWhiteSpace( item.KeyAccountManager )
                    ? item.KeyAccountManager
                    : "No Key Account Manager";
            }

            return results;
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

        /// <summary>
        /// Checks if a customer with the given customer number exists for any client
        /// </summary>
        /// <param name="customerNumber"></param>
        /// <returns></returns>
        public bool ExistByCustomerNumber( string customerNumber )
        {
            return context.ClientCustomers.Any( cc => cc.CustomerNumber.Trim().ToLower() == customerNumber.Trim().ToLower() );
        }

        /// <summary>
        /// Gets the Key Account Manager for a client or customer
        /// </summary>
        /// <param name="id">The ID of the client or customer</param>
        /// <param name="objectType">Either "Client" or "Customer"</param>
        /// <returns></returns>
        public string GetKeyAccountManager( int id, string objectType )
        {
            var parameters = new List<object>
        {
            new SqlParameter("@id", id),
            new SqlParameter("@objectType", objectType)
        };

            string query = @"
            SELECT TOP 1 ContactName
            FROM [dbo].[Contact]
            WHERE ObjectId = @id 
            AND ObjectType = @objectType
            AND JobTitle = 2
            ORDER BY CreatedOn DESC";

            var result = context.Database.SqlQuery<string>( query, parameters.ToArray() ).FirstOrDefault();
            return result ?? "No Key Account Manager assigned.";
        }

        public string GetKeyAccountManagerFromContacts( int clientId )
        {
            var parameters = new List<object>
            {
                new SqlParameter("@clientId", clientId)
            };

            string query = @"
                            SELECT TOP 1 ContactName
                            FROM [dbo].[Contact]
                            WHERE ObjectId = @clientId 
                            AND ObjectType = 'Client'
                            AND JobTitle = 2
                            ORDER BY CreatedOn DESC";

            var result = context.Database.SqlQuery<string>( query, parameters.ToArray() ).FirstOrDefault();
            return result ?? "No Key Account Manager assigned.";
        }

        public Dictionary<int, string> ListCustomers( bool v, int clientId = 0 )
        {
            Dictionary<int, string> customerOptions = new Dictionary<int, string>();
            List<object> parameters = new List<object>()
            {
                new SqlParameter("clientid", clientId),
                new SqlParameter("userid", (CurrentUser != null) ? CurrentUser.Id : 0),
            };

            string query = @"
            SELECT cc.Id AS [TKey], cc.CustomerName AS [TValue]
            FROM [dbo].[ClientCustomer] cc
            INNER JOIN [dbo].[Client] c ON c.Id = cc.ClientId
            WHERE cc.Status = 1";

            if ( CurrentUser.RoleType == RoleType.PSP )
            {
                query += @" AND EXISTS(SELECT 1 FROM [dbo].[PSPUser] pu 
                        INNER JOIN [dbo].[PSPClient] pc ON pc.PSPId=pu.PSPId 
                        WHERE pc.ClientId=cc.ClientId AND pu.UserId=@userid)";
            }
            else if ( CurrentUser.RoleType == RoleType.Client )
            {
                query += @" AND EXISTS(SELECT 1 FROM [dbo].[ClientUser] cu 
                        WHERE cu.UserId=@userid AND cu.ClientId=cc.ClientId)";
            }

            if ( clientId > 0 )
            {
                query += " AND cc.ClientId=@clientid";
            }

            query += " ORDER BY cc.CustomerName";

            var model = context.Database.SqlQuery<IntStringKeyValueModel>( query, parameters.ToArray() ).ToList();

            if ( model != null && model.Any() )
            {
                foreach ( var k in model )
                {
                    if ( !customerOptions.ContainsKey( k.TKey ) )
                    {
                        customerOptions.Add( k.TKey, k.TValue?.Trim() ?? "" );
                    }
                }
            }

            return customerOptions;
        }

        public List<SelectListItem> GetCustomerSelectList( bool v, int clientId = 0 )
        {
            Dictionary<int, string> customerDict = ListCustomers( v, clientId );
            return customerDict.Select( kvp => new SelectListItem
            {
                Value = kvp.Key.ToString(),
                Text = kvp.Value
            } ).ToList();
        }
    }
}
