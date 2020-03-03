using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using ACT.Core.Enums;
using ACT.Core.Models;
using ACT.Data.Models;

namespace ACT.Core.Services
{
    public class ClientService : BaseService<Client>, IDisposable
    {
        public ClientService()
        {

        }

        /// <summary>
        /// Gets a list of clients
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

            string query = string.Empty;

            query = $"SELECT c.Id AS [TKey], c.CompanyName AS [TValue] FROM [dbo].[Client] c WHERE (1=1)";

            if ( !CurrentUser.IsAdmin )
            {
                query = $"{query} AND EXISTS(SELECT 1 FROM [dbo].[ClientUser] cu WHERE cu.UserId=@userid AND cu.UserId=c.Id)";
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

        public bool ExistByCompanyRegistrationNumber( string registrationNumber )
        {
            return context.Clients.Any( c => c.CompanyRegistrationNumber == registrationNumber );
        }
    }
}
