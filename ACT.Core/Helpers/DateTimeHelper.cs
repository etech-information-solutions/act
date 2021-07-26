using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACT.Core.Helpers
{
    public static class DateTimeHelper
    {
        public static DateTime? NullableTryParseDateTime(string stringDate)
        {
            return DateTime.TryParse( stringDate, out DateTime date ) ? date : ( DateTime? ) null;
        }

        private static Dictionary<string, string> GetValuesFromDictionaryAndRemove(List<string> headers, List<string> values, string[] picks)
        {
            var temp = DictionaryHelpers.ZipCsvHeadersAndValues(headers, values);
            var dictionary = new Dictionary<string, string>();
            foreach (var pick in picks)
            {

                dictionary.Add(pick, temp[pick]);
            }
            return dictionary;
        }

        public static DateTime formatImportDate(string inDate)
        {
            //string[] strValues = new string[] { "-", "/", "." };
            if (!string.IsNullOrEmpty(inDate) || (inDate.Contains("/") || inDate.Contains("-") || inDate.Contains(".")))
            {
                char parseSplit = '.';
                if (inDate.Contains(".")) //needed for split style
                {
                    //do nothing everything is already set
                }
                else if (inDate.Contains("-"))
                {
                    parseSplit = '-';
                }
                else if (inDate.Contains("/"))
                {
                    parseSplit = '/';
                }
                string[] strArr = inDate.Split(parseSplit);
                string splitDate = "";
                string Y = "";
                string M = "";
                string D = "";

                if (strArr[2].Length == 4) //discard times for now, doesnt matter - Needed for date formatting
                {
                    //splitDate = strArr[2] + parseSplit + strArr[1] + parseSplit + strArr[0];
                    Y = strArr[2];
                    M = strArr[1];
                    D = strArr[0];
                }
                else if (strArr[0].Length == 4)
                {
                    splitDate = strArr[0] + parseSplit + strArr[1] + parseSplit + strArr[2];
                    Y = strArr[0];
                    M = strArr[1];
                    D = strArr[2];
                }
                inDate = Y + "/" + M + "/" + D;
                DateTime oDate;
                DateTime.TryParse(inDate, out oDate);

                return oDate;//DateTime.ParseExact(inDate + " 00:00:00", parseFormat + " HH:mm:ss", null);
            }
            else return DateTime.Now;
        }
    }
}
