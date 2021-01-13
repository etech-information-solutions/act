using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

using ACT.Core.Enums;
using ACT.Core.Services;
using ACT.Data.Models;

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
        [Display( Name = "Docket #" )]
        [StringLength( 50, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 0 )]
        public string DocketNumber { get; set; }

        [Display( Name = "Original Docket #" )]
        [StringLength( 50, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 0 )]
        public string OriginalDocketNumber { get; set; }

        [Required]
        [Display( Name = "Dispute Reason" )]
        [StringLength( 500, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 0 )]
        public string DisputeReason { get; set; }

        [Display( Name = "Dispute Email" )]
        [StringLength( 150, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 0 )]
        public string DisputeEmail { get; set; }

        [Display( Name = "TDN #" )]
        [StringLength( 50, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 0 )]
        public string TDNNumber { get; set; }

        [Display( Name = "Reference #" )]
        [StringLength( 50, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 0 )]
        public string Reference { get; set; }

        [Display( Name = "Other Reference #" )]
        [StringLength( 50, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 0 )]
        public string OtherReference { get; set; }

        [Display( Name = "Equipment" )]
        [StringLength( 50, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 0 )]
        public string Equipment { get; set; }

        [Display( Name = "Equipment Code" )]
        [StringLength( 50, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 0 )]
        public string EquipmentCode { get; set; }

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

        //[Display( Name = "Product" )]
        //[StringLength( 50, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 0 )]
        //public string Product { get; set; }

        [Display( Name = "Quantity" )]
        public int Quantity { get; set; }

        [Required]
        [Display( Name = "Status" )]
        public DisputeStatus Status { get; set; }

        [Display( Name = "Location" )]
        [StringLength( 50, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 0 )]
        public string Location { get; set; }

        [Display( Name = "Location ID" )]
        [StringLength( 50, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 0 )]
        public string LocationId { get; set; }

        [Display( Name = "Action" )]
        [StringLength( 50, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 0 )]
        public string Action { get; set; }

        [Display( Name = "ActionBy" )]
        [StringLength( 150, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 0 )]
        public string ActionBy { get; set; }

        [Display( Name = "Effective Date" )]
        public DateTime? EffectiveDate { get; set; }

        [Display( Name = "Ship Date" )]
        public DateTime? ShipDate { get; set; }

        [Display( Name = "Delilvery Date" )]
        public DateTime? DelilveryDate { get; set; }

        [Display( Name = "Days Left" )]
        public int? DaysLeft { get; set; }

        [Display( Name = "Correction Request Date" )]
        public DateTime? CorrectionRequestDate { get; set; }

        [Display( Name = "Correction Request #" )]
        [StringLength( 50, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 0 )]
        public string CorrectionRequestNumber { get; set; }

        [Display( Name = "Transaction Type" )]
        [StringLength( 50, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 0 )]
        public string TransactionType { get; set; }

        [Display( Name = "Dispute Comment" )]
        [StringLength( 1000, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 0 )]
        public string DisputeComment { get; set; }

        [Display( Name = "Data Source" )]
        [StringLength( 50, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 0 )]
        public string DataSource { get; set; }

        [Display( Name = "Import Loads" )]
        public HttpPostedFileBase File { get; set; }

        public List<Comment> ChepLoadComments { get; set; }

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
