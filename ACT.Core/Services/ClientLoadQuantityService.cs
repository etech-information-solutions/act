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
    }
}
