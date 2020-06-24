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
        /// Gets a list of Items matching the specified search params
        /// </summary>
        /// <param name="pm"></param>
        /// <param name="csm"></param>
        /// <returns></returns>
        public List<ChepLoadCustomModel> ListCSM( PagingModel pm, CustomSearchModel csm )
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
                { new SqlParameter( "csmReconciliation", ( int ) csm.ReconciliationStatus ) },
                { new SqlParameter( "csmStatus", ( int ) csm.Status ) },
                //{ new SqlParameter( "clientid", clientId > 0 ? clientId : 0 ) },
            };

            #endregion

            string query = @"SELECT
                                p.*,
                                (SELECT COUNT(1) FROM [dbo].[Document] d WHERE p.Id=d.ObjectId AND d.ObjectType='ChepLoad') AS [DocumentCount]
                             FROM
                                [dbo].[ChepLoad] p";

            // WHERE

            #region WHERE

            query = $"{query} WHERE (1=1)";

            //if (CurrentUser.RoleType == RoleType.PSP)
            //{
            //    query = $"{query} AND EXISTS(SELECT 1 FROM [dbo].[PSPUser] pu, [dbo].[PSPClient] pc WHERE pu.[PSPId]=pc.[PSPId] AND pc.[ClientId]=cl.[ClientId] AND pu.[UserId]=@userid) ";
            //}
            //else if (CurrentUser.RoleType == RoleType.Client)
            //{
            //    query = $"{query} AND EXISTS(SELECT 1 FROM [dbo].[ClientUser] cu WHERE cu.[ClientId]=cl.[ClientId] AND cu.[UserId]=@userid) ";
            //}

            #endregion

            #region WHERE IF CLIENT

            if ( csm.ClientId > 0 )
            {
                //TODO: Give this attention for Client COntext
                //query = $"{query} AND EXISTS (SELECT Id FROM [dbo].[ClientProduct] cp WHERE cp.ProductId = p.Id AND cp.ClientId = @clientid)";
            }

            #endregion


            // Custom Search

            #region Custom Search

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


            if ( csm.ReconciliationStatus != ReconciliationStatus.Unreconcilable )
            {
                query = $"{query} AND (p.Status=@csmReconciliation)";
            }


            //if (csm.Status != Status.All)
            //{
            //    query = $"{query} AND (p.Status=@csmReconciliation)";
            //}

            if ( !string.IsNullOrEmpty( csm.Name ) )
            {
                query = string.Format( @"{0} AND (p.ClientDescription LIKE '%{1}%' )", query, csm.Description );
            }

            if ( !string.IsNullOrEmpty( csm.ReferenceNumber ) )
            {
                query = string.Format( @"{0} AND (p.DocketNumber LIKE '%{1}%' OR p.ReferenceNumber LIKE '%{1}%' OR p.ReceiverNumber LIKE '%{1}%' OR p.AccountNumber LIKE '%{1}%' )", query, csm.ReferenceNumber );
            }

            if ( !string.IsNullOrEmpty( csm.ReferenceNumberOther ) )
            {
                query = string.Format( @"{0} AND (p.DocketNumber LIKE '%{1}%' OR p.ReferenceNumber LIKE '%{1}%' OR p.ReceiverNumber LIKE '%{1}%' OR p.AccountNumber LIKE '%{1}%' )", query, csm.ReferenceNumberOther );
            }

            if ( !string.IsNullOrEmpty( csm.Description ) )
            {
                query = string.Format( @"{0} AND (p.Equipment LIKE '%{1}%' OR p.ClientDescription LIKE '%{1}%' )", query, csm.Description );
            }

            #endregion

            // Normal Search

            #region Normal Search

            if ( !string.IsNullOrEmpty( csm.Query ) )
            {
                query = string.Format( @"{0} AND (p.[DocketNumber] LIKE '%{1}%' OR
                                                  p.[ReferenceNumber] LIKE '%{1}%' OR
                                                  p.[DeliveryNote] LIKE '%{1}%' OR
                                                  p.[ClientDescription] LIKE '%{1}%' OR
                                                  p.[Equipment] LIKE '%{1}%' OR
                                                  p.[ReceiverNumber] LIKE '%{1}%'
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
                using ( DocumentService dservice = new DocumentService() )
                {
                    foreach ( ChepLoadCustomModel item in model.Where( p => p.DocumentCount > 0 ) )
                    {
                        item.Documents = dservice.List( item.Id, "ChepLoad" );
                    }
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
            //if ( CurrentUser.RoleType == RoleType.PSP )
            //{
            //    query = $"{query} AND EXISTS(SELECT 1 FROM [dbo].[PSPUser] pu WHERE u.Id=pu.UserId AND pu.PSPId IN({string.Join( ",", CurrentUser.PSPs.Select( s => s.Id ) )})) ";
            //}
            //else if ( CurrentUser.RoleType == RoleType.Client )
            //{
            //    query = $"{query} AND EXISTS(SELECT 1 FROM [dbo].[ClientUser] cu WHERE u.Id=cu.UserId AND cu.ClientId IN({string.Join( ",", CurrentUser.Clients.Select( s => s.Id ) )})) ";
            //}

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
