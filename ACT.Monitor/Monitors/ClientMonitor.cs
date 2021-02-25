using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

using ACT.Core.Extension;
using ACT.Core.Services;
using ACT.Data.Models;
using ACT.Core.Enums;
using ACT.Core.Models.Custom;
using ACT.Core.Models;
using ACT.Mailer;

namespace ACT.Monitor.Monitors
{
    class ClientMonitor : BaseMonitor
    {
        /// <summary>
        /// Indicates if this Monitor is Running
        /// </summary>
        public static bool IsRunning
        {
            get; set;
        }

        public static int ClientMonitorCount
        {
            get; set;
        }

        /// <summary>
        /// Runs all the operations of this monitor 
        /// </summary>
        /// <param name="writer"></param>
        /// <returns></returns>
        public static bool Run( StreamWriter writer )
        {
            if ( IsRunning ) return false;

            IsRunning = true;


            // 1. Excute CheckClientContracts
            Info( writer, ":: ClientMonitor :: CheckClientContracts() :: Executed" );

            CheckClientContracts( writer );

            // 2. ....

            IsRunning = false;

            #region Update Last Run

            using ( SystemConfigService service = new SystemConfigService() )
            {
                SystemConfig rules = service.GetById( ConfigSettings.SystemRules.Id );

                if ( rules != null )
                {
                    rules.LastClientMonitorRun = DateTime.Now;
                    rules.LastClientMonitorCount = ClientMonitorCount;

                    service.Update( rules );
                }
            }

            #endregion

            return true;
        }

        /// <summary>
        /// Exports FAMOUS BRANDS Campaign Purchases to Clientele using SFTP
        /// </summary>
        /// <param name="writer"></param>
        /// <returns></returns>
        public static bool CheckClientContracts( StreamWriter writer )
        {
            try
            {
                if ( !ConfigSettings.SystemRules.ClientMonitorEnabled )
                {
                    Info( writer, $"    Checking of Client Contracts is disabled by Admin. RETURNING @{DateTime.Now}" );

                    return true;
                }

                using ( ClientService cservice = new ClientService() )
                {
                    Info( writer, $"    BEGIN: Checking of Client Contracts... @{DateTime.Now}" );

                    List<ClientCustomModel> list = cservice.List1( new PagingModel() { Take = int.MaxValue }, new CustomSearchModel() { PSPClientStatus = PSPClientStatus.Verified } );

                    int months = ConfigSettings.SystemRules.ClientContractRenewalReminderMonths ?? 3;
                    List<ClientCustomModel> due = list.Where( c => c.ContractRenewalDate.HasValue && ( DateTime.Now - c.ContractRenewalDate.Value ).Days >= ( months * 30 ) ).ToList();

                    Info( writer, $"        - There are {list.Count} Active Clients... @{DateTime.Now}" );
                    Info( writer, $"        - There are {due.Count} Active Clients with contracts expiring in {months} months or less... @{DateTime.Now}" );

                    if ( due.Any() )
                    {
                        return true;
                    }

                    foreach ( ClientCustomModel item in due )
                    {
                        List<string> receivers = new List<string>
                        {
                            ConfigSettings.SystemRules.SystemContactEmail,
                            ConfigSettings.SystemRules.FinancialContactEmail
                        };

                        EmailModel mail = new EmailModel()
                        {
                            Recipients = receivers,
                            Body = $"<p>The contract between ACT and <b>{item.CompanyName}</b> will be expiring on {item.ContractRenewalDate?.ToString( "yyyy-MM-dd" )} (in {( DateTime.Now - item.ContractRenewalDate.Value ).Days} days). Please rectify accordingly to stop receiving this daily email notification.</p><p>You can safely ignore this e-mail but you'll be held responsible for customer unsatisfictory!</p><p>Best Regards<br /> ACT Team</p>",
                            From = ConfigSettings.SystemRules.SystemContactEmail,
                            Subject = $"{item.CompanyName} - Client Contract Expiry Notice"
                        };

                        bool sent = Mail.Send( mail );
                        mail.Dispose();
                    }

                    Info( writer, $"    END: Checking of Client Contracts... @{DateTime.Now}" );
                }
            }
            catch ( Exception ex )
            {
                Error( writer, ex, "CheckClientContracts" );

                return false;
            }

            return true;
        }
    }
}
