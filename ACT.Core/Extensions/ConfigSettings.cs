using System;
using System.Configuration;
using System.Web.Security;
using ACT.Core.Services;
using ACT.Data.Models;

namespace ACT.Core.Extension
{
    public static class ConfigSettings
    {
        #region Properties

        public static TimeSpan FormsTimeOut
        {
            get
            {
                TimeSpan timeout = FormsAuthentication.Timeout;

                return timeout;
            }
        }

        public static int MaxMailsToProcess
        {
            get
            {
                string maxMailsToProcess = ConfigurationManager.AppSettings[ "MaxMailsToProcess" ] ?? "1";
                int mails = int.TryParse( maxMailsToProcess, out mails ) ? mails : 1;

                return mails; 
            }
        }

        public static string AdminEmailAddress
        {
            get
            {
                return ConfigurationManager.AppSettings[ "AdminEmailAddress" ] ?? "msimwaba@gmail.com";
            }
        }

        public static int PagingTake
        {
            get
            {
                string take = ConfigurationManager.AppSettings[ "PagingTake" ] ?? "50";
                int limit = int.TryParse( take, out limit ) ? limit : 50;

                return limit;
            }
        }

        public static SystemConfig SystemRules { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public static string AdminEmailAddress1
        {
            get
            {
                return ConfigurationManager.AppSettings[ "AdminEmailAddress1" ] ?? "msimwaba@gmail.com";
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static string AdminEmailAddress2
        {
            get
            {
                return ConfigurationManager.AppSettings[ "AdminEmailAddress2" ] ?? "dsouchon@gmail.com";
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static string AdminEmailAddress3
        {
            get
            {
                return ConfigurationManager.AppSettings[ "AdminEmailAddress3" ] ?? "heather.sadie@etechsolutions.co.za";
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static string HRManageTest
        {
            get
            {
                return ConfigurationManager.AppSettings[ "HRManageTest" ] ?? "";
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static string HRManageLive
        {
            get
            {
                return ConfigurationManager.AppSettings[ "HRManageLive" ] ?? "";
            }
        }

        /// <summary>
        /// Determines if the budget monitor is running
        /// </summary>
        public static bool BudgetMonitorRunning
        {
            get
            {
                bool isBudgetMonitorRunning = false;

                if ( !string.IsNullOrEmpty( ConfigurationManager.AppSettings[ "BudgetMonitorRunning" ] ) )
                {
                    bool.TryParse( ConfigurationManager.AppSettings[ "BudgetMonitorRunning" ], out isBudgetMonitorRunning );
                }

                return isBudgetMonitorRunning;
            }
        }

        /// <summary>
        /// Determines if the payments monitor is running
        /// </summary>
        public static bool PaymentsMonitorRunning
        {
            get
            {
                bool isPaymentsMonitorRunning = false;

                if ( !string.IsNullOrEmpty( ConfigurationManager.AppSettings[ "PaymentsMonitorRunning" ] ) )
                {
                    bool.TryParse( ConfigurationManager.AppSettings[ "PaymentsMonitorRunning" ], out isPaymentsMonitorRunning );
                }

                return isPaymentsMonitorRunning;
            }
        }

        /// <summary>
        /// Determines if the bank monitor is running
        /// </summary>
        public static bool BankMonitorRunning
        {
            get
            {
                bool isBankMonitorRunning = false;

                if ( !string.IsNullOrEmpty( ConfigurationManager.AppSettings[ "BankMonitorRunning" ] ) )
                {
                    bool.TryParse( ConfigurationManager.AppSettings[ "BankMonitorRunning" ], out isBankMonitorRunning );
                }

                return isBankMonitorRunning;
            }
        }

        /// <summary>
        /// Determines if the general ledger (aka GL) monitor is running
        /// </summary>
        public static bool GLMonitorRunning
        {
            get
            {
                bool isGLMonitorRunning = false;

                if ( !string.IsNullOrEmpty( ConfigurationManager.AppSettings[ "GLMonitorRunning" ] ) )
                {
                    bool.TryParse( ConfigurationManager.AppSettings[ "GLMonitorRunning" ], out isGLMonitorRunning );
                }

                return isGLMonitorRunning;
            }
        }

        /// <summary>
        /// Determines if the email monitor is running
        /// </summary>
        public static bool EmailMonitorRunning
        {
            get
            {
                bool isEmailMonitorRunning = false;

                if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["EmailMonitorRunning"]))
                {
                    bool.TryParse(ConfigurationManager.AppSettings["EmailMonitorRunning"], out isEmailMonitorRunning);
                }

                return isEmailMonitorRunning;
            }
        }

        /// <summary>
        /// Determines if the pending files monitor is running
        /// </summary>
        public static bool PendingFilesMonitorRunning
        {
            get
            {
                bool isPendingFilesMonitorRunning = false;

                if ( !string.IsNullOrEmpty( ConfigurationManager.AppSettings[ "PendingFilesMonitorRunning" ] ) )
                {
                    bool.TryParse( ConfigurationManager.AppSettings[ "PendingFilesMonitorRunning" ], out isPendingFilesMonitorRunning );
                }

                return isPendingFilesMonitorRunning;
            }
        }

        #endregion

        /// <summary> 
        /// Gets the system rules
        /// </summary>
        /// <returns></returns>
        public static bool SetRules()
        {
            SystemRules = ( SystemConfig ) ContextExtensions.GetCachedData( "SR_ca" );

            if ( SystemRules != null )
            {
                return true;
            }

            using ( SystemConfigService service = new SystemConfigService() )
            {
                SystemRules = @service.List()[ 0 ] ?? new SystemConfig();

                ContextExtensions.CacheData( "SR_ca", SystemRules );
            }

            return true;
        }
    }
}