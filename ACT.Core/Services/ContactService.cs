using System;
using System.Linq;
using System.Collections.Generic;
using System.Linq.Dynamic;

using ACT.Data.Models;

namespace ACT.Core.Services
{
    public class ContactService : BaseService<Contact>, IDisposable
    {
        public ContactService()
        {

        }

        /// <summary>
        /// Gets a list of contacts for the specified object
        /// </summary>
        /// <param name="objectId"></param>
        /// <param name="objectType"></param>
        /// <returns></returns>
        public List<Contact> List( int objectId, string objectType )
        {
            return context.Contacts.Where( c => c.ObjectId == objectId && c.ObjectType == objectType ).ToList();
        }

        /// <summary>
        /// Gets a Contact using the specified contact Id Number and Object Type
        /// </summary>
        /// <param name="idNo"></param>
        /// <param name="objectType"></param>
        /// <returns></returns>
        public Contact Get( string idNo, string objectType )
        {
            return context.Contacts.FirstOrDefault( v => v.ContactIdNo.Trim() == idNo.Trim() && v.ObjectType == objectType );
        }
    }
}
