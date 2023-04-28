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

        public Address GetProvinceName(int? id)
        {

            Address address = (from a in context.Addresses where a.ProvinceId == id select a).FirstOrDefault();
            return address;
        }

        /// </summary>      
        /// <param name="model">model of the user to be fetched</param>
        /// <returns></returns>
        public Address GetAddress(int id)
        {
            var tt = (from d in context.Addresses select d);
            if (tt.Count() > 0)
            {
                Address addRecord = (from s in context.Addresses where s.ObjectId == id select s).FirstOrDefault();

                if (addRecord != null)
                {
                    return addRecord;
                }
                else return null;
            }
            else
                return null;
        }
    }
}
