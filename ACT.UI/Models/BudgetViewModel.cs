using System;
using System.ComponentModel.DataAnnotations;
using ACT.Core.Enums;

namespace ACT.UI.Models
{
    public class BudgetViewModel
    {
        #region Properties

        public int Id { get; set; }
        public int ObjectId { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime ModifiedOn { get; set; }
        public string ModifiedBy { get; set; }
        public int? BudgetYear { get; set; }
        public decimal? January { get; set; }
        public decimal? February { get; set; }
        public decimal? March { get; set; }
        public decimal? April { get; set; }
        public decimal? May { get; set; }
        public decimal? June { get; set; }
        public decimal? July { get; set; }
        public decimal? August { get; set; }
        public decimal? September { get; set; }
        public decimal? October { get; set; }
        public decimal? November { get; set; }
        public decimal? December { get; set; }
        public int Status { get; set; }

        #endregion

        #region Model Options



        #endregion
    }
}