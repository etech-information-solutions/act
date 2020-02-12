using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.Caching;
using System.Web.Script.Serialization;
using ACT.Core.Services;
using ACT.Data.Models;

namespace System
{
    public static class ContextExtensions
    {
        private static decimal AutoSeconds
        {
            get
            {
                return 1800;
                //using ( SystemConfigService service = new SystemConfigService() )
                //{
                //    SystemConfig c = service.Get();

                //    return c.AutoSeconds;
                //}
            }
        }

        public static void CacheData( string key, object value, double minutes = 0 )
        {
            try
            {
                object _value = HttpContext.Current.Cache.Get( key );

                if ( _value != null )
                {
                    HttpContext.Current.Cache.Remove( key ); 
                }

                if ( minutes == 0 )
                {
                    minutes = 30;
                }

                HttpContext.Current.Cache.Insert( key, value, null, Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes( minutes ), CacheItemPriority.Default, new CacheItemRemovedCallback( CacheRemovedCallBack ) );
            }
            catch ( Exception ex )
            {

            }
        }

        public static void CacheUserData( string key, object value )
        {
            try
            {
                object _value = HttpContext.Current.Session[ key ];

                if ( _value != null )
                {
                    HttpContext.Current.Session.Remove( key );
                }

                HttpContext.Current.Session.Add( key, value );
            }
            catch ( Exception ex )
            {

            }
        }

        public static object GetCachedData( string key )
        {
            try
            {
                object value = HttpContext.Current.Cache.Get( key );

                return value;
            }
            catch ( Exception ex )
            {
                return null;
            }
        }

        public static object GetCachedUserData( string key )
        {
            try
            {
                object value = HttpContext.Current.Session[ key ];

                return value;
            }
            catch ( Exception ex )
            {
                return null;
            }
        }

        public static bool RemoveCachedData( string key )
        {
            try
            {
                HttpContext.Current.Cache.Remove( key );
            }
            catch ( Exception ex )
            {
            }

            return true;
        }

        public static bool RemoveCachedUserData( string key )
        {
            try
            {
                HttpContext.Current.Session.Remove( key );
            }
            catch ( Exception ex )
            {
            }

            return true;
        }

        public static bool RemoveCachedData( List<string> keys )
        {
            try
            {
                foreach ( string key in keys )
                {
                    HttpContext.Current.Cache.Remove( key );
                }
            }
            catch ( Exception ex )
            {
            }

            return true;
        }

        public static bool RemoveCachedUserData( List<string> keys )
        {
            try
            {
                foreach ( string key in keys )
                {
                    HttpContext.Current.Session.Remove( key );
                }
            }
            catch ( Exception ex )
            {
            }

            return true;
        }

        public static bool RemoveCachedData()
        {
            try
            {
                foreach ( DictionaryEntry entry in HttpContext.Current.Cache )
                {
                    HttpContext.Current.Cache.Remove( ( string ) entry.Key );
                }
            }
            catch ( Exception ex )
            {
            }

            return true;
        }

        public static bool RemoveCachedUserData()
        {
            try
            {
                foreach ( DictionaryEntry entry in HttpContext.Current.Session )
                {
                    HttpContext.Current.Session.Remove( ( string ) entry.Key );
                }
            }
            catch ( Exception ex )
            {

            }

            return true;
        }

        public static void CacheRemovedCallBack( string key, object value, CacheItemRemovedReason reason )
        {
            //CacheData( key, value );
            //using ( StreamWriter writer = new StreamWriter( @"C:\inetpub\wwwroot\vhosts\payments.voteda.org\wwwroot\Logs\Cache\log.log", true ) )
            //{
            //    writer.AutoFlush = true;

            //    writer.WriteLine();
            //    writer.WriteLine( $"CACHE ITEM <{key} => {new JavaScriptSerializer().Serialize( value )}> REMOVED Because <{reason}> @ {DateTime.Now }   ========================" );
            //}
        }
    }
}
