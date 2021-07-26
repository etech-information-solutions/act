using System;
using System.Linq;
using ACT.Data.Models;

namespace ACT.Core.Services
{
    public class AddressService : BaseService<Address>, IDisposable
    {
        public AddressService()
        {

        }

        /// <summary>
        /// Gets an address record using the specified objectId and objectType
        /// </summary>
        /// <param name="objectId"></param>
        /// <param name="objectType"></param>
        /// <returns></returns>
        public Address Get( int objectId, string objectType )
        {
            context.Configuration.LazyLoadingEnabled = true;
            context.Configuration.ProxyCreationEnabled = true;

            return context.Addresses.FirstOrDefault( a => a.ObjectId == objectId && a.ObjectType == objectType );
        }
    }
}
