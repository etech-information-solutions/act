using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using ACT.Core.Models;
using ACT.Data.Models;

namespace ACT.Core.Services
{
    public class ClientSiteService : BaseService<ClientSite>, IDisposable
    {
        public ClientSiteService()
        {

        }

        /// <summary>
        /// Gets a Client site using the specified Client Id and Site Id
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="siteId"></param>
        /// <returns></returns>
        public ClientSite GetBySiteId( int clientId, int siteId )
        {
            return context.ClientSites.FirstOrDefault( cs => cs.SiteId == siteId && cs.ClientId == clientId );
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

            query = $"SELECT cs.[Id] AS [TKey], s.[Description] AS [TValue] FROM [dbo].[ClientSite] cs, [dbo].[Site] s WHERE (s.Id=cs.SiteId)";

            if ( clientId > 0 )
            {
                query = $"{query} AND cs.ClientId=@clientid";
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
    }
}
