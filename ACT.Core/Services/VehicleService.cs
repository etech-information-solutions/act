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
                query = string.Format( @"{0} AND (LOWER(REPLACE(v.Registration, ' ', '')) LIKE '%{1}%') ", query, csm.Query.Trim().ToLower() );
            }

            #endregion

            // ORDER

            query = $"{query} ORDER BY {pm.SortBy} {pm.Sort}";

            // SKIP, TAKE

            query = string.Format( "{0} OFFSET (@skip) ROWS FETCH NEXT (@take) ROWS ONLY ", query );

            return context.Database.SqlQuery<VehicleCustomModel>( query.Trim(), parameters.ToArray() ).ToList();
        }


        /// <summary>
        /// Gets a list of clients
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public Dictionary<int, string> List( bool v, int objectId = 0, string objectType = "" )
        {
            Dictionary<int, string> clientOptions = new Dictionary<int, string>();
            List<IntStringKeyValueModel> model = new List<IntStringKeyValueModel>();

            List<object> parameters = new List<object>()
            {
                { new SqlParameter( "objectId", objectId ) },
                { new SqlParameter( "objectType", objectType ) },
                { new SqlParameter( "userid", ( CurrentUser != null ) ? CurrentUser.Id : 0 ) },
            };

            string query = string.Empty;

            query = $"SELECT v.[Id] AS [TKey], v.[Descriptoin] AS [TValue] FROM [dbo].[Vehicle] v WHERE (1=1)";

            if ( objectId > 0 )
            {
                query = $"{query} AND v.ObjectId=@objectId";
            }
            if ( !string.IsNullOrEmpty( objectType ) )
            {
                query = $"{query} AND v.ObjectType=@objectType";
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
        /// Gets a list of contacts for the specified object
        /// </summary>
        /// <param name="objectId"></param>
        /// <param name="objectType"></param>
        /// <returns></returns>
        public List<Vehicle> List( int objectId, string objectType )
        {
            return context.Vehicles.Where( c => c.ObjectId == objectId && c.ObjectType == objectType ).ToList();
        }

        /// <summary>
        /// Gets a vehicle using the specified VIN and Object Type
        /// </summary>
        /// <param name="registration"></param>
        /// <param name="objectType"></param>
        /// <returns></returns>
        public Vehicle Get( string registration, string objectType )
        {
            return context.Vehicles.FirstOrDefault( v => v.Registration.Trim() == registration.Trim() && v.ObjectType == objectType );
        }

        /// <summary>
        /// Gets a vehicle using the specified Registration Number and Object Type
        /// </summary>
        /// <param name="reg"></param>
        /// <param name="objectType"></param>
        /// <returns></returns>
        public Vehicle GetByRegistrationNumber( string reg, string objectType )
        {
            return context.Vehicles.FirstOrDefault( v => v.Registration.Trim() == reg.Trim() && v.ObjectType == objectType );
        }

        /// <summary>
        /// Gets a vehicle using the specified Registration Number and Object Type
        /// </summary>
        /// <param name="reg"></param>
        /// <param name="objectType"></param>
        /// <returns></returns>
        public int? GetIdByRegistrationNumber( string reg, string objectType )
        {
            return context.Database.SqlQuery<int?>( $"SELECT v.[Id] FROM [dbo].[Vehicle] v WHERE v.[Registration]='{reg}' AND v.[ObjectType]='{objectType}';" ).FirstOrDefault();
        }
    }
}
