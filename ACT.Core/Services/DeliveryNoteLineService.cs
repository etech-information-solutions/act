using System;

using ACT.Data.Models;

namespace ACT.Core.Services
{
    public class DeliveryNoteLineService : BaseService<DeliveryNoteLine>, IDisposable
    {
        public DeliveryNoteLineService()
        {

        }

        /// <summary>
        /// Gets a Delivery Note Line using the specified Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public override DeliveryNoteLine GetById( int id )
        {
            context.Configuration.LazyLoadingEnabled = true;
            context.Configuration.ProxyCreationEnabled = true;

            return base.GetById( id );
        }
    }
}
