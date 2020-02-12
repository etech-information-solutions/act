using System;
using System.Linq;
using ACT.Data.Models;

namespace ACT.Core.Services
{
    public class SystemConfigService : BaseService<SystemConfig>, IDisposable
    {
        public SystemConfigService()
        {

        }

        public SystemConfig Get()
        {
            return context.SystemConfigs.FirstOrDefault();
        }
    }
}
