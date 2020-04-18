using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace ACT.UI.Models
{
    public class ChepLoadViewModel
    {
        #region Properties

        public int Id { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public System.DateTime ModifiedOn { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> LoadDate { get; set; }
        public Nullable<System.DateTime> NotifyDate { get; set; }
        public Nullable<System.DateTime> EffectiveDate { get; set; }
        public Nullable<int> PostingType { get; set; }
        public string AccountNumber { get; set; }
        public string ClientDescription { get; set; }
        public string DeliveryNote { get; set; }
        public string ReferenceNumber { get; set; }
        public string ReceiverNumber { get; set; }
        public string Equipment { get; set; }
        public Nullable<decimal> OriginalQuantity { get; set; }
        public Nullable<decimal> NewQuantity { get; set; }
        public string DocketNumber { get; set; }
        public int Status { get; set; }
        [Display(Name = "Load Files")]
        public ICollection<FileViewModel> Documents { get; set; }
        public int? DocumentCount { get; set; }
        //public List<Document> Documents { get; set; }

        public bool EditMode { get; set; }
        public bool ContextualMode { get; set; }
        public string DocsList { get; set; }

        #endregion


        #region Model Options


        #endregion
    }
}