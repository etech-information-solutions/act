using System;
using System.ComponentModel.DataAnnotations;
using ACT.Core.Enums;

namespace ACT.UI.Models
{
    public class BroadcastViewModel
    {
        #region Properties

        public int Id { get; set; }

        [Required]
        [Display( Name = "Start Date" )]
        public DateTime? StartDate { get; set; }

        [Display( Name = "End Date" )]
        public DateTime? EndDate { get; set; }
        
        [Required]
        [Display( Name = "Message" )]
        [StringLength( 500, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 0 )]
        public string Message { get; set; }
        
        [Display( Name = "Type" )]
        public BroadcastType BroadcastType { get; set; }

        [Required]
        [Display( Name = "Status" )]
        public Status Status { get; set; }

        public bool EditMode { get; set; }

        #endregion

        #region Model Options



        #endregion
    }
}