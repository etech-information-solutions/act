using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

using ACT.Core.Enums;
using ACT.Core.Services;

namespace ACT.UI.Models
{
    public class DisputeViewModel
    {
        #region Properties

        public int Id { get; set; }

        [Display( Name = "Chep Load" )]
        public int? ChepLoadId { get; set; }

        [Required]
        [Display( Name = "Action By" )]
        public int? ActionById { get; set; }

        [Display( Name = "Resolved By" )]
        public int? ResolvedById { get; set; }

        [Display( Name = "Resolved On" )]
        public DateTime? ResolvedOn { get; set; }

        [Required]
        [Display( Name = "Docket Number" )]
        [StringLength( 50, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 0 )]
        public string DocketNumber { get; set; }

        [Required]
        [Display( Name = "Dispute Reason" )]
        [StringLength( 500, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 0 )]
        public string DisputeReason { get; set; }

        [Display( Name = "Dispute Email" )]
        [StringLength( 150, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 0 )]
        public string DisputeEmail { get; set; }

        [Display( Name = "TDN Number" )]
        [StringLength( 50, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 0 )]
        public string TDNNumber { get; set; }

        [Display( Name = "Equipment" )]
        [StringLength( 10, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 0 )]
        public string Equipment { get; set; }

        [Display( Name = "Other Party" )]
        [StringLength( 200, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 0 )]
        public string OtherParty { get; set; }

        [Display( Name = "Sender" )]
        [StringLength( 50, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 0 )]
        public string Sender { get; set; }

        [Display( Name = "Receiver" )]
        [StringLength( 50, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 0 )]
        public string Receiver { get; set; }

        [Display( Name = "Declarer" )]
        [StringLength( 50, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 0 )]
        public string Declarer { get; set; }

        [Display( Name = "Product" )]
        [StringLength( 50, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 0 )]
        public string Product { get; set; }

        [Display( Name = "Quantity" )]
        public int Quantity { get; set; }

        [Required]
        [Display( Name = "Status" )]
        public DisputeStatus Status { get; set; }

        public bool EditMode { get; set; }

        #endregion



        #region Model Options

        public Dictionary<int, string> ChepLoadOptions
        {
            get
            {
                using ( ChepLoadService clservice = new ChepLoadService() )
                {
                    return clservice.List( true );
                }
            }
        }

        public Dictionary<int, string> UserOptions
        {
            get
            {
                using ( UserService uservice = new UserService() )
                {
                    return uservice.List( true, RoleType.Client );
                }
            }
        }

        #endregion
    }
}
