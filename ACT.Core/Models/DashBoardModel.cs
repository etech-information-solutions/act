using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACT.Core.Models
{
    public class DashBoardModel
    {
    }

    public class AgeOfOutstandingPallets
    {
        public int DurationNumber { get; set; }

        public string DurationName { get; set; }

        public decimal? Oustanding { get; set; }
    }

    public class LoadsPerMonth
    {
        public int MonthNumber { get; set; }

        public string MonthName { get; set; }

        public int Loads { get; set; }

        public int POD { get; set; }

        public decimal? Quantity { get; set; }
    }

    public class AuthorisationCodes
    {
        public int MonthNumber { get; set; }

        public string MonthName { get; set; }

        public int Loads { get; set; }

        public int Codes { get; set; }
    }

    public class NumberOfPalletsManaged
    {
        public int MonthNumber { get; set; }

        public string MonthName { get; set; }

        public decimal? Unreconciled { get; set; }

        public decimal? Reconciled { get; set; }

        public decimal? Total { get; set; }
    }

    public class NumberOfDisputes
    {
        public int MonthNumber { get; set; }

        public string MonthName { get; set; }

        public int Total { get; set; }
    }
}
