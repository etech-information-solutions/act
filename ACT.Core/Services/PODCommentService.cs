using System;
using System.Collections.Generic;
using System.Linq;

using ACT.Core.Enums;
using ACT.Core.Models;
using ACT.Data.Models;

namespace ACT.Core.Services
{
    public class PODCommentService : BaseService<PODComment>, IDisposable
    {
        public PODCommentService()
        {

        }

        public Dictionary<int, string> List( bool v )
        {
            Dictionary<int, string> options = new Dictionary<int, string>();
            List<IntStringKeyValueModel> model = new List<IntStringKeyValueModel>();

            List<object> parameters = new List<object>();

            string query = $"SELECT o.Id AS [TKey], o.[Comment] AS [TValue] FROM [dbo].[PODComment] o WHERE (o.[Status]={( int ) Status.Active});";

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

        /// <summary>
        /// Gets a POD Comment using the comment specified
        /// </summary>
        /// <param name="comment"></param>
        /// <returns></returns>
        public PODComment GetByComment( string comment )
        {
            return context.PODComments.FirstOrDefault( p => p.Comment == comment );
        }
    }
}
