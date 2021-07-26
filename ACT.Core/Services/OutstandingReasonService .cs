using System;
using System.Collections.Generic;
using System.Linq;

using ACT.Core.Models;
using ACT.Data.Models;

namespace ACT.Core.Services
{
    public class OutstandingReasonService : BaseService<OutstandingReason>, IDisposable
    {
        public OutstandingReasonService()
        {

        }

        public Dictionary<int, string> List( bool v )
        {
            Dictionary<int, string> options = new Dictionary<int, string>();
            List<IntStringKeyValueModel> model = new List<IntStringKeyValueModel>();

            List<object> parameters = new List<object>();

            string query = $"SELECT o.Id AS [TKey], o.[Description] AS [TValue] FROM [dbo].[OutstandingReason] o WHERE (1=1)";

            model = context.Database.SqlQuery<IntStringKeyValueModel>( query.Trim(), parameters.ToArray() ).ToList();

            if ( model != null && model.Any() )
            {
                foreach ( var k in model )
                {
                    if ( options.Keys.Any( x => x == k.TKey ) )
                        continue;

                    options.Add( k.TKey, ( k.TValue ?? "" ).Trim() );
                }
            }

            return options;
        }
    }
}
