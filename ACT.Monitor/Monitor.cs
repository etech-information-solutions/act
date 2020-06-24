using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.IO;
using System.ServiceProcess;
using System.Threading;
using ACT.Core.Enums;
using ACT.Core.Extension;
using ACT.Core.Models;
using ACT.Mailer;
using ACT.Monitor.Monitors;

namespace ACT.Monitor
{
    public partial class Monitor : ServiceBase
    {
        //private System.Timers.Timer DinnerMonitorTimer;

        private Timer DisputeTimer;

        private StreamWriter Writer;

        public static string LogFile
        {
            get
            {
                return ConfigurationManager.AppSettings[ "LogFile" ] ?? "C:/Etech_FTP_007/ACT";
            }
        }

        public Monitor()
        {
            InitializeComponent();

            if ( !Directory.Exists( LogFile ) )
            {
                Directory.CreateDirectory( LogFile );
            }

            FileStream fs = new FileStream( $"{LogFile}/log.log", FileMode.Append, FileAccess.Write, FileShare.ReadWrite );

            Writer = new StreamWriter( fs )
            {
                AutoFlush = true
            };
        }

        protected override void OnStart( string[] args )
        {
            try
            {
                ConfigSettings.SetRules();

                Writer.WriteLine();
                Writer.WriteLine( $"========================   SERVICE STARTED @ {DateTime.Now}   ========================" );

                // Start independent Timers
                this.ConfigureTimers( Timers.DisputeMonitorTimer );

                // Polls are in seconds, so we multiply by 1000 to convert seconds to milliseconds
                //this.DinnerMonitorTimer = new System.Timers.Timer( ( ( ( double ) ConfigSettings.SystemRules.DinnerMonitorPoll ) * 1000 ) );  // 300000 milliseconds = 5 Minutes 60000 milliseconds = 1 Minute
                //this.DinnerMonitorTimer.AutoReset = true;
                //this.DinnerMonitorTimer.Elapsed += new System.Timers.ElapsedEventHandler( this.DinnerMonitorTimer_Elapsed );
                //this.DinnerMonitorTimer.Start();

                // Polls are in seconds, so we multiply by 1000 to convert seconds to milliseconds
                //this.RebateMonitorTimer = new System.Timers.Timer( ( ( ( double ) ConfigSettings.SystemRules.RebateMonitorPoll ) * 1000 ) );  // 300000 milliseconds = 5 Minutes 60000 milliseconds = 1 Minute
                //this.RebateMonitorTimer.AutoReset = true;
                //this.RebateMonitorTimer.Elapsed += new System.Timers.ElapsedEventHandler( this.RebateMonitorTimer_Elapsed );
                //this.RebateMonitorTimer.Start();
            }
            catch ( Exception ex )
            {
                Writer.WriteLine( ex.ToString() );
            }
        }

        protected override void OnStop()
        {
            try
            {
                ConfigSettings.SetRules();

                string path = Environment.GetFolderPath( Environment.SpecialFolder.LocalApplicationData );

                try
                {
                    //if ( DinnerMonitorTimer != null )
                    //{
                    //    DinnerMonitorTimer.Stop();
                    //    DinnerMonitorTimer = null;
                    //}

                    // Fire an E-mail for RED ALERT
                    //List<string> receivers = new List<string>
                    //{
                    //    "msimwaba@gmail.com"
                    //};

                    //EmailModel mail = new EmailModel()
                    //{
                    //    Recipients = receivers,
                    //    Body = "<p>THE <b>ACT MONITOR (Windows Service)</b> HAS BEEN STOPPED! PLEASE RECTIFY IMMEDIATELY...</p><p>You can safely ignore this e-mail but you'll be held responsible for customer unsatisfictory!</p><p>Best Regards<br /> ACT Team</p>",
                    //    From = "support@testact.co.za",
                    //    Subject = "ACT MONITOR HAS BEEN STOPPED!"
                    //};

                    //bool sent = Mail.Send( mail );
                    //mail.Dispose();
                }
                catch ( Exception ex )
                {
                    BaseMonitor.Error( Writer, ex, "OnStop - Email" );
                }

                Writer.WriteLine();
                Writer.WriteLine( string.Format( "========================   SERVICE STOPPED @ {0}   ========================", DateTime.Now ) );
                Writer.Flush();
            }
            catch ( Exception ex )
            {
                BaseMonitor.Error( Writer, ex, "OnStop - Main" );
            }
        }

        /// <summary>
        /// Archives a log file in the specified path when it reaches a specified size
        /// </summary>
        /// <param name="path"></param>
        /// <param name="size"></param>
        private void ArchiveLog( string path, double size )
        {
            try
            {
                string logPath = $"{path}\\log.log";

                if ( File.Exists( path: logPath ) )
                {
                    FileInfo log = new FileInfo( logPath );

                    if ( ( log.Length / 1024 / 1024 ) >= size )
                    {
                        // Rename the file to .archive!
                        @File.Move( sourceFileName: logPath, destFileName: $"{path}\\log-{log.CreationTime.ToString( "yyyyMMdd HH_mm" )} to {DateTime.Now.ToString( "yyyyMMdd HH_mm" )}.archive" );
                    }
                }
            }
            catch ( Exception ex )
            {
                Writer.WriteLine( ex.ToString() );
                Writer.WriteLine( ex.Message ?? null );
                Writer.WriteLine( ex.InnerException?.Message );
            }
        }

        private void DinnerMonitorTimer_Elapsed( object sender, System.Timers.ElapsedEventArgs e )
        {
            try
            {
                //if ( DinnerMonitor.IsRunning )
                //{
                //    return;
                //}

                //ConfigSettings.SetRules();

                //ArchiveLog( ConfigSettings.SystemRules.DinnerMonitorExportPath.Replace( "\\Export", "" ), 10.0 );

                //using ( StreamWriter writer = new StreamWriter( string.Format( "{0}\\log.log", ConfigSettings.SystemRules.DinnerMonitorExportPath.Replace( "\\Export", "" ) ), true ) )
                //{
                //    try
                //    {
                //        writer.AutoFlush = true;

                //        writer.WriteLine();
                //        writer.WriteLine( string.Format( "BEGIN DINNERS MONITOR @ {0}   ========================", DateTime.Now ) );

                //        bool done = DinnerMonitor.Run( writer );

                //        writer.WriteLine();
                //        writer.WriteLine( string.Format( "END DINNERS MONITOR @ {0}   ========================", DateTime.Now ) );
                //    }
                //    catch ( Exception ex )
                //    {
                //        BaseMonitor.Error( writer, ex, "DinnerMonitorTimer_Elapsed" );
                //    }

                //    writer.Flush();
                //}
            }
            catch ( Exception ex )
            {
                BaseMonitor.Error( Writer, ex, "DinnerMonitorTimer_Elapsed" );
            }
        }



        private void ConfigureTimers( Timers timer )
        {
            int dueTime;

            DateTime scheduledTime;
            TimeSpan timeSpan, span;

            switch ( timer )
            {
                case Timers.DisputeMonitorTimer:

                    #region Member Monitor Timer

                    DisputeTimer = new Timer( new TimerCallback( DisputeTimerCallback ) );

                    //Set the Default Time.
                    scheduledTime = DateTime.MinValue;

                    //Get the Scheduled Time from AppSettings.
                    span = ConfigSettings.SystemRules.DisputeMonitorTime.Value;
                    scheduledTime = DateTime.Parse( $"{span.Hours}:{span.Minutes}:{span.Seconds}" );

                    if ( DateTime.Now > scheduledTime )
                    {
                        //If Scheduled Time is passed set Schedule for the next day.
                        scheduledTime = scheduledTime.AddDays( 1 );
                    }

                    Writer.WriteLine();
                    Writer.WriteLine( "Dispute Monitor Scheduled Time: {0}", scheduledTime );

                    timeSpan = scheduledTime.Subtract( DateTime.Now );

                    Writer.WriteLine();
                    Writer.WriteLine( "Dispute Monitor Time Span: {0}", timeSpan );

                    //Get the difference in Minutes between the Scheduled and Current Time.
                    dueTime = Convert.ToInt32( timeSpan.TotalMilliseconds );

                    Writer.WriteLine();
                    Writer.WriteLine( "Dispute Monitor Due Time: {0}", dueTime );

                    // Change the Timer's Due Time.
                    DisputeTimer.Change( dueTime, Timeout.Infinite );

                    #endregion

                    break;
            }
        }



        public void DisputeTimerCallback( object e )
        {
            try
            {
                ConfigSettings.SetRules();

                ArchiveLog( ConfigSettings.SystemRules.DisputeMonitorPath, 10.0 );

                using ( StreamWriter writer = new StreamWriter( $"{ConfigSettings.SystemRules.DisputeMonitorPath}\\log.log", true ) )
                {
                    writer.AutoFlush = true;

                    try
                    {
                        writer.WriteLine();
                        writer.WriteLine( $"BEGIN Dispute MONITOR @ {DateTime.Now}   ========================" );

                        DisputeMonitor.Run( writer );

                        writer.WriteLine();
                        writer.WriteLine( $"END Dispute MONITOR @ {DateTime.Now}   ========================" );
                    }
                    catch ( Exception ex )
                    {
                        BaseMonitor.Error( writer, ex, "DisputeTimerCallback" );
                    }
                }
            }
            catch ( Exception ex )
            {
                BaseMonitor.Error( Writer, ex, "DisputeTimerCallback" );
            }

            // Reset Timer
            ConfigureTimers( Timers.DisputeMonitorTimer );
        }
    }
}
