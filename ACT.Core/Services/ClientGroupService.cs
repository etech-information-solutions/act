using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.SqlClient;
using ACT.Core.Enums;
using ACT.Core.Models;
using ACT.Data.Models;
using ACT.Core.Models.Custom;
using System.Data.Entity;

namespace ACT.Core.Services
{
    public class ClientGroupService : BaseService<ClientGroup>, IDisposable
    {
        public ClientGroupService()
        {

        }

        public Dictionary<int, string> List( bool activeOnly = false )
        {
            Dictionary<int, string> clientGroupOptions = new Dictionary<int, string>();
            List<IntStringKeyValueModel> model = new List<IntStringKeyValueModel>();

            string query = "SELECT cg.Id AS [TKey], cg.GroupName AS [TValue] FROM [dbo].[ClientGroup] cg WHERE (1=1)";

            if ( activeOnly )
            {
                query += " AND cg.Status = @status";
            }

            query += " ORDER BY cg.GroupName";

            var parameters = new List<object>
            {
                new SqlParameter("@status", (int)Status.Active)
            };

            model = context.Database.SqlQuery<IntStringKeyValueModel>( query, parameters.ToArray() ).ToList();

            if ( model != null && model.Any() )
            {
                foreach ( var k in model )
                {
                    if ( clientGroupOptions.Keys.Any( x => x == k.TKey ) )
                        continue;
                    clientGroupOptions.Add( k.TKey, ( k.TValue ?? "" ).Trim() );
                }
            }

            return clientGroupOptions;
        }

        public ClientGroup GetByClientId( int clientId )
        {
            return context.ClientGroups.FirstOrDefault( cg => cg.ClientId == clientId && cg.Status == ( int ) Status.Active );
        }

        public void SaveOrUpdate( ClientGroup clientGroup )
        {
            if ( clientGroup.Id == 0 )
            {
                context.ClientGroups.Add( clientGroup );
            }
            else
            {
                context.Entry( clientGroup ).State = EntityState.Modified;
            }
            context.SaveChanges();
        }
    }
}
