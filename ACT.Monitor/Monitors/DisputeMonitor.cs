using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

using MailKit.Net.Pop3;
using MailKit.Net.Imap;
using MailKit;
using MimeKit;

using ACT.Core.Services;
using ACT.Data.Models;
using ACT.Core.Enums;

namespace ACT.Monitor.Monitors
{
    class DisputeMonitor : BaseMonitor
    {
        /// <summary>
        /// Indicates if this Monitor is Running
        /// </summary>
        public static bool IsRunning
        {
            get; set;
        }

        public static int DisputeMonitorCount
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


            // 1. Excute Import
            Info( writer, ":: DisputeMonitor :: Import() :: Executed" );

            Import( writer );

            // 2. ....

            IsRunning = false;

            #region Update Last Run

            using ( SystemConfigService service = new SystemConfigService() )
            {
                SystemConfig rules = service.GetById( ConfigSettings.SystemRules.Id );

                if ( rules != null )
                {
                    rules.LastDisputeMonitorRun = DateTime.Now;
                    rules.LastDisputeMonitorCount = DisputeMonitorCount;

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
        public static bool Import( StreamWriter writer )
        {
            try
            {
                if ( !ConfigSettings.SystemRules.DisputeMonitorEnabled )
                {
                    Info( writer, $"    Extract of Disputes is disabled by Admin. RETURNING @{DateTime.Now}" );

                    return true;
                }

                using ( PSPConfigService cpservice = new PSPConfigService() )
                {
                    List<PSPConfig> configs = cpservice.List()?.Where( pc => !string.IsNullOrWhiteSpace( pc.ImportEmailHost ) &&
                                                                             !string.IsNullOrWhiteSpace( pc.ImportEmailPassword ) &&
                                                                             !string.IsNullOrWhiteSpace( pc.ImportEmailUsername )
                                                                     )?.ToList();

                    if ( !configs.NullableAny() )
                    {
                        Info( writer, $"    *** There are no PSPs with correctly setup Email Settings to extract Email Disputes. Returning @ {DateTime.Now}" );

                        return true;
                    }

                    Info( writer, $"    BEGIN: Dispute Extract for {configs.Count} PSPs... @{DateTime.Now}" );


                    foreach ( PSPConfig p in configs )
                    {
                        if ( p.ImportEmailHost.ToLower().Contains( "pop" ) )
                        {
                            #region Pop3 Client

                            using ( Pop3Client client = new Pop3Client() )
                            {
                                Info( writer, $"    - Connecting to Server [{p.ImportEmailHost}] for {p.PSP.CompanyName} using POP3 Client... @{DateTime.Now}" );

                                client.Connect( p.ImportEmailHost?.Trim(), p.ImportEmailPort ?? 110, p.ImportUseSSL ?? false );

                                Info( writer, $"    - Authenticating with CREDENTIALS: {p.ImportEmailUsername}, **** using POP3 Client... @{DateTime.Now}" );

                                client.Authenticate( p.ImportEmailUsername, p.ImportEmailPassword );

                                int count = 1;

                                for ( int i = 0; i < client.Count; i++ )
                                {
                                    MimeMessage message = client.GetMessage( i );

                                    CreateDispute( writer, message, count );

                                    count++;

                                    DisputeMonitorCount += 1;
                                }

                                client.Disconnect( true );
                            }

                            #endregion
                        }
                        else if ( p.ImportEmailHost.ToLower().Contains( "imap" ) )
                        {
                            #region Imap Client

                            using ( ImapClient client = new ImapClient() )
                            {
                                Info( writer, $"    - Connecting to Server [{p.ImportEmailHost}] for {p.PSP.CompanyName} using Imap Client... @{DateTime.Now}" );

                                client.Connect( p.ImportEmailHost?.Trim(), p.ImportEmailPort ?? 993, p.ImportUseSSL ?? true );

                                Info( writer, $"    - Authenticating with CREDENTIALS: {p.ImportEmailUsername}, **** using Imap Client... @{DateTime.Now}" );

                                client.Authenticate( p.ImportEmailUsername, p.ImportEmailPassword );

                                IMailFolder inbox = client.Inbox;
                                inbox.Open( FolderAccess.ReadOnly );

                                int count = 1;

                                for ( int i = 0; i < inbox.Count; i++ )
                                {
                                    MimeMessage message = inbox.GetMessage( i );

                                    CreateDispute( writer, message, count );

                                    count++;

                                    DisputeMonitorCount += 1;
                                }

                                client.Disconnect( true );
                            }

                            #endregion
                        }
                    }

                    Info( writer, $"    END: Dispute Extract for {configs.Count} PSPs... @{DateTime.Now}" );
                }
            }
            catch ( Exception ex )
            {
                Error( writer, ex, "Export" );

                return false;
            }

            return true;
        }

        /// <summary>
        /// Creates a Dispute using the specified TDN and Docket number if it does not already exists
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="message"></param>
        /// <param name="count"></param>
        private static void CreateDispute( StreamWriter writer, MimeMessage message, int count )
        {
            try
            {
                // Skip if this dispute has already been read/processed
                if ( message.Date.Date <= ConfigSettings.SystemRules.LastDisputeMonitorRun?.Date ) return;

                // Skip if this email is not a dispute
                if ( !message.Subject.ToLower().Contains( "chep transaction in dispute" ) ) return;

                string text = message.TextBody ?? message.HtmlBody;

                if ( string.IsNullOrWhiteSpace( text ) )
                {
                    Info( writer, $"        x EMPTY Email Body FOUND for email #{count}! SKIPPING MessageId {message.MessageId} @{DateTime.Now}" );

                    return;
                }

                if ( !text.ToLower().Contains( "tdn number:" ) || !text.ToLower().Contains( "docket number:" ) )
                {
                    Info( writer, $"        x Email Body for email #{count} does not contain TDN or DOCKET number! SKIPPING MessageId {message.MessageId} @{DateTime.Now}" );

                    return;
                }

                List<string> lines = text.Split( new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries )
                                         .Where( l => l.ToLower().Contains( "tdn number:" ) ||
                                                    l.ToLower().Contains( "docket number:" ) ||
                                                    l.ToLower().Contains( "sender:" ) ||
                                                    l.ToLower().Contains( "receiver:" ) ||
                                                    l.ToLower().Contains( "declarer:" ) ||
                                                    l.ToLower().Contains( "effect date:" ) ||
                                                    l.ToLower().Contains( "product:" ) ||
                                                    l.ToLower().Contains( "quantity:" ) ||
                                                    l.ToLower().Contains( "actioned by:" ) ||
                                                    l.ToLower().Contains( "status changed from" )
                                               )
                                         .ToList();

                if ( !lines.NullableAny() )
                {
                    Info( writer, $"        x Email Body for email #{count} does not contain ALL required information! SKIPPING MessageId {message.MessageId} @{DateTime.Now}" );

                    return;
                }

                using ( UserService uservice = new UserService() )
                using ( DisputeService dservice = new DisputeService() )
                using ( ChepLoadService clservice = new ChepLoadService() )
                {
                    string docketNumber = lines.FirstOrDefault( l => l.Contains( "docket number:" ) ).Split( ':' )[ 1 ].Trim();

                    // Try and locate a ChepLoad using the email's DocketNumber
                    ChepLoad cl = clservice.GetByDocketNumber( docketNumber );

                    Dispute dispute = dservice.GetByDocketNumber( docketNumber );

                    if ( dispute != null )
                    {
                        Info( writer, $"        x Dispute email #{count} with Docket Number {docketNumber} ALREADY EXISTS on the system! SKIPPING MessageId {message.MessageId} @{DateTime.Now}" );

                        return;
                    }

                    string[] disputeReason = lines.FirstOrDefault( l => l.Contains( "status changed from" ) ).Split( ':' );

                    int.TryParse( lines.FirstOrDefault( l => l.Contains( "quantity:" ) ).Split( ':' )[ 1 ].Trim(), out int quantity );

                    string actionBy = lines.FirstOrDefault( l => l.Contains( "actioned by:" ) ).Split( ':' )[ 1 ].Trim();

                    User user = uservice.GetByNameAndSurname( actionBy.Split( ' ' )[ 0 ].Trim(), actionBy.Split( ' ' )[ 1 ].Trim() );

                    dispute = new Dispute()
                    {
                        Imported = true,
                        ChepLoadId = cl?.Id,
                        Quantity = quantity,
                        ActionBy = actionBy,
                        ActionedById = user?.Id,
                        Status = ( int ) Status.Active,
                        DisputeEmail = string.Join( ";", message.From ),
                        //DisputeReason = disputeReason[ disputeReason.Length - 1 ],
                        Sender = lines.FirstOrDefault( l => l.Contains( "sender:" ) ).Split( ':' )[ 1 ].Trim(),
                        Product = lines.FirstOrDefault( l => l.Contains( "product:" ) ).Split( ':' )[ 1 ].Trim(),
                        Declarer = lines.FirstOrDefault( l => l.Contains( "declarer:" ) ).Split( ':' )[ 1 ].Trim(),
                        Receiver = lines.FirstOrDefault( l => l.Contains( "receiver:" ) ).Split( ':' )[ 1 ].Trim(),
                        Equipment = lines.FirstOrDefault( l => l.Contains( "product:" ) ).Split( ':' )[ 1 ].Trim(),
                        TDNNumber = lines.FirstOrDefault( l => l.Contains( "tdn number:" ) ).Split( ':' )[ 1 ].Trim(),
                        Reference = lines.FirstOrDefault( l => l.Contains( "docket number:" ) ).Split( ':' )[ 1 ].Trim(),
                        DocketNumber = lines.FirstOrDefault( l => l.Contains( "docket number:" ) ).Split( ':' )[ 1 ].Trim(),
                    };

                    dispute = dservice.Create( dispute );

                    Info( writer, $"        - SUCCESSFULLY CREATED Dispute for Docket Number {docketNumber} ({dispute.Id}) @{DateTime.Now}" );

                }
            }
            catch ( Exception ex )
            {
                Error( writer, ex, "CreateDispute" );
            }


        }
    }
}
