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
    public class ClientAuthorisationService : BaseService<ClientAuthorisation>, IDisposable
    {
        public ClientAuthorisationService()
        {

        }

        /// <summary>
        /// Gets a Client Authorisation using the specified Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public override ClientAuthorisation GetById( int id )
        {
            context.Configuration.LazyLoadingEnabled = true;
            context.Configuration.ProxyCreationEnabled = true;

            return base.GetById( id );
        }

        /// <summary>
        /// Gets a total count of Items matching the specified search params
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
                { new SqlParameter( "query", csm.Query ?? ( object ) DBNull.Value ) },
                { new SqlParameter( "csmToDate", csm.ToDate ?? ( object ) DBNull.Value ) },
                { new SqlParameter( "userid", ( CurrentUser != null ) ? CurrentUser.Id : 0 ) },
                { new SqlParameter( "csmFromDate", csm.FromDate ?? ( object ) DBNull.Value ) },
                { new SqlParameter( "csmSiteId", csm.SiteId ) },
                { new SqlParameter( "csmClientId", csm.ClientId ) },
                { new SqlParameter( "csmTransporterId", csm.TransporterId ) },
            };

            #endregion

            string query = @"SELECT
                                COUNT(ca.Id) AS [Total]
                             FROM
                                [dbo].[ClientAuthorisation] ca
                                LEFT OUTER JOIN [dbo].[Transporter] t ON t.[Id]=ca.[TransporterId]
                                LEFT OUTER JOIN [dbo].[ClientSite] cs ON cs.[Id]=ca.[ClientSiteId]
                                LEFT OUTER JOIN [dbo].[Site] s ON s.[Id]=cs.[SiteId]
                                LEFT OUTER JOIN [dbo].[Client] c ON c.[Id]=cs.[ClientId]";

            // WHERE

            #region WHERE

            query = $"{query} WHERE (1=1)";

            if ( CurrentUser.RoleType == RoleType.PSP )
            {
                query = $"{query} AND EXISTS(SELECT 1 FROM [dbo].[PSPUser] pu, [dbo].[PSPClient] pc WHERE pu.[PSPId]=pc.[PSPId] AND pc.[ClientId]=cs.[ClientId] AND pu.[UserId]=@userid) ";
            }
            else if ( CurrentUser.RoleType == RoleType.Client )
            {
                query = $"{query} AND EXISTS(SELECT 1 FROM [dbo].[ClientUser] cu WHERE cu.[ClientId]=cs.[ClientId] AND cu.[UserId]=@userid) ";
            }

            #endregion

            // Custom Search

            #region Custom Search

            if ( csm.SiteId > 0 )
            {
                query = $"{query} AND (s.Id=@csmSiteId)";
            }
            if ( csm.ClientId > 0 )
            {
                query = $"{query} AND (c.Id=@csmClientId)";
            }
            if ( csm.TransporterId > 0 )
            {
                query = $"{query} AND (t.Id=@csmTransporterId)";
            }

            if ( csm.FromDate.HasValue && csm.ToDate.HasValue )
            {
                query = $"{query} AND (ca.AuthorisationDate >= @csmFromDate AND ca.AuthorisationDate <= @csmToDate) ";
            }
            else if ( csm.FromDate.HasValue || csm.ToDate.HasValue )
            {
                if ( csm.FromDate.HasValue )
                {
                    query = $"{query} AND (ca.AuthorisationDate>=@csmFromDate) ";
                }
                if ( csm.ToDate.HasValue )
                {
                    query = $"{query} AND (ca.AuthorisationDate<=@csmToDate) ";
                }
            }


            #endregion

            // Normal Search

            #region Normal Search

            if ( !string.IsNullOrEmpty( csm.Query ) )
            {
                query = string.Format( @"{0} AND (ca.[LoadNumber] LIKE '%{1}%' OR
                                                  ca.[DocketNumber] LIKE '%{1}%' OR
                                                  ca.[Code] LIKE '%{1}%' OR
                                                  s.[Description] LIKE '%{1}%' OR
                                                  c.[CompanyName] LIKE '%{1}%' OR
                                                  t.[Name] LIKE '%{1}%'
                                                 ) ", query, csm.Query.Trim() );
            }

            #endregion

            CountModel model = context.Database.SqlQuery<CountModel>( query, parameters.ToArray() ).FirstOrDefault();

            return model.Total;
        }


        /// <summary>
        /// Gets a list of Items matching the specified search params
        /// </summary>
        /// <param name="pm"></param>
        /// <param name="csm"></param> 
        /// <returns></returns>
        public List<ClientAuthorisationCustomModel> List1( PagingModel pm, CustomSearchModel csm )
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
                { new SqlParameter( "query", csm.Query ?? ( object ) DBNull.Value ) },
                { new SqlParameter( "csmToDate", csm.ToDate ?? ( object ) DBNull.Value ) },
                { new SqlParameter( "userid", ( CurrentUser != null ) ? CurrentUser.Id : 0 ) },
                { new SqlParameter( "csmFromDate", csm.FromDate ?? ( object ) DBNull.Value ) },
                { new SqlParameter( "csmSiteId", csm.SiteId ) },
                { new SqlParameter( "csmClientId", csm.ClientId ) },
                { new SqlParameter( "csmTransporterId", csm.TransporterId ) },
            };

            #endregion

            string query = @"SELECT
                            ca.*,
                            c.CompanyName AS [ClientName],
                            s.Name AS [SiteName],
                            t.TradingName AS [TransporterName]
                          FROM
                            [dbo].[ClientAuthorisation] ca
                            LEFT OUTER JOIN [dbo].[Transporter] t ON t.[Id]=ca.[TransporterId]
                            LEFT OUTER JOIN [dbo].[ClientSite] cs ON cs.[Id]=ca.[ClientSiteId]
                            LEFT OUTER JOIN [dbo].[Site] s ON s.[Id]=cs.[SiteId]
                            LEFT OUTER JOIN [dbo].[Client] c ON c.[Id]=cs.[ClientId]";

            // WHERE

            #region WHERE

            query = $"{query} WHERE (1=1)";

            if ( CurrentUser.RoleType == RoleType.PSP )
            {
                query = $"{query} AND EXISTS(SELECT 1 FROM [dbo].[PSPUser] pu, [dbo].[PSPClient] pc WHERE pu.[PSPId]=pc.[PSPId] AND pc.[ClientId]=cs.[ClientId] AND pu.[UserId]=@userid) ";
            }
            else if ( CurrentUser.RoleType == RoleType.Client )
            {
                query = $"{query} AND EXISTS(SELECT 1 FROM [dbo].[ClientUser] cu WHERE cu.[ClientId]=cs.[ClientId] AND cu.[UserId]=@userid) ";
            }

            #endregion

            // Custom Search

            #region Custom Search

            if ( csm.SiteId > 0 )
            {
                query = $"{query} AND (s.Id=@csmSiteId)";
            }
            if ( csm.ClientId > 0 )
            {
                query = $"{query} AND (c.Id=@csmClientId)";
            }
            if ( csm.TransporterId > 0 )
            {
                query = $"{query} AND (t.Id=@csmTransporterId)";
            }

            if ( csm.FromDate.HasValue && csm.ToDate.HasValue )
            {
                query = $"{query} AND (ca.AuthorisationDate >= @csmFromDate AND ca.AuthorisationDate <= @csmToDate) ";
            }
            else if ( csm.FromDate.HasValue || csm.ToDate.HasValue )
            {
                if ( csm.FromDate.HasValue )
                {
                    query = $"{query} AND (ca.AuthorisationDate>=@csmFromDate) ";
                }
                if ( csm.ToDate.HasValue )
                {
                    query = $"{query} AND (ca.AuthorisationDate<=@csmToDate) ";
                }
            }


            #endregion

            // Normal Search

            #region Normal Search

            if ( !string.IsNullOrEmpty( csm.Query ) )
            {
                query = string.Format( @"{0} AND (ca.[LoadNumber] LIKE '%{1}%' OR
                                                  ca.[DocketNumber] LIKE '%{1}%' OR
                                                  ca.[Code] LIKE '%{1}%' OR
                                                  s.[Description] LIKE '%{1}%' OR
                                                  c.[CompanyName] LIKE '%{1}%' OR
                                                  t.[Name] LIKE '%{1}%'
                                                 ) ", query, csm.Query.Trim() );
            }

            #endregion

            // ORDER

            query = $"{query} ORDER BY {pm.SortBy} {pm.Sort}";

            // SKIP, TAKE

            query = string.Format( "{0} OFFSET (@skip) ROWS FETCH NEXT (@take) ROWS ONLY ", query );

            return context.Database.SqlQuery<ClientAuthorisationCustomModel>( query, parameters.ToArray() ).ToList();
        }

        /// <summary>
        /// Checks if a Client Authorisation with the specified docket number already exists
        /// </summary>
        /// <param name="loadNumber"></param>
        /// <returns></returns>
        public bool ExistByDocketNumber( string docketNumber )
        {
            return context.ClientAuthorisations.Any( d => d.DocketNumber == docketNumber );
        }

        /// <summary>
        /// Checks if a Client Authorisation with the specified load number already exists
        /// </summary>
        /// <param name="loadNumber"></param>
        /// <returns></returns>
        public bool ExistByLoadNumber( string loadNumber )
        {
            return context.ClientAuthorisations.Any( d => d.LoadNumber == loadNumber );
        }
    }
}
