using System;
using System.Collections.Generic;
using System.Linq;

using ACT.Data.Models;

namespace ACT.Core.Services
{
    public class DeliveryNoteLineService : BaseService<DeliveryNoteLine>, IDisposable
    {
        public DeliveryNoteLineService()
        {

        }

        public List<DeliveryNoteLine> ListByLoadNumber( string loadNumber )
        {
            return context.DeliveryNoteLines.Where( dnl => dnl.DeliveryNote.Reference306 == loadNumber.Trim() ).ToList();
        }
    }
}
