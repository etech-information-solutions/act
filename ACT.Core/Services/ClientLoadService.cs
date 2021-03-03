using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

using ACT.Core.Enums;
using ACT.Core.Models;
using ACT.Core.Models.Custom;
using ACT.Core.Models.Simple;
using ACT.Data.Models;

namespace ACT.Core.Services
{
    public class ClientLoadService : BaseService<ClientLoad>, IDisposable
    {
        public ClientLoadService()
        {

        }

        /// <summary>
        /// Gets a Client Load using the specified refernce number
        /// </summary>
        /// <param name="referenceNumber"></param>
        /// <returns></returns>
        public ClientLoad GetByRefernce( string referenceNumber )
        {
            return context.ClientLoads.FirstOrDefault( cl => cl.ReferenceNumber == referenceNumber );
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
                { new SqlParameter( "csmSiteId", csm.SiteId ) },
                { new SqlParameter( "csmClientId", csm.ClientId ) },
                { new SqlParameter( "csmVehicleId", csm.VehicleId ) },
                { new SqlParameter( "csmTransporterId", csm.TransporterId ) },
                { new SqlParameter( "csmInvoiceStatus", ( int ) csm.InvoiceStatus ) },
                { new SqlParameter( "query", csm.Query ?? ( object ) DBNull.Value ) },
                { new SqlParameter( "csmOutstandingReasonId", csm.OutstandingReasonId ) },
                { new SqlParameter( "csmToDate", csm.ToDate ?? ( object ) DBNull.Value ) },
                { new SqlParameter( "userid", ( CurrentUser != null ) ? CurrentUser.Id : 0 ) },
                { new SqlParameter( "csmFromDate", csm.FromDate ?? ( object ) DBNull.Value ) },
                { new SqlParameter( "csmReconciliationStatus", ( int ) csm.ReconciliationStatus ) },
                { new SqlParameter( "csmManuallyMatchedUID", csm.ManuallyMatchedUID ?? ( object ) DBNull.Value ) },
            };

            #endregion

            string query = @"SELECT
                                COUNT(cl.Id) AS [Total]
                             FROM
                                 [dbo].[ClientLoad] cl
                                 INNER JOIN [dbo].[Client] c ON c.[Id]=cl.[ClientId]
                                 LEFT OUTER JOIN [dbo].[ChepClient] cc ON cl.[Id]=cc.[ClientLoadsId]
                                 LEFT OUTER JOIN [dbo].[ChepLoad] ch ON ch.[Id]=cc.[ChepLoadsId]
                                 LEFT OUTER JOIN [dbo].[ClientSite] cs ON cs.[Id]=cl.[ClientSiteId]
                                 LEFT OUTER JOIN [dbo].[Site] s ON s.[Id]=cs.[SiteId]
                                 LEFT OUTER JOIN [dbo].[Site] s1 ON s.[Id]=s1.[SiteId]
                                 LEFT OUTER JOIN [dbo].[OutstandingReason] otr ON otr.[Id]=cl.[OutstandingReasonId]";

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
                query = $"{query} AND (cl.ClientId=@csmClientId)";
            }
            if ( csm.VehicleId > 0 )
            {
                query = $"{query} AND (cl.VehicleId=@csmVehicleId)";
            }
            if ( csm.TransporterId > 0 )
            {
                query = $"{query} AND (cl.TransporterId=@csmTransporterId)";
            }
            if ( csm.OutstandingReasonId > 0 )
            {
                query = $"{query} AND (cl.OutstandingReasonId=@csmOutstandingReasonId)";
            }
            if ( csm.ReconciliationStatus != ReconciliationStatus.All )
            {
                query = $"{query} AND (cl.Status=@csmReconciliationStatus)";
            }
            if ( csm.InvoiceStatus != InvoiceStatus.All )
            {
                query = $"{query} AND (cl.InvoiceStatus=@csmInvoiceStatus)";
            }

            if ( csm.FromDate.HasValue && csm.ToDate.HasValue )
            {
                query = $"{query} AND (cl.LoadDate >= @csmFromDate AND cl.LoadDate <= @csmToDate) ";
            }
            else if ( csm.FromDate.HasValue || csm.ToDate.HasValue )
            {
                if ( csm.FromDate.HasValue )
                {
                    query = $"{query} AND (cl.LoadDate>=@csmFromDate) ";
                }
                if ( csm.ToDate.HasValue )
                {
                    query = $"{query} AND (cl.LoadDate<=@csmToDate) ";
                }
            }

            if ( !string.IsNullOrEmpty( csm.ManuallyMatchedUID ) )
            {
                query = $"{query} AND (cl.ManuallyMatchedUID=@csmManuallyMatchedUID) ";
            }

            #endregion

            // Normal Search

            #region Normal Search

            if ( !string.IsNullOrEmpty( csm.Query ) )
            {
                query = string.Format( @"{0} AND (cl.[ReferenceNumber] LIKE '%{1}%' OR
                                                  cl.[DeliveryNote] LIKE '%{1}%' OR
                                                  cl.[ClientDescription] LIKE '%{1}%' OR
                                                  cl.[Equipment] LIKE '%{1}%' OR
                                                  cl.[LoadNumber] LIKE '%{1}%' OR
                                                  cl.[ReceiverNumber] LIKE '%{1}%' OR
                                                  cl.[AccountNumber] LIKE '%{1}%' OR
                                                  cl.[PODNumber] LIKE '%{1}%' OR
                                                  cl.[PCNNumber] LIKE '%{1}%' OR
                                                  cl.[PRNNumber] LIKE '%{1}%' OR
                                                  s.[Description] LIKE '%{1}%' OR
                                                  c.[CompanyName] LIKE '%{1}%' OR
                                                  otr.[Description] LIKE '%{1}%'
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
        public List<ClientLoadCustomModel> List1( PagingModel pm, CustomSearchModel csm )
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
                { new SqlParameter( "csmSiteId", csm.SiteId ) },
                { new SqlParameter( "csmClientId", csm.ClientId ) },
                { new SqlParameter( "csmVehicleId", csm.VehicleId ) },
                { new SqlParameter( "csmTransporterId", csm.TransporterId ) },
                { new SqlParameter( "csmInvoiceStatus", ( int ) csm.InvoiceStatus ) },
                { new SqlParameter( "query", csm.Query ?? ( object ) DBNull.Value ) },
                { new SqlParameter( "csmOutstandingReasonId", csm.OutstandingReasonId ) },
                { new SqlParameter( "csmToDate", csm.ToDate ?? ( object ) DBNull.Value ) },
                { new SqlParameter( "userid", ( CurrentUser != null ) ? CurrentUser.Id : 0 ) },
                { new SqlParameter( "csmFromDate", csm.FromDate ?? ( object ) DBNull.Value ) },
                { new SqlParameter( "csmReconciliationStatus", ( int ) csm.ReconciliationStatus ) },
                { new SqlParameter( "csmManuallyMatchedUID", csm.ManuallyMatchedUID ?? ( object ) DBNull.Value ) },
            };

            #endregion

            string query = @"";

            if ( true == false /*csm.ReconciliationStatus == ReconciliationStatus.Reconciled*/ )
            {
                query = @"SELECT
                            cl.*,
                            cl1.PostingType,
                            cl1.DocketNumber,
                            s.Id AS [SiteId],
                            s.Description AS [SiteName],
                            c.CompanyName AS [ClientName],
                            s1.Description AS [SubSiteName],
                            otr.[Description] AS [OutstandingReason],
                            (SELECT COUNT(1) FROM [dbo].[Task] t WHERE cl.Id=t.ClientLoadId) AS [TaskCount],
                            (SELECT COUNT(1) FROM [dbo].[Journal] j WHERE cl.Id=j.ClientLoadId) AS [JournalCount],
                            (SELECT COUNT(1) FROM [dbo].[Document] d WHERE cl.Id=d.ObjectId AND d.ObjectType='ClientLoad') AS [DocumentCount]
                          FROM
                            [dbo].[ClientLoad] cl
                            INNER JOIN [dbo].[Client] c ON c.[Id]=cl.[ClientId]
                            INNER JOIN [dbo].[ChepClient] cc ON cl.[Id]=cc.[ClientLoadsId]
                            INNER JOIN [dbo].[ChepLoad] cl1 ON cl1.[Id]=cc.[ChepLoadsId]
                            LEFT OUTER JOIN [dbo].[ClientSite] cs ON cs.[Id]=cl.[ClientSiteId]
                            LEFT OUTER JOIN [dbo].[Site] s ON s.[Id]=cs.[SiteId]
                            LEFT OUTER JOIN [dbo].[Site] s1 ON s.[Id]=s1.[SiteId]
                            LEFT OUTER JOIN [dbo].[OutstandingReason] otr ON otr.[Id]=cl.[OutstandingReasonId]";
            }
            else
            {
                query = @"SELECT
                            cl.*,
                            s.Id AS [SiteId],
                            s.Description AS [SiteName],
                            c.CompanyName AS [ClientName],
                            s1.Description AS [SubSiteName],
                            otr.[Description] AS [OutstandingReason],
                            (SELECT COUNT(1) FROM [dbo].[Task] t WHERE cl.Id=t.ClientLoadId) AS [TaskCount],
                            (SELECT COUNT(1) FROM [dbo].[Journal] j WHERE cl.Id=j.ClientLoadId) AS [JournalCount],
                            (SELECT COUNT(1) FROM [dbo].[Document] d WHERE cl.Id=d.ObjectId AND d.ObjectType='ClientLoad') AS [DocumentCount]
                          FROM
                            [dbo].[ClientLoad] cl
                            INNER JOIN [dbo].[Client] c ON c.[Id]=cl.[ClientId]
                            LEFT OUTER JOIN [dbo].[ChepClient] cc ON cl.[Id]=cc.[ClientLoadsId]
                            LEFT OUTER JOIN [dbo].[ChepLoad] ch ON ch.[Id]=cc.[ChepLoadsId]
                            LEFT OUTER JOIN [dbo].[ClientSite] cs ON cs.[Id]=cl.[ClientSiteId]
                            LEFT OUTER JOIN [dbo].[Site] s ON s.[Id]=cs.[SiteId]
                            LEFT OUTER JOIN [dbo].[Site] s1 ON s.[Id]=s1.[SiteId]
                            LEFT OUTER JOIN [dbo].[OutstandingReason] otr ON otr.[Id]=cl.[OutstandingReasonId]";
            }

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
                query = $"{query} AND (cl.ClientId=@csmClientId)";
            }
            if ( csm.VehicleId > 0 )
            {
                query = $"{query} AND (cl.VehicleId=@csmVehicleId)";
            }
            if ( csm.TransporterId > 0 )
            {
                query = $"{query} AND (cl.TransporterId=@csmTransporterId)";
            }
            if ( csm.OutstandingReasonId > 0 )
            {
                query = $"{query} AND (cl.OutstandingReasonId=@csmOutstandingReasonId)";
            }
            if ( csm.ReconciliationStatus != ReconciliationStatus.All )
            {
                query = $"{query} AND (cl.Status=@csmReconciliationStatus)";
            }
            if ( csm.InvoiceStatus != InvoiceStatus.All )
            {
                query = $"{query} AND (cl.InvoiceStatus=@csmInvoiceStatus)";
            }

            if ( csm.FromDate.HasValue && csm.ToDate.HasValue )
            {
                query = $"{query} AND (cl.LoadDate >= @csmFromDate AND cl.LoadDate <= @csmToDate) ";
            }
            else if ( csm.FromDate.HasValue || csm.ToDate.HasValue )
            {
                if ( csm.FromDate.HasValue )
                {
                    query = $"{query} AND (cl.LoadDate>=@csmFromDate) ";
                }
                if ( csm.ToDate.HasValue )
                {
                    query = $"{query} AND (cl.LoadDate<=@csmToDate) ";
                }
            }

            if ( !string.IsNullOrEmpty( csm.ManuallyMatchedUID ) )
            {
                query = $"{query} AND (cl.ManuallyMatchedUID=@csmManuallyMatchedUID) ";
            }

            #endregion

            // Normal Search

            #region Normal Search

            if ( !string.IsNullOrEmpty( csm.Query ) )
            {
                query = string.Format( @"{0} AND (cl.[ReferenceNumber] LIKE '%{1}%' OR
                                                  cl.[DeliveryNote] LIKE '%{1}%' OR
                                                  cl.[ClientDescription] LIKE '%{1}%' OR
                                                  cl.[Equipment] LIKE '%{1}%' OR
                                                  cl.[LoadNumber] LIKE '%{1}%' OR
                                                  cl.[ReceiverNumber] LIKE '%{1}%' OR
                                                  cl.[AccountNumber] LIKE '%{1}%' OR
                                                  cl.[PODNumber] LIKE '%{1}%' OR
                                                  cl.[PCNNumber] LIKE '%{1}%' OR
                                                  cl.[PRNNumber] LIKE '%{1}%' OR
                                                  s.[Description] LIKE '%{1}%' OR
                                                  c.[CompanyName] LIKE '%{1}%' OR
                                                  otr.[Description] LIKE '%{1}%'
                                                 ) ", query, csm.Query.Trim() );
            }

            #endregion

            // ORDER

            query = $"{query} ORDER BY {pm.SortBy} {pm.Sort}";

            // SKIP, TAKE

            query = string.Format( "{0} OFFSET (@skip) ROWS FETCH NEXT (@take) ROWS ONLY ", query );

            List<ClientLoadCustomModel> model = context.Database.SqlQuery<ClientLoadCustomModel>( query, parameters.ToArray() ).ToList();

            if ( model.NullableAny( p => p.DocumentCount > 0 ) )
            {
                using ( DocumentService dservice = new DocumentService() )
                {
                    foreach ( ClientLoadCustomModel item in model.Where( p => p.DocumentCount > 0 ) )
                    {
                        item.Documents = dservice.List( item.Id, "ClientLoad" );
                    }
                }
            }

            return model;
        }

        /// <summary>
        /// Gets loads per month for the specified From-To date as well as other applicable search params
        /// </summary>
        /// <param name="csm"></param>
        /// <returns></returns>
        public LoadsPerMonth LoadsPerMonth( CustomSearchModel csm )
        {
            // Parameters

            #region Parameters

            List<object> parameters = new List<object>()
            {
                { new SqlParameter( "csmSiteId", csm.SiteId ) },
                { new SqlParameter( "csmClientId", csm.ClientId ) },
                { new SqlParameter( "csmGroupId", ( int ) csm.GroupId ) },
                { new SqlParameter( "csmRegionId", ( int ) csm.RegionId ) },
                { new SqlParameter( "csmToDate", csm.ToDate ?? ( object ) DBNull.Value ) },
                { new SqlParameter( "userid", ( CurrentUser != null ) ? CurrentUser.Id : 0 ) },
                { new SqlParameter( "csmFromDate", csm.FromDate ?? ( object ) DBNull.Value ) },
            };

            #endregion


            #region Query

            string query = @"SELECT
	                            COUNT(cl.[Id]) AS [Loads],
	                            SUM(cl.[NewQuantity]) AS [Quantity],
	                            COUNT(CASE WHEN cl.[PODNumber] IS NULL THEN 1 ELSE 0 END) AS [POD]
                             FROM 
	                            [dbo].[ClientLoad] cl
                             WHERE (1=1)";

            #endregion

            // WHERE

            #region WHERE

            if ( CurrentUser.RoleType == RoleType.PSP )
            {
                query = $"{query} AND EXISTS(SELECT 1 FROM [dbo].[PSPUser] pu, [dbo].[PSPClient] pc WHERE pu.[PSPId]=pc.[PSPId] AND pc.[ClientId]=cl.[ClientId] AND pu.[UserId]=@userid) ";
            }
            else if ( CurrentUser.RoleType == RoleType.Client )
            {
                query = $"{query} AND EXISTS(SELECT 1 FROM [dbo].[ClientUser] cu WHERE cu.[ClientId]=cl.[ClientId] AND cu.[UserId]=@userid) ";
            }

            #endregion

            // CUSTOM SEARCH

            #region Custom Search

            if ( csm.ClientId > 0 )
            {
                query = $"{query} AND (cl.ClientId=@csmClientId) ";
            }

            if ( csm.SiteId > 0 )
            {
                query = $"{query} AND EXISTS(SELECT 1 FROM [dbo].[ClientSite] cs WHERE cs.[ClientId]=cl.[ClientId] AND cs.[SiteId]=@csmSiteId) ";
            }

            if ( csm.GroupId > 0 )
            {
                query = $"{query} AND EXISTS(SELECT 1 FROM [dbo].[ClientGroup] cg WHERE cg.[ClientId]=cl.[ClientId] AND cg.[GroupId]=@csmGroupId) ";
            }

            if ( csm.RegionId > 0 )
            {
                query = $"{query} AND EXISTS(SELECT 1 FROM [dbo].[Site] s, [dbo].[ClientSite] cs WHERE s.[Id]=cs.[SiteId] AND cs.[ClientId]=cl.[ClientId] AND s.[RegionId]=@csmRegionId) ";
            }

            if ( csm.FromDate.HasValue && csm.ToDate.HasValue )
            {
                query = $"{query} AND (cl.LoadDate >= @csmFromDate AND cl.LoadDate <= @csmToDate) ";
            }
            else if ( csm.FromDate.HasValue || csm.ToDate.HasValue )
            {
                if ( csm.FromDate.HasValue )
                {
                    query = $"{query} AND (cl.LoadDate>=@csmFromDate) ";
                }
                if ( csm.ToDate.HasValue )
                {
                    query = $"{query} AND (cl.LoadDate<=@csmToDate) ";
                }
            }

            #endregion

            return context.Database.SqlQuery<LoadsPerMonth>( query, parameters.ToArray() ).FirstOrDefault();
        }

        /// <summary>
        /// Gets a comparison of loads vs authorisation codes issued for the specified From-To date as well as other applicable search params
        /// </summary>
        /// <param name="csm"></param>
        /// <returns></returns>
        public AuthorisationCodes AuthorisationCodesPerMonth( CustomSearchModel csm )
        {
            // Parameters

            #region Parameters

            List<object> parameters = new List<object>()
            {
                { new SqlParameter( "csmSiteId", csm.SiteId ) },
                { new SqlParameter( "csmClientId", csm.ClientId ) },
                { new SqlParameter( "csmGroupId", ( int ) csm.GroupId ) },
                { new SqlParameter( "csmRegionId", ( int ) csm.RegionId ) },
                { new SqlParameter( "csmToDate", csm.ToDate ?? ( object ) DBNull.Value ) },
                { new SqlParameter( "userid", ( CurrentUser != null ) ? CurrentUser.Id : 0 ) },
                { new SqlParameter( "csmFromDate", csm.FromDate ?? ( object ) DBNull.Value ) },
            };

            #endregion


            #region Query

            string clQuery = @"SELECT
	                              COUNT(cl.[Id]) AS [Loads]
                               FROM
	                              [dbo].[ClientLoad] cl
                               WHERE
	                              (1=1)";

            string caQuery = @"SELECT
	                              COUNT(ca.[Id]) AS [Codes]
                               FROM
	                              [dbo].[ClientAuthorisation] ca,
	                              [dbo].[ClientLoad] cl
                               WHERE
	                              (ca.[LoadNumber]=cl.[LoadNumber])";

            #endregion

            // WHERE

            #region WHERE

            if ( CurrentUser.RoleType == RoleType.PSP )
            {
                clQuery = $"{clQuery} AND EXISTS(SELECT 1 FROM [dbo].[PSPUser] pu, [dbo].[PSPClient] pc WHERE pu.[PSPId]=pc.[PSPId] AND pc.[ClientId]=cl.[ClientId] AND pu.[UserId]=@userid) ";
                caQuery = $"{caQuery} AND EXISTS(SELECT 1 FROM [dbo].[PSPUser] pu, [dbo].[PSPClient] pc WHERE pu.[PSPId]=pc.[PSPId] AND pc.[ClientId]=cl.[ClientId] AND pu.[UserId]=@userid) ";
            }
            else if ( CurrentUser.RoleType == RoleType.Client )
            {
                clQuery = $"{clQuery} AND EXISTS(SELECT 1 FROM [dbo].[ClientUser] cu WHERE cu.[ClientId]=cl.[ClientId] AND cu.[UserId]=@userid) ";
                caQuery = $"{caQuery} AND EXISTS(SELECT 1 FROM [dbo].[ClientUser] cu WHERE cu.[ClientId]=cl.[ClientId] AND cu.[UserId]=@userid) ";
            }

            #endregion

            // CUSTOM SEARCH

            #region Custom Search

            if ( csm.ClientId > 0 )
            {
                clQuery = $"{clQuery} AND (cl.ClientId=@csmClientId) ";
                caQuery = $"{caQuery} AND (cl.ClientId=@csmClientId) ";
            }

            if ( csm.SiteId > 0 )
            {
                clQuery = $"{clQuery} AND EXISTS(SELECT 1 FROM [dbo].[ClientSite] cs WHERE cs.[ClientId]=cl.[ClientId] AND cs.[SiteId]=@csmSiteId) ";
                caQuery = $"{caQuery} AND EXISTS(SELECT 1 FROM [dbo].[ClientSite] cs WHERE cs.[ClientId]=cl.[ClientId] AND cs.[SiteId]=@csmSiteId) ";
            }

            if ( csm.GroupId > 0 )
            {
                clQuery = $"{clQuery} AND EXISTS(SELECT 1 FROM [dbo].[ClientGroup] cg WHERE cg.[ClientId]=cl.[ClientId] AND cg.[GroupId]=@csmGroupId) ";
                caQuery = $"{caQuery} AND EXISTS(SELECT 1 FROM [dbo].[ClientGroup] cg WHERE cg.[ClientId]=cl.[ClientId] AND cg.[GroupId]=@csmGroupId) ";
            }

            if ( csm.RegionId > 0 )
            {
                clQuery = $"{clQuery} AND EXISTS(SELECT 1 FROM [dbo].[Site] s, [dbo].[ClientSite] cs WHERE s.[Id]=cs.[SiteId] AND cs.[ClientId]=cl.[ClientId] AND s.[RegionId]=@csmRegionId) ";
                caQuery = $"{caQuery} AND EXISTS(SELECT 1 FROM [dbo].[Site] s, [dbo].[ClientSite] cs WHERE s.[Id]=cs.[SiteId] AND cs.[ClientId]=cl.[ClientId] AND s.[RegionId]=@csmRegionId) ";
            }

            if ( csm.FromDate.HasValue && csm.ToDate.HasValue )
            {
                clQuery = $"{clQuery} AND (cl.LoadDate >= @csmFromDate AND cl.LoadDate <= @csmToDate) ";
                caQuery = $"{caQuery} AND (ca.AuthorisationDate >= @csmFromDate AND ca.AuthorisationDate <= @csmToDate) ";
            }
            else if ( csm.FromDate.HasValue || csm.ToDate.HasValue )
            {
                if ( csm.FromDate.HasValue )
                {
                    clQuery = $"{clQuery} AND (cl.LoadDate>=@csmFromDate) ";
                    caQuery = $"{caQuery} AND (ca.AuthorisationDate>=@csmFromDate) ";
                }
                if ( csm.ToDate.HasValue )
                {
                    clQuery = $"{clQuery} AND (cl.LoadDate<=@csmToDate) ";
                    caQuery = $"{caQuery} AND (ca.AuthorisationDate<=@csmToDate) ";
                }
            }

            #endregion

            AuthorisationCodes clAuth = context.Database.SqlQuery<AuthorisationCodes>( clQuery, parameters.ToArray() ).FirstOrDefault();

            parameters = new List<object>()
            {
                { new SqlParameter( "csmSiteId", csm.SiteId ) },
                { new SqlParameter( "csmClientId", csm.ClientId ) },
                { new SqlParameter( "csmGroupId", ( int ) csm.GroupId ) },
                { new SqlParameter( "csmRegionId", ( int ) csm.RegionId ) },
                { new SqlParameter( "csmToDate", csm.ToDate ?? ( object ) DBNull.Value ) },
                { new SqlParameter( "userid", ( CurrentUser != null ) ? CurrentUser.Id : 0 ) },
                { new SqlParameter( "csmFromDate", csm.FromDate ?? ( object ) DBNull.Value ) },
            };

            AuthorisationCodes caAuth = context.Database.SqlQuery<AuthorisationCodes>( caQuery, parameters.ToArray() ).FirstOrDefault();

            return new AuthorisationCodes()
            {
                Codes = caAuth.Codes,
                Loads = clAuth.Loads
            };
        }

        /// <summary>
        /// Gets the number of pallets managed for the specified From-To date as well as other applicable search params
        /// </summary>
        /// <param name="csm"></param>
        /// <param name="isYear"></param>
        /// <returns></returns>
        public NumberOfPalletsManaged NumberOfPalletsManaged( CustomSearchModel csm )
        {
            // Parameters

            #region Parameters

            List<object> parameters = new List<object>()
            {
                { new SqlParameter( "csmSiteId", csm.SiteId ) },
                { new SqlParameter( "csmClientId", csm.ClientId ) },
                { new SqlParameter( "csmGroupId", ( int ) csm.GroupId ) },
                { new SqlParameter( "csmRegionId", ( int ) csm.RegionId ) },
                { new SqlParameter( "csmToDate", csm.ToDate ?? ( object ) DBNull.Value ) },
                { new SqlParameter( "userid", ( CurrentUser != null ) ? CurrentUser.Id : 0 ) },
                { new SqlParameter( "csmFromDate", csm.FromDate ?? ( object ) DBNull.Value ) },
                { new SqlParameter( "rsReconciled", ReconciliationStatus.Reconciled ) },
                { new SqlParameter( "rsUnreconciled", ReconciliationStatus.Unreconciled ) },
            };

            #endregion


            #region Query

            string query = @"SELECT
	                            SUM(cl.[NewQuantity]) AS [Total],
	                            SUM(CASE WHEN cl.[Status]=@rsReconciled THEN cl.[NewQuantity] ELSE 0 END) AS [Reconciled],
	                            SUM(CASE WHEN cl.[Status]=@rsUnreconciled THEN cl.[NewQuantity] ELSE 0 END) AS [Unreconciled]
                             FROM 
	                            [dbo].[ClientLoad] cl
                             WHERE (1=1)";

            #endregion

            // WHERE

            #region WHERE

            if ( CurrentUser.RoleType == RoleType.PSP )
            {
                query = $"{query} AND EXISTS(SELECT 1 FROM [dbo].[PSPUser] pu, [dbo].[PSPClient] pc WHERE pu.[PSPId]=pc.[PSPId] AND pc.[ClientId]=cl.[ClientId] AND pu.[UserId]=@userid) ";
            }
            else if ( CurrentUser.RoleType == RoleType.Client )
            {
                query = $"{query} AND EXISTS(SELECT 1 FROM [dbo].[ClientUser] cu WHERE cu.[ClientId]=cl.[ClientId] AND cu.[UserId]=@userid) ";
            }

            #endregion

            // CUSTOM SEARCH

            #region Custom Search

            if ( csm.ClientId > 0 )
            {
                query = $"{query} AND (cl.ClientId=@csmClientId) ";
            }

            if ( csm.SiteId > 0 )
            {
                query = $"{query} AND EXISTS(SELECT 1 FROM [dbo].[ClientSite] cs WHERE cs.[ClientId]=cl.[ClientId] AND cs.[SiteId]=@csmSiteId) ";
            }

            if ( csm.GroupId > 0 )
            {
                query = $"{query} AND EXISTS(SELECT 1 FROM [dbo].[ClientGroup] cg WHERE cg.[ClientId]=cl.[ClientId] AND cg.[GroupId]=@csmGroupId) ";
            }

            if ( csm.RegionId > 0 )
            {
                query = $"{query} AND EXISTS(SELECT 1 FROM [dbo].[Site] s, [dbo].[ClientSite] cs WHERE s.[Id]=cs.[SiteId] AND cs.[ClientId]=cl.[ClientId] AND s.[RegionId]=@csmRegionId) ";
            }

            if ( csm.FromDate.HasValue && csm.ToDate.HasValue )
            {
                query = $"{query} AND (cl.LoadDate >= @csmFromDate AND cl.LoadDate <= @csmToDate) ";
            }
            else if ( csm.FromDate.HasValue || csm.ToDate.HasValue )
            {
                if ( csm.FromDate.HasValue )
                {
                    query = $"{query} AND (cl.LoadDate>=@csmFromDate) ";
                }
                if ( csm.ToDate.HasValue )
                {
                    query = $"{query} AND (cl.LoadDate<=@csmToDate) ";
                }
            }

            #endregion

            return context.Database.SqlQuery<NumberOfPalletsManaged>( query, parameters.ToArray() ).FirstOrDefault();
        }

        public List<SimpleOutstandingPallets> ListOutstandingPalletsPerClient( CustomSearchModel csm )
        {
            // Parameters

            #region Parameters

            List<object> parameters = new List<object>()
            {
                { new SqlParameter( "csmBalanceStatus", ( int ) csm.BalanceStatus ) },
                { new SqlParameter( "csmToDate", csm.ToDate ?? ( object ) DBNull.Value ) },
                { new SqlParameter( "userid", ( CurrentUser != null ) ? CurrentUser.Id : 0 ) },
                { new SqlParameter( "csmFromDate", csm.FromDate ?? ( object ) DBNull.Value ) },
                { new SqlParameter( "csmReconciliationStatus", ( int ) csm.ReconciliationStatus ) },
            };

            #endregion

            string query = @"SELECT
	                            c.[Id] AS [ClientId],
	                            c.[CompanyName] AS [ClientName],
	                            COUNT(ch.[Id]) AS [Total]
                            FROM
	                            [dbo].[ChepLoad] ch,
	                            [dbo].[Client] c
                            WHERE
	                            (c.[Id]=ch.[ClientId])";

            // Custom Search

            #region Custom Search

            if ( csm.ClientId > 0 )
            {
                query = $"{query} AND (ch.ClientId=@csmClientId) ";
            }

            if ( csm.BalanceStatus != BalanceStatus.None )
            {
                query = $"{query} AND (ch.[BalanceStatus]=@csmBalanceStatus) ";
            }

            if ( csm.ReconciliationStatus != ReconciliationStatus.All )
            {
                query = $"{query} AND (ch.[Status]=@csmReconciliationStatus) ";
            }

            if ( csm.FromDate.HasValue && csm.ToDate.HasValue )
            {
                query = $"{query} AND (ch.EffectiveDate >= @csmFromDate AND ch.EffectiveDate <= @csmToDate) ";
            }
            else if ( csm.FromDate.HasValue || csm.ToDate.HasValue )
            {
                if ( csm.FromDate.HasValue )
                {
                    query = $"{query} AND (ch.EffectiveDate>=@csmFromDate) ";
                }
                if ( csm.ToDate.HasValue )
                {
                    query = $"{query} AND (ch.EffectiveDate<=@csmToDate) ";
                }
            }

            #endregion

            query = $@"{query} GROUP BY
	                            c.[Id],
	                            c.[CompanyName] ";

            return context.Database.SqlQuery<SimpleOutstandingPallets>( query, parameters.ToArray() ).ToList();
        }

        public List<SimpleOutstandingPallets> ListOutstandingPalletsPerSite( CustomSearchModel csm )
        {
            // Parameters

            #region Parameters

            List<object> parameters = new List<object>()
            {
                { new SqlParameter( "csmToDate", csm.ToDate ?? ( object ) DBNull.Value ) },
                { new SqlParameter( "userid", ( CurrentUser != null ) ? CurrentUser.Id : 0 ) },
                { new SqlParameter( "csmFromDate", csm.FromDate ?? ( object ) DBNull.Value ) },
                { new SqlParameter( "csmReconciliationStatus", ( int ) csm.ReconciliationStatus ) },
            };

            #endregion

            string query = @"SELECT
	                        s.[Id] AS [SiteId],
	                        s.[Name] AS [SiteName],
	                        SUM(ch.[NewQuantity] - cl.[NewQuantity]) AS [Total]
                         FROM
	                        [dbo].[ClientLoad] cl,
	                        [dbo].[ChepClient] cc,
	                        [dbo].[ChepLoad] ch,
	                        [dbo].[ClientSite] cs,
	                        [dbo].[Site] s
                         WHERE
	                        (cl.[Id]=cc.[ClientLoadsId]) AND
	                        (ch.[Id]=cc.[ChepLoadsId]) AND
	                        (cs.[Id]=cl.[ClientSiteId]) AND
	                        (s.[Id]=cs.[SiteId]) AND
	                        (cl.[Status]=0)";

            // Custom Search

            #region Custom Search

            if ( csm.ClientId > 0 )
            {
                query = $"{query} AND (cl.ClientId=@csmClientId) ";
            }

            if ( csm.FromDate.HasValue && csm.ToDate.HasValue )
            {
                query = $"{query} AND (cl.LoadDate >= @csmFromDate AND cl.LoadDate <= @csmToDate) ";
            }
            else if ( csm.FromDate.HasValue || csm.ToDate.HasValue )
            {
                if ( csm.FromDate.HasValue )
                {
                    query = $"{query} AND (cl.LoadDate>=@csmFromDate) ";
                }
                if ( csm.ToDate.HasValue )
                {
                    query = $"{query} AND (cl.LoadDate<=@csmToDate) ";
                }
            }

            query = $@"{query} GROUP BY
	                           s.[Id],
	                           s.[Name] ";

            #endregion

            return context.Database.SqlQuery<SimpleOutstandingPallets>( query, parameters.ToArray() ).ToList();
        }


        /// <summary>
        /// Gets a list of Items matching the specified search params
        /// </summary>
        /// <param name="pm"></param>
        /// <param name="csm"></param>
        /// <returns></returns>
        public List<ClientLoadCustomModel> ListOutstandingShipments( PagingModel pm, CustomSearchModel csm )
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
                { new SqlParameter( "csmReconciliation", ( int ) csm.ReconciliationStatus ) },
            };

            #endregion

            string query = @"SELECT
                                cl.*,
                                (SELECT COUNT(1) FROM [dbo].[Image] i WHERE cl.Id=i.ObjectId AND i.ObjectType IN('PCNNumber', 'PODNumber', 'PRNNumber')) AS [ImageCount]
                              FROM
                                [dbo].[ClientLoad] cl";

            // WHERE

            #region WHERE

            query = $"{query} WHERE (1=1)";
            //query = $"{query} WHERE (cl.[PCNNumber] IS NULL OR cl.[PODNumber] IS NULL OR cl.[PRNNumber] IS NULL)";

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

            if ( csm.ClientId > 0 )
            {
                query = $"{query} AND (cl.ClientId=@csmClientId)";
            }

            if ( csm.FromDate.HasValue && csm.ToDate.HasValue )
            {
                query = $"{query} AND (cl.CreatedOn >= @csmFromDate AND cl.CreatedOn <= @csmToDate) ";
            }
            else if ( csm.FromDate.HasValue || csm.ToDate.HasValue )
            {
                if ( csm.FromDate.HasValue )
                {
                    query = $"{query} AND (cl.CreatedOn>=@csmFromDate) ";
                }
                if ( csm.ToDate.HasValue )
                {
                    query = $"{query} AND (cl.CreatedOn<=@csmToDate) ";
                }
            }

            if ( csm.ReconciliationStatus != ReconciliationStatus.All )
            {
                query = $"{query} AND (cl.Status=@csmReconciliation)";
            }


            #endregion

            // Normal Search

            #region Normal Search

            if ( !string.IsNullOrEmpty( csm.Query ) )
            {
                query = string.Format( @"{0} AND (cl.[ReferenceNumber] LIKE '%{1}%' OR
                                                  cl.[DeliveryNote] LIKE '%{1}%' OR
                                                  cl.[ClientDescription] LIKE '%{1}%' OR
                                                  cl.[Equipment] LIKE '%{1}%' OR
                                                  cl.[LoadNumber] LIKE '%{1}%' OR
                                                  cl.[ReceiverNumber] LIKE '%{1}%' OR
                                                  cl.[AccountNumber] LIKE '%{1}%' OR
                                                  cl.[PODNumber] LIKE '%{1}%' OR
                                                  cl.[PCNNumber] LIKE '%{1}%' OR
                                                  cl.[PRNNumber] LIKE '%{1}%'
                                                 ) ", query, csm.Query.Trim() );
            }

            #endregion

            // ORDER

            query = $"{query} ORDER BY {pm.SortBy}";

            List<ClientLoadCustomModel> model = context.Database.SqlQuery<ClientLoadCustomModel>( query, parameters.ToArray() ).ToList();

            if ( model.NullableAny( p => p.ImageCount > 0 ) )
            {
                foreach ( ClientLoadCustomModel item in model.Where( p => p.ImageCount > 0 ) )
                {
                    item.Images = context.Images.Where( i => i.ObjectId == item.Id && new string[] { "PCNNumber", "PODNumber", "PRNNumber" }.Contains( i.ObjectType ) ).ToList();
                }
            }

            return model;
        }

        /// <summary>
        /// Gets a Client Load by UID
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public ClientLoad GetByUID( string uid )
        {
            return context.ClientLoads.FirstOrDefault( cl => cl.UID == uid );
        }

        /// <summary>
        /// Gets the total number of loads that can automatically be reconcilled
        /// </summary>
        /// <returns></returns>
        public int GetAutoReconcilliableLoadTotal()
        {
            List<object> parameters = new List<object>()
            {
                { new SqlParameter( "ReconciliationStatus", ( object ) ( int ) ReconciliationStatus.Unreconciled ) },
            };

            string query = @"SELECT
	                            COUNT(1) AS [Total]
                             FROM 
	                            ClientLoad cl 
                             WHERE 
	                            cl.[Status]=@ReconciliationStatus AND
	                            EXISTS
	                            (
		                            SELECT
			                            1
		                            FROM
			                            ChepLoad ch
		                            WHERE
			                            RTRIM(LTRIM(ch.[Ref]))=RTRIM(LTRIM(cl.[ReceiverNumber])) OR
			                            RTRIM(LTRIM(ch.[OtherRef]))=RTRIM(LTRIM(cl.[ReceiverNumber]))
	                            )";

            context.Database.CommandTimeout = 3600;

            CountModel model = context.Database.SqlQuery<CountModel>( query, parameters.ToArray() ).FirstOrDefault();

            return model.Total;
        }

        /// <summary>
        /// Gets a list of loads that can automatically be reconcilled
        /// </summary>
        /// <returns></returns>
        public List<ClientLoad> GetAutoReconcilliableLoads()
        {
            context.Database.CommandTimeout = 3600;

            return context.ClientLoads.Where( cl => cl.Status == ( int ) ReconciliationStatus.Unreconciled && context.ChepLoads.Any( ch => ch.Ref.Trim() == cl.ReceiverNumber.Trim() || ch.OtherRef.Trim() == cl.ReceiverNumber.Trim() ) ).ToList();
        }

        /// <summary>
        /// Gets a list of Client Loads using the specified ref or otherref
        /// </summary>
        /// <param name="reference"></param>
        /// <param name="otherRef"></param>
        /// <returns></returns>
        public List<ClientLoad> ListByChepRefOtherRef( string reference, string otherRef )
        {
            context.Database.CommandTimeout = 3600;
            context.Configuration.LazyLoadingEnabled = true;
            context.Configuration.ProxyCreationEnabled = true;

            return context.ClientLoads.Where( cl => cl.ReceiverNumber.Trim() == reference.Trim() || cl.ReceiverNumber == otherRef.Trim() ).ToList();
        }
    }
}

