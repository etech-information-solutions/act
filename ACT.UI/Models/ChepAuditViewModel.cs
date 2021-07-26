using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ACT.Core.Enums;
using ACT.Core.Services;
using ACT.Data.Models;

namespace ACT.UI.Models
{
    public class ChepAuditViewModel
    {
        #region Properties

        public int Id { get; set; }

        [Required]
        [Display( Name = "Site" )]
        public int SiteId { get; set; }

        [Required]
        [Display( Name = "Equipment" )]
        [StringLength( 50, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 1 )]
        public string Equipment { get; set; }

        [Required]
        [Display( Name = "Chep Stock Balance" )]
        public decimal? ChepStockBalance { get; set; }

        [Display( Name = "Invoice In" )]
        public decimal? NotInvoiceIn { get; set; }

        [Display( Name = "Invoice Out" )]
        public decimal? NotInvoiceOut { get; set; }

        [Display( Name = "Req Adjustment" )]
        public decimal? ReqAdjustment { get; set; }

        [Display( Name = "MCC In" )]
        public decimal? NotMCCIn { get; set; }

        [Display( Name = "MCC Out" )]
        public decimal? NotMCCOut { get; set; }

        [Display( Name = "Suspend ITL" )]
        public decimal? SuspendITL { get; set; }

        [Display( Name = "Suspend MCC" )]
        public decimal? SuspendMCC { get; set; }

        [Display( Name = "Adjusted Invoice Balance" )]
        public decimal? AdjustedInvBalance { get; set; }

        [Display( Name = "MCC Balance" )]
        public decimal? MccBalance { get; set; }

        [Display( Name = "Audit Report File" )]
        public FileViewModel AuditReportFile { get; set; }

        public bool EditMode { get; set; }

        #endregion

        #region Model Options

        public Dictionary<int, string> SiteOptions
        {
            get
            {
                if ( !EditMode ) return new Dictionary<int, string>();

                using ( SiteService sservice = new SiteService() )
                {
                    return sservice.List( true );
                }
            }
        }

        #endregion
    }
}
