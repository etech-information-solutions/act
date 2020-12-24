using System;
using System.ComponentModel.DataAnnotations;
using ACT.Core.Enums;
using System.Web;
using System.Collections.Generic;
using ACT.Core.Services;
using ACT.Data.Models;

namespace ACT.UI.Models
{
    public class JournalViewModel
    {
        #region Properties

        public int Id { get; set; }

        [Display( Name = "Client Load" )]
        public int? ClientLoadId { get; set; }

        [Required]
        [Display( Name = "Posting Description" )]
        [StringLength( 50, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 0 )]
        public string PostingDescription { get; set; }

        [Required]
        [Display( Name = "Posting Quantity" )]
        public decimal? PostingQuantity { get; set; }
        
        [Display( Name = "In/Out?" )]
        public bool? InOutInd { get; set; }
        
        [Display( Name = "THAN" )]
        [StringLength( 50, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 0 )]
        public string THAN { get; set; }

        [Display( Name = "Status" )]
        public Status Status { get; set; }

        [Display( Name = "JournalType" )]
        public JournalType JournalType { get; set; }

        public bool EditMode { get; set; }

        [Display( Name = "File" )]
        public HttpPostedFileBase File { get; set; }

        public List<Document> Documents { get; set; }

        #endregion

        #region Model Options

        

        #endregion
    }
}