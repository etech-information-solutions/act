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

        [Display( Name = "Client" )]
        public int ClientId { get; set; }

        [Required]
        [Display( Name = "KPI Description" )]
        [StringLength( 250, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 0 )]
        public string KPIDescription { get; set; }

        [Display( Name = "Disputes" )]
        public Nullable<decimal> Disputes { get; set; }

        [Display( Name = "Outstanding Pallets" )]
        public Nullable<decimal> OutstandingPallets { get; set; }

        [Display( Name = "Passons" )]
        public Nullable<decimal> Passons { get; set; }

        [Display( Name = "Monthly Cost" )]
        public Nullable<decimal> MonthlyCost { get; set; }

        [Display( Name = "Outstanding Days" )]
        public Nullable<int> OutstandingDays { get; set; }

        [Display( Name = "Resolve Days" )]
        public Nullable<int> ResolveDays { get; set; }

        [Display( Name = "Weight" )]
        public Nullable<decimal> Weight { get; set; }

        [Display( Name = "Target Amount" )]
        public Nullable<decimal> TargetAmount { get; set; }

        [Required]
        [Display( Name = "Target Period" )]
        public TargetPeriod TargetPeriod { get; set; }

        [Required]
        [Display( Name = "Status" )]
        public Status Status { get; set; }

        public bool EditMode { get; set; }

        #endregion


        #region Model Options

        public Dictionary<int, string> ClientOptions
        {
            get
            {
                using ( ClientService cservice = new ClientService() )
                {
                    return cservice.List( true );
                }
            }
        }

        #endregion
    }
}