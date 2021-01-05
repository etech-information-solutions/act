using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

using ACT.Core.Enums;
using ACT.Core.Models;
using ACT.Core.Models.Custom;
using ACT.Data.Models;

namespace ACT.Core.Services
{
    public class ChepLoadChepService : BaseService<ChepLoadChep>, IDisposable
    {
        public ChepLoadChepService()
        {

        }

        /// <summary>
        /// Gets a list of manual allocations for the specified ChepLoadId
        /// </summary>
        /// <param name="chepLoadId"></param>
        /// <returns></returns>
        public List<ChepLoadChep> ListByChepLoadId( int chepLoadId )
        {
            return context.ChepLoadCheps.Where( clc => clc.ChepLoadId == chepLoadId ).ToList();
        }
    }
}
