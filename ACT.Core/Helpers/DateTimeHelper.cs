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
    }
}
