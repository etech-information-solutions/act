using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using ACT.Core.Enums;
using ACT.Core.Models;
using ACT.Data.Models;

namespace ACT.Core.Services
{
    public class PSPProductService : BaseService<PSPProduct>, IDisposable
    {
        public PSPProductService()
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

            query = $"SELECT p.Id AS [TKey], p.Name AS [TValue] FROM [dbo].[PSPProduct] p WHERE (1=1)";

            #region WHERE

            if ( CurrentUser.RoleType == RoleType.PSP )
            {
                query = $@"{query} AND EXISTS(SELECT
                                                1
                                              FROM
                                                [dbo].[PSPUser] pu
                                              WHERE
                                                (pu.UserId=@userid) AND
                                                (p.[PSPId]=pu.[PSPId])
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
                                                            (pc.[PSPId]=p.[PSPId])
                                                       )
                                                )
                                             ) ";
            }

            #endregion

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
