using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System {
    public static class DateTimeExtensions {


        public static string ToDefaultString(this DateTime dateTime) {
            //TODO: in future we will add Date format functionality here
            return dateTime.ToString("dd/MM/yyyy HH:mm:ss");
        }

        //http://stackoverflow.com/questions/7029353/c-sharp-round-up-time-to-nearest-x-minutes
        public static DateTime RoundUp(this DateTime dt, TimeSpan d) {
            return new DateTime(((dt.Ticks + d.Ticks - 1) / d.Ticks) * d.Ticks);
        }

        //http://stackoverflow.com/questions/8134170/c-sharp-version-of-javascript-date-gettime
        /// <summary>
        /// This time can be used as is for HighCharts - even if the users's browser is
        /// on a different timezone than the server.
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        public static long ToJsGetTime(this DateTime d) {
            TimeSpan t = d - _jsBaseDate;
            long result = Convert.ToInt64(t.TotalMilliseconds + 0.5);
            return result;

        }

        private static readonly DateTime _jsBaseDate = new DateTime(1970,1,1);

        public static DateTime formatImportDate(string inDate)
        {
            string parseFormat = "dd.MM.yyyy";
            char parseSplit = '.';
            if (inDate.Contains("."))
            {
                //do nothing everything is already set
            }
            else if (inDate.Contains("-"))
            {
                parseFormat = "dd-MM-yyyy";
                parseSplit = '-';
            }
            else if (inDate.Contains("/"))
            {
                parseFormat = "dd/MM/yyyy";
                parseSplit = '/';
            }
            string[] strArr = inDate.Split(parseSplit);
            string splitDate = "";
            if (strArr[2].Length == 4) //discard times for now, doesnt matter
            {
                splitDate = strArr[2] + parseSplit + strArr[1] + parseSplit + strArr[0];
            }
            else if (strArr[0].Length == 4)
            {
                splitDate = strArr[0] + parseSplit + strArr[1] + parseSplit + strArr[2];
            }


          return DateTime.ParseExact(inDate + " 00:00:00", parseFormat + " HH:mm:ss", null);
        }


    }
}
