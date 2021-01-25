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
    public class ChepLoadService : BaseService<ChepLoad>, IDisposable
    {
        public ChepLoadService()
        {

        }

        /// <summary>
        /// Gets a total number of Items matching the specified search params
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
                { new SqlParameter( "cmsClientId", csm.ClientId ) },
                { new SqlParameter( "csmStatus", ( int ) csm.Status ) },
                { new SqlParameter( "csmBalanceStatus", ( int ) csm.BalanceStatus ) },
                { new SqlParameter( "query", csm.Query ?? ( object ) DBNull.Value ) },
                { new SqlParameter( "csmOutstandingReasonId", csm.OutstandingReasonId ) },
                { new SqlParameter( "csmToDate", csm.ToDate ?? ( object ) DBNull.Value ) },
                { new SqlParameter( "userid", ( CurrentUser != null ) ? CurrentUser.Id : 0 ) },
                { new SqlParameter( "csmFromDate", csm.FromDate ?? ( object ) DBNull.Value ) },
                { new SqlParameter( "csmReconciliationStatus", ( int ) csm.ReconciliationStatus ) },
            };

            #endregion

            string query = @"SELECT
                                COUNT(1) AS [Total]
                             FROM
                                [dbo].[ChepLoad] cl
                                INNER JOIN [dbo].[Client] c ON c.Id=cl.ClientId
                                LEFT OUTER JOIN [dbo].[OutstandingReason] r ON r.Id=cl.OutstandingReasonId";

            // WHERE

            #region WHERE

            query = $"{ query} WHERE (1=1)";

            if ( CurrentUser.RoleType == RoleType.PSP )
            {
                query = $"{query} AND EXISTS(SELECT 1 FROM [dbo].[PSPUser] pu, [dbo].[PSPClient] pc WHERE pu.[PSPId]=pc.[PSPId] AND pc.[ClientId]=c.[Id] AND pu.[UserId]=@userid) ";
            }

            #endregion

            // Custom Search

            #region Custom Search

            if ( csm.IsOP )
            {
                query = $"{query} AND (cl.Quantity > 0)";
                query = $"{query} AND (cl.Ref LIKE '5000%')";
            }
            if ( csm.ClientId > 0 )
            {
                query = $"{query} AND (c.Id=@cmsClientId)";
            }
            if ( csm.BalanceStatus != BalanceStatus.None )
            {
                query = $"{query} AND (cl.BalanceStatus=@csmBalanceStatus)";
            }
            if ( csm.ReconciliationStatus != ReconciliationStatus.All )
            {
                query = $"{query} AND (cl.Status=@csmReconciliationStatus)";
            }
            if ( csm.OutstandingReasonId > 0 )
            {
                query = $"{query} AND (cl.OutstandingReasonId=@csmOutstandingReasonId)";
            }

            if ( csm.FromDate.HasValue && csm.ToDate.HasValue )
            {
                query = $"{query} AND (cl.ShipmentDate >= @csmFromDate AND cl.ShipmentDate <= @csmToDate) ";
            }
            else if ( csm.FromDate.HasValue || csm.ToDate.HasValue )
            {
                if ( csm.FromDate.HasValue )
                {
                    query = $"{query} AND (cl.ShipmentDate >= @csmFromDate) ";
                }
                if ( csm.ToDate.HasValue )
                {
                    query = $"{query} AND (cl.ShipmentDate <= @csmToDate) ";
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
                                                  c.[Email] LIKE '%{1}%' OR
                                                  c.[AdminEmail] LIKE '%{1}%' OR
                                                  c.[AdminPerson] LIKE '%{1}%' OR
                                                  c.[CompanyName] LIKE '%{1}%' OR
                                                  c.[ChepReference] LIKE '%{1}%' OR
                                                  c.[CompanyRegistrationNumber] LIKE '%{1}%' OR
                                                  r.[Description] LIKE '%{1}%'
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
        public List<ChepLoadCustomModel> List1( PagingModel pm, CustomSearchModel csm )
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
                { new SqlParameter( "cmsClientId", csm.ClientId ) },
                { new SqlParameter( "csmStatus", ( int ) csm.Status ) },
                { new SqlParameter( "query", csm.Query ?? ( object ) DBNull.Value ) },
                { new SqlParameter( "csmBalanceStatus", ( int ) csm.BalanceStatus ) },
                { new SqlParameter( "csmOutstandingReasonId", csm.OutstandingReasonId ) },
                { new SqlParameter( "csmToDate", csm.ToDate ?? ( object ) DBNull.Value ) },
                { new SqlParameter( "userid", ( CurrentUser != null ) ? CurrentUser.Id : 0 ) },
                { new SqlParameter( "csmFromDate", csm.FromDate ?? ( object ) DBNull.Value ) },
                { new SqlParameter( "csmReconciliationStatus", ( int ) csm.ReconciliationStatus ) },
            };

            #endregion

            string query = @"SELECT
                                cl.*,
                                c.[CompanyName] AS [ClientName],
                                r.[Description] AS [OutstandingReason],
                                (SELECT COUNT(1) FROM [dbo].[Document] d WHERE cl.Id=d.ObjectId AND d.ObjectType='ChepLoad') AS [DocumentCount],
                                (SELECT COUNT(1) FROM [dbo].[Comment] d WHERE cl.Id=d.ObjectId AND d.ObjectType='ChepLoad') AS [CommentCount]
                             FROM
                                [dbo].[ChepLoad] cl
                                INNER JOIN [dbo].[Client] c ON c.Id=cl.ClientId
                                LEFT OUTER JOIN [dbo].[OutstandingReason] r ON r.Id=cl.OutstandingReasonId";

            // WHERE

            #region WHERE

            query = $"{query} WHERE (1=1)";

            if ( CurrentUser.RoleType == RoleType.PSP )
            {
                query = $"{query} AND EXISTS(SELECT 1 FROM [dbo].[PSPUser] pu, [dbo].[PSPClient] pc WHERE pu.[PSPId]=pc.[PSPId] AND pc.[ClientId]=c.[Id] AND pu.[UserId]=@userid) ";
            }

            #endregion

            // Custom Search

            #region Custom Search

            if ( csm.IsOP )
            {
                query = $"{query} AND (cl.Quantity > 0)";
                query = $"{query} AND (cl.Ref LIKE '5000%')";
            }
            if ( csm.ClientId > 0 )
            {
                query = $"{query} AND (c.Id=@cmsClientId)";
            }
            if ( csm.BalanceStatus != BalanceStatus.None )
            {
                query = $"{query} AND (cl.BalanceStatus=@csmBalanceStatus)";
            }
            if ( csm.ReconciliationStatus != ReconciliationStatus.All )
            {
                query = $"{query} AND (cl.Status=@csmReconciliationStatus)";
            }
            if ( csm.OutstandingReasonId > 0 )
            {
                query = $"{query} AND (cl.OutstandingReasonId=@csmOutstandingReasonId)";
            }

            if ( csm.FromDate.HasValue && csm.ToDate.HasValue )
            {
                query = $"{query} AND (cl.ShipmentDate >= @csmFromDate AND cl.ShipmentDate <= @csmToDate) ";
            }
            else if ( csm.FromDate.HasValue || csm.ToDate.HasValue )
            {
                if ( csm.FromDate.HasValue )
                {
                    query = $"{query} AND (cl.ShipmentDate >= @csmFromDate) ";
                }
                if ( csm.ToDate.HasValue )
                {
                    query = $"{query} AND (cl.ShipmentDate <= @csmToDate) ";
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
                                                  c.[Email] LIKE '%{1}%' OR
                                                  c.[AdminEmail] LIKE '%{1}%' OR
                                                  c.[AdminPerson] LIKE '%{1}%' OR
                                                  c.[CompanyName] LIKE '%{1}%' OR
                                                  c.[ChepReference] LIKE '%{1}%' OR
                                                  c.[CompanyRegistrationNumber] LIKE '%{1}%' OR
                                                  r.[Description] LIKE '%{1}%'
                                                 ) ", query, csm.Query.Trim() );
            }

            #endregion

            // ORDER

            query = $"{query} ORDER BY {( pm.SortBy.Contains( "." ) ? pm.SortBy : "cl." + pm.SortBy )} {pm.Sort}";

            // SKIP, TAKE

            query = string.Format( "{0} OFFSET (@skip) ROWS FETCH NEXT (@take) ROWS ONLY ", query );

            List<ChepLoadCustomModel> model = context.Database.SqlQuery<ChepLoadCustomModel>( query, parameters.ToArray() ).ToList();

            if ( model.NullableAny( p => p.DocumentCount > 0 ) )
            {
                foreach ( ChepLoadCustomModel item in model.Where( p => p.DocumentCount > 0 ) )
                {
                    item.Documents = context.Documents.Where( d => d.ObjectId == item.Id && d.ObjectType == "ChepLoad" ).ToList();
                }
            }

            if ( model.NullableAny( p => p.CommentCount > 0 ) )
            {
                foreach ( ChepLoadCustomModel item in model.Where( p => p.CommentCount > 0 ) )
                {
                    item.Comments = context.Comments.Include( "User" ).Where( d => d.ObjectId == item.Id && d.ObjectType == "ChepLoad" ).ToList();
                }
            }

            return model;
        }


        /// <summary>
        /// Gets a list of Chep Loads
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public Dictionary<int, string> List( bool v )
        {
            List<IntStringKeyValueModel> model;
            Dictionary<int, string> chepLoadOptions = new Dictionary<int, string>();

            List<object> parameters = new List<object>()
            {
                { new SqlParameter( "actStatus", Status.Active ) },
            };

            string query = string.Empty;

            query = $"SELECT cl.[Id] AS [TKey], cl.[Ref] + ' (Docket # ' + cl.[DocketNumber] + ')' AS [TValue] FROM [dbo].[ChepLoad] cl INNER JOIN [dbo].[Client] c ON c.Id=cl.ClientId WHERE (1=1)";

            // Limit to only show PSP for logged in user
            if ( CurrentUser.RoleType == RoleType.PSP )
            {
                query = $"{query} AND EXISTS(SELECT 1 FROM [dbo].[PSPUser] pu, [dbo].[PSPClient] pc WHERE pu.[PSPId]=pc.[PSPId] AND pc.[ClientId]=c.[Id] AND pu.[UserId]=@userid) ";
            }

            model = context.Database.SqlQuery<IntStringKeyValueModel>( query.Trim(), parameters.ToArray() ).ToList();

            if ( model != null && model.Any() )
            {
                foreach ( var k in model )
                {
                    if ( chepLoadOptions.Keys.Any( x => x == k.TKey ) )
                        continue;

                    chepLoadOptions.Add( k.TKey, ( k.TValue ?? "" ).Trim() );
                }
            }

            return chepLoadOptions;
        }

        /// <summary>
        /// Gets a ChepLoad using the specified docketNumber
        /// </summary>
        /// <param name="docketNumber"></param>
        /// <returns></returns>
        public ChepLoad GetByDocketNumber( string docketNumber )
        {
            return context.ChepLoads.FirstOrDefault( cl => cl.DocketNumber.Trim() == docketNumber );
        }

        /// <summary>
        /// Gets a list of ChepLoads using the specified Ref #
        /// </summary>
        /// <param name="reference"></param>
        /// <returns></returns>
        public List<ChepLoad> ListByReference( string reference, bool specific )
        {
            return context.ChepLoads.Where( ch => ch.Ref.Trim() == reference.Trim() ).ToList();
        }

        /// <summary>
        /// Gets a ChepLoad using the specified clientId and docketNumber
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="docketNumber"></param>
        /// <returns></returns>
        public ChepLoad Get( int clientId, string docketNumber )
        {
            return context.ChepLoads.FirstOrDefault( ch => ch.ClientId == clientId && ch.DocketNumber.Trim() == docketNumber.Trim() );
        }

        /// <summary>
        /// Gets a ChepLoad using the specified clientId, Transaction Type and docketNumber
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="docketNumber"></param>
        /// <param name="transactionType"></param>
        /// <returns></returns>
        public ChepLoad Get( int clientId, string docketNumber, string transactionType )
        {
            return context.ChepLoads.FirstOrDefault( ch => ch.ClientId == clientId && ch.DocketNumber.Trim() == docketNumber.Trim() && ch.TransactionType.Trim() == transactionType.Trim() );
        }

        /// <summary>
        /// Gets a list of Chep loads matching the client load receiver number
        /// </summary>
        /// <param name="receiverNumber"></param>
        /// <returns></returns>
        public List<ChepLoadCustomModel> ListClientLoadMatch( string receiverNumber )
        {
            string q = $"SELECT ch.Id, ch.Quantity FROM [dbo].[ChepLoad] ch WHERE RTRIM(LTRIM(ch.Ref))='{receiverNumber}' OR RTRIM(LTRIM(ch.OtherRef))='{receiverNumber}';";

            return context.Database.SqlQuery<ChepLoadCustomModel>( q ).ToList();
        }

        /// <summary>
        /// Gets a list of ChepLoads using the specified Ref #
        /// </summary>
        /// <param name="reference"></param>
        /// <returns></returns>
        public List<ChepLoad> ListByReference( string reference )
        {
            return context.ChepLoads.Where( ch => ch.Ref.Trim() == reference.Trim() || ch.OtherRef.Trim() == reference.Trim() ).ToList();
        }

        /// <summary>
        /// Gets a list of ChepLoads using the specified docket #
        /// </summary>
        /// <param name="docketNumber"></param>
        /// <returns></returns>
        public List<ChepLoad> ListByDocketNumber( string docketNumber )
        {
            return context.ChepLoads.Where( ch => ch.DocketNumber.Trim() == docketNumber.Trim() ).ToList();
        }

        /// <summary>
        /// Gets a list of ChepLoads using the specified Docket and Ref #
        /// </summary>
        /// <param name="docketNumber"></param>
        /// <param name="reference"></param>
        /// <returns></returns>
        public List<ChepLoad> ListDocketNumberAndReference( string docketNumber, string reference )
        {
            return context.ChepLoads.Where( ch => ch.DocketNumber.Trim() == docketNumber.Trim() || ch.Ref.Trim() == reference.Trim() || ch.OtherRef.Trim() == reference.Trim() ).ToList();
        }

        public List<ChepLoadCustomModel> ListTopOustandingCustomers( PagingModel pm, CustomSearchModel csm )
        {
            // Parameters

            #region Parameters

            List<object> parameters = new List<object>()
            {
                { new SqlParameter( "csmToDate", csm.ToDate ?? ( object ) DBNull.Value ) },
                { new SqlParameter( "userid", ( CurrentUser != null ) ? CurrentUser.Id : 0 ) },
                { new SqlParameter( "csmFromDate", csm.FromDate ?? ( object ) DBNull.Value ) },
                { new SqlParameter( "csmClientId", csm.ClientId ) },
                { new SqlParameter( "csmOutstandingReasonId", csm.OutstandingReasonId ) },
                { new SqlParameter( "csmBalanceStatus", ( int ) csm.BalanceStatus ) },
                { new SqlParameter( "csmReconciliationStatus", ( int ) csm.ReconciliationStatus ) },
            };

            #endregion

            string query = @"SELECT
	                            cl.*,
	                            c.[CompanyName] AS [ClientName],
	                            o.[Description] AS [OutstandingReason],
	                            r.[Description] AS [RegionName],
	                            s.[Description] AS [SiteName]
                             FROM
	                            [dbo].[ChepLoad] cl
	                            LEFT OUTER JOIN [dbo].[Client] c ON c.[Id]=cl.[ClientId]
	                            LEFT OUTER JOIN [dbo].[OutstandingReason] o ON o.[Id]=cl.[OutstandingReasonId]
	                            LEFT OUTER JOIN [dbo].[ClientSite] cs ON cs.[Id]=cl.[ClientSiteId]
	                            LEFT OUTER JOIN [dbo].[Site] s ON s.[Id]=cs.[SiteId]
	                            LEFT OUTER JOIN [dbo].[Region] r ON r.[Id]=s.[RegionId]
                              WHERE
	                            cl.[Status]=@csmReconciliationStatus AND
	                            cl.[BalanceStatus]=@csmBalanceStatus";

            // Custom Search

            #region Custom Search

            if ( csm.ClientId > 0 )
            {
                query = $"{query} AND (cl.ClientId=@csmClientId) ";
            }
            if ( csm.OutstandingReasonId > 0 )
            {
                query = $"{query} AND (cl.OutstandingReasonId=@csmOutstandingReasonId) ";
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

            // ORDER BY
            query = $@"{query} ORDER BY
	                               cl.[ClientId] ASC ";

            return context.Database.SqlQuery<ChepLoadCustomModel>( query, parameters.ToArray() ).ToList();
        }
    }
}
