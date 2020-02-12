using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

namespace System {
    public static class DoubleExtensions {

        public static decimal ToDecimal(this double dbl) {
            return Convert.ToDecimal(dbl);
        }

        public static double Power(this double dbl, double power) {
            return Math.Pow(dbl, power);
        }

        public static int Round(this double dbl) {
            return Convert.ToInt32(dbl.Round(0));
        }

        public static int RoundUp(this double dbl) {
            int rnd = dbl.Round();
            if ( rnd < dbl) {
                return rnd + 1;
            }
            return rnd;
        }

        public static double Round(this double dbl, int decimals) {
            return Math.Round(dbl, decimals, MidpointRounding.AwayFromZero);
        }

        public static string ToString(this double dbl, int decimals) {
            return dbl.ToString(decimals, CultureInfo.InvariantCulture);
        }


        public static string ToString(this double dbl, int decimals, IFormatProvider provider) {

            if (decimals < 0) {
                return dbl.ToString(provider);
            }

            double rounded = dbl.Round(decimals);

            string fmt = "{0:0.";
            for (int i = 0; i < decimals; i++) {
                fmt += "0";
            }
            fmt += "}";

           
            return string.Format(provider, fmt, rounded);

        }



    }
}
