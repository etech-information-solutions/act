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
        public System.DateTime CreatedOn { get; set; }
        public System.DateTime ModifiedOn { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<int> BudgetYear { get; set; }
        public Nullable<decimal> January { get; set; }
        public Nullable<decimal> February { get; set; }
        public Nullable<decimal> March { get; set; }
        public Nullable<decimal> April { get; set; }
        public Nullable<decimal> May { get; set; }
        public Nullable<decimal> June { get; set; }
        public Nullable<decimal> July { get; set; }
        public Nullable<decimal> August { get; set; }
        public Nullable<decimal> September { get; set; }
        public Nullable<decimal> October { get; set; }
        public Nullable<decimal> November { get; set; }
        public Nullable<decimal> December { get; set; }
        public int Status { get; set; }

        #endregion

        #region Model Options



        #endregion
    }
}