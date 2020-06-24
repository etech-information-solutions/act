using System;
using System.Collections.Generic;

using ACT.Data.Models;

namespace ACT.Core.Services
{
    public class PSPConfigService : BaseService<PSPConfig>, IDisposable
    {
        public PSPConfigService()
        {

        }

        /// <summary>
        /// Gets a list of PSPConfigs
        /// </summary>
        /// <returns></returns>
        public override List<PSPConfig> List()
        {
            context.Configuration.LazyLoadingEnabled = true;
            context.Configuration.ProxyCreationEnabled = true;

            return base.List();
        }
    }
}
