using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ACT.Core.Services;
using ACT.Data.Models;

namespace System
{
    public static class VariableExtension
    {
        public static SystemConfig systemRules;

        public static SystemConfig SystemRules
        {
            get
            {
                if ( systemRules == null )
                {
                    systemRules = SetRules( true );
                }

                return systemRules;
            }
            set
            {
                systemRules = value;
            }
        }

        /// <summary>
        /// Gets the system rules
        /// </summary>
        /// <returns></returns>
        public static bool SetRules()
        {
            SystemRules = SetRules( true );

            return true;
        }

        private static SystemConfig SetRules( bool set )
        {
            SystemConfig sr = ( SystemConfig ) ContextExtensions.GetCachedData( "SR_ca" );

            if ( sr != null )
            {
                return sr;
            }

            using ( SystemConfigService service = new SystemConfigService() )
            {
                sr = service.List().FirstOrDefault() ?? new SystemConfig();

                ContextExtensions.CacheData( "SR_ca", sr );
            }

            return sr;
        }
    }
}
