using ACT.Core.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ACT.UI.Models
{
    public class BankDetailViewModel
    {
        #region Properties

        public int Id { get; set; }

        [Required]
        [Display(Name = "Account")]
        [StringLength(50, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 0)]
        public string Account { get; set; }

        [Required]
        [Display(Name = "ObjectType")]
        [StringLength(50, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 0)]
        public string ObjectType { get; set; }
        public int ObjectId { get; set; }

        [Display(Name = "Beneficiary")]
        [StringLength(6, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 0)]
        public string Beneficiary { get; set; }
        public string Branch { get; set; }
        public string Bank { get; set; }
        public int AccountType { get; set; }

        [Required]
        [Display(Name = "Status")]
        public Status Status { get; set; }

        public bool EditMode { get; set; }

        #endregion

        #region Model Options

        #endregion
    }
}