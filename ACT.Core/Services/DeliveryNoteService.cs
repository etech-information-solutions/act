using System;
using ACT.Data.Models;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using ACT.Core.Enums;
using ACT.Core.Models;
using ACT.Core.Models.Custom;

namespace ACT.Core.Services
{
    public class DeliveryNoteService : BaseService<DeliveryNote>, IDisposable
    {
        public DeliveryNoteService()
        {

        }

        /// <summary>
        /// Gets a delivery note using the specified Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public override DeliveryNote GetById( int id )
        {
            context.Configuration.LazyLoadingEnabled = true;
            context.Configuration.ProxyCreationEnabled = true;

            return base.GetById( id );
        }

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
                { new SqlParameter( "query", csm.Query ?? ( object ) DBNull.Value ) },
                { new SqlParameter( "csmToDate", csm.ToDate ?? ( object ) DBNull.Value ) },
                { new SqlParameter( "userid", ( CurrentUser != null ) ? CurrentUser.Id : 0 ) },
                { new SqlParameter( "csmFromDate", csm.FromDate ?? ( object ) DBNull.Value ) },
            };

            #endregion

            string query = @"SELECT
	                            COUNT(d.[Id]) AS [Total]
                             FROM
	                            [dbo].[DeliveryNote] d
                                INNER JOIN [dbo].[Client] c ON c.[Id]=d.[ClientId]
                                LEFT OUTER JOIN [dbo].[ClientSite] cs ON cs.[Id]=d.[ClientSiteId]
                                LEFT OUTER JOIN [dbo].[Site] s ON s.[Id]=cs.[SiteId]";

            // WHERE

            #region WHERE

            query = $"{query} WHERE (1=1)";

            // Limit to only show Disputes for logged in user
            if ( CurrentUser.RoleType == RoleType.PSP )
            {
                query = $@"{query} AND EXISTS(SELECT
                                                1
                                              FROM
                                                [dbo].[PSPUser] pu
                                              WHERE
                                                (pu.UserId=@userid) AND
                                                (EXISTS(SELECT 1 FROM [dbo].[PSPClient] pc WHERE (pc.[PSPId]=pu.[PSPId] AND pc.[ClientId]=c.[Id])
                                             ) ";
            }
            else if ( CurrentUser.RoleType == RoleType.Client )
            {
                query = $@"{query} AND EXISTS(SELECT
                                                1
                                              FROM
                                                [dbo].[ClientUser] cu 
                                              WHERE
                                                (cu.UserId=@userid) AND
                                                (cu.ClientId=c.Id)
                                             ) ";
            }

            #endregion

            // Custom Search

            #region Custom Search

            if ( csm.ClientId != 0 )
            {
                query = $"{query} AND (d.[ClientId]=@csmClientId) ";
            }
            if ( csm.SiteId != 0 )
            {
                query = $"{query} AND (s.Id=@csmSiteId) ";
            }

            if ( csm.FromDate.HasValue && csm.ToDate.HasValue )
            {
                query = $"{query} AND (d.CreatedOn >= @csmFromDate AND d.CreatedOn <= @csmToDate) ";
            }
            else if ( csm.FromDate.HasValue || csm.ToDate.HasValue )
            {
                if ( csm.FromDate.HasValue )
                {
                    query = $"{query} AND (d.CreatedOn>=@csmFromDate) ";
                }
                if ( csm.ToDate.HasValue )
                {
                    query = $"{query} AND (d.CreatedOn<=@csmToDate) ";
                }
            }

            #endregion

            // Normal Search

            #region Normal Search

            if ( !string.IsNullOrEmpty( csm.Query ) )
            {
                query = string.Format( @"{0} AND (d.[CustomerName] LIKE '%{1}%' OR
                                                  d.[CustomerAddress] LIKE '%{1}%' OR
                                                  d.[InvoiceNumber] LIKE '%{1}%' OR
                                                  d.[OrderNumber] LIKE '%{1}%' OR
                                                  d.[Reference306] LIKE '%{1}%' OR
                                                  d.[EmailAddress] LIKE '%{1}%' OR
                                                  d.[BililngPostalCode] LIKE '%{1}%' OR
                                                  d.[DeliveryPostalCode] LIKE '%{1}%' OR
                                                  d.[CustomerPostalCode] LIKE '%{1}%' OR
                                                  d.[DeliveryAddress] LIKE '%{1}%' OR
                                                  d.[BillingAddress] LIKE '%{1}%' OR
                                                  s.[Name] LIKE '%{1}%' OR
                                                  s.[Description] LIKE '%{1}%' OR
                                                  c.[CompanyName] LIKE '%{1}%' OR
                                                  c.[TradingAs] LIKE '%{1}%'
                                             ) ", query, csm.Query.Trim() );
            }

            #endregion

            CountModel model = context.Database.SqlQuery<CountModel>( query, parameters.ToArray() ).FirstOrDefault();

            return model.Total;
        }

        /// <summary>
        /// Gets a list of Delivery Notes matching the specified search params
        /// </summary>
        /// <param name="pm"></param>
        /// <param name="csm"></param>
        /// <returns></returns>
        public List<DeliveryNoteCustomModel> List1( PagingModel pm, CustomSearchModel csm )
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
                { new SqlParameter( "query", csm.Query ?? ( object ) DBNull.Value ) },
                { new SqlParameter( "csmToDate", csm.ToDate ?? ( object ) DBNull.Value ) },
                { new SqlParameter( "userid", ( CurrentUser != null ) ? CurrentUser.Id : 0 ) },
                { new SqlParameter( "csmFromDate", csm.FromDate ?? ( object ) DBNull.Value ) },
            };

            #endregion

            string query = @"SELECT
	                            d.*,
	                            s.[Description] AS [SiteName],
	                            c.[CompanyName] AS [ClientName],
	                            (SELECT SUM(dnl.Returned) FROM [dbo].[DeliveryNoteLine] dnl WHERE dnl.[DeliveryNoteId]=d.[Id]) AS [ReturnedCountCount],
	                            (SELECT SUM(dnl.Delivered) FROM [dbo].[DeliveryNoteLine] dnl WHERE dnl.[DeliveryNoteId]=d.[Id]) AS [DeliveredCountCount],
	                            (SELECT SUM(dnl.OrderQuantity) FROM [dbo].[DeliveryNoteLine] dnl WHERE dnl.[DeliveryNoteId]=d.[Id]) AS [OrderedCountCount]
                             FROM
	                            [dbo].[DeliveryNote] d
                                INNER JOIN [dbo].[Client] c ON c.[Id]=d.[ClientId]
                                LEFT OUTER JOIN [dbo].[ClientSite] cs ON cs.[Id]=d.[ClientSiteId]
                                LEFT OUTER JOIN [dbo].[Site] s ON s.[Id]=cs.[SiteId]";

            // WHERE

            #region WHERE

            query = $"{query} WHERE (1=1)";

            // Limit to only show Disputes for logged in user
            if ( CurrentUser.RoleType == RoleType.PSP )
            {
                query = $@"{query} AND EXISTS(SELECT
                                                1
                                              FROM
                                                [dbo].[PSPUser] pu
                                              WHERE
                                                (pu.UserId=@userid) AND
                                                (EXISTS(SELECT 1 FROM [dbo].[PSPClient] pc WHERE (pc.[PSPId]=pu.[PSPId] AND pc.[ClientId]=c.[Id])
                                             ) ";
            }
            else if ( CurrentUser.RoleType == RoleType.Client )
            {
                query = $@"{query} AND EXISTS(SELECT
                                                1
                                              FROM
                                                [dbo].[ClientUser] cu 
                                              WHERE
                                                (cu.UserId=@userid) AND
                                                (cu.ClientId=c.Id)
                                             ) ";
            }

            #endregion

            // Custom Search

            #region Custom Search

            if ( csm.ClientId != 0 )
            {
                query = $"{query} AND (d.[ClientId]=@csmClientId) ";
            }
            if ( csm.SiteId != 0 )
            {
                query = $"{query} AND (s.Id=@csmSiteId) ";
            }

            if ( csm.FromDate.HasValue && csm.ToDate.HasValue )
            {
                query = $"{query} AND (d.CreatedOn >= @csmFromDate AND d.CreatedOn <= @csmToDate) ";
            }
            else if ( csm.FromDate.HasValue || csm.ToDate.HasValue )
            {
                if ( csm.FromDate.HasValue )
                {
                    query = $"{query} AND (d.CreatedOn>=@csmFromDate) ";
                }
                if ( csm.ToDate.HasValue )
                {
                    query = $"{query} AND (d.CreatedOn<=@csmToDate) ";
                }
            }

            #endregion

            // Normal Search

            #region Normal Search

            if ( !string.IsNullOrEmpty( csm.Query ) )
            {
                query = string.Format( @"{0} AND (d.[CustomerName] LIKE '%{1}%' OR
                                                  d.[CustomerAddress] LIKE '%{1}%' OR
                                                  d.[InvoiceNumber] LIKE '%{1}%' OR
                                                  d.[OrderNumber] LIKE '%{1}%' OR
                                                  d.[Reference306] LIKE '%{1}%' OR
                                                  d.[EmailAddress] LIKE '%{1}%' OR
                                                  d.[BililngPostalCode] LIKE '%{1}%' OR
                                                  d.[DeliveryPostalCode] LIKE '%{1}%' OR
                                                  d.[CustomerPostalCode] LIKE '%{1}%' OR
                                                  d.[DeliveryAddress] LIKE '%{1}%' OR
                                                  d.[BillingAddress] LIKE '%{1}%' OR
                                                  s.[Name] LIKE '%{1}%' OR
                                                  s.[Description] LIKE '%{1}%' OR
                                                  c.[CompanyName] LIKE '%{1}%' OR
                                                  c.[TradingAs] LIKE '%{1}%'
                                             ) ", query, csm.Query.Trim() );
            }

            #endregion

            // ORDER

            query = $"{query} ORDER BY {pm.SortBy} {pm.Sort}";

            // SKIP, TAKE

            query = string.Format( "{0} OFFSET (@skip) ROWS FETCH NEXT (@take) ROWS ONLY ", query );

            return context.Database.SqlQuery<DeliveryNoteCustomModel>( query, parameters.ToArray() ).ToList();
        }
    }
}
