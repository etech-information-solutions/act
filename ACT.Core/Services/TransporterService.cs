using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

using ACT.Core.Models;
using ACT.Core.Models.Custom;
using ACT.Data.Models;

namespace ACT.Core.Services
{
    public class TransporterService : BaseService<Transporter>, IDisposable
    {
        public TransporterService()
        {

        }

        /// <summary>
        /// Gets a list of Campaign Purchases using the specified params
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
                { new SqlParameter( "userid", ( CurrentUser != null ) ? CurrentUser.Id : 0 ) },
                { new SqlParameter( "query", csm.Query ?? ( object ) DBNull.Value ) },
                { new SqlParameter( "csmToDate", csm.ToDate ?? ( object ) DBNull.Value ) },
                { new SqlParameter( "csmFromDate", csm.FromDate ?? ( object ) DBNull.Value ) },
            };

            #endregion

            string query = string.Format( @"SELECT t.* FROM [dbo].[Transporter] t" );

            // WHERE

            #region WHERE

            query = $"{query} WHERE (1=1)";

            #endregion

            // Custom Search

            #region Custom Search

            if ( csm.FromDate.HasValue && csm.ToDate.HasValue )
            {
                query = $"{query} AND (t.CreatedOn >= @csmFromDate AND t.CreatedOn <= @csmToDate) ";
            }
            else if ( csm.FromDate.HasValue || csm.ToDate.HasValue )
            {
                if ( csm.FromDate.HasValue )
                {
                    query = $"{query} AND (t.CreatedOn>=@csmFromDate) ";
                }
                if ( csm.ToDate.HasValue )
                {
                    query = $"{query} AND (t.CreatedOn<=@csmToDate) ";
                }
            }

            #endregion

            // Normal Search

            #region Normal Search

            if ( !string.IsNullOrEmpty( csm.Query ) )
            {
                query = string.Format( @"{0} AND (v.TradingName LIKE '%{1}%' OR
                                                  v.Name LIKE '%{1}%' OR
                                                  v.Email LIKE '%{1}%' OR
                                                  v.ContactNumber LIKE '%{1}%' OR
                                                  v.RegistrationNumber LIKE '%{1}%'
                                                  ) ", query, csm.Query.Trim() );
            }

            #endregion

            CountModel model = context.Database.SqlQuery<CountModel>( query.Trim(), parameters.ToArray() ).FirstOrDefault();

            return model.Total;
        }

        /// <summary>
        /// Gets a list of Campaign Purchases using the specified params
        /// </summary>
        /// <param name="pm"></param> 
        /// <param name="csm"></param> 
        /// <returns></returns>
        public List<TransporterCustomModel> List1( PagingModel pm, CustomSearchModel csm )
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
                                               t.*,
                                               (SELECT COUNT(1) FROM [dbo].[Contact] c WHERE c.ObjectType='Transporter' AND c.ObjectId=t.Id) AS ContactCount,
                                               (SELECT COUNT(1) FROM [dbo].[Vehicle] v WHERE v.ObjectType='Transporter' AND v.ObjectId=t.Id) AS VehicleCount
                                            FROM
                                               [dbo].[Transporter] t" );

            // WHERE

            #region WHERE

            query = $"{query} WHERE (1=1)";

            #endregion

            // Custom Search

            #region Custom Search

            if ( csm.FromDate.HasValue && csm.ToDate.HasValue )
            {
                query = $"{query} AND (t.CreatedOn >= @csmFromDate AND t.CreatedOn <= @csmToDate) ";
            }
            else if ( csm.FromDate.HasValue || csm.ToDate.HasValue )
            {
                if ( csm.FromDate.HasValue )
                {
                    query = $"{query} AND (t.CreatedOn>=@csmFromDate) ";
                }
                if ( csm.ToDate.HasValue )
                {
                    query = $"{query} AND (t.CreatedOn<=@csmToDate) ";
                }
            }

            #endregion

            // Normal Search

            #region Normal Search

            if ( !string.IsNullOrEmpty( csm.Query ) )
            {
                query = string.Format( @"{0} AND (v.TradingName LIKE '%{1}%' OR
                                                  v.Name LIKE '%{1}%' OR
                                                  v.Email LIKE '%{1}%' OR
                                                  v.ContactNumber LIKE '%{1}%' OR
                                                  v.RegistrationNumber LIKE '%{1}%'
                                                  ) ", query, csm.Query.Trim() );
            }

            #endregion

            // ORDER

            query = $"{query} ORDER BY {pm.SortBy} {pm.Sort}";

            // SKIP, TAKE

            query = string.Format( "{0} OFFSET (@skip) ROWS FETCH NEXT (@take) ROWS ONLY ", query );

            List<TransporterCustomModel> model = context.Database.SqlQuery<TransporterCustomModel>( query.Trim(), parameters.ToArray() ).ToList();

            if ( model.Any( t => t.ContactCount > 0 ) )
            {
                foreach ( TransporterCustomModel item in model.Where( t => t.ContactCount > 0 ) )
                {
                    item.Contacts = context.Contacts.Where( c => c.ObjectId == item.Id && c.ObjectType == "Transporter" ).ToList();
                }
            }
            if ( model.Any( t => t.VehicleCount > 0 ) )
            {
                foreach ( TransporterCustomModel item in model.Where( t => t.VehicleCount > 0 ) )
                {
                    item.Vehicles = context.Vehicles.Where( c => c.ObjectId == item.Id && c.ObjectType == "Transporter" ).ToList();
                }
            }

            return model;
        }

        /// <summary>
        /// Gets a list of Transporters
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public Dictionary<int, string> List( bool v )
        {
            Dictionary<int, string> clientOptions = new Dictionary<int, string>();
            List<IntStringKeyValueModel> model = new List<IntStringKeyValueModel>();

            List<object> parameters = new List<object>()
            {
                { new SqlParameter( "userid", ( CurrentUser != null ) ? CurrentUser.Id : 0 ) },
            };

            string query = $"SELECT c.Id AS [TKey], c.TradingName AS [TValue] FROM [dbo].[Transporter] c WHERE (1=1)";

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
    }
}
