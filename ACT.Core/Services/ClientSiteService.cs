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
        /// <param name="clientCustomerId"></param>
        /// <param name="siteId"></param>
        /// <returns></returns>
        public ClientSite GetBySiteId( int clientCustomerId, int siteId )
        {
            return context.ClientSites.FirstOrDefault( cs => cs.SiteId == siteId && cs.ClientCustomerId == clientCustomerId );
        }



        /// <summary>
        /// Gets a list of clients
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public int Total( bool v, PagingModel pm, CustomSearchModel csm )
        {
            // Parameters

            #region Parameters

            List<object> parameters = new List<object>()
            {
                { new SqlParameter( "skip", pm.Skip ) },
                { new SqlParameter( "take", pm.Take ) },
                { new SqlParameter( "clientid", csm.ClientId ) },
                { new SqlParameter( "userid", ( CurrentUser != null ) ? CurrentUser.Id : 0 ) },
            };

            #endregion

            string query = $"SELECT COUNT(1) AS [Total] FROM [dbo].[ClientSite] cs, [dbo].[Site] s WHERE (s.Id=cs.SiteId)";

            if ( csm.ClientId > 0 )
            {
                query = $"{query} AND EXISTS(SELECT 1 FROM [dbo].[ClientCustomer] cc WHERE cc.ClientId=@clientid)";
            }

            // Normal Search

            #region Normal Search

            if ( !string.IsNullOrEmpty( csm.Query ) )
            {
                query = string.Format( @"{0} AND (s.[Name] LIKE '%{1}%' OR
                                                  s.[Description] LIKE '%{1}%' OR
                                                  s.[AccountCode] LIKE '%{1}%'
                                             ) ", query, csm.Query.Trim() );
            }

            #endregion

            CountModel model = context.Database.SqlQuery<CountModel>( query.Trim(), parameters.ToArray() ).FirstOrDefault();

            return model.Total;
        }

        /// <summary>
        /// Gets a list of clients
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public Dictionary<int, string> List( bool v, PagingModel pm, CustomSearchModel csm )
        {
            Dictionary<int, string> clientOptions = new Dictionary<int, string>();
            List<IntStringKeyValueModel> model = new List<IntStringKeyValueModel>();

            if ( csm.ClientId <= 0 && SelectedClient != null )
            {
                csm.ClientId = SelectedClient.Id;
            }

            // Parameters

            #region Parameters

            List<object> parameters = new List<object>()
            {
                { new SqlParameter( "skip", pm.Skip ) },
                { new SqlParameter( "take", pm.Take ) },
                { new SqlParameter( "clientid", csm.ClientId ) },
                { new SqlParameter( "userid", ( CurrentUser != null ) ? CurrentUser.Id : 0 ) },
            };

            #endregion

            string query = $"SELECT cs.[Id] AS [TKey], s.[Description] AS [TValue] FROM [dbo].[Site] s LEFT OUTER JOIN [dbo].[ClientSite] cs ON cs.[Id]=(SELECT TOP 1 cs1.[Id] FROM [dbo].[ClientSite] cs1 WHERE cs1.[SiteId]=s.[Id]) WHERE (1=1)";

            if ( csm.ClientId > 0 )
            {
                query = $"{query} AND EXISTS(SELECT 1 FROM [dbo].[ClientCustomer] cc WHERE cs.ClientCustomerId=cc.Id AND cc.ClientId=@clientid)";
            }

            // Normal Search

            #region Normal Search

            if ( !string.IsNullOrEmpty( csm.Query ) )
            {
                query = string.Format( @"{0} AND (s.[Name] LIKE '%{1}%' OR
                                                  s.[Description] LIKE '%{1}%' OR
                                                  s.[AccountCode] LIKE '%{1}%'
                                             ) ", query, csm.Query.Trim() );
            }

            #endregion

            // ORDER

            query = $"{query} ORDER BY {pm.SortBy} {pm.Sort}";

            // SKIP, TAKE
            if ( pm.Take != ConfigSettings.PagingTake )
            {
                query = $"{query} OFFSET (@skip) ROWS FETCH NEXT (@take) ROWS ONLY;";
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
        /// Gets a client site using the specified site code
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public ClientSite GetBySiteCode( string code )
        {
            return context.ClientSites.FirstOrDefault( s => s.ClientSiteCode.Trim() == code.Trim() );
        }
    }
}
