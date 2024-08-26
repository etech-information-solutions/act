using System;
using System.Linq;

using ACT.Data.Models;

namespace ACT.Core.Services
{
    public class ClientLoadQuantityService : BaseService<ClientLoadQuantity>, IDisposable
    {
        public ClientLoadQuantityService()
        {
        }

        /// <summary>
        /// Gets a ClientLoadQuantity using the specified client load id and equipment code
        /// </summary>
        /// <param name="clientLoadId"></param>
        /// <param name="equipmentCode"></param>
        /// <returns></returns>
        public ClientLoadQuantity GetByClientLoadAndProduct( int clientLoadId, string equipmentCode )
        {
            return context.ClientLoadQuantities.FirstOrDefault( cq => cq.ClientLoadId == clientLoadId && cq.EquipmentCode == equipmentCode );
        }

        /// <summary>
        /// Deletes all ClientLoadQuantity entries for a specific ClientLoadId
        /// </summary>
        /// <param name="clientLoadId"></param>
        public void DeleteByClientLoadId( int clientLoadId )
        {
            var entitiesToDelete = context.ClientLoadQuantities.Where( cq => cq.ClientLoadId == clientLoadId ).ToList();

            foreach ( var entity in entitiesToDelete )
            {
                context.ClientLoadQuantities.Remove( entity );
            }

            context.SaveChanges();
        }
    }
}