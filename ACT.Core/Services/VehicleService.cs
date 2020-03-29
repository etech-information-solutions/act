using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using ACT.Core.Models;
using ACT.Core.Models.Custom;
using ACT.Data.Models;

namespace ACT.Core.Services
{
    public class VehicleService : BaseService<Vehicle>, IDisposable
    {
        public VehicleService()
        {

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
                { new SqlParameter( "userid", ( CurrentUser != null ) ? CurrentUser.Id : 0 ) },
                { new SqlParameter( "query", csm.Query ?? ( object ) DBNull.Value ) },
                { new SqlParameter( "csmToDate", csm.ToDate ?? ( object ) DBNull.Value ) },
                { new SqlParameter( "csmFromDate", csm.FromDate ?? ( object ) DBNull.Value ) },
            };

            #endregion

            string query = string.Format( @"SELECT
	                                         COUNT(v.Id) AS [Total]
                                           FROM
	                                         [dbo].[Vehicle] v" );

            // WHERE

            #region WHERE

            query = $"{query} WHERE (1=1)";

            #endregion

            // Custom Search

            #region Custom Search

            if ( csm.FromDate.HasValue && csm.ToDate.HasValue )
            {
                query = $"{query} AND (v.CreatedOn >= @csmFromDate AND v.CreatedOn <= @csmToDate) ";
            }
            else if ( csm.FromDate.HasValue || csm.ToDate.HasValue )
            {
                if ( csm.FromDate.HasValue )
                {
                    query = $"{query} AND (v.CreatedOn>=@csmFromDate) ";
                }
                if ( csm.ToDate.HasValue )
                {
                    query = $"{query} AND (v.CreatedOn<=@csmToDate) ";
                }
            }

            #endregion

            // Normal Search

            #region Normal Search

            if ( !string.IsNullOrEmpty( csm.Query ) )
            {
                //query = string.Format( @"{0} AND (cp.DRReference LIKE '%{1}%' OR
                //                                  cp.Description LIKE '%{1}%' OR
                //                                  cp.PaymentReference LIKE '%{1}%' OR
                //                                  m.Name LIKE '%{1}%' OR
                //                                  m.Surname LIKE '%{1}%' OR
                //                                  r.Name LIKE '%{1}%' OR
                //                                  r.Region LIKE '%{1}%' OR
                //                                  m.MembershipNo LIKE '%{1}%' OR
                //                                  m.EmailAddress LIKE '%{1}%' OR
                //                                  m.Identification LIKE '%{1}%'
                //                             ) ", query, csm.Query.Trim() );
            }

            #endregion

            CountModel c = context.Database.SqlQuery<CountModel>( query.Trim(), parameters.ToArray() ).FirstOrDefault();

            return c.Total;
        }

        /// <summary>
        /// Gets a list of Campaign Purchases using the specified params
        /// </summary>
        /// <param name="pm"></param> 
        /// <param name="csm"></param> 
        /// <returns></returns>
        public List<VehicleCustomModel> ListCSM( PagingModel pm, CustomSearchModel csm )
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
                { new SqlParameter( "userid", ( CurrentUser != null ) ? CurrentUser.Id : 0 ) },
                { new SqlParameter( "query", csm.Query ?? ( object ) DBNull.Value ) },
                { new SqlParameter( "csmToDate", csm.ToDate ?? ( object ) DBNull.Value ) },
                { new SqlParameter( "csmFromDate", csm.FromDate ?? ( object ) DBNull.Value ) },
            };

            #endregion

            string query = string.Format( @"SELECT
	                                         v.*
                                           FROM
	                                         [dbo].[Vehicle] v" );

            // WHERE

            #region WHERE

            query = $"{query} WHERE (1=1)";

            #endregion

            // Custom Search

            #region Custom Search

            if ( csm.FromDate.HasValue && csm.ToDate.HasValue )
            {
                query = $"{query} AND (v.CreatedOn >= @csmFromDate AND v.CreatedOn <= @csmToDate) ";
            }
            else if ( csm.FromDate.HasValue || csm.ToDate.HasValue )
            {
                if ( csm.FromDate.HasValue )
                {
                    query = $"{query} AND (v.CreatedOn>=@csmFromDate) ";
                }
                if ( csm.ToDate.HasValue )
                {
                    query = $"{query} AND (v.CreatedOn<=@csmToDate) ";
                }
            }

            #endregion

            // Normal Search

            #region Normal Search

            if ( !string.IsNullOrEmpty( csm.Query ) )
            {
                //query = string.Format( @"{0} AND (cp.DRReference LIKE '%{1}%' OR
                //                                  cp.Description LIKE '%{1}%' OR
                //                                  cp.PaymentReference LIKE '%{1}%' OR
                //                                  m.Name LIKE '%{1}%' OR
                //                                  m.Surname LIKE '%{1}%' OR
                //                                  m.MembershipNo LIKE '%{1}%' OR
                //                                  m.EmailAddress LIKE '%{1}%' OR
                //                                  r.Name LIKE '%{1}%' OR
                //                                  r.Region LIKE '%{1}%' OR
                //                                  m.Identification LIKE '%{1}%'
                //                             ) ", query, csm.Query.Trim() );
            }

            #endregion

            // ORDER

            query = $"{query} ORDER BY {pm.SortBy} {pm.Sort}";

            // SKIP, TAKE

            query = string.Format( "{0} OFFSET (@skip) ROWS FETCH NEXT (@take) ROWS ONLY ", query );

            return context.Database.SqlQuery<VehicleCustomModel>( query.Trim(), parameters.ToArray() ).ToList();
        }
    }
}
