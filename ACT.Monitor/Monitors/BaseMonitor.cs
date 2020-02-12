using ACT.Core.Enums;
using ACT.Core.Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity.Validation;
using System.IO;
using System.Net;

namespace ACT.Monitor.Monitors
{
    public class BaseMonitor
    {
        /// <summary>
        /// Gets the service's current path
        /// </summary>
        public static string MyPath
        {
            get
            {
                return AppDomain.CurrentDomain.BaseDirectory;
            }
        }

        /// <summary>
        /// Performs a GET request
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string Get( string url )
        {
            string responseString = string.Empty;

            try
            {
                WebRequest request = ( HttpWebRequest ) WebRequest.Create( url );
                WebResponse response = ( HttpWebResponse ) request.GetResponse();

                responseString = new StreamReader( response.GetResponseStream() ).ReadToEnd();
            }
            catch ( Exception ex )
            {

            }

            return responseString;
        }

        public static List<string> Errors { get; set; }

        public static void Error( StreamWriter writer, Exception ex, string method )
        {
            #region Catch It Like:

            string _err, err = _err = $":: ERROR :: IN {method}() :: {ex.ToString() }";

            writer.WriteLine();
            writer.WriteLine( err );

            #endregion
        }

        public static bool Info( StreamWriter writer, string message )
        {
            writer.WriteLine();
            writer.WriteLine( message );

            return true;
        }
    }
}
