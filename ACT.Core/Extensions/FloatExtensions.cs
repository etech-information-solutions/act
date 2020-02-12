using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

namespace System {
    public static class FloatExtensions {

        public static decimal ToDecimal(this float flt) {
            return Convert.ToDecimal(flt);
        }

        public static float Power(this float flt, float power) {
            return Convert.ToSingle(Math.Pow(Convert.ToDouble(flt), power));
        }

        public static int Round(this float flt) {
            return Convert.ToInt32(flt.Round(0));
        }

        public static float Round(this float flt, int decimals) {
            return Convert.ToSingle(Math.Round(Convert.ToDouble(flt), decimals, MidpointRounding.AwayFromZero));
        }

        public static string ToString(this float flt, int decimals) {
            //NB: Serializing floats with decimal comma (",") breaks high charts
            return flt.ToString(decimals, CultureInfo.GetCultureInfo("en-US"));
        }


        public static string ToString(this float flt, int decimals, IFormatProvider provider) {

            if (decimals < 0) {
                return flt.ToString(provider);
            }

            float rounded = flt.Round(decimals);

            string fmt = "{0:0.";
            for (int i = 0; i < decimals; i++) {
                fmt += "0";
            }
            fmt += "}";


            return string.Format(provider, fmt, rounded);

        }

    }
}
