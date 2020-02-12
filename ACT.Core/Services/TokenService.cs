using System;
using System.Data.Entity;
using System.Linq;
using ACT.Core.Enums;
using ACT.Data.Models;

namespace ACT.Core.Services
{
    public class TokenService : BaseService<Token>, IDisposable
    {
        public TokenService()
        {

        }

        /// <summary>
        /// Gets a Token using the specified UID
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public Token GetByUid( Guid uid )
        {
            return context.Tokens.FirstOrDefault( t => t.UID == uid && t.Status == ( int ) Status.Active );
        }

        /// <summary>
        /// Checks if a Token with the specified uid and date exists...?
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public bool Exist( Guid uid, DateTime date )
        {
            return context.Tokens.Any( t => t.UID == uid && t.Status == ( int ) Status.Active && DbFunctions.TruncateTime( t.CreatedOn ) == DbFunctions.TruncateTime( date.Date ) );
        }
    }
}
