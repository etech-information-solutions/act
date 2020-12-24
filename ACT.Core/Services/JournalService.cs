using System;
using System.Collections.Generic;
using System.Linq;

using ACT.Data.Models;

namespace ACT.Core.Services
{
    public class JournalService : BaseService<Journal>, IDisposable
    {
        public JournalService()
        {

        }

        /// <summary>
        /// Gets a list of Journals using the specified Client Load Id
        /// </summary>
        /// <param name="clientLoadId"></param>
        /// <returns></returns>
        public List<Journal> ListByClientLoad( int clientLoadId )
        {
            return context.Journals.Where( j => j.ClientLoadId == clientLoadId ).ToList();
        }
    }
}
