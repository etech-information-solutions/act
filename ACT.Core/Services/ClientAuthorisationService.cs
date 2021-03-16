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
                                COUNT(cl.Id) AS [Total]
                             FROM
                                [dbo].[ClientLoad] cl
                                LEFT OUTER JOIN [dbo].[ClientAuthorisation] ca ON ca.[LoadNumber]=cl.[LoadNumber]
                                LEFT OUTER JOIN [dbo].[User] u ON u.[Id]=ca.[UserId]
                                LEFT OUTER JOIN [dbo].[Vehicle] v ON v.[Id]=cl.[VehicleId]
                                LEFT OUTER JOIN [dbo].[Transporter] t ON t.[Id]=cl.[TransporterId]

                                LEFT OUTER JOIN [dbo].[ClientSite] cs1 ON cs1.[Id]=cl.[ClientSiteId]
                                LEFT OUTER JOIN [dbo].[Site] s1 ON s1.[Id]=cs1.[SiteId]

                                LEFT OUTER JOIN [dbo].[ClientSite] cs2 ON cs2.[Id]=cl.[ToClientSiteId]
                                LEFT OUTER JOIN [dbo].[Site] s2 ON s2.[Id]=cs2.[SiteId]

                                LEFT OUTER JOIN [dbo].[Client] c ON c.[Id]=cl.[ClientId]";

            // WHERE

            #region WHERE

            query = $"{query} WHERE (1=1)";

            if ( CurrentUser.RoleType == RoleType.PSP )
            {
                query = $"{query} AND EXISTS(SELECT 1 FROM [dbo].[PSPUser] pu, [dbo].[PSPClient] pc WHERE pu.[PSPId]=pc.[PSPId] AND pc.[ClientId]=cl.[ClientId] AND pu.[UserId]=@userid) ";
            }
            else if ( CurrentUser.RoleType == RoleType.Client )
            {
                query = $"{query} AND EXISTS(SELECT 1 FROM [dbo].[ClientUser] cu WHERE cu.[ClientId]=cl.[ClientId] AND cu.[UserId]=@userid) ";
            }

            #endregion

            // Custom Search

            #region Custom Search

            if ( csm.SiteId > 0 )
            {
                query = $"{query} AND (s1.Id=@csmSiteId)";
            }
            if ( csm.ClientId > 0 )
            {
                query = $"{query} AND (c.Id=@csmClientId)";
            }
            if ( csm.VehicleId > 0 )
            {
                query = $"{query} AND (v.Id=@csmVehicleId)";
            }
            if ( csm.TransporterId > 0 )
            {
                query = $"{query} AND (t.Id=@csmTransporterId)";
            }
            if ( csm.HasAuthorisationCode )
            {
                query = $"{query} AND (ca.Id IS NOT NULL)";
            }

            if ( csm.FromDate.HasValue && csm.ToDate.HasValue )
            {
                query = $"{query} AND (ca.[AuthorisationDate] >= @csmFromDate AND ca.[AuthorisationDate] <= @csmToDate) ";
            }
            else if ( csm.FromDate.HasValue || csm.ToDate.HasValue )
            {
                if ( csm.FromDate.HasValue )
                {
                    query = $"{query} AND (ca.[AuthorisationDate]>=@csmFromDate) ";
                }
                if ( csm.ToDate.HasValue )
                {
                    query = $"{query} AND (ca.[AuthorisationDate]<=@csmToDate) ";
                }
            }

            #endregion

            // Normal Search

            #region Normal Search

            if ( !string.IsNullOrEmpty( csm.Query ) )
            {
                query = string.Format( @"{0} AND (cl.[LoadNumber] LIKE '%{1}%' OR
                                                  cl.[AccountNumber] LIKE '%{1}%' OR
                                                  cl.[ClientDescription] LIKE '%{1}%' OR
                                                  cl.[DeliveryNote] LIKE '%{1}%' OR
                                                  cl.[ReferenceNumber] LIKE '%{1}%' OR
                                                  cl.[ReceiverNumber] LIKE '%{1}%' OR
                                                  cl.[Equipment] LIKE '%{1}%' OR
                                                  cl.[PODNumber] LIKE '%{1}%' OR
                                                  cl.[PCNNumber] LIKE '%{1}%' OR
                                                  cl.[PRNNumber] LIKE '%{1}%' OR
                                                  cl.[THAN] LIKE '%{1}%' OR
                                                  cl.[ChepCompensationNo] LIKE '%{1}%' OR
                                                  cl.[LoadNumber] LIKE '%{1}%' OR
                                                  ca.[LoadNumber] LIKE '%{1}%' OR
                                                  ca.[Code] LIKE '%{1}%' OR
                                                  s1.[Name] LIKE '%{1}%' OR
                                                  s1.[Description] LIKE '%{1}%' OR
                                                  s2.[Name] LIKE '%{1}%' OR
                                                  s2.[Description] LIKE '%{1}%' OR
                                                  c.[CompanyName] LIKE '%{1}%' OR
                                                  t.[Name] LIKE '%{1}%' OR
                                                  v.[Registration] LIKE '%{1}%' OR
                                                  u.[Name] LIKE '%{1}%' OR
                                                  u.[Surname] LIKE '%{1}%' OR
                                                  u.[Email] LIKE '%{1}%' OR
                                                  u.[Cell] LIKE '%{1}%'
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
                                ca.Status AS [Status],
                                cl.*,
                                ca.Code AS [Code],
                                c.CompanyName AS [ClientName],
                                s2.Description AS [ToSiteName],
                                s1.Description AS [FromSiteName],
                                ca.Id AS [ClientAuthorisationId],
                                t.TradingName AS [TransporterName],
                                v.Registration AS [VehicleRegistration],
                                ca.AuthorisationDate AS [AuthorisationDate],
                                u.Name + ' ' + u.Surname AS [AuthoriserName]
                             FROM
                                [dbo].[ClientLoad] cl
                                LEFT OUTER JOIN [dbo].[ClientAuthorisation] ca ON ca.[LoadNumber]=cl.[LoadNumber]
                                LEFT OUTER JOIN [dbo].[User] u ON u.[Id]=ca.[UserId]
                                LEFT OUTER JOIN [dbo].[Vehicle] v ON v.[Id]=cl.[VehicleId]
                                LEFT OUTER JOIN [dbo].[Transporter] t ON t.[Id]=cl.[TransporterId]

                                LEFT OUTER JOIN [dbo].[ClientSite] cs1 ON cs1.[Id]=cl.[ClientSiteId]
                                LEFT OUTER JOIN [dbo].[Site] s1 ON s1.[Id]=cs1.[SiteId]

                                LEFT OUTER JOIN [dbo].[ClientSite] cs2 ON cs2.[Id]=cl.[ToClientSiteId]
                                LEFT OUTER JOIN [dbo].[Site] s2 ON s2.[Id]=cs2.[SiteId]

                                LEFT OUTER JOIN [dbo].[Client] c ON c.[Id]=cl.[ClientId]";

            // WHERE

            #region WHERE

            query = $"{query} WHERE (1=1)";

            if ( CurrentUser.RoleType == RoleType.PSP )
            {
                query = $"{query} AND EXISTS(SELECT 1 FROM [dbo].[PSPUser] pu, [dbo].[PSPClient] pc WHERE pu.[PSPId]=pc.[PSPId] AND pc.[ClientId]=cl.[ClientId] AND pu.[UserId]=@userid) ";
            }
            else if ( CurrentUser.RoleType == RoleType.Client )
            {
                query = $"{query} AND EXISTS(SELECT 1 FROM [dbo].[ClientUser] cu WHERE cu.[ClientId]=cl.[ClientId] AND cu.[UserId]=@userid) ";
            }

            #endregion

            // Custom Search

            #region Custom Search

            if ( csm.SiteId > 0 )
            {
                query = $"{query} AND (s1.Id=@csmSiteId)";
            }
            if ( csm.ClientId > 0 )
            {
                query = $"{query} AND (c.Id=@csmClientId)";
            }
            if ( csm.VehicleId > 0 )
            {
                query = $"{query} AND (v.Id=@csmVehicleId)";
            }
            if ( csm.TransporterId > 0 )
            {
                query = $"{query} AND (t.Id=@csmTransporterId)";
            }
            if ( csm.HasAuthorisationCode )
            {
                query = $"{query} AND (ca.Id IS NOT NULL)";
            }

            if ( csm.FromDate.HasValue && csm.ToDate.HasValue )
            {
                query = $"{query} AND (ca.[AuthorisationDate] >= @csmFromDate AND ca.[AuthorisationDate] <= @csmToDate) ";
            }
            else if ( csm.FromDate.HasValue || csm.ToDate.HasValue )
            {
                if ( csm.FromDate.HasValue )
                {
                    query = $"{query} AND (ca.[AuthorisationDate]>=@csmFromDate) ";
                }
                if ( csm.ToDate.HasValue )
                {
                    query = $"{query} AND (ca.[AuthorisationDate]<=@csmToDate) ";
                }
            }

            #endregion

            // Normal Search

            #region Normal Search

            if ( !string.IsNullOrEmpty( csm.Query ) )
            {
                query = string.Format( @"{0} AND (cl.[LoadNumber] LIKE '%{1}%' OR
                                                  cl.[AccountNumber] LIKE '%{1}%' OR
                                                  cl.[ClientDescription] LIKE '%{1}%' OR
                                                  cl.[DeliveryNote] LIKE '%{1}%' OR
                                                  cl.[ReferenceNumber] LIKE '%{1}%' OR
                                                  cl.[ReceiverNumber] LIKE '%{1}%' OR
                                                  cl.[Equipment] LIKE '%{1}%' OR
                                                  cl.[PODNumber] LIKE '%{1}%' OR
                                                  cl.[PCNNumber] LIKE '%{1}%' OR
                                                  cl.[PRNNumber] LIKE '%{1}%' OR
                                                  cl.[THAN] LIKE '%{1}%' OR
                                                  cl.[ChepCompensationNo] LIKE '%{1}%' OR
                                                  cl.[LoadNumber] LIKE '%{1}%' OR
                                                  ca.[LoadNumber] LIKE '%{1}%' OR
                                                  ca.[Code] LIKE '%{1}%' OR
                                                  s1.[Name] LIKE '%{1}%' OR
                                                  s1.[Description] LIKE '%{1}%' OR
                                                  s2.[Name] LIKE '%{1}%' OR
                                                  s2.[Description] LIKE '%{1}%' OR
                                                  c.[CompanyName] LIKE '%{1}%' OR
                                                  t.[Name] LIKE '%{1}%' OR
                                                  v.[Registration] LIKE '%{1}%' OR
                                                  u.[Name] LIKE '%{1}%' OR
                                                  u.[Surname] LIKE '%{1}%' OR
                                                  u.[Email] LIKE '%{1}%' OR
                                                  u.[Cell] LIKE '%{1}%'
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
        /// Checks if a Client Authorisation with the specified load number already exists
        /// </summary>
        /// <param name="loadNumber"></param>
        /// <returns></returns>
        public bool ExistByLoadNumber( string loadNumber )
        {
            return context.ClientAuthorisations.Any( d => d.LoadNumber == loadNumber );
        }

        public List<CountModel> ListStats( PagingModel pm, CustomSearchModel csm )
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
                                COUNT(cl.Id) AS [Total],
	                            c.CompanyName AS [Description]
                             FROM
                                [dbo].[ClientAuthorisation] ca
                                INNER JOIN [dbo].[ChepLoad] cl ON cl.[Id]=(SELECT TOP 1 cl1.[Id] FROM [dbo].[ChepLoad] cl1 WHERE ca.[DocketNumber]=cl1.[DocketNumber])
                                LEFT OUTER JOIN [dbo].[Transporter] t ON t.[Id]=ca.[TransporterId]
                                LEFT OUTER JOIN [dbo].[ClientSite] cs ON cs.[Id]=ca.[ClientSiteId]
                                LEFT OUTER JOIN [dbo].[Site] s ON s.[Id]=cs.[SiteId]
                                LEFT OUTER JOIN [dbo].[Client] c ON c.[Id]=cl.[ClientId]";

            // WHERE

            #region WHERE

            query = $"{query} WHERE (1=1)";

            if ( CurrentUser.RoleType == RoleType.PSP )
            {
                query = $"{query} AND EXISTS(SELECT 1 FROM [dbo].[PSPUser] pu, [dbo].[PSPClient] pc WHERE pu.[PSPId]=pc.[PSPId] AND pc.[ClientId]=cl.[ClientId] AND pu.[UserId]=@userid) ";
            }
            else if ( CurrentUser.RoleType == RoleType.Client )
            {
                query = $"{query} AND EXISTS(SELECT 1 FROM [dbo].[ClientUser] cu WHERE cu.[ClientId]=cl.[ClientId] AND cu.[UserId]=@userid) ";
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
                query = $"{query} AND (cl.[EffectiveDate] >= @csmFromDate AND cl.[EffectiveDate] <= @csmToDate) ";
            }
            else if ( csm.FromDate.HasValue || csm.ToDate.HasValue )
            {
                if ( csm.FromDate.HasValue )
                {
                    query = $"{query} AND (cl.[EffectiveDate]>=@csmFromDate) ";
                }
                if ( csm.ToDate.HasValue )
                {
                    query = $"{query} AND (cl.[EffectiveDate]<=@csmToDate) ";
                }
            }

            #endregion

            // Normal Search

            #region Normal Search

            if ( !string.IsNullOrEmpty( csm.Query ) )
            {
                query = string.Format( @"{0} AND (cl.[ChepStatus] LIKE '%{1}%' OR
                                                  cl.[TransactionType] LIKE '%{1}%' OR
                                                  cl.[DocketNumber] LIKE '%{1}%' OR
                                                  cl.[OriginalDocketNumber] LIKE '%{1}%' OR
                                                  cl.[UMI] LIKE '%{1}%' OR
                                                  cl.[LocationId] LIKE '%{1}%' OR
                                                  cl.[Location] LIKE '%{1}%' OR
                                                  cl.[OtherPartyId] LIKE '%{1}%' OR
                                                  cl.[OtherParty] LIKE '%{1}%' OR
                                                  cl.[OtherPartyCountry] LIKE '%{1}%' OR
                                                  cl.[EquipmentCode] LIKE '%{1}%' OR
                                                  cl.[Equipment] LIKE '%{1}%' OR
                                                  cl.[Ref] LIKE '%{1}%' OR
                                                  cl.[OtherRef] LIKE '%{1}%' OR
                                                  cl.[BatchRef] LIKE '%{1}%' OR
                                                  cl.[InvoiceNumber] LIKE '%{1}%' OR
                                                  cl.[DataSource] LIKE '%{1}%' OR
                                                  cl.[CreatedBy] LIKE '%{1}%' OR
                                                  cl.[Quantity] LIKE '%{1}%' OR
                                                  ca.[LoadNumber] LIKE '%{1}%' OR
                                                  ca.[DocketNumber] LIKE '%{1}%' OR
                                                  ca.[Code] LIKE '%{1}%' OR
                                                  s.[Description] LIKE '%{1}%' OR
                                                  c.[CompanyName] LIKE '%{1}%' OR
                                                  t.[Name] LIKE '%{1}%'
                                                 ) ", query, csm.Query.Trim() );
            }

            #endregion

            // GROUP

            query = $"{query} GROUP BY c.[CompanyName]";

            // ORDER

            query = $"{query} ORDER BY {pm.SortBy} {pm.Sort}";

            // SKIP, TAKE

            query = string.Format( "{0} OFFSET (@skip) ROWS FETCH NEXT (@take) ROWS ONLY ", query );

            List<CountModel> model = context.Database.SqlQuery<CountModel>( query, parameters.ToArray() ).ToList();

            return model;
        }

        /// <summary>
        /// Gets an authorisation code using the specified code
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public ClientAuthorisation GetByCode( string code )
        {
            context.Configuration.LazyLoadingEnabled = true;
            context.Configuration.ProxyCreationEnabled = true;

            return context.ClientAuthorisations.FirstOrDefault( ca => ca.Code == code );
        }

        /// <summary>
        /// Checks if a Client Authorisation with the specified load number already exists
        /// </summary>
        /// <param name="loadNumber"></param>
        /// <returns></returns>
        public ClientAuthorisation GetByLoadNumber( string loadNumber )
        {
            return context.ClientAuthorisations.Include( "User" ).FirstOrDefault( d => d.LoadNumber == loadNumber );
        }
    }
}
