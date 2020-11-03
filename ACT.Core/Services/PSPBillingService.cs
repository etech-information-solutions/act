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
    public class PSPBillingService : BaseService<PSPBilling>, IDisposable
    {
        public PSPBillingService()
        {

        }

        /// <summary>
        /// Gets a total count of Billing items matching the specified search params
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
                { new SqlParameter( "csmPSPId", csm.PSPId ) },
                { new SqlParameter( "csmPSPProductId", csm.PSPProductId ) },
                { new SqlParameter( "query", csm.Query ?? ( object ) DBNull.Value ) },
                { new SqlParameter( "csmToDate", csm.ToDate ?? ( object ) DBNull.Value ) },
                { new SqlParameter( "userid", ( CurrentUser != null ) ? CurrentUser.Id : 0 ) },
                { new SqlParameter( "csmFromDate", csm.FromDate ?? ( object ) DBNull.Value ) },
            };

            #endregion

            string query = @"SELECT
	                            COUNT(b.[Id]) AS [Total]
                             FROM
	                            [dbo].[PSPBilling] b
                                INNER JOIN [dbo].[PSP] p ON p.[Id]=b.[PSPId]
                                INNER JOIN [dbo].[PSPProduct] p1 ON p1.[Id]=b.[PSPProductId]";

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
                                                (b.[PSPId]=pu.[PSPId])
                                                )
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
                                                (EXISTS(SELECT 1 FROM [dbo].[PSPClient] pc
                                                        WHERE
                                                            (pc.[ClientId]=cu.[ClientId]) AND
                                                            (pc.[PSPId]=b.[PSPId])
                                                       )
                                                )
                                             ) ";
            }

            #endregion

            // Custom Search

            #region Custom Search

            if ( csm.PSPId != 0 )
            {
                query = $"{query} AND (p.Id=@csmPSPId) ";
            }
            if ( csm.PSPProductId != 0 )
            {
                query = $"{query} AND (p1.Id=@csmPSPProductId) ";
            }

            if ( csm.FromDate.HasValue && csm.ToDate.HasValue )
            {
                query = $"{query} AND (b.CreatedOn >= @csmFromDate AND b.CreatedOn <= @csmToDate) ";
            }
            else if ( csm.FromDate.HasValue || csm.ToDate.HasValue )
            {
                if ( csm.FromDate.HasValue )
                {
                    query = $"{query} AND (b.CreatedOn>=@csmFromDate) ";
                }
                if ( csm.ToDate.HasValue )
                {
                    query = $"{query} AND (b.CreatedOn<=@csmToDate) ";
                }
            }

            #endregion

            // Normal Search

            #region Normal Search

            if ( !string.IsNullOrEmpty( csm.Query ) )
            {
                query = string.Format( @"{0} AND (b.[StatementNumber] LIKE '%{1}%' OR
                                                  b.[ReferenceNumber] LIKE '%{1}%' OR
                                                  b.[NominatedAccount] LIKE '%{1}%' OR
                                                  p.[CompanyRegistrationNumber] LIKE '%{1}%' OR
                                                  p.[CompanyName] LIKE '%{1}%' OR
                                                  p.[TradingAs] LIKE '%{1}%' OR
                                                  p.[Description] LIKE '%{1}%' OR
                                                  p.[VATNumber] LIKE '%{1}%' OR
                                                  p.[ContactNumber] LIKE '%{1}%' OR
                                                  p.[ContactPerson] LIKE '%{1}%' OR
                                                  p.[FinancialPerson] LIKE '%{1}%' OR
                                                  p.[Email] LIKE '%{1}%' OR
                                                  p.[AdminEmail] LIKE '%{1}%' OR
                                                  p1.[Name] LIKE '%{1}%' OR
                                                  p1.[Description] LIKE '%{1}%'
                                             ) ", query, csm.Query.Trim() );
            }

            #endregion

            CountModel model = context.Database.SqlQuery<CountModel>( query, parameters.ToArray() ).FirstOrDefault();

            return model.Total;
        }

        /// <summary>
        /// Gets a list of Billing items matching the specified search params
        /// </summary>
        /// <param name="pm"></param>
        /// <param name="csm"></param>
        /// <returns></returns>
        public List<PSPBillingCustomModel> List1( PagingModel pm, CustomSearchModel csm )
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
                { new SqlParameter( "csmPSPId", csm.PSPId ) },
                { new SqlParameter( "csmPSPProductId", csm.PSPProductId ) },
                { new SqlParameter( "query", csm.Query ?? ( object ) DBNull.Value ) },
                { new SqlParameter( "csmToDate", csm.ToDate ?? ( object ) DBNull.Value ) },
                { new SqlParameter( "userid", ( CurrentUser != null ) ? CurrentUser.Id : 0 ) },
                { new SqlParameter( "csmFromDate", csm.FromDate ?? ( object ) DBNull.Value ) },
            };

            #endregion

            string query = @"SELECT
	                            b.*,
	                            p.CompanyName AS [PSPName],
	                            p1.Name AS [ProductName]
                             FROM
	                            [dbo].[PSPBilling] b
                                INNER JOIN [dbo].[PSP] p ON p.[Id]=b.[PSPId]
                                INNER JOIN [dbo].[PSPProduct] p1 ON p1.[Id]=b.[PSPProductId]";

            // WHERE

            #region WHERE

            query = $"{query} WHERE (1=1)";

            // Limit to only show Disputes for logged in user
            if ( CurrentUser.RoleType == RoleType.PSP )
            {
                query = $@"{query} AND EXISTS (SELECT
                                                1
                                              FROM
                                                [dbo].[PSPUser] pu
                                              WHERE
                                                (pu.UserId=@userid) AND
                                                (b.[PSPId]=pu.[PSPId])
                                             ) ";
            }
            else if ( CurrentUser.RoleType == RoleType.Client )
            {
                query = $@"{query} AND EXISTS (SELECT
                                                1
                                              FROM
                                                [dbo].[ClientUser] cu 
                                              WHERE
                                                (cu.UserId=@userid) AND
                                                (EXISTS(SELECT 1 FROM [dbo].[PSPClient] pc
                                                        WHERE
                                                            (pc.[ClientId]=cu.[ClientId]) AND
                                                            (pc.[PSPId]=b.[PSPId])
                                                       )
                                                )
                                             ) ";
            }

            #endregion

            // Custom Search

            #region Custom Search

            if ( csm.PSPId != 0 )
            {
                query = $"{query} AND (p.Id=@csmPSPId) ";
            }
            if ( csm.PSPProductId != 0 )
            {
                query = $"{query} AND (p1.Id=@csmPSPProductId) ";
            }

            if ( csm.FromDate.HasValue && csm.ToDate.HasValue )
            {
                query = $"{query} AND (b.CreatedOn >= @csmFromDate AND b.CreatedOn <= @csmToDate) ";
            }
            else if ( csm.FromDate.HasValue || csm.ToDate.HasValue )
            {
                if ( csm.FromDate.HasValue )
                {
                    query = $"{query} AND (b.CreatedOn>=@csmFromDate) ";
                }
                if ( csm.ToDate.HasValue )
                {
                    query = $"{query} AND (b.CreatedOn<=@csmToDate) ";
                }
            }

            #endregion

            // Normal Search

            #region Normal Search

            if ( !string.IsNullOrEmpty( csm.Query ) )
            {
                query = string.Format( @"{0} AND (b.[StatementNumber] LIKE '%{1}%' OR
                                                  b.[ReferenceNumber] LIKE '%{1}%' OR
                                                  b.[NominatedAccount] LIKE '%{1}%' OR
                                                  p.[CompanyRegistrationNumber] LIKE '%{1}%' OR
                                                  p.[CompanyName] LIKE '%{1}%' OR
                                                  p.[TradingAs] LIKE '%{1}%' OR
                                                  p.[Description] LIKE '%{1}%' OR
                                                  p.[VATNumber] LIKE '%{1}%' OR
                                                  p.[ContactNumber] LIKE '%{1}%' OR
                                                  p.[ContactPerson] LIKE '%{1}%' OR
                                                  p.[FinancialPerson] LIKE '%{1}%' OR
                                                  p.[Email] LIKE '%{1}%' OR
                                                  p.[AdminEmail] LIKE '%{1}%' OR
                                                  p1.[Name] LIKE '%{1}%' OR
                                                  p1.[Description] LIKE '%{1}%'
                                             ) ", query, csm.Query.Trim() );
            }

            #endregion

            // ORDER

            query = $"{query} ORDER BY {pm.SortBy} {pm.Sort}";

            // SKIP, TAKE

            query = string.Format( "{0} OFFSET (@skip) ROWS FETCH NEXT (@take) ROWS ONLY ", query );

            return context.Database.SqlQuery<PSPBillingCustomModel>( query, parameters.ToArray() ).ToList();
        }
    }
}
