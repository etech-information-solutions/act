using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ACT.Core.Enums;
using ACT.Core.Services;
using ACT.Data.Models;
namespace ACT.UI.Models
{
    public class ClientKPIViewModel
    {
        #region Properties

        public int Id { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public System.DateTime ModifiedOn { get; set; }
        public string ModifiedBy { get; set; }
        public int ClientId { get; set; }
        [Display(Name = "KPI Description")]
        [StringLength(250, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 0)]
        public string KPIDescription { get; set; }
        public string ClientName { get; set; }
        public string TradingAs { get; set; }
        public Nullable<decimal> Disputes { get; set; }
        public Nullable<decimal> OutstandingPallets { get; set; }
        public Nullable<decimal> Passons { get; set; }
        public Nullable<decimal> MonthlyCost { get; set; }
        public Nullable<int> OutstandingDays { get; set; }
        public Nullable<int> ResolveDays { get; set; }
        public int Status { get; set; }
        [Range(0, 100)]
        public Nullable<decimal> Weight { get; set; }
        public Nullable<decimal> TargetAmount { get; set; }
        public Nullable<int> TargetPeriod { get; set; }

        public bool EditMode { get; set; }
        public bool ContextualMode { get; set; }

        #endregion


        #region Model Options


        #endregion
    }
}