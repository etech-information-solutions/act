using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using ACT.Core.Enums;
using ACT.Core.Services;

namespace ACT.UI.Models
{
    public class AuthorisationCodeViewModel
    {
        #region Properties

        public int Id { get; set; }

        public int ClientLoadId { get; set; }

        [Display( Name = "Code" )]
        [StringLength( 50, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 0 )]
        public string Code { get; set; }

        //[Required]
        [Display( Name = "Authorisation Date" )]
        public DateTime? AuthorisationDate { get; set; }

        //[Required]
        [Display( Name = "Load Number" )]
        [StringLength( 50, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 0 )]
        public string LoadNumber { get; set; }

        //[Required]
        [Display( Name = "Status" )]
        public Status Status { get; set; }

        [Display( Name = "Comment" )]
        [StringLength( 250, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 0 )]
        public string Comment { get; set; }

        public bool EditMode { get; set; }

        #endregion



        #region Model Options

        #endregion
    }
}