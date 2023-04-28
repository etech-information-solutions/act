using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

using ACT.Core.Enums;
using ACT.Core.Models;
using ACT.Data.Models;

namespace ACT.Core.Services
{
    public class ProvinceService : BaseService<Province>, IDisposable
    {
        public ProvinceService()
        {

        }

        /// <summary>
        /// Gets a list of provinces using the specified params
        /// </summary>
        /// <param name="v"></param>
        /// <param name="countryId"></param>
        /// <returns></returns>
        public Dictionary<int, string> List( bool v, int countryId = 0 )
        {
            Dictionary<int, string> clientOptions = new Dictionary<int, string>();
            List<IntStringKeyValueModel> model = new List<IntStringKeyValueModel>();

            List<object> parameters = new List<object>()
            {
                { new SqlParameter( "countryid", countryId ) },
                { new SqlParameter( "userid", ( CurrentUser != null ) ? CurrentUser.Id : 0 ) },
            };

            string query = $"SELECT p.[Id] AS [TKey], p.[Description] AS [TValue] FROM [dbo].[Province] p WHERE (1=1)";

            if ( countryId > 0 )
            {
                query = $"{query} AND (p.[CountryId]=@countryid)";
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
        /// Gets a province id using the specified name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public int? GetIdByName( string name )
        {
            return context.Provinces.FirstOrDefault( p => p.Name == name || p.Description == name )?.Id;
        }

        public Province GetProvinceName(int? id)
        {

            Province province = (from a in context.Provinces where a.Id == id select a).FirstOrDefault();
            return province;
        }

    }
}
