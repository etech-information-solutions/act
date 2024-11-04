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

        public (decimal delivered, decimal returned, decimal outstanding) GetQuantitiesForClientLoad( int clientLoadId )
        {
            var quantities = context.ClientLoadQuantities
                .Where( clq => clq.ClientLoadId == clientLoadId )
                .GroupBy( clq => clq.ClientLoadId )
                .Select( g => new
                {
                    Delivered = g.Sum( clq => clq.OriginalQuantity ),
                    Returned = g.Sum( clq => clq.ReturnQty ),
                    Outstanding = g.Sum( clq => clq.OutstandingQty )
                } )
                .FirstOrDefault();

            return quantities == null
                ? (0, 0, 0)
                : (quantities.Delivered, quantities.Returned, quantities.Outstanding);
        }
    }
}