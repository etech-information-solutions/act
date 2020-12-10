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
                { new SqlParameter( "query", csm.Query ?? ( object ) DBNull.Value ) },
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
                                INNER JOIN [dbo].[Client] c ON c.Id=cl.ClientId";

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

            if ( csm.ClientId > 0 )
            {
                query = $"{query} AND (c.Id=@cmsClientId)";
            }
            if ( csm.ReconciliationStatus != ReconciliationStatus.Unreconcilable )
            {
                query = $"{query} AND (cl.BalanceStatus=@csmReconciliationStatus)";
            }

            if ( csm.FromDate.HasValue && csm.ToDate.HasValue )
            {
                query = $"{query} AND (cl.CreatedOn >= @csmFromDate AND cl.CreatedOn <= @csmToDate) ";
            }
            else if ( csm.FromDate.HasValue || csm.ToDate.HasValue )
            {
                if ( csm.FromDate.HasValue )
                {
                    query = $"{query} AND (cl.CreatedOn >= @csmFromDate) ";
                }
                if ( csm.ToDate.HasValue )
                {
                    query = $"{query} AND (cl.CreatedOn <= @csmToDate) ";
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
                                                  c.[Email] LIKE '%{1}%' OR
                                                  c.[AdminEmail] LIKE '%{1}%' OR
                                                  c.[AdminPerson] LIKE '%{1}%' OR
                                                  c.[CompanyName] LIKE '%{1}%' OR
                                                  c.[ChepReference] LIKE '%{1}%' OR
                                                  c.[CompanyRegistrationNumber] LIKE '%{1}%'
                                                 ) ", query, csm.Query.Trim() );
            }

            #endregion

            // ORDER

            query = $"{query} ORDER BY {pm.SortBy} {pm.Sort}";

            // SKIP, TAKE

            query = string.Format( "{0} OFFSET (@skip) ROWS FETCH NEXT (@take) ROWS ONLY ", query );

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
                { new SqlParameter( "csmToDate", csm.ToDate ?? ( object ) DBNull.Value ) },
                { new SqlParameter( "userid", ( CurrentUser != null ) ? CurrentUser.Id : 0 ) },
                { new SqlParameter( "csmFromDate", csm.FromDate ?? ( object ) DBNull.Value ) },
                { new SqlParameter( "csmReconciliationStatus", ( int ) csm.ReconciliationStatus ) },
            };

            #endregion

            string query = @"SELECT
                                cl.*,
                                c.CompanyName AS [ClientName],
                                (SELECT COUNT(1) FROM [dbo].[Document] d WHERE cl.Id=d.ObjectId AND d.ObjectType='ChepLoad') AS [DocumentCount]
                             FROM
                                [dbo].[ChepLoad] cl
                                INNER JOIN [dbo].[Client] c ON c.Id=cl.ClientId";

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

            if ( csm.ClientId > 0 )
            {
                query = $"{query} AND (c.Id=@cmsClientId)";
            }
            if ( csm.ReconciliationStatus != ReconciliationStatus.Unreconcilable )
            {
                query = $"{query} AND (cl.BalanceStatus=@csmReconciliationStatus)";
            }

            if ( csm.FromDate.HasValue && csm.ToDate.HasValue )
            {
                query = $"{query} AND (cl.CreatedOn >= @csmFromDate AND cl.CreatedOn <= @csmToDate) ";
            }
            else if ( csm.FromDate.HasValue || csm.ToDate.HasValue )
            {
                if ( csm.FromDate.HasValue )
                {
                    query = $"{query} AND (cl.CreatedOn >= @csmFromDate) ";
                }
                if ( csm.ToDate.HasValue )
                {
                    query = $"{query} AND (cl.CreatedOn <= @csmToDate) ";
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
                                                  c.[Email] LIKE '%{1}%' OR
                                                  c.[AdminEmail] LIKE '%{1}%' OR
                                                  c.[AdminPerson] LIKE '%{1}%' OR
                                                  c.[CompanyName] LIKE '%{1}%' OR
                                                  c.[ChepReference] LIKE '%{1}%' OR
                                                  c.[CompanyRegistrationNumber] LIKE '%{1}%'
                                                 ) ", query, csm.Query.Trim() );
            }

            #endregion

            // ORDER

            query = $"{query} ORDER BY {pm.SortBy} {pm.Sort}";

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

            query = $"SELECT cl.[Id] AS [TKey], cl.[AccountNumber] + ' (Docket # ' + cl.[DocketNumber] + ')' AS [TValue] FROM [dbo].[ChepLoad] cl WHERE (1=1)";

            // Limit to only show PSP for logged in user
            if ( CurrentUser.RoleType == RoleType.PSP )
            {
                query = $"{query} AND EXISTS(SELECT 1 FROM [dbo].[PSPUser] pu WHERE u.Id=pu.UserId AND pu.PSPId IN({string.Join( ",", CurrentUser.PSPs.Select( s => s.Id ) )})) ";
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
    }
}
