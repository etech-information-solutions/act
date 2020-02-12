using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACT.Core.Helpers
{
    public static class DictionaryHelpers
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="headers"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public static Dictionary<string, string> GetValuesFromDictionary(List<string> headers, List<string> values)
        {
            // @TODO: Aderito. Complete or remove method; might not be necessary anymore
            var dictionary = headers
                .Zip(values, (k, v) => new { Key = k, Value = v })
                .ToDictionary(x => x.Key, x => x.Value);

            return dictionary;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="headers"></param>
        /// <param name="values"></param>
        /// <param name="picks"></param>
        /// <returns></returns>
        private static Dictionary<string, string> GetValuesFromDictionaryAndRemove(List<string> headers, List<string> values, string[] picks)
        {
            // @TODO: Aderito. Complete or remove method; might not be necessary anymore
            var temp = ZipCsvHeadersAndValues(headers, values);
            var dictionary = new Dictionary<string, string>();
            foreach (var pick in picks)
            {

                dictionary.Add(pick, temp[pick]);
            }
            return dictionary;
        }

        /// <summary>
        /// Returns a mapping of header => value for given headers and values.
        /// </summary>
        /// <param name="headers"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public static Dictionary<string, string> ZipCsvHeadersAndValues(List<string> headers, List<string> values)
        {
            var dictionary = headers
                .Zip(values, (k, v) => new { Key = k, Value = v })
                .ToDictionary(x => x.Key, x => x.Value);

            return dictionary;
        }
    }
}
